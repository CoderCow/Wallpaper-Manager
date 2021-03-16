// Note: This options affect the Graphical User Interface only.
//#define EmulateSinglescreenSystem
//#define EmulateWindowsVista
//#define EmulateWindowsXP

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Security;

using Common.Text;

namespace Common {
  /// <summary>
  ///   Provides serveral data related to the current application's environment.
  /// </summary>
  /// <remarks>
  ///   It is recommended to inherit a class with the same name for any new application using the Common Library, and 
  ///   implementing more specific application data such as paths of commonly used files etc.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [SecuritySafeCritical]
  public class AppEnvironment {
    #region Static Property: OsVersion
    /// <summary>
    ///   <inheritdoc cref="OSVersion" select='../value/node()' />
    /// </summary>
    private static Version osVersion;

    /// <summary>
    ///   Gets the Operating System's <see cref="Version" />.
    /// </summary>
    /// <value>
    ///   The Operating System's <see cref="Version" />.
    /// </value>
    public static Version OSVersion {
      get { return AppEnvironment.osVersion; }
    }
    #endregion

    #region Static Property: UserName
    /// <summary>
    ///   <inheritdoc cref="UserName" select='../value/node()' />
    /// </summary>
    private static String userName;

    /// <summary>
    ///   Gets the name of the current user being logged on.
    /// </summary>
    /// <value>
    ///   The name of the current user being logged on.
    /// </value>
    /// <inheritdoc cref="Environment.UserName" select='permission|exception' />
    public static String UserName {
      get {
        if (AppEnvironment.userName != null) {
          AppEnvironment.userName = Environment.UserName;
        }

        return AppEnvironment.userName;
      }
    }
    #endregion

    #region Static Property: MachineName
    /// <summary>
    ///   <inheritdoc cref="MachineName" select='../value/node()' />
    /// </summary>
    private static String machineName;

    /// <summary>
    ///   Gets the NetBIOS name of the computer.
    /// </summary>
    /// <value>
    ///   The NetBIOS name of the computer.
    /// </value>
    /// <inheritdoc cref="Environment.MachineName" select='permission|exception' />
    public static String MachineName {
      get {
        if (AppEnvironment.machineName == null) {
          AppEnvironment.machineName = Environment.MachineName;
        }

        return AppEnvironment.machineName;
      }
    }
    #endregion

    #region Static Property: EngineVersion
    private static Version engineVersion;

    public static Version EngineVersion {
      get {
        if (AppEnvironment.engineVersion == null) {
          AppEnvironment.engineVersion = Assembly.GetAssembly(typeof(AppEnvironment)).GetName().Version;
        }

        return AppEnvironment.engineVersion;
      }
    }
    #endregion

    #region Static Property: GeneralAppDataPath
    /// <summary>
    ///   <inheritdoc cref="GeneralAppDataPath" select='../value/node()' />
    /// </summary>
    private static String generalAppDataPath;

    /// <summary>
    ///   Gets the common repository for application-specific data for the current roaming user.
    /// </summary>
    /// <value>
    ///   The common repository for application-specific data for the current roaming user.
    /// </value>
    public static String GeneralAppDataPath {
      get {
        if (AppEnvironment.generalAppDataPath == null) {
          AppEnvironment.generalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        return AppEnvironment.generalAppDataPath;
      }
    }
    #endregion

    #region Static Property: GeneralLocalAppDataPath
    /// <summary>
    ///   <inheritdoc cref="GeneralLocalAppDataPath" select='../value/node()' />
    /// </summary>
    private static String generalLocalAppDataPath;

    /// <summary>
    ///   Gets the common repository for application-specific data for the current local user.
    /// </summary>
    /// <value>
    ///   The common repository for application-specific data for the current local user.
    /// </value>
    public static String GeneralLocalAppDataPath {
      get {
        if (AppEnvironment.generalLocalAppDataPath == null) {
          AppEnvironment.generalLocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        return AppEnvironment.generalLocalAppDataPath;
      }
    }
    #endregion

    #region Static Property: GeneralCommonAppDataPath
    /// <summary>
    ///   <inheritdoc cref="GeneralCommonAppDataPath" select='../value/node()' />
    /// </summary>
    private static String generalCommonAppDataPath;

    /// <summary>
    ///   Gets the common repository for application-specific data for all users.
    /// </summary>
    /// <value>
    ///   The common repository for application-specific data for all users.
    /// </value>
    public static String GeneralCommonAppDataPath {
      get {
        if (AppEnvironment.generalCommonAppDataPath == null) {
          AppEnvironment.generalCommonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        return AppEnvironment.generalCommonAppDataPath;
      }
    }
    #endregion

    #region Static Property: CurrentDirectory
    /// <summary>
    ///   Gets or sets the the current directory of the application's environment.
    /// </summary>
    /// <value>
    ///   The the current directory of the application's environment.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <inheritdoc cref="Environment.CurrentDirectory" select='permission|exception' />
    public static String CurrentDirectory {
      get { return Environment.CurrentDirectory; }
      set { 
        if (value == null) throw new ArgumentNullException();
        if (value.Length == 0 || value.Length > 260) throw new ArgumentOutOfRangeException();

        Environment.CurrentDirectory = value;
      }
    }
    #endregion

    #region Property: AppAssembly
    /// <summary>
    ///   <inheritdoc cref="AppAssembly" select='../value/node()' />
    /// </summary>
    private readonly Assembly appAssembly;

    /// <summary>
    ///   Gets the <see cref="Assembly" /> of the current application.
    /// </summary>
    /// <value>
    ///   The <see cref="Assembly" /> of the current application.
    /// </value>
    /// <seealso cref="Assembly">Assembly Class</seealso>
    public Assembly AppAssembly {
      get { return this.appAssembly; }
    }
    #endregion

    #region Property: AppAssemblyName
    /// <summary>
    ///   <inheritdoc cref="AppAssemblyName" select='../value/node()' />
    /// </summary>
    private readonly AssemblyName appAssemblyName;

    /// <summary>
    ///   Gets the <see cref="AssemblyName" /> of the current application's <see cref="Assembly" />.
    /// </summary>
    /// <value>
    ///   The <see cref="AssemblyName" /> of the current application's <see cref="Assembly" />.
    /// </value>
    /// <seealso cref="AssemblyName">AssemblyName Class</seealso>
    /// <seealso cref="Assembly">Assembly Class</seealso>
    public AssemblyName AppAssemblyName {
      get { return this.appAssemblyName; }
    }
    #endregion

    #region Property: AppName
    /// <summary>
    ///   Gets the name of the application's assembly.
    /// </summary>
    /// <value>
    ///   The name of the application's assembly.
    /// </value>
    public virtual String AppName {
      get { return this.AppAssemblyName.Name; }
    }
    #endregion

    #region Property: AppVersion
    /// <summary>
    ///   Gets the version number of the application's assembly.
    /// </summary>
    /// <value>
    ///   The version number of the application's assembly.
    /// </value>
    public virtual Version AppVersion {
      get { return this.AppAssemblyName.Version; }
    }
    #endregion

    #region Property: AppArguments
    /// <summary>
    ///   <inheritdoc cref="AppArguments" select='../value/node()' />
    /// </summary>
    private static AppArgumentCollection appArguments;

    /// <summary>
    ///   Gets the collection of arguments passed to the application when it got started.
    /// </summary>
    /// <value>
    ///   The collection of arguments passed to the application when it got started.
    /// </value>
    public AppArgumentCollection AppArguments {
      get { return AppEnvironment.appArguments; }
    }
    #endregion

    #region Property: AppExePath
    /// <summary>
    ///   <inheritdoc cref="AppExePath" select='../value/node()' />
    /// </summary>
    private String appExePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's executable.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's executable.
    /// </value>
    public virtual String AppExePath {
      get { 
        if (this.appExePath == null) {
          this.appExePath = this.AppAssembly.Location;
        }

        return this.appExePath;
      }
    }
    #endregion

    #region Property: AppPath
    /// <summary>
    ///   <inheritdoc cref="AppPath" select='../value/node()' />
    /// </summary>
    private String appPath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application executable's directory.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application executable's directory.
    /// </value>
    public virtual String AppPath {
      get {
        if (this.appPath == null) {
          this.appPath = Path.GetDirectoryName(this.AppExePath);
        }

        return this.appPath;
      }
    }
    #endregion
    
    #region Property: AppDataPath
    /// <summary>
    ///   <inheritdoc cref="AppDataPath" select='../value/node()' />
    /// </summary>
    private String appDataPath;

    /// <summary>
    ///   Gets the application's data Path for the current roaming user.
    /// </summary>
    /// <value>
    ///   The application's data Path for the current roaming user.
    /// </value>
    public virtual String AppDataPath {
      get {
        if (this.appDataPath == null) {
          this.appDataPath = Path.Combine(AppEnvironment.GeneralAppDataPath, this.AppName);
        }

        return this.appDataPath;
      }
    }
    #endregion

    #region Property: LocalAppDataPath
    /// <summary>
    ///   <inheritdoc cref="LocalAppDataPath" select='../value/node()' />
    /// </summary>
    private String localAppDataPath;

    /// <summary>
    ///   Gets the application's data Path for the current local user.
    /// </summary>
    /// <value>
    ///   The application's data Path for the current local user.
    /// </value>
    public virtual String LocalAppDataPath {
      get {
        if (this.localAppDataPath == null) {
          this.localAppDataPath = Path.Combine(AppEnvironment.GeneralLocalAppDataPath, this.AppName);
        }

        return this.localAppDataPath;
      }
    }
    #endregion

    #region Property: CommonAppDataPath
    /// <summary>
    ///   <inheritdoc cref="CommonAppDataPath" select='../value/node()' />
    /// </summary>
    private String commonAppDataPath;

    /// <summary>
    ///   Gets the application's data Path for all users.
    /// </summary>
    /// <value>
    ///   The application's data Path for all users.
    /// </value>
    public virtual String CommonAppDataPath {
      get {
        if (this.commonAppDataPath == null) {
          this.commonAppDataPath = Path.Combine(AppEnvironment.GeneralCommonAppDataPath, this.AppName);
        }

        return this.commonAppDataPath;
      }
    }
    #endregion

    #region Property: SystemTempPath
    /// <summary>
    ///   <inheritdoc cref="SystemTempPath" select='../value/node()' />
    /// </summary>
    private String systemTempPath;

    /// <summary>
    ///   Gets the temporary directory of the operating system.
    /// </summary>
    /// <value>
    ///   The temporary directory of the operating system.
    /// </value>
    public virtual String SystemTempPath {
      get {
        if (this.systemTempPath == null) {
          this.systemTempPath = Path.GetTempPath();
        }

        return this.systemTempPath;
      }
    }
    #endregion


    #region Methods: Static Constructor, Constructor
    /// <summary>
    ///   Initializes static members of the <see cref="AppEnvironment">AppEnvironment Class</see>.
    /// </summary>
    static AppEnvironment() {
      AppEnvironment.osVersion = Environment.OSVersion.Version;
      AppEnvironment.appArguments = new AppArgumentCollection(Environment.GetCommandLineArgs());
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="AppEnvironment">AppEnvironment Class</see>.
    /// </summary>
    /// <remarks>
    ///   This class is not meant to be initialized.
    /// </remarks>
    /// <param name="appAssembly">
    ///   <inheritdoc cref="AppAssembly" select='../value/node()' />
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="appAssembly" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="Assembly">Assembly Class</seealso>
    public AppEnvironment(Assembly appAssembly) {
      if (appAssembly == null) throw new ArgumentNullException();

      this.appAssembly = appAssembly;
      this.appAssemblyName = appAssembly.GetName();
    }
    #endregion

    #region Method: TraceWriteAppInfo
    /// <summary>
    ///   Writes application and operating system related information to the debug output if the <c>Trace</c> symbol is 
    ///   defined.
    /// </summary>
    [Conditional("TRACE")]
    public void TraceWriteAppInfo() {
      Trace.WriteLine("-- Environment Info --");
      Trace.Indent();
      this.TraceWriteAppInfoInternal();
      Trace.Unindent();
      Trace.WriteLine("----------------------");
    }

    /// <summary>
    ///   Writes raw application and operating system related information to the debug output.
    /// </summary>
    protected virtual void TraceWriteAppInfoInternal() {
      Trace.WriteLine("Application version: " + this.AppVersion);
      Trace.WriteLine("Application arguments: " + StringGenerator.FromList(this.AppArguments));
      Trace.WriteLine("OS version: " + AppEnvironment.OSVersion);
    }
    #endregion
  }
}