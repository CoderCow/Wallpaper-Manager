// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WallpaperManager.Models;
using Path = Common.IO.Path;
using Size = System.Drawing.Size;

namespace WallpaperManager.ViewModels {
  /// <commondoc select='WrappingViewModels/General/*' params="WrappedType=Wallpaper" />
  /// <threadsafety static="true" instance="false" />
  public class WallpaperVM : IWeakEventListener, IDataErrorInfo, INotifyPropertyChanged {
    /// <summary>
    ///   Represents the width value of the generated thumbnail image.
    /// </summary>
    /// <remarks>
    ///   Increasing this value will result in much more memory costs.
    /// </remarks>
    private const int ThumbnailWidth = 150;

    /// <summary>
    ///   Represents the height value of the generated thumbnail image.
    /// </summary>
    /// <inheritdoc cref="ThumbnailWidth" />
    private const int ThumbnailHeight = 100;

    /// <summary>
    ///   The last occurred <see cref="Exception" /> instance which was thrown when generating the thumbnail.
    /// </summary>
    private Exception getThumbailError;

    /// <summary>
    ///   <inheritdoc cref="IsApplied" select='../value/node()' />
    /// </summary>
    private bool isApplied;

    /// <summary>
    ///   Indicates whether a process in another Thread is actually generating a thumbnail image or not.
    /// </summary>
    private bool isGettingThumbnail;

    /// <summary>
    ///   <inheritdoc cref="Thumbnail" select='../value/node()' />
    /// </summary>
    private ImageSource thumbnail;

    /// <summary>
    ///   Gets the wrapped <see cref="Models.Wallpaper" /> instance.
    /// </summary>
    /// <value>
    ///   The wrapped <see cref="Models.Wallpaper" /> instance.
    /// </value>
    public Wallpaper Wallpaper { get; }

    /// <summary>
    ///   Gets or sets the thumbnail image source.
    /// </summary>
    /// <remarks>
    ///   When getting this value and no thumbnail is actually available, this property will call
    ///   <see cref="GetThumbnailAsync" />.
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

    /// <summary>
    ///   Gets or sets a value indicating whether the wrapped <see cref="Models.Wallpaper" />
    ///   instance is actually applied on the users Desktop.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the wrapped <see cref="Models.Wallpaper" /> instance is actually applied
    ///   on the users Desktop; otherwise, <c>false</c>.
    /// </value>
    public bool IsApplied {
      get { return this.isApplied; }
      set {
        this.isApplied = value;
        this.OnPropertyChanged("IsApplied");
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperVM" /> class.
    /// </summary>
    /// <param name="wallpaper">
    ///   The <see cref="WallpaperManager.Models.Wallpaper" /> which should be wrapped by this View Model.
    /// </param>
    /// <commondoc select='ViewModels/General/seealso' />
    public WallpaperVM(Wallpaper wallpaper) {
      this.Wallpaper = wallpaper;
      // Make sure to keep the thumbnail image up to date.
      PropertyChangedEventManager.AddListener(this.Wallpaper, this, "ImagePath");
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        if (!this.isGettingThumbnail) {
          if (this.Wallpaper.ImagePath != Path.None)
            this.GetThumbnailAsync();
          else {
            this.thumbnail = null;
            this.OnPropertyChanged("Thumbnail");
          }
        }

        return true;
      }

      return false;
    }
    #endregion

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.Wallpaper != null);
    }

    /// <summary>
    ///   Generates a thumbnail image of the <see cref="WallpaperManager.Models.Wallpaper.ImagePath" /> file
    ///   asynchronous and sets the <see cref="Thumbnail" /> property.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    ///   the file <see cref="WallpaperManager.Models.Wallpaper.ImagePath" /> could not be found.
    /// </exception>
    public void GetThumbnailAsync() {
      if (this.Wallpaper.ImagePath == Path.None)
        return;
      if (!File.Exists(this.Wallpaper.ImagePath))
        throw new FileNotFoundException(this.Wallpaper.ImagePath);

      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerSupportsCancellation = false;

      worker.DoWork += delegate(object sender, DoWorkEventArgs e) {
        using (Image wallpaperImage = Image.FromFile(e.Argument.ToString()))
          e.Result = new object[] {this.GetThumbnailInternal(wallpaperImage), wallpaperImage.Size};
      };

      worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
        try {
          if (e.Error != null)
            this.getThumbailError = e.Error;

          if (!e.Cancelled) {
            var results = (object[])e.Result;

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
      Contract.Requires<ArgumentNullException>(source != null);

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

      if (thumbnail.CanFreeze)
        thumbnail.Freeze();

      return thumbnail;
    }

    #region IDataErrorInfo Implementation
    /// <inheritdoc cref="IDataErrorInfo.Error" />
    public string Error {
      get { return string.Empty; }
    }

    /// <inheritdoc cref="IDataErrorInfo.this" />
    public string this[string columnName] {
      get {
        if (columnName == "Thumbnail")
          return this.getThumbailError?.Message;

        return null;
      }
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}