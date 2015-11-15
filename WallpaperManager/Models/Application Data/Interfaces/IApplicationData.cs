using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IApplicationDataContracts))]
  public interface IApplicationData {
    IConfiguration Configuration { get; }
    ObservableCollection<IWallpaperCategory> WallpaperCategories { get; }
  }

  [ContractClassFor(typeof(IApplicationData))]
  internal abstract class IApplicationDataContracts: IApplicationData {
    public IConfiguration Configuration {
      get {
        Contract.Ensures(Contract.Result<IConfiguration>() != null);
        throw new NotImplementedException();
      }
    }

    public ObservableCollection<IWallpaperCategory> WallpaperCategories {
      get {
        Contract.Ensures(Contract.Result<ObservableCollection<IWallpaperCategory>>() != null);
        throw new NotImplementedException();
      }
    }
  }
}