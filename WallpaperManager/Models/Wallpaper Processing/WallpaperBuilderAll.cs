// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Builds a wallpaper image and implements a special behaviour when building Multiscreen Wallpapers by using
  ///   <see cref="CreateMultiscreenFromMultiple" />.
  /// </summary>
  /// <inheritdoc select='seealso' />
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperBuilderAll : WallpaperBuilderBase {
    /// <inheritdoc />
    public override ReadOnlyCollection<int> RequiredWallpapersByScreen {
      get {
        var requiredWallpapersByScreen = new int[this.ScreensSettings.Count];

        for (int i = 0; i < this.ScreensSettings.Count; i++) {
          // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
          // cycle conditions don't match.
          if ((this.ScreensSettings[i].CycleRandomly) || (!this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions()))
            requiredWallpapersByScreen[i] = 1;
        }

        return new ReadOnlyCollection<int>(requiredWallpapersByScreen);
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBuilderAll" /> class.
    /// </summary>
    /// <inheritdoc />
    public WallpaperBuilderAll(ScreenSettingsCollection screensSettings) : base(screensSettings) {}

    /// <summary>
    ///   Creates a multiscreen wallpaper from multiple <see cref="Wallpaper" /> objects (from multiple images), where each
    ///   <see cref="Wallpaper" /> in the given collection is drawn in the same order as the screen indexes for each screen.
    /// </summary>
    /// <inheritdoc />
    public override Image CreateMultiscreenFromMultiple(IList<IList<IWallpaper>> wallpapers, float scaleFactor, bool useWindowsFix) {
      // This is the collection of Wallpapers which is finally given to the generator method.
      // Note that the order of the Wallpapers in this collection has to be equal with the screen order.
      var usedWallpapers = new List<IWallpaper>(this.ScreensSettings.Count);

      // Loop through all screen settings and use the defined static wallpaper if necessary.
      for (int i = 0; i < this.ScreensSettings.Count; i++) {
        // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
        // cycle conditions don't match.
        if ((this.ScreensSettings[i].CycleRandomly) || (!this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions())) {
          // A random Wallpaper is requested for this screen.
          usedWallpapers.Add(wallpapers[i][0]);
        } else {
          // The Static Wallpaper should be used for this screen.
          usedWallpapers.Add(this.ScreensSettings[i].StaticWallpaper);
        }
      }

      // Generate the wallpaper.
      return this.CreateMultiscreenFromMultipleInternal(usedWallpapers, scaleFactor, useWindowsFix);
    }
  }
}