// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.IO;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ImplementPropertyChanged]
  public class Wallpaper : WallpaperBase, IWallpaper, ICloneable, IAssignable {
    /// <inheritdoc />
    [DependsOn(nameof(ImageSize))]
    public bool IsImageSizeResolved => this.ImageSize != null;

    /// <inheritdoc />
    public Path ImagePath { get; set; }

    /// <inheritdoc />
    public Size? ImageSize { get; set; }

    /// <inheritdoc />
    public DateTime TimeLastCycled { get; set; }

    /// <inheritdoc />
    public DateTime TimeAdded { get; set; }

    /// <inheritdoc />
    public int CycleCountWeek { get; set; }

    /// <inheritdoc />
    public int CycleCountTotal { get; set; }
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class using the given image path.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the image file of this wallpaper.
    /// </param>
    public Wallpaper(Path imagePath) {
      this.ImagePath = imagePath;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.ImageSize)) {
        if (this.ImageSize != null && (this.ImageSize.Value.Width < 0 || this.ImageSize.Value.Height < 0))
          return LocalizationManager.GetLocalizedString("Error.Image.CantBeNegativeSize");
      } else if (propertyName == nameof(this.TimeAdded) || propertyName == nameof(this.TimeLastCycled)) {
        if (this.TimeAdded > this.TimeLastCycled)
          return LocalizationManager.GetLocalizedString("Error.Wallpaper.AddedCycledTimeInvalid");
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

      clone.DisabledDevices = new Collection<string>();
      foreach (string uniqueDeviceId in this.DisabledDevices)
        clone.DisabledDevices.Add(uniqueDeviceId);

      return clone;
    }

    /// <inheritdoc />
    protected override void AssignTo(WallpaperBase other) {
      base.AssignTo(other);

      Wallpaper wallpaperInstance = (other as Wallpaper);
      if (wallpaperInstance != null) {
        wallpaperInstance.ImagePath = this.ImagePath;
        wallpaperInstance.ImageSize = this.ImageSize;
        wallpaperInstance.TimeAdded = this.TimeAdded;
        wallpaperInstance.TimeLastCycled = this.TimeLastCycled;
        wallpaperInstance.CycleCountTotal = this.CycleCountTotal;
        wallpaperInstance.CycleCountWeek = this.CycleCountWeek;
      }
    }
    #endregion
  }
}