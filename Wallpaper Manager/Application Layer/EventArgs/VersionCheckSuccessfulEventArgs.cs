// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;

using Common;

using WallpaperManager.Data;

namespace WallpaperManager.Application {
  /// <summary>
  ///   Provides data to a successful version check done by the <see cref="UpdateManager" /> class.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class VersionCheckSuccessfulEventArgs: EventArgs {
    #region Property: IsUpdate
    /// <summary>
    ///   <inheritdoc cref="IsUpdate" select='../value/node()' />
    /// </summary>
    private readonly Boolean isUpdate;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the version on the server is greater than the current application's 
    ///   version or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the version on the server is greater than the current application's 
    ///   version or not.
    /// </value>
    public Boolean IsUpdate {
      get { return this.isUpdate; }
    }
    #endregion

    #region Property: Version
    /// <summary>
    ///   <inheritdoc cref="Version" select='../value/node()' />
    /// </summary>
    private readonly Version version;

    /// <summary>
    ///   Gets the <see cref="Version" /> instance representing the available application version on the server.
    /// </summary>
    /// <value>
    ///   The <see cref="Version" /> instance representing the available application version on the server.
    /// </value>
    public Version Version {
      get { return this.version; }
    }
    #endregion

    #region Property: CriticalMessage
    /// <summary>
    ///   <inheritdoc cref="CriticalMessage" select='../value/node()' />
    /// </summary>
    private readonly String criticalMessage;

    /// <summary>
    ///   Gets the critical message attached with the version info provided by the update server.
    /// </summary>
    /// <value>
    ///   The critical message attached with the version info provided by the update server.
    /// </value>
    public String CriticalMessage {
      get { return this.criticalMessage; }
    }
    #endregion

    #region Property: InfoMessage
    /// <summary>
    ///   <inheritdoc cref="InfoMessage" select='../value/node()' />
    /// </summary>
    private readonly String infoMessage;

    /// <summary>
    ///   Gets the info message attached with the version info provided by the update server.
    /// </summary>
    /// <value>
    ///   The info message attached with the version info provided by the update server.
    /// </value>
    public String InfoMessage {
      get { return this.infoMessage; }
    }
    #endregion


    #region Method: Constructor, ToString
    /// <summary>
    ///   Initializes a new instance of the <see cref="VersionCheckSuccessfulEventArgs" /> class.
    /// </summary>
    /// <param name="isUpdate">
    ///   A <see cref="Boolean" /> indicating whether the version on the server is greater than the current application's 
    ///   version or not.
    /// </param>
    /// <param name="version">
    ///   The <see cref="Version" /> instance representing the available application version on the server.
    /// </param>
    /// <param name="criticalMessage">
    ///   The critical message attached with the version info provided by the update server.
    /// </param>
    /// <param name="infoMessage">
    ///   The info message attached with the version info provided by the update server.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="version" /> is <c>null</c>.
    /// </exception>
    public VersionCheckSuccessfulEventArgs(Boolean isUpdate, Version version, String criticalMessage, String infoMessage) {
      if (version == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("version"));
      }

      this.isUpdate = isUpdate;
      this.version = version;
      this.criticalMessage = criticalMessage;
      this.infoMessage = infoMessage;
    }

    /// <inheritdoc />
    public override String ToString() {
      return StringGenerator.FromListKeyed(
        new String[] { "Version", "Critical Message", "Info Message" },
        new Object[] { this.Version, this.CriticalMessage, this.InfoMessage }
      );
    }
    #endregion
  }
}