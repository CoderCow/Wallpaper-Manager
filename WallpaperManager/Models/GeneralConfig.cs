// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using Common;
using Common.Windows;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains general configuration data.
  /// </summary>
  /// <seealso cref="Configuration">Configuration Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class GeneralConfig : ICloneable, IAssignable, INotifyPropertyChanged {
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

    /// <summary>
    ///   <inheritdoc cref="AutocycleInterval" select='../value/node()' />
    /// </summary>
    private TimeSpan autocycleInterval;

    /// <summary>
    ///   <inheritdoc cref="CycleAfterDisplaySettingsChanged" select='../value/node()' />
    /// </summary>
    private bool cycleAfterDisplaySettingsChanged;

    /// <summary>
    ///   <inheritdoc cref="CycleAfterStartup" select='../value/node()' />
    /// </summary>
    private bool cycleAfterStartup;

    /// <summary>
    ///   <inheritdoc cref="DisplayCycleTimeAsIconOverlay" select='../value/node()' />
    /// </summary>
    private bool displayCycleTimeAsIconOverlay;

    /// <summary>
    ///   <inheritdoc cref="LastActiveListSize" select='../value/node()' />
    /// </summary>
    private byte lastActiveListSize;

    /// <summary>
    ///   <inheritdoc cref="MinimizeAfterStartup" select='../value/node()' />
    /// </summary>
    private bool minimizeAfterStartup;

    /// <summary>
    ///   <inheritdoc cref="MinimizeOnClose" select='../value/node()' />
    /// </summary>
    private bool minimizeOnClose;

    /// <summary>
    ///   <inheritdoc cref="ScreensSettings" select='../value/node()' />
    /// </summary>
    private ScreenSettingsCollection screensSettings;

    /// <summary>
    ///   <inheritdoc cref="StartAutocyclingAfterStartup" select='../value/node()' />
    /// </summary>
    private bool startAutocyclingAfterStartup;

    /// <summary>
    ///   <inheritdoc cref="TerminateAfterStartup" select='../value/node()' />
    /// </summary>
    private bool terminateAfterStartup;

    /// <summary>
    ///   <inheritdoc cref="TrayIconDoubleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconDoubleClickAction;

    /// <summary>
    ///   <inheritdoc cref="TrayIconSingleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconSingleClickAction;

    /// <summary>
    ///   <inheritdoc cref="WallpaperChangeType" select='../value/node()' />
    /// </summary>
    private WallpaperChangeType wallpaperChangeType;

    /// <summary>
    ///   <inheritdoc cref="WallpaperDoubleClickAction" select='../value/node()' />
    /// </summary>
    private WallpaperClickAction wallpaperDoubleClickAction;

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
      get { return Autostart.CurrentUserEntries.ContainsKey(GeneralConfig.AutostartEntryRegKeyName); }
      set {
        if (value) {
          // Make sure the entry really doesn't exist.
          if (!this.StartWithWindows)
            Autostart.CurrentUserEntries.Add(GeneralConfig.AutostartEntryRegKeyName, Assembly.GetExecutingAssembly().Location);
        } else {
          // If the entry does exist.
          if (this.StartWithWindows)
            Autostart.CurrentUserEntries.Remove(GeneralConfig.AutostartEntryRegKeyName);
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
    public bool CycleAfterStartup {
      get { return this.cycleAfterStartup; }
      set {
        this.cycleAfterStartup = value;
        this.OnPropertyChanged("CycleAfterStartup");
      }
    }

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
    public bool TerminateAfterStartup {
      get { return this.terminateAfterStartup; }
      set {
        this.terminateAfterStartup = value;
        this.OnPropertyChanged("TerminateAfterStartup");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </value>
    public bool MinimizeAfterStartup {
      get { return this.minimizeAfterStartup; }
      set {
        this.minimizeAfterStartup = value;
        this.OnPropertyChanged("MinimizeAfterStartup");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    public bool StartAutocyclingAfterStartup {
      get { return this.startAutocyclingAfterStartup; }
      set {
        this.startAutocyclingAfterStartup = value;
        this.OnPropertyChanged("startAutocyclingAfterStartup");
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    public WallpaperChangeType WallpaperChangeType {
      get { return this.wallpaperChangeType; }
      set {
        this.wallpaperChangeType = value;
        this.OnPropertyChanged("WallpaperChangeType");
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    public TimeSpan AutocycleInterval {
      get { return this.autocycleInterval; }
      set {
        this.autocycleInterval = value;
        this.OnPropertyChanged("AutocycleInterval");
      }
    }

    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    public byte LastActiveListSize {
      get { return this.lastActiveListSize; }
      set {
        this.lastActiveListSize = value;
        this.OnPropertyChanged("LastActiveListSize");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    public bool CycleAfterDisplaySettingsChanged {
      get { return this.cycleAfterDisplaySettingsChanged; }
      set {
        this.cycleAfterDisplaySettingsChanged = value;
        this.OnPropertyChanged("CycleAfterDisplaySettingsChanged");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    public bool MinimizeOnClose {
      get { return this.minimizeOnClose; }
      set {
        this.minimizeOnClose = value;
        this.OnPropertyChanged("MinimizeOnClose");
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether to show the remaining time for the next random cycle
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether to show the remaining time for the next random cycle as overlay icon
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    public bool DisplayCycleTimeAsIconOverlay {
      get { return this.displayCycleTimeAsIconOverlay; }
      set {
        this.displayCycleTimeAsIconOverlay = value;
        this.OnPropertyChanged("DisplayCycleTimeAsIconOverlay");
      }
    }

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    public WallpaperClickAction WallpaperDoubleClickAction {
      get { return this.wallpaperDoubleClickAction; }
      set {
        this.wallpaperDoubleClickAction = value;
        this.OnPropertyChanged("WallpaperDoubleClickAction");
      }
    }

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconSingleClickAction {
      get { return this.trayIconSingleClickAction; }
      set {
        this.trayIconSingleClickAction = value;
        this.OnPropertyChanged("TrayIconSingleClickAction");
      }
    }

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconDoubleClickAction {
      get { return this.trayIconDoubleClickAction; }
      set {
        this.trayIconDoubleClickAction = value;
        this.OnPropertyChanged("TrayIconDoubleClickAction");
      }
    }

    /// <summary>
    ///   Gets or sets a collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public ScreenSettingsCollection ScreensSettings {
      get { return this.screensSettings; }
      set {
        this.screensSettings = value;
        this.OnPropertyChanged("ScreensSettings");
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="GeneralConfig" /> class.
    /// </summary>
    public GeneralConfig() {
      this.cycleAfterStartup = false;
      this.terminateAfterStartup = false;
      this.startAutocyclingAfterStartup = false;
      this.minimizeOnClose = false;
      this.cycleAfterDisplaySettingsChanged = false;
      this.autocycleInterval = new TimeSpan(0, 0, 30);
      this.lastActiveListSize = 30;
      this.wallpaperChangeType = WallpaperChangeType.ChangeAll;
      this.wallpaperDoubleClickAction = WallpaperClickAction.ShowConfigurationWindow;
      this.trayIconSingleClickAction = TrayIconClickAction.NoAction;
      this.trayIconDoubleClickAction = TrayIconClickAction.ShowMainWindow;
      this.screensSettings = new ScreenSettingsCollection();
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual object Clone() {
      return new GeneralConfig() {
        CycleAfterStartup = this.CycleAfterStartup,
        TerminateAfterStartup = this.TerminateAfterStartup,
        StartAutocyclingAfterStartup = this.StartAutocyclingAfterStartup,
        MinimizeAfterStartup = this.MinimizeAfterStartup,
        WallpaperChangeType = this.WallpaperChangeType,
        AutocycleInterval = this.AutocycleInterval,
        LastActiveListSize = this.LastActiveListSize,
        CycleAfterDisplaySettingsChanged = this.CycleAfterDisplaySettingsChanged,
        ScreensSettings = (ScreenSettingsCollection)this.ScreensSettings.Clone(),
        MinimizeOnClose = this.MinimizeOnClose,
        DisplayCycleTimeAsIconOverlay = this.DisplayCycleTimeAsIconOverlay,
        WallpaperDoubleClickAction = this.WallpaperDoubleClickAction,
        TrayIconSingleClickAction = this.TrayIconSingleClickAction,
        TrayIconDoubleClickAction = this.TrayIconDoubleClickAction
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
    ///   <paramref name="other" /> is not castable to the <see cref="GeneralConfig" /> type.
    /// </exception>
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentNullException>(other != null);
      Contract.Requires<ArgumentException>(other is GeneralConfig);

      GeneralConfig otherInstance = (GeneralConfig)other;
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
      otherInstance.ScreensSettings = (ScreenSettingsCollection)this.ScreensSettings.Clone();
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