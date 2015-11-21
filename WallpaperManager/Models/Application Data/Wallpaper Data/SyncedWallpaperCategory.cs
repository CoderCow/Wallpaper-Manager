using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  public class SyncedWallpaperCategory : WallpaperCategory, ISyncedWallpaperCategory {
    public Path DirectoryPath { get; }

    public SyncedWallpaperCategory(
      string name, IWallpaperDefaultSettings defaultSettings, Path directoryPath, IEnumerable<IWallpaper> wallpapers = null): base(name, defaultSettings, wallpapers) {

      Contract.Requires<ArgumentException>(directoryPath != Path.Invalid);
      
      this.DirectoryPath = directoryPath;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.DirectoryPath))
        if (!Directory.Exists(this.DirectoryPath))
          return string.Format(LocalizationManager.GetLocalizedString("Error.Path.DirectoryNotFound"), this.DirectoryPath);
      
      return null;
    }
    #endregion
  }
}
