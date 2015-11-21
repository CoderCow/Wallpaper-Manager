// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains settings related to a screen.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [DataContract]
  [ImplementPropertyChanged]
  public class ScreenSettings : IScreenSettings, ICloneable, IAssignable {
    /// <inheritdoc />
    [DataMember(Order = 1)]
    public bool CycleRandomly { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 2)]
    public int MarginLeft { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 3)]
    public int MarginRight { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 4)]
    public int MarginTop { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 5)]
    public int MarginBottom { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 6)]
    public IWallpaper StaticWallpaper { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 7)]
    public ObservableCollection<ITextOverlay> TextOverlays { get; private set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenSettings" /> class for the screen with the given index.
    /// </summary>
    public ScreenSettings() {
      this.CycleRandomly = true;
      this.TextOverlays = new ObservableCollection<ITextOverlay>();
    }

    /// <inheritdoc />
    public override string ToString()
      => $"{nameof(this.CycleRandomly)}: {this.CycleRandomly}, {nameof(this.StaticWallpaper)}: {(this.StaticWallpaper != null ? this.StaticWallpaper.ToString() : "null")}";

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual object Clone() {
      ScreenSettings clone = (ScreenSettings)this.MemberwiseClone();

      if (this.StaticWallpaper != null)
        clone.StaticWallpaper = (Wallpaper)this.StaticWallpaper.Clone();

      clone.TextOverlays = new ObservableCollection<ITextOverlay>();
      foreach (ITextOverlay overlayText in this.TextOverlays)
        clone.TextOverlays.Add((ITextOverlay)overlayText.Clone());

      return clone;
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentNullException>(other != null);
      Contract.Requires<ArgumentException>(other is ScreenSettings);

      ScreenSettings otherInstance = (ScreenSettings)other;
      otherInstance.CycleRandomly = this.CycleRandomly;
      otherInstance.StaticWallpaper = this.StaticWallpaper;
      otherInstance.MarginLeft = this.MarginLeft;
      otherInstance.MarginTop = this.MarginTop;
      otherInstance.MarginRight = this.MarginRight;
      otherInstance.MarginBottom = this.MarginBottom;
      otherInstance.TextOverlays = this.TextOverlays;
    }
    #endregion
  }
}