// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines main application configuration data.
  /// </summary>
  [ContractClass(typeof(IConfigurationContracts))]
  public interface IConfiguration {
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </value>
    /// <remarks>
    ///   This value is not saved into the configuration file. Its directly accessed from the registry.
    /// </remarks>
    bool StartWithWindows { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    bool CycleAfterStartup { get; set; }

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
    bool TerminateAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </value>
    bool MinimizeAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    bool StartAutocyclingAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the change type to a value which is not a constant of
    ///   <see cref="Models.WallpaperChangeType" />.
    /// </exception>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    WallpaperChangeType WallpaperChangeType { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the interval to a value which is lower than <see cref="GeneralConfig.MinAutocycleIntervalSeconds" />
    ///   .
    /// </exception>
    TimeSpan AutocycleInterval { get; set; }

    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is not between <c>1</c> and <see cref="GeneralConfig.LastActiveListSizeMax" />.
    /// </exception>
    byte LastActiveListSize { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    bool CycleAfterDisplaySettingsChanged { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    bool MinimizeOnClose { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle as overlay icon
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    bool DisplayCycleTimeAsIconOverlay { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    WallpaperClickAction WallpaperDoubleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    TrayIconClickAction TrayIconSingleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    TrayIconClickAction TrayIconDoubleClickAction { get; set; }

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
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    Dictionary<string, ScreenSettings> ScreensSettings { get; set; }

    /// <summary>
    ///   Gets the <see cref="WallpaperCategoryCollection" /> holding the
    ///   <see cref="WallpaperCategory">Wallpaper wallpaperCategories</see> which's <see cref="Wallpaper" /> instances should
    ///   be cycled.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperCategoryCollection" /> holding the <see cref="WallpaperCategory" /> instances
    ///   which's <see cref="Wallpaper" /> instances should be cycled.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
    WallpaperCategoryCollection WallpaperCategories { get; }
  }

  [ContractClassFor(typeof(IConfiguration))]
  internal abstract class IConfigurationContracts : IConfiguration {
    public abstract bool StartWithWindows { get; set; }
    public abstract bool CycleAfterStartup { get; set; }
    public abstract bool TerminateAfterStartup { get; set; }
    public abstract bool MinimizeAfterStartup { get; set; }
    public abstract bool StartAutocyclingAfterStartup { get; set; }
    public abstract WallpaperChangeType WallpaperChangeType { get; set; }
    public abstract TimeSpan AutocycleInterval { get; set; }
    public abstract byte LastActiveListSize { get; set; }
    public abstract bool CycleAfterDisplaySettingsChanged { get; set; }
    public abstract bool MinimizeOnClose { get; set; }
    public abstract bool DisplayCycleTimeAsIconOverlay { get; set; }
    public abstract WallpaperClickAction WallpaperDoubleClickAction { get; set; }
    public abstract TrayIconClickAction TrayIconSingleClickAction { get; set; }
    public abstract TrayIconClickAction TrayIconDoubleClickAction { get; set; }
    public abstract Dictionary<string, ScreenSettings> ScreensSettings { get; set; }
    public abstract WallpaperCategoryCollection WallpaperCategories { get; }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.AutocycleInterval.TotalSeconds >= Configuration.MinAutocycleIntervalSeconds);
      Contract.Invariant(this.LastActiveListSize.IsBetween(1, Configuration.LastActiveListSizeMax));
      Contract.Invariant(Enum.IsDefined(typeof(WallpaperClickAction), this.WallpaperDoubleClickAction));
      Contract.Invariant(Enum.IsDefined(typeof(WallpaperChangeType), this.WallpaperChangeType));
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconSingleClickAction));
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconDoubleClickAction));
      Contract.Invariant(this.ScreensSettings != null);
      Contract.Invariant(this.WallpaperCategories != null);
    }
  }
}