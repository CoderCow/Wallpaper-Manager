// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Path = Common.IO.Path;
using Size = System.Drawing.Size;

using WallpaperManager.Data;

namespace WallpaperManager.ApplicationInterface {
  /// <commondoc select='WrappingViewModels/General/*' params="WrappedType=Wallpaper" />
  /// <threadsafety static="true" instance="false" />
  public class WallpaperVM: IWeakEventListener, IDataErrorInfo, INotifyPropertyChanged {
    #region Constants: ThumbnailWidth, ThumbnailHeight
    /// <summary>
    ///   Represents the width value of the generated thumbnail image.
    /// </summary>
    /// <remarks>
    ///   Increasing this value will result in much more memory costs.
    /// </remarks>
    private const Int32 ThumbnailWidth = 150;
    /// <summary>
    ///   Represents the height value of the generated thumbnail image.
    /// </summary>
    /// <inheritdoc cref="ThumbnailWidth" />
    private const Int32 ThumbnailHeight = 100;
    #endregion

    #region Fields: isGettingThumbnail, getThumbailError
    /// <summary>
    ///   Indicates whether a process in another Thread is actually generating a thumbnail image or not.
    /// </summary>
    private Boolean isGettingThumbnail;

    /// <summary>
    ///   The last occured <see cref="Exception" /> instance which was thrown when generating the thumbnail.
    /// </summary>
    private Exception getThumbailError;
    #endregion

    #region Property: Wallpaper
    /// <summary>
    ///   <inheritdoc cref="Wallpaper" select='../value/node()' />
    /// </summary>
    private readonly Wallpaper wallpaper;

    /// <summary>
    ///   Gets the wrapped <see cref="WallpaperManager.Data.Wallpaper" /> instance.
    /// </summary>
    /// <value>
    ///   The wrapped <see cref="WallpaperManager.Data.Wallpaper" /> instance.
    /// </value>
    public Wallpaper Wallpaper {
      get { return this.wallpaper; }
    }
    #endregion

    #region Property: Thumbnail
    /// <summary>
    ///   <inheritdoc cref="Thumbnail" select='../value/node()' />
    /// </summary>
    private ImageSource thumbnail;

    /// <summary>
    ///   Gets or sets the thumbnail image source.
    /// </summary>
    /// <remarks>
    ///   When getting this value and no thumbnail is actually available, this property will call <see cref="GetThumbnailAsync" />.
    /// </remarks>
    /// <value>
    ///   The thumbnail image source.
    /// </value>
    public ImageSource Thumbnail {
      get {
        if ((this.thumbnail == null) && (!this.isGettingThumbnail)) {
          try {
            this.GetThumbnailAsync();
          } catch (Exception exception) {
            this.getThumbailError = exception;
          }
        }

        return this.thumbnail;
      }
      set {
        this.thumbnail = value;
        this.OnPropertyChanged("Thumbnail");
      }
    }
    #endregion

    #region Property: IsApplied
    /// <summary>
    ///   <inheritdoc cref="IsApplied" select='../value/node()' />
    /// </summary>
    private Boolean isApplied;

    /// <summary>
    ///   Gets or sets a value indicating whether the wrapped <see cref="WallpaperManager.Data.Wallpaper" /> 
    ///   instance is actually applied on the users Desktop.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the wrapped <see cref="WallpaperManager.Data.Wallpaper" /> instance is actually applied 
    ///   on the users Desktop; otherwise, <c>false</c>.
    /// </value>
    public Boolean IsApplied {
      get { return this.isApplied; }
      set {
        this.isApplied = value;
        this.OnPropertyChanged("IsApplied");
      }
    }
    #endregion

    #region IDataErrorInfo Implementation
    /// <inheritdoc cref="IDataErrorInfo.Error" />
    public String Error {
      get { return String.Empty; }
    }

    /// <inheritdoc cref="IDataErrorInfo.this" />
    public String this[String columnName] {
      get {
        if (columnName == "Thumbnail") {
          if (this.getThumbailError != null) {
            return this.getThumbailError.Message;
          }
        }

        return null;
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperVM" /> class.
    /// </summary>
    /// <param name="wallpaper">
    ///   The <see cref="WallpaperManager.Data.Wallpaper" /> which should be wrapped by this View Model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpaper" /> is <c>null</c>.
    /// </exception>
    /// <commondoc select='ViewModels/General/seealso' />
    public WallpaperVM(Wallpaper wallpaper) {
      if (wallpaper == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaper"));
      }
      
      this.wallpaper = wallpaper;
      // Make sure to keep the thumbnail image up to date.
      PropertyChangedEventManager.AddListener(this.Wallpaper, this, "ImagePath");
    }
    #endregion

    #region Methods: GetThumbnailInternal, GetThumbnailAsync
    /// <summary>
    ///   Generates a thumbnail image of this given source image and sets the <see cref="Thumbnail" /> 
    ///   property.
    /// </summary>
    /// <param name="source">
    ///   The source image where the thumbnail should be generated of.
    /// </param>
    /// <remarks>
    ///   <note type="implementnotes">
    ///     This method is also called by another thread, watch for thread safety here.
    ///   </note>
    /// </remarks>
    /// <returns>
    ///   A freezed <see cref="ImageSource" /> object, containg the thumbnail image.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="source" /> is <c>null</c>.
    /// </exception>
    protected virtual ImageSource GetThumbnailInternal(Image source) {
      if (source == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("source"));
      }
      
      ImageSource thumbnail;

      // Using WPF classes instead of GDI+ to create the thumbnail would be better because it wouldn't
      // require additional conversion to an ImageSource. However when using the TransformedBitmap WPF class
      // to resize the image it gets attached with an Dispatcher and is that way locked to the Background-
      // Worker thread which maybe calls this method, its also slower than the GDI+ GetThumbnailImage method.
      using (MemoryStream imageStream = new MemoryStream()) {
        source.GetThumbnailImage(
        WallpaperVM.ThumbnailWidth, WallpaperVM.ThumbnailHeight,
        null,
        new IntPtr()
      ).Save(imageStream, ImageFormat.Bmp);

        thumbnail = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
      }
      
      if (thumbnail.CanFreeze) {
        thumbnail.Freeze();
      }

      return thumbnail;
    }

    /// <summary>
    ///   Generates a thumbnail image of the <see cref="WallpaperManager.Data.Wallpaper.ImagePath" /> file 
    ///   asynchronous and sets the <see cref="Thumbnail" /> property.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    ///   the file <see cref="WallpaperManager.Data.Wallpaper.ImagePath" /> could not be found.
    /// </exception>
    public void GetThumbnailAsync() {
      if (this.Wallpaper.ImagePath == Path.None) {
        return;
      }
      if (!File.Exists(this.Wallpaper.ImagePath)) {
        throw new FileNotFoundException(ExceptionMessages.GetFileNotFound(this.Wallpaper.ImagePath));
      }

      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerSupportsCancellation = false;

      worker.DoWork += delegate (Object sender, DoWorkEventArgs e) {
        using (Image wallpaperImage = Image.FromFile(e.Argument.ToString())) {
          e.Result = new Object[] { this.GetThumbnailInternal(wallpaperImage), wallpaperImage.Size };
        }
      };
      
      worker.RunWorkerCompleted += delegate (Object sender, RunWorkerCompletedEventArgs e) {
        try {
          if (e.Error != null) {
            this.getThumbailError = e.Error;
          }

          if (!e.Cancelled) {
            Object[] results = (Object[])e.Result;

            this.Thumbnail = (ImageSource)results[0];
            this.Wallpaper.ImageSize = (Size)results[1];
          }
        } finally {
          this.isGettingThumbnail = false;
        }
      };

      this.isGettingThumbnail = true;

      // We use another Thread to get the thumbnail.
      worker.RunWorkerAsync(this.Wallpaper.ImagePath);
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        if (!this.isGettingThumbnail) {
          if (this.Wallpaper.ImagePath != Path.None) {
            this.GetThumbnailAsync();
          } else {
            this.thumbnail = null;
            this.OnPropertyChanged("Thumbnail");
          }
        }

        return true;
      }

      return false;
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
  }
}
