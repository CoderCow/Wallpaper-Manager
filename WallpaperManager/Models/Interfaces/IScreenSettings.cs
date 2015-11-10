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
  ///   Defines screen related configuration data.
  /// </summary>
  [ContractClass(typeof(IScreenSettingsContracts))]
  public interface IScreenSettings: ICloneable, IAssignable {
    /// <summary>
    ///   Gets the index of the screen of which this instance defines settings for.
    /// </summary>
    /// <value>
    ///   The index of the screen of which this instance defines settings for.
    /// </value>
    int Index { get; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </value>
    bool CycleRandomly { get; set; }

    /// <summary>
    ///   Gets the static <see cref="Wallpaper" /> which is used if <see cref="CycleRandomly" /> is set to <c>false</c>.
    /// </summary>
    /// <value>
    ///   The static <see cref="Wallpaper" /> which is used if <see cref="CycleRandomly" /> is <c>false</c>.
    /// </value>
    /// <seealso cref="CycleRandomly">CycleRandomly Property</seealso>
    /// <seealso cref="IWallpaper">Wallpaper Class</seealso>
    IWallpaper StaticWallpaper { get; set; }

    /// <summary>
    ///   Gets or sets the left border value in pixels.
    /// </summary>
    /// <value>
    ///   The left border value in pixels.
    /// </value>
    int MarginLeft { get; set; }

    /// <summary>
    ///   Gets or sets the right border value in pixels.
    /// </summary>
    /// <value>
    ///   The right border value in pixels.
    /// </value>
    int MarginRight { get; set; }

    /// <summary>
    ///   Gets or sets the top border value in pixels.
    /// </summary>
    /// <value>
    ///   The top border value in pixels.
    /// </value>
    int MarginTop { get; set; }

    /// <summary>
    ///   Gets or sets the bottom border value in pixels.
    /// </summary>
    /// <value>
    ///   The bottom border value in pixels.
    /// </value>
    int MarginBottom { get; set; }

    /// <summary>
    ///   Gets the bounds of the assigned screen.
    /// </summary>
    /// <value>
    ///   The bounds of the assigned screen.
    /// </value>
    Rectangle Bounds { get; }

    /// <summary>
    ///   Gets the bounds of the assigned screen with their margin substracted.
    /// </summary>
    /// <value>
    ///   The bounds of the assigned screen with their margin substracted.
    /// </value>
    Rectangle BoundsWithMargin { get; }

    /// <summary>
    ///   Gets the collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </value>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    ObservableCollection<IWallpaperTextOverlay> TextOverlays { get; }
  }

  [ContractClassFor(typeof(IScreenSettings))]
  internal abstract class IScreenSettingsContracts : IScreenSettings {
    public abstract int Index { get; }
    public abstract bool CycleRandomly { get; set; }
    public abstract IWallpaper StaticWallpaper { get; set; }
    public abstract int MarginLeft { get; set; }
    public abstract int MarginRight { get; set; }
    public abstract int MarginTop { get; set; }
    public abstract int MarginBottom { get; set; }
    public abstract Rectangle Bounds { get; }
    public abstract Rectangle BoundsWithMargin { get; }
    public abstract ObservableCollection<IWallpaperTextOverlay> TextOverlays { get; }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.Index.IsBetween(0, Screen.AllScreens.Length - 1));
      Contract.Invariant(this.StaticWallpaper != null);
      Contract.Invariant(this.TextOverlays != null);
      Contract.Invariant(this.TextOverlays != null);
      Contract.Invariant(this.Bounds.Width > 0 && this.Bounds.Height > 0);
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}