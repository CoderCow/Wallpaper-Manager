// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Presentation;
using WallpaperManager.Models;
using WallpaperManager.ViewModels;

namespace WallpaperManager.Views {
  /// <summary>
  ///   The Monitor Settings <see cref="Page" /> used by the <see cref="ConfigWindow" />.
  /// </summary>
  /// <seealso cref="ConfigWindow">ConfigWindow Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class MonitorSettingsPage : Page {
    /// <summary>
    ///   Gets or sets the <see cref="ConfigurationVM" /> object used as data context.
    /// </summary>
    /// <inheritdoc cref="System.Windows.FrameworkElement.DataContext" />
    /// <value>
    ///   The <see cref="ConfigurationVM" /> object used as data context.
    /// </value>
    /// <seealso cref="ConfigurationVM">ConfigurationVM Class</seealso>
    public new ConfigurationVM DataContext {
      get { return (ConfigurationVM)base.DataContext; }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="MonitorSettingsPage" /> class.
    /// </summary>
    public MonitorSettingsPage() {
      this.InitializeComponent();
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(MonitorSettingsPage.ConfigureStaticWallpaperCommand != null);
      Contract.Invariant(MonitorSettingsPage.ConfigureTextOverlaysCommand != null);
    }

    #region Command: ConfigureStaticWallpaper
    /// <summary>
    ///   The Configure Static Wallpaper <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand ConfigureStaticWallpaperCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureStaticWallpaperCommand" />
    protected virtual void ConfigureStaticWallpaperCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows the <see cref="ConfigWallpaperWindow" /> to configure the static
    ///   <see cref="Wallpaper" /> of the current screen.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureStaticWallpaperCommand" />
    protected virtual void ConfigureStaticWallpaperCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      ConfigWallpaperWindow configWallpaperWindow = new ConfigWallpaperWindow(
        new ConfigWallpaperVM(this.DataContext.SelectedScreenSettings.StaticWallpaper),
        this.DataContext.Configuration.ScreensSettings);

      configWallpaperWindow.Owner = this.GetClosestParentOfType<Window>();
      configWallpaperWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

      configWallpaperWindow.ShowDialog();
    }
    #endregion

    #region Command: ConfigureTextOverlays
    /// <summary>
    ///   The Configur Overlay Texts <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand ConfigureTextOverlaysCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureTextOverlaysCommand" />
    protected virtual void ConfigureTextOverlaysCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows a <see cref="ConfigTextOverlaysWindow" /> to configure the static
    ///   <see cref="Wallpaper" /> of the current screen.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureTextOverlaysCommand" />
    protected virtual void ConfigureTextOverlaysCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      ConfigTextOverlaysWindow configTextOverlaysWindow = new ConfigTextOverlaysWindow(new ConfigTextOverlaysVM(this.DataContext.SelectedScreenSettings.TextOverlays));

      configTextOverlaysWindow.Owner = this.GetClosestParentOfType<Window>();
      configTextOverlaysWindow.ShowDialog();
    }
    #endregion
  }
}