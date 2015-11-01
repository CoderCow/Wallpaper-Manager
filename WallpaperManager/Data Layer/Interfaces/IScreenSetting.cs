using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Defines screen related configuration data.
  /// </summary>
  public interface IScreenSetting {
    #region Property: Index
    /// <summary>
    ///   Gets the index of the screen of which this instance defines settings for.
    /// </summary>
    /// <value>
    ///   The index of the screen of which this instance defines settings for.
    /// </value>
    Int32 Index { get; }
    #endregion

    #region Property: CycleRandomly
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether wallpapers will be cycled randomly on this screen or not.
    /// </value>
    Boolean CycleRandomly { get; set; }
    #endregion

    #region Property: StaticWallpaper
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
    IWallpaper StaticWallpaper { get; set; }
    #endregion

    #region Property: Margins
    /// <summary>
    ///   Gets the margin definitions for this screen.
    /// </summary>
    /// <value>
    ///   The margin definitions for this screen.
    /// </value>
    /// <seealso cref="ScreenMargins">ScreenMargins Class</seealso>
    ScreenMargins Margins { get; }
    #endregion

    #region Properties: Bounds, BoundsWithMargin
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
    #endregion

    #region Property: TextOverlays
    /// <summary>
    ///   Gets the collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="WallpaperTextOverlay" /> objects which should be applied on this screen.
    /// </value>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    ObservableCollection<WallpaperTextOverlay> TextOverlays { get; }
    #endregion
  }
}