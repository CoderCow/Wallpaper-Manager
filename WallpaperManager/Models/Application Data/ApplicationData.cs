using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Common.IO;

namespace WallpaperManager.Models {
  public class ApplicationData : IApplicationData {
    public IConfiguration Configuration { get; }
    public ObservableCollection<IWallpaperCategory> WallpaperCategories { get; }

    public ApplicationData(IConfiguration configuration, ObservableCollection<IWallpaperCategory> categories) {
      Contract.Requires<ArgumentNullException>(configuration != null);
      Contract.Requires<ArgumentNullException>(categories != null);

      this.Configuration = configuration;
      this.WallpaperCategories = categories;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.Configuration != null);
      Contract.Invariant(this.WallpaperCategories != null);
    }
  }
}