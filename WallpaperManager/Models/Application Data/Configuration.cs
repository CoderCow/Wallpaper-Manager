// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Common;
using Common.Presentation;
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
    public const string DataVersion = "1.2";

    /// <summary>
    ///   Represents the minimum auto cycle interval value.
    /// </summary>
    /// <seealso cref="AutocycleInterval">AutocycleInterval Property</seealso>
    public const int MinAutocycleIntervalSeconds = 10;

    public static TimeSpan DefaultAutocycleInterval = new TimeSpan(0, 0, 30);
    public static WallpaperChangeType DefaultWallpaperChangeType = WallpaperChangeType.ChangeAll;
    public static WallpaperClickAction DefaultWallpaperDoubleClickAction = WallpaperClickAction.ShowConfigurationWindow;
    public static TrayIconClickAction DefaultTrayIconSingleClickAction = TrayIconClickAction.NoAction;
    public static TrayIconClickAction DefaultTrayIconDoubleClickAction = TrayIconClickAction.ShowMainWindow;

    /// <inheritdoc />
    public bool CycleAfterStartup { get; set; }

    /// <inheritdoc />
    public bool TerminateAfterStartup { get; set; }

    /// <inheritdoc />
    public bool MinimizeAfterStartup { get; set; }

    /// <inheritdoc />
    public bool StartAutocyclingAfterStartup { get; set; }

    /// <inheritdoc />
    public WallpaperChangeType WallpaperChangeType { get; set; }

    /// <inheritdoc />
    public TimeSpan AutocycleInterval { get; set; }
    
    /// <inheritdoc />
    public bool CycleAfterDisplaySettingsChanged { get; set; }

    /// <inheritdoc />
    public bool MinimizeOnClose { get; set; }

    /// <inheritdoc />
    public bool DisplayCycleTimeAsIconOverlay { get; set; }

    /// <inheritdoc />
    public WallpaperClickAction WallpaperDoubleClickAction { get; set; }

    /// <inheritdoc />
    public TrayIconClickAction TrayIconSingleClickAction { get; set; }

    /// <inheritdoc />
    public TrayIconClickAction TrayIconDoubleClickAction { get; set; }

    /// <inheritdoc />
    public Dictionary<string, ScreenSettings> ScreenSettings { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Configuration" /> class.
    /// </summary>
    public Configuration() {
      this.AutocycleInterval = DefaultAutocycleInterval;
      this.WallpaperChangeType = DefaultWallpaperChangeType;
      this.WallpaperDoubleClickAction = DefaultWallpaperDoubleClickAction;
      this.TrayIconSingleClickAction = DefaultTrayIconSingleClickAction;
      this.TrayIconDoubleClickAction = DefaultTrayIconDoubleClickAction;
      this.ScreenSettings = new Dictionary<string, ScreenSettings>();
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.AutocycleInterval))
        if (this.AutocycleInterval.TotalSeconds < MinAutocycleIntervalSeconds)
          return string.Format(LocalizationManager.GetLocalizedString("Error.Time.Minimum"), MinAutocycleIntervalSeconds);
      
      return null;
    }
    #endregion

    /// <inheritdoc />
    public object Clone() {
      Configuration clone = (Configuration)this.MemberwiseClone();

      clone.ScreenSettings = new Dictionary<string, ScreenSettings>();
      foreach (KeyValuePair<string, ScreenSettings> pair in this.ScreenSettings)
        clone.ScreenSettings.Add(pair.Key, (ScreenSettings)pair.Value.Clone());

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
      otherInstance.CycleAfterDisplaySettingsChanged = this.CycleAfterDisplaySettingsChanged;
      otherInstance.MinimizeOnClose = this.MinimizeOnClose;
      otherInstance.DisplayCycleTimeAsIconOverlay = this.DisplayCycleTimeAsIconOverlay;
      otherInstance.WallpaperDoubleClickAction = this.WallpaperDoubleClickAction;
      otherInstance.TrayIconSingleClickAction = this.TrayIconSingleClickAction;
      otherInstance.TrayIconDoubleClickAction = this.TrayIconDoubleClickAction;
      otherInstance.ScreenSettings = this.ScreenSettings;
    }
  }
}