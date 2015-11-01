// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

using Path = Common.IO.Path;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Provides serveral Wallpaper Manager environment data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class AppEnvironment: Common.AppEnvironment {
    #region Constants: AppGuid
    /// <summary>
    ///   Represents the Global Unique Identifier of the application.
    /// </summary>
    /// <remarks>
    ///   This value could also be reflected from the assembly at run time, but since that could maybe throw
    ///   exceptions etc. it is better to simply define it as a constant.
    /// </remarks>
    public const String AppGuid = "{47e92347-58f0-4881-ab6b-2f9fc2362b76}";
    #endregion

    #region Constants: DebugFilename, ConfigurationFilename, ChangelogFileName, AppliedWallpaperFilename
    /// <summary>
    ///   Represents the filename of the application's configuration file.
    /// </summary>
    private const String DebugFilename = "Debug.txt";
    
    /// <summary>
    ///   Represents the filename of the application's configuration file.
    /// </summary>
    private const String ConfigurationFilename = "Configuration.xml";

    /// <summary>
    ///   Represents the file name of the changelog file.
    /// </summary>
    private const String ChangelogFileName = "Changelog.txt";

    /// <summary>
    ///   Represents the filename of the last generated wallpaper image.
    /// </summary>
    private const String AppliedWallpaperFilename = "Applied Wallpaper.bmp";
    #endregion

    #region Constants: ExeParam_DefaultSettings, ExeParam_NoAutoTerminate
    /// <summary>
    ///   Represents the executeable parameter yielding to use the default settings instead of loading the configuration file.
    /// </summary>
    public const String Argument_DefaultSettings = "-defaultsettings";

    /// <summary>
    ///   Represents the executeable parameter yielding to ignore the <see cref="GeneralConfig.TerminateAfterStartup" /> 
    ///   configuration option.
    /// </summary>
    public const String Argument_NoAutoTerminate = "-noautoterminate";
    #endregion

    #region Constants: WebsiteUrl
    /// <summary>
    ///   Represents the adress of the Wallpaper Manager - Homepage.
    /// </summary>
    public const String WebsiteUrl = @"http://www.WallpaperManager.de.vu";
    #endregion

    #region Static Properties: IsMultiscreenSystem, IsWindowsVista, IsWindows7
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the user's system has multiple screens.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the user's system has multiple screens.
    /// </value>
    public static Boolean IsMultiscreenSystem {
      #if EmulateSinglescreenSystem
      get { return false; }
      #else
      get { return (System.Windows.Forms.Screen.AllScreens.Length > 1); }
      #endif
    }

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the user's operating system is at least Windows Vista.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the user's operating system is at least Windows Vista.
    /// </value>
    public static Boolean IsWindowsVista {
      #if EmulateWindowsXP
      get { return false; }
      #else
      get { return (AppEnvironment.OSVersion.Major >= 6); }
      #endif
    }

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the user's operating system is at least Windows 7.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the user's operating system is at least Windows 7.
    /// </value>
    public static Boolean IsWindows7 {
      #if (EmulateWindowsXP || EmulateWindowsVista)
      get { return false; }
      #else
      get { return (AppEnvironment.IsWindowsVista && AppEnvironment.OSVersion.Minor > 0); }
      #endif
    }
    #endregion

    #region Property: DebugFilePath
    /// <summary>
    ///   <inheritdoc cref="DebugFilePath" select='../value/node()' />
    /// </summary>
    private Path debugFilePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's debug file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's debug file.
    /// </value>
    public Path DebugFilePath {
      get {
        if (this.debugFilePath == Path.None) {
          this.debugFilePath = Path.Concat(this.AppPath, AppEnvironment.DebugFilename);
        }

        return this.debugFilePath;
      }
    }
    #endregion

    #region Property: ConfigFilePath
    /// <summary>
    ///   <inheritdoc cref="ConfigFilePath" select='../value/node()' />
    /// </summary>
    private Path configFilePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's configuration file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's configuration file.
    /// </value>
    /// <remarks>
    ///   This <see cref="Path" /> usually points to a configuration Wallpaper Manager directory in the common repository for 
    ///   application-specific data for the current roaming user. However, if the LocalConfig symbol is defined, it will point to a 
    ///   configuration file which is in the same directory as the executeable file.
    /// </remarks>
    public Path ConfigFilePath {
      get {
        if (this.configFilePath == Path.None) {
          #if !LocalConfig
          this.configFilePath = Path.Concat(this.AppDataPath, AppEnvironment.ConfigurationFilename);
          #else
          this.configFilePath = Path.Concat(this.AppPath, AppEnvironment.ConfigurationFilename);
          #endif
        }

        return this.configFilePath;
      }
    }
    #endregion

    #region Property: ChangelogFilePath
    /// <summary>
    ///   <inheritdoc cref="ChangelogFilePath" select='../value/node()' />
    /// </summary>
    private Path changelogFilePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's changelog file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's changelog file.
    /// </value>
    public Path ChangelogFilePath {
      get {
        if (this.changelogFilePath == Path.None) {
          this.changelogFilePath = Path.Concat(this.AppPath, AppEnvironment.ChangelogFileName);
        }

        return this.changelogFilePath;
      }
    }
    #endregion

    #region Property: AppliedWallpaperFilePath
    /// <summary>
    ///   <inheritdoc cref="AppliedWallpaperFilePath" select='../value/node()' />
    /// </summary>
    private Path appliedWallpaperFilePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </value>
    public Path AppliedWallpaperFilePath {
      get {
        if (this.appliedWallpaperFilePath == Path.None) {
          this.appliedWallpaperFilePath = Path.Concat(this.AppDataPath, AppEnvironment.AppliedWallpaperFilename);
        }

        return this.appliedWallpaperFilePath;
      }
    }
    #endregion

    #region Property: IsUseDefaultSettingsDefined
    /// <summary>
    ///   <inheritdoc cref="IsUseDefaultSettingsDefined" select='../value/node()' />
    /// </summary>
    private readonly Boolean isUseDefaultSettingsDefined;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the <see cref="Argument_DefaultSettings" /> argument is defined 
    ///   or not.
    /// </summary>
    /// <value>
    ///   Indicates whether the <see cref="Argument_DefaultSettings" /> argument is defined or not.
    /// </value>
    public Boolean IsUseDefaultSettingsDefined {
      get { return this.isUseDefaultSettingsDefined; }
    }
    #endregion

    #region Property: IsNoAutoTerminateDefined
    /// <summary>
    ///   <inheritdoc cref="IsNoAutoTerminateDefined" select='../value/node()' />
    /// </summary>
    private Boolean isNoAutoTerminateDefined;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the <see cref="Argument_NoAutoTerminate" /> argument is 
    ///   defined or not.
    /// </summary>
    /// <value>
    ///   Indicates whether the <see cref="Argument_NoAutoTerminate" /> argument is defined or not.
    /// </value>
    public Boolean IsNoAutoTerminateDefined {
      get { return this.isNoAutoTerminateDefined; }
      set { this.isNoAutoTerminateDefined = value; }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="AppEnvironment" /> class.
    /// </summary>
    public AppEnvironment(): base(Assembly.GetAssembly(typeof(AppEnvironment))) {
      this.isUseDefaultSettingsDefined = (this.AppArguments.Contains(AppEnvironment.Argument_DefaultSettings));
      this.isNoAutoTerminateDefined = (this.AppArguments.Contains(AppEnvironment.Argument_NoAutoTerminate));
    }
    #endregion

    #region Method: DebugWriteAppInfoInternal
    /// <inheritdoc />
    protected override void DebugWriteAppInfoInternal() {
      base.DebugWriteAppInfoInternal();

      Debug.WriteLine("Multiscreen system: " + AppEnvironment.IsMultiscreenSystem);
      Debug.WriteLine("Use default settings defined: " + this.IsUseDefaultSettingsDefined);
      Debug.WriteLine("No Auto termination defined: " + this.IsNoAutoTerminateDefined);
    }
    #endregion

    #region Static Method: IconFromEmbeddedResource
    /// <summary>
    ///   Gets an <see cref="Icon" /> instance from an embedded icons resource.
    /// </summary>
    /// <param name="resourceName">
    ///   The name of the resource.
    /// </param>
    /// <returns>
    ///   A new <see cref="Icon" /> instance created from the embedded resource or <c>null</c> if the resource was not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="resourceName" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="resourceName" /> is an empty string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   An error occurred when requesting the resource. See inner exception for details.
    /// </exception>
    /// <inheritdoc cref="Assembly.GetManifestResourceStream(String)" select='exception/*' />
    public static Icon IconFromEmbeddedResource(String resourceName) {
      if (resourceName == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("resourceName"));
      }

      Assembly thisAssembly = Assembly.GetAssembly(typeof(AppEnvironment));

      Stream iconStream = null;
      try {
        // TODO: Check for resource existence instead of catchting exceptions.
        iconStream = thisAssembly.GetManifestResourceStream(resourceName);
        if (iconStream == null) {
          return null;
        }

        return new Icon(iconStream);
      } catch (Exception exception) {
        // If the resource was not found, return null.
        if (exception is FileLoadException || exception is FileNotFoundException) {
          return null;
        }
        if (exception is ArgumentException) {
          throw;
        }

        throw new InvalidOperationException(
          String.Format(CultureInfo.CurrentCulture, "Resource \"{0}\" could not be resolved.", resourceName), exception
        );
      } finally {
        if (iconStream != null) {
          iconStream.Dispose();
        }
      }
    }
    #endregion
  }
}