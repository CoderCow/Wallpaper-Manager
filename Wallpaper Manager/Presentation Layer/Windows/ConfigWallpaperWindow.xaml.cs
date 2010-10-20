// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using ColorDialog = System.Windows.Forms.ColorDialog;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

using Common;
using Path = Common.IO.Path;

using WallpaperManager.Data;
using WallpaperManager.Application;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The Configuration Window used to configure one or more <see cref="WallpaperSettingsBase" /> instances using the
  ///   <see cref="ConfigWallpaperVM">ConfigWallpaper View Model</see>.
  /// </summary>
  /// <seealso cref="ConfigWallpaperVM">ConfigWallpaperVM Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class ConfigWallpaperWindow: Window {
    #region Constants: ConfigureWallpapersMode_Title, ConfigureDefaultSettingsMode_Title, ConfigureStaticWallpaperMode_Title
    /// <summary>
    ///   Defines the title of this window if 
    ///   <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM.ConfigurationMode" /> is set to 
    ///   <see cref="ConfigWallpaperMode.ConfigureWallpapers" />.
    /// </summary>
    private const String ConfigureWallpapersMode_Title = "Configure Wallpaper(s)";

    /// <summary>
    ///   Defines the title of this window if 
    ///   <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM.ConfigurationMode" /> is set to 
    ///   <see cref="ConfigWallpaperMode.ConfigureDefaultSettings" />.
    /// </summary>
    private const String ConfigureDefaultSettingsMode_Title = "Configure Wallpaper Default Settings";

    /// <summary>
    ///   Defines the title of this window if 
    ///   <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM.ConfigurationMode" /> is set to 
    ///   <see cref="ConfigWallpaperMode.ConfigureStaticWallpaper" />.
    /// </summary>
    private const String ConfigureStaticWallpaperMode_Title = "Configure Static Wallpaper";

    /// <summary>
    ///   Defines the resource key of the <see cref="BitmapImage" /> which is displayed if no preview can be generated.
    /// </summary>
    private const String NoPreviewImageResourceKey = "Images.NoPreview";
    #endregion

    #region Fields: previewWorker
    /// <summary>
    ///   The <see cref="BackgroundWorker" /> which generates the preview image in another <see cref="Thread" />.
    /// </summary>
    BackgroundWorker previewWorker;
    #endregion

    #region Dependency Property: WallpaperPreview
    /// <summary>
    ///   Identifies the <see cref="WallpaperPreview" /> Dependency Property.
    /// </summary>
    protected static readonly DependencyPropertyKey WallpaperPreviewPropertyKey = DependencyProperty.RegisterReadOnly(
      "WallpaperPreview", 
      typeof(ImageSource), 
      typeof(ConfigWallpaperWindow), 
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
    );
    
    /// <summary>
    ///   Gets the generated preview image.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The generated preview image.
    /// </value>
    [Bindable(true)]
    public ImageSource WallpaperPreview {
      get { return (ImageSource)this.GetValue(ConfigWallpaperWindow.WallpaperPreviewPropertyKey.DependencyProperty); }
      protected set { this.SetValue(ConfigWallpaperWindow.WallpaperPreviewPropertyKey, value); }
    }
    #endregion

    #region Property: ConfigWallpaperVM
    /// <summary>
    ///   <inheritdoc cref="ConfigWallpaperVM" select='../value/node()' />
    /// </summary>
    private readonly ConfigWallpaperVM configWallpaperVM;

    /// <summary>
    ///   Gets the <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM" /> instance used as interface to 
    ///   communicate with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM" /> instance used as interface to communicate 
    ///   with the application.
    /// </value>
    public ConfigWallpaperVM ConfigWallpaperVM {
      get { return this.configWallpaperVM; }
    }
    #endregion

    #region Property: ScreensSettings
    /// <summary>
    ///   <inheritdoc cref="ScreensSettings" select='../value/node()' />
    /// </summary>
    private readonly ScreenSettingsCollection screensSettings;

    /// <summary>
    ///   Gets the collection of <see cref="ScreenSettings" /> objects defining configuration values related to single screens.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="ScreenSettings" /> objects defining configuration values related to single screens.
    /// </value>
    public ScreenSettingsCollection ScreensSettings {
      get { return this.screensSettings; }
    }
    #endregion


    #region Methods: Constructor, UpdatePreviewBox
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigWallpaperWindow" /> class.
    /// </summary>
    /// <param name="configWallpaperVM">
    ///   The <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM" /> instance used as interface to communicate 
    ///   with the application.
    /// </param>
    /// <param name="screensSettings">
    ///   The collection of <see cref="ScreenSettings" /> objects defining configuration values related to single screens.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="configWallpaperVM" /> or <paramref name="screensSettings" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM">ConfigWallpaperVM Class</seealso>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreensSettings Class</seealso>
    public ConfigWallpaperWindow(ConfigWallpaperVM configWallpaperVM, ScreenSettingsCollection screensSettings) {
      if (configWallpaperVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("configWallpaperVM"));
      }
      if (screensSettings == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("screensSettings"));
      }

      this.configWallpaperVM = configWallpaperVM;
      this.configWallpaperVM.RequestClose += delegate (Object sender, RequestCloseEventArgs e) {
        this.DialogResult = e.Result;
        this.Close();
      };

      this.screensSettings = screensSettings;
      this.WallpaperPreview = null;

      this.InitializeComponent();
      
      switch (configWallpaperVM.ConfigurationMode) {
        case ConfigWallpaperMode.ConfigureDefaultSettings:
          this.Title = ConfigWallpaperWindow.ConfigureDefaultSettingsMode_Title;
          this.lblGeneral_ImagePath.Visibility = Visibility.Collapsed;
          this.pnlGeneral_ImagePath.Visibility = Visibility.Collapsed;
          this.rdbPlacement_AutoDetermine.Visibility = Visibility.Visible;
          this.ctlMultiscreenSetting.Content = this.ctlMultiscreenSetting.Resources["ConfigureDefaultSettingsContent"];

          break;
        case ConfigWallpaperMode.ConfigureStaticWallpaper:
          this.Title = ConfigWallpaperWindow.ConfigureStaticWallpaperMode_Title;
          this.slbPreview.Visibility = Visibility.Collapsed;
          this.ctlMultiscreenSetting.Content = null;
          this.ctlMultiscreenSetting.Visibility = Visibility.Collapsed;

          break;
        default:
          this.Title = ConfigWallpaperWindow.ConfigureWallpapersMode_Title;
          this.ctlMultiscreenSetting.Content = this.ctlMultiscreenSetting.Resources["ConfigureWallpapersContent"];

          break;
      }

      this.UpdatePreviewImage(null, null);
    }

    /// <summary>
    ///   Updates the wallpaper preview image in the preview box.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void UpdatePreviewImage(Object sender, RoutedEventArgs e) {
      if (this.ConfigWallpaperVM.ConfigurationMode == ConfigWallpaperMode.ConfigureStaticWallpaper) {
        return;
      }

      Path imageFilePath = Path.None;
      if ((this.ConfigWallpaperVM.HasImagePath != null) && (this.ConfigWallpaperVM.HasImagePath.Value)) {
        if (this.ConfigWallpaperVM.ImagePath != Path.None) {
          imageFilePath = this.ConfigWallpaperVM.ImagePath;
        } else {
          imageFilePath = ((Wallpaper)this.ConfigWallpaperVM.WallpaperData[0]).ImagePath;
        }
      }

      // We can not generate a preview if the values of the wallpapers differ.
      if (
        (this.ConfigWallpaperVM.HasImagePath != null) && 
        (this.ConfigWallpaperVM.IsMultiscreen != null) && 
        (this.ConfigWallpaperVM.Placement != null) &&
        (this.ConfigWallpaperVM.HorizontalOffset != null) && (this.ConfigWallpaperVM.VerticalOffset != null) &&
        (this.ConfigWallpaperVM.HorizontalScale != null) && (this.ConfigWallpaperVM.VerticalScale != null) &&
        (this.ConfigWallpaperVM.Effects != null) &&
        (this.ConfigWallpaperVM.BackgroundColor != null)
      ) {
        // Create a new Wallpaper from the image of the first given Wallpaper.
        Wallpaper tempWallpaper = new Wallpaper();
        tempWallpaper.ImagePath = imageFilePath;
        tempWallpaper.IsMultiscreen = this.ConfigWallpaperVM.IsMultiscreen.Value;
        tempWallpaper.Placement = this.ConfigWallpaperVM.Placement.Value;
        tempWallpaper.Offset = new Point(
          this.ConfigWallpaperVM.HorizontalOffset.Value, this.ConfigWallpaperVM.VerticalOffset.Value
        );
        tempWallpaper.Scale = new Point(
          this.ConfigWallpaperVM.HorizontalScale.Value, this.ConfigWallpaperVM.VerticalScale.Value
        );
        tempWallpaper.Effects = this.ConfigWallpaperVM.Effects.Value;
        tempWallpaper.BackgroundColor = this.ConfigWallpaperVM.BackgroundColor.Value;

        // Look if there is another worker already running.
        if (this.previewWorker != null && this.previewWorker.IsBusy) {
          this.previewWorker.CancelAsync();
        }

        // We use a Background Worker to create the preview in another thread making the GUI Thread not block.
        this.previewWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };

        this.previewWorker.DoWork += (senderLocal, eLocal) => {
          Object[] workerArgs = (Object[])eLocal.Argument;
          Wallpaper tempWallpaperLocal = (Wallpaper)workerArgs[0];
          Boolean isMultiscreenWallpaperLocal = (Boolean)workerArgs[1];

          Image previewImage = null;
          MemoryStream previewStream = null;
          try {
            // Draw the wallpaper using the All Cloned - Builder.
            WallpaperBuilderBase previewBuilder = new WallpaperBuilderAllCloned(this.ScreensSettings);
            if (isMultiscreenWallpaperLocal) {
              previewImage = previewBuilder.CreateMultiscreenFromSingle(tempWallpaperLocal, 0.2f, false);
            } else {
              Wallpaper[] wallpapersForScreen = new[] { tempWallpaperLocal };
              Wallpaper[][] wallpapersByScreen = new Wallpaper[this.ScreensSettings.Count][];

              for (Int32 i = 0; i < wallpapersByScreen.Length; i++) {
                wallpapersByScreen[i] = wallpapersForScreen;
              }

              previewImage = previewBuilder.CreateMultiscreenFromMultiple(wallpapersByScreen, 0.2f, false);
            }

            // Convert to ImageSource.
            using (previewStream = new MemoryStream()) {
              previewImage.Save(previewStream, ImageFormat.Bmp);
              
              eLocal.Result = BitmapDecoder.Create(previewStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
            }
          } finally {
            if (previewImage != null) {
              previewImage.Dispose();
            }

            if (previewStream != null) {
              previewStream.Dispose();
            }
          }
        };

        this.previewWorker.RunWorkerCompleted += (senderLocal, eLocal) => {
          try {
            if (eLocal.Error != null) {
              throw eLocal.Error;
            } else {
              if (!eLocal.Cancelled) {
                this.WallpaperPreview = (ImageSource)eLocal.Result;
              }
            }
          } finally {
            ((BackgroundWorker)senderLocal).Dispose();
          }
        };

        this.previewWorker.RunWorkerAsync(new Object[] { tempWallpaper, this.ConfigWallpaperVM.IsMultiscreen.Value });
      } else {
        if (this.Resources.Contains(ConfigWallpaperWindow.NoPreviewImageResourceKey)) {
          ImageSource noPreviewImage = (this.Resources[ConfigWallpaperWindow.NoPreviewImageResourceKey] as ImageSource);

          if (noPreviewImage != null) {
            this.WallpaperPreview = noPreviewImage;
          }
        }
      }
    }
    #endregion

    #region Command: Select Image Path
    /// <summary>
    ///   Contains the Select Image Path <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand SelectImagePathCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectImagePathCommand" />
    protected virtual void SelectImagePathCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = (
        this.ConfigWallpaperVM.ConfigurationMode != ConfigWallpaperMode.ConfigureDefaultSettings &&
        !this.ConfigWallpaperVM.ParentIsSynchronizedCategory
      );
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method saves the configuration files and closes the view.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectImagePathCommand" />
    protected virtual void SelectImagePathCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      using (OpenFileDialog fileDialog = new OpenFileDialog()) {
        fileDialog.Filter = MainWindow.WallpaperSelectionDialogFilter;
        fileDialog.CheckFileExists = true;

        if (fileDialog.ShowDialog() == FormsDialogResult.OK) {
          this.ConfigWallpaperVM.ImagePath = new Path(fileDialog.FileName);
          this.UpdatePreviewImage(null, null);
        }

        // For some reason the Window doesn't get the focus back if the open file dialog has been shown.
        this.Focus();
      }
    }
    #endregion

    #region Command: RemoveImagePath
    /// <summary>
    ///   The Remove Image Path <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand RemoveImagePathCommand = new RoutedCommand();
    
    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source object trying to call the command.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the command data.
    /// </param>
    /// <seealso cref="RemoveImagePathCommand">Remove Image Path Command</seealso>
    protected virtual void RemoveImagePathCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = this.ConfigWallpaperVM.RemoveImagePathCommand.CanExecute();
    }
    
    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This commands executes <see cref="WallpaperManager.ApplicationInterface.ConfigWallpaperVM.RemoveImagePathCommand" /> 
    ///   and updates the preview image.
    /// </summary>
    /// <param name="sender">
    ///   The source object which called the command.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the command data.
    /// </param>
    /// <seealso cref="RemoveImagePathCommand">Remove Image Path</seealso>
    protected virtual void RemoveImagePathCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      this.ConfigWallpaperVM.RemoveImagePathCommand.Execute();
      this.UpdatePreviewImage(null, null);
    }
    #endregion

    #region Command: Select Background Color
    /// <summary>
    ///   Contains the Select Background Color <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand SelectBackgroundColorCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectBackgroundColorCommand" />
    protected virtual void SelectBackgroundColorCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method saves the configuration files and closes the view.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectBackgroundColorCommand" />
    protected virtual void SelectBackgroundColorCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      using (ColorDialog colorDialog = new ColorDialog()) {
        colorDialog.AnyColor = true;
        colorDialog.AllowFullOpen = true;
        colorDialog.FullOpen = true;

        if (this.ConfigWallpaperVM.BackgroundColor != null) {
          colorDialog.Color = this.ConfigWallpaperVM.BackgroundColor.Value;
        }

        if (colorDialog.ShowDialog() == FormsDialogResult.OK) {
          this.ConfigWallpaperVM.BackgroundColor = colorDialog.Color;
          this.UpdatePreviewImage(null, null);
        }
        
        // For some reason the Window doesn't get the focus back if the dialog has been shown.
        this.Focus();
      }
    }
    #endregion
  }
}
