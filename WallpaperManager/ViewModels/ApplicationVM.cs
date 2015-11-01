// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.ComponentModel;

using Common;
using Common.Presentation;

using WallpaperManager.Models;

namespace WallpaperManager.ViewModels {
  /// <summary>
  ///   Provides a basic interface for one or multiple Views to interact with the most parts of the application.
  ///   This is a View Model.
  /// </summary>
  /// <commondoc select='ViewModels/General/*' />
  /// <threadsafety static="true" instance="false" />
  public class ApplicationVM: INotifyPropertyChanged {
    #region Property: WallpaperCategoryCollectionVM
    /// <summary>
    ///   <inheritdoc cref="WallpaperCategoryCollectionVM" select='../value/node()' />
    /// </summary>
    private readonly WallpaperCategoryCollectionVM wallpaperCategoryCollectionVM;

    /// <summary>
    ///   Gets the <see cref="ViewModels.WallpaperCategoryCollectionVM" /> instance.
    /// </summary>
    /// <value>
    ///   The <see cref="ViewModels.WallpaperCategoryCollectionVM" /> instance.
    /// </value>
    /// <seealso cref="ViewModels.WallpaperCategoryCollectionVM">WallpaperCategoryCollectionVM Class</seealso>
    public WallpaperCategoryCollectionVM WallpaperCategoryCollectionVM {
      get { return this.wallpaperCategoryCollectionVM; }
    }
    #endregion

    #region Property: WallpaperChangerVM
    /// <summary>
    ///   <inheritdoc cref="WallpaperChangerVM" select='../value/node()' />
    /// </summary>
    private readonly WallpaperChangerVM wallpaperChangerVM;

    /// <summary>
    ///   Gets the the <see cref="ViewModels.WallpaperChangerVM" /> instance providing an interface to control the cycling 
    ///   of wallpapers.
    /// </summary>
    /// <value>
    ///   The the <see cref="ViewModels.WallpaperChangerVM" /> instance providing an interface to control the cycling of 
    ///   wallpapers.
    /// </value>
    /// <seealso cref="ViewModels.WallpaperChangerVM">WallpaperChangerVM Class</seealso>
    public WallpaperChangerVM WallpaperChangerVM {
      get { return this.wallpaperChangerVM; }
    }
    #endregion

    #region Property: RequestShowMain
    /// <summary>
    ///   <inheritdoc cref="RequestShowMain" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM,Boolean> requestShowMain;

    /// <summary>
    ///   Gets the delegate invoked to request the showing of the main view.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the showing of the main view.
    /// </value>
    protected Action<ApplicationVM,Boolean> RequestShowMain {
      get { return this.requestShowMain; }
    }
    #endregion

    #region Property: RequestShowConfiguration
    /// <summary>
    ///   <inheritdoc cref="RequestShowConfiguration" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM> requestShowConfiguration;

    /// <summary>
    ///   Gets the delegate invoked to request the showing of the configuration view.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the showing of the configuration view.
    /// </value>
    protected Action<ApplicationVM> RequestShowConfiguration {
      get { return this.requestShowConfiguration; }
    }
    #endregion

    #region Property: RequestShowChangelog
    /// <summary>
    ///   <inheritdoc cref="RequestShowChangelog" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM> requestShowChangelog;

    /// <summary>
    ///   Gets the delegate invoked to request the showing of the Changelog.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the showing of the Changelog.
    /// </value>
    protected Action<ApplicationVM> RequestShowChangelog {
      get { return this.requestShowChangelog; }
    }
    #endregion

    #region Property: RequestShowAbout
    /// <summary>
    ///   <inheritdoc cref="RequestShowAbout" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM> requestShowAbout;

    /// <summary>
    ///   Gets the delegate invoked to request the showing of the about view.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the showing of the about view.
    /// </value>
    protected Action<ApplicationVM> RequestShowAbout {
      get { return this.requestShowAbout; }
    }
    #endregion

    #region Property: RequestUpdateCheck
    /// <summary>
    ///   <inheritdoc cref="RequestUpdateCheck" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM> requestUpdateCheck;

    /// <summary>
    ///   Gets the delegate invoked to request a check for updates.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request a check for updates.
    /// </value>
    protected Action<ApplicationVM> RequestUpdateCheck {
      get { return this.requestUpdateCheck; }
    }
    #endregion

    #region Property: RequestTerminateApplication
    /// <summary>
    ///   <inheritdoc cref="RequestTerminateApplication" select='../value/node()' />
    /// </summary>
    private readonly Action<ApplicationVM> requestTerminateApplication;

    /// <summary>
    ///   Gets the delegate invoked to request a termination of the application.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request a termination of the application.
    /// </value>
    protected Action<ApplicationVM> RequestTerminateApplication {
      get { return this.requestTerminateApplication; }
    }
    #endregion

    #region Event: RequestViewClose
    /// <summary>
    ///   Occurs when the View Model requests the View to close.
    /// </summary>
    public event EventHandler RequestViewClose;

    /// <summary>
    ///   Called when the View Model requests the View to close.
    /// </summary>
    /// <remarks>
    ///   This method raises the <see cref="RequestViewClose">RequestViewClose Event</see>.
    /// </remarks>
    /// <seealso cref="RequestViewClose">RequestViewClose Event</seealso>
    protected virtual void OnRequestViewClose() {
      if (this.RequestViewClose != null) {
        this.RequestViewClose(this, EventArgs.Empty);
      }
    }
    #endregion

    #region Event: UnhandledCommandException
    /// <commondoc select='ViewModels/Events/UnhandledCommandException/*' />
    public event EventHandler<CommandExceptionEventArgs> UnhandledCommandException;

    /// <commondoc select='ViewModels/Methods/OnUnhandledCommandException/*' />
    protected virtual void OnUnhandledCommandException(CommandExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.UnhandledCommandException != null) {
        this.UnhandledCommandException.ReverseInvoke(this, e);
      }

      if (!e.IsHandled) {
        throw e.Exception;
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ApplicationVM" /> class.
    /// </summary>
    /// <param name="categoriesVM">
    ///   The <see cref="ViewModels.WallpaperCategoryCollectionVM" /> instance.
    /// </param>
    /// <param name="wallpaperChangerVM">
    ///   The the <see cref="ViewModels.WallpaperChangerVM" /> instance providing an interface to 
    ///   control the cycling of wallpapers.
    /// </param>
    /// <param name="requestShowMain">
    ///   The delegate invoked to request the showing of the main view.
    /// </param>
    /// <param name="requestShowConfiguration">
    ///   The delegate invoked to request the showing of the configuration view.
    /// </param>
    /// <param name="requestShowChangelog">
    ///   The delegate invoked to request the showing of the Changelog.
    /// </param>
    /// <param name="requestShowAbout">
    ///   The delegate invoked to request the showing of the about view.
    /// </param>
    /// <param name="requestUpdateCheck">
    ///   The delegate invoked to request a check for updates.
    /// </param>
    /// <param name="requestTerminateApplication">
    ///   The delegate invoked to request a termination of the application.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="categoriesVM" /> or <paramref name="wallpaperChangerVM" /> or <paramref name="requestShowMain" /> or 
    ///   <paramref name="requestShowConfiguration" /> or <paramref name="requestShowChangelog" /> or 
    ///   <paramref name="requestShowAbout" /> or <paramref name="requestUpdateCheck" /> or 
    ///   <paramref name="requestTerminateApplication" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="ViewModels.WallpaperCategoryCollectionVM">WallpaperCategoryCollectionVM Class</seealso>
    /// <seealso cref="ViewModels.WallpaperChangerVM">WallpaperChangerVM Class</seealso>
    public ApplicationVM(
      WallpaperCategoryCollectionVM categoriesVM, 
      WallpaperChangerVM wallpaperChangerVM,
      Action<ApplicationVM,Boolean> requestShowMain,
      Action<ApplicationVM> requestShowConfiguration,
      Action<ApplicationVM> requestShowChangelog,
      Action<ApplicationVM> requestShowAbout,
      Action<ApplicationVM> requestUpdateCheck,
      Action<ApplicationVM> requestTerminateApplication
    ) {
      if (categoriesVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("categoriesVM"));
      }
      if (wallpaperChangerVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperChangerVM"));
      }
      if (requestUpdateCheck == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestUpdateCheck"));
      }
      if (requestShowMain == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestShowMain"));
      }
      if (requestShowConfiguration == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestShowConfiguration"));
      }
      if (requestShowChangelog == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestShowChangelog"));
      }
      if (requestShowAbout == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestShowAbout"));
      }
      if (requestTerminateApplication == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestTerminateApplication"));
      }
      
      this.wallpaperCategoryCollectionVM = categoriesVM;
      this.wallpaperChangerVM = wallpaperChangerVM;
      this.requestUpdateCheck = requestUpdateCheck;
      this.requestShowMain = requestShowMain;
      this.requestShowConfiguration = requestShowConfiguration;
      this.requestShowChangelog = requestShowChangelog;
      this.requestShowAbout = requestShowAbout;
      this.requestTerminateApplication = requestTerminateApplication;
    }
    #endregion

    #region Command: ShowMain
    /// <summary>
    ///   <inheritdoc cref="ShowMainCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand<Boolean> showMainCommand;

    /// <summary>
    ///   Gets the Show Main <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Show Main <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ShowMainCommand_CanExecute">ShowMainCommand_CanExecute Method</seealso>
    /// <seealso cref="ShowMainCommand_Execute">ShowMainCommand_Execute Method</seealso>
    public DelegateCommand<Boolean> ShowMainCommand {
      get {
        if (this.showMainCommand == null) {
          this.showMainCommand = new DelegateCommand<Boolean>(this.ShowMainCommand_Execute, this.ShowMainCommand_CanExecute);
        }

        return this.showMainCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ShowMainCommand" /> can be executed.
    /// </summary>
    /// <param name="showMinimized">
    ///   A <see cref="Boolean" /> indicating whether the main view should be shown minimized or not.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ShowMainCommand" />
    protected Boolean ShowMainCommand_CanExecute(Boolean showMinimized) {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ShowMainCommand" /> is executed and requests the main view to be shown.
    /// </summary>
    /// <param name="showMinimized">
    ///   A <see cref="Boolean" /> indicating whether the main view should be shown minimized or not.
    /// </param>
    /// <seealso cref="ShowMainCommand" />
    protected void ShowMainCommand_Execute(Boolean showMinimized) {
      try {
      this.RequestShowMain(this, showMinimized);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ShowMainCommand, exception));
      }
    }
    #endregion

    #region Command: ShowConfiguration
    /// <summary>
    ///   <inheritdoc cref="ShowConfigurationCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand showConfigurationCommand;

    /// <summary>
    ///   Gets the Show Configuration <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Show Configuration <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ShowConfigurationCommand_CanExecute">ShowConfigurationCommand_CanExecute Method</seealso>
    /// <seealso cref="ShowConfigurationCommand_Execute">ShowConfigurationCommand_Execute Method</seealso>
    public DelegateCommand ShowConfigurationCommand {
      get {
        if (this.showConfigurationCommand == null) {
          this.showConfigurationCommand = new DelegateCommand(this.ShowConfigurationCommand_Execute, this.ShowConfigurationCommand_CanExecute);
        }

        return this.showConfigurationCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ShowConfigurationCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ShowConfigurationCommand" />
    protected Boolean ShowConfigurationCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ShowConfigurationCommand" /> is executed and requests the configuration window to be shown.
    /// </summary>
    /// <seealso cref="ShowConfigurationCommand" />
    protected void ShowConfigurationCommand_Execute() {
      try {
        this.RequestShowConfiguration(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ShowConfigurationCommand, exception));
      }
    }
    #endregion

    #region Command: ShowChangelog
    /// <summary>
    ///   <inheritdoc cref="ShowChangelogCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand showChangelogCommand;

    /// <summary>
    ///   Gets the Show Changelog <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Show Changelog <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ShowChangelogCommand_CanExecute">ShowChangelogCommand_CanExecute Method</seealso>
    /// <seealso cref="ShowChangelogCommand_Execute">ShowChangelogCommand_Execute Method</seealso>
    public DelegateCommand ShowChangelogCommand {
      get {
        if (this.showChangelogCommand == null) {
          this.showChangelogCommand = new DelegateCommand(this.ShowChangelogCommand_Execute, this.ShowChangelogCommand_CanExecute);
        }

        return this.showChangelogCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ShowChangelogCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ShowChangelogCommand" />
    protected Boolean ShowChangelogCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ShowChangelogCommand" /> is executed and requests the Changelog to be shown.
    /// </summary>
    /// <seealso cref="ShowChangelogCommand" />
    protected void ShowChangelogCommand_Execute() {
      try {
        this.RequestShowChangelog(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ShowChangelogCommand, exception));
      }
    }
    #endregion

    #region Command: ShowAbout
    /// <summary>
    ///   <inheritdoc cref="ShowAboutCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand showAboutCommand;

    /// <summary>
    ///   Gets the Show About <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Show About <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ShowAboutCommand_CanExecute">ShowAboutCommand_CanExecute Method</seealso>
    /// <seealso cref="ShowAboutCommand_Execute">ShowAboutCommand_Execute Method</seealso>
    public DelegateCommand ShowAboutCommand {
      get {
        if (this.showAboutCommand == null) {
          this.showAboutCommand = new DelegateCommand(this.ShowAboutCommand_Execute, this.ShowAboutCommand_CanExecute);
        }

        return this.showAboutCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ShowAboutCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ShowAboutCommand" />
    protected Boolean ShowAboutCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ShowAboutCommand" /> is executed and requests the about window to be shown.
    /// </summary>
    /// <seealso cref="ShowAboutCommand" />
    protected void ShowAboutCommand_Execute() {
      try {
        this.RequestShowAbout(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ShowAboutCommand, exception));
      }
    }
    #endregion

    #region Command: UpdateCheck
    /// <summary>
    ///   <inheritdoc cref="UpdateCheckCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand updateCheckCommand;

    /// <summary>
    ///   Gets the Update Check <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Update Check <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="UpdateCheckCommand_CanExecute">UpdateCheckCommand_CanExecute Method</seealso>
    /// <seealso cref="UpdateCheckCommand_Execute">UpdateCheckCommand_Execute Method</seealso>
    public DelegateCommand UpdateCheckCommand {
      get {
        if (this.updateCheckCommand == null) {
          this.updateCheckCommand = new DelegateCommand(this.UpdateCheckCommand_Execute, this.UpdateCheckCommand_CanExecute);
        }

        return this.updateCheckCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="UpdateCheckCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="UpdateCheckCommand" />
    protected Boolean UpdateCheckCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="UpdateCheckCommand" /> is executed and requests an update check.
    /// </summary>
    /// <seealso cref="UpdateCheckCommand" />
    protected void UpdateCheckCommand_Execute() {
      try {
        this.RequestUpdateCheck(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.UpdateCheckCommand, exception));
      }
    }
    #endregion

    #region Command: Close
    /// <summary>
    ///   <inheritdoc cref="CloseCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand closeCommand;

    /// <summary>
    ///   Gets the Close <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Close <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="CloseCommand_CanExecute">CloseCommand_CanExecute Method</seealso>
    /// <seealso cref="CloseCommand_Execute">CloseCommand_Execute Method</seealso>
    public DelegateCommand CloseCommand {
      get {
        if (this.closeCommand == null) {
          this.closeCommand = new DelegateCommand(this.CloseCommand_Execute, this.CloseCommand_CanExecute);
        }

        return this.closeCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="CloseCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="CloseCommand" />
    protected Boolean CloseCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="CloseCommand" /> is executed and requests the attached Views to close.
    /// </summary>
    /// <seealso cref="CloseCommand" />
    protected void CloseCommand_Execute() {
      try {
        this.OnRequestViewClose();
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.CloseCommand, exception));
      }
    }
    #endregion

    #region Command: TerminateApplication
    /// <summary>
    ///   <inheritdoc cref="TerminateApplicationCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand terminateApplicationCommand;

    /// <summary>
    ///   Gets the Terminate Application <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Terminate Application <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="TerminateApplicationCommand_CanExecute">TerminateApplicationCommand_CanExecute Method</seealso>
    /// <seealso cref="TerminateApplicationCommand_Execute">TerminateApplicationCommand_Execute Method</seealso>
    public DelegateCommand TerminateApplicationCommand {
      get {
        if (this.terminateApplicationCommand == null) {
          this.terminateApplicationCommand = new DelegateCommand(this.TerminateApplicationCommand_Execute, this.TerminateApplicationCommand_CanExecute);
        }

        return this.terminateApplicationCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="TerminateApplicationCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="TerminateApplicationCommand" />
    protected Boolean TerminateApplicationCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="TerminateApplicationCommand" /> is executed and requests the termination of the application.
    /// </summary>
    /// <seealso cref="TerminateApplicationCommand" />
    protected void TerminateApplicationCommand_Execute() {
      try {
        this.RequestTerminateApplication(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.TerminateApplicationCommand, exception));
      }
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