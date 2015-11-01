// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines wallpaper category related data and represents a collection of <see cref="IWallpaper" /> objects.
  /// </summary>
  public interface IWallpaperCategory: IList<IWallpaper> {
    #region Property: IsActivated
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether this category and the underlying <see cref="Wallpaper" /> objects 
    ///   are activated or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indiciating whether this category and the underlying <see cref="Wallpaper" /> objects are 
    ///   activated or not. <c>null</c> if the underlying <see cref="Wallpaper" /> objects have a different activated status.
    /// </value>
    /// <remarks>
    ///   <para>
    ///     The activated status of a category usually indicates if its contained <see cref="Wallpaper" /> objects should be 
    ///     automatically cycled or not.
    ///   </para>
    ///   <para>
    ///     Setting this property will also update the <see cref="WallpaperSettingsBase.IsActivated" /> property of all 
    ///     underlying <see cref="Wallpaper" /> objects of this collection.
    ///   </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperSettingsBase.IsActivated">WallpaperSettingsBase.IsActivated Property</seealso>
    Boolean? IsActivated { get; set; }
    #endregion

    #region Property: Name
    /// <summary>
    ///   Gets or sets the name of this category.
    /// </summary>
    /// <value>
    ///   The name of this category.
    /// </value>
    /// <exception cref="ArgumentException">
    ///   Attempted to set a <see cref="String" /> which contains invalid characters. Refer to the 
    ///   <see cref="Name_InvalidChars" /> property to get a list of invalid characters for a category name.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a <see cref="String" /> of an invalid length. Refer to the <see cref="Name_MinLength" /> and
    ///   <see cref="Name_MaxLength" /> constants for the respective suitable lengths.
    /// </exception>
    /// <seealso cref="Name_InvalidChars">Name_InvalidChars Property</seealso>
    /// <seealso cref="Name_MinLength">Name_MinLength Constant</seealso>
    /// <seealso cref="Name_MaxLength">Name_MaxLength Constant</seealso>
    String Name { get; set; }
    #endregion

    #region Property: WallpaperDefaultSettings
    /// <summary>
    ///   Gets or sets the settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </summary>
    /// <value>
    ///   The settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="Models.WallpaperDefaultSettings">WallpaperDefaultSettings Class</seealso>
    IWallpaperDefaultSettings WallpaperDefaultSettings { get; set; }
    #endregion
  }
}