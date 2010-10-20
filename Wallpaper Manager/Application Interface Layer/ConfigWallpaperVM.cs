// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

using Common;
using Common.IO;
using Common.Presentation;

using WallpaperManager.Data;
using WallpaperManager.Application;

namespace WallpaperManager.ApplicationInterface {
  /// <commondoc select='WrappingCollectionViewModels/General/*' params="WrappedType=Wallpaper" />
  /// <threadsafety static="true" instance="false" />
  public class ConfigWallpaperVM: INotifyPropertyChanged {
    #region Property: ConfigurationMode
    /// <summary>
    ///   <inheritdoc cref="ConfigurationMode" select='../value/node()' />
    /// </summary>
    private readonly ConfigWallpaperMode configurationMode;

    /// <summary>
    ///   Gets the configuration mode for the wallpaper configuration window and <see cref="ConfigWallpaperVM" />. 
    /// </summary>
    /// <value>
    ///   The configuration mode for the wallpaper configuration window and <see cref="ConfigWallpaperVM" />.
    /// </value>
    public ConfigWallpaperMode ConfigurationMode {
      get { return this.configurationMode; }
    }
    #endregion

    #region Property: ParentIsSynchronizedCategory
    /// <summary>
    ///   <inheritdoc cref="ParentIsSynchronizedCategory" select='../value/node()' />
    /// </summary>
    private readonly Boolean parentIsSynchronizedCategory;
    
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the category of the <see cref="Wallpaper" /> instances is a 
    ///   <see cref="SynchronizedWallpaperCategory" /> or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the category of the <see cref="Wallpaper" /> instances is a 
    ///   <see cref="SynchronizedWallpaperCategory" /> or not.
    /// </value>
    public Boolean ParentIsSynchronizedCategory {
      get { return this.parentIsSynchronizedCategory; }
    }
    #endregion

    #region Property: WallpaperData
    /// <summary>
    ///   <inheritdoc cref="WallpaperData" select='../value/node()' />
    /// </summary>
    private readonly IList<WallpaperSettingsBase> wallpaperData;

    /// <summary>
    ///   Gets the collection of <see cref="WallpaperSettingsBase" /> objects to be configured.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="WallpaperSettingsBase" /> objects to be configured.
    /// </value>
    public IList<WallpaperSettingsBase> WallpaperData {
      get { return this.wallpaperData; }
    }
    #endregion

    #region Property: ImagePath
    /// <summary>
    ///   <inheritdoc cref="ImagePath" select='../value/node()' />
    /// </summary>
    private Path imagePath;

    /// <summary>
    ///   Gets or sets the cached <see cref="Wallpaper.ImagePath">image file's path</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="Wallpaper.ImagePath">image file's path</see> value. 
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <remarks>
    ///   This is a cached setting and will be applied to all <see cref="Wallpaper" /> instances when 
    ///   <see cref="ApplySettingsCommand" /> is executed.
    /// </remarks>
    public Path ImagePath {
      get { return this.imagePath; }
      set {
        this.imagePath = value;

        if (value != Path.None) {
          this.HasImagePath = true;
        }

        this.OnPropertyChanged("ImagePath");
      }
    }
    #endregion

    #region Property: HasImagePath
    /// <summary>
    ///   <inheritdoc cref="HasImagePath" select='../value/node()' />
    /// </summary>
    private Boolean? hasImagePath;
    
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether an <see cref="ImagePath" /> is set or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether an <see cref="ImagePath" /> is set or not.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? HasImagePath {
      get { return this.hasImagePath; }
      protected set {
        this.hasImagePath = value;
        this.OnPropertyChanged("HasImagePath");
      }
    }
    #endregion

    #region Property: IsMultiscreen
    /// <summary>
    ///   <inheritdoc cref="IsMultiscreen" select='../value/node()' />
    /// </summary>
    private Boolean? isMultiscreen;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.IsMultiscreen">multiscreen</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.IsMultiscreen">multiscreen</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? IsMultiscreen {
      get { return this.isMultiscreen; }
      set {
        this.isMultiscreen = value;
        this.OnPropertyChanged("IsMultiscreen");
      }
    }
    #endregion

    #region Property: AutoDetermineIsMultiscreen
    /// <summary>
    ///   <inheritdoc cref="AutoDetermineIsMultiscreen" select='../value/node()' />
    /// </summary>
    private Boolean autoDetermineIsMultiscreen;

    /// <summary>
    ///   Gets or sets a cached value indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> 
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be 
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean AutoDetermineIsMultiscreen {
      get { return this.autoDetermineIsMultiscreen; }
      set { 
        this.autoDetermineIsMultiscreen = value; 
        this.OnPropertyChanged("AutoDetermineIsMultiscreen");
      }
    }
    #endregion

    #region Property: Priority
    /// <summary>
    ///   <inheritdoc cref="Priority" select='../value/node()' />
    /// </summary>
    private Byte? priority;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Priority">priority</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Priority">priority</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Byte? Priority {
      get { return this.priority; }
      set {
        if (value < 0) {
          value = 0;
        }
        if (value > 100) {
          value = 100;
        }

        this.priority = value;
        this.OnPropertyChanged("Priority");
      }
    }
    #endregion

    #region Property: Placement
    /// <summary>
    ///   <inheritdoc cref="Placement" select='../value/node()' />
    /// </summary>
    private WallpaperPlacement? placement;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Placement">placement</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Placement">placement</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public WallpaperPlacement? Placement {
      get { return this.placement; }
      set {
        this.placement = value;
        this.OnPropertyChanged("Placement");
      }
    }
    #endregion

    #region Property: AutoDeterminePlacement
    /// <summary>
    ///   <inheritdoc cref="AutoDeterminePlacement" select='../value/node()' />
    /// </summary>
    private Boolean autoDeterminePlacement;

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.Placement" /> 
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.Placement" /> property should be 
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean AutoDeterminePlacement {
      get { return this.autoDeterminePlacement; }
      set { 
        this.autoDeterminePlacement = value; 
        this.OnPropertyChanged("AutoDeterminePlacement");
      }
    }
    #endregion

    #region Property: HorizontalScale
    /// <summary>
    ///   <inheritdoc cref="HorizontalScale" select='../value/node()' />
    /// </summary>
    private Int32? horizontalScale;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Scale">horizontal scale</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Scale">horizontal scale</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Int32? HorizontalScale {
      get { return this.horizontalScale; }
      set {
        this.horizontalScale = value;
        this.OnPropertyChanged("HorizontalScale");
      }
    }
    #endregion

    #region Property: VerticalScale
    /// <summary>
    ///   <inheritdoc cref="VerticalScale" select='../value/node()' />
    /// </summary>
    private Int32? verticalScale;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Scale">vertical scale</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Scale">vertical scale</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Int32? VerticalScale {
      get { return this.verticalScale; }
      set {
        this.verticalScale = value;
        this.OnPropertyChanged("VerticalScale");
      }
    }
    #endregion

    #region Property: HorizontalOffset
    /// <summary>
    ///   <inheritdoc cref="HorizontalOffset" select='../value/node()' />
    /// </summary>
    private Int32? horizontalOffset;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Offset">horizontal offset</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Offset">horizontal offset</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Int32? HorizontalOffset {
      get { return this.horizontalOffset; }
      set {
        this.horizontalOffset = value;
        this.OnPropertyChanged("HorizontalOffset");
      }
    }
    #endregion

    #region Property: VerticalOffset
    /// <summary>
    ///   <inheritdoc cref="VerticalOffset" select='../value/node()' />
    /// </summary>
    private Int32? verticalOffset;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.Offset">vertical offset</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.Offset">vertical offset</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Int32? VerticalOffset {
      get { return this.verticalOffset; }
      set {
        this.verticalOffset = value;
        this.OnPropertyChanged("VerticalOffset");
      }
    }
    #endregion

    #region Property: Effects
    /// <summary>
    ///   <inheritdoc cref="Effects" select='../value/node()' />
    /// </summary>
    private WallpaperEffects? effects;

    /// <summary>
    ///   Gets or sets the drawing <see cref="WallpaperSettingsBase.Effects">effects</see> value.
    /// </summary>
    /// <value>
    ///   The drawing <see cref="WallpaperSettingsBase.Effects">effects</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public WallpaperEffects? Effects {
      get { return this.effects; }
      set {
        this.effects = value;
        this.OnPropertyChanged("Effects");
      }
    }
    #endregion

    #region Property: EffectFlipHorizontal
    /// <summary>
    ///   <inheritdoc cref="EffectFlipHorizontal" select='../value/node()' />
    /// </summary>
    private Boolean? effectFlipHorizontal;

    /// <summary>
    ///   Gets or sets a value indicating whether the flip horizontal effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the flip horizontal effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectFlipHorizontal {
      get { return this.effectFlipHorizontal; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.FlipHorizontal;
        } else {
          if ((this.Effects & WallpaperEffects.FlipHorizontal) == WallpaperEffects.FlipHorizontal) {
            this.Effects = this.Effects ^ WallpaperEffects.FlipHorizontal;
          }
        }

        this.effectFlipHorizontal = value;
        this.OnPropertyChanged("EffectFlipHorizontal");
      }
    }
    #endregion

    #region Property: EffectFlipVertical
    /// <summary>
    ///   <inheritdoc cref="EffectFlipVertical" select='../value/node()' />
    /// </summary>
    private Boolean? effectFlipVertical;

    /// <summary>
    ///   Gets or sets a value indicating whether the flip vertical effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the flip vertical effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectFlipVertical {
      get { return this.effectFlipVertical; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.FlipVertical;
        } else {
          if ((this.Effects & WallpaperEffects.FlipVertical) == WallpaperEffects.FlipVertical) {
            this.Effects = this.Effects ^ WallpaperEffects.FlipVertical;
          }
        }

        this.effectFlipVertical = value;
        this.OnPropertyChanged("EffectFlipVertical");
      }
    }
    #endregion

    #region Property: EffectMirrorLeft
    /// <summary>
    ///   <inheritdoc cref="EffectMirrorLeft" select='../value/node()' />
    /// </summary>
    private Boolean? effectMirrorLeft;

    /// <summary>
    ///   Gets or sets a value indicating whether the mirror left effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the mirror left effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectMirrorLeft {
      get { return this.effectMirrorLeft; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.MirrorLeft;
        } else {
          if ((this.Effects & WallpaperEffects.MirrorLeft) == WallpaperEffects.MirrorLeft) {
            this.Effects = this.Effects ^ WallpaperEffects.MirrorLeft;
          }
        }

        this.effectMirrorLeft = value;
        this.OnPropertyChanged("EffectMirrorLeft");
      }
    }
    #endregion

    #region Property: EffectMirrorRight
    /// <summary>
    ///   <inheritdoc cref="EffectMirrorRight" select='../value/node()' />
    /// </summary>
    private Boolean? effectMirrorRight;

    /// <summary>
    ///   Gets or sets a value indicating whether the mirror right effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the mirror right effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectMirrorRight {
      get { return this.effectMirrorRight; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.MirrorRight;
        } else {
          if ((this.Effects & WallpaperEffects.MirrorRight) == WallpaperEffects.MirrorRight) {
            this.Effects = this.Effects ^ WallpaperEffects.MirrorRight;
          }
        }

        this.effectMirrorRight = value;
        this.OnPropertyChanged("EffectMirrorRight");
      }
    }
    #endregion

    #region Property: EffectMirrorTop
    /// <summary>
    ///   <inheritdoc cref="EffectMirrorTop" select='../value/node()' />
    /// </summary>
    private Boolean? effectMirrorTop;

    /// <summary>
    ///   Gets or sets a value indicating whether the mirror top effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the mirror top effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectMirrorTop {
      get { return this.effectMirrorTop; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.MirrorTop;
        } else {
          if ((this.Effects & WallpaperEffects.MirrorTop) == WallpaperEffects.MirrorTop) {
            this.Effects = this.Effects ^ WallpaperEffects.MirrorTop;
          }
        }

        this.effectMirrorTop = value;
        this.OnPropertyChanged("EffectMirrorTop");
      }
    }
    #endregion

    #region Property: EffectMirrorBottom
    /// <summary>
    ///   <inheritdoc cref="EffectMirrorBottom" select='../value/node()' />
    /// </summary>
    private Boolean? effectMirrorBottom;

    /// <summary>
    ///   Gets or sets a value indicating whether the mirror bottom effect is used or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the mirror bottom effect is used; otherwise <c>false</c>.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    public Boolean? EffectMirrorBottom {
      get { return this.effectMirrorBottom; }
      set {
        if ((value != null) && (value.Value)) {
          this.Effects = this.Effects | WallpaperEffects.MirrorBottom;
        } else {
          if ((this.Effects & WallpaperEffects.MirrorBottom) == WallpaperEffects.MirrorBottom) {
            this.Effects = this.Effects ^ WallpaperEffects.MirrorBottom;
          }
        }

        this.effectMirrorBottom = value;
        this.OnPropertyChanged("EffectMirrorBottom");
      }
    }
    #endregion

    #region Property: OnlyCycleBetweenStart
    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStart" select='../value/node()' />
    /// </summary>
    private TimeSpan? onlyCycleBetweenStart;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.OnlyCycleBetweenStart">OnlyCycleBetweenStart</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.OnlyCycleBetweenStart">OnlyCycleBetweenStart</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    /// <seealso cref="OnlyCycleBetweenStop">OnlyCycleBetweenStop Property</seealso>
    public TimeSpan? OnlyCycleBetweenStart {
      get { return this.onlyCycleBetweenStart; }
      set {
        if (value > this.OnlyCycleBetweenStop) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueMustBeLowerThanVariable(
            value.ToString(), greaterVariableName: "OnlyCycleBetweenStop"
          ));
        }

        this.onlyCycleBetweenStart = value;
        this.OnPropertyChanged("OnlyCycleBetweenStart");
      }
    }
    #endregion

    #region Property: OnlyCycleBetweenStop
    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStop" select='../value/node()' />
    /// </summary>
    private TimeSpan? onlyCycleBetweenStop;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.OnlyCycleBetweenStop">OnlyCycleBetweenStop</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.OnlyCycleBetweenStop">OnlyCycleBetweenStop</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    /// <seealso cref="OnlyCycleBetweenStart">OnlyCycleBetweenStart Property</seealso>
    public TimeSpan? OnlyCycleBetweenStop {
      get { return this.onlyCycleBetweenStop; }
      set {
        if (value < this.OnlyCycleBetweenStart) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueMustBeGreaterThanVariable(
            value.ToString(), lesserVariableName: "OnlyCycleBetweenStart"
          ));
        }

        this.onlyCycleBetweenStop = value;
        this.OnPropertyChanged("OnlyCycleBetweenStop");
      }
    }
    #endregion

    #region Property: BackgroundColor
    /// <summary>
    ///   <inheritdoc cref="BackgroundColor" select='../value/node()' />
    /// </summary>
    private Color? backgroundColor;

    /// <summary>
    ///   Gets or sets the cached <see cref="WallpaperSettingsBase.BackgroundColor">background color</see> value.
    /// </summary>
    /// <value>
    ///   The cached <see cref="WallpaperSettingsBase.BackgroundColor">background color</see> value.
    ///   <c>null</c> if <see cref="WallpaperData" /> contains multiple items which values vary from each other.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    /// <seealso cref="WallpaperChanger" />
    public Color? BackgroundColor {
      get { return this.backgroundColor; }
      set {
        this.backgroundColor = value;
        this.OnPropertyChanged("BackgroundColor");
      }
    }
    #endregion

    #region Property: ScreensCycleState
    /// <summary>
    ///   <inheritdoc cref="ScreensCycleState" select='../value/node()' />
    /// </summary>
    private readonly ObservableCollection<Boolean?> screensCycleState;

    /// <summary>
    ///   Gets a cached collection containing the cycle status for this wallpaper setting instance on all screens.
    /// </summary>
    /// <value>
    ///   A cached collection containing the cycle status for this wallpaper setting instance on all screens.
    /// </value>
    /// <inheritdoc cref="ImagePath" select='remarks' />
    /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
    public ObservableCollection<Boolean?> ScreensCycleState {
      get { return this.screensCycleState; }
    }
    #endregion

    #region Event: RequestClose
    /// <summary>
    ///   Occurs when closing of the bound Views is requested.
    /// </summary>
    /// <seealso cref="RequestCloseEventArgs">RequestCloseEventArgs Class</seealso>
    public event EventHandler<RequestCloseEventArgs> RequestClose;

    /// <summary>
    ///   Called when closing of the bound Views is requested.
    /// </summary>
    /// <remarks>
    ///   This method raises the <see cref="RequestClose">RequestClose Event</see>.
    /// </remarks>
    /// <param name="e">
    ///   The arguments for the event.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="e" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="RequestClose">RequestClose Event</seealso>
    /// <seealso cref="RequestCloseEventArgs">RequestCloseEventArgs Class</seealso>
    protected virtual void OnRequestClose(RequestCloseEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.RequestClose != null) {
        this.RequestClose(this, e);
      }
    }
    #endregion

    #region Event: UnhandledCommandException
    /// <commondoc select='ViewModels/Events/UnhandledCommandException/*' />
    public event EventHandler<CommandExceptionEventArgs> UnhandledCommandException;

    /// <commondoc select='ViewModels/Methods/OnUnhandledCommandException/*' />
    protected virtual void OnUnhandledCommandException(CommandExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.UnhandledCommandException != null) {
        this.UnhandledCommandException.ReverseInvoke(this, e);
      }

      if (!e.IsHandled) {
        throw e.Exception;
      }
    }
    #endregion


    #region Methods: Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigWallpaperVM" /> class and sets it into
    ///   <see cref="ConfigWallpaperMode.ConfigureWallpapers" /> mode.
    /// </summary>
    /// <remarks>
    ///   Call this constructor to configure a collection of <see cref="WallpaperVM" /> instances.
    /// </remarks>
    /// <param name="wallpapers">
    ///   The collection of <see cref="WallpaperSettingsBase" /> objects to be configured.
    /// </param>
    /// <param name="parentIsSynchronizedCategory">
    ///   A <see cref="Boolean" /> indicating whether the category of the <see cref="Wallpaper" /> instances
    ///   is a <see cref="SynchronizedWallpaperCategory" /> or not.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpapers" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="wallpapers" /> is empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="wallpapers" /> contains a <c>null</c> item.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="ConfigWallpaperVM" /> class.
    ///   </summary>
    /// </overloads>
    public ConfigWallpaperVM(ICollection<Wallpaper> wallpapers, Boolean parentIsSynchronizedCategory) {
      if (wallpapers == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapers"));
      }
      if (wallpapers.Count == 0) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCollectionIsEmpty("wallpapers"));
      }
      if (wallpapers.Contains(null)) {
        throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapers"));
      }

      this.configurationMode = ConfigWallpaperMode.ConfigureWallpapers;
      this.parentIsSynchronizedCategory = parentIsSynchronizedCategory;

      this.screensCycleState = new ObservableCollection<Boolean?>();
      for (Int32 i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++) {
        this.ScreensCycleState.Add(true);
      }
      
      this.wallpaperData = new List<WallpaperSettingsBase>(wallpapers.Count);
      foreach (Wallpaper wallpaper in wallpapers) {
        this.WallpaperData.Add(wallpaper);
      }

      // Check whether the data of the wallpapers are different, if so put them into three state (null).
      this.FromWallpaperSettings(this.WallpaperData[0], false);

      for (Int32 i = 1; i < wallpapers.Count; i++) {
        this.FromWallpaperSettings(this.WallpaperData[i], true);
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigWallpaperVM" /> class set in 
    ///   <see cref="ConfigWallpaperMode.ConfigureDefaultSettings" /> mode.
    /// </summary>
    /// <remarks>
    ///   Call this constructor to configure the general default settings for new <see cref="Wallpaper" /> instances.
    /// </remarks>
    /// <param name="wallpaperDefaultSettings">
    ///   The <see cref="WallpaperDefaultSettings" /> instance to be configured.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpaperDefaultSettings" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperDefaultSettings">WallpaperDefaultSettings Class</seealso>
    public ConfigWallpaperVM(WallpaperDefaultSettings wallpaperDefaultSettings) {
      if (wallpaperDefaultSettings == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperDefaultSettings"));
      }
      
      this.configurationMode = ConfigWallpaperMode.ConfigureDefaultSettings;
      this.screensCycleState = new ObservableCollection<Boolean?>();
      for (Int32 i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++) {
        this.ScreensCycleState.Add(true);
      }

      this.wallpaperData = new List<WallpaperSettingsBase>();
      this.WallpaperData.Add(wallpaperDefaultSettings);

      this.FromWallpaperSettings(wallpaperDefaultSettings, false);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigWallpaperVM" /> class set in 
    ///   <see cref="ConfigWallpaperMode.ConfigureStaticWallpaper" /> mode.
    /// </summary>
    /// <remarks>
    ///   Call this constructor to configure the a static <see cref="Wallpaper" />.
    /// </remarks>
    /// <param name="staticWallpaper">
    ///   The <see cref="Wallpaper" /> instance to be configured.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="staticWallpaper" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public ConfigWallpaperVM(Wallpaper staticWallpaper) {
      if (staticWallpaper == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("staticWallpaper"));
      }

      this.configurationMode = ConfigWallpaperMode.ConfigureStaticWallpaper;
      this.screensCycleState = new ObservableCollection<Boolean?>();
      for (Int32 i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++) {
        this.ScreensCycleState.Add(true);
      }

      this.wallpaperData = new List<WallpaperSettingsBase>();
      this.WallpaperData.Add(staticWallpaper);

      this.FromWallpaperSettings(staticWallpaper, false);
    }
    #endregion

    #region Methods: FromWallpaperSettings, ToWallpaperSettings
    /// <summary>
    ///   Assigns all settings of <paramref name="baseSettings" /> with the settings in this instance.
    /// </summary>
    /// <param name="baseSettings">
    ///   The <see cref="WallpaperSettingsBase">wallpaper base settings</see>.
    /// </param>
    /// <param name="nullIfDifferent">
    ///   A <see cref="Boolean" /> indicating whether properties of this instance should be set to <c>null</c> if
    ///   the values of the <paramref name="baseSettings" /> parameter are different.
    /// </param>
    private void FromWallpaperSettings(WallpaperSettingsBase baseSettings, Boolean nullIfDifferent) {
      Wallpaper wallpaperSettings = (baseSettings as Wallpaper);
      WallpaperDefaultSettings wallpaperDefaultSettings = (baseSettings as WallpaperDefaultSettings);
      
      if (nullIfDifferent) {
        if (this.Priority != baseSettings.Priority) {
          this.Priority = null;
        }
      } else {
        this.Priority = baseSettings.Priority;
      }

      if (nullIfDifferent) {
        if (this.IsMultiscreen != baseSettings.IsMultiscreen) {
          this.IsMultiscreen = null;
        }
      } else {
        this.IsMultiscreen = baseSettings.IsMultiscreen;
      }

      if (nullIfDifferent) {
        if (this.Placement != baseSettings.Placement) {
          this.Placement = null;
        }
      } else {
        this.Placement = baseSettings.Placement;
      }

      if (nullIfDifferent) {
        if (this.HorizontalScale != baseSettings.Scale.X) {
          this.HorizontalScale = null;
        }
      } else {
        this.HorizontalScale = baseSettings.Scale.X;
      }

      if (nullIfDifferent) {
        if (this.VerticalScale != baseSettings.Scale.Y) {
          this.VerticalScale = null;
        }
      } else {
        this.VerticalScale = baseSettings.Scale.Y;
      }

      if (nullIfDifferent) {
        if (this.HorizontalOffset != baseSettings.Offset.X) {
          this.HorizontalOffset = null;
        }
      } else {
        this.HorizontalOffset = baseSettings.Offset.X;
      }

      if (nullIfDifferent) {
        if (this.VerticalOffset != baseSettings.Offset.Y) {
          this.VerticalOffset = null;
        }
      } else {
        this.VerticalOffset = baseSettings.Offset.Y;
      }

      if (nullIfDifferent) {
        if (this.BackgroundColor != baseSettings.BackgroundColor) {
          this.BackgroundColor = null;
        }
      } else {
        this.BackgroundColor = baseSettings.BackgroundColor;
      }

      if (nullIfDifferent) {
        if (this.OnlyCycleBetweenStart != baseSettings.OnlyCycleBetweenStart) {
          this.OnlyCycleBetweenStart = null;
        }
      } else {
        this.OnlyCycleBetweenStart = baseSettings.OnlyCycleBetweenStart;
      }

      if (nullIfDifferent) {
        if (this.OnlyCycleBetweenStop != baseSettings.OnlyCycleBetweenStop) {
          this.OnlyCycleBetweenStop = null;
        }
      } else {
        this.OnlyCycleBetweenStop = baseSettings.OnlyCycleBetweenStop;
      }


      if (nullIfDifferent) {
        if (this.Effects != baseSettings.Effects) {
          this.Effects = null;
        }
      } else {
        this.Effects = baseSettings.Effects;
      }

      if (nullIfDifferent) {
        if (
          (this.EffectFlipHorizontal != null) && 
          (this.EffectFlipHorizontal.Value != ((baseSettings.Effects & WallpaperEffects.FlipHorizontal) == WallpaperEffects.FlipHorizontal))
        ) {
          this.EffectFlipHorizontal = null;
        }
      } else {
        this.EffectFlipHorizontal = ((this.Effects & WallpaperEffects.FlipHorizontal) == WallpaperEffects.FlipHorizontal);
      }

      if (nullIfDifferent) {
        if (
          (this.EffectFlipVertical != null) && 
          (this.EffectFlipVertical.Value != ((baseSettings.Effects & WallpaperEffects.FlipVertical) == WallpaperEffects.FlipVertical))
        ) {
          this.EffectFlipVertical = null;
        }
      } else {
        this.EffectFlipVertical = ((this.Effects & WallpaperEffects.FlipVertical) == WallpaperEffects.FlipVertical);
      }

      if (nullIfDifferent) {
        if (
          (this.EffectMirrorLeft != null) && 
          (this.EffectMirrorLeft.Value != ((baseSettings.Effects & WallpaperEffects.MirrorLeft) == WallpaperEffects.MirrorLeft))
        ) {
          this.EffectMirrorLeft = null;
        }
      } else {
        this.EffectMirrorLeft = ((this.Effects & WallpaperEffects.MirrorLeft) == WallpaperEffects.MirrorLeft);
      }

      if (nullIfDifferent) {
        if (
          (this.EffectMirrorRight != null) && 
          (this.EffectMirrorRight.Value != ((baseSettings.Effects & WallpaperEffects.MirrorRight) == WallpaperEffects.MirrorRight))
        ) {
          this.EffectMirrorRight = null;
        }
      } else {
        this.EffectMirrorRight = ((this.Effects & WallpaperEffects.MirrorRight) == WallpaperEffects.MirrorRight);
      }

      if (nullIfDifferent) {
        if (
          (this.EffectMirrorTop != null) && 
          (this.EffectMirrorTop.Value != ((baseSettings.Effects & WallpaperEffects.MirrorTop) == WallpaperEffects.MirrorTop))
        ) {
          this.EffectMirrorTop = null;
        }
      } else {
        this.EffectMirrorTop = ((this.Effects & WallpaperEffects.MirrorTop) == WallpaperEffects.MirrorTop);
      }

      if (nullIfDifferent) {
        if (
          (this.EffectMirrorBottom != null) && 
          (this.EffectMirrorBottom.Value != ((baseSettings.Effects & WallpaperEffects.MirrorBottom) == WallpaperEffects.MirrorBottom))
        ) {
          this.EffectMirrorBottom = null;
        }
      } else {
        this.EffectMirrorBottom = ((this.Effects & WallpaperEffects.MirrorBottom) == WallpaperEffects.MirrorBottom);
      }
  
      for (Int32 i = 0; i < this.ScreensCycleState.Count; i++) {
        if (nullIfDifferent) {
          if (
            (this.ScreensCycleState[i] != null) && 
            (this.ScreensCycleState[i].Value == baseSettings.DisabledScreens.Contains(i))
          ) {
            this.ScreensCycleState[i] = null;
          }
        } else {
          this.ScreensCycleState[i] = !baseSettings.DisabledScreens.Contains(i);
        }
      }

      if (wallpaperSettings != null) {
        if (nullIfDifferent) {
          if (this.HasImagePath != null) {
            if ((this.HasImagePath.Value) && (wallpaperSettings.ImagePath == Path.None)) {
              this.HasImagePath = null;
            }
          }
        } else {
          this.HasImagePath = (wallpaperSettings.ImagePath != Path.None);
        }

        if (nullIfDifferent) {
          if (this.ImagePath != wallpaperSettings.ImagePath) {
            this.ImagePath = Path.None;
          }
        } else {
          this.ImagePath = wallpaperSettings.ImagePath;
        }
      }

      if (wallpaperDefaultSettings != null) {
        if (wallpaperDefaultSettings.AutoDetermineIsMultiscreen) {
          this.IsMultiscreen = null;
        }

        if (wallpaperDefaultSettings.AutoDeterminePlacement) {
          this.Placement = null;
        }
      }
    }

    /// <summary>
    ///   Assigns all settings to the given <paramref name="baseSettings" /> instance.
    /// </summary>
    /// <param name="baseSettings">
    ///   The <see cref="WallpaperSettingsBase">wallpaper base settings</see> where the current settings should be assigned to.
    /// </param>
    private void ToWallpaperSettings(WallpaperSettingsBase baseSettings) {
      Wallpaper wallpaperSettings = (baseSettings as Wallpaper);
      WallpaperDefaultSettings wallpaperDefaultSettings = (baseSettings as WallpaperDefaultSettings);
      
      if (this.Priority != null) {
        baseSettings.Priority = this.Priority.Value;
      }

      if (this.IsMultiscreen != null) {
        baseSettings.IsMultiscreen = this.IsMultiscreen.Value;
      }

      if (this.Placement != null) {
        baseSettings.Placement = this.Placement.Value;
      }

      if (this.HorizontalScale != null) {
        baseSettings.Scale = new Point(this.HorizontalScale.Value, baseSettings.Scale.Y);
      }

      if (this.VerticalScale != null) {
        baseSettings.Scale = new Point(baseSettings.Scale.X, this.VerticalScale.Value);
      }

      if (this.HorizontalOffset != null) {
        baseSettings.Offset = new Point(this.HorizontalOffset.Value, baseSettings.Offset.Y);
      }

      if (this.VerticalOffset != null) {
        baseSettings.Offset = new Point(baseSettings.Offset.X, this.VerticalOffset.Value);
      }

      if (this.EffectFlipHorizontal != null) {
        if (this.EffectFlipHorizontal.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.FlipHorizontal;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.FlipHorizontal) == WallpaperEffects.FlipHorizontal) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.FlipHorizontal;
          }
        }
      }

      if (this.EffectFlipVertical != null) {
        if (this.EffectFlipVertical.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.FlipVertical;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.FlipVertical) == WallpaperEffects.FlipVertical) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.FlipVertical;
          }
        }
      }

      if (this.EffectMirrorLeft != null) {
        if (this.EffectMirrorLeft.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.MirrorLeft;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.MirrorLeft) == WallpaperEffects.MirrorLeft) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.MirrorLeft;
          }
        }
      }

      if (this.EffectMirrorRight != null) {
        if (this.EffectMirrorRight.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.MirrorRight;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.MirrorRight) == WallpaperEffects.MirrorRight) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.MirrorRight;
          }
        }
      }

      if (this.EffectMirrorTop != null) {
        if (this.EffectMirrorTop.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.MirrorTop;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.MirrorTop) == WallpaperEffects.MirrorTop) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.MirrorTop;
          }
        }
      }

      if (this.EffectMirrorBottom != null) {
        if (this.EffectMirrorBottom.Value) {
          baseSettings.Effects = baseSettings.Effects | WallpaperEffects.MirrorBottom;
        } else {
          if ((baseSettings.Effects & WallpaperEffects.MirrorBottom) == WallpaperEffects.MirrorBottom) {
            baseSettings.Effects = baseSettings.Effects ^ WallpaperEffects.MirrorBottom;
          }
        }
      }

      for (Int32 i = 0; i < this.ScreensCycleState.Count; i++) {
        if (this.ScreensCycleState[i] != null) {
          Int32 itemIndex = baseSettings.DisabledScreens.IndexOf(i);

          if (!this.ScreensCycleState[i].Value) {
            if (itemIndex == -1) {
              baseSettings.DisabledScreens.Add(i);
            }
          } else {
            if (itemIndex != -1) {
              baseSettings.DisabledScreens.Remove(i);
            }
          }
        }
      }

      if (this.BackgroundColor != null) {
        baseSettings.BackgroundColor = this.BackgroundColor.Value;
      }

      if (this.OnlyCycleBetweenStart != null) {
        baseSettings.OnlyCycleBetweenStart = this.OnlyCycleBetweenStart.Value;
      }

      if (this.OnlyCycleBetweenStop != null) {
        baseSettings.OnlyCycleBetweenStop = this.OnlyCycleBetweenStop.Value;
      }

      if (wallpaperSettings != null && this.HasImagePath != null) {
        if (this.HasImagePath.Value) {
          if (this.ImagePath != Path.None) {
            wallpaperSettings.ImagePath = this.ImagePath;
          }
        } else {
          wallpaperSettings.ImagePath = Path.None;
        }
      }

      if (wallpaperDefaultSettings != null) {
        wallpaperDefaultSettings.AutoDetermineIsMultiscreen = (this.IsMultiscreen == null);
        wallpaperDefaultSettings.AutoDeterminePlacement = (this.Placement == null);
      }
    }
    #endregion

    #region Command: RemoveImagePath
    /// <summary>
    ///   <inheritdoc cref="RemoveImagePathCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand removeImagePathCommand;
    
    /// <summary>
    ///   Gets the Remove Image Path <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Remove Image Path <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RemoveImagePathCommand_CanExecute">RemoveImagePathCommand_CanExecute Method</seealso>
    /// <seealso cref="RemoveImagePathCommand_Execute">RemoveImagePathCommand_Execute Method</seealso>
    public DelegateCommand RemoveImagePathCommand {
      get {
        if (this.removeImagePathCommand == null) {
          this.removeImagePathCommand = new DelegateCommand(
            this.RemoveImagePathCommand_Execute, this.RemoveImagePathCommand_CanExecute
          );
        }
        
        return this.removeImagePathCommand;
      }
    }
    
    /// <summary>
    ///   Determines if <see cref="RemoveImagePathCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveImagePathCommand" />
    protected Boolean RemoveImagePathCommand_CanExecute() {
      return (!this.ParentIsSynchronizedCategory);
    }
    
    /// <summary>
    ///   Called when <see cref="RemoveImagePathCommand" /> is executed.
    ///   This commands sets <see cref="ImagePath" /> to <c>null</c> and <see cref="HasImagePath" /> to <c>false</c>.
    /// </summary>
    /// <seealso cref="RemoveImagePathCommand" />
    protected void RemoveImagePathCommand_Execute() {
      this.ImagePath = Path.None;
      this.HasImagePath = false;
    }
    #endregion

    #region Command: ApplySettings
    /// <summary>
    ///   <inheritdoc cref="ApplySettingsCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand applySettingsCommand;
    
    /// <summary>
    ///   Gets the Apply Settings Command <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Apply Settings Command <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ApplySettingsCommand_CanExecute">ApplySettingsCommand_CanExecute Method</seealso>
    /// <seealso cref="ApplySettingsCommand_Execute">ApplySettingsCommand_Execute Method</seealso>
    public DelegateCommand ApplySettingsCommand {
      get {
        if (this.applySettingsCommand == null) {
          this.applySettingsCommand = new DelegateCommand(this.ApplySettingsCommand_Execute, this.ApplySettingsCommand_CanExecute);
        }
        
        return this.applySettingsCommand;
      }
    }
    
    /// <summary>
    ///   Determines whether <see cref="ApplySettingsCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ApplySettingsCommand" />
    protected Boolean ApplySettingsCommand_CanExecute() {
      return true;
    }
    
    /// <summary>
    ///   Called when the <see cref="ApplySettingsCommand" /> is executed.
    ///   Assigns all cached settings to the <see cref="WallpaperVM">WallpaperVM instances</see>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   <see cref="IsMultiscreen" /> is <c>null</c> or <c>true</c> and <see cref="ScreensCycleState" /> contains a 
    ///   <c>null</c> or <c>false</c> item.
    /// </exception>
    /// <seealso cref="ApplySettingsCommand" />
    protected void ApplySettingsCommand_Execute() {
      #if !DEBUG
      try {
      #endif
      // If the state of IsMultiscreen differs or is true.
      if ((this.IsMultiscreen == null) || (this.IsMultiscreen.Value)) {
        // Check whether all screens are enabled.
        Boolean allScreensEnabled = true;
        for (Int32 i = 0; i < this.ScreensCycleState.Count; i++) {
          if ((this.ScreensCycleState[i] == null) || (!this.ScreensCycleState[i].Value)) {
            allScreensEnabled = false;
            break;
          }
        }

        if (!allScreensEnabled) {
          this.OnUnhandledCommandException(new CommandExceptionEventArgs(
            this.ApplySettingsCommand,
            new InvalidOperationException(ExceptionMessages.GetMultiscreenWallpaperCanNotBeDisabledForAScreen())
          ));
          return;
        }
      }

      switch (this.ConfigurationMode) {
        case ConfigWallpaperMode.ConfigureWallpapers:
          foreach (Wallpaper wallpaper in this.WallpaperData) {
            // Apply all settings of all not-null properties.
            this.ToWallpaperSettings(wallpaper);
          }

          break;
        case ConfigWallpaperMode.ConfigureDefaultSettings:
          this.ToWallpaperSettings(this.WallpaperData[0]);

          break;
        case ConfigWallpaperMode.ConfigureStaticWallpaper:
          this.ToWallpaperSettings(this.WallpaperData[0]);

          break;
      }

      this.OnRequestClose(new RequestCloseEventArgs(true));
      #if !DEBUG
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ApplySettingsCommand, exception));
      }
      #endif
    }
    #endregion

    #region Command: Cancel
    /// <summary>
    ///   <inheritdoc cref="CancelCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand cancelCommand;

    /// <summary>
    ///   Gets the Cancel <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Cancel <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="CancelCommand_CanExecute">CancelCommand_CanExecute Method</seealso>
    /// <seealso cref="CancelCommand_Execute">CancelCommand_Execute Method</seealso>
    public DelegateCommand CancelCommand {
      get {
        if (this.cancelCommand == null) {
          this.cancelCommand = new DelegateCommand(this.CancelCommand_Execute, this.CancelCommand_CanExecute);
        }

        return this.cancelCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="CancelCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="CancelCommand" />
    protected Boolean CancelCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="CancelCommand" /> is executed and requests the attached views to cancel the configuration 
    ///   process.
    /// </summary>
    /// <seealso cref="CancelCommand" />
    protected void CancelCommand_Execute() {
      try {
        this.OnRequestClose(new RequestCloseEventArgs(false));
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.CancelCommand, exception));
      }
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}
