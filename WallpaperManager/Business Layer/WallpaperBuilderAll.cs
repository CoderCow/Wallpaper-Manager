// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using WallpaperManager.Data;

namespace WallpaperManager.Business {
  /// <summary>
  ///   Builds a wallpaper image and implements a special behaviour when building Multiscreen Wallpapers by using 
  ///   <see cref="CreateMultiscreenFromMultiple" />.
  /// </summary>
  /// <inheritdoc select='seealso' />
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperBuilderAll: WallpaperBuilderBase {
    #region Property: RequiredWallpapersByScreen
    /// <inheritdoc />
    public override ReadOnlyCollection<Int32> RequiredWallpapersByScreen {
      get {
        Int32[] requiredWallpapersByScreen = new Int32[this.ScreensSettings.Count];

        for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
          // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
          // cycle conditions don't match.
          if ((this.ScreensSettings[i].CycleRandomly) || (!this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions())) {
            requiredWallpapersByScreen[i] = 1;
          }
        }

        return new ReadOnlyCollection<Int32>(requiredWallpapersByScreen);
      }
    }
    #endregion

    #region Methods: Constructor, CreateMultiscreenFromMultiple
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBuilderAll"/> class.
    /// </summary>
    /// <inheritdoc />
    public WallpaperBuilderAll(ScreenSettingsCollection screensSettings): base(screensSettings) {}

    /// <summary>
    ///   Creates a multiscreen wallpaper from multiple <see cref="Wallpaper" /> objects (from multiple images), where each 
    ///   <see cref="Wallpaper" /> in the given collection is drawn in the same order as the screen indexes for each screen.
    /// </summary>
    /// <inheritdoc />
    public override Image CreateMultiscreenFromMultiple(
      IList<IList<Wallpaper>> wallpapers, Single scaleFactor, Boolean useWindowsFix
    ) {
      if (wallpapers == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapers"));
      }
      if (wallpapers.Count == 0) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCollectionIsEmpty("wallpapers"));
      }
      if (wallpapers.Contains(null)) {
        throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapers"));
      }

      // This is the collection of Wallpapers which is finally given to the generator method.
      // Note that the order of the Wallpapers in this collection has to be equal with the screen order.
      List<Wallpaper> usedWallpapers = new List<Wallpaper>(this.ScreensSettings.Count);

      // Loop through all screen settings and use the defined static wallpaper if necessary.
      for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
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
    #endregion
  }
}
