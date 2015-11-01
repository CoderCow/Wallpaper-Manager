using System;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Defines data to be applied to newly added wallpapers.
  /// </summary>
  public interface IWallpaperDefaultSettings: IWallpaperCommonAttributes {
    #region Property: AutoDetermineIsMultiscreen
    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be 
    ///   determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be determined 
    ///   automatically; otherwise <c>false</c>.
    /// </value>
    Boolean AutoDetermineIsMultiscreen { get; set; }
    #endregion

    #region Property: AutoDeterminePlacement
    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.Placement" /> 
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.Placement" /> property should be 
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    Boolean AutoDeterminePlacement { get; set; }
    #endregion
  }
}