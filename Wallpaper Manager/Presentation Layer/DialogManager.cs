// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;

using Avalon.Windows.Controls;

using Common.IO;

using WallpaperManager.Data;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Provides methods to show information-, warning-, error- and input-dialogs of this application.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public static class DialogManager {
    #region Cycling Related Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that wallpaper are missing to do a cycle.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="autocycleDisabled">
    ///   Indicates whether autocycling has been disabled before this dialog got shown.
    /// </param>
    public static void ShowCycle_MissingWallpapers(Window owner, Boolean autocycleDisabled) {
      Int32 screenCount = Screen.AllScreens.Length;

      StringBuilder message = new StringBuilder();
      message.AppendLine("More wallpapers are required to cycle.");

      if (screenCount > 1) {
        message.Append("There should be at least 1 activated multiscreen or ");
        message.Append(Screen.AllScreens.Length);
        message.AppendLine(" activated singlescreen wallpapers in the list.");
      } else {
        message.Append("There should be at least one activated wallpaper in the list.");
      }

      if (autocycleDisabled) {
        message.AppendLine();
        message.Append("Autocycling has been disabled.");
      }

      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Not Enought Wallpapers to Cycle", message.ToString(), TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that a manual cycle with the selected wallpapers is not possible.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCycle_WallpapersToUseInvalid(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Invalid Selection", "You have to select one or more singlescreen or exactly one multiscreen wallpaper.",
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }
    #endregion

    #region Category Related Dialogs
    /// <summary>
    ///   Creates and shows an input dialog requesting the name for a new category.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="categoryName">
    ///   The name of the category.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowCategory_AddNew(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, "Add Category", "Please enter the name of the new category below.", ref categoryName
      );
    }

    /// <summary>
    ///   Creates and shows an input dialog requesting the name for a new category.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="categoryName">
    ///   The name of the category.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowSynchronizedCategory_AddNew(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, "Add Synchronized Folder", "Please enter the name of the new synchronized folder below.", ref categoryName
      );
    }

    /// <summary>
    ///   Creates and shows an input dialog requesting the path of the directory to be watched.
    /// </summary>
    /// <param name="directoryPath">
    ///   The selected directory path.
    /// </param>
    public static Boolean ShowSynchronizedCategory_SelectDirectory(ref Path directoryPath) {
      FolderBrowserDialog dialog = new FolderBrowserDialog() {
        Description = "Please select the folder which should be watched for changes.",
        ShowNewFolderButton = true,
        SelectedPath = directoryPath
      };
      Boolean result = (dialog.ShowDialog() == DialogResult.OK);

      directoryPath = new Path(dialog.SelectedPath);
      
      return result;
    }

    /// <summary>
    ///   Creates and shows an input dialog requesting the new name for a category.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="categoryName">
    ///   The name of the category.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowCategory_Rename(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, "Rename Category", "Please enter the new name of the category below.", ref categoryName
      );
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the directory assigned with a <see cref="SynchronizedWallpaperCategory" /> 
    ///   was not found.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="directoryPath">
    ///   The selected directory path.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowSynchronizedCategory_DirectoryNotObservable(
      Window owner, String directoryPath
    ) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Directory Not Observable",
        DialogManager.CreateInsertedBoldText(
          "The directory\n", directoryPath, "\nis not observable because it could not be found."
        ),
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the directory assigned with a 
    ///   <see cref="SynchronizedWallpaperCategory" /> was not found.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="filePath">
    ///   The selected file path.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowSynchronizedCategory_FileAlreadyExist(
      Window owner, String filePath
    ) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Image File Does Already Exist",
        DialogManager.CreateInsertedBoldText(
          "The image file\n", filePath,
          "\ndoes already exist in the synchronized folder.\n\nDo you want to overwrite the existing file?"
        ),
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Error"
      );
      
      taskDialog.Show();

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates and shows a dialog asking if a new category should be created because no one exist.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowCategory_NoCategoryAvailable(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Create a New Category?", "There are no categories available. Do you want to create a new category now?",
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );
      
      taskDialog.Show();

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that no category is selected.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCategory_NoOneSelected(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "No Category Selected", "You have to select a category from the list first.", TaskDialogButtons.OK, "Warning"
      );
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog asking if the category should be deleted.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="categoryName">
    ///   The name of the category.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowCategory_WantDelete(Window owner, String categoryName) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Delete This Category?", null, TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );

      taskDialog.Content = DialogManager.CreateInsertedBoldText(
        "Are you sure you want to delete ", categoryName, " and all its containing wallpaper entries?"
      );

      taskDialog.Show();

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the entered category name is invalid.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCategory_NameInvalid(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Category Name Invalid",
        String.Concat(
          "The name is invalid because it contains one or more of the following invalid characters:\n",
          "\"[\", \"]\", line breaks, control characters and tabs."
        ),
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the entered category name is too short or too long.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCategory_NameInvalidLength(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Category Name Invalid",
        String.Format(
          CultureInfo.CurrentCulture, "The name is invalid. It has to be at least {0} and at most {1} characters long.",
          WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength
        ),
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }
    #endregion

    #region Wallpaper Related Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that at least one wallpaper has to be selected.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowWallpapers_NoOneSelected(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "No Wallpaper Selected", "You have to select at least one wallpaper from the list.", 
        TaskDialogButtons.OK, "Warning"
      );
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog asking if the wallpapers should be deleted physically.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    public static TaskDialogResult ShowWallpaper_WantDeletePhysically(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Delete This Wallpaper(s)?", "Are you sure you want to delete the selected wallpapers from your hard disk?",
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );

      taskDialog.ShowFooter = true;
      taskDialog.FooterIcon = TaskDialogIconConverter.ConvertFrom("Warning");
      taskDialog.Footer = "The image(s) are not moved to the recycle bin and will be deleted permanently.";

      taskDialog.Show();

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that disabling cycling of a multiscreen wallpaper on a screen is impossible.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowWallpapers_MultiscreenWallpaperOnDisabledScreens(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Multiscreen Wallpaper on Disabled Screens",
        "A multiscreen wallpaper can't have cycling disabled on a single monitor and has to be always applied on all monitors.",
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the disabled monitors settings of the wallpapers will not take effect
    ///   on the <see cref="WallpaperChangeType.ChangeAllCloned" /> mode.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowWallpapers_DisabledScreensOnCloneAllChangeMode(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Disabled Screens on Clone All Monitors",
        "You've configured that some of your wallpapers should not be cycled on some of your monitors. Note that this settings are ignored when using the \"Clone All Monitors\" cycle mode.",
        TaskDialogButtons.OK, "Warning"
      );

      taskDialog.Show();
    }
    #endregion

    #region Image Related Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that the image file has an invalid format or is not supported.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowImage_LoadError(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Invalid File Format", "The image file has an invalid or unsupported file format.", TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
    }
    #endregion

    #region Unsupported Operation Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that loading of directories is not supported yet.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowUnsupported_LoadDirectory(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Loading of Directories is not Supported", "Loading of directories is not supported by Wallpaper Manager yet.",
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
    }
    #endregion

    #region Configuration Related Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that the configuration file could not be written.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="directoryPath">
    ///   The path of the configuration file.
    /// </param>
    public static void ShowConfig_UnableToWrite(Window owner, String directoryPath) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Configuration Could Not Be Written", 
        DialogManager.CreateInsertedBoldText(
          "The configuration file ", directoryPath,
          " could not be written. Make sure Wallpaper Mananger has sufficient rights to access this path."
        ), 
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the configuration file could not be loaded because
    ///   it was not found.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="configPath">
    ///   The path of the configuration file.
    /// </param>
    public static void ShowConfig_FileNotFound(Window owner, String configPath) {
      DialogManager.ShowConfig_LoadErrorInternal(owner, configPath, "because it was not found", null);
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the configuration file could not be loaded because
    ///   its file format is invalid.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="configPath">
    ///   The path of the configuration file.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowConfig_InvalidFormat(Window owner, String configPath, String exceptionDetailText) {
      DialogManager.ShowConfig_LoadErrorInternal(
        owner, configPath, "because it has an invalid file format", exceptionDetailText
        );
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the configuration file could not be loaded because
    ///   an unhandled error occured.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="configPath">
    ///   The path of the configuration file.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowConfig_UnhandledLoadError(Window owner, String configPath, String exceptionDetailText) {
      DialogManager.ShowConfig_LoadErrorInternal(owner, configPath, null, exceptionDetailText);
    }

    /// <summary>
    ///   Creates and shows a dialog saying that a configuration couldn't be loaded because the given reason.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="configPath">
    ///   The path where the config file was expected.
    /// </param>
    /// <param name="reason">
    ///   The reason why the configuration could not be loaded.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    private static void ShowConfig_LoadErrorInternal(Window owner, String configPath, String reason, String exceptionDetailText) {
      TextBlock message = new TextBlock();

      message.Inlines.Add("The configuration file ");
      message.Inlines.Add(new LineBreak());

      // Part 2
      message.Inlines.Add(new TextBlock() {
        Text = configPath,
        FontWeight = FontWeights.Bold,
        TextWrapping = TextWrapping.Wrap
      });
      message.Inlines.Add(new LineBreak());
      
      // Part 3
      StringBuilder messagePart3 = new StringBuilder();
      if (reason != null) {
        messagePart3.Append("could not be loaded ");
        messagePart3.Append(reason);
        messagePart3.AppendLine(".");
      } else {
        messagePart3.AppendLine("could not be loaded.");
      }

      if (exceptionDetailText != null) {
        messagePart3.AppendLine("See Exception Details for more information.");
      }

      messagePart3.AppendLine();
      messagePart3.Append("The default configuration settings will be used instead.");

      message.Inlines.Add(messagePart3.ToString());

      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Configuration Cannot Be Loaded", message, TaskDialogButtons.OK, "Warning"
      );

      if (exceptionDetailText != null) {
        DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);
      }

      taskDialog.Show();
    }
    #endregion

    #region Update Related Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that a new version is available.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="currentVersion">
    ///   The current application's version.
    /// </param>
    /// <param name="newVersion">
    ///   The new version of the application available.
    /// </param>
    /// <param name="criticalMessage">
    ///   A critical message which should also be shown to the user.
    /// </param>
    /// <param name="infoMessage">
    ///   A info message which should also be shown to the user.
    /// </param>
    /// <returns>
    ///   The key of the button being pressed (<c>"Install"</c>, <c>"OpenWebsite"</c> or <c>"Cancel"</c>).
    /// </returns>
    public static String ShowUpdate_Available(
      Window owner, String currentVersion, String newVersion, String criticalMessage, String infoMessage
    ) {
      TaskDialog taskDialog = DialogManager.CreateDialog(owner, "Update Available", null, 0, "Question");
      
      taskDialog.Content = DialogManager.CreateInsertedBoldText(
        String.Concat("There is a new version of Wallpaper Manager available.\n\nCurrent Version: ", currentVersion, "\n"),
        String.Concat("Available Version: ", newVersion),
        String.Empty
      );
      
      // Note: We use the tag values as kind of button-key.
      taskDialog.Buttons.Add(new System.Windows.Controls.Button() { Content = "Install Now", Tag = "Install" });
      taskDialog.Buttons.Add(new System.Windows.Controls.Button() { Content = "Open Website", Tag = "OpenWebsite" });
      DialogManager.AddTaskDialogStandardButtons(taskDialog, TaskDialogButtons.Cancel);

      if (!String.IsNullOrEmpty(criticalMessage)) {
        taskDialog.Footer = criticalMessage;
        taskDialog.FooterIcon = TaskDialogIconConverter.ConvertFrom("Error");
        taskDialog.ShowFooter = true;
      } else if (!String.IsNullOrEmpty(infoMessage)) {
        taskDialog.Footer = infoMessage;
        taskDialog.FooterIcon = TaskDialogIconConverter.ConvertFrom("Information");
        taskDialog.ShowFooter = true;
      }

      taskDialog.Show();

      String pressedButtonKey;
      if (taskDialog.Result.StandardButton == TaskDialogButtons.Cancel) {
        pressedButtonKey = "Cancel";
      } else {
        if (taskDialog.Result.Button != null) {
          pressedButtonKey = (((System.Windows.Controls.Button)taskDialog.Result.Button).Tag as String);
        } else {
          pressedButtonKey = "Cancel";
        }
      }

      return pressedButtonKey;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that an update is actually being downloaded.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <returns>
    ///   The <see cref="Window" /> instance representing the non-modal task dialog.
    /// </returns>
    public static Window ShowUpdate_Downloading(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Downloading Update", String.Empty, TaskDialogButtons.None, null
      );

      taskDialog.ShowProgressBar = true;
      taskDialog.IsProgressIndeterminate = true;
      taskDialog.AllowDialogCancellation = false;

      // Usually TaskDialog.Show() would create its own Window, but since we cannot show this Window Non-Modal, we have to
      // create the Window on our own...
      Window taskDialogWindow = new Window() {
        SizeToContent = SizeToContent.WidthAndHeight, 
        WindowStyle = WindowStyle.SingleBorderWindow,
        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        Content = taskDialog,
        Title = "Downloading Update"
      };

      System.Windows.Controls.Button cancelButton = new System.Windows.Controls.Button();
      cancelButton.Content = "_Cancel";
      cancelButton.Click += delegate {
        taskDialogWindow.Close();
      };
      taskDialog.Buttons.Add(cancelButton);

      taskDialogWindow.Show();

      return taskDialogWindow;
    }

    /// <summary>
    ///   Creates and shows a dialog saying that no update is available.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowUpdate_NoUpdateAvailable(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "No Update Available", "This is the newest version of this application.", TaskDialogButtons.OK, "Information"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that no connection to the update server could be established.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowUpdate_UnableToConnect(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Unable to Connect to Update Server",
        "Wallpaper Manager can not contact the update server. \nPlease try again later.",
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that no update file could be found on the server.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static TaskDialogButtons ShowUpdate_UpdateFileNotFound(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "No Update File Found",
        "No update file could be found on the server. \nDo you want to open the website now to install the update manually?",
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Error"
      );

      taskDialog.Show();

      return taskDialog.Result.StandardButton;
    }
    #endregion

    #region Error Dialogs
    /// <summary>
    ///   Creates and shows a dialog saying that a file could not be found or the application doesn't have
    ///   the required rights to access it.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="filePath">
    ///   The path of the file.
    /// </param>
    public static void ShowGeneral_FileNotFound(Window owner, String filePath) {
      TaskDialog taskDialog;
      
      if (!String.IsNullOrEmpty(filePath)) {
        taskDialog = DialogManager.CreateDialog(owner, "File Not Found", null, TaskDialogButtons.OK, "Error");

        taskDialog.Content = DialogManager.CreateInsertedBoldText(
          "The requested file ", filePath, 
          " could not be found or this program does not have the required rights to access it."
        );
      } else {
        taskDialog = DialogManager.CreateDialog(
          owner, "File Not Found", "A requested file could not be found. Make sure it hasn't been moved or deleted.", 
          TaskDialogButtons.OK, "Error"
        );
      }
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that a file is currently in use by another application.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="filePath">
    ///   The path of the file.
    /// </param>
    public static void ShowGeneral_FileInUse(Window owner, String filePath) {
      TaskDialog taskDialog;
      
      if (!String.IsNullOrEmpty(filePath)) {
        taskDialog = DialogManager.CreateDialog(owner, "File In Use", null, TaskDialogButtons.OK, "Error");

        taskDialog.Content = DialogManager.CreateInsertedBoldText("The file ", filePath, " is in use by another program.");
      } else {
        taskDialog = DialogManager.CreateDialog(owner, "File In Use", "The file is in use.", TaskDialogButtons.OK, "Error");
      }
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that a directory could not be found.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="directoryPath">
    ///   The path of the directory.
    /// </param>
    public static void ShowGeneral_DirectoryNotFound(Window owner, String directoryPath) {
      TaskDialog taskDialog;
      
      if (!String.IsNullOrEmpty(directoryPath)) {
        taskDialog = DialogManager.CreateDialog(owner, "Directory Not Found", null, TaskDialogButtons.OK, "Error");

        taskDialog.Content = DialogManager.CreateInsertedBoldText(
          "The requested directory ", directoryPath, 
          " could either not be found or this program does not have the required rights to access it."
        );
      } else {
        taskDialog = DialogManager.CreateDialog(
          owner, "Directory Not Found", 
          "A requested directory could either not be found or this program does not have the required rights to access it.", 
          TaskDialogButtons.OK, "Error"
        );
      }
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that there is not enought memory available.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_OutOfMemory(Window owner, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Out of Memory", "There is not enough memory for this operation available.", TaskDialogButtons.OK, "Error"
      );

      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the path or file can not be accessed because 
    ///   of insufficient file system rights.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="path">
    ///   The path tried to access.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_MissingFileSystemRights(Window owner, String path, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Missing File System Rights", null, TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Owner = DialogManager.CreateInsertedBoldText(
        "Insufficient file system rights to access the file or directory", path, "."
      );
      
      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the path or file can not be accessed because 
    ///   of insufficient file system rights or the file is write protected.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="path">
    ///   The path tried to access.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_MissingFileSystemRightsOrWriteProtected(
      Window owner, String path, String exceptionDetailText
    ) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Missing File System Rights", null, TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Owner = DialogManager.CreateInsertedBoldText(
        "The file or directory ", path, 
        " is write protected or the user has insufficient file system rights to access it."
      );
      
      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the application misses framework rights to perform a
    ///   requested operation.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_MissingFrameworkRights(Window owner, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Wallpaper Manager requires Framework Rights", 
        "Wallpaper Manager is missing .NET Framework rights to perform the requested operation.",
        TaskDialogButtons.OK, "Error"
      );

      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);

      /*TextBlock tbkFooter = new TextBlock();
      tbkFooter.Inlines.Add("To modify .NET Framework access rights goto the ");
      tbkFooter.Inlines.Add(DialogManager.CreateControlPanelHyperlink(owner));
      tbkFooter.Inlines.Add(" and search for .NET Framework 2.0 Configuration.");
      taskDialog.Footer = tbkFooter;
      taskDialog.FooterIcon = TaskDialogIconConverter.ConvertFrom("Information");
      taskDialog.ShowFooter = true;*/

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the control panel couldn't be opened.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_UnableToOpenControlPanel(Window owner, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Unable to Open Windows Control Panel", "Unable to open the Windows Control Panel.", TaskDialogButtons.OK, "Error"
      );

      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the application is already running.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowGeneral_AppAlreadyRunning(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Wallpaper Manager Already Running", "Another instance of Wallpaper Manager is already running.",
        TaskDialogButtons.OK, "Error"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the application's data directory could not be created.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="directoryPath">
    ///   The path of the directory.
    /// </param>
    public static void ShowGeneral_UnableToCreateAppDataDirectory(Window owner, String directoryPath) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Can Not Create Directory", 
        DialogManager.CreateInsertedBoldText(
          "The application's data directory ", directoryPath, 
          " could not be created. Make sure Wallpaper Mananger has sufficient rights to access it."
        ), 
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that an unhandled exception occured.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_UnhandledException(Window owner, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, "Unhandled Exception Occured", 
        String.Format("Wallpaper Manager {0} encountered an unhandled exception, see Exception Details for more information.", Assembly.GetAssembly(typeof(DialogManager)).GetName().Version),
        TaskDialogButtons.OK, null
      );

      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);

      taskDialog.Show();
    }
    #endregion

    #region Helper Methods
    /// <summary>
    ///   Creates and shows an input dialog.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="title">
    ///   The dialog's title text.
    /// </param>
    /// <param name="message">
    ///   The message content.
    /// </param>
    /// <param name="inputText">
    ///   The default text of the input textbox.
    /// </param>
    /// <returns>
    ///   The <see cref="TaskDialogResult" /> containing return data of the shown <see cref="TaskDialog" />.
    /// </returns>
    private static TaskDialogResult ShowInputDialog(Window owner, String title, String message, ref String inputText) {
      StackPanel stpContent = new StackPanel();
      stpContent.Children.Add(new TextBlock() { Text = message });

      TextBox txtInput = new TextBox() { Text = inputText };
      stpContent.Children.Add(txtInput);

      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, title, stpContent, TaskDialogButtons.OK | TaskDialogButtons.Cancel, null
      );
      
      taskDialog.Loaded += delegate {
        txtInput.Focus();
        txtInput.SelectAll();
      };
      
      taskDialog.Show();

      inputText = txtInput.Text;

      return taskDialog.Result;
    }

    /// <summary>
    ///   Creates a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="title">
    ///   The dialog's title text.
    /// </param>
    /// <param name="message">
    ///   The message content.
    /// </param>
    /// <param name="buttons">
    ///   The buttons on the dialog.
    /// </param>
    /// <param name="icon">
    ///   The string containg the name of the dialog's icon. 
    /// </param>
    /// <returns>
    ///   The created <see cref="TaskDialog" />.
    /// </returns>
    private static TaskDialog CreateDialog(
      Window owner, String title, Object message, TaskDialogButtons buttons, String icon
    ) {
      TaskDialog taskDialog = new TaskDialog() {
        SnapsToDevicePixels = true, 
        Owner = owner, 
        Title = "Wallpaper Manager",
        Header = title,
        MaxWidth = 400,
        Content = message,
      };

      if (icon != null) {
        taskDialog.MainIcon = TaskDialogIconConverter.ConvertFrom(icon);
      }

      DialogManager.AddTaskDialogStandardButtons(taskDialog, buttons);

      return taskDialog;
    }

    /*
    /// <summary>
    ///   Creates a control panel hyperlink.
    /// </summary>
    /// <param name="owner">
    ///   If DialogManager hyperlink shows an error dialog, DialogManager is the owner <see cref="Window" /> of the dialog.
    /// </param>
    /// <returns>
    ///   The created hyperlink instance.
    /// </returns>
    private static Hyperlink CreateControlPanelHyperlink(Window owner) {
      Hyperlink lnkControlPanelLink = new Hyperlink(new Run("Windows Control Panel"));

      lnkControlPanelLink.Click += delegate (Object sender, RoutedEventArgs e) {
        Hyperlink lnkSender = (Hyperlink)sender;

        lnkSender.Cursor = Cursors.Wait;

        try {
          Process.Start("control.exe");
        } catch (Exception exception) {
          DialogManager.ShowGeneral_UnableToOpenControlPanel(owner, exception.ToString());
        }

        lnkSender.Cursor = Cursors.Hand;

        e.Handled = true;
      };

      return lnkControlPanelLink;
    }
    */

    /// <summary>
    ///   Adds standard buttons to a <see cref="TaskDialog" /> instance.
    /// </summary>
    /// <param name="taskDialog">
    ///   The task dialog.
    /// </param>
    /// <param name="buttons">
    ///   The standardbuttons to be added.
    /// </param>
    private static void AddTaskDialogStandardButtons(TaskDialog taskDialog, TaskDialogButtons buttons) {
      foreach (TaskDialogButtonData buttonData in TaskDialogButtonData.FromStandardButtons(buttons)) {
        if ((buttonData.Button == TaskDialogButtons.OK) || buttonData.Button == TaskDialogButtons.Yes) {
          buttonData.IsDefault = true;
        }

        taskDialog.Buttons.Add(buttonData);
      }
    }

    /// <summary>
    ///   Creates 3 text elements, the second one containing a bold text and attaches as message content 
    /// </summary>
    /// <param name="before">
    ///   The first element's text.
    /// </param>
    /// <param name="boldText">
    ///   The second element's text.
    /// </param>
    /// <param name="after">
    ///   The third element's text.
    /// </param>
    /// <returns>
    ///   The created <see cref="TextBlock" /> containing the 3 inline elements.
    /// </returns>
    private static TextBlock CreateInsertedBoldText(String before, String boldText, String after) {
      TextBlock textBlock = new TextBlock();

      textBlock.TextWrapping = TextWrapping.Wrap;
      textBlock.Inlines.Add(before);
      textBlock.Inlines.Add(new TextBlock() {
        Text = boldText, 
        FontWeight = FontWeights.Bold
      });
      textBlock.Inlines.Add(after);

      return textBlock;
    }

    /// <summary>
    ///   Attaches the given expansion text with the given <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="taskDialog">
    ///   The <see cref="TaskDialog" /> instance.
    /// </param>
    /// <param name="details">
    ///   The expansion detail content text.
    /// </param>
    private static void AttachExpansion_ExceptionDetails(TaskDialog taskDialog, String details) {
      taskDialog.ExpansionContent = new TextBlock() { 
        Text = String.Concat("Exception Details: \n", details), 
        FontStyle = FontStyles.Italic 
      };

      taskDialog.ExpansionButtonContent = "Exception Details";
      taskDialog.ExpansionPosition = TaskDialogExpansionPosition.Header;
    }
    #endregion
  }
}
