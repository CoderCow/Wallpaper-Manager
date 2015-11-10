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
    public bool IsImageSizeResolved { get; set; }

    /// <inheritdoc />
    public Path ImagePath { get; set; }

    /// <inheritdoc />
    public Size ImageSize { get; set; }

    /// <inheritdoc />
    public DateTime TimeLastCycled { get; set; }

    /// <inheritdoc />
    public DateTime TimeAdded { get; set; }

    /// <inheritdoc />
    public int CycleCountWeek { get; set; }

    /// <inheritdoc />
    public int CycleCountTotal { get; set; }
    

    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class.
    /// </summary>
    public Wallpaper() {}

    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class using the given image path.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the image file of this wallpaper.
    /// </param>
    public Wallpaper(Path imagePath) : this() {
      this.ImagePath = imagePath;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.ImageSize))
        if (this.ImageSize.Width < 0 || this.ImageSize.Height < 0)
          return "Image width and height must be greater or eqal to zero.";

      else if (propertyName == nameof(this.ImagePath))
        if (this.ImagePath == Path.Invalid)
          return "This is not a valid path.";
      
      string error = base.InvalidatePropertyInternal(propertyName);
      if (!string.IsNullOrEmpty(error))
        return error;

      return null;
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

      clone.DisabledScreens = new Collection<int>();
      foreach (int screenIndex in this.DisabledScreens)
        clone.DisabledScreens.Add(screenIndex);

      return clone;
    }

    /// <inheritdoc />
    protected override void AssignTo(WallpaperBase other) {
      base.AssignTo(other);

      Wallpaper wallpaperInstance = (other as Wallpaper);
      if (wallpaperInstance != null) {
        wallpaperInstance.ImagePath = this.ImagePath;
        wallpaperInstance.ImageSize = this.ImageSize;
        wallpaperInstance.TimeLastCycled = this.TimeLastCycled;
        wallpaperInstance.TimeAdded = this.TimeAdded;
        wallpaperInstance.CycleCountWeek = this.CycleCountWeek;
        wallpaperInstance.CycleCountTotal = this.CycleCountTotal;
      }
    }
    #endregion
  }
}