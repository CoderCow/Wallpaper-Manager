// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines main application configuration data.
  /// </summary>
  [ContractClass(typeof(IConfigurationContracts))]
  public interface IConfiguration : INotifyPropertyChanged, IAssignable, ICloneable {
    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application should do one cycle right after it has been
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    bool CycleAfterStartup { get; set; }

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
    bool TerminateAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </value>
    bool MinimizeAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
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
    TimeSpan AutocycleInterval { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    bool CycleAfterDisplaySettingsChanged { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    bool MinimizeOnClose { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether to show the remaining time for the next random cycle
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether to show the remaining time for the next random cycle as overlay icon
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
    ///   Gets or sets a collection of <see cref="Models.ScreenSettings" /> objects containing the specific properties for each single
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="Models.ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="Models.ScreenSettings">ScreenSettings Class</seealso>
    Dictionary<string, IScreenSettings> ScreenSettings { get; set; }
  }

  [DoNotNotify]
  [ContractClassFor(typeof(IConfiguration))]
  internal abstract class IConfigurationContracts : IConfiguration {
    public abstract bool CycleAfterStartup { get; set; }
    public abstract bool TerminateAfterStartup { get; set; }
    public abstract bool MinimizeAfterStartup { get; set; }
    public abstract bool StartAutocyclingAfterStartup { get; set; }
    public abstract bool CycleAfterDisplaySettingsChanged { get; set; }
    public abstract bool MinimizeOnClose { get; set; }
    public abstract bool DisplayCycleTimeAsIconOverlay { get; set; }
    public abstract TimeSpan AutocycleInterval { get; set; }

    public WallpaperClickAction WallpaperDoubleClickAction {
      get {
        Contract.Ensures(Enum.IsDefined(typeof(WallpaperClickAction), Contract.Result<WallpaperClickAction>()));
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(WallpaperClickAction), value));
        throw new NotImplementedException();
      }
    }

    public TrayIconClickAction TrayIconSingleClickAction {
      get {
        Contract.Ensures(Enum.IsDefined(typeof(TrayIconClickAction), Contract.Result<TrayIconClickAction>()));
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TrayIconClickAction), value));  
        throw new NotImplementedException();
      }
    }

    public TrayIconClickAction TrayIconDoubleClickAction {
      get {
        Contract.Ensures(Enum.IsDefined(typeof(TrayIconClickAction), Contract.Result<TrayIconClickAction>()));
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TrayIconClickAction), value));  
        throw new NotImplementedException();
      }
    }

    public WallpaperChangeType WallpaperChangeType {
      get {
        Contract.Ensures(Enum.IsDefined(typeof(WallpaperChangeType), Contract.Result<WallpaperChangeType>()));
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(WallpaperChangeType), value));
        throw new NotImplementedException();
      }
    }

    public Dictionary<string, IScreenSettings> ScreenSettings {
      get {
        Contract.Ensures(Contract.Result<Dictionary<string, IScreenSettings>>() != null);
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentNullException>(value != null);
        throw new NotImplementedException();
      }
    }

    public abstract void AssignTo(object other);
    public abstract object Clone();
    public abstract event PropertyChangedEventHandler PropertyChanged;
  }
}