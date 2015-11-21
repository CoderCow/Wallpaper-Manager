using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using PropertyChanged;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  [DataContract]
  [ImplementPropertyChanged]
  public class SyncedWallpaperCategory : WallpaperCategory, ISyncedWallpaperCategory {
    [DataMember(Order = 1)]
    public Path DirectoryPath { get; set; }

    public SyncedWallpaperCategory(
      string name, IWallpaperDefaultSettings defaultSettings, Path directoryPath, IEnumerable<IWallpaper> wallpapers = null): base(name, defaultSettings, wallpapers) {

      this.DirectoryPath = directoryPath;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.DirectoryPath)) {
        if (this.DirectoryPath == Path.Invalid)
          return LocalizationManager.GetLocalizedString("Error.FieldIsInvalid");
        if (!Directory.Exists(this.DirectoryPath))
          return string.Format(LocalizationManager.GetLocalizedString("Error.Path.DirectoryNotFound"), this.DirectoryPath);
      }
      
      return null;
    }
    #endregion
  }
}
