// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
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
    /// <summary>
    ///   <inheritdoc cref="ImageSize" select='../value/node()' />
    /// </summary>
    private Size imageSize;

    /// <inheritdoc />
    public bool IsBlank { get; private set; }

    /// <inheritdoc />
    public bool SuggestIsMultiscreen { get; set; }

    /// <inheritdoc />
    public bool SuggestPlacement { get; set; }

    /// <inheritdoc />
    public Path ImagePath { get; set; }

    /// <inheritdoc />
    public Size ImageSize {
      get { return this.imageSize; }
      set {
        // Auto determine the best settings for the wallpaper, if necessary.
        // TODO: Dont do this in a property setter
        this.SuggestSettings(value);

        this.imageSize = value;
      }
    }

    // TODO: Implement those in cycler
    /// <inheritdoc />
    public DateTime TimeLastCycled { get; set; }

    /// <inheritdoc />
    public DateTime TimeAdded { get; set; }

    /// <inheritdoc />
    public int CycleCount { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class.
    /// </summary>
    public Wallpaper() {
      this.IsBlank = true;
    }

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
      if (propertyName == nameof(this.imageSize))
        if (this.imageSize.Width < 0 || this.ImageSize.Height < 0)
          return "Image width and height must be greater or eqal to zero.";

      else if (propertyName == nameof(this.ImagePath))
        if (this.ImagePath == Path.None)
          return "This is not a valid path.";
      
      string error = base.InvalidatePropertyInternal(propertyName);
      if (!string.IsNullOrEmpty(error))
        return error;

      return null;
    }
    #endregion

    // TODO: move to other type
    /// <inheritdoc />
    public bool EvaluateCycleConditions() {
      TimeSpan timeOfDay = DateTime.Now.TimeOfDay;

      return ((timeOfDay >= this.OnlyCycleBetweenStart) && (timeOfDay <= this.OnlyCycleBetweenStop));
    }

    // TODO: move to other type
    /// <summary>
    ///   Automatically suggests <see cref="WallpaperBase.IsMultiscreen" /> and
    ///   <see cref="WallpaperBase.Placement" /> using the given <paramref name="imageSize" /> value if their
    ///   respective
    ///   <see cref="SuggestIsMultiscreen" /> and <see cref="SuggestPlacement" /> properties return <c>true</c>.
    /// </summary>
    /// <param name="imageSize">
    ///   The size of the image to suggest settings for.
    /// </param>
    protected void SuggestSettings(Size imageSize) {
      if (this.SuggestPlacement) {
        // If the wallpaper is pretty small, we guess that it will maybe used 
        // in "Tile" mode, otherwise we recommend "StretchWithRatio" mode.
        if ((imageSize.Width < 640) || (imageSize.Height < 480))
          this.Placement = WallpaperPlacement.Tile;
        else
          this.Placement = WallpaperPlacement.Uniform;

        this.SuggestPlacement = false;
      }

      if (Screen.AllScreens.Length > 1) {
        if (this.SuggestIsMultiscreen) {
          Rectangle primaryScreenBounds = Screen.PrimaryScreen.Bounds;

          // If the wallpaper's width is at least 150% of the primary screen, we guess that it will 
          // be used as an multi screen wallpaper.
          if (imageSize.Width > (primaryScreenBounds.Width * 1.50)) {
            this.IsMultiscreen = true;
            this.Placement = WallpaperPlacement.UniformToFill;
          }

          this.SuggestIsMultiscreen = false;
        }
      } else
        this.IsMultiscreen = false;
    }

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
      Wallpaper clonedInstance = new Wallpaper(this.ImagePath);

      // Clone all fields defined by WallpaperBase.
      base.Clone(clonedInstance);

      clonedInstance.IsBlank = this.IsBlank;
      clonedInstance.ImagePath = this.ImagePath;
      clonedInstance.imageSize = this.ImageSize;

      return clonedInstance;
    }

    /// <inheritdoc />
    protected override void AssignTo(WallpaperBase other) {
      // Assign all members defined by WallpaperBase.
      base.AssignTo(other);

      Wallpaper wallpaperInstance = (other as Wallpaper);
      if (wallpaperInstance != null) {
        wallpaperInstance.ImagePath = this.ImagePath;
        wallpaperInstance.ImageSize = this.ImageSize;
      }
    }
    #endregion
  }
}