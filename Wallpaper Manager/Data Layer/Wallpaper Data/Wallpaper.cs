// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Drawing;
using System.Windows.Forms;

using Common;
using Common.IO;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Contains wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class Wallpaper: WallpaperSettingsBase, ICloneable, IAssignable {
    #region Property: IsBlank
    /// <summary>
    ///   <inheritdoc cref="IsBlank" select='../value/node()' />
    /// </summary>
    private Boolean isBlank;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether any properties of this instance had been changed since it has been
    ///   instanced.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether any properties of this instance had been changed since it has been 
    ///   instanced.
    /// </value>
    public Boolean IsBlank {
      get { return this.isBlank; }
    }
    #endregion

    #region Property: SuggestIsMultiscreen
    /// <summary>
    ///   <inheritdoc cref="SuggestIsMultiscreen" select='../value/node()' />
    /// </summary>
    private Boolean suggestIsMultiscreen;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should 
    ///   be automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    public Boolean SuggestIsMultiscreen {
      get { return this.suggestIsMultiscreen; }
      set {
        this.suggestIsMultiscreen = value;
        this.OnPropertyChanged("SuggestIsMultiscreen");
      }
    }
    #endregion

    #region Property: SuggestPlacement
    /// <summary>
    ///   <inheritdoc cref="SuggestPlacement" select='../value/node()' />
    /// </summary>
    private Boolean suggestPlacement;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether <see cref="WallpaperSettingsBase.Placement" /> setting should be 
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    public Boolean SuggestPlacement {
      get { return this.suggestPlacement; }
      set {
        this.suggestPlacement = value;
        this.OnPropertyChanged("SuggestPlacement");
      }
    }
    #endregion

    #region Property: ImagePath
    /// <summary>
    ///   <inheritdoc cref="ImagePath" select='../value/node()' />
    /// </summary>
    private Path imagePath;

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
    #endregion

    #region Property: ImageSize
    /// <summary>
    ///   <inheritdoc cref="ImageSize" select='../value/node()' />
    /// </summary>
    private Size imageSize;

    /// <summary>
    ///   Gets or sets the size of the image where <see cref="ImagePath" /> is reffering to.
    /// </summary>
    /// <value>
    ///   The size of the image where <see cref="ImagePath" /> is reffering to.
    /// </value>
    /// <remarks>
    ///   When this property is changed for the first time, and their respective <see cref="SuggestPlacement" /> and 
    ///   <see cref="SuggestIsMultiscreen" /> properties are <c>true</c>, it will cause the 
    ///   <see cref="WallpaperSettingsBase.Placement" /> and <see cref="WallpaperSettingsBase.IsMultiscreen" /> properties to be 
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
        this.SuggestSettings(value);

        this.imageSize = value;
        this.OnPropertyChanged("ImageSize");
      }
    }
    #endregion


    #region Methods: Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class.
    /// </summary>
    public Wallpaper() {
      this.isBlank = true;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Wallpaper" /> class using the given image path.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the image file of this wallpaper.
    /// </param>
    public Wallpaper(Path imagePath): this() {
      this.imagePath = imagePath;
    }
    #endregion

    #region Methods: EvaluateCycleConditions, SuggestSettings, OnPropertyChanged, ToString
    /// <summary>
    ///   Checks whether the cycle conditions for this wallpaper match or not. 
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the cycle conditions match.
    /// </returns>
    public Boolean EvaluateCycleConditions() {
      TimeSpan timeOfDay = DateTime.Now.TimeOfDay;

      return ((timeOfDay >= this.OnlyCycleBetweenStart) && (timeOfDay <= this.OnlyCycleBetweenStop));
    }

    /// <summary>
    ///   Automatically suggests <see cref="WallpaperSettingsBase.IsMultiscreen" /> and 
    ///   <see cref="WallpaperSettingsBase.Placement" /> using the given <paramref name="imageSize" /> value if their respective
    ///   <see cref="SuggestIsMultiscreen" /> and <see cref="SuggestPlacement" /> properties return <c>true</c>.
    /// </summary>
    /// <param name="imageSize">
    ///   The size of the image to suggest settings for.
    /// </param>
    protected void SuggestSettings(Size imageSize) {
      if (this.SuggestPlacement) {
        // If the wallpaper is pretty small, we guess that it will maybe used 
        // in "Tile" mode, otherwise we recommend "StretchWithRatio" mode.
        if ((imageSize.Width < 640) || (imageSize.Height < 480)) {
          this.Placement = WallpaperPlacement.Tile;
        } else {
          this.Placement = WallpaperPlacement.Uniform;
        }

        this.SuggestPlacement = false;
      }

      if (Screen.AllScreens.Length > 1) {
        if (this.SuggestIsMultiscreen) {
          System.Drawing.Rectangle primaryScreenBounds = Screen.PrimaryScreen.Bounds;

          // If the wallpaper's width is at least 150% of the primary screen, we guess that it will 
          // be used as an multi screen wallpaper.
          if (imageSize.Width > (primaryScreenBounds.Width * 1.50)) {
            this.IsMultiscreen = true;
            this.Placement = WallpaperPlacement.UniformToFill;
          }

          this.SuggestIsMultiscreen = false;
        }
      } else {
        this.IsMultiscreen = false;
      }
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected override void OnPropertyChanged(String propertyName) {
      base.OnPropertyChanged(propertyName);

      // Image size is sometimes set after the class is being constructed.
      if (propertyName != "ImageSize") {
        this.isBlank = false;
      }

      base.OnPropertyChanged("IsBlank");
    }

    /// <summary>
    ///   Generates a <see cref="String" /> containing the <see cref="ImagePath" />.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> containing the <see cref="ImagePath" />.
    /// </returns>
    public override String ToString() {
      return StringGenerator.FromListKeyed(
        new String[] { "ImagePath" },
        new Object[] { this.ImagePath }
      );
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public override Object Clone() {
      Wallpaper clonedInstance = new Wallpaper(this.ImagePath);

      // Clone all fields defined by WallpaperSettingsBase.
      base.Clone(clonedInstance);

      clonedInstance.isBlank = this.IsBlank;
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
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

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
