// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Common;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The Configuration Window used to configure the applicaiton's base settings which are stored by the 
  ///   <see cref="Configuration" /> class using the <see cref="ConfigurationVM">Configuration View Model</see>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This <see cref="Window" /> manages 3 different <see cref="Page">Pages</see> using a <see cref="Frame" />:
  ///     <list type="bullet">
  ///       <item>
  ///         <description>
  ///           <see cref="GeneralSettingsPage">The General Settings Page</see>
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <description>
  ///           <see cref="CyclingSettingsPage">The Cycling Settings Page</see>
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <description>
  ///           <see cref="MonitorSettingsPage">The Monitor Settings Page</see>
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  /// </remarks>
  /// <seealso cref="ConfigurationVM">ConfigurationVM Class</seealso>
  /// <seealso cref="GeneralSettingsPage">GeneralSettingsPage Class</seealso>
  /// <seealso cref="CyclingSettingsPage">CyclingSettingsPage Class</seealso>
  /// <seealso cref="MonitorSettingsPage">MonitorSettingsPage Class</seealso>
  /// <seealso cref="Page">Page Class</seealso>
  /// <seealso cref="Frame">Frame Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class ConfigWindow: Window {
    #region Property: ConfigurationVM
    /// <summary>
    ///   <inheritdoc cref="ConfigurationVM" select='../value/node()' />
    /// </summary>
    private readonly ConfigurationVM configurationVM;

    /// <summary>
    ///   Gets the <see cref="WallpaperManager.ApplicationInterface.ConfigurationVM" /> instance used as interface to communicate 
    ///   with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperManager.ApplicationInterface.ConfigurationVM" /> instance used as interface to communicate with 
    ///   the application.
    /// </value>
    public ConfigurationVM ConfigurationVM {
      get { return this.configurationVM; }
    }
    #endregion

    #region Command: ChangePage
    /// <summary>
    ///   Contains the Change Page <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand ChangePageCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ChangePageCommand" />
    protected virtual void ChangePageCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method saves the configuration files and closes the view.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ChangePageCommand" />
    protected virtual void ChangePageCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      FrameworkElement navigateTo = (e.Parameter as FrameworkElement);

      if (navigateTo != null && this.frmContent.Navigate(navigateTo)) {
        this.frmContent.Focus();
      }
    }
    #endregion


    #region Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigWindow" /> class.
    /// </summary>
    /// <param name="configurationVM">
    ///   The <see cref="WallpaperManager.ApplicationInterface.ConfigurationVM" /> instance used as interface to communicate with 
    ///   the application.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="configurationVM" /> is <c>null</c>.
    /// </exception>
    public ConfigWindow(ConfigurationVM configurationVM) {
      if (configurationVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("configurationVM"));
      }

      this.configurationVM = configurationVM;
      this.configurationVM.RequestClose += delegate(Object sender, RequestCloseEventArgs e) {
        this.DialogResult = e.Result;
        this.Close();
      };

      this.InitializeComponent();
    }
    #endregion

    #region Method: FrmContent_ContentRendered
    /// <summary>
    ///   Handles the <see cref="Frame.ContentRendered" /> event of the ContentFrame-Element.
    ///   Sets the content's data context to the frame's data context.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="System.EventArgs" /> instance containing the event data.
    /// </param>
    private void FrmContent_ContentRendered(Object sender, EventArgs e) {
      FrameworkElement element = (this.frmContent.Content as FrameworkElement);

      if (element != null) {
        element.DataContext = this.frmContent.DataContext;
      }
    }
    #endregion
  }
}
