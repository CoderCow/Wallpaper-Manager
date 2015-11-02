// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Provides data to a successful version check done by the <see cref="UpdateManager" /> class.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class VersionCheckSuccessfulEventArgs : EventArgs {
    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the version on the server is greater than the current application's
    ///   version or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the version on the server is greater than the current application's
    ///   version or not.
    /// </value>
    public bool IsUpdate { get; }

    /// <summary>
    ///   Gets the <see cref="Version" /> instance representing the available application version on the server.
    /// </summary>
    /// <value>
    ///   The <see cref="Version" /> instance representing the available application version on the server.
    /// </value>
    public Version Version { get; }

    /// <summary>
    ///   Gets the critical message attached with the version info provided by the update server.
    /// </summary>
    /// <value>
    ///   The critical message attached with the version info provided by the update server.
    /// </value>
    public string CriticalMessage { get; }

    /// <summary>
    ///   Gets the info message attached with the version info provided by the update server.
    /// </summary>
    /// <value>
    ///   The info message attached with the version info provided by the update server.
    /// </value>
    public string InfoMessage { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="VersionCheckSuccessfulEventArgs" /> class.
    /// </summary>
    /// <param name="isUpdate">
    ///   A <see cref="bool" /> indicating whether the version on the server is greater than the current application's
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
    public VersionCheckSuccessfulEventArgs(bool isUpdate, Version version, string criticalMessage, string infoMessage) {
      this.IsUpdate = isUpdate;
      this.Version = version;
      this.CriticalMessage = criticalMessage;
      this.InfoMessage = infoMessage;
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.Version != null);
    }

    /// <inheritdoc />
    public override string ToString() {
      return $"Version: {this.Version}, Critical Message: {this.CriticalMessage ?? "null"}, Info Message: {this.InfoMessage ?? "null"}";
    }
  }
}