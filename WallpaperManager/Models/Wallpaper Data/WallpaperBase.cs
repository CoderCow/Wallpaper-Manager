// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains general wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public abstract class WallpaperBase : ValidatableBase, IWallpaperBase, ICloneable, IAssignable {
    /// <summary>
    ///   Gets the default background color used for a new instance of this class.
    /// </summary>
    /// <value>
    ///   The default background color used for a new instance of this class.
    /// </value>
    public static Color DefaultBackgroundColor { get; } = Color.Black;

    /// <inheritdoc />
    public bool IsActivated { get; set; }

    /// <inheritdoc />
    public bool IsMultiscreen { get; set; }

    /// <inheritdoc />
    public byte Priority { get; set; }

    /// <inheritdoc />
    public TimeSpan OnlyCycleBetweenStart { get; set; }

    /// <inheritdoc />
    public TimeSpan OnlyCycleBetweenStop { get; set; }

    /// <inheritdoc />
    public WallpaperPlacement Placement { get; set; }

    /// <inheritdoc />
    public Point Offset { get; set; }

    /// <inheritdoc />
    public Point Scale { get; set; }

    /// <inheritdoc />
    public WallpaperEffects Effects { get; set; }

    /// <inheritdoc />
    public Color BackgroundColor { get; set; }

    /// <inheritdoc />
    public ICollection<int> DisabledScreens { get; protected set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBase" /> class.
    /// </summary>
    protected WallpaperBase() {
      this.IsActivated = true;
      this.Priority = 100;
      this.Offset = new Point(0, 0);
      this.Scale = new Point(0, 0);
      this.OnlyCycleBetweenStart = new TimeSpan(0, 0, 0);
      this.OnlyCycleBetweenStop = new TimeSpan(23, 59, 59);
      this.DisabledScreens = new Collection<int>();
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.OnlyCycleBetweenStart))
        if (this.OnlyCycleBetweenStart > this.OnlyCycleBetweenStop)
          return "Start time cannot be greater than stop time.";

      else if (propertyName == nameof(this.OnlyCycleBetweenStop))
        if (this.OnlyCycleBetweenStop < this.OnlyCycleBetweenStart)
          return "Stop time cannot be less than start time.";

      else if (propertyName == nameof(this.Placement))
        if (!Enum.IsDefined(typeof(WallpaperPlacement), this.Placement))
          return "Unknown placement provided.";

      else if (propertyName == nameof(this.DisabledScreens))
        if (this.DisabledScreens == null)
          return "This field is mandatory.";

      return null;
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public abstract object Clone();

    /// <inheritdoc />
    public void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is WallpaperBase);

      WallpaperBase otherInstance = (WallpaperBase)other;
      this.AssignTo(otherInstance);
    }

    /// <inheritdoc cref="AssignTo(object)" />
    protected virtual void AssignTo(WallpaperBase other) {
      Contract.Requires<ArgumentNullException>(other != null);

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
    }
    #endregion
  }
}