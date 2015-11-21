using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Common.IO;

namespace WallpaperManager.Models {
  public class ApplicationData : IApplicationData {
    public IConfiguration Configuration { get; }
    public ObservableCollection<IWallpaperCategory> WallpaperCategories { get; }
    public Dictionary<IWallpaperCategory, Path> CategoryWatchedFoldersAssociations { get; }

    public ApplicationData(IConfiguration configuration, ObservableCollection<IWallpaperCategory> categories, Dictionary<IWallpaperCategory, Path> categoryWatchedFolderAssociations) {
      Contract.Requires<ArgumentNullException>(configuration != null);
      Contract.Requires<ArgumentNullException>(categories != null);
      Contract.Requires<ArgumentNullException>(categoryWatchedFolderAssociations != null);

      this.Configuration = configuration;
      this.WallpaperCategories = categories;
      this.CategoryWatchedFoldersAssociations = categoryWatchedFolderAssociations;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.Configuration != null);
      Contract.Invariant(this.WallpaperCategories != null);
      Contract.Invariant(this.CategoryWatchedFoldersAssociations != null);
    }
  }
}