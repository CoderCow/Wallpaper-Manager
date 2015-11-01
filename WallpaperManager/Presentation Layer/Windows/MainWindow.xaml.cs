// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Shell;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Bitmap = System.Drawing.Bitmap;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;

using Path = Common.IO.Path;
using Common.Presentation;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The Main Window used to display the <see cref="Wallpaper" /> and <see cref="WallpaperCategory" /> objects and to
  ///   provide main Graphical User Interface functions.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public partial class MainWindow: Window, IWeakEventListener, INotifyPropertyChanged, IDisposable {
    #region Constants: WallpaperSelectionDialogFilter, AutomaticallyCreatedCategoryName
    /// <summary>
    ///   Represents the filter string used in the "Add Wallpaper(s)" dialog.
    /// </summary>
    public const String WallpaperSelectionDialogFilter = @"JPEG Files (*.jpg, *.jpeg, *.jpe, *.jfef)|*.jpg;*.jpeg;*.jpe;*.jfef|EXIF Files (*.exif)|*.exif|GIF Files (*.gif)|*.gif|PNG Files (*.png)|*.png|TIFF Files (*.tif, *.tiff)|*.tif;*.tiff|Bitmap Files (*.bmp, *.dib)|*.bmp;*.dib|All Supported Files|*.jpg;*.jpeg;*.jpe;*.jfif;*.exif;*.gif;*.png;*.tif;*.tiff;*.bmp;*.dib|All Files|*.*";
    #endregion

    #region Constants: MainIconResName, TimeIconOverlayAccessibilityText, AutocyclingActivatedOverlayAccessibilityText, AutocyclingActivatedIconResPath, AutocyclingDeactivatedIconResPath
    /// <summary>
    ///   Represents the resource name of the main icon.
    /// </summary>
    private const String MainIconResName = "WallpaperManager.Presentation_Layer.Resources.Icons.Main.ico";

    /// <summary>
    ///   Represents the resource path of the autocycling started icon used as icon overlay.
    /// </summary>
    private const String AutocyclingActivatedIconResPath = @"Presentation Layer/Resources/Icons/Start Cycling.png";

    /// <summary>
    ///   Represents the resource path of the autocycling stopped icon used as icon overlay.
    /// </summary>
    private const String AutocyclingDeactivatedIconResPath = @"Presentation Layer/Resources/Icons/Stop Cycling.png";
    #endregion

    #region Static Property: IconOverlayTextFont, IconOverlayTextFormat, IconOverlayTextColor
    /// <summary>
    ///   <inheritdoc cref="IconOverlayTextFont" select='../value/node()' />
    /// </summary>
    private static readonly Font iconOverlayTextFont = new Font("Arial", 7);
    
    /// <summary>
    ///   Gets the <see cref="Font" /> used when drawing the overlay icon text.
    /// </summary>
    /// <value>
    ///   The <see cref="Font" /> used when drawing the overlay icon text.
    /// </value>
    protected static Font IconOverlayTextFont {
      get { return MainWindow.iconOverlayTextFont; }
    }

    /// <summary>
    ///   <inheritdoc cref="IconOverlayTextFormat" select='../value/node()' />
    /// </summary>
    private static readonly StringFormat iconOverlayTextFormat = new StringFormat() {
      FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip,
      Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center
    };
    
    /// <summary>
    ///   Gets the <see cref="StringFormat" /> used when drawing the overlay icon text.
    /// </summary>
    /// <value>
    ///   The <see cref="StringFormat" /> used when drawing the overlay icon text.
    /// </value>
    protected static StringFormat IconOverlayTextFormat {
      get { return MainWindow.iconOverlayTextFormat; }
    }

    /// <summary>
    ///   <inheritdoc cref="IconOverlayTextColor" select='../value/node()' />
    /// </summary>
    private static readonly Brush iconOverlayTextColor = Brushes.White;
    
    /// <summary>
    ///   Gets the <see cref="Brush" /> used when drawing the overlay icon text.
    /// </summary>
    /// <value>
    ///   The <see cref="Brush" /> used when drawing the overlay icon text.
    /// </value>
    protected static Brush IconOverlayTextColor {
      get { return MainWindow.iconOverlayTextColor; }
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
    protected AppEnvironment Envirnoment {
      get { return this.environment; }
    }
    #endregion

    #region Property: ApplicationVM
    /// <summary>
    ///   <inheritdoc cref="ApplicationVM" select='../value/node()' />
    /// </summary>
    private readonly ApplicationVM applicationVM;

    /// <summary>
    ///   Gets the <see cref="ApplicationVM" /> instance providing the main interface to the application.
    /// </summary>
    /// <value>
    ///   The <see cref="ApplicationVM" /> instance providing the main interface to the application.
    /// </value>
    /// <seealso cref="WallpaperManager.ApplicationInterface.ApplicationVM">ApplicationVM Class</seealso>
    public ApplicationVM ApplicationVM {
      get { return this.applicationVM; }
    }
    #endregion
    
    #region Property: DisplayCycleTimeAsIconOverlay
    /// <summary>
    ///   <inheritdoc cref="DisplayCycleTimeAsIconOverlay" select='../value/node()' />
    /// </summary>
    private Boolean displayCycleTimeAsIconOverlay;

    /// <inheritdoc cref="GeneralConfig.DisplayCycleTimeAsIconOverlay" />
    public Boolean DisplayCycleTimeAsIconOverlay {
      get { return this.displayCycleTimeAsIconOverlay; }
      set {
        this.displayCycleTimeAsIconOverlay = value;

        if (value) {
          this.refreshOverlayIconTimer.Start();
        } else {
          this.refreshOverlayIconTimer.Stop();
        }
        this.UpdateOverlayIcon();
        this.OnPropertyChanged("DisplayCycleTimeAsIconOverlay");
      }
    }
    #endregion

    #region Property: MinimizeOnClose
    /// <summary>
    ///   <inheritdoc cref="MinimizeOnClose" select='../value/node()' />
    /// </summary>
    private Boolean minimizeOnClose;

    /// <inheritdoc cref="GeneralConfig.MinimizeOnClose" />
    public Boolean MinimizeOnClose {
      get { return this.minimizeOnClose; }
      set {
        this.minimizeOnClose = value;
        this.OnPropertyChanged("MinimizeOnClose");
      }
    }
    #endregion

    #region Property: WallpaperDoubleClickAction
    /// <summary>
    ///   <inheritdoc cref="WallpaperDoubleClickAction" select='../value/node()' />
    /// </summary>
    private WallpaperClickAction wallpaperDoubleClickAction;

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    public WallpaperClickAction WallpaperDoubleClickAction {
      get { return this.wallpaperDoubleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(WallpaperClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(WallpaperClickAction), value));
        }

        this.wallpaperDoubleClickAction = value;
        this.OnPropertyChanged("WallpaperDoubleClickAction");
      }
    }
    #endregion

    #region Property: RefreshOverlayIconTimer
    /// <summary>
    ///   <inheritdoc cref="RefreshOverlayIconTimer" select='../value/node()' />
    /// </summary>
    private readonly DispatcherTimer refreshOverlayIconTimer;
    
    /// <summary>
    ///   Gets the <see cref="DispatcherTimer" /> used to update the overlay icon with the time remaining to the next cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="DispatcherTimer" /> used to update the overlay icon with the time remaining to the next cycle.
    /// </value>
    protected DispatcherTimer RefreshOverlayIconTimer {
      get { return this.refreshOverlayIconTimer; }
    }
    #endregion

    /*** Category Related Commands ***/
    #region Command: AddCategoryOrSyncFolderCommand
    /// <summary>
    ///   The Add Category Or Sync Folder <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand AddCategoryOrSyncFolderCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddCategoryOrSyncFolderCommand" />
    protected virtual void AddCategoryOrSyncFolderCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and creates a new 
    ///   <see cref="WallpaperCategory" /> or <see cref="SynchronizedWallpaperCategory" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddCategoryOrSyncFolderCommand" />
    /// <seealso cref="SynchronizedWallpaperCategory">SynchronizedWallpaperCategory Class</seealso>
    protected virtual void AddCategoryOrSyncFolderCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
        if (MainWindow.AddSynchronizedCategoryCommand.CanExecute(null, e.Source as IInputElement)) {
          MainWindow.AddSynchronizedCategoryCommand.Execute(null, e.Source as IInputElement);
        }
      } else {
        if (MainWindow.AddCategoryCommand.CanExecute(null, e.Source as IInputElement)) {
          MainWindow.AddCategoryCommand.Execute(null, e.Source as IInputElement);
        }
      }
    }
    #endregion

    #region Command: AddCategory
    /// <summary>
    ///   The Add Category <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand AddCategoryCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddCategoryCommand" />
    protected virtual void AddCategoryCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = this.ApplicationVM.WallpaperCategoryCollectionVM.AddCategoryCommand.CanExecute(null);
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and creates a new 
    ///   <see cref="WallpaperCategory" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddCategoryCommand" />
    /// <seealso cref="WallpaperCategory">WallpaperCategory Class</seealso>
    protected virtual void AddCategoryCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      String categoryName = null;

      // Loop showing the input dialog until a valid name is entered or cancel is pressed.
      while (true) {
        if (DialogManager.ShowCategory_AddNew(this, ref categoryName)) {
          WallpaperCategory wallpaperCategory;

          try {
            wallpaperCategory = new WallpaperCategory(categoryName);
          } catch (ArgumentOutOfRangeException) {
            DialogManager.ShowCategory_NameInvalidLength(this);
            continue;
          } catch (ArgumentException) {
            DialogManager.ShowCategory_NameInvalid(this);
            continue;
          }

          this.ApplicationVM.WallpaperCategoryCollectionVM.AddCategoryCommand.Execute(wallpaperCategory);
        }

        break;
      }
    }
    #endregion

    #region Command: AddSynchronizedCategory
    /// <summary>
    ///   The Add Synchronized Category <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand AddSynchronizedCategoryCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddSynchronizedCategoryCommand" />
    protected virtual void AddSynchronizedCategoryCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = this.ApplicationVM.WallpaperCategoryCollectionVM.AddCategoryCommand.CanExecute(null);
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and creates a new 
    ///   <see cref="SynchronizedWallpaperCategory" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddCategoryCommand" />
    /// <seealso cref="SynchronizedWallpaperCategory">SynchronizedWallpaperCategory Class</seealso>
    protected virtual void AddSynchronizedCategoryCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      Path selectedDirectory = new Path("C:\\");
          
      if (DialogManager.ShowSynchronizedCategory_SelectDirectory(ref selectedDirectory)) {
        String categoryName = selectedDirectory.FileName;

        // Loop showing the input dialog until a valid name is entered or cancel is pressed.
        while (true) {
          if (DialogManager.ShowSynchronizedCategory_AddNew(this, ref categoryName)) {
            WallpaperCategory wallpaperCategory;

            try {
              wallpaperCategory = new SynchronizedWallpaperCategory(categoryName, selectedDirectory);
            } catch (ArgumentOutOfRangeException) {
              DialogManager.ShowCategory_NameInvalidLength(this);
              continue;
            } catch (ArgumentException) {
              DialogManager.ShowCategory_NameInvalid(this);
              continue;
            } catch (DirectoryNotFoundException) {
              DialogManager.ShowGeneral_DirectoryNotFound(this, selectedDirectory);
              continue;
            }

            this.ApplicationVM.WallpaperCategoryCollectionVM.AddCategoryCommand.Execute(wallpaperCategory);
          }

          break;
        }
      }
    }
    #endregion

    #region Command: ConfigureWallpaperDefaultSettings
    /// <summary>
    ///   The Configure Wallpaper Default Settings <see cref="RoutedCommand" />.
    /// </summary>
    public static readonly RoutedCommand ConfigureWallpaperDefaultSettingsCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureWallpaperDefaultSettingsCommand" />
    protected virtual void ConfigureWallpaperDefaultSettingsCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.ConfigureDefaultSettingsCommand.CanExecute()
      ); 
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and executed the
    ///   <see cref="WallpaperCategoryVM.ConfigureDefaultSettingsCommand" /> of the underlying 
    ///   <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="ConfigureWallpaperDefaultSettingsCommand" />
    protected virtual void ConfigureWallpaperDefaultSettingsCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.ConfigureDefaultSettingsCommand.Execute();
    }
    #endregion

    #region Command: RenameCategory
    /// <summary>
    ///   The Rename Category <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand RenameCategoryCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="RenameCategoryCommand" />
    protected virtual void RenameCategoryCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = this.ApplicationVM.WallpaperCategoryCollectionVM.RenameSelectedCommand.CanExecute(null);
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method renames the selected <see cref="WallpaperCategory" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="RenameCategoryCommand" />
    protected virtual void RenameCategoryCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      String categoryName = this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.Category.Name;
      
      // Loop showing the input dialog until a valid name is entered or cancel is pressed.
      while (true) {
        if (!DialogManager.ShowCategory_Rename(this, ref categoryName)) {
          break;
        }

        try {
          this.ApplicationVM.WallpaperCategoryCollectionVM.RenameSelectedCommand.Execute(categoryName);
          break;
        } catch (ArgumentOutOfRangeException) {
          DialogManager.ShowCategory_NameInvalidLength(this);
          continue;
        } catch (ArgumentException) {
          DialogManager.ShowCategory_NameInvalid(this);
          continue;
        }
      }
    }
    #endregion

    #region Command: RemoveCategory
    /// <summary>
    ///   The Remove Category <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand RemoveCategoryCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="RemoveCategoryCommand" />
    protected virtual void RemoveCategoryCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = this.ApplicationVM.WallpaperCategoryCollectionVM.RemoveSelectedCommand.CanExecute();
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method removes the selected <see cref="WallpaperCategory" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="RemoveCategoryCommand" />
    protected virtual void RemoveCategoryCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      // If the user is holding shift, we don't show a confirmation dialog.
      if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)) {
        Boolean dialogResult = DialogManager.ShowCategory_WantDelete(
          this, this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.Category.Name
        );

        if (!dialogResult) {
          return;
        }
      }
      
      this.ApplicationVM.WallpaperCategoryCollectionVM.RemoveSelectedCommand.Execute();
    }
    #endregion

    /*** Wallpaper Related Commands ***/
    #region Command: WallpaperClickAction
    /// <summary>
    ///   The Wallpaper Click Action <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand WallpaperClickActionCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="WallpaperClickActionCommand">Wallpaper Click Action Command</seealso>
    protected virtual void WallpaperClickActionCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method adds one or more <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="WallpaperClickActionCommand">Wallpaper Click Action Command</seealso>
    protected virtual void WallpaperClickActionCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      switch (this.WallpaperDoubleClickAction) {
        case WallpaperClickAction.ApplyOnDesktop:
          if (MainWindow.ApplyWallpapersCommand.CanExecute(null, this)) {
            MainWindow.ApplyWallpapersCommand.Execute(null, this);
          }

          break;
        case WallpaperClickAction.ShowConfigurationWindow:
          if (MainWindow.ConfigureWallpapersCommand.CanExecute(null, this)) {
            MainWindow.ConfigureWallpapersCommand.Execute(null, this);
          }

          break;
      }
    }
    #endregion

    #region Command: AddWallpapers
    /// <summary>
    ///   The Add Wallpaper <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand AddWallpapersCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddWallpapersCommand" />
    protected virtual void AddWallpapersCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.Category != null
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method adds one or more <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="AddWallpapersCommand" />
    protected virtual void AddWallpapersCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (this.CreateOrSelectCategoryIfNecessary()) {
        using (OpenFileDialog fileDialog = new OpenFileDialog()) {
          fileDialog.Filter = MainWindow.WallpaperSelectionDialogFilter;
          fileDialog.Multiselect = true;
          fileDialog.CheckFileExists = true;

          if (fileDialog.ShowDialog() == FormsDialogResult.OK) {
            for (Int32 i = 0; i < fileDialog.FileNames.Length; i++) {
              Path filePath = new Path(fileDialog.FileNames[i]);

              this.AddWallpaper(filePath);
            }
          }

          // For some reason the Window doesn't get the focus back if the open file dialog has been shown.
          this.Focus();
        }
      }
    }
    #endregion

    #region Command: ApplyWallpapers
    /// <summary>
    ///   The Apply Wallpapers <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand ApplyWallpapersCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="ApplyWallpapersCommand" />
    protected virtual void ApplyWallpapersCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.ApplySelectedCommand.CanExecute()
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method applys the selected <see cref="Wallpaper" /> instances on the users Desktop.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="ApplyWallpapersCommand" />
    protected virtual void ApplyWallpapersCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      try {
        if (this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.SelectedWallpaperVMs.Count == 0) {
          DialogManager.ShowWallpapers_NoOneSelected(this);
          return;
        }

        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.ApplySelectedCommand.Execute();
      } catch (InvalidOperationException) {
        DialogManager.ShowCycle_WallpapersToUseInvalid(this);
      } catch (FileNotFoundException exception) {
        DialogManager.ShowGeneral_FileNotFound(this, exception.FileName);
      }
    }
    #endregion

    #region Command: ConfigureWallpapers
    /// <summary>
    ///   The Configure Wallpapers <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand ConfigureWallpapersCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="ConfigureWallpapersCommand" />
    protected virtual void ConfigureWallpapersCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.SelectedWallpaperVMs.Count > 0
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows the <see cref="ConfigWallpaperWindow" /> for the selected <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="ConfigureWallpapersCommand" />
    protected virtual void ConfigureWallpapersCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.SelectedWallpaperVMs.Count > 0) {
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.ConfigureSelectedCommand.Execute();
      } else {
        DialogManager.ShowWallpapers_NoOneSelected(this);
      }
    }
    #endregion

    #region Command: OpenWallpapersFolder
    /// <summary>
    ///   The Open Wallpapers Folder <see cref="RoutedCommand" />.
    /// </summary>
    public static readonly RoutedCommand OpenWallpapersFolderCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="OpenWallpapersFolderCommand" />
    protected virtual void OpenWallpapersFolderCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.OpenFolderOfSelectedCommand.CanExecute()
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and executes the
    ///   <see cref="WallpaperCategoryVM.OpenFolderOfSelectedCommand" /> of the underlying <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="OpenWallpapersFolderCommand" />
    protected virtual void OpenWallpapersFolderCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.OpenFolderOfSelectedCommand.Execute();
    }
    #endregion

    #region Command: RemoveWallpapers
    /// <summary>
    ///   The Remove Wallpapers <see cref="RoutedCommand" />.
    /// </summary>
    public static readonly RoutedCommand RemoveWallpapersCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="RemoveWallpapersCommand" />
    protected virtual void RemoveWallpapersCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.RemoveSelectedCommand.CanExecute()
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" /> and executes the
    ///   <see cref="WallpaperCategoryVM.RemoveSelectedCommand" /> of the underlying <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="RemoveWallpapersCommand" />
    protected virtual void RemoveWallpapersCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.RemoveSelectedCommand.Execute();
    }
    #endregion

    #region Command: RemoveWallpapersPhysically
    /// <summary>
    ///   The Remove Wallpapers Physically <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand RemoveWallpapersPhysicallyCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    /// <seealso cref="RemoveWallpapersPhysicallyCommand" />
    protected virtual void RemoveWallpapersPhysicallyCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM != null &&
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.RemoveSelectedPhysicallyCommand.CanExecute()
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method removes the physical image files of the selected 
    ///   <see cref="WallpaperVM">WallpaperVMs</see>.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="RemoveWallpapersPhysicallyCommand" />
    protected virtual void RemoveWallpapersPhysicallyCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (DialogManager.ShowWallpaper_WantDeletePhysically(this)) {
        try {
          this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.RemoveSelectedPhysicallyCommand.Execute();
        } catch (FileNotFoundException exception) {
          DialogManager.ShowGeneral_FileNotFound(this, exception.FileName);
        } catch (UnauthorizedAccessException exception) {
          DialogManager.ShowGeneral_MissingFileSystemRightsOrWriteProtected(
            this, null, exception.ToString()
          );
        } catch (IOException) {
          DialogManager.ShowGeneral_FileInUse(this, null);
        }
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    /// <param name="environment">
    ///   The <see cref="AppEnvironment" /> instance providing serveral data related to Wallpaper Manager's environment.
    /// </param>
    /// <param name="applicationVM">
    ///   The <see cref="ApplicationVM" /> instance providing the main interface to the application.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="environment" /> or <paramref name="applicationVM" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="AppEnvironment">AppEnvironment Class</seealso>
    /// <seealso cref="WallpaperManager.ApplicationInterface.ApplicationVM">WallpaperManager.ApplicationVM Class</seealso>
    public MainWindow(AppEnvironment environment, ApplicationVM applicationVM) {
      if (environment == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("environment"));
      }
      if (applicationVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("ApplicationVM"));
      }

      this.environment = environment;
      this.applicationVM = applicationVM;
      this.applicationVM.RequestViewClose += this.ApplicationVM_RequestViewClose;

      this.InitializeComponent();
      
      #if BetaBuild
      this.Title += String.Format(
        " {0}.{1} Beta {2}", environment.AppVersion.Major, environment.AppVersion.Minor, environment.AppVersion.Revision
      );
      #endif
      // Initialize the timer used to update the overlay icon if requested.
      this.refreshOverlayIconTimer = new DispatcherTimer(DispatcherPriority.Background);
      this.RefreshOverlayIconTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
      this.RefreshOverlayIconTimer.Tick += this.RefreshOverlayIconTimer_Tick;

      // We have to watch the IsAutocycling property to update the overlay icon.
      PropertyChangedEventManager.AddListener(this.ApplicationVM.WallpaperChangerVM, this, String.Empty);

      // Since XAML allows to set png files as icon only, we do it by code.
      using (Stream iconStream = Assembly.GetAssembly(
        typeof(NotifyIconManager)).GetManifestResourceStream(MainWindow.MainIconResName)
      ) {
        this.Icon = BitmapFrame.Create(iconStream);
      }

      // We have to call this after the Window is fully constructed.
      this.Loaded += delegate {
        // This sleep prevents a Windows Taskbar error. When closing the main window, and opening it again the overlay
        // icon will be, for some reason, not applied after the window loaded.. but if waiting for one millisecond it works.
        System.Threading.Thread.Sleep(1);

        this.UpdateOverlayIcon();
      };
    }
    #endregion

    #region Methods: WallpaperChangerVM_PropertyChanged, RefreshOverlayIconTimer_Tick, UpdateOverlayIcon
    /// <summary>
    ///   Handles the <see cref="INotifyPropertyChanged.PropertyChanged" /> event of a <see cref="WallpaperChangerVM" />
    ///   instance.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WallpaperChangerVM_PropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "IsAutocycling") {
        this.RefreshOverlayIconTimer.IsEnabled = this.ApplicationVM.WallpaperChangerVM.IsAutocycling;
        this.UpdateOverlayIcon();
      }
    }

    /// <summary>
    ///   Handles the <see cref="DispatcherTimer.Tick" /> event of the <see cref="RefreshOverlayIconTimer" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void RefreshOverlayIconTimer_Tick(Object sender, EventArgs e) {
      if (
        !this.ApplicationVM.WallpaperChangerVM.IsAutocycling || 
        !this.DisplayCycleTimeAsIconOverlay || 
        !AppEnvironment.IsWindows7 || 
        this.isDisposed
      ) {
        this.RefreshOverlayIconTimer.Stop();
      }
      
      this.UpdateOverlayIcon();
    }

    /// <summary>
    ///   The last applied overlay icon text.
    /// </summary>
    private String lastOverlayIconText;

    /// <summary>
    ///   Draws a new overlay icon for the Windows 7 Taskbar.
    /// </summary>
    /// <remarks>
    ///   This method will simply do nothing if <see cref="AppEnvironment" />.<see cref="AppEnvironment.IsWindows7" /> is 
    ///   <c>false</c>.
    /// </remarks>
    private void UpdateOverlayIcon() {
      if (!AppEnvironment.IsWindows7) {
        return;
      }

      // TODO: Exception Handling
      if (this.TaskbarItemInfo == null) {
        this.TaskbarItemInfo = new TaskbarItemInfo();
        this.TaskbarItemInfo.Description = LocalizationManager.GetLocalizedString("ToolTip.AutocyclingActivated.Description");
      }

      if (this.ApplicationVM.WallpaperChangerVM.IsAutocycling) {
        if (this.DisplayCycleTimeAsIconOverlay) {
          TimeSpan timeSpanUntilNextCycle = this.ApplicationVM.WallpaperChangerVM.TimeSpanUntilNextCycle;

          if (timeSpanUntilNextCycle.Seconds > 0) {
            String timeOverlayText;

            if (timeSpanUntilNextCycle.TotalMinutes > 60) {
              if (timeSpanUntilNextCycle.TotalHours > 9) {
                timeOverlayText = "9h";
              } else {
                timeOverlayText = Math.Round(timeSpanUntilNextCycle.TotalHours + 1, 0) + "h";
              }
            } else if (timeSpanUntilNextCycle.TotalMinutes > 10) {
              timeOverlayText = Math.Truncate(timeSpanUntilNextCycle.TotalMinutes + 1).ToString(CultureInfo.CurrentCulture);
            } else if (timeSpanUntilNextCycle.TotalSeconds > 60) {
              timeOverlayText = Math.Truncate(timeSpanUntilNextCycle.TotalMinutes + 1) + "m";
            } else if (timeSpanUntilNextCycle.TotalSeconds > 10) {
              timeOverlayText = Math.Truncate(timeSpanUntilNextCycle.TotalSeconds).ToString(CultureInfo.CurrentCulture);
            } else {
              timeOverlayText = Math.Truncate(timeSpanUntilNextCycle.TotalSeconds) + "s";
            }

            // Prevent the icon from being updated if it is not required.
            if (this.lastOverlayIconText != timeOverlayText) {
              Brush timeOverlayBackgroundBrush = null;
              Bitmap timeOverlayBitmap = null;
              Graphics timeOverlayGraphics = null;

              try {
                timeOverlayBitmap = new Bitmap(16, 16);
                timeOverlayGraphics = Graphics.FromImage(timeOverlayBitmap);

                if (timeSpanUntilNextCycle.TotalSeconds > 60) {
                  timeOverlayBackgroundBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                } else {
                  timeOverlayBackgroundBrush = new SolidBrush(Color.FromArgb(160, 255, 0, 0));
                }

                timeOverlayGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                timeOverlayGraphics.FillEllipse(timeOverlayBackgroundBrush, 0, 0, 16, 16);
                timeOverlayGraphics.DrawString(
                  timeOverlayText,
                  MainWindow.IconOverlayTextFont,
                  MainWindow.IconOverlayTextColor,
                  new RectangleF(0, 1, 18, 16),
                  MainWindow.IconOverlayTextFormat
                );

                timeOverlayGraphics.Flush();
                using (MemoryStream bitmapStream = new MemoryStream()) {
                  timeOverlayBitmap.Save(bitmapStream, System.Drawing.Imaging.ImageFormat.Png);
                  bitmapStream.Position = 0;

                  this.TaskbarItemInfo.Overlay = BitmapFrame.Create(
                    bitmapStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad
                  );
                }
              } finally {
                if (timeOverlayBackgroundBrush != null) {
                  timeOverlayBackgroundBrush.Dispose();
                }
                if (timeOverlayBitmap != null) {
                  timeOverlayBitmap.Dispose();
                }
                if (timeOverlayGraphics != null) {
                  timeOverlayGraphics.Dispose();
                }
              }

              this.lastOverlayIconText = timeOverlayText;
            }
          }
        } else {
          this.TaskbarItemInfo.Overlay = BitmapFrame.Create(
            new Uri(@"pack://application:,,,/" + MainWindow.AutocyclingActivatedIconResPath, UriKind.Absolute), 
            BitmapCreateOptions.None, BitmapCacheOption.OnLoad
          );
        }
      } else {
        this.TaskbarItemInfo.Overlay = BitmapFrame.Create(
          new Uri(@"pack://application:,,,/" + MainWindow.AutocyclingDeactivatedIconResPath, UriKind.Absolute),
          BitmapCreateOptions.None, BitmapCacheOption.OnLoad
        );
      }
      
      this.TaskbarItemInfo.Overlay.Freeze();
    }
    #endregion

    #region Methods: WallpapersVM_AddWallpaperException, OnDrop, OnClosing, ApplicationVM_RequestViewClose
    // TODO: Reimplement MainWindow.WallpapersVM_AddWallpaperException.
    /*/// <summary>
    ///   Handles the <see cref="WallpaperCategoryVM.AddWallpaperException" /> event of the 
    ///   selected <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExceptionEventArgs" /> instance containing the event data.
    /// </param>
    private void WallpapersVM_AddWallpaperException(Object sender, ExceptionEventArgs e) {
      if (e.Exception is OutOfMemoryException) {
        // TODO: Somehow include the image's file name here.
        DialogManager.ShowWallpaper_LoadError(this);
      }

      if (e.Exception is FileNotFoundException) {
        DialogManager.ShowGeneral_FileNotFound(
          this, (e.Exception as FileNotFoundException).FileName
        );
      }
    }*/

    /// <inheritdoc />
    protected override void OnDrop(DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop, true)) {
        if (MainWindow.AddCategoryCommand.CanExecute(null, this)) {
          if (this.CreateOrSelectCategoryIfNecessary()) {
            if (MainWindow.AddWallpapersCommand.CanExecute(null, this)) {
              String[] data = (String[])e.Data.GetData(DataFormats.FileDrop, true);

              // TODO: Support folders. Also support creating Categories from folders, or just enumerating all images.
              for (Int32 i = 0; i < data.Length; i++) {
                Path dataPath = new Path(data[i]);

                // Does this path point at a directory?
                if (Directory.Exists(dataPath)) {
                  DialogManager.ShowUnsupported_LoadDirectory(this);
                  return;
                }

                this.AddWallpaper(dataPath);
              }
            }
          }
        }
      }

      base.OnDrop(e);
    }

    /// <inheritdoc />
    protected override void OnClosing(CancelEventArgs e) {
      if (this.MinimizeOnClose) {
        e.Cancel = true;
        this.WindowState = WindowState.Minimized;
      }

      base.OnClosing(e);
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperManager.ApplicationInterface.ApplicationVM.RequestViewClose" /> event of an 
    ///   <see cref="WallpaperManager.ApplicationInterface.ApplicationVM" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ApplicationVM_RequestViewClose(Object sender, EventArgs e) {
      this.Close();
    }
    #endregion

    #region Methods: CreateOrSelectCategoryIfNecessary, AddWallpaper
    /// <summary>
    ///   Tries to select an existing <see cref="WallpaperCategory" /> or shows a dialog asking whether to create a new 
    ///   <see cref="WallpaperCategory" /> if none exists.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if a category is selected at all; otherwise <c>false</c>.
    /// </returns>
    private Boolean CreateOrSelectCategoryIfNecessary() {
      if (this.ApplicationVM.WallpaperCategoryCollectionVM.Categories.Count == 0) {
        if (DialogManager.ShowCategory_NoCategoryAvailable(this)) {
          this.ApplicationVM.WallpaperCategoryCollectionVM.AddCategoryCommand.Execute(
            new WallpaperCategory(LocalizationManager.GetLocalizedString("CategoryData.DefaultName"))
            );
        } else {
          // The user does not want to create a new category, so we cannot continue.
          return false;
        }
      }

      // Is no category actually being selected?
      if (this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM == null) {
        if (this.ApplicationVM.WallpaperCategoryCollectionVM.Categories.Count != 1) {
          DialogManager.ShowCategory_NoOneSelected(this);

          return false;
        }

        // If there is no category selected, but only one in the list, we suggest that the user wants to work with this category.
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVMIndex = 0;
      }

      return true;
    }

    /// <summary>
    ///   Adds a new <see cref="Wallpaper" /> to the selected <see cref="WallpaperCategory" />.
    /// </summary>
    /// <param name="filePath">
    ///   The path of the image file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="filePath" /> is <c>null</c>.
    /// </exception>
    private void AddWallpaper(Path filePath) {
      if (filePath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone("filePath"));
      }

      if (this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.IsSynchronizedCategory) {
        Boolean doOverwrite = false;

        Retry:
        try {
          this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.AddSynchronizedImage(filePath, doOverwrite);
        } catch (FileNotFoundException) {
          DialogManager.ShowGeneral_FileNotFound(this, filePath);
        } catch (DirectoryNotFoundException) {
          DialogManager.ShowGeneral_DirectoryNotFound(this, null);
        } catch (IOException) {
          if (DialogManager.ShowSynchronizedCategory_FileAlreadyExist(this, null)) {
            doOverwrite = true;
            goto Retry;
          }
        }
      } else {
        this.ApplicationVM.WallpaperCategoryCollectionVM.SelectedCategoryVM.Add(
          new WallpaperVM(new Wallpaper(filePath))
          );
      }
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        if (sender is WallpaperChangerVM) {
          this.WallpaperChangerVM_PropertyChanged(sender, (PropertyChangedEventArgs)e);
          return true;
        }
      }

      return false;
    }
    #endregion

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    private Boolean isDisposed;

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          if (this.applicationVM != null) {
            this.applicationVM.RequestViewClose -= this.ApplicationVM_RequestViewClose;
          }
          if (this.refreshOverlayIconTimer != null) {
            this.refreshOverlayIconTimer.Stop();
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
    ///   Finalizes an instance of the <see cref="Application" /> class.
    /// </summary>
    ~MainWindow() {
      this.Dispose(false);
    }
    #endregion
  }

  /// <summary>
  ///   Immediate class, because generic type arguments are only supported in loose XAML.
  /// </summary>
  [ValueConversion(typeof(IList<WallpaperVM>), typeof(IList))]
  public class MainWindowWallpaperGenericListConverter: GenericToNonGenericCollectionConverter<WallpaperVM> {}
}
