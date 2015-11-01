// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines main application configuration data.
  /// </summary>
  public interface IConfiguration {
    #region Property: General
    /// <summary>
    ///   Gets the <see cref="GeneralConfig" /> instance containing the general configuration data.
    /// </summary>
    /// <value>
    ///   The <see cref="GeneralConfig" /> instance containing the general configuration data.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="GeneralConfig">GeneralConfig Class</seealso>
    IGeneralConfiguration General { get; }
    #endregion

    #region Property: WallpaperCategories
    /// <summary>
    ///   Gets the <see cref="WallpaperCategoryCollection" /> holding the 
    ///   <see cref="WallpaperCategory">Wallpaper wallpaperCategories</see> which's <see cref="Wallpaper" /> instances should 
    ///   be cycled.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperCategoryCollection" /> holding the <see cref="WallpaperCategory" /> instances
    ///   which's <see cref="Wallpaper" /> instances should be cycled.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
    WallpaperCategoryCollection WallpaperCategories { get; }
    #endregion
  }
}