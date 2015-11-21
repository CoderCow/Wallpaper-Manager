// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines data to be applied to newly added wallpapers.
  /// </summary>
  [ContractClass(typeof(IWallpaperDefaultSettingsContracts))]
  public interface IWallpaperDefaultSettings : ICloneable, IAssignable {
    IWallpaperBase Settings { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="IWallpaperBase.IsMultiscreen" /> property should be
    ///   determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="IWallpaperBase.IsMultiscreen" /> property should be determined
    ///   automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDetermineIsMultiscreen { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="IWallpaperBase.Placement" />
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="IWallpaperBase.Placement" /> property should be
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDeterminePlacement { get; set; }

    /// <summary>
    ///   Automatically suggests <see cref="IWallpaperBase.IsMultiscreen" /> and <see cref="IWallpaperBase.Placement" /> for the given <paramref name="target" /> 
    ///   according to <see cref="WallpaperDefaultSettings.AutoDetermineIsMultiscreen" /> and <see cref="WallpaperDefaultSettings.AutoDeterminePlacement" />.
    /// </summary>
    /// <param name="target">
    ///   The <see cref="IWallpaper" /> to apply the settings to.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="target" /> has no image size assigned.
    /// </exception>
    void ApplyToWallpaper(IWallpaper target);
  }

  [ContractClassFor(typeof(IWallpaperDefaultSettings))]
  internal abstract class IWallpaperDefaultSettingsContracts: IWallpaperDefaultSettings {
    public abstract bool AutoDetermineIsMultiscreen { get; set; }
    public abstract bool AutoDeterminePlacement { get; set; }
    public abstract IWallpaperBase Settings { get; set; }

    public void ApplyToWallpaper(IWallpaper target) {
      Contract.Requires<ArgumentNullException>(target != null);
      Contract.Requires<ArgumentException>(target.IsImageSizeResolved);
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}