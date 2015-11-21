using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Common.IO;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IApplicationDataContracts))]
  public interface IApplicationData {
    IConfiguration Configuration { get; }
    ObservableCollection<IWallpaperCategory> WallpaperCategories { get; }
    Dictionary<IWallpaperCategory, Path> CategoryWatchedFoldersAssociations { get; }
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

    public Dictionary<IWallpaperCategory, Path> CategoryWatchedFoldersAssociations {
      get {
        Contract.Ensures(Contract.Result<Dictionary<IWallpaperCategory, Path>>() != null);
        throw new NotImplementedException();
      }
    }
  }
}