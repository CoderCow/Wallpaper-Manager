// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains data to be applied to newly added wallpapers.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ImplementPropertyChanged]
  public class WallpaperDefaultSettings : IWallpaperDefaultSettings, ICloneable, IAssignable {
    private static Size MaxImageSizeSuitableForTiledWallpaper = new Size(640, 480);
    /// <summary>
    ///   The factor by which a wallpaper's width must be larger than the primary screen to be 
    ///   automatically considered a multiscreen wallpaper.
    /// </summary>
    private const float MultiscreenWidthOversizeFactor = 0.5f;

    /// <inheritdoc />  
    public WallpaperBase Settings { get; set; }

    /// <inheritdoc />
    public bool AutoDetermineIsMultiscreen { get; set; }

    /// <inheritdoc />
    public bool AutoDeterminePlacement { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperDefaultSettings" /> class.
    /// </summary>
    public WallpaperDefaultSettings(WallpaperBase baseSettings) {
      this.Settings = baseSettings;
      this.AutoDetermineIsMultiscreen = true;
      this.AutoDeterminePlacement = true;
    }

    /// <summary>
    ///   Automatically suggests <see cref="WallpaperBase.IsMultiscreen" /> and <see cref="WallpaperBase.Placement" /> for the given <paramref name="target" /> 
    ///   according to <see cref="AutoDetermineIsMultiscreen" /> and <see cref="AutoDeterminePlacement" />.
    /// </summary>
    /// <param name="target">
    ///   The <see cref="Wallpaper" /> to apply the settings to.
    /// </param>
    public void ApplyToWallpaper(Wallpaper target) {
      this.Settings.AssignTo(target);

      if (this.AutoDeterminePlacement) {
        // If the wallpaper is pretty small, we guess that it will maybe used 
        // in "Tile" mode, otherwise we recommend "StretchWithRatio" mode.
        if ((target.ImageSize.Width <= MaxImageSizeSuitableForTiledWallpaper.Width) || (target.ImageSize.Height <= MaxImageSizeSuitableForTiledWallpaper.Height))
          target.Placement = WallpaperPlacement.Tile;
        else
          target.Placement = WallpaperPlacement.Uniform;
      }

      bool isMultiscreenSystem = Screen.AllScreens.Length > 1;
      if (!isMultiscreenSystem) {
        target.IsMultiscreen = false;
      } else {
        if (this.AutoDetermineIsMultiscreen) {
          Rectangle primaryScreenBounds = Screen.PrimaryScreen.Bounds;

          if (target.ImageSize.Width > (primaryScreenBounds.Width * (MultiscreenWidthOversizeFactor + 1f))) {
            target.IsMultiscreen = true;
            target.Placement = WallpaperPlacement.UniformToFill;
          }
        }
      }
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public object Clone() {
      return this.MemberwiseClone();
    }

    /// <inheritdoc />
    public void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is WallpaperDefaultSettings);

      WallpaperDefaultSettings defaultSettingsInstance = (WallpaperDefaultSettings)other;
      defaultSettingsInstance.Settings = this.Settings;
      defaultSettingsInstance.AutoDetermineIsMultiscreen = this.AutoDetermineIsMultiscreen;
      defaultSettingsInstance.AutoDeterminePlacement = this.AutoDeterminePlacement;
    }
    #endregion
  }
}