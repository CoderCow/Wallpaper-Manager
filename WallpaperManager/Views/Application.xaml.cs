// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

using Common;
using Common.ObjectModel;
using Common.Presentation;

using WallpaperManager.Models;
using WallpaperManager.ViewModels;
using AppEnvironment = WallpaperManager.Models.AppEnvironment;

namespace WallpaperManager.Views {
  /// <summary>
  ///   The root class of the application. This is a Singleton class.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This class handles most of the application's initialize process and publishes data, functions and components to 
  ///     be used globally over this program. It also handles most Graphical User Interface related dialog and window management
  ///     and exception handling.
  ///   </para>
  ///   <para>
  ///     The initialization process performed by this class covers the following tasks (in the given order):
  ///     <list type="bullet">
  ///       <item>
  ///         <description>
  ///           Setting the <see cref="CultureInfo">Culture</see> of the app and handling the <see cref="Mutex" /> to 
  ///           prevent multiple instances.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <description>
  ///           Register global error handlers (if DEBUG is not defined) to show dialogs instead of the default exception 
  ///           window.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <description>
  ///           If existing, load the configuration file and apply its startup-related settings (also includes creating and 
  ///           showing the <see cref="MainWindow" />, creating the notify icon and the <see cref="WallpaperChanger" />, 
  ///           and starting the auto termination process if necessary).
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <description>
  ///           Perform an update check.
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  ///   <commondoc select='Singleton/General/remarks/*' params="AccessProperty=Current" />
  /// </remarks>
  /// <commondoc select='Singleton/General/seealso' params="AccessProperty=Current" />
  /// <seealso cref="Configuration">Configuration Property</seealso>
  /// <seealso cref="Environment">Environment Property</seealso>
  /// <seealso cref="MainWindow">MainWindow Property</seealso>
  /// <seealso cref="WallpaperChanger">WallpaperChanger Property</seealso>
  /// <seealso cref="NotifyIcon">NotifyIcon Property</seealso>
  /// <seealso cref="ApplicationViewModel">ApplicationViewModel Property</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class Application: System.Windows.Application, INotifyPropertyChanged, IDisposable {
    #region Constants: AutoTerminate_Seconds, isFirstInstance
    /// <summary>
    ///   Represents the time in seconds to wait until the application gets automatically terminated.
    /// </summary>
    public const Int32 AutoTerminate_Seconds = 5;

    /// <summary>
    ///   A <see cref="Boolean" /> indicating whether this is the first instance of the application being run.
    /// </summary>
    private static Boolean isFirstInstance;
    #endregion

    #region Fields: propertyBindingManager
    /// <summary>
    ///   The <see cref="LightPropertyBindingManager" /> used to register bindings between the configuration and related objects.
    /// </summary>
    private LightPropertyBindingManager propertyBindingManager;
    #endregion

    #region Static Property: Current
    /// <summary>
    ///   The Application object for the current <see cref="AppDomain" />. 
    /// </summary>
    private static Application current;

    /// <inheritdoc cref="System.Windows.Application.Current" />
    public static new Application Current {
      get { return Application.current; }
    }
    #endregion

    #region Property: Environment
    /// <summary>
    ///   <inheritdoc cref="Environment" select='../value/node()' />
    /// </summary>
    private AppEnvironment environment;

    /// <summary>
    ///   Gets the <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </summary>
    /// <value>
    ///   The <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </value>
    public AppEnvironment Environment {
      get { return this.environment; }
    }
    #endregion

    #region Property: Configuration
    /// <summary>
    ///   <inheritdoc cref="Configuration" select='../value/node()' />
    /// </summary>
    private Configuration configuration;

    /// <summary>
    ///   Gets the <see cref="Configuration" /> instance, containing several user-defined configuration data.
    /// </summary>
    /// <value>
    ///   The <see cref="Configuration" /> instance, containing several user-defined configuration data.
    /// </value>
    protected Configuration Configuration {
      get { return this.configuration; }
    }
    #endregion

    #region Property: WallpaperChanger
    /// <summary>
    ///   <inheritdoc cref="WallpaperChanger" select='../value/node()' />
    /// </summary>
    /// <inheritdoc cref="WallpaperChanger" select='seealso' />
    private WallpaperChanger wallpaperChanger;

    /// <summary>
    ///   Gets the changer used to manage the change and generation process of wallpapers.
    /// </summary>
    /// <value>
    ///   The chager object used to manage the change and generation process of wallpapers.
    /// </value>
    /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
    protected WallpaperChanger WallpaperChanger {
      get { return this.wallpaperChanger; }
    }
    #endregion

