// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Common;
using PropertyChanged;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [DataContract]
  [ImplementPropertyChanged]
  public class Wallpaper : WallpaperBase, IWallpaper, ICloneable, IAssignable {
    /// <inheritdoc />
    [DependsOn(nameof(ImageSize))]
    public bool IsImageSizeResolved => this.ImageSize != null;

    /// <inheritdoc />
    [DataMember(Order = 1)]
    public Path ImagePath { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 2)]
    public Size? ImageSize { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 3)]
    public DateTime TimeLastCycled { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 4)]
    public DateTime TimeAdded { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 5)]
    public int CycleCountWeek { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 6)]
    public int CycleCountTotal { get; set; }
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class using the given image path.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the image file of this wallpaper.
    /// </param>
    public Wallpaper(Path imagePath = default(Path)) {
      this.ImagePath = imagePath;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.ImagePath)) {
        if (this.ImagePath == Path.Invalid)
          return LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
        else if (!File.Exists(this.ImagePath))
          return string.Format(LocalizationManager.GetLocalizedString("Error.Path.FileNotFound"), this.ImagePath);
      } else if (propertyName == nameof(this.ImageSize)) {
        if (this.ImageSize != null && (this.ImageSize.Value.Width <= 0 || this.ImageSize.Value.Height <= 0))
          return LocalizationManager.GetLocalizedString("Error.Image.CantBeNegativeSize");
      } else if (propertyName == nameof(this.TimeAdded) || propertyName == nameof(this.TimeLastCycled)) {
        if (this.TimeAdded > this.TimeLastCycled)
          return LocalizationManager.GetLocalizedString("Error.Wallpaper.AddedCycledTimeInvalid");
      } else if (propertyName == nameof(this.CycleCountTotal)) {
        if (this.CycleCountTotal < 0)
          return LocalizationManager.GetLocalizedString("Error.FieldIsInvalid");
      } else if (propertyName == nameof(this.CycleCountWeek)) {
        if (this.CycleCountWeek < 0)
          return LocalizationManager.GetLocalizedString("Error.FieldIsInvalid");
      }

      return base.InvalidatePropertyInternal(propertyName);
    }
    #endregion

    /// <summary>
    ///   Generates a <see cref="string" /> containing the <see cref="ImagePath" />.
    /// </summary>
    /// <returns>
    ///   A <see cref="string" /> containing the <see cref="ImagePath" />.
    /// </returns>
    public override string ToString() {
      return $"{nameof(this.ImagePath)}: {this.ImagePath}";
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public override object Clone() {
      Wallpaper clone = (Wallpaper)this.MemberwiseClone();
      clone.DisabledDevices = new HashSet<string>(this.DisabledDevices);

      return clone;
    }

    /// <inheritdoc />
    public override void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is WallpaperBase || other is Wallpaper);

      base.AssignTo(other);

      Wallpaper otherWallpaper = (other as Wallpaper);
      if (otherWallpaper != null) {
        otherWallpaper.ImagePath = this.ImagePath;
        otherWallpaper.ImageSize = this.ImageSize;
        otherWallpaper.TimeAdded = this.TimeAdded;
        otherWallpaper.TimeLastCycled = this.TimeLastCycled;
        otherWallpaper.CycleCountTotal = this.CycleCountTotal;
        otherWallpaper.CycleCountWeek = this.CycleCountWeek;
      }
    }
    #endregion
  }
}