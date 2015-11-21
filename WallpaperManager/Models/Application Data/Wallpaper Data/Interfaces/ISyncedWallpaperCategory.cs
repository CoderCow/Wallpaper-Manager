using System;
using Common.IO;

namespace WallpaperManager.Models {
  public interface ISyncedWallpaperCategory {
    Path DirectoryPath { get; }
  }
}
