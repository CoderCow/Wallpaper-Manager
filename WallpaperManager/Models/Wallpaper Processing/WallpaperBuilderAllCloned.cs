// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Builds a wallpaper image and implements a special behaviour when building Multiscreen Wallpapers by using
  ///   <see cref="CreateMultiscreenFromMultiple" />.
  /// </summary>
  /// <inheritdoc select='seealso' />
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperBuilderAllCloned : WallpaperBuilderBase {
    /// <inheritdoc />
    public override ReadOnlyCollection<int> RequiredWallpapersByScreen {
      get {
        var requiredWallpapersByScreen = new int[this.ScreensSettings.Count];

        for (int i = 0; i < this.ScreensSettings.Count; i++) {
          // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
          // cycle conditions don't match.
          if ((this.ScreensSettings[i].CycleRandomly) || (!this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions())) {
            requiredWallpapersByScreen[i] = 1;
            break;
          }
        }

        return new ReadOnlyCollection<int>(requiredWallpapersByScreen);
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBuilderAllCloned" /> class.
    /// </summary>
    /// <inheritdoc />
    public WallpaperBuilderAllCloned(ScreenSettingsCollection screensSettings) : base(screensSettings) {}

    /// <summary>
    ///   Creates a multiscreen wallpaper from one <see cref="Wallpaper" /> object which is drawn for each screen.
    /// </summary>
    /// <inheritdoc />
    public override Image CreateMultiscreenFromMultiple(IList<IList<IWallpaper>> wallpapers, float scaleFactor, bool useWindowsFix) {
      // This is the collection of Wallpapers which is finally assigned to the generator method.
      // Note that the order of the Wallpapers in this collection has to be equal with the screen order.
      // Since we want to clone the Wallpapers, this collection will probably contain the same Wallpaper object multiple times.
      var usedWallpapers = new List<IWallpaper>(this.ScreensSettings.Count);

      // This is the screen index where the wallpaper is picked for which should be cloned over this and the other screens.
      int cloneSourceScreenIndex = 0;
      for (int i = 0; i < this.ScreensSettings.Count; i++) {
        // We want to use a random Wallpaper if random cycling is requested or if a static wallpaper should be used but its
        // cycle conditions don't match.
        if ((this.ScreensSettings[i].CycleRandomly) || (!this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions())) {
          cloneSourceScreenIndex = i;
          break;
        }
      }

      // Loop through all screen settings and use the defined static wallpaper if necessary.
      foreach (ScreenSettings screenSetting in this.ScreensSettings) {
        // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
        // cycle conditions don't match.
        if ((screenSetting.CycleRandomly) || (!screenSetting.StaticWallpaper.EvaluateCycleConditions())) {
          // A random Wallpaper is requested for this screen.
          // Note that we always add the first random wallpaper here since it should be cloned on all screens.
          usedWallpapers.Add(wallpapers[cloneSourceScreenIndex][0]);
        } else {
          // The Static Wallpaper should be used for this screen.
          usedWallpapers.Add(screenSetting.StaticWallpaper);
        }
      }

      // Generate the wallpaper.
      return this.CreateMultiscreenFromMultipleInternal(usedWallpapers, scaleFactor, useWindowsFix);
    }
  }
}