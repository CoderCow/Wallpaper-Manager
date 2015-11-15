using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IApplicationDataReaderContracts))]
  public interface IApplicationDataReader {
    /// <exception cref="IOException">An I/O error has occured.</exception>
    IApplicationData Read(); 
  }

  [ContractClassFor(typeof(IApplicationDataReader))]
  internal abstract class IApplicationDataReaderContracts: IApplicationDataReader {
    public IApplicationData Read() {
      Contract.Ensures(Contract.Result<IApplicationData>() != null);

      throw new NotImplementedException();
    }
  }
}