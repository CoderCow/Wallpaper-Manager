// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Xml;

using Common;
using Path = Common.IO.Path;

using WallpaperManager.Data;
using AppEnvironment = WallpaperManager.Data.AppEnvironment;

namespace WallpaperManager.Application {
  /// <summary>
  ///   Provides functionality to check and apply application updates of Wallpaper Manager.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Use instances of this class to check for application updates, download application updates and install them using
  ///     Wallpaper Manager setup executables.
  ///   </para>
  ///   <note type="implementnotes">
  ///     Version check- and update download operations are perfomed asynchronously by instances of this class. One instance of
  ///     this class can only perform one version check operation at the same time. Also, this class supports only one 
  ///     update download operation at the same time which is, not like the version check, executed in a static context and
  ///     therefore statically limited to one asynchronous operation at the same time. <br />
  ///     Use the <see cref="IsVersionChecking" /> property or the static <see cref="IsDownloadingUpdate" /> property to check 
  ///     whether an aynchronous operation is already in progress.
  ///   </note>
  /// </remarks>
  /// <seealso cref="BeginVersionCheck">BeginVersionCheck Method</seealso>
  /// <seealso cref="BeginDownloadUpdate">BeginDownloadUpdate Method</seealso>
  /// <seealso cref="IsVersionChecking">IsVersionChecking Property</seealso>
  /// <seealso cref="IsDownloadingUpdate">IsDownloadingUpdate Property</seealso>
  /// <seealso cref="ApplyUpdate">ApplyUpdate Method</seealso>
  /// <threadsafety static="false" instance="false" />
  public class UpdateManager: IDisposable {
    #region Constants: Update_UpdateInfoPage, Update_UpdateInfoPageAlt
    /// <summary>
    ///   Represents the web-address of the update information page.
    /// </summary>
    private const String Update_UpdateInfoPage = @"http://codercow.111mb.de/WallpaperManager/VersionInfoNew.php?v={0}";

    /// <summary>
    ///   Represents the alternative web-address of the update information page.
    /// </summary>
    private const String Update_UpdateInfoPageAlt = @"http://codercow.redio.de/WallpaperManager/VersionInfoNew.php?v={0}";
    #endregion

    #region Constants: Update_SetupDownloadURL, Update_SetupMinSize, Update_SetupDestFilePath, Update_BatchFilePath
    /// <summary>
    ///   Represents the web-address where the application's setup executable should be downloaded from.
    /// </summary>
    private const String Update_SetupDownloadURL = @"http://codercow.111mb.de/WallpaperManager/Resources/Wallpaper%20Manager%20Install.exe";

    /// <summary>
    ///   Represents the mimimum size in bytes expected for the executable after a download operation has been completed.
    /// </summary>
    private const Int32 Update_SetupMinSize = 51200; // 50 KiB

    /// <summary>
    ///   Represents the destination file name where the downloaded setup file is written to.
    /// </summary>
    private const String Update_SetupDestFileName = @"Wallpaper Manager Install.exe";

    /// <summary>
    ///   Represents the file name of the batch file which is created for the update installation.
    /// </summary>
    private const String Update_BatchFileName = @"Wallpaper Manager Update.bat";
    #endregion

    #region Static Fields: currentUpdateDownloadWebClient, currentUpdateDownloadCaller
    /// <summary>
    ///   The <see cref="WebClient" /> performing the current update download operation asynchronously.
    /// </summary>
    private static WebClient currentUpdateDownloadWebClient;

    /// <summary>
    ///   The <see cref="UpdateManager" /> which initiated the current update download.
    /// </summary>
    private static UpdateManager currentUpdateDownloadCaller;
    #endregion

    #region Fields: currentVersionCheckWorker
    /// <summary>
    ///   The <see cref="BackgroundWorker" /> performing the current version check operation asynchronously.
    /// </summary>
    private BackgroundWorker currentVersionCheckWorker;
    #endregion

    #region Static Property: IsDownloadingUpdate
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether an update is actually being installed or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether an update is actually being installed or not.
    /// </value>
    /// <seealso cref="BeginDownloadUpdate">BeginDownloadUpdate Method</seealso>
    /// <seealso cref="AbortDownloadUpdate">AbortDownloadUpdate Method</seealso>
    public static Boolean IsDownloadingUpdate {
      get { return (UpdateManager.currentUpdateDownloadWebClient != null); }
    }
    #endregion

    #region Property: Environment
    /// <summary>
    ///   <inheritdoc cref="Environment" select='../value/node()' />
    /// </summary>
    private readonly AppEnvironment environment;

    /// <summary>
    ///   Gets the <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </summary>
    /// <value>
    ///   The <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </value>
    /// <seealso cref="AppEnvironment">AppEnvironment Class</seealso>
    public AppEnvironment Environment {
      get { return this.environment; }
    }
    #endregion

    #region Property: IsVersionChecking
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether a version check operation is actually running or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether a version check operation is actually running or not.
    /// </value>
    /// <seealso cref="BeginVersionCheck">BeingVersionCheck Method</seealso>
    /// <seealso cref="AbortVersionCheck">AbortVersionCheck Method</seealso>
    public Boolean IsVersionChecking {
      get { return (this.currentVersionCheckWorker != null); }
    }
    #endregion

    #region Event: DownloadUpdateError
    /// <summary>
    ///   Occurs when an error aborted the update download operation.
    /// </summary>
    /// <seealso cref="ExceptionEventArgs">ExceptionEventArgs Class</seealso>
    /// <seealso cref="BeginDownloadUpdate">BeginDownloadUpdate Method</seealso>
    public event EventHandler<ExceptionEventArgs> DownloadUpdateError;

    /// <summary>
    ///   Called when an error aborted the update download operation.
    /// </summary>
    /// <commondoc select='All/Methods/EventRaisers[@Params="ExceptionEventArgs"]/*' params='EventName=DownloadUpdateError' />
    protected virtual void OnDownloadUpdateError(ExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.DownloadUpdateError != null) {
        this.DownloadUpdateError(this, e);

        if (!e.IsHandled) {
          throw new InvalidOperationException(ExceptionMessages.GetExceptionNotHandled("DownloadUpdateError"), e.Exception);
        }
      }
    }
    #endregion

    #region Event: DownloadUpdateSuccessful
    /// <summary>
    ///   Occurs when the update files have been downloaded completely and are ready to be applied.
    /// </summary>
    /// <seealso cref="BeginDownloadUpdate">BeginDownloadUpdate Method</seealso>
    public event EventHandler DownloadUpdateSuccessful;

    /// <summary>
    ///   Called when the update files have been downloaded completely and are ready to be applied.
    /// </summary>
    /// <remarks>
    ///   This method raises the <see cref="DownloadUpdateSuccessful" /> event.
    /// </remarks>
    /// <seealso cref="DownloadUpdateSuccessful">DownloadUpdateSuccessful Event</seealso>
    protected virtual void OnDownloadUpdateSucessful() {
      if (this.DownloadUpdateSuccessful != null) {
        this.DownloadUpdateSuccessful(this, EventArgs.Empty);
      }
    }
    #endregion

    #region Event: VersionCheckSuccessful
    /// <summary>
    ///   Occurs when a version check operation has been performed sucessfully.
    /// </summary>
    /// <seealso cref="VersionCheckSuccessfulEventArgs">VersionCheckSuccessfulEventArgs Class</seealso>
    /// <seealso cref="BeginVersionCheck">BeginVersionCheck Method</seealso>
    public event EventHandler<VersionCheckSuccessfulEventArgs> VersionCheckSuccessful;

    /// <summary>
    ///   Called when a version check operation has been performed sucessfully.s
    /// </summary>
    /// <commondoc select='All/Methods/EventRaisers[@Params="+EventArgs"]/*' params='EventName=VersionCheckSuccessful' />
    protected virtual void OnVersionCheckSuccessful(VersionCheckSuccessfulEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.VersionCheckSuccessful != null) {
        this.VersionCheckSuccessful(this, e);
      }
    }
    #endregion

    #region Event: VersionCheckError
    /// <summary>
    ///   Occurs when an <see cref="Exception" /> was thrown when checking for the application's version on the server.
    /// </summary>
    /// <seealso cref="ExceptionEventArgs">ExceptionEventArgs Class</seealso>
    /// <seealso cref="BeginVersionCheck">BeginVersionCheck Method</seealso>
    public event EventHandler<ExceptionEventArgs> VersionCheckError;

    /// <summary>
    ///   Called when an <see cref="Exception" /> was thrown when checking for the application's version on the server.
    /// </summary>
    /// <commondoc select='All/Methods/EventRaisers[@Params="ExceptionEventArgs"]/*' params='EventName=VersionCheckError' />
    protected virtual void OnVersionCheckError(ExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.VersionCheckError != null) {
        this.VersionCheckError(this, e);
      }

      if (!e.IsHandled) {
        throw new InvalidOperationException(ExceptionMessages.GetExceptionNotHandled("VersionCheckError"), e.Exception);
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="UpdateManager" /> class for the given <see cref="AppEnvironment" />.
    /// </summary>
    /// <param name="environment">
    ///   The <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="environment" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="AppEnvironment">Environment Class</seealso>
    public UpdateManager(AppEnvironment environment) {
      if (environment == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("environment"));
      }

      this.environment = environment;
    }
    #endregion

    #region Methods: BeginVersionCheck, AbortVersionCheck
    /// <summary>
    ///   Beings connecting to the update server asynchronously and raises either the <see cref="VersionCheckSuccessful" /> 
    ///   or <see cref="VersionCheckError" /> event when done.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The download of the data is performed asynchronously by using a <see cref="BackgroundWorker" /> object. <br />
    ///     Multiple calls of this method at once can lead in a possible <see cref="NotSupportedException" />,
    ///     because one instance of this class is not able to handle multiple version check operations at the same time. <br />
    ///   </para>
    ///   <note type="implementnotes">
    ///     Use the <see cref="IsVersionChecking" /> property to check whether an aynchronous version check operation is 
    ///     already in progress before calling this method multiple times.
    ///   </note>
    ///   <note type="implementnotes">
    ///     Use the <see cref="AbortVersionCheck" /> method to cancel an asynchronous version check operation.
    ///   </note>
    /// </remarks>
    /// <exception cref="WebException">
    ///   Error on connecting to server or downloading data or both <see cref="Update_UpdateInfoPage" /> and 
    ///   <see cref="Update_UpdateInfoPageAlt" /> constants contain an invalid URL.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///   Another asynchronous version check operation is already in progress by this instance.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="VersionCheckSuccessful">VersionCheckSuccessful Event</seealso>
    /// <seealso cref="VersionCheckError">VersionCheckError Event</seealso>
    /// <seealso cref="IsVersionChecking">IsVersionChecking Property</seealso>
    /// <seealso cref="AbortVersionCheck">AbortVersionCheck Method</seealso>
    public void BeginVersionCheck() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if (this.IsVersionChecking) {
        throw new NotSupportedException(ExceptionMessages.GetVersionCheckOperationAlreadyRunning());
      }

      this.currentVersionCheckWorker = new BackgroundWorker();
      this.currentVersionCheckWorker.WorkerSupportsCancellation = true;
      this.currentVersionCheckWorker.DoWork += delegate(Object sender, DoWorkEventArgs e) {
        if (e.Cancel) {
          return;
        }

        Tuple<String[], Version> arguments = (Tuple<String[], Version>)e.Argument;
        String[] urlsToCheck = arguments.Item1;
        Version currentAppVersion = arguments.Item2;
        
        Exception error = null;
        Version mostRecentVersion = null;
        String infoMessage = null;
        String criticalMessage = null;

        // Work through all available servers to check for a new version.
        using (WebClient webClient = new WebClient()) {
          for (Int32 i = 0; i < urlsToCheck.Length; i++) {
            if (e.Cancel) {
              return;
            }

            try {
              Byte[] data = webClient.DownloadData(String.Format(CultureInfo.InvariantCulture, urlsToCheck[i], currentAppVersion));

              using (MemoryStream dataStream = new MemoryStream(data)) {
                XmlDocument versionCheckData = new XmlDocument();
                versionCheckData.Load(dataStream);

                if (versionCheckData.DocumentElement == null) {
                  throw new FormatException(ExceptionMessages.GetUpdatePageVersionDataInvalid());
                }

                XmlNode newestVersionNode = versionCheckData.DocumentElement["NewestVersion"];
                XmlNode infoMessageNode = versionCheckData.DocumentElement["InfoMessage"];
                XmlNode criticalMessageNode = versionCheckData.DocumentElement["CriticalMessage"];
                if ((newestVersionNode == null) || (criticalMessageNode == null)) {
                  throw new FormatException(ExceptionMessages.GetUpdatePageVersionDataInvalid());
                }

                Version versionNumberFromThisServer;
                try {
                  versionNumberFromThisServer = new Version(newestVersionNode.InnerText);
                } catch (Exception exception) {
                  throw new FormatException(ExceptionMessages.GetUpdatePageVersionDataInvalid(), exception);
                }
                // Make sure that this server didn't return an older version number than another server.
                if ((mostRecentVersion != null) && (versionNumberFromThisServer < mostRecentVersion)) {
                  continue;
                }
                
                mostRecentVersion = versionNumberFromThisServer;
                criticalMessage = criticalMessageNode.InnerText;
                if (infoMessageNode != null) {
                  infoMessage = infoMessageNode.InnerText;
                }
              }
            } catch (Exception exception) {
              error = exception;
            }
          }
        }

        // We don't want to throw an Exception if we got the data we needed from at least one server.
        if (mostRecentVersion != null) {
          error = null;
        }

        VersionCheckSuccessfulEventArgs successfulResultData = null;
        if (error == null) {
          successfulResultData = new VersionCheckSuccessfulEventArgs(
            (currentAppVersion < mostRecentVersion), mostRecentVersion, criticalMessage, infoMessage
          );
        }
        e.Result = new Tuple<Exception,VersionCheckSuccessfulEventArgs>(error, successfulResultData);
      };

      this.currentVersionCheckWorker.RunWorkerCompleted += delegate(Object sender, RunWorkerCompletedEventArgs e) {
        try {
          var result = (Tuple<Exception, VersionCheckSuccessfulEventArgs>)e.Result;

          if (result.Item1 != null) {
            this.OnVersionCheckError(new ExceptionEventArgs(result.Item1));
          } else {
            this.OnVersionCheckSuccessful(result.Item2);
          }
        } finally {
          if (this.currentVersionCheckWorker != null) {
            this.currentVersionCheckWorker.Dispose();
            this.currentVersionCheckWorker = null;
          }
        }
      };

      this.currentVersionCheckWorker.RunWorkerAsync(new Tuple<String[],Version>(
        new[] { UpdateManager.Update_UpdateInfoPage, UpdateManager.Update_UpdateInfoPageAlt },
        (Version)this.Environment.AppVersion.Clone()
      ));
    }

    /// <summary>
    ///   Cancels the current asynchronous version check operation.
    /// </summary>
    /// <remarks>
    ///   <note type="implementnotes">
    ///     Use the <see cref="IsVersionChecking" /> property to check whether an aynchronous version check operation is
    ///     in progress before calling this method.
    ///   </note>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   There is actually no asynchronous version check operation running.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="IsVersionChecking">IsVersionChecking Property</seealso>
    public void AbortVersionCheck() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if (!this.IsVersionChecking) {
        throw new InvalidOperationException(ExceptionMessages.GetVersionCheckOperationNotRunning());
      }

      this.currentVersionCheckWorker.CancelAsync();
      this.currentVersionCheckWorker.Dispose();
    }
    #endregion

    #region Methods: BeginDownloadUpdate, AbortDownloadUpdate
    /// <summary>
    ///   Beings downloading the most recent version of this application from the url represented by the
    ///   <see cref="Update_SetupDownloadURL" /> constant and then raises either the <see cref="DownloadUpdateSuccessful" />
    ///   or <see cref="DownloadUpdateError" /> event.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The download is performed by using a static <see cref="WebClient" /> object, which is used asynchronously.
    ///     Multiple calls of this method at once can lead in a possible <see cref="NotSupportedException" />,
    ///     because this class is not able to handle multiple update install operations at the same time.
    ///   </para>
    ///   <note type="implementnotes">
    ///     Use the static <see cref="IsDownloadingUpdate" /> property to check whether an aynchronous update download operation 
    ///     is already in progress before calling this method.
    ///   </note>
    ///   <note type="implementnotes">
    ///     Use the <see cref="AbortDownloadUpdate" /> method to cancel the asynchronous update download operation. Note that 
    ///     only the same instance who started the download operation can cancel it again.
    ///   </note>
    /// </remarks>
    /// <exception cref="SecurityException">
    ///   Missing <see cref="FileIOPermissionAccess.PathDiscovery" /> or <see cref="FileIOPermissionAccess.Write" /> for the 
    ///   system's temporary directory.
    /// </exception>
    /// <exception cref="WebException">
    ///   <see cref="Update_SetupDownloadURL">Update_SetupDownloadURL Constant</see> represents an invalid address or an error
    ///   occurred while downloading the data.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///   Another asynchronous update download operation is already in progress.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <permission cref="FileIOPermission">
    ///   to write to the system's temporary directory contents. Associated enumerations: 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery" />, <see cref="FileIOPermissionAccess.Write" />.
    /// </permission>
    /// <seealso cref="DownloadUpdateSuccessful">DownloadUpdateSuccessful Event</seealso>
    /// <seealso cref="DownloadUpdateError">DownloadUpdateError Event</seealso>
    /// <seealso cref="IsDownloadingUpdate">IsDownloadingUpdate Property</seealso>
    /// <seealso cref="AbortDownloadUpdate">AbortDownloadUpdate Method</seealso>
    public void BeginDownloadUpdate() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if (UpdateManager.currentUpdateDownloadWebClient != null) {
        throw new NotSupportedException(ExceptionMessages.GetUpdateDownloadOperationAlreadyRunning());
      }

      Path setupFilePath = Path.Concat(this.Environment.SystemTempPath, UpdateManager.Update_SetupDestFileName);
      UpdateManager.currentUpdateDownloadWebClient = new WebClient();
      UpdateManager.currentUpdateDownloadWebClient.DownloadFileCompleted += (sender, e) => {
        try {
          if (this.isDisposed || e.Cancelled) {
            return;
          }

          if (e.Error != null) {
            this.OnDownloadUpdateError(new ExceptionEventArgs(e.Error));
            return;
          }

          if (!e.Cancelled) {
            // If the file size is lower than UpdateManager.Update_SetupMinSize, then it looks like we downloaded a web page 
            // responding an error instead of the intended update file.
            if (new FileInfo(setupFilePath).Length < UpdateManager.Update_SetupMinSize) {
              this.OnDownloadUpdateError(
                new ExceptionEventArgs(new FileNotFoundException(ExceptionMessages.GetUpdateDownloadFileNotFound()))
              );
              return;
            }
          }
        } finally {
          if (UpdateManager.currentUpdateDownloadCaller == this) {
            if (UpdateManager.currentUpdateDownloadCaller != null) {
              UpdateManager.currentUpdateDownloadCaller.Dispose();
              UpdateManager.currentUpdateDownloadCaller = null;
            }
          }
        }

        this.OnDownloadUpdateSucessful();
      };

      UpdateManager.currentUpdateDownloadWebClient.DownloadFileAsync(new Uri(UpdateManager.Update_SetupDownloadURL), setupFilePath);
    }

    /// <summary>
    ///   Cancels the current update download operation.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Note that only the same instance who started the download operation can cancel it again.
    ///   </para>
    ///   <note type="implementnotes">
    ///     Use the <see cref="IsVersionChecking" /> property to check whether an aynchronous version check operation is
    ///     in progress before calling this method.
    ///   </note>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   There is currently no update download operation running.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="IsDownloadingUpdate">IsDownloadingUpdate Property</seealso>
    public void AbortDownloadUpdate() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if ((!UpdateManager.IsDownloadingUpdate) && (UpdateManager.currentUpdateDownloadCaller == this)) {
        throw new InvalidOperationException(ExceptionMessages.GetUpdateDownloadOperationNotRunning());
      }

      UpdateManager.currentUpdateDownloadWebClient.CancelAsync();
      UpdateManager.currentUpdateDownloadWebClient.Dispose();
      UpdateManager.currentUpdateDownloadWebClient = null;
      UpdateManager.currentUpdateDownloadCaller = null;
    }
    #endregion

    #region Methods: ApplyUpdate
    /// <summary>
    ///   Applies the downloaded update by generating a batch file and starting it.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This method generates a batch file in the temporary directory, where the <see cref="Update_BatchFileName" /> constant 
    ///     represents the file name, and executes it. If a batch file with the same name already exists, it will be overwriten.
    ///   </para>
    ///   <para>
    ///     Before calling this method, make sure that a Wallpaper Manager setup executable exists in the temporary directory
    ///     where the <see cref="Update_SetupDestFileName" /> constant represents the expected name of the file.
    ///     A <see cref="FileNotFoundException" /> will be thrown if the executeable doesn't exists.
    ///   </para>
    ///   <para>
    ///     The application should be terminated immediately after this method has been called so that the application's 
    ///     executable file can be replaced by the Windows batch interpreter.
    ///   </para>
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///   The Wallpaper Manager setup executable could not be found.
    /// </exception>
    /// <exception cref="IOException">
    ///   Error on writing the batch file to the temporary directory. See inner exception for more details.
    /// </exception>
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <permission cref="SecurityAction.LinkDemand">
    ///   for full trust for the immediate caller. This member cannot be used by partially trusted code.
    /// </permission>
    /// <permission cref="FileIOPermission">
    ///   to write the batch file contents. Associated enumerations: 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery" />, <see cref="FileIOPermissionAccess.Write" />.
    /// </permission>
    /// <seealso href="http://technet.microsoft.com/en-us/library/cc759262%28WS.10%29.aspx">Msiexec (command-line options)</seealso>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public void ApplyUpdate() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      Path setupPath = Path.Concat(this.Environment.SystemTempPath, UpdateManager.Update_SetupDestFileName);
      if (!File.Exists(setupPath)) {
        throw new FileNotFoundException(ExceptionMessages.GetFileNotFound(setupPath));
      }
      
      // We write the batch file even before the download started at all to check for errors.
      StreamWriter batchWriter = null;
      Path batchFilePath = Path.None;
      try {
        batchFilePath = Path.Concat(this.Environment.SystemTempPath, UpdateManager.Update_BatchFileName);

        batchWriter = new StreamWriter(batchFilePath);
        batchWriter.WriteLine("@echo off");

        batchWriter.WriteLine("cd \"{0}\"", batchFilePath.ParentDirectory);
        batchWriter.WriteLine("echo Waiting for Wallpaper Mananger to close...");
        // A little trick to make the batch wait for 2 seconds until Wallpaper Manager is closed.
        batchWriter.WriteLine("ping -n 2 127.0.0.1>NUL");

        batchWriter.WriteLine("echo Uninstalling old version...");
        // This uninstalls the old version.
        // Note: Run msiexec /? to get help to the msi parameters.
        batchWriter.WriteLine("msiexec /q /x {0}", AppEnvironment.AppGuid);

        batchWriter.WriteLine("echo Installing new version...");
        // The rar self extract exe which is used for any Wallpaper Manager setup (to put the Setup.exe and Setup.msi
        // which is generated by the Installer Project into one file) requires the parameter -sp<Args> to "route" the
        // given commands to the "real" setup.exe which then "routes" em to the setup.msi.
        batchWriter.WriteLine("\"{0}\" -sp\"/q\"", UpdateManager.Update_SetupDestFileName);

        /* Removed due an error caused when the bootstrapper restarted the system and tried to re-execute the deleted files.
        batchWriter.WriteLine("echo Deleting temporary files...");
        // The batch should delete itself and the setup file after the update.
        batchWriter.WriteLine("del \"{0}\"", UpdateManager.Update_SetupDestFileName);
        batchWriter.WriteLine("del \"{0}\"", UpdateManager.Update_BatchFileName);*/

        batchWriter.WriteLine("echo Done.");

        batchWriter.WriteLine("ping -n 2 127.0.0.1>NUL");
      } catch (Exception exception) {
        throw new IOException(ExceptionMessages.GetUpdateBatchFileWriteError(batchFilePath), exception);
      } finally {
        if (batchWriter != null) {
          batchWriter.Dispose();
        }
      }

      // When at least Windows Vista, we better use runas to get administrator rights if we don't have them.
      if (AppEnvironment.IsWindowsVista) {
        Process.Start(new ProcessStartInfo() {
          WorkingDirectory = batchFilePath.ParentDirectory, 
          Verb = "runas", 
          FileName = UpdateManager.Update_BatchFileName
        });
      } else {
        Process.Start(new ProcessStartInfo {
          WorkingDirectory = batchFilePath.ParentDirectory,
          FileName = "cmd",
          Arguments = String.Format(CultureInfo.InvariantCulture, "/C \"{0}\"", UpdateManager.Update_BatchFileName)
        });
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
          if (this.currentVersionCheckWorker != null) {
            this.currentVersionCheckWorker.CancelAsync();
            this.currentVersionCheckWorker.Dispose();
          }

          if ((UpdateManager.IsDownloadingUpdate) && (UpdateManager.currentUpdateDownloadCaller == this)) {
            UpdateManager.currentUpdateDownloadWebClient.CancelAsync();
            UpdateManager.currentUpdateDownloadWebClient.Dispose();
            UpdateManager.currentUpdateDownloadWebClient = null;
            UpdateManager.currentUpdateDownloadCaller = null;
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
    ///   Finalizes an instance of the <see cref="UpdateManager" /> class.
    /// </summary>
    ~UpdateManager() {
      this.Dispose(false);
    }
    #endregion
  }
}