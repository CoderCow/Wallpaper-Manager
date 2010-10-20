// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Navigation;

using Common.Presentation;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///  The About Window showing application related information.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public partial class AboutWindow: Window {
    #region Constants: VersionStringFormat
    /// <summary>
    ///   Represents the format string for the <see cref="VersionFormatConverter" />.
    /// </summary>
    #if BetaBuild
    protected const String VersionStringFormat = @"{0}.{1} Beta {3}";
    #else
    protected const String VersionStringFormat = @"{0}.{1} Revision {3}";
    #endif

    /// <summary>
    ///   Represents the format short string for the <see cref="VersionFormatConverter" />.
    /// </summary>
    protected const String VersionStringFormatShort = @"{0}.{1}";
    #endregion

    #region Property: Environment
    /// <summary>
    ///   <inheritdoc cref="Environment" select='../value/node()' />
    /// </summary>
    private readonly AppEnvironment environment;

    /// <summary>
    ///   Gets the <see cref="AppEnvironment" /> instance providing the version info to be displayed in this window.
    /// </summary>
    /// <value>
    ///   The <see cref="AppEnvironment" /> instance providing the version info to be displayed in this window.
    /// </value>
    /// <seealso cref="AppEnvironment">Environment Class</seealso>
    public AppEnvironment Environment {
      get { return this.environment; }
    }
    #endregion

    #region Property: VersionFormatConverter
    /// <summary>
    ///   Gets the <see cref="VersionFormatConverter" /> instance used to convert the application's version 
    ///   number to a pretty string.
    /// </summary>
    /// <value>
    ///   The <see cref="VersionFormatConverter" /> instance used to convert the application's version number 
    ///   to a pretty string.
    /// </value>
    /// <seealso cref="VersionFormatConverter">VersionFormatConverter Class</seealso>
    public VersionFormatConverter VersionFormatConverter {
      get { return (VersionFormatConverter)this.Resources["VersionFormatConverter"]; }
    }
    #endregion


    #region Methods: Constructor, Hyperlink_RequestNavigate
    /// <summary>
    ///   Initializes a new instance of the <see cref="AboutWindow" /> class.
    /// </summary>
    /// <param name="environment">
    ///   The <see cref="AppEnvironment" /> instance providing the version info to be displayed in this window.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="environment" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="AppEnvironment">Environment Class</seealso>
    public AboutWindow(AppEnvironment environment) {
      if (environment == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("Environment"));
      }

      this.environment = environment;
      this.InitializeComponent();

      if (environment.AppVersion.Revision != 0) {
        this.VersionFormatConverter.StringFormat = AboutWindow.VersionStringFormat;
      } else {
        this.VersionFormatConverter.StringFormat = AboutWindow.VersionStringFormatShort;
      }

      BindingOperations.GetBindingExpression(this.txtAppVersion, TextBlock.TextProperty).UpdateTarget();
    }

    /// <summary>
    ///   Handles the <see cref="Hyperlink.RequestNavigate" /> event of a <see cref="Hyperlink">Hyperlink</see> instance.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    /// <seealso cref="Hyperlink">Hyperlink Class</seealso>
    private void Hyperlink_RequestNavigate(Object sender, RequestNavigateEventArgs e) {
      Hyperlink hyperlink = (sender as Hyperlink);

      if (hyperlink != null) {
        Process.Start(hyperlink.Tag.ToString());
      }
    }
    #endregion
  }
}
