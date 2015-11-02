// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains settings related to a screen.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ScreenSettings : INotifyPropertyChanged, ICloneable, IAssignable, IWeakEventListener {
    /// <summary>
    ///   <inheritdoc cref="CycleRandomly" select='../value/node()' />
    /// </summary>
    private bool cycleRandomly;

    /// <summary>
    ///   <inheritdoc cref="StaticWallpaper" select='../value/node()' />
    /// </summary>
    private Wallpaper staticWallpaper;

    /// <summary>
    ///   Gets the index of the screen of which this instance defines settings for.
    /// </summary>
    /// <value>
    ///   The index of the screen of which this instance defines settings for.
    /// </value>
    public int Index { get; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </value>
    public bool CycleRandomly {
      get { return this.cycleRandomly; }
      set {
        this.cycleRandomly = value;
        this.OnPropertyChanged("CycleRandomly");
      }
    }

    /// <summary>
    ///   Gets the static <see cref="Wallpaper" /> which is used if <see cref="CycleRandomly" /> is set to <c>false</c>.
    /// </summary>
    /// <value>
    ///   The static <see cref="Wallpaper" /> which is used if <see cref="CycleRandomly" /> is <c>false</c>.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="CycleRandomly">CycleRandomly Property</seealso>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public Wallpaper StaticWallpaper {
      get { return this.staticWallpaper; }
      set {
        this.staticWallpaper = value;
        this.OnPropertyChanged("StaticWallpaper");
      }
    }

    /// <summary>
    ///   Gets the margin definitions for this screen.
    /// </summary>
    /// <value>
    ///   The margin definitions for this screen.
    /// </value>
    /// <seealso cref="ScreenMargins">ScreenMargins Class</seealso>
    public ScreenMargins Margins { get; private set; }

    /// <summary>
    ///   Gets the bounds of the assigned screen.
    /// </summary>
    /// <value>
    ///   The bounds of the assigned screen.
    /// </value>
    public Rectangle Bounds { get; private set; }

    /// <summary>
    ///   Gets the bounds of the assigned screen with their margin substracted.
    /// </summary>
    /// <value>
    ///   The bounds of the assigned screen with their margin substracted.
    /// </value>
    public Rectangle BoundsWithMargin { get; private set; }

    /// <summary>
    ///   Gets the collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </value>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    public ObservableCollection<WallpaperTextOverlay> TextOverlays { get; private set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenSettings" /> class for the screen with the given index.
    /// </summary>
    /// <param name="index">
    ///   The index of the screen of which this instance defines settings for.
    /// </param>
    public ScreenSettings(int index) {
      this.Index = index;
      this.cycleRandomly = true;
      this.staticWallpaper = null;
      this.staticWallpaper = new Wallpaper();

      this.Margins = new ScreenMargins();
      PropertyChangedEventManager.AddListener(this.Margins, this, string.Empty);

      // Cache the Bounds and BoundsWithMargin rectangles for this first time.
      this.RefreshBounds();

      this.TextOverlays = new ObservableCollection<WallpaperTextOverlay>();
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        this.Margins_PropertyChanged(sender, (PropertyChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion

    /// <summary>
    ///   Refreshes the cached <see cref="Bounds" /> and recalculates <see cref="BoundsWithMargin" />.
    /// </summary>
    public void RefreshBounds() {
      this.Bounds = Screen.AllScreens[this.Index].Bounds;
      this.Margins_PropertyChanged(this, null);
    }

    /// <summary>
    ///   Handles the <see cref="ScreenMargins.PropertyChanged" /> event of the <see cref="ScreenMargins" /> instance.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Margins_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      this.BoundsWithMargin = new Rectangle(
        this.Bounds.X + this.Margins.Left,
        this.Bounds.Y + this.Margins.Top,
        this.Bounds.Width - this.Margins.Right - this.Margins.Left,
        this.Bounds.Height - this.Margins.Bottom - this.Margins.Top);
    }

    /// <inheritdoc />
    public override string ToString() {
      return StringGenerator.FromListKeyed(
        new[] {"Cycle Randomly", "Margins", "Static Wallpaper"},
        (IList<object>)new object[] {this.CycleRandomly, this.Margins, this.StaticWallpaper});
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual object Clone() {
      var overlayTextCollection = new ObservableCollection<WallpaperTextOverlay>();
      foreach (WallpaperTextOverlay overlayText in this.TextOverlays)
        overlayTextCollection.Add((WallpaperTextOverlay)overlayText.Clone());

      return new ScreenSettings(this.Index) {
        cycleRandomly = this.CycleRandomly,
        staticWallpaper = (Wallpaper)this.StaticWallpaper.Clone(),
        Margins = (ScreenMargins)this.Margins.Clone(),
        BoundsWithMargin = this.BoundsWithMargin,
        TextOverlays = overlayTextCollection
      };
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
    ///   <paramref name="other" /> is not castable to the <see cref="ScreenSettings" /> type.
    /// </exception>
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentNullException>(other != null);
      Contract.Requires<ArgumentException>(other is ScreenSettings);

      ScreenSettings otherInstance = (ScreenSettings)other;
      otherInstance.CycleRandomly = this.CycleRandomly;
      this.StaticWallpaper.AssignTo(otherInstance.staticWallpaper);
      this.Margins.AssignTo(otherInstance.Margins);

      otherInstance.TextOverlays.Clear();
      for (int i = 0; i < this.TextOverlays.Count; i++)
        otherInstance.TextOverlays.Add(this.TextOverlays[i]);
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}