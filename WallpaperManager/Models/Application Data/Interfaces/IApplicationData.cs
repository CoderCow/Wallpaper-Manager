using System;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IApplicationDataContracts))]
  public interface IApplicationData {
    IConfiguration Configuration { get; }

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

  [ContractClassFor(typeof(IApplicationData))]
  internal abstract class IApplicationDataContracts: IApplicationData {
    public IConfiguration Configuration {
      get {
        Contract.Ensures(Contract.Result<IConfiguration>() != null);
        throw new NotImplementedException();
      }
    }

    public WallpaperCategoryCollection WallpaperCategories {
      get {
        Contract.Ensures(Contract.Result<WallpaperCategoryCollection>() != null);
        throw new NotImplementedException();
      }
    }
  }
}