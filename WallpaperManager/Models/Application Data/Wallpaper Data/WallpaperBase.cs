// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;
using Common.Presentation;

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
    public ICollection<string> DisabledDevices { get; protected set; }

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
      this.BackgroundColor = DefaultBackgroundColor;
      this.DisabledDevices = new Collection<string>();
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.OnlyCycleBetweenStart) || propertyName == nameof(this.OnlyCycleBetweenStop)) {
        if (this.OnlyCycleBetweenStart > this.OnlyCycleBetweenStop)
          return LocalizationManager.GetLocalizedString("Error.Wallpaper.CycleTime.Greater");
        else if (this.OnlyCycleBetweenStop < this.OnlyCycleBetweenStart)
          return LocalizationManager.GetLocalizedString("Error.Wallpaper.CycleTime.Lesser");
        else if (this.OnlyCycleBetweenStart < TimeSpan.Zero)
          return LocalizationManager.GetLocalizedString("Error.Time.CantBeNegative");
      } else if (propertyName == nameof(this.BackgroundColor)) {
        if (this.BackgroundColor == Color.Empty)
          return LocalizationManager.GetLocalizedString("Error.Color.CantBeEmpty");
      }

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
      other.DisabledDevices = this.DisabledDevices;
    }
    #endregion
  }
}