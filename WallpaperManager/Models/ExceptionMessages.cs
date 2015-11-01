// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Provides yet unlocalized descriptions of several error-causing circumstances to be used as messages for exceptions.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ExceptionMessages: ExceptionMessagesCommon {
    #region General
    /// <summary>
    ///   Returns a <see cref="String" /> stating that no event handlers are registered with the RequestWallpapers-event.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that no event handlers are registered with the RequestWallpapers-event.
    /// </returns>
    public static String GetNoHandlersForRequestWallpapersEvent() {
      return "No event handlers are registered with the RequestWallpapers event.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that there are not enough wallpapers available to cycle.
    /// </summary>
    /// <param name="cycleStep">
    ///   A <see cref="Byte" /> representing the current step of the cycle where the error was caused for debug purposes.
    /// </param>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that there are not enough wallpapers available to cycle.
    /// </returns>
    public static String GetNotEnoughtWallpapersToCycle(
      Byte? cycleStep = null, String variableName = null, VariableType variableType = VariableType.Parameter
    ) {
      return ExceptionMessagesCommon.GenerateMessage(
        "There are not enough wallpapers available to cycle.", 
        variableType, variableName, "Cycle Step", cycleStep.ToString()
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that another cycle is already in progress performed by another thread.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that another cycle is already in progress performed by another thread.
    /// </returns>
    public static String GetCyclingAlreadyInProgress() {
      return "Another cycle is already in progress performed by another thread.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that a request operation returned a null value.
    /// </summary>
    /// <param name="delegateVariableName">
    ///   The name of the variable containing the delegate to request.
    /// </param>
    /// <returns>
    ///   A <see cref="String" /> stating that a request operation returned a null value.
    /// </returns>
    public static String GetRequestReturnedNull(String delegateVariableName = null) {
      return ExceptionMessagesCommon.GenerateMessage(
        "A request operation returned a null value.", "Requesting Delegate", delegateVariableName
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that a View Model request operation returned a View Model which does not wrap 
    ///   the requested Model.
    /// </summary>
    /// <param name="delegateVariableName">
    ///   The name of the variable holding the delegate to request.
    /// </param>
    /// <param name="modelType">
    ///   The <see cref="Type" /> of the View Model.
    /// </param>
    /// <param name="viewModelType">
    ///   The <see cref="Type" /> of the Model.
    /// </param>
    /// <returns>
    ///   A <see cref="String" /> stating that a View Model request operation returned a View Model which does not wrap the 
    ///   requested Model.
    /// </returns>
    public static String GetVMRequestReturnedWrongWrapper(
      String delegateVariableName = null, Type viewModelType = null, Type modelType = null
    ) {
      String viewModelTypeName = null;
      String modelTypeName = null;

      if (viewModelType != null) {
        viewModelTypeName = viewModelType.Name;
      }
      if (modelType != null) {
        modelTypeName = modelType.Name;
      }

      return ExceptionMessagesCommon.GenerateMessage(
        "A View Model request operation returned a View Model which does not wrap the requested Model.", 
        new[] { "Requesting Delegate", "ViewModel Type", "Model Type" }, 
        new[] { delegateVariableName, viewModelTypeName, modelTypeName }
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that another cycle is already in progress performed by another thread.
    /// </summary>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that another cycle is already in progress performed by another thread.
    /// </returns>
    public static String GetCollectionItemsNotEqualToScreenCount(
      String variableName = null, VariableType variableType = VariableType.Parameter
    ) {
      return ExceptionMessagesCommon.GenerateMessage(
        "The amount of items in the collection has to be equal to the amount of screens.", variableType, variableName
      );
    }
    #endregion

    #region Category Related
    /// <summary>
    ///   Returns a <see cref="String" /> stating that the category is read only.
    /// </summary>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that the category is read only.
    /// </returns>
    public static String GetCategoryIsReadOnly(String variableName = null, VariableType variableType = VariableType.Parameter) {
      return ExceptionMessagesCommon.GenerateMessage("The category is read only.", variableType, variableName);
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the the length of the category name is invalid.
    /// </summary>
    /// <param name="currentLength">
    ///   The length of the invalid <see cref="String" /> causing the exception.
    /// </param>
    /// <param name="minLength">
    ///   The minimal length expected for the <see cref="String" /> to be valid.
    /// </param>
    /// <param name="maxLength">
    ///   The maximal length expected for the <see cref="String" /> to be valid.
    /// </param>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that the length of the category name is invalid.
    /// </returns>
    public static String GetCategoryNameLengthInvalid(
      Int32? currentLength = null, 
      Int32? minLength = null,
      Int32? maxLength = null,
      String variableName = null, 
      VariableType variableType = VariableType.Parameter
    ) {
      return ExceptionMessagesCommon.GenerateMessage(
        "The length of the category name is invalid.", 
        variableType, variableName, 
        new[] { "Current Length", "Minimum Length Allowed", "Maximum Length Allowed" }, 
        new[] { currentLength.ToString(), minLength.ToString(), maxLength.ToString() }
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the the category name is invalid.
    /// </summary>
    /// <param name="categoryName">
    ///   The <see cref="String" /> holding the invlaid category name which caused the exception.
    /// </param>
    /// <param name="forbiddenChars">
    ///   An array of <see cref="Char" /> representing the characters which are not allowed in the string.
    /// </param>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that the category name is invalid.
    /// </returns>
    public static String GetCategoryNameInvalid(
      String categoryName = null,
      IList<Char> forbiddenChars = null,
      String variableName = null,
      VariableType variableType = VariableType.Parameter
    ) {
      String forbiddenCharsString = null;
      if (forbiddenChars != null) {
        forbiddenCharsString = String.Join(", ", forbiddenChars);
      }
      
      return ExceptionMessagesCommon.GenerateMessage(
        "The category name is invalid.",
        variableType, variableName,
        new[] { "Category Name", "Forbidden Characters" },
        new[] { categoryName, forbiddenCharsString }
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the Wallpaper Category has to be a Synchronized Folder to perform the 
    ///   requested operation.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that the Wallpaper Category has to be a Synchronized Folder to perform the requested 
    ///   operation.
    /// </returns>
    public static String GetCategoryIsNoSynchronizedFolder() {
      return "The Wallpaper Category has to be a Synchronized Folder to perform the requested operation";
    }
    #endregion

    #region Wallpaper Related
    /// <summary>
    ///   Returns a <see cref="String" /> stating that a multiscreen wallpaper can not be disabled on a single screen.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that a multiscreen wallpaper can not be disabled on a single screen.
    /// </returns>
    public static String GetMultiscreenWallpaperCanNotBeDisabledForAScreen() {
      return "A multiscreen wallpaper can not be disabled on a single screen.";
    }
    #endregion

    #region Update Related
    /// <summary>
    ///   Returns a <see cref="String" /> stating that the version string responsed by the update server is invalid.
    /// </summary>
    /// <param name="versionString">
    ///   The <see cref="String" /> holding the invalid version number returned by the server.
    /// </param>
    /// <commondoc select='ExceptionMessages/Methods/All[@Params="String,VariableType"]/*' />
    /// <returns>
    ///   A <see cref="String" /> stating that the version string responsed by the update server is invalid.
    /// </returns>
    public static String GetUpdatePageVersionDataInvalid(
      String versionString = null,
      String variableName = null, 
      VariableType variableType = VariableType.Parameter
    ) {
      return ExceptionMessagesCommon.GenerateMessage(
        "The version string responsed by the update server is invalid.", 
        variableType, variableName, 
        "Version String", versionString
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the batch file could not be written.
    /// </summary>
    /// <param name="batchFilePath">
    ///   The path of the batch file.
    /// </param>
    /// <returns>
    ///   A <see cref="String" /> stating that the batch file could not be written.
    /// </returns>
    /// <exception cref="SecurityException">
    ///   Missing <see cref="FileIOPermissionAccess.PathDiscovery">FileIOPermissionAccess.PathDiscovery</see> for the given
    ///   path string.
    /// </exception>
    /// <permission cref="FileIOPermission">
    ///   for displaying the path to the user. Associated enumerations:
    ///   <see cref="FileIOPermissionAccess.PathDiscovery">FileIOPermissionAccess.PathDiscovery</see>.
    /// </permission>
    public static String GetUpdateBatchFileWriteError(String batchFilePath) {
      new FileIOPermission(FileIOPermissionAccess.PathDiscovery, batchFilePath).Demand();

      return ExceptionMessagesCommon.GenerateMessage(
        "The batch file to update the application could not be written.", "File Path", batchFilePath
      );
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the update could not be downloaded.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that the update could not be downloaded.
    /// </returns>
    public static String GetUpdateDownloadError() {
      return "The update could not be downloaded due an error. See inner exception for more details.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that the update could not be downloaded because the file is not found on the 
    ///   server.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that the update could not be downloaded because the file is not found on the server.
    /// </returns>
    public static String GetUpdateDownloadFileNotFound() {
      return "The update could not be downloaded because the file is not found on the server.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that another asynchronous download update process is already running.
    /// </summary>
    /// <returns>
    ///   A string saying that another asynchronous download update process is already running.
    /// </returns>
    public static String GetUpdateDownloadOperationAlreadyRunning() {
      return "Another asynchronous download update process is already running.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that there is no asynchronous download update process initiated by the current 
    ///   instance running.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that there is no asynchronous download update process initiated by the current 
    ///   instance running.
    /// </returns>
    public static String GetUpdateDownloadOperationNotRunning() {
      return "There is no asynchronous download update process initiated by the current instance running.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that another asynchronous version check operation is already running.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that another asynchronous version check operation is already running.
    /// </returns>
    public static String GetVersionCheckOperationAlreadyRunning() {
      return "Another asynchronous version check operation is already running.";
    }

    /// <summary>
    ///   Returns a <see cref="String" /> stating that there is no asynchronous version check operation running for the current 
    ///   instance.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> stating that there is no asynchronous version check operation running for the current instance.
    /// </returns>
    public static String GetVersionCheckOperationNotRunning() {
      return "There is no asynchronous version check operation running for the current instance.";
    }
    #endregion
  }
}
