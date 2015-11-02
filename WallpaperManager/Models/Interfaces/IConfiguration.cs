// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines main application configuration data.
  /// </summary>
  [ContractClass(typeof(IConfigurationContracts))]
  public interface IConfiguration {
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
  }

  [ContractClassFor(typeof(IConfiguration))]
  internal abstract class IConfigurationContracts : IConfiguration {
    public abstract IGeneralConfiguration General { get; }
    public abstract WallpaperCategoryCollection WallpaperCategories { get; }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.General != null);
      Contract.Invariant(this.WallpaperCategories != null);
    }
  }
}