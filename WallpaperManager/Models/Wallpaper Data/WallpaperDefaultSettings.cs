// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains data to be applied to newly added wallpapers.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ImplementPropertyChanged]
  public class WallpaperDefaultSettings : IWallpaperDefaultSettings, ICloneable, IAssignable {
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