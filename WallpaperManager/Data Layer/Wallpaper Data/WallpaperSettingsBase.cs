// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

using Common;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Contains general wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public abstract class WallpaperSettingsBase: INotifyPropertyChanged, ICloneable, IAssignable {
    #region Static Property: DefaultBackgroundColor
    /// <summary>
    ///   Gets the default background color used for a new instance of this class.
    /// </summary>
    /// <value>
    ///   The default background color used for a new instance of this class.
    /// </value>
    public static Color DefaultBackgroundColor {
      get { return Color.Black; }
    }
    #endregion

    #region Property: IsActivated
    /// <summary>
    ///   <inheritdoc cref="IsActivated" select='../value/node()' />
    /// </summary>
    private Boolean isActivated;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether this wallpaper is activated.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this wallpaper is activated.
    /// </value>
    /// <remarks>
    ///   The activated status of a wallpaper usually indicates if it should be automatically cycled or not.
    /// </remarks>
    public Boolean IsActivated {
      get { return this.isActivated; }
      set {
        this.isActivated = value;
        this.OnPropertyChanged("IsActivated");
      }
    }
    #endregion

    #region Property: IsMultiscreen
    /// <summary>
    ///   <inheritdoc cref="IsMultiscreen" select='../value/node()' />
    /// </summary>
    private Boolean isMultiscreen;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </value>
    public Boolean IsMultiscreen {
      get { return this.isMultiscreen; }
      set {
        this.isMultiscreen = value;
        this.OnPropertyChanged("IsMultiscreen");
      }
    }
    #endregion

    #region Property: Priority
    /// <summary>
    ///   <inheritdoc cref="Priority" select='../value/node()' />
    /// </summary>
    private Byte priority;

    /// <summary>
    ///   Gets or sets the pick priority of this wallpaper.
    /// </summary>
    /// <value>
    ///   The pick priority of this wallpaper.
    /// </value>
    /// <remarks>
    ///   The pick priority usually represents the chance for the wallpaper of being automatically picked when cycling.
    /// </remarks>
    public Byte Priority {
      get { return this.priority; }
      set {
        this.priority = value;
        this.OnPropertyChanged("Priority");
      }
    }
    #endregion

    #region Property: OnlyCycleBetweenStart
    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStart" select='../value/node()' />
    /// </summary>
    private TimeSpan onlyCycleBetweenStart;

    /// <summary>
    ///   Gets or sets the start time of the range in which this wallpaper should only be cycled.
    /// </summary>
    /// <value>
    ///   The start time of the range in which this wallpaper should only be cycled.
    /// </value>
    /// <seealso cref="OnlyCycleBetweenStop">OnlyCycleBetweenStop Property</seealso>
    public TimeSpan OnlyCycleBetweenStart {
      get { return this.onlyCycleBetweenStart; }
      set {
        this.onlyCycleBetweenStart = value;
        this.OnPropertyChanged("OnlyCycleBetweenStart");
      }
    }
    #endregion

    #region Property: OnlyCycleBetweenStop
    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStop" select='../value/node()' />
    /// </summary>
    private TimeSpan onlyCycleBetweenStop;

    /// <summary>
    ///   Gets or sets the end time of the range in which this wallpaper should only be cycled.
    /// </summary>
    /// <value>
    ///   The end time of the range in which this wallpaper should only be cycled.
    /// </value>
    /// <seealso cref="OnlyCycleBetweenStart">OnlyCycleBetweenStart Property</seealso>
    public TimeSpan OnlyCycleBetweenStop {
      get { return this.onlyCycleBetweenStop; }
      set {
        this.onlyCycleBetweenStop = value;
        this.OnPropertyChanged("OnlyCycleBetweenStop");
      }
    }
    #endregion

    #region Property: Placement
    /// <summary>
    ///   <inheritdoc cref="Placement" select='../value/node()' />
    /// </summary>
    private WallpaperPlacement placement;

    /// <summary>
    ///   Gets or sets a value defining how this wallpaper should be placed when drawn on a screen.
    /// </summary>
    /// <value>
    ///   A value defining how this wallpaper should be placed when drawn on a screen.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is not represented by a constant of the <see cref="WallpaperPlacement" /> enumeration.
    /// </exception>
    public WallpaperPlacement Placement {
      get { return this.placement; }
      set {
        if (!Enum.IsDefined(typeof(WallpaperPlacement), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(WallpaperPlacement), value));
        }

        this.placement = value;
        this.OnPropertyChanged("Placement");
      }
    }
    #endregion

    #region Property: Offset
    /// <summary>
    ///   <inheritdoc cref="Offset" select='../value/node()' />
    /// </summary>
    private Point offset;

    /// <summary>
    ///   Gets or sets the horizontal and vertical placement offset the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The horizontal and vertical placement offset the wallpaper should be drawn with.
    /// </value>
    public Point Offset {
      get { return this.offset; }
      set {
        this.offset = value;
        this.OnPropertyChanged("Offset");
      }
    }
    #endregion

    #region Property: Scale
    /// <summary>
    ///   <inheritdoc cref="Scale" select='../value/node()' />
    /// </summary>
    private Point scale;

    /// <summary>
    ///   Gets or sets the horizontal and vertical scale the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The horizontal and vertical scale the wallpaper should be drawn with.
    /// </value>
    public Point Scale {
      get { return this.scale; }
      set {
        this.scale = value;
        this.OnPropertyChanged("Scale");
      }
    }
    #endregion

    #region Property: Effects
    /// <summary>
    ///   <inheritdoc cref="Effects" select='../value/node()' />
    /// </summary>
    private WallpaperEffects effects;

    /// <summary>
    ///   Gets or sets the effects the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The effects the wallpaper should be drawn with.
    /// </value>
    public WallpaperEffects Effects {
      get { return this.effects; }
      set {
        this.effects = value;
        this.OnPropertyChanged("Effects");
      }
    }
    #endregion

    #region Property: BackgroundColor
    /// <summary>
    ///   <inheritdoc cref="BackgroundColor" select='../value/node()' />
    /// </summary>
    private Color backgroundColor = Wallpaper.DefaultBackgroundColor;

    /// <summary>
    ///   Gets or sets the background color drawn for this wallpaper if it does not fill out the whole screen.
    /// </summary>
    /// <value>
    ///   background color drawn for this wallpaper if it does not fill out the whole screen.
    /// </value>
    public Color BackgroundColor {
      get { return this.backgroundColor; }
      set {
        this.backgroundColor = value;
        this.OnPropertyChanged("BackgroundColor");
      }
    }
    #endregion

    #region Property: DisabledScreens
    /// <summary>
    ///   <inheritdoc cref="DisabledScreens" select='../value/node()' />
    /// </summary>
    private Collection<Int32> disabledScreens = new Collection<Int32>();
    
    /// <summary>
    ///   Gets a collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </summary>
    /// <value>
    ///   A collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </value>
    public Collection<Int32> DisabledScreens {
      get { return this.disabledScreens; }
    }
    #endregion
    

    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperSettingsBase" /> class.
    /// </summary>
    protected WallpaperSettingsBase() {
      this.isActivated = true;
      this.priority = 100;
      this.offset = new Point(0, 0);
      this.scale = new Point(0, 0);
      this.onlyCycleBetweenStart = new TimeSpan(0, 0, 0);
      this.onlyCycleBetweenStop = new TimeSpan(23, 59, 59);
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

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public abstract Object Clone();

    /// <summary>
    ///   Clones all members of the current instance and assigns them to the given instance.
    /// </summary>
    /// <param name="instance">
    ///   The instance to assign the cloned members to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="instance" /> is <c>null</c>.
    /// </exception>
    protected void Clone(WallpaperSettingsBase instance) {
      if (instance == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("instance"));
      }

      this.AssignTo(instance);

      instance.disabledScreens = new Collection<Int32>();
      foreach (Int32 screenIndex in this.DisabledScreens) {
        instance.disabledScreens.Add(screenIndex);
      }
      instance.OnPropertyChanged("DisabledScreens");
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
    /// <exception cref="ArgumentException">
    ///   <paramref name="other" /> is not castable to <see cref="WallpaperSettingsBase" />.
    /// </exception>
    public void AssignTo(Object other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      WallpaperSettingsBase otherInstance = other as WallpaperSettingsBase;
      if (other == null) {
        throw new ArgumentException(ExceptionMessages.GetTypeIsNotCastable("Object", "WallpaperSettingsBase", "other"));
      }

      this.AssignTo(otherInstance);
    }

    /// <inheritdoc cref="AssignTo(Object)" />
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    protected virtual void AssignTo(WallpaperSettingsBase other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      other.IsActivated = this.IsActivated;
      other.IsMultiscreen = this.IsMultiscreen;
      other.Placement = this.Placement;
      other.Scale = this.Scale;
      other.Offset = this.Offset;
      other.Effects = this.Effects;
      other.Priority = this.Priority;
      other.BackgroundColor = this.BackgroundColor;
      other.OnlyCycleBetweenStart = this.OnlyCycleBetweenStart;
      other.OnlyCycleBetweenStop = this.OnlyCycleBetweenStop;
      other.disabledScreens = this.DisabledScreens;
      this.OnPropertyChanged("DisabledScreens");
    }
    #endregion
  }
}
