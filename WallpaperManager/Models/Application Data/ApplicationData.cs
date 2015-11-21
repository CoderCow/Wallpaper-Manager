using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using Common.IO;
using Common.Presentation;
using PropertyChanged;

namespace WallpaperManager.Models {
  [DataContract]
  [ImplementPropertyChanged]
  public class ApplicationData : ValidatableBase, IApplicationData {
    [DataMember(Order = 1)]
    public int DataVersion { get; set; }

    [DataMember(Order = 2)]
    public IConfiguration Configuration { get; set; }

    [DataMember(Order = 3)]
    public ObservableCollection<IWallpaperCategory> WallpaperCategories { get; set; }

    public ApplicationData(IConfiguration configuration = null, ObservableCollection<IWallpaperCategory> categories = null) {
      this.Configuration = configuration;
      this.WallpaperCategories = categories;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.Configuration)) {
        if (this.Configuration == null)
          return LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      } else if (propertyName == nameof(this.WallpaperCategories)) {
        if (this.WallpaperCategories == null)
          return LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      }
      
      return null;
    }
    #endregion
  }
}