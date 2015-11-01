// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
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

using WallpaperManager.Models;

namespace WallpaperManager.Views {
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
      message.AppendLine(LocalizationManager.GetLocalizedString("Dialog.Cycle.MissingWallpapers.Description1"));

      if (screenCount > 1) {
        message.AppendFormat(
          LocalizationManager.GetLocalizedString("Dialog.Cycle.MissingWallpapers.Description2"), Screen.AllScreens.Length
        );
        message.AppendLine();
      } else {
        message.AppendLine(LocalizationManager.GetLocalizedString("Dialog.Cycle.MissingWallpapers.Description3"));
      }

      if (autocycleDisabled) {
        message.AppendLine();
        message.Append(LocalizationManager.GetLocalizedString("Dialog.Cycle.MissingWallpapers.AutocyclingDisabled"));
      }

      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Cycle.MissingWallpapers.Title"), 
        message.ToString(), TaskDialogButtons.OK, "Error"
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Cycle.InvalidSelection.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Cycle.InvalidSelection.Description"), TaskDialogButtons.OK, "Error"
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
    public static Boolean ShowCategory_AddNew(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.AddNew.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Category.AddNew.Description"), ref categoryName
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
    public static Boolean ShowSynchronizedCategory_AddNew(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.AddSynchronizedFolder.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Category.AddSynchronizedFolder.Description"), ref categoryName
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
        Description = LocalizationManager.GetLocalizedString("Dialog.Category.SelectSynchronizedFolder.Description"),
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
    public static Boolean ShowCategory_Rename(Window owner, ref String categoryName) {
      return DialogManager.ShowInputDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.Rename.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Category.Rename.Description"), ref categoryName
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
    public static void ShowSynchronizedCategory_DirectoryNotObservable(Window owner, String directoryPath) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderNotObservable.Title"),
        DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderNotObservable.Description1") + "\n", 
          directoryPath, 
          "\n" + LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderNotObservable.Description2")
        ),
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
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
    public static Boolean ShowSynchronizedCategory_FileAlreadyExist(Window owner, String filePath) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderFileAlreadyExist.Title"),
        DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderFileAlreadyExist.Description1") + "\n", 
          filePath,
          "\n" + LocalizationManager.GetLocalizedString("Dialog.Category.SynchronizedFolderFileAlreadyExist.Description2")
        ),
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Error"
      );
      
      taskDialog.Show();

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Yes);
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
    public static Boolean ShowCategory_NoCategoryAvailable(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.NoCategoryAvailable.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Category.NoCategoryAvailable.Description"),
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );
      
      taskDialog.Show();

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Yes);
    }

    /// <summary>
    ///   Creates and shows a dialog saying that no category is selected.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCategory_NoOneSelected(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.NoCategorySelected.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Category.NoCategorySelected.Description"), TaskDialogButtons.OK, "Warning"
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
    public static Boolean ShowCategory_WantDelete(Window owner, String categoryName) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.Delete.Title"), null, 
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );

      taskDialog.Content = DialogManager.CreateInsertedBoldText(
        LocalizationManager.GetLocalizedString("Dialog.Category.Delete.Description1"), categoryName, 
        LocalizationManager.GetLocalizedString("Dialog.Category.Delete.Description2")
      );

      taskDialog.Show();

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Yes);
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the entered category name is invalid.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowCategory_NameInvalid(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.NameInvalid.Title"),
        String.Concat(
          LocalizationManager.GetLocalizedString("Dialog.Category.NameInvalid.Description1"), "\n",
          LocalizationManager.GetLocalizedString("Dialog.Category.NameInvalid.Description2")
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Category.NameInvalidLength.Title"),
        String.Format(
          CultureInfo.CurrentCulture, LocalizationManager.GetLocalizedString("Dialog.Category.NameInvalidLength.Description"),
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Wallpaper.NoneSelected.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Wallpaper.NoneSelected.Description"), TaskDialogButtons.OK, "Warning"
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
    public static Boolean ShowWallpaper_WantDeletePhysically(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Wallpaper.DeletePhysically.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Wallpaper.DeletePhysically.Description"),
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Question"
      );

      taskDialog.ShowFooter = true;
      taskDialog.FooterIcon = TaskDialogIconConverter.ConvertFrom("Warning");
      taskDialog.Footer = LocalizationManager.GetLocalizedString("Dialog.Wallpaper.DeletePhysically.NoteText");

      taskDialog.Show();

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Yes);
    }

    /// <summary>
    ///   Creates and shows a dialog saying that disabling cycling of a multiscreen wallpaper on a screen is impossible.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowWallpapers_MultiscreenWallpaperOnDisabledScreens(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Wallpaper.MultiscreenOnDisabledScreens.Title"),
        LocalizationManager.GetLocalizedString("Dialog.Wallpaper.MultiscreenOnDisabledScreens.Description"),
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Wallpaper.DisabledScreensAndCloneAllCycleMode.Title"),
        LocalizationManager.GetLocalizedString("Dialog.Wallpaper.DisabledScreensAndCloneAllCycleMode.Description"),
        TaskDialogButtons.OK, "Warning"
      );

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the image file has an invalid format or is not supported.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowWallpaper_LoadError(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Wallpaper.ImageFormatError.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Wallpaper.ImageFormatError.Description"), TaskDialogButtons.OK, "Error"
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Config.WriteError.Title"), 
        DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Config.WriteError.Description1"), directoryPath,
          LocalizationManager.GetLocalizedString("Dialog.Config.WriteError.Description2")
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
      DialogManager.ShowConfig_LoadErrorInternal(
        owner, configPath, LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Description2Alt1"), null
      );
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
        owner, configPath, LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Description2Alt2"), exceptionDetailText
      );
    }

    /// <summary>
    ///   Creates and shows a dialog saying that the configuration file could not be loaded because
    ///   an unhandled error occurred.
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

      message.Inlines.Add(LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Description1"));
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
        messagePart3.Append(reason);
      } else {
        messagePart3.AppendLine(LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Description2"));
      }

      if (exceptionDetailText != null) {
        messagePart3.AppendLine(LocalizationManager.GetLocalizedString("Dialog.Error.Global.ExceptionDetailsInfo"));
      }

      messagePart3.AppendLine();
      messagePart3.Append(LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Description3"));

      message.Inlines.Add(messagePart3.ToString());

      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Config.ReadError.Title"), message, TaskDialogButtons.OK, "Warning"
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
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Update.Available.Title"), null, 0, "Question"
      );
      
      taskDialog.Content = DialogManager.CreateInsertedBoldText(
        String.Concat(
          LocalizationManager.GetLocalizedString("Dialog.Update.Available.Description1"), "\n\n", 
          LocalizationManager.GetLocalizedString("Dialog.Update.Available.Description2"), currentVersion, "\n"
        ),
        String.Concat(LocalizationManager.GetLocalizedString("Dialog.Update.Available.Description3"), newVersion),
        String.Empty
      );
      
      // Note: We use the tag values as kind of button-key.
      taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
        Content = LocalizationManager.GetLocalizedString("Dialog.Update.Available.InstallNow"), Tag = "Install"
      });
      taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
        Content = LocalizationManager.GetLocalizedString("Dialog.Update.Available.OpenWebsite"), Tag = "OpenWebsite"
      });
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

      if (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Cancel) {
        return "Cancel";
      }

      return (((System.Windows.Controls.Button)taskDialog.Result.Button).Tag as String);
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Update.Downloading.Title"), String.Empty, 
        TaskDialogButtons.None, null
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
        Title = LocalizationManager.GetLocalizedString("Dialog.Update.Downloading.Title")
      };

      System.Windows.Controls.Button cancelButton = new System.Windows.Controls.Button();
      cancelButton.Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.Cancel");
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Update.NoUpdateAvailable.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Update.NoUpdateAvailable.Description"), 
        TaskDialogButtons.OK, "Information"
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Update.UnableToConnect.Description1"),
        "\n" + LocalizationManager.GetLocalizedString("Dialog.Update.UnableToConnect.Description2"),
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
    public static Boolean ShowUpdate_UpdateFileNotFound(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Update.FileNotFound.Title"),
        String.Concat(
          LocalizationManager.GetLocalizedString("Dialog.Update.FileNotFound.Description1"), "\n",
          LocalizationManager.GetLocalizedString("Dialog.Update.FileNotFound.Description2")
        ),
        TaskDialogButtons.Yes | TaskDialogButtons.No, "Error"
      );

      taskDialog.Show();

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.Yes);
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
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.FileNotFound.Title"), null, TaskDialogButtons.OK, "Error"
        );

        taskDialog.Content = DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Error.FileNotFound.Description1"), filePath, 
          LocalizationManager.GetLocalizedString("Dialog.Error.FileNotFound.Description2")
        );
      } else {
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.FileNotFound.Title"), 
          LocalizationManager.GetLocalizedString("Dialog.Error.FileNotFound.AltDescription"),
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
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.FileInUse.Title"), null, TaskDialogButtons.OK, "Error"
        );

        taskDialog.Content = DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Error.FileInUse.Description1"), filePath, 
          LocalizationManager.GetLocalizedString("Dialog.Error.FileInUse.Description2")
        );
      } else {
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.FileInUse.Title"), 
          LocalizationManager.GetLocalizedString("Dialog.Error.FileInUse.AltDescription"), TaskDialogButtons.OK, "Error"
        );
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
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryNotFound.Title"), null, 
          TaskDialogButtons.OK, "Error"
        );

        taskDialog.Content = DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryNotFound.Description1"), directoryPath, 
          LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryNotFound.Description2")
        );
      } else {
        taskDialog = DialogManager.CreateDialog(
          owner, LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryNotFound.Title"), 
          LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryNotFound.AltDescription"), 
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.OutOfMemory.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Error.OutOfMemory.Description"), TaskDialogButtons.OK, "Error"
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.MissingFileSystemRights.Title"), null, 
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Owner = DialogManager.CreateInsertedBoldText(
        LocalizationManager.GetLocalizedString("Dialog.Error.MissingFileSystemRights.Description1"), path, 
        LocalizationManager.GetLocalizedString("Dialog.Error.MissingFileSystemRights.Description2")
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.MissingFrameworkRights.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Error.MissingFrameworkRights.Description"),
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
    ///   Creates and shows a dialog saying that the application is already running.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowGeneral_AppAlreadyRunning(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.AlreadyRunning.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Error.AlreadyRunning.Description"), TaskDialogButtons.OK, "Error"
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
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.AppDataDirectoryCreation.Title"), 
        DialogManager.CreateInsertedBoldText(
          LocalizationManager.GetLocalizedString("Dialog.Error.AppDataDirectoryCreation.Description1"), directoryPath, 
          LocalizationManager.GetLocalizedString("Dialog.Error.AppDataDirectoryCreation.Description2")
        ), 
        TaskDialogButtons.OK, "Error"
      );
      
      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that an unhandled exception occurred.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    /// <param name="exceptionDetailText">
    ///   The string containing the exception detail text.
    /// </param>
    public static void ShowGeneral_UnhandledException(Window owner, String exceptionDetailText) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner, LocalizationManager.GetLocalizedString("Dialog.Error.UnhandledException.Title"), 
        String.Format(
          LocalizationManager.GetLocalizedString("Dialog.Error.UnhandledException.Description"), 
          Assembly.GetAssembly(typeof(DialogManager)).GetName().Version
        ),
        TaskDialogButtons.OK, null
      );

      DialogManager.AttachExpansion_ExceptionDetails(taskDialog, exceptionDetailText);

      taskDialog.Show();
    }

    /// <summary>
    ///   Creates and shows a dialog saying that loading of directories is not supported yet.
    /// </summary>
    /// <param name="owner">
    ///   The owner of the dialog window.
    /// </param>
    public static void ShowUnsupported_LoadDirectory(Window owner) {
      TaskDialog taskDialog = DialogManager.CreateDialog(
        owner,LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryLoadingNotSupported.Title"), 
        LocalizationManager.GetLocalizedString("Dialog.Error.DirectoryLoadingNotSupported.Description"),
        TaskDialogButtons.OK, "Error"
      );
      
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
    private static Boolean ShowInputDialog(Window owner, String title, String message, ref String inputText) {
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

      return (DialogManager.GetPressedButton(taskDialog.Result) == TaskDialogButtons.OK);
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
        switch (buttonData.Button) {
          case TaskDialogButtons.OK:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.OK"),
              IsDefault = true,
              Tag = TaskDialogButtons.OK
            });

            break;
          case TaskDialogButtons.Cancel:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.Cancel"),
              IsCancel = true,
              Tag = TaskDialogButtons.Cancel
            });

            break;
          case TaskDialogButtons.Yes:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.Yes"),
              IsDefault = true,
              Tag = TaskDialogButtons.Yes
            });

            break;
          case TaskDialogButtons.No:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.No"),
              IsCancel = true,
              Tag = TaskDialogButtons.No
            });

            break;
          case TaskDialogButtons.Retry:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.Retry"),
              Tag = TaskDialogButtons.Retry
            });

            break;
          case TaskDialogButtons.Close:
            taskDialog.Buttons.Add(new System.Windows.Controls.Button() {
              Content = LocalizationManager.GetLocalizedString("DialogGlobal.Button.Close"),
              IsCancel = true,
              Tag = TaskDialogButtons.Close
            });

            break;
        }
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
        Text = String.Concat(LocalizationManager.GetLocalizedString("Dialog.Error.Global.ExceptionDetails"), "\n", details), 
        FontStyle = FontStyles.Italic 
      };

      taskDialog.ExpansionButtonContent = LocalizationManager.GetLocalizedString("Dialog.Error.Global.ExceptionDetailsExpander");
      taskDialog.ExpansionPosition = TaskDialogExpansionPosition.Header;
    }

    private static TaskDialogButtons? GetPressedButton(TaskDialogResult result) {
      var button = (result.Button as System.Windows.Controls.Button);

      if (button != null) {
        var standardButton = button.Tag as TaskDialogButtons?;

        if (standardButton != null) {
          return standardButton;
        }
      }

      return null;
    }
    #endregion
  }
}
