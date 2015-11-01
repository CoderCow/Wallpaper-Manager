// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Defines general application configuration data.
  /// </summary>
  public interface IGeneralConfiguration {
    #region Property: StartWithWindows
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </value>
    /// <remarks>
    ///   This value is not saved into the configuration file. Its directly accessed from the registry.
    /// </remarks>
    Boolean StartWithWindows { get; set; }
    #endregion

    #region Property: CycleAfterStartup
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been 
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    Boolean CycleAfterStartup { get; set; }
    #endregion

    #region Property: TerminateAfterStartup
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application should terminate right after it had been 
    ///   started.
    /// </summary>
    /// <remarks>
    ///   If <c>true</c>, this property requires <see cref="CycleAfterStartup" /> also to be <c>true</c> to take effect.
    /// </remarks>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application should terminate right after it had been started.
    /// </value>
    Boolean TerminateAfterStartup { get; set; }
    #endregion

    #region Property: MinimizeAfterStartup
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </value>
    Boolean MinimizeAfterStartup { get; set; }
    #endregion

    #region Property: StartAutocyclingAfterStartup
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    Boolean StartAutocyclingAfterStartup { get; set; }
    #endregion

    #region Property: WallpaperChangeType
    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the 
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the change type to a value which is not a constant of 
    ///   <see cref="WallpaperManager.Data.WallpaperChangeType" />.
    /// </exception>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    WallpaperChangeType WallpaperChangeType { get; set; }
    #endregion

    #region Property: AutocycleInterval
    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the interval to a value which is lower than <see cref="GeneralConfig.MinAutocycleIntervalSeconds" />.
    /// </exception>
    TimeSpan AutocycleInterval { get; set; }
    #endregion

    #region Property: LastActiveListSize
    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is not between <c>1</c> and <see cref="GeneralConfig.LastActiveListSizeMax" />.
    /// </exception>
    Byte LastActiveListSize { get; set; }
    #endregion

    #region Property: CycleAfterDisplaySettingsChanged
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings 
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    Boolean CycleAfterDisplaySettingsChanged { get; set; }
    #endregion

    #region Property: MinimizeOnClose
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the 
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the 
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    Boolean MinimizeOnClose { get; set; }
    #endregion

    #region Property: DisplayCycleTimeAsIconOverlay
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle 
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle as overlay icon 
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    Boolean DisplayCycleTimeAsIconOverlay { get; set; }
    #endregion

    #region Property: WallpaperDoubleClickAction
    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    WallpaperClickAction WallpaperDoubleClickAction { get; set; }
    #endregion

    #region Property: TrayIconSingleClickAction
    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    TrayIconClickAction TrayIconSingleClickAction { get; set; }
    #endregion

    #region Property: TrayIconDoubleClickAction
    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    TrayIconClickAction TrayIconDoubleClickAction { get; set; }
    #endregion

    #region Property: ScreensSettings
    /// <summary>
    ///   Gets or sets a collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single 
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    ScreenSettingsCollection ScreensSettings { get; set; }
    #endregion
  }
}