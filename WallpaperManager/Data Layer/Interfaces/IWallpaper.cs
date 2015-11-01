// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Diagnostics.Contracts;
using System.Drawing;

using Common.IO;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Defines general wallpaper related data.
  /// </summary>
  public interface IWallpaper: IWallpaperCommonAttributes {
    #region Property: SuggestIsMultiscreen
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should 
    ///   be automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    Boolean SuggestIsMultiscreen { get; set; }
    #endregion

    #region Property: SuggestPlacement
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    Boolean SuggestPlacement { get; set; }
    #endregion

    #region Property: ImagePath
    /// <summary>
    ///   Gets or sets the path of the image file of this wallpaper.
    /// </summary>
    /// <value>
    ///   The path of the image file of this wallpaper.
    /// </value>
    Path ImagePath { get; set; }
    #endregion

    #region Property: ImageSize
    /// <summary>
    ///   Gets or sets the size of the image where <see cref="ImagePath" /> is reffering to.
    /// </summary>
    /// <value>
    ///   The size of the image where <see cref="ImagePath" /> is reffering to.
    /// </value>
    /// <remarks>
    ///   When this property is changed for the first time, and their respective <see cref="SuggestPlacement" /> and 
    ///   <see cref="SuggestIsMultiscreen" /> properties are <c>true</c>, it will cause the 
    ///   <see cref="WallpaperSettingsBase.Placement" /> and <see cref="WallpaperSettingsBase.IsMultiscreen" /> properties to be 
    ///   suggested automatically related to the new image size.
    /// </remarks>
    /// <seealso cref="WallpaperSettingsBase.Placement">WallpaperSettingsBase.Placement Property</seealso>
    /// <seealso cref="WallpaperSettingsBase.IsMultiscreen">WallpaperSettingsBase.IsMultiscreen Property</seealso>
    /// <seealso cref="SuggestPlacement">SuggestPlacement Property</seealso>
    /// <seealso cref="SuggestIsMultiscreen">SuggestIsMultiscreen Property</seealso>
    Size ImageSize { get; set; }
    #endregion
  }
}