    #region Property: WallpaperChangerVM
    /// <summary>
    ///   <inheritdoc cref="WallpaperChanger" select='../value/node()' />
    /// </summary>
    private WallpaperChangerVM wallpaperChangerVM;

    /// <summary>
    ///   Gets the <see cref="WallpaperChangerVM" /> instance used to wrap the <see cref="WallpaperChanger" /> instance in 
    ///   a Graphical User Interface context.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperChangerVM" /> instance used to wrap the <see cref="WallpaperChanger" /> instance in 
    ///   a Graphical User Interface context.
    /// </value>
    protected WallpaperChangerVM WallpaperChangerVM {
      get { return this.wallpaperChangerVM; }
    }
    #endregion

    #region Property: MainWindow
    /// <summary>
    ///   Gets the <see cref="MainWindow" /> instance.
    /// </summary>
    /// <value>
    ///   The <see cref="MainWindow" /> instance. <c>null</c> if no <see cref="MainWindow" /> is actually shown.
    /// </value>
    /// <seealso cref="MainWindow">MainWindow Class</seealso>
    public new MainWindow MainWindow {
      get {
        // It's possible that the Main Window is another Window (because WPF suggests any Window shown as Main Window if 
        // no Main Window is actually set).
        if (base.MainWindow != null && !(base.MainWindow is MainWindow)) {
          return null;
        }

        return (MainWindow)base.MainWindow;
      }
    }
    #endregion

    #region Property: NotifyIcon
    /// <summary>
    ///   <inheritdoc cref="NotifyIcon" select='../value/node()' />
    /// </summary>
    /// <inheritdoc cref="NotifyIcon" select='seealso' />
    private NotifyIconManager notifyIcon;

    /// <summary>
    ///   Gets the <see cref="NotifyIconManager" /> instance used to control the application's notify icon.
    /// </summary>
    /// <value>
    ///   The <see cref="NotifyIconManager" /> instance used to control the application's notify icon.
    /// </value>
    /// <seealso cref="NotifyIconManager">NotifyIconManager Class</seealso>
    protected NotifyIconManager NotifyIcon {
      get { return this.notifyIcon; }
    }
    #endregion

    #region Property: ApplicationViewModel
    /// <summary>
    ///   <inheritdoc cref="ApplicationViewModel" select='../value/node()' />
    /// </summary>
    private ApplicationVM applicationViewModel;

    /// <summary>
    ///   Gets the <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <value>
    ///   The <see cref="ApplicationVM" /> instance.
    /// </value>
    public ApplicationVM ApplicationViewModel {
      get { return this.applicationViewModel; }
    }
    #endregion


    #region Methods: Main, Constructor
    /// <summary>
    ///   The main entry point of the application.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This is the only point where an instance of this class is being created.
    ///   </para>
    ///   <para>
    ///     A <see cref="Mutex" /> is created by this method to check whether this application has been started multiple times.
    ///   </para>
    /// </remarks>
    /// <seealso cref="isFirstInstance">isFirstInstance Field</seealso>
    /// <seealso cref="Mutex">Mutex Class</seealso>
    [STAThread]
    private static void Main() {
      Thread.CurrentThread.Name = "Wallpaper Manager STA-Thread";
      
      using (new Mutex(true, AppEnvironment.AppGuid, out Application.isFirstInstance)) {
        Application.current = new Application();
        Application.Current.environment = new AppEnvironment();
        Debug.Listeners.Add(new TextWriterTraceListener(Application.Current.Environment.DebugFilePath));
        Debug.WriteLine("Listeners registered.");
        Application.Current.Environment.DebugWriteAppInfo();

        Application.Current.InitializeComponent();
        Application.Current.Run();
      }
    }

    /// <inheritdoc />
    private Application() {}
    #endregion

    #region Methods: OnStartup, OnExit
    /// <summary>
    ///   Raises the <see cref="System.Windows.Application.Startup" /> event and handles the main initialization process of the 
    ///   program.
    /// </summary>
    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e) {
      if (this.isDisposed) {
        return;
      }
      base.OnStartup(e);

      // Note: Has to be set BEFORE showing the first dialog, because the first shown dialog will be suggested as main window 
      // and the application will therefore close when the window closes.
      this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

      // Register global error handler.
      this.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;

      Debug.WriteLine("-- Application init process --");
      Debug.Indent();
      // We want only one instance being run at the same time because multiple instances could result in a mess of autocycles.
      // TODO: If another instance is already running, bring its main window to the front.
      if (!Application.isFirstInstance) {
        Debug.WriteLine("Detected another running instance of the application.");
        DialogManager.ShowGeneral_AppAlreadyRunning(null);
        Debug.Unindent();
        this.Shutdown();

        return;
      }

      Debug.WriteLine("First step."); Debug.Flush();
      // Check whether the Use Default Settings argument is defined.
      if (!this.Environment.IsUseDefaultSettingsDefined) {
        if (!File.Exists(this.Environment.ConfigFilePath)) {
          Debug.Write("Configuration file at \"");
          Debug.Write(this.Environment.ConfigFilePath);
          Debug.WriteLine("\" not found. Using default settings.");
          DialogManager.ShowConfig_FileNotFound(this.MainWindow, this.Environment.ConfigFilePath);

          this.configuration = new Configuration();
        } else {
          #if !DEBUG
          try {
            this.configuration = Configuration.Read(this.Environment.ConfigFilePath);
          } catch (Exception exception) {
            if (exception is FileNotFoundException || exception is DirectoryNotFoundException) {
              DialogManager.ShowConfig_FileNotFound(this.MainWindow, this.Environment.ConfigFilePath);
            } else if (exception is XmlException || exception is InvalidOperationException) {
              DialogManager.ShowConfig_InvalidFormat(this.MainWindow, this.Environment.ConfigFilePath, exception.ToString());
            } else {
              DialogManager.ShowConfig_UnhandledLoadError(this.MainWindow, this.Environment.ConfigFilePath, exception.ToString());
            }
          }
          #else
          this.configuration = Configuration.Read(this.Environment.ConfigFilePath);
          #endif
        }
      }

      this.propertyBindingManager = new LightPropertyBindingManager();
      
      Debug.WriteLine("Second step."); Debug.Flush();
      this.wallpaperChanger = new WallpaperChanger(
        this.Environment.AppliedWallpaperFilePath, this.Configuration.General.ScreensSettings
      );
      this.WallpaperChanger.RequestWallpapers += (senderLocal, eLocal) => {
        eLocal.Wallpapers.AddRange(this.Configuration.WallpaperCategories.GetAllWallpapers());
      };
      this.WallpaperChanger.AutocycleException += this.WallpaperChanger_AutocycleException;

      // Register light property bindings which make sure that the WallpaperChanger gets always updated with any changed 
      // configuration settings related to it.
      this.propertyBindingManager.Register(
        this.Configuration.General, this.WallpaperChanger, new[] {
          new LightBoundProperty("WallpaperChangeType"),
          new LightBoundProperty("AutocycleInterval"),
          new LightBoundProperty("LastActiveListSize"),
          new LightBoundProperty("CycleAfterDisplaySettingsChanged"),
          new LightBoundProperty("ScreensSettings")
        }
      );

      this.wallpaperChangerVM = new WallpaperChangerVM(this.WallpaperChanger);
      this.WallpaperChangerVM.UnhandledCommandException += this.WallpaperChangerVM_UnhandledCommandException;

      this.applicationViewModel = new ApplicationVM(
        new WallpaperCategoryCollectionVM(
          this.Configuration.WallpaperCategories, this.WallpaperCategoryCollectionVM_RequestWallpaperCategoryVM
        ),
        this.WallpaperChangerVM, 
        this.ApplicationVM_RequestShowMainWindow,
        this.ApplicationVM_RequestShowConfiguration,
        this.ApplicationVM_RequestShowChangelog,
        this.ApplicationVM_RequestShowAbout,
        this.ApplicationVM_RequestUpdateCheck,
        this.ApplicationVM_RequestTerminateApplication
      );
      
      Debug.WriteLine("Third step."); Debug.Flush();
      // Cycle and start autocycling if requested.
      try {
        if (this.Configuration.General.StartAutocyclingAfterStartup) {
          this.WallpaperChanger.StartCycling();
        }

        if (this.Configuration.General.CycleAfterStartup) {
          this.WallpaperChanger.CycleNextRandomly();
        }
      } catch (InvalidOperationException) {
        if (this.WallpaperChanger.IsAutocycling) {
          this.WallpaperChanger.StopCycling();

          DialogManager.ShowCycle_MissingWallpapers(this.MainWindow, true);
        } else {
          DialogManager.ShowCycle_MissingWallpapers(this.MainWindow, false);
        }
      } catch (FileNotFoundException exception) {
        DialogManager.ShowGeneral_FileNotFound(this.MainWindow, exception.FileName);
      }

      // Check whether auto termination is requested.
      if ((this.Configuration.General.TerminateAfterStartup) && (!this.Environment.IsNoAutoTerminateDefined)) {
        DispatcherTimer autoTerminateTimer = new DispatcherTimer(DispatcherPriority.Background);
        autoTerminateTimer.Interval = new TimeSpan(0, 0, Application.AutoTerminate_Seconds);
        autoTerminateTimer.Tick += delegate {
          autoTerminateTimer.Stop();

          // Make sure that auto termination is still requested. It could have been disabled because the user performed
          // some actions in the Graphical User Interface.
          if ((!this.isDisposed) && (!this.Environment.IsNoAutoTerminateDefined)) {
            this.Shutdown();
          }
        };

        autoTerminateTimer.Start();
      } else {
        if (this.Configuration.General.MinimizeAfterStartup) {
          if (this.Configuration.General.MinimizeOnClose) {
            // If the Main Window should be minimized when closed and minimized after startup, we suggest that the user wants 
            // it to be displayed in the Task Bar after application start.
            this.ApplicationViewModel.ShowMainCommand.Execute(true);
          }
        } else {
          this.ApplicationViewModel.ShowMainCommand.Execute(false);
        }
      }

      Debug.WriteLine("Fourth step."); Debug.Flush();
      this.notifyIcon = new NotifyIconManager(this.ApplicationViewModel);

      // Register light property bindings which make sure that the NotifyIconManager gets always updated with any changed 
      // configuration settings related to it.
      this.propertyBindingManager.Register(
        this.Configuration.General, this.NotifyIcon, new[] {
          new LightBoundProperty("TrayIconSingleClickAction"), 
          new LightBoundProperty("TrayIconDoubleClickAction")
        }
      );

      UpdateManager updateManager = new UpdateManager(this.Environment);
      updateManager.VersionCheckSuccessful += (senderLocal, eLocal) => {
        // Execute the usual update handling, but only if there is actually an update available.
        if (eLocal.IsUpdate) {
          this.UpdateManager_VersionCheckSuccessful(senderLocal, eLocal);
        }
      };
      updateManager.VersionCheckError += (senderLocal, eLocal) => {
        if (eLocal.Exception is WebException || eLocal.Exception is FormatException) {
          eLocal.IsHandled = true;
        }

        this.UpdateManager_VersionCheckError(senderLocal, eLocal);
      };
      updateManager.DownloadUpdateSuccessful += this.UpdateManager_DownloadUpdateSuccessful;
      updateManager.DownloadUpdateError += this.UpdateManager_DownloadUpdateError;
      updateManager.BeginVersionCheck();

      Debug.WriteLine("Initialization succeeded."); Debug.Flush();
      Debug.Unindent();
    }

    /// <summary>
    ///   Raises the <see cref="System.Windows.Application.Exit" /> event and disposes this instance.
    /// </summary>
    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e) {
      Debug.WriteLine("Shutting down application.");
      Debug.Flush();
      this.Dispose();

      base.OnExit(e);
    }
    #endregion

    #region Methods: WriteConfigFile
    /// <summary>
    ///   Writes the configuration data into a file.
    /// </summary>
    /// <exception cref="SecurityException">
    ///   Missing <see cref="FileIOPermissionAccess.Write" /> for the <see cref="AppEnvironment.ConfigFilePath" />.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing file system access rights to write the configuration file.
    /// </exception>
    /// <permission cref="FileIOPermission">
    ///   to write the configuration file's contents. Associated enumerations: 
    ///   <see cref="FileIOPermissionAccess.Write" />.
    /// </permission>
    /// <exception cref="IOException">
    ///   The Application Data Directory could not be created.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="Configuration">Configuration Class</seealso>
    protected void WriteConfigFile() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }

      try {
        // Note: If the directory already exists this method will simply do nothing.
        Directory.CreateDirectory(this.Environment.AppDataPath);
      } catch (Exception exception) {
        if ((exception is UnauthorizedAccessException) || (exception is SecurityException) || (exception is IOException)) {
          DialogManager.ShowGeneral_UnableToCreateAppDataDirectory(this.MainWindow, this.Environment.AppDataPath);
          return;
        }

        throw;
      }

      try {
        this.Configuration.Write(this.Environment.ConfigFilePath);
      } catch (Exception exception) {
        if ((exception is UnauthorizedAccessException) || (exception is SecurityException) || (exception is IOException)) {
          DialogManager.ShowConfig_UnableToWrite(this.MainWindow, this.Environment.ConfigFilePath);
          return;
        }

        throw;
      }
    }
    #endregion

    #region Methods: View Model Request Handlers
    /// <summary>
    ///   Performs an update check for a <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the update check.
    /// </param>
    /// <param name="showMinimized">
    ///   A <see cref="Boolean" /> indicating whether the main view should be shown minimized or not.
    /// </param>
    private void ApplicationVM_RequestShowMainWindow(ApplicationVM applicationVM, Boolean showMinimized) {
      // Make sure that auto termination is no longer requested if a window is shown.
      this.Environment.IsNoAutoTerminateDefined = true;

      if (this.MainWindow == null) {
        base.MainWindow = new MainWindow(this.Environment, this.ApplicationViewModel);
        
        // Register light property bindings which make sure that the MainWindow gets always updated with any changed 
        // configuration settings.
        LightPropertyBindingManager propertyBindings = new LightPropertyBindingManager();
        propertyBindings.Register(
          this.Configuration.General, base.MainWindow, new[] {
            new LightBoundProperty("DisplayCycleTimeAsIconOverlay"), 
            new LightBoundProperty("MinimizeOnClose"),
            new LightBoundProperty("WallpaperDoubleClickAction")
          }
        );

        if (showMinimized) {
          base.MainWindow.WindowState = WindowState.Minimized;
        }

        base.MainWindow.Closed += (sender, e) => {
          MainWindow mainWindow = (MainWindow)sender;
          propertyBindings.DeregisterAll(null, mainWindow);
          mainWindow.Dispose();

          this.WriteConfigFile();
          GC.Collect();
        };
      } else {
        if (this.MainWindow.WindowState == WindowState.Minimized) {
          this.MainWindow.WindowState = WindowState.Normal;
        }
      }

      base.MainWindow.Show();
    }

    /// <summary>
    ///   Performs an update check for a <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the update check.
    /// </param>
    private void ApplicationVM_RequestShowConfiguration(ApplicationVM applicationVM) {
      // Make sure that auto termination is no longer requested if a window is shown.
      this.Environment.IsNoAutoTerminateDefined = true;

      ConfigurationVM configurationVM = new ConfigurationVM(this.Configuration.General);
      configurationVM.UnhandledCommandException += this.ConfigWallpaperVM_UnhandledCommandException;

      ConfigWindow configWindow = new ConfigWindow(configurationVM);
      configWindow.Owner = this.MainWindow;
      configWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

      Boolean? dialogResult = configWindow.ShowDialog();
      if ((dialogResult != null) && dialogResult.Value) {
        this.WriteConfigFile();
      }
    }

    /// <summary>
    ///   Performs an update check for a <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the update check.
    /// </param>
    private void ApplicationVM_RequestShowChangelog(ApplicationVM applicationVM) {
      // Make sure that auto termination is no longer requested if the Changelog is shown.
      this.Environment.IsNoAutoTerminateDefined = true;

      if (!File.Exists(this.Environment.ChangelogFilePath)) {
        DialogManager.ShowGeneral_FileNotFound(this.MainWindow, this.Environment.ChangelogFilePath);
        return;
      }

      Process.Start(this.Environment.ChangelogFilePath);
    }

    /// <summary>
    ///   Performs an update check for a <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the update check.
    /// </param>
    private void ApplicationVM_RequestShowAbout(ApplicationVM applicationVM) {
      // Make sure that auto termination is no longer requested if a window is shown.
      this.Environment.IsNoAutoTerminateDefined = true;

      AboutWindow aboutWindow = new AboutWindow(this.Environment);
      aboutWindow.Owner = this.MainWindow;
      aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      aboutWindow.ShowDialog();
    }

    /// <summary>
    ///   Performs an update check for a <see cref="ApplicationVM" /> instance.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the update check.
    /// </param>
    private void ApplicationVM_RequestUpdateCheck(ApplicationVM applicationVM) {
      if (UpdateManager.IsDownloadingUpdate) {
        return;
      }

      UpdateManager updateManager = new UpdateManager(this.Environment);
      updateManager.VersionCheckSuccessful += this.UpdateManager_VersionCheckSuccessful;
      updateManager.VersionCheckError += this.UpdateManager_VersionCheckError;
      updateManager.DownloadUpdateSuccessful += this.UpdateManager_DownloadUpdateSuccessful;
      updateManager.DownloadUpdateError += this.UpdateManager_DownloadUpdateError;
      updateManager.BeginVersionCheck();
    }

    /// <summary>
    ///   Shuts down this application.
    /// </summary>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance requesting the termination.
    /// </param>
    private void ApplicationVM_RequestTerminateApplication(ApplicationVM applicationVM) {
      this.Shutdown();
    }

    /// <summary>
    ///   Creates a new <see cref="WallpaperCategoryVM" /> from a given <see cref="WallpaperCategory" />.
    /// </summary>
    /// <param name="category">
    ///   The <see cref="WallpaperCategory" /> instance to be wrapped by a new <see cref="WallpaperCategoryVM" />.
    /// </param>
    /// <returns>
    ///   The new <see cref="WallpaperCategoryVM" /> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="category" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperCategory">WallpaperCategory Class</seealso>
    /// <seealso cref="WallpaperCategoryVM">WallpaperCategoryVM Class</seealso>
    private WallpaperCategoryVM WallpaperCategoryCollectionVM_RequestWallpaperCategoryVM(WallpaperCategory category) {
      if (category == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("category"));
      }

      // Provide the View Model with a new WallpaperCategoryVM object.
      return new WallpaperCategoryVM(
        category,
        this.WallpaperChangerVM,
        this.WallpaperCategoryVM_RequestConfigureSelected, 
        this.WallpaperCategoryVM_RequestConfigureDefaultSettings
      );
    }

    /// <summary>
    ///   Shows the <see cref="ConfigWallpaperWindow" /> to configure the selected <see cref="Wallpaper" /> instances of the
    ///   given <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="categoryVM">
    ///   The <see cref="WallpaperCategoryVM" /> to configure the selected <see cref="Wallpaper" /> instances for.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="categoryVM" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperCategoryVM">WallpaperCategoryVM Class</seealso>
    private void WallpaperCategoryVM_RequestConfigureSelected(WallpaperCategoryVM categoryVM) {
      if (categoryVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("categoryVM"));
      }

      Wallpaper[] wallpapersToConfigure = new Wallpaper[categoryVM.SelectedWallpaperVMs.Count];
      for (Int32 i = 0; i < categoryVM.SelectedWallpaperVMs.Count; i++) {
        wallpapersToConfigure[i] = categoryVM.SelectedWallpaperVMs[i].Wallpaper;
      }

      ConfigWallpaperVM configWallpaperVM = new ConfigWallpaperVM(this.configuration.General, wallpapersToConfigure, categoryVM.IsSynchronizedCategory);
      configWallpaperVM.UnhandledCommandException += this.ConfigWallpaperVM_UnhandledCommandException;

      ConfigWallpaperWindow configWallpaperWindow = new ConfigWallpaperWindow(
        configWallpaperVM, this.Configuration.General.ScreensSettings
      );

      configWallpaperWindow.Owner = this.MainWindow;

      Boolean? result = configWallpaperWindow.ShowDialog();
      // If the settings have been confirmed by the user.
      if ((result != null) && result.Value) {
        this.WriteConfigFile();
      }

      // It could be possible that the ImagePath of a wallpaper has been changed, so we clear the thumbnails which will
      // then be recreated when the GUI requests them.
      for (Int32 i = 0; i < categoryVM.SelectedWallpaperVMs.Count; i++) {
        categoryVM.SelectedWallpaperVMs[i].Thumbnail = null;
      }
    }

    /// <summary>
    ///   Shows the <see cref="ConfigWallpaperWindow" /> to configure the <see cref="WallpaperDefaultSettings" /> of the
    ///   given <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="categoryVM">
    ///   The <see cref="WallpaperCategoryVM" /> to configure the <see cref="WallpaperDefaultSettings" /> for.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="categoryVM" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperCategoryVM">WallpaperCategoryVM Class</seealso>
    private void WallpaperCategoryVM_RequestConfigureDefaultSettings(WallpaperCategoryVM categoryVM) {
      if (categoryVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("categoryVM"));
      }

      ConfigWallpaperVM configWallpaperVM = new ConfigWallpaperVM(categoryVM.Category.WallpaperDefaultSettings);
      configWallpaperVM.UnhandledCommandException += this.ConfigWallpaperVM_UnhandledCommandException;

      ConfigWallpaperWindow configWallpaperWindow = new ConfigWallpaperWindow(
        configWallpaperVM, this.Configuration.General.ScreensSettings
      );

      configWallpaperWindow.Owner = this.MainWindow;

      Boolean? result = configWallpaperWindow.ShowDialog();
      // If the settings have been confirmed by the user.
      if ((result != null) && result.Value) {
        this.WriteConfigFile();
      }
    }
    #endregion

    #region Methods: Dialog/Exception Handlers
    /// <summary>
    ///   Handles the <see cref="UpdateManager.VersionCheckSuccessful" /> event of a <see cref="UpdateManager" /> instance and
    ///   manages the dialog handling.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void UpdateManager_VersionCheckSuccessful(Object sender, VersionCheckSuccessfulEventArgs e) {
      UpdateManager updateManager = (UpdateManager)sender;

      if (!e.IsUpdate) {
        DialogManager.ShowUpdate_NoUpdateAvailable(this.MainWindow);
        return;
      }

      String resultKey = DialogManager.ShowUpdate_Available(
        this.MainWindow, this.Environment.AppVersion.ToString(), e.Version.ToString(), e.CriticalMessage, e.InfoMessage
      );
      
      switch (resultKey) {
        case "Install":
          Window updateDownloadingWindow = DialogManager.ShowUpdate_Downloading(this.MainWindow);
          // Check if the cancel button was pressed.
          updateDownloadingWindow.Closed += delegate {
            if (UpdateManager.IsDownloadingUpdate) {
              updateManager.AbortDownloadUpdate();
            }
          };
          updateManager.DownloadUpdateError += delegate { updateDownloadingWindow.Close(); };
          updateManager.DownloadUpdateSuccessful += delegate { updateDownloadingWindow.Close(); };

          updateManager.BeginDownloadUpdate();
          break;
        case "OpenWebsite":
          Process.Start(AppEnvironment.WebsiteUrl);
          break;
      }
    }

    /// <summary>
    ///   Handles the <see cref="UpdateManager.VersionCheckError" /> event of a <see cref="UpdateManager" /> instance and
    ///   manages the dialog handling and downloads the update if requested.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void UpdateManager_VersionCheckError(Object sender, ExceptionEventArgs e) {
      if (e.IsHandled) {
        return;
      }

      if (e.Exception is WebException) {
        DialogManager.ShowUpdate_UnableToConnect(this.MainWindow);
        e.IsHandled = true;
        return;
      }

      if (e.Exception is FormatException) {
        if (DialogManager.ShowUpdate_UpdateFileNotFound(this.MainWindow)) {
          Process.Start(AppEnvironment.WebsiteUrl);
        }
        e.IsHandled = true;
        return;
      }
    }

    /// <summary>
    ///   Handles the <see cref="UpdateManager.DownloadUpdateSuccessful" /> event of a <see cref="UpdateManager" /> instance and
    ///   manages the dialog handling and apply operations.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void UpdateManager_DownloadUpdateSuccessful(Object sender, EventArgs e) {
      UpdateManager updateManager = (UpdateManager)sender;

      updateManager.ApplyUpdate();
      this.Shutdown();
    }

    /// <summary>
    ///   Handles the <see cref="UpdateManager.DownloadUpdateError" /> event of a <see cref="UpdateManager" /> instance and
    ///   manages the exception handling.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void UpdateManager_DownloadUpdateError(Object sender, ExceptionEventArgs e) {
      if (e.Exception is FormatException) {
        if (DialogManager.ShowUpdate_UpdateFileNotFound(this.MainWindow)) {
          Process.Start(AppEnvironment.WebsiteUrl);
        }

        e.IsHandled = true;
        return;
      }

      if (e.Exception is WebException) {
        DialogManager.ShowUpdate_UnableToConnect(this.MainWindow);

        e.IsHandled = true;
        return;
      }
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperManager.Models.WallpaperChanger.AutocycleException" /> event of a 
    ///   <see cref="WallpaperChanger" /> instance and handles the thrown exception if possible.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WallpaperChanger_AutocycleException(Object sender, ExceptionEventArgs e) {
      if (e.IsHandled) {
        return;
      }

      // Exceptions caused by overlapping cycles are simply ignored.
      if (e.Exception is NotSupportedException) {
        e.IsHandled = true;
        return;
      }

      if (this.HandleCycleException(e.Exception)) {
        e.IsHandled = true;
        return;
      }
    }

    /// <summary>
    ///   Handles the <see cref="ConfigWallpaperVM.UnhandledCommandException" /> event of a <see cref="ConfigWallpaperVM" /> 
    ///   instance and handles the thrown exception if possible.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ConfigWallpaperVM_UnhandledCommandException(Object sender, CommandExceptionEventArgs e) {
      if (e.IsHandled) {
        return;
      }

      ConfigWallpaperVM configWallpaperVM = (ConfigWallpaperVM)sender;

      if (e.Command == configWallpaperVM.ApplySettingsCommand) {
        if (e.Exception is InvalidOperationException) {
          DialogManager.ShowWallpapers_MultiscreenWallpaperOnDisabledScreens(null);
        }

        e.IsHandled = true;
        return;
      }
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperManager.ViewModels.WallpaperChangerVM.UnhandledCommandException" /> event of a 
    ///   <see cref="WallpaperManager.ViewModels.WallpaperChangerVM" /> instance and handles the thrown exception if 
    ///   possible.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WallpaperChangerVM_UnhandledCommandException(Object sender, CommandExceptionEventArgs e) {
      if (e.IsHandled) {
        return;
      }

      WallpaperChangerVM wallpaperChangerVM = (WallpaperChangerVM)sender;

      if (e.Command == wallpaperChangerVM.StartCyclingCommand || e.Command == wallpaperChangerVM.CycleNextCommand) {
        if (this.HandleCycleException(e.Exception)) {
          e.IsHandled = true;
          return;
        }
      }
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Application.DispatcherUnhandledException" /> event of an 
    ///   <see cref="System.Windows.Application" /> instance and represents the last point where exceptions are handled.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Application_DispatcherUnhandledException(Object sender, DispatcherUnhandledExceptionEventArgs e) {
      if (e.Handled) {
        return;
      }

      #if !DEBUG
      if (e.Exception is SecurityException) {
        DialogManager.ShowGeneral_MissingFrameworkRights(this.MainWindow, e.Exception.ToString());
      } else if (e.Exception is DirectoryNotFoundException) {
        DialogManager.ShowGeneral_DirectoryNotFound(this.MainWindow, null);
      } else if (e.Exception is FileNotFoundException) {
        DialogManager.ShowGeneral_FileNotFound(this.MainWindow, (e.Exception as FileNotFoundException).FileName);
      } else if (e.Exception is OutOfMemoryException) {
        DialogManager.ShowGeneral_OutOfMemory(this.MainWindow, e.Exception.ToString());
      } else {
        DialogManager.ShowGeneral_UnhandledException(this.MainWindow, e.Exception.ToString());
      }

      e.Handled = true;
      #else
      Debug.Write("Exception ");
      Debug.WriteLine(e.Exception.ToString());
      Debug.Flush();
      #endif
    }

    /// <summary>
    ///   Handles a cycle related <see cref="Exception" />.
    /// </summary>
    /// <remarks>
    ///   When <paramref name="occurredWhenAutocycling" /> is set to <c>false</c>, then this method will disable the autocycling 
    ///   of wallpapers.
    /// </remarks>
    /// <param name="exception">
    ///   The exception being thrown.
    /// </param>
    /// <param name="occurredWhenAutocycling">
    ///   Indicates whether the exception occurred on an autocycle.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the exception has been handled or not.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="exception" /> is <c>null</c>
    /// </exception>
    protected Boolean HandleCycleException(Exception exception, Boolean occurredWhenAutocycling = false) {
      if (exception == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("exception"));
      }

      if (exception is InvalidOperationException) {
        if ((occurredWhenAutocycling) && (this.WallpaperChanger.IsAutocycling)) {
          this.WallpaperChanger.StopCycling();
          DialogManager.ShowCycle_MissingWallpapers(this.MainWindow, true);
        } else {
          DialogManager.ShowCycle_MissingWallpapers(this.MainWindow, false);
        }

        return true;
      } 
      
      if (exception is FileNotFoundException) {
        DialogManager.ShowGeneral_FileNotFound(this.MainWindow, ((FileNotFoundException)exception).FileName);
        return true;
      }
      if (exception is NotSupportedException) {
        // Thrown when the user either performed a manual cycle just when a random cycle occurs by the timer or when the user
        // manually cycles too quickly. We can safely ignore this exception here.
        DialogManager.ShowGeneral_UnhandledException(this.MainWindow, exception.ToString());

        return true;
      }

      return false;
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
    
    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    private Boolean isDisposed;

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          if (this.propertyBindingManager != null) {
            this.propertyBindingManager.Dispose();
          }
          if (this.notifyIcon != null) {
            this.notifyIcon.Dispose();
          }
          if (this.MainWindow != null) {
            this.MainWindow.Dispose();
          }
        }
      }
      
      this.isDisposed = true;
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="Application" /> class.
    /// </summary>
    ~Application() {
      this.Dispose(false);
    }
    #endregion
  }
}
