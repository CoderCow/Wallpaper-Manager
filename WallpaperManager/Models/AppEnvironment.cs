// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Provides serveral Wallpaper Manager environment data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class AppEnvironment : Common.AppEnvironment {
    /// <summary>
    ///   Represents the Global Unique Identifier of the application.
    /// </summary>
    /// <remarks>
    ///   This value could also be reflected from the assembly at run time, but since that could maybe throw
    ///   exceptions etc. it is better to simply define it as a constant.
    /// </remarks>
    public const string AppGuid = "{47e92347-58f0-4881-ab6b-2f9fc2362b76}";

    /// <summary>
    ///   Represents the adress of the Wallpaper Manager - Homepage.
    /// </summary>
    public const string WebsiteUrl = @"http://www.WallpaperManager.de.vu";

    /// <summary>
    ///   Represents the filename of the application's configuration file.
    /// </summary>
    private const string DebugFilename = "Debug.txt";

    /// <summary>
    ///   Represents the filename of the application's configuration file.
    /// </summary>
    private const string ConfigurationFilename = "Configuration.xml";

    /// <summary>
    ///   Represents the file name of the changelog file.
    /// </summary>
    private const string ChangelogFileName = "Changelog.txt";

    /// <summary>
    ///   Represents the filename of the last generated wallpaper image.
    /// </summary>
    private const string AppliedWallpaperFilename = "Applied Wallpaper.bmp";

    /// <summary>
    ///   Represents the executeable parameter yielding to use the default settings instead of loading the configuration file.
    /// </summary>
    public const string Argument_DefaultSettings = "-defaultsettings";

    /// <summary>
    ///   Represents the executeable parameter yielding to ignore the <see cref="GeneralConfig.TerminateAfterStartup" />
    ///   configuration option.
    /// </summary>
    public const string Argument_NoAutoTerminate = "-noautoterminate";

    /// <summary>
    ///   <inheritdoc cref="AppliedWallpaperFilePath" select='../value/node()' />
    /// </summary>
    private Path appliedWallpaperFilePath;

    /// <summary>
    ///   <inheritdoc cref="ChangelogFilePath" select='../value/node()' />
    /// </summary>
    private Path changelogFilePath;

    /// <summary>
    ///   <inheritdoc cref="ConfigFilePath" select='../value/node()' />
    /// </summary>
    private Path configFilePath;

    /// <summary>
    ///   <inheritdoc cref="DebugFilePath" select='../value/node()' />
    /// </summary>
    private Path debugFilePath;

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the user's system has multiple screens.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the user's system has multiple screens.
    /// </value>
    public static bool IsMultiscreenSystem {
#if EmulateSinglescreenSystem
      get { return false; }
      #else
      get { return (Screen.AllScreens.Length > 1); }
#endif
    }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the user's operating system is at least Windows Vista.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the user's operating system is at least Windows Vista.
    /// </value>
    public static bool IsWindowsVista {
#if EmulateWindowsXP
      get { return false; }
      #else
      get { return (AppEnvironment.OSVersion.Major >= 6); }
#endif
    }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the user's operating system is at least Windows 7.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the user's operating system is at least Windows 7.
    /// </value>
    public static bool IsWindows7 {
#if (EmulateWindowsXP || EmulateWindowsVista)
      get { return false; }
      #else
      get { return (AppEnvironment.IsWindowsVista && AppEnvironment.OSVersion.Minor > 0); }
#endif
    }

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's debug file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's debug file.
    /// </value>
    public Path DebugFilePath {
      get {
        if (this.debugFilePath == Path.None)
          this.debugFilePath = Path.Concat(this.AppPath, AppEnvironment.DebugFilename);

        return this.debugFilePath;
      }
    }

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's configuration file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's configuration file.
    /// </value>
    /// <remarks>
    ///   This <see cref="Path" /> usually points to a configuration Wallpaper Manager directory in the common repository for
    ///   application-specific data for the current roaming user. However, if the LocalConfig symbol is defined, it will point
    ///   to a
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

    /// <summary>
    ///   Gets the <see cref="Path" /> of the application's changelog file.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the application's changelog file.
    /// </value>
    public Path ChangelogFilePath {
      get {
        if (this.changelogFilePath == Path.None)
          this.changelogFilePath = Path.Concat(this.AppPath, AppEnvironment.ChangelogFileName);

        return this.changelogFilePath;
      }
    }

    /// <summary>
    ///   Gets the <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </value>
    public Path AppliedWallpaperFilePath {
      get {
        if (this.appliedWallpaperFilePath == Path.None)
          this.appliedWallpaperFilePath = Path.Concat(this.AppDataPath, AppEnvironment.AppliedWallpaperFilename);

        return this.appliedWallpaperFilePath;
      }
    }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the <see cref="Argument_DefaultSettings" /> argument is defined
    ///   or not.
    /// </summary>
    /// <value>
    ///   Indicates whether the <see cref="Argument_DefaultSettings" /> argument is defined or not.
    /// </value>
    public bool IsUseDefaultSettingsDefined { get; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the <see cref="Argument_NoAutoTerminate" /> argument is
    ///   defined or not.
    /// </summary>
    /// <value>
    ///   Indicates whether the <see cref="Argument_NoAutoTerminate" /> argument is defined or not.
    /// </value>
    public bool IsNoAutoTerminateDefined { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="AppEnvironment" /> class.
    /// </summary>
    public AppEnvironment() : base(Assembly.GetAssembly(typeof(AppEnvironment))) {
      this.IsUseDefaultSettingsDefined = (this.AppArguments.Contains(AppEnvironment.Argument_DefaultSettings));
      this.IsNoAutoTerminateDefined = (this.AppArguments.Contains(AppEnvironment.Argument_NoAutoTerminate));
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.DebugFilePath != Path.None);
      Contract.Invariant(this.ConfigFilePath != Path.None);
      Contract.Invariant(this.ChangelogFilePath != Path.None);
      Contract.Invariant(this.AppliedWallpaperFilePath != Path.None);
    }

    /// <inheritdoc />
    protected override void DebugWriteAppInfoInternal() {
      base.DebugWriteAppInfoInternal();

      Debug.WriteLine("Multiscreen system: " + AppEnvironment.IsMultiscreenSystem);
      Debug.WriteLine("Use default settings defined: " + this.IsUseDefaultSettingsDefined);
      Debug.WriteLine("No Auto termination defined: " + this.IsNoAutoTerminateDefined);
    }

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
    /// <inheritdoc cref="Assembly.GetManifestResourceStream(string)" select='exception/*' />
    public static Icon IconFromEmbeddedResource(string resourceName) {
      Contract.Requires<ArgumentNullException>(resourceName != null);

      Assembly thisAssembly = Assembly.GetAssembly(typeof(AppEnvironment));
      Stream iconStream = null;
      try {
        // TODO: Check for resource existence instead of catchting exceptions.
        iconStream = thisAssembly.GetManifestResourceStream(resourceName);
        if (iconStream == null)
          return null;

        return new Icon(iconStream);
      } catch (Exception exception) {
        // If the resource was not found, return null.
        if (exception is FileLoadException || exception is FileNotFoundException)
          return null;
        if (exception is ArgumentException)
          throw;

        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resource \"{0}\" could not be resolved.", resourceName), exception);
      } finally {
        iconStream?.Dispose();
      }
    }
  }
}