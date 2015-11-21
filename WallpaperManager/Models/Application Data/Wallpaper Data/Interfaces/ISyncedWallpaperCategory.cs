using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.IO;

namespace WallpaperManager.Models {
  [ContractClass(typeof(ISyncedWallpaperCategoryContracts))]
  public interface ISyncedWallpaperCategory {
    Path DirectoryPath { get; }
  }

  [ContractClassFor(typeof(ISyncedWallpaperCategory))]
  internal abstract class ISyncedWallpaperCategoryContracts: ISyncedWallpaperCategory {
    public Path DirectoryPath {
      get {
        Contract.Ensures(Contract.Result<Path>() != Path.Invalid);
        throw new NotImplementedException();
      }
    }
  }
}
