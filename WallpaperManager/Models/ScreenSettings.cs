// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains settings related to a screen.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ScreenSettings : ValidatableBase, IScreenSettings, ICloneable, IAssignable {
    /// <inheritdoc />
    public int Index { get; }

    /// <inheritdoc />
    public bool CycleRandomly { get; set; }

    /// <inheritdoc />
    public IWallpaper StaticWallpaper { get; set; }

    /// <inheritdoc />
    public int MarginLeft { get; set; }

    /// <inheritdoc />
    public int MarginRight { get; set; }

    /// <inheritdoc />
    public int MarginTop { get; set; }

    /// <inheritdoc />
    public int MarginBottom { get; set; }

    /// <inheritdoc />
    public Rectangle Bounds { get; private set; }

    /// <inheritdoc />
    public Rectangle BoundsWithMargin { get; private set; }

    /// <inheritdoc />
    public ObservableCollection<IWallpaperTextOverlay> TextOverlays { get; private set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenSettings" /> class for the screen with the given index.
    /// </summary>
    /// <param name="index">
    ///   The index of the screen of which this instance defines settings for.
    /// </param>
    public ScreenSettings(int index) {
      this.Index = index;
      this.CycleRandomly = true;
      this.StaticWallpaper = null;
      
      // Cache the Bounds and BoundsWithMargin rectangles for this first time.
      this.RefreshBounds();

      this.TextOverlays = new ObservableCollection<IWallpaperTextOverlay>();
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.BoundsWithMargin) || propertyName == nameof(this.MarginLeft) || propertyName == nameof(this.MarginRight))
        if (this.BoundsWithMargin.Width <= 0)
          return "The horizontal margins for this screen are too limiting.";
      else if (propertyName == nameof(this.BoundsWithMargin) || propertyName == nameof(this.MarginTop) || propertyName == nameof(this.MarginBottom))
        if (this.BoundsWithMargin.Height <= 0)
          return "The vertical margins for this screen are too limiting.";
      
      return null;
    }
    #endregion

    /// <summary>
    ///   Refreshes the cached <see cref="Bounds" /> and recalculates <see cref="BoundsWithMargin" />.
    /// </summary>
    public void RefreshBounds() {
      this.Bounds = Screen.AllScreens[this.Index].Bounds;
      this.BoundsWithMargin = new Rectangle(
        this.Bounds.X + this.MarginLeft,
        this.Bounds.Y + this.MarginTop,
        this.Bounds.Width - this.MarginRight - this.MarginLeft,
        this.Bounds.Height - this.MarginBottom - this.MarginTop);
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

      var overlayTextCollection = new ObservableCollection<IWallpaperTextOverlay>();
      foreach (IWallpaperTextOverlay overlayText in this.TextOverlays)
        overlayTextCollection.Add((IWallpaperTextOverlay)overlayText.Clone());

        clone.TextOverlays = overlayTextCollection;

      return clone;
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentNullException>(other != null);
      Contract.Requires<ArgumentException>(other is ScreenSettings);

      ScreenSettings otherInstance = (ScreenSettings)other;
      otherInstance.CycleRandomly = this.CycleRandomly;
      this.StaticWallpaper?.AssignTo(otherInstance.StaticWallpaper);
      otherInstance.MarginLeft = this.MarginLeft;
      otherInstance.MarginTop = this.MarginTop;
      otherInstance.MarginRight = this.MarginRight;
      otherInstance.MarginBottom = this.MarginBottom;

      otherInstance.TextOverlays.Clear();
      for (int i = 0; i < this.TextOverlays.Count; i++)
        otherInstance.TextOverlays.Add(this.TextOverlays[i]);
    }
    #endregion
  }
}