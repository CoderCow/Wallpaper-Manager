// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Common;
using Common.Windows;

namespace WallpaperManager.Models {
  // TODO: Add zoom functionality
  /// <summary>
  ///   The application's user-defined configuration data holder.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class Configuration : ValidatableBase, IConfiguration, ICloneable, IAssignable {
    /// <summary>
    ///   Represents the version number of the configuration file for backward compatibility.
    /// </summary>
    protected const string DataVersion = "1.2";

    /// <summary>
    ///   Represents the name of the root node of the XML-data.
    /// </summary>
    public const string RootNodeName = "WallpaperManagerConfiguration";

    /// <summary>
    ///   Represents the XML namespace of the root node of the XML-data.
    /// </summary>
    public const string XmlNamespace = "http://www.WallpaperManager.de.vu";

    /// <summary>
    ///   Represents the value name of the autostart registry key.
    /// </summary>
    private const string AutostartEntryRegKeyName = "Wallpaper Manager";

    /// <summary>
    ///   Represents the minimum auto cycle interval value.
    /// </summary>
    /// <seealso cref="AutocycleInterval">AutocycleInterval Property</seealso>
    public const int MinAutocycleIntervalSeconds = 10;

    /// <summary>
    ///   Represents the maxium last active list size value in percentage to the overall count of wallpapers.
    /// </summary>
    /// <seealso cref="LastActiveListSize">LastActiveListSize Property</seealso>
    public const byte LastActiveListSizeMax = 80;

    // TODO: Doesn't really seem to belong here, might as well want to extract this.
    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application will start when the user logs in.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application will start when the user logs in.
    /// </value>
    /// <remarks>
    ///   This value is not saved into the configuration file. Its directly accessed from the registry.
    /// </remarks>
    public bool StartWithWindows {
      get { return Autostart.CurrentUserEntries.ContainsKey(Configuration.AutostartEntryRegKeyName); }
      set {
        if (value) {
          // Make sure the entry really doesn't exist.
          if (!this.StartWithWindows)
            Autostart.CurrentUserEntries.Add(Configuration.AutostartEntryRegKeyName, Assembly.GetExecutingAssembly().Location);
        } else {
          // If the entry does exist.
          if (this.StartWithWindows)
            Autostart.CurrentUserEntries.Remove(Configuration.AutostartEntryRegKeyName);
        }
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application should do one cycle right after it has been
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    public bool CycleAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application should terminate right after it had been
    ///   started.
    /// </summary>
    /// <remarks>
    ///   If <c>true</c>, this property requires <see cref="CycleAfterStartup" /> also to be <c>true</c> to take effect.
    /// </remarks>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application should terminate right after it had been started.
    /// </value>
    public bool TerminateAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </value>
    public bool MinimizeAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    public bool StartAutocyclingAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    public WallpaperChangeType WallpaperChangeType { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    public TimeSpan AutocycleInterval { get; set; }

    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    public byte LastActiveListSize { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    public bool CycleAfterDisplaySettingsChanged { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    public bool MinimizeOnClose { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether to show the remaining time for the next random cycle
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether to show the remaining time for the next random cycle as overlay icon
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    public bool DisplayCycleTimeAsIconOverlay { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    public WallpaperClickAction WallpaperDoubleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconSingleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconDoubleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets a collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public Dictionary<string, ScreenSettings> ScreensSettings { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Configuration" /> class.
    /// </summary>
    public Configuration() {
      this.CycleAfterStartup = false;
      this.TerminateAfterStartup = false;
      this.StartAutocyclingAfterStartup = false;
      this.MinimizeOnClose = false;
      this.CycleAfterDisplaySettingsChanged = false;
      this.AutocycleInterval = new TimeSpan(0, 0, 30);
      this.LastActiveListSize = 30;
      this.WallpaperChangeType = WallpaperChangeType.ChangeAll;
      this.WallpaperDoubleClickAction = WallpaperClickAction.ShowConfigurationWindow;
      this.TrayIconSingleClickAction = TrayIconClickAction.NoAction;
      this.TrayIconDoubleClickAction = TrayIconClickAction.ShowMainWindow;
      this.ScreensSettings = new Dictionary<string, ScreenSettings>();
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.AutocycleInterval))
        if (this.AutocycleInterval.TotalSeconds >= MinAutocycleIntervalSeconds)
          return LocalizationManager.GetLocalizedString("Error.Time.Minimum");
      
      return null;
    }
    #endregion

    /// <inheritdoc />
    public object Clone() {
      Configuration clone = (Configuration)this.MemberwiseClone();

      clone.ScreensSettings = new Dictionary<string, ScreenSettings>();
      foreach (KeyValuePair<string, ScreenSettings> pair in this.ScreensSettings)
        clone.ScreensSettings.Add(pair.Key, pair.Value);

      return clone;
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is Configuration);

      Configuration otherInstance = (Configuration)other;
      otherInstance.CycleAfterStartup = this.CycleAfterStartup;
      otherInstance.TerminateAfterStartup = this.TerminateAfterStartup;
      otherInstance.StartAutocyclingAfterStartup = this.StartAutocyclingAfterStartup;
      otherInstance.MinimizeAfterStartup = this.MinimizeAfterStartup;
      otherInstance.WallpaperChangeType = this.WallpaperChangeType;
      otherInstance.AutocycleInterval = this.AutocycleInterval;
      otherInstance.LastActiveListSize = this.LastActiveListSize;
      otherInstance.CycleAfterDisplaySettingsChanged = this.CycleAfterDisplaySettingsChanged;
      otherInstance.MinimizeOnClose = this.MinimizeOnClose;
      otherInstance.DisplayCycleTimeAsIconOverlay = this.DisplayCycleTimeAsIconOverlay;
      otherInstance.WallpaperDoubleClickAction = this.WallpaperDoubleClickAction;
      otherInstance.TrayIconSingleClickAction = this.TrayIconSingleClickAction;
      otherInstance.TrayIconDoubleClickAction = this.TrayIconDoubleClickAction;
      otherInstance.ScreensSettings = this.ScreensSettings;
    }
  }
}