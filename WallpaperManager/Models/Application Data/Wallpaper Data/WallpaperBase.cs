// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.Serialization;
using Common;
using Common.Presentation;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains general wallpaper related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [DataContract]
  [ImplementPropertyChanged]
  public class WallpaperBase : ValidatableBase, IWallpaperBase, ICloneable, IAssignable {
    /// <summary>
    ///   Gets the default background color used for a new instance of this class.
    /// </summary>
    /// <value>
    ///   The default background color used for a new instance of this class.
    /// </value>
    public static Color DefaultBackgroundColor { get; } = Color.Black;

    /// <inheritdoc />
    [DataMember(Order = 1)]
    public bool IsActivated { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 2)]
    public bool IsMultiscreen { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 3)]
    public byte Priority { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 4)]
    public TimeSpan OnlyCycleBetweenStart { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 5)]
    public TimeSpan OnlyCycleBetweenStop { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 6)]
    public WallpaperPlacement Placement { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 7)]
    public Point Offset { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 8)]
    public Point Scale { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 9)]
    public WallpaperEffects Effects { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 10)]
    public Color BackgroundColor { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 11)]
    public HashSet<string> DisabledDevices { get; protected set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBase" /> class.
    /// </summary>
    public WallpaperBase() {
      this.IsActivated = true;
      this.Priority = 100;
      this.Offset = new Point(0, 0);
      this.Scale = new Point(0, 0);
      this.OnlyCycleBetweenStart = new TimeSpan(0, 0, 0);
      this.OnlyCycleBetweenStop = new TimeSpan(23, 59, 59);
      this.BackgroundColor = DefaultBackgroundColor;
      this.DisabledDevices = new HashSet<string>();
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
    public virtual object Clone() {
      WallpaperBase clone = (WallpaperBase)this.MemberwiseClone();
      clone.DisabledDevices = new HashSet<string>(this.DisabledDevices);

      return clone;
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is WallpaperBase);

      WallpaperBase otherWallpaperBase = (WallpaperBase)other;
      otherWallpaperBase.IsActivated = this.IsActivated;
      otherWallpaperBase.IsMultiscreen = this.IsMultiscreen;
      otherWallpaperBase.Placement = this.Placement;
      otherWallpaperBase.Scale = this.Scale;
      otherWallpaperBase.Offset = this.Offset;
      otherWallpaperBase.Effects = this.Effects;
      otherWallpaperBase.Priority = this.Priority;
      otherWallpaperBase.BackgroundColor = this.BackgroundColor;
      otherWallpaperBase.OnlyCycleBetweenStart = this.OnlyCycleBetweenStart;
      otherWallpaperBase.OnlyCycleBetweenStop = this.OnlyCycleBetweenStop;
      otherWallpaperBase.DisabledDevices = this.DisabledDevices;
    }
    #endregion
  }
}