using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IApplicationDataWriterContracts))]
  public interface IApplicationDataWriter {
    /// <exception cref="IOException">An I/O error has occured.</exception>
    void Write(IApplicationData appData);
  }

  [ContractClassFor(typeof(IApplicationDataWriter))]
  internal abstract class IApplicationDataWriterContracts: IApplicationDataWriter {
    public void Write(IApplicationData appData) {
      Contract.Requires<ArgumentNullException>(appData != null);

      throw new NotImplementedException();
    }
  }
}