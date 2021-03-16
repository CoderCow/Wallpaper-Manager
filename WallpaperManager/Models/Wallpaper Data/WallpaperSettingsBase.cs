// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains general wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public abstract class WallpaperSettingsBase : INotifyPropertyChanged, ICloneable, IAssignable {
    /// <summary>
    ///   <inheritdoc cref="BackgroundColor" select='../value/node()' />
    /// </summary>
    private Color backgroundColor = Wallpaper.DefaultBackgroundColor;

    /// <summary>
    ///   <inheritdoc cref="Effects" select='../value/node()' />
    /// </summary>
    private WallpaperEffects effects;

    /// <summary>
    ///   <inheritdoc cref="IsActivated" select='../value/node()' />
    /// </summary>
    private bool isActivated;

    /// <summary>
    ///   <inheritdoc cref="IsMultiscreen" select='../value/node()' />
    /// </summary>
    private bool isMultiscreen;

    /// <summary>
    ///   <inheritdoc cref="Offset" select='../value/node()' />
    /// </summary>
    private Point offset;

    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStart" select='../value/node()' />
    /// </summary>
    private TimeSpan onlyCycleBetweenStart;

    /// <summary>
    ///   <inheritdoc cref="OnlyCycleBetweenStop" select='../value/node()' />
    /// </summary>
    private TimeSpan onlyCycleBetweenStop;

    /// <summary>
    ///   <inheritdoc cref="Placement" select='../value/node()' />
    /// </summary>
    private WallpaperPlacement placement;

    /// <summary>
    ///   <inheritdoc cref="Priority" select='../value/node()' />
    /// </summary>
    private byte priority;

    /// <summary>
    ///   <inheritdoc cref="Scale" select='../value/node()' />
    /// </summary>
    private Point scale;

    /// <summary>
    ///   Gets the default background color used for a new instance of this class.
    /// </summary>
    /// <value>
    ///   The default background color used for a new instance of this class.
    /// </value>
    public static Color DefaultBackgroundColor { get; } = Color.Black;

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether this wallpaper is activated.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether this wallpaper is activated.
    /// </value>
    /// <remarks>
    ///   The activated status of a wallpaper usually indicates if it should be automatically cycled or not.
    /// </remarks>
    public bool IsActivated {
      get { return this.isActivated; }
      set {
        this.isActivated = value;
        this.OnPropertyChanged("IsActivated");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </value>
    public bool IsMultiscreen {
      get { return this.isMultiscreen; }
      set {
        this.isMultiscreen = value;
        this.OnPropertyChanged("IsMultiscreen");
      }
    }

    /// <summary>
    ///   Gets or sets the pick priority of this wallpaper.
    /// </summary>
    /// <value>
    ///   The pick priority of this wallpaper.
    /// </value>
    /// <remarks>
    ///   The pick priority usually represents the chance for the wallpaper of being automatically picked when cycling.
    /// </remarks>
    public byte Priority {
      get { return this.priority; }
      set {
        this.priority = value;
        this.OnPropertyChanged("Priority");
      }
    }

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

    /// <summary>
    ///   Gets or sets a value defining how this wallpaper should be placed when drawn on a screen.
    /// </summary>
    /// <value>
    ///   A value defining how this wallpaper should be placed when drawn on a screen.
    /// </value>
    public WallpaperPlacement Placement {
      get { return this.placement; }
      set {
        this.placement = value;
        this.OnPropertyChanged("Placement");
      }
    }

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

    /// <summary>
    ///   Gets a collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </summary>
    /// <value>
    ///   A collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </value>
    public Collection<int> DisabledScreens { get; private set; }

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
      this.DisabledScreens = new Collection<int>();
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(Enum.IsDefined(typeof(WallpaperPlacement), this.Placement));
      Contract.Invariant(this.DisabledScreens != null);
    }

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public abstract object Clone();

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
      if (instance == null) throw new ArgumentNullException();

      this.AssignTo(instance);

      instance.DisabledScreens = new Collection<int>();
      foreach (int screenIndex in this.DisabledScreens)
        instance.DisabledScreens.Add(screenIndex);

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
    public void AssignTo(object other) {
      if (other == null) throw new ArgumentNullException();
      if (!(other is WallpaperSettingsBase)) throw new ArgumentException();

      WallpaperSettingsBase otherInstance = (WallpaperSettingsBase)other;
      this.AssignTo(otherInstance);
    }

    /// <inheritdoc cref="AssignTo(object)" />
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    protected virtual void AssignTo(WallpaperSettingsBase other) {
      if (other == null) throw new ArgumentNullException();

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
      other.DisabledScreens = this.DisabledScreens;
      this.OnPropertyChanged("DisabledScreens");
    }
    #endregion
  }
}