// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.IO;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class Wallpaper : WallpaperSettingsBase, ICloneable, IAssignable {
    /// <summary>
    ///   <inheritdoc cref="ImagePath" select='../value/node()' />
    /// </summary>
    private Path imagePath;

    /// <summary>
    ///   <inheritdoc cref="ImageSize" select='../value/node()' />
    /// </summary>
    private Size imageSize;

    /// <summary>
    ///   <inheritdoc cref="SuggestIsMultiscreen" select='../value/node()' />
    /// </summary>
    private bool suggestIsMultiscreen;

    /// <summary>
    ///   <inheritdoc cref="SuggestPlacement" select='../value/node()' />
    /// </summary>
    private bool suggestPlacement;

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether any properties of this instance had been changed since it has been
    ///   instanced.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether any properties of this instance had been changed since it has been
    ///   instanced.
    /// </value>
    public bool IsBlank { get; private set; }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should
    ///   be automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    public bool SuggestIsMultiscreen {
      get { return this.suggestIsMultiscreen; }
      set {
        this.suggestIsMultiscreen = value;
        this.OnPropertyChanged("SuggestIsMultiscreen");
      }
    }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    public bool SuggestPlacement {
      get { return this.suggestPlacement; }
      set {
        this.suggestPlacement = value;
        this.OnPropertyChanged("SuggestPlacement");
      }
    }

    /// <summary>
    ///   Gets or sets the path of the image file of this wallpaper.
    /// </summary>
    /// <value>
    ///   The path of the image file of this wallpaper.
    /// </value>
    public Path ImagePath {
      get { return this.imagePath; }
      set {
        this.imagePath = value;
        this.OnPropertyChanged("ImagePath");
      }
    }

    /// <summary>
    ///   Gets or sets the size of the image where <see cref="ImagePath" /> is reffering to.
    /// </summary>
    /// <value>
    ///   The size of the image where <see cref="ImagePath" /> is reffering to.
    /// </value>
    /// <remarks>
    ///   When this property is changed for the first time, and their respective <see cref="SuggestPlacement" /> and
    ///   <see cref="SuggestIsMultiscreen" /> properties are <c>true</c>, it will cause the
    ///   <see cref="WallpaperSettingsBase.Placement" /> and <see cref="WallpaperSettingsBase.IsMultiscreen" /> properties to
    ///   be
    ///   suggested automatically related to the new image size.
    /// </remarks>
    /// <seealso cref="WallpaperSettingsBase.Placement">WallpaperSettingsBase.Placement Property</seealso>
    /// <seealso cref="WallpaperSettingsBase.IsMultiscreen">WallpaperSettingsBase.IsMultiscreen Property</seealso>
    /// <seealso cref="SuggestPlacement">SuggestPlacement Property</seealso>
    /// <seealso cref="SuggestIsMultiscreen">SuggestIsMultiscreen Property</seealso>
    public Size ImageSize {
      get { return this.imageSize; }
      set {
        // Auto determine the best settings for the wallpaper, if necessary.
        // TODO: Dont do this in a property setter
        this.SuggestSettings(value);

        this.imageSize = value;
        this.OnPropertyChanged("ImageSize");
      }
    }

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
      this.imagePath = imagePath;
    }

    /// <summary>
    ///   Checks whether the cycle conditions for this wallpaper match or not.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the cycle conditions match.
    /// </returns>
    public bool EvaluateCycleConditions() {
      TimeSpan timeOfDay = DateTime.Now.TimeOfDay;

      return ((timeOfDay >= this.OnlyCycleBetweenStart) && (timeOfDay <= this.OnlyCycleBetweenStop));
    }

    /// <summary>
    ///   Automatically suggests <see cref="WallpaperSettingsBase.IsMultiscreen" /> and
    ///   <see cref="WallpaperSettingsBase.Placement" /> using the given <paramref name="imageSize" /> value if their
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

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected override void OnPropertyChanged(string propertyName) {
      base.OnPropertyChanged(propertyName);

      // Image size is sometimes set after the class is being constructed.
      if (propertyName != "ImageSize")
        this.IsBlank = false;

      base.OnPropertyChanged("IsBlank");
    }

    /// <summary>
    ///   Generates a <see cref="string" /> containing the <see cref="ImagePath" />.
    /// </summary>
    /// <returns>
    ///   A <see cref="string" /> containing the <see cref="ImagePath" />.
    /// </returns>
    public override string ToString() {
      return $"ImagePath: {this.ImagePath}";
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public override object Clone() {
      Wallpaper clonedInstance = new Wallpaper(this.ImagePath);

      // Clone all fields defined by WallpaperSettingsBase.
      base.Clone(clonedInstance);

      clonedInstance.IsBlank = this.IsBlank;
      clonedInstance.imagePath = this.ImagePath;
      clonedInstance.imageSize = this.ImageSize;

      return clonedInstance;
    }

    /// <summary>
    ///   Assigns all member values of this instance to the respective members of the given instance.
    /// </summary>
    /// <param name="other">
    ///   The target instance to assign to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    protected override void AssignTo(WallpaperSettingsBase other) {
      Contract.Requires<ArgumentNullException>(other != null);

      // Assign all members defined by WallpaperSettingsBase.
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