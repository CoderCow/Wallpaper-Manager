using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

using Common;
using Common.Windows;

using WallpaperManager.Application;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Contains general configuration data.
  /// </summary>
  /// <seealso cref="Configuration">Configuration Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class GeneralConfig: ICloneable, IAssignable, INotifyPropertyChanged {
    #region Constants: AutostartEntryRegKeyName
    /// <summary>
    ///   Represents the value name of the autostart registry key.
    /// </summary>
    private const String AutostartEntryRegKeyName = "Wallpaper Manager";
    #endregion

    #region Property: StartWithWindows
    // TODO: Document and handle necessary exceptions raised by this property.
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application will start when the user logs in.
    /// </value>
    /// <remarks>
    ///   This value is not saved into the configuration file. Its directly accessed from the registry.
    /// </remarks>
    public Boolean StartWithWindows {
      get { return Autostart.CurrentUserEntries.ContainsKey(GeneralConfig.AutostartEntryRegKeyName); }
      set {
        if (value) {
          // Make sure the entry really doesn't exist.
          if (!this.StartWithWindows) {
            Autostart.CurrentUserEntries.Add(GeneralConfig.AutostartEntryRegKeyName, Assembly.GetExecutingAssembly().Location);
          }
        } else {
          // If the entry does exist.
          if (this.StartWithWindows) {
            Autostart.CurrentUserEntries.Remove(GeneralConfig.AutostartEntryRegKeyName);
          }
        }
      }
    }
    #endregion

    #region Property: CycleAfterStartup
    /// <summary>
    ///   <inheritdoc cref="CycleAfterStartup" select='../value/node()' />
    /// </summary>
    private Boolean cycleAfterStartup;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been 
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    public Boolean CycleAfterStartup {
      get { return this.cycleAfterStartup; }
      set {
        this.cycleAfterStartup = value;
        this.OnPropertyChanged("CycleAfterStartup");
      }
    }
    #endregion

    #region Property: TerminateAfterStartup
    /// <summary>
    ///   <inheritdoc cref="TerminateAfterStartup" select='../value/node()' />
    /// </summary>
    private Boolean terminateAfterStartup;

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
    public Boolean TerminateAfterStartup {
      get { return this.terminateAfterStartup; }
      set {
        this.terminateAfterStartup = value;
        this.OnPropertyChanged("TerminateAfterStartup");
      }
    }
    #endregion

    #region Property: MinimizeAfterStartup
    /// <summary>
    ///   <inheritdoc cref="MinimizeAfterStartup" select='../value/node()' />
    /// </summary>
    private Boolean minimizeAfterStartup;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main view will be shown after startup.
    /// </value>
    public Boolean MinimizeAfterStartup {
      get { return this.minimizeAfterStartup; }
      set {
        this.minimizeAfterStartup = value;
        this.OnPropertyChanged("MinimizeAfterStartup");
      }
    }
    #endregion

    #region Property: StartAutocyclingAfterStartup
    /// <summary>
    ///   <inheritdoc cref="StartAutocyclingAfterStartup" select='../value/node()' />
    /// </summary>
    private Boolean startAutocyclingAfterStartup;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    public Boolean StartAutocyclingAfterStartup {
      get { return this.startAutocyclingAfterStartup; }
      set {
        this.startAutocyclingAfterStartup = value;
        this.OnPropertyChanged("startAutocyclingAfterStartup");
      }
    }
    #endregion

    #region Property: WallpaperChangeType
    /// <summary>
    ///   <inheritdoc cref="WallpaperChangeType" select='../value/node()' />
    /// </summary>
    private WallpaperChangeType wallpaperChangeType;

    /// <inheritdoc cref="WallpaperChanger.WallpaperChangeType" />
    public WallpaperChangeType WallpaperChangeType {
      get { return this.wallpaperChangeType; }
      set {
        if (!Enum.IsDefined(typeof(WallpaperChangeType), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(WallpaperChangeType), value));
        }

        this.wallpaperChangeType = value;
        this.OnPropertyChanged("WallpaperChangeType");
      }
    }
    #endregion

    #region Property: AutocycleInterval
    /// <summary>
    ///   <inheritdoc cref="AutocycleInterval" select='../value/node()' />
    /// </summary>
    private TimeSpan autocycleInterval;

    /// <inheritdoc cref="WallpaperChanger.AutocycleInterval" />
    public TimeSpan AutocycleInterval {
      get { return this.autocycleInterval; }
      set {
        if (value.TotalSeconds < WallpaperChanger.MinAutocycleIntervalSeconds) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueMustBeGreaterThanValue(
            value.ToString(), WallpaperChanger.MinAutocycleIntervalSeconds.ToString(CultureInfo.CurrentCulture)
          ));
        }

        this.autocycleInterval = value;
        this.OnPropertyChanged("AutocycleInterval");
      }
    }
    #endregion

    #region Property: LastActiveListSize
    /// <summary>
    ///   <inheritdoc cref="LastActiveListSize" select='../value/node()' />
    /// </summary>
    private Byte lastActiveListSize;

    /// <inheritdoc cref="WallpaperChanger.LastActiveListSize" />
    public Byte LastActiveListSize {
      get { return this.lastActiveListSize; }
      set {
        if (!value.IsBetween((Byte)1, WallpaperChanger.LastActiveListSizeMax)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueOutOfRange(
            null, value, 
            1.ToString(CultureInfo.CurrentCulture), 
            WallpaperChanger.LastActiveListSizeMax.ToString(CultureInfo.CurrentCulture)
          ));
        }

        this.lastActiveListSize = value;
      }
    }
    #endregion

    #region Property: CycleAfterDisplaySettingsChanged
    /// <summary>
    ///   <inheritdoc cref="CycleAfterDisplaySettingsChanged" select='../value/node()' />
    /// </summary>
    private Boolean cycleAfterDisplaySettingsChanged;

    /// <inheritdoc cref="WallpaperChanger.CycleAfterDisplaySettingsChanged" />
    public Boolean CycleAfterDisplaySettingsChanged {
      get { return this.cycleAfterDisplaySettingsChanged; }
      set {
        this.cycleAfterDisplaySettingsChanged = value;
        this.OnPropertyChanged("CycleAfterDisplaySettingsChanged");
      }
    }
    #endregion

    #region Property: MinimizeOnClose
    /// <summary>
    ///   <inheritdoc cref="MinimizeOnClose" select='../value/node()' />
    /// </summary>
    private Boolean minimizeOnClose;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the 
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the main window will always be minimized and be shown in the 
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    public Boolean MinimizeOnClose {
      get { return this.minimizeOnClose; }
      set {
        this.minimizeOnClose = value;
        this.OnPropertyChanged("MinimizeOnClose");
      }
    }
    #endregion

    #region Property: DisplayCycleTimeAsIconOverlay
    /// <summary>
    ///   <inheritdoc cref="DisplayCycleTimeAsIconOverlay" select='../value/node()' />
    /// </summary>
    private Boolean displayCycleTimeAsIconOverlay;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle 
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether to show the remaining time for the next random cycle as overlay icon 
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    public Boolean DisplayCycleTimeAsIconOverlay {
      get { return this.displayCycleTimeAsIconOverlay; }
      set {
        this.displayCycleTimeAsIconOverlay = value;
        this.OnPropertyChanged("DisplayCycleTimeAsIconOverlay");
      }
    }
    #endregion

    #region Property: WallpaperDoubleClickAction
    /// <summary>
    ///   <inheritdoc cref="WallpaperDoubleClickAction" select='../value/node()' />
    /// </summary>
    private WallpaperClickAction wallpaperDoubleClickAction;

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    public WallpaperClickAction WallpaperDoubleClickAction {
      get { return this.wallpaperDoubleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(WallpaperClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(WallpaperClickAction), value));
        }

        this.wallpaperDoubleClickAction = value;
        this.OnPropertyChanged("WallpaperDoubleClickAction");
      }
    }
    #endregion

    #region Property: TrayIconSingleClickAction
    /// <summary>
    ///   <inheritdoc cref="TrayIconSingleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconSingleClickAction;

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconSingleClickAction {
      get { return this.trayIconSingleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(TrayIconClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(TrayIconClickAction), value));
        }

        this.trayIconSingleClickAction = value;
        this.OnPropertyChanged("TrayIconSingleClickAction");
      }
    }
    #endregion

    #region Property: TrayIconDoubleClickAction
    /// <summary>
    ///   <inheritdoc cref="TrayIconDoubleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconDoubleClickAction;

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconDoubleClickAction {
      get { return this.trayIconDoubleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(TrayIconClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(TrayIconClickAction), value));
        }

        this.trayIconDoubleClickAction = value;
        this.OnPropertyChanged("TrayIconDoubleClickAction");
      }
    }
    #endregion

    #region Property: ScreensSettings
    /// <summary>
    ///   <inheritdoc cref="ScreensSettings" select='../value/node()' />
    /// </summary>
    private ScreenSettingsCollection screensSettings;

    /// <inheritdoc cref="WallpaperChanger.ScreensSettings" />
    public ScreenSettingsCollection ScreensSettings {
      get { return this.screensSettings; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        this.screensSettings = value;
        this.OnPropertyChanged("ScreensSettings");
      }
    }
    #endregion


    #region Method: Constructor
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
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual Object Clone() {
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
    public virtual void AssignTo(Object other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      GeneralConfig otherInstance = (other as GeneralConfig);
      if (otherInstance == null) {
        throw new ArgumentException(ExceptionMessages.GetTypeIsNotCastable("Object", "GeneralConfig", "other"));
      }

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
    protected virtual void OnPropertyChanged(String propertyName) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}