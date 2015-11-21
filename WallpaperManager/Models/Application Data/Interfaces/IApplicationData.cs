using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Common.IO;

namespace WallpaperManager.Models {
  public interface IApplicationData {
    int DataVersion { get; set; }
    IConfiguration Configuration { get; }
    ObservableCollection<IWallpaperCategory> WallpaperCategories { get; }
  }
}