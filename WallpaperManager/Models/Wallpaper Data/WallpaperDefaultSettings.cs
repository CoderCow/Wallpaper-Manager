// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;
using Common.Windows;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains data to be applied to newly added wallpapers.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ImplementPropertyChanged]
  public class WallpaperDefaultSettings : IWallpaperDefaultSettings, ICloneable, IAssignable {
    public static Size MaxImageSizeSuitableForTiledWallpaper = new Size(640, 480);
    /// <summary>
    ///   The factor by which a wallpaper's width must be larger than the primary screen to be 
    ///   automatically considered a multiscreen wallpaper.
    /// </summary>
    public const float MultiscreenWidthOversizeFactor = 0.5f;

    private readonly IDisplayInfo displayInfo;

    /// <inheritdoc />  
    public IWallpaperBase Settings { get; set; }

    /// <inheritdoc />
    public bool AutoDetermineIsMultiscreen { get; set; }

    /// <inheritdoc />
    public bool AutoDeterminePlacement { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperDefaultSettings" /> class.
    /// </summary>
    public WallpaperDefaultSettings(IWallpaperBase baseSettings, IDisplayInfo displayInfo) {
      Contract.Requires<ArgumentNullException>(baseSettings != null);
      Contract.Requires<ArgumentNullException>(displayInfo != null);

      this.Settings = baseSettings;
      this.displayInfo = displayInfo;
      this.AutoDetermineIsMultiscreen = true;
      this.AutoDeterminePlacement = true;
    }

    /// <summary>
    ///   Automatically suggests <see cref="IWallpaperBase.IsMultiscreen" /> and <see cref="IWallpaperBase.Placement" /> for the given <paramref name="target" /> 
    ///   according to <see cref="AutoDetermineIsMultiscreen" /> and <see cref="AutoDeterminePlacement" />.
    /// </summary>
    /// <param name="target">
    ///   The <see cref="IWallpaper" /> to apply the settings to.
    /// </param>
    public void ApplyToWallpaper(IWallpaper target) {
      this.Settings.AssignTo(target);

      if (this.AutoDeterminePlacement) {
        // If the wallpaper is pretty small, we guess that it will maybe used 
        // in "Tile" mode, otherwise we recommend "StretchWithRatio" mode.
        if ((target.ImageSize.Value.Width <= MaxImageSizeSuitableForTiledWallpaper.Width) || (target.ImageSize.Value.Height <= MaxImageSizeSuitableForTiledWallpaper.Height))
          target.Placement = WallpaperPlacement.Tile;
        else
          target.Placement = WallpaperPlacement.Uniform;
      }

      if (this.AutoDetermineIsMultiscreen) {
        Rectangle primaryDisplayBounds = this.displayInfo.PrimaryDisplay.Bounds;

        target.IsMultiscreen = this.displayInfo.IsMultiDisplaySystem && (target.ImageSize.Value.Width > (primaryDisplayBounds.Width * (MultiscreenWidthOversizeFactor + 1f)));
        if (this.AutoDeterminePlacement && target.IsMultiscreen)
          target.Placement = WallpaperPlacement.UniformToFill;
      }
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public object Clone() {
      var clone = (WallpaperDefaultSettings)this.MemberwiseClone();
      clone.Settings = (IWallpaperBase)this.Settings.Clone();

      return clone;
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