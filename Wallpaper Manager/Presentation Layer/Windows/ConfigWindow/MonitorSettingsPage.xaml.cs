// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Common.Presentation;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The Monitor Settings <see cref="Page" /> used by the <see cref="ConfigWindow" />.
  /// </summary>
  /// <seealso cref="ConfigWindow">ConfigWindow Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class MonitorSettingsPage: Page {
    #region Property: DataContext
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
    #endregion

    #region Command: ConfigureStaticWallpaper
    /// <summary>
    ///   The Configure Static Wallpaper <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public readonly static RoutedCommand ConfigureStaticWallpaperCommand = new RoutedCommand();

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
    protected virtual void ConfigureStaticWallpaperCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
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
    protected virtual void ConfigureStaticWallpaperCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      ConfigWallpaperWindow configWallpaperWindow = new ConfigWallpaperWindow(
        new ConfigWallpaperVM(this.DataContext.SelectedScreenSettings.StaticWallpaper),
        this.DataContext.Configuration.ScreensSettings
      );
      configWallpaperWindow.Owner = this.GetClosestParentOfType<Window>();
      configWallpaperWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

      configWallpaperWindow.ShowDialog();
    }
    #endregion

    #region Command: ConfigureOverlayTexts
    /// <summary>
    ///   The Configur Overlay Texts <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public readonly static RoutedCommand ConfigureOverlayTextsCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureOverlayTextsCommand" />
    protected virtual void ConfigureOverlayTextsCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
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
    /// <seealso cref="ConfigureOverlayTextsCommand" />
    protected virtual void ConfigureOverlayTextsCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      ConfigTextOverlaysWindow configTextOverlaysWindow = new ConfigTextOverlaysWindow(
        new ConfigTextOverlaysVM(this.DataContext.SelectedScreenSettings.OverlayTexts)
      );

      configTextOverlaysWindow.Owner = this.GetClosestParentOfType<Window>();
      configTextOverlaysWindow.ShowDialog();
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="MonitorSettingsPage" /> class.
    /// </summary>
    public MonitorSettingsPage() {
      this.InitializeComponent();
    }
    #endregion
  }
}
