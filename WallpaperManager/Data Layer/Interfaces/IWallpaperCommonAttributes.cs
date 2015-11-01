using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Defines common wallpaper attributes.
  /// </summary>
  public interface IWallpaperCommonAttributes {
    #region Property: IsActivated
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether this wallpaper is activated.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this wallpaper is activated.
    /// </value>
    /// <remarks>
    ///   The activated status of a wallpaper usually indicates if it should be automatically cycled or not.
    /// </remarks>
    Boolean IsActivated { get; set; }
    #endregion

    #region Property: IsMultiscreen
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this wallpaper represents a wallpaper for multiple screens.
    /// </value>
    Boolean IsMultiscreen { get; set; }
    #endregion

    #region Property: Priority
    /// <summary>
    ///   Gets or sets the pick priority of this wallpaper.
    /// </summary>
    /// <value>
    ///   The pick priority of this wallpaper.
    /// </value>
    /// <remarks>
    ///   The pick priority usually represents the chance for the wallpaper of being automatically picked when cycling.
    /// </remarks>
    Byte Priority { get; set; }
    #endregion

    #region Property: OnlyCycleBetweenStart
    /// <summary>
    ///   Gets or sets the start time of the range in which this wallpaper should only be cycled.
    /// </summary>
    /// <value>
    ///   The start time of the range in which this wallpaper should only be cycled.
    /// </value>
    /// <seealso cref="OnlyCycleBetweenStop">OnlyCycleBetweenStop Property</seealso>
    TimeSpan OnlyCycleBetweenStart { get; set; }
    #endregion

    #region Property: OnlyCycleBetweenStop
    /// <summary>
    ///   Gets or sets the end time of the range in which this wallpaper should only be cycled.
    /// </summary>
    /// <value>
    ///   The end time of the range in which this wallpaper should only be cycled.
    /// </value>
    /// <seealso cref="OnlyCycleBetweenStart">OnlyCycleBetweenStart Property</seealso>
    TimeSpan OnlyCycleBetweenStop { get; set; }
    #endregion

    #region Property: Placement
    /// <summary>
    ///   Gets or sets a value defining how this wallpaper should be placed when drawn on a screen.
    /// </summary>
    /// <value>
    ///   A value defining how this wallpaper should be placed when drawn on a screen.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is not represented by a constant of the <see cref="WallpaperPlacement" /> enumeration.
    /// </exception>
    WallpaperPlacement Placement { get; set; }
    #endregion

    #region Property: Offset
    /// <summary>
    ///   Gets or sets the horizontal and vertical placement offset the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The horizontal and vertical placement offset the wallpaper should be drawn with.
    /// </value>
    Point Offset { get; set; }
    #endregion

    #region Property: Scale
    /// <summary>
    ///   Gets or sets the horizontal and vertical scale the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The horizontal and vertical scale the wallpaper should be drawn with.
    /// </value>
    Point Scale { get; set; }
    #endregion

    #region Property: Effects
    /// <summary>
    ///   Gets or sets the effects the wallpaper should be drawn with.
    /// </summary>
    /// <value>
    ///   The effects the wallpaper should be drawn with.
    /// </value>
    WallpaperEffects Effects { get; set; }
    #endregion

    #region Property: BackgroundColor
    /// <summary>
    ///   Gets or sets the background color drawn for this wallpaper if it does not fill out the whole screen.
    /// </summary>
    /// <value>
    ///   background color drawn for this wallpaper if it does not fill out the whole screen.
    /// </value>
    Color BackgroundColor { get; set; }
    #endregion

    #region Property: DisabledScreens
    /// <summary>
    ///   Gets a collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </summary>
    /// <value>
    ///   A collection of screen indexes where this wallpaper is not allowed to be cycled on.
    /// </value>
    Collection<Int32> DisabledScreens { get; }
    #endregion
  }
}