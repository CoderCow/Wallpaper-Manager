// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines data to be applied to newly added wallpapers.
  /// </summary>
  public interface IWallpaperDefaultSettings : IWallpaperCommonAttributes {
    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be
    ///   determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be determined
    ///   automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDetermineIsMultiscreen { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.Placement" />
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.Placement" /> property should be
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDeterminePlacement { get; set; }
  }
}