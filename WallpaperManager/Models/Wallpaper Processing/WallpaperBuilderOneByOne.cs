// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Builds a wallpaper image and implements a special behaviour when building Multiscreen Wallpapers by using
  ///   <see cref="CreateMultiscreenFromMultiple" />.
  /// </summary>
  /// <inheritdoc select='seealso' />
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperBuilderOneByOne : WallpaperBuilderBase {
    /// <inheritdoc />
    public override ReadOnlyCollection<int> RequiredWallpapersByScreen {
      get {
        var requiredWallpapersByScreen = new int[this.ScreensSettings.Count];

        // Check whether we can use the last layout for the next cycle.
        if (this.LastScreenLayout.Count == this.ScreensSettings.Count) {
          // We start searching for a screen which requires a random wallpaper from the last screen cycled + 1.
          byte currentScreenIndex = (byte)(this.LastChangedScreenIndex + 1);

          // Find a screen which requests a random Wallpaper.
          for (int i = 0; i < this.ScreensSettings.Count; i++) {
            // Did we reach the last screen?
            if (currentScreenIndex >= this.ScreensSettings.Count)
              currentScreenIndex = 0;

            // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
            // cycle conditions don't match.
            if (
              this.ScreensSettings[currentScreenIndex].CycleRandomly ||
              !WallpaperChanger.EvaluateCycleConditions(this.ScreensSettings[currentScreenIndex].StaticWallpaper)) {
              requiredWallpapersByScreen[currentScreenIndex] = 1;
              break;
            }

            currentScreenIndex++;
          }
        } else {
          for (int i = 0; i < this.ScreensSettings.Count; i++) {
            // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
            // cycle conditions don't match.
            if ((this.ScreensSettings[i].CycleRandomly) || (!WallpaperChanger.EvaluateCycleConditions(this.ScreensSettings[i].StaticWallpaper)))
              requiredWallpapersByScreen[i] = 1;
          }
        }

        return new ReadOnlyCollection<int>(requiredWallpapersByScreen);
      }
    }

    /// <summary>
    ///   Gets or sets the zero-based index of the last screen changed.
    /// </summary>
    /// <value>
    ///   The zero-based index of the last screen changed.
    /// </value>
    protected byte LastChangedScreenIndex { get; set; }

    /// <summary>
    ///   Gets the <see cref="Wallpaper" /> instances used for the last build.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="Wallpaper" /> instances used for the last build.
    /// </value>
    protected List<IWallpaper> LastScreenLayout { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBuilderOneByOne" /> class.
    /// </summary>
    /// <inheritdoc />
    public WallpaperBuilderOneByOne(ScreenSettingsCollection screensSettings) : base(screensSettings) {
      this.LastScreenLayout = new List<IWallpaper>(Screen.AllScreens.Length);
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.LastChangedScreenIndex.IsBetween(0, (byte)(this.ScreensSettings.Count - 1)));
      Contract.Invariant(this.LastScreenLayout != null);
    }

    /// <summary>
    ///   Creates a multiscreen wallpaper from one or multiple <see cref="Wallpaper" /> objects where one
    ///   <see cref="Wallpaper" /> is required for each screen on the first call and exactly one <see cref="Wallpaper" /> is
    ///   required on further calls.
    /// </summary>
    /// <inheritdoc />
    public override Image CreateMultiscreenFromMultiple(IList<IList<IWallpaper>> wallpapers, float scaleFactor, bool useWindowsFix) {
      // This is the collection of Wallpapers which is finally given to the generator method.
      // Note that the order of the Wallpapers in this collection has to be equal with the screen order.
      List<IWallpaper> usedWallpapers = this.LastScreenLayout;

      // Check if we can use the last layout and if so, change just one of the Wallpapers in it.
      if ((this.LastScreenLayout.Count == this.ScreensSettings.Count) && (!this.ScreensSettings.AllStatic)) {
        // We start searching for a screen which requires a random wallpaper from the last screen cycled + 1.
        byte currentScreenIndex = (byte)(this.LastChangedScreenIndex + 1);

        // Find a screen which requests a random Wallpaper.
        for (int i = 0; i < this.ScreensSettings.Count; i++) {
          // Did we reach the last screen?
          if (currentScreenIndex >= this.ScreensSettings.Count)
            currentScreenIndex = 0;

          // We want to use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
          // cycle conditions don't match.
          if (this.ScreensSettings[currentScreenIndex].CycleRandomly || !WallpaperChanger.EvaluateCycleConditions(this.ScreensSettings[currentScreenIndex].StaticWallpaper))
            break;

          currentScreenIndex++;
        }

        // Change the screen which requires a random Wallpaper in the layout.
        this.LastScreenLayout[currentScreenIndex] = wallpapers[currentScreenIndex][0];
        this.LastChangedScreenIndex = currentScreenIndex;

        // Regenerate the changed screen layout.
        Image wallpaper = this.CreateMultiscreenFromMultipleInternal(this.LastScreenLayout, scaleFactor, useWindowsFix);

        return wallpaper;
      }

      // The old layout is not useable or maybe this is the first use of this builder, so we need to generate a new layout.
      this.LastScreenLayout.Clear();

      // Loop through all screen settings and use the defined static wallpaper if necessary.
      for (int i = 0; i < this.ScreensSettings.Count; i++) {
        // We use a random Wallpaper if random cycling is requested or if a Static Wallpaper should be used but its
        // cycle conditions don't match.
        if ((this.ScreensSettings[i].CycleRandomly) || (!WallpaperChanger.EvaluateCycleConditions(this.ScreensSettings[i].StaticWallpaper)))
          usedWallpapers.Add(wallpapers[i][0]);
        else
          usedWallpapers.Add(this.ScreensSettings[i].StaticWallpaper);
      }

      // By setting the last index to the last screen, we make sure that the next call of this method will change the wallpaper 
      // of the first screen.
      this.LastChangedScreenIndex = (byte)(this.ScreensSettings.Count - 1);

      // Generate the wallpaper.
      return this.CreateMultiscreenFromMultipleInternal(usedWallpapers, scaleFactor, useWindowsFix);
    }
  }
}