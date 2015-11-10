// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines wallpaper category related data and represents a collection of <see cref="IWallpaper" /> objects.
  /// </summary>
  [ContractClass(typeof(IWallpaperCategoryContracts))]
  public interface IWallpaperCategory {
    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether this category and the underlying <see cref="Wallpaper" /> objects
    ///   are activated or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indiciating whether this category and the underlying <see cref="Wallpaper" /> objects are
    ///   activated or not. <c>null</c> if the underlying <see cref="Wallpaper" /> objects have a different activated status.
    /// </value>
    /// <remarks>
    ///   <para>
    ///     The activated status of a category usually indicates if its contained <see cref="Wallpaper" /> objects should be
    ///     automatically cycled or not.
    ///   </para>
    ///   <para>
    ///     Setting this property will also update the <see cref="WallpaperBase.IsActivated" /> property of all
    ///     underlying <see cref="Wallpaper" /> objects of this collection.
    ///   </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperBase.IsActivated">WallpaperBase.IsActivated Property</seealso>
    [DoNotNotify]
    bool? IsActivated { get; set; }

    /// <summary>
    ///   Gets or sets the name of this category.
    /// </summary>
    /// <value>
    ///   The name of this category.
    /// </value>
    /// <seealso cref="Name_InvalidChars">Name_InvalidChars Property</seealso>
    /// <seealso cref="Name_MinLength">Name_MinLength Constant</seealso>
    /// <seealso cref="Name_MaxLength">Name_MaxLength Constant</seealso>
    string Name { get; set; }

    /// <summary>
    ///   Gets or sets the settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </summary>
    /// <value>
    ///   The settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </value>
    /// <seealso cref="Models.WallpaperDefaultSettings">WallpaperDefaultSettings Class</seealso>
    IWallpaperDefaultSettings WallpaperDefaultSettings { get; set; }

    ObservableCollection<Wallpaper> Wallpapers { get; }

    int Count { get; }
  }

  [ContractClassFor(typeof(IWallpaperCategory))]
  internal abstract class IWallpaperCategoryContracts : IWallpaperCategory {
    public abstract bool? IsActivated { get; set; }
    public abstract string Name { get; set; }
    public abstract IWallpaperDefaultSettings WallpaperDefaultSettings { get; set; }
    public abstract ObservableCollection<Wallpaper> Wallpapers { get; }
    public abstract int Count { get; }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.WallpaperDefaultSettings != null);
      Contract.Invariant(!this.Wallpapers.Contains(null));
    }
  }
}