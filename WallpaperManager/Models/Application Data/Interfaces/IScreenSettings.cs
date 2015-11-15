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
    ///   Gets the collection of <see cref="TextOverlay" /> objects which should be applied on this screen.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="TextOverlay" /> objects which should be applied on this screen.
    /// </value>
    /// <seealso cref="TextOverlay">TextOverlay Class</seealso>
    ObservableCollection<ITextOverlay> TextOverlays { get; }
  }

  [ContractClassFor(typeof(IScreenSettings))]
  internal abstract class IScreenSettingsContracts : IScreenSettings {
    public abstract bool CycleRandomly { get; set; }
    public abstract IWallpaper StaticWallpaper { get; set; }
    public abstract int MarginLeft { get; set; }
    public abstract int MarginRight { get; set; }
    public abstract int MarginTop { get; set; }
    public abstract int MarginBottom { get; set; }

    public ObservableCollection<ITextOverlay> TextOverlays {
      get {
        Contract.Ensures(Contract.Result<ObservableCollection<ITextOverlay>>() != null);
        throw new NotImplementedException();
      }
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}