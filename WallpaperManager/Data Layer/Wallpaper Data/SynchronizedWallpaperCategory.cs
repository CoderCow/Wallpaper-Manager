// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Threading;

using Path = Common.IO.Path;

namespace WallpaperManager.Data {
  /// <summary>
  ///   An extended <see cref="WallpaperCategory" /> with the additional feature to watch a given directory of the file system 
  ///   for changes and add or remove <see cref="Wallpaper" /> objects automatically.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This extended <see cref="WallpaperCategory" /> got the ability to watch a directory of the users file system for 
  ///     changes and add or remove a given <see cref="Wallpaper" /> if a new image has been added or removed from the file 
  ///     system's directory.
  ///   </para>
  ///   <para>
  ///     <inheritdoc cref="WallpaperCategory" select='../para/node()' />
  ///   </para>
  ///   <note type="implementnotes">
  ///     This collection is read only and will throws an <see cref="InvalidOperationException" /> for any altering operation 
  ///     except moving of items.
  ///   </note>
  /// </remarks>
  /// <seealso cref="System.IO.FileSystemWatcher">FileSystemWatcher Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class SynchronizedWallpaperCategory: WallpaperCategory, IDisposable {
    #region Property: InvokeDispatcher
    /// <summary>
    ///   <inheritdoc cref="InvokeDispatcher" select='../value/node()' />
    /// </summary>
    private readonly Dispatcher invokeDispatcher;

    /// <summary>
    ///   Gets the <see cref="Dispatcher" /> used to invoke operations finished by another thread.
    /// </summary>
    /// <value>
    ///   The <see cref="Dispatcher" /> used to invoke operations finished by another thread.
    /// </value>
    public Dispatcher InvokeDispatcher {
      get { return this.invokeDispatcher; }
    }
    #endregion

    #region Property: SynchronizedDirectoryPath
    /// <summary>
    ///   <inheritdoc cref="SynchronizedDirectoryPath" select='../value/node()' />
    /// </summary>
    private readonly Path synchronizedDirectoryPath;

    /// <summary>
    ///   Gets the path of the directory being watched.
    /// </summary>
    /// <value>
    ///   The path of the directory being watched.
    /// </value>
    public Path SynchronizedDirectoryPath {
      get { return this.synchronizedDirectoryPath; }
    }
    #endregion

    #region Property: WallpaperFileExtensions
    /// <summary>
    ///   <inheritdoc cref="WallpaperFileExtensions" select='../value/node()' />
    /// </summary>
    private static readonly String[] wallpaperFileExtensions = new[] {
      "jpg", "jpeg", "jpe", "jfif", "exif", "gif", "png", "tif", "tiff", "bmp", "dib"
    };

    /// <summary>
    ///   Gets the file extensions for files which are suggested as wallpaper images.
    /// </summary>
    /// <value>
    ///   The file extensions for files which are suggested as wallpaper images.
    /// </value>
    protected static ReadOnlyCollection<String> WallpaperFileExtensions {
      get { return new ReadOnlyCollection<String>(SynchronizedWallpaperCategory.wallpaperFileExtensions); }
    }
    #endregion

    #region Property: FileSystemWatcher
    /// <summary>
    ///   <inheritdoc cref="FileSystemWatcher" select='../value/node()' />
    /// </summary>
    private readonly FileSystemWatcher fileSystemWatcher;

    /// <summary>
    ///   Gets the <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </summary>
    /// <value>
    ///   The <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </value>
    /// <seealso cref="System.IO.FileSystemWatcher">FileSystemWatcher Class</seealso>
    protected FileSystemWatcher FileSystemWatcher {
      get { return this.fileSystemWatcher; }
    }
    #endregion


    #region Methods: Constructors, Resynchronize
    /// <summary>
    ///   Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class with the given 
    ///   <see cref="Wallpaper" /> objects added natively.
    /// </summary>
    /// <inheritdoc cref="WallpaperCategory(String, IEnumerable{Wallpaper})" select='param[@name="name"]' />
    /// <param name="synchronizedDirectoryPath">
    ///   The path of the directory to be watched.
    /// </param>
    /// <param name="wallpapers">
    ///   A collection of wallpapers which should be added to the category nativley. <c>null</c> to create an empty category.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="name" /> or <paramref name="synchronizedDirectoryPath" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///   The directory where <paramref name="synchronizedDirectoryPath" /> is refering to doesn't exist.
    /// </exception>
    /// <permission cref="SecurityAction.LinkDemand">
    ///   for full trust for the immediate caller. This member cannot be used by partially trusted code.
    /// </permission>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class.
    ///   </summary>
    ///   <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// </overloads>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public SynchronizedWallpaperCategory(String name, Path synchronizedDirectoryPath, IEnumerable<Wallpaper> wallpapers): 
      base(name) 
    {
      if (synchronizedDirectoryPath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone(synchronizedDirectoryPath));
      }
      if (!Directory.Exists(synchronizedDirectoryPath)) {
        throw new DirectoryNotFoundException(
          ExceptionMessages.GetDirectoryNotFound(synchronizedDirectoryPath, "synchronizedDirectoryPath")
        );
      }
      
      if (wallpapers != null) {
        foreach (Wallpaper wallpaper in wallpapers) {
          base.InsertItem(this.Count, wallpaper);
        }
      }

      this.synchronizedDirectoryPath = synchronizedDirectoryPath;

      // TODO: Crosslayer-Access, try to provide the application's dispatcher via the constructor somehow.
      this.invokeDispatcher = WallpaperManager.Presentation.Application.Current.Dispatcher;
      
      this.fileSystemWatcher = new FileSystemWatcher();
      this.fileSystemWatcher.BeginInit();
      this.fileSystemWatcher.Path = synchronizedDirectoryPath;
      this.fileSystemWatcher.IncludeSubdirectories = false;
      this.fileSystemWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName);
      this.fileSystemWatcher.EnableRaisingEvents = true;
      this.fileSystemWatcher.Deleted += this.FileSystemWatcher_Deleted;
      this.fileSystemWatcher.Created += this.FileSystemWatcher_Changed;
      this.fileSystemWatcher.Changed += this.FileSystemWatcher_Changed;
      this.fileSystemWatcher.Renamed += this.FileSystemWatcher_Renamed;
      this.fileSystemWatcher.EndInit();

      this.Resynchronize();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class.
    /// </summary>
    /// <inheritdoc cref="SynchronizedWallpaperCategory(String, Path, IEnumerable{Wallpaper})" />
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public SynchronizedWallpaperCategory(String name, Path synchronizedDirectoryPath): 
      this(name, synchronizedDirectoryPath, null) {}

    /// <summary>
    ///   Forces a resynchronization of the containing <see cref="Wallpaper" /> objects with the physical image files on the 
    ///   users hard disk.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void Resynchronize() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }

      String[] existingFiles = Directory.GetFiles(this.SynchronizedDirectoryPath);

      // Add wallpapers that are on the hard disk but not in this category.
      for (Int32 i = 0; i < existingFiles.Length; i++) {
        Path existingPath = new Path(existingFiles[i]);

        if (this.IndexOfByImagePath(existingPath) == -1) {
          if (SynchronizedWallpaperCategory.IsImageFileExtensionSupported(existingPath)) {
            this.AddAsync(existingPath);
          }
        }
      }

      // Remove wallpapers which are in this category but doesn't exist on the hard disk.
      for (Int32 i = this.Count - 1; i >= 0; i--) {
        if ((this[i].ImagePath == Path.None) || (!existingFiles.Contains(this[i].ImagePath))) {
          base.RemoveItem(i);
        }
      }
    }
    #endregion

    #region Methods: FileSystemWatcher_Changed, FileSystemWatcher_Deleted, FileSystemWatcher_Renamed, IsImageFileExtensionSupported
    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Changed" /> event of a <see cref="System.IO.FileSystemWatcher" /> 
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Changed(Object sender, FileSystemEventArgs e) {
      Action<Path> code = delegate(Path filePath) {
        // Make sure the item doesn't already exist.
        if (this.IndexOfByImagePath(filePath) == -1) {
          if (SynchronizedWallpaperCategory.IsImageFileExtensionSupported(filePath)) {
            this.AddAsync(filePath);
          }
        }
      };

      this.InvokeDispatcher.Invoke(DispatcherPriority.Background, code, new Path(e.FullPath));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Deleted" /> event of a <see cref="System.IO.FileSystemWatcher" /> 
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Deleted(Object sender, FileSystemEventArgs e) {
      Action<Path> code = delegate(Path filePath) {
        Int32 wallpaperIndex = this.IndexOfByImagePath(filePath);

        if (wallpaperIndex != -1) {
          base.RemoveItem(wallpaperIndex);
        }
      };

      this.InvokeDispatcher.Invoke(DispatcherPriority.Background, code, new Path(e.FullPath));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Renamed" /> event of a <see cref="System.IO.FileSystemWatcher" /> 
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Renamed(Object sender, RenamedEventArgs e) {
      if (e.OldFullPath != e.FullPath) {
        Action<Path, Path> code = (oldPath, newPath) => {
          Int32 wallpaperIndex = this.IndexOfByImagePath(oldPath);

          if (wallpaperIndex != -1) {
            this[wallpaperIndex].ImagePath = newPath;
          }
        };

        this.InvokeDispatcher.Invoke(DispatcherPriority.Background, code, new Path(e.OldFullPath), new Path(e.FullPath));
      }
    }

    /// <summary>
    ///   Determines whether the file extension of the given file path is a supported image format.
    /// </summary>
    /// <param name="filePath">
    ///   The path of the file.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the file extension of the specified <paramref name="filePath" /> is a 
    ///   supported image format or not.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="filePath" /> is <c>null</c>.
    /// </exception>
    private static Boolean IsImageFileExtensionSupported(Path filePath) {
      if (filePath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone("filePath"));
      }

      String fileNameString = filePath.FileName;
      if (!fileNameString.Contains('.')) {
        return false;
      }

      return SynchronizedWallpaperCategory.wallpaperFileExtensions.Contains(
        fileNameString.Substring(fileNameString.IndexOf('.') + 1).ToLower(CultureInfo.CurrentCulture)
      );
    }
    #endregion

    #region Methods: AddAsync, InsertItem, SetItem, RemoveItem, ClearItems
    /// <summary>
    ///   Creates a new <see cref="Wallpaper" /> instance and adds it to the collection.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the wallpaper image.
    /// </param>
    /// <remarks>
    ///   <para>
    ///     The wallpaper is created asynchronously by using a <see cref="BackgroundWorker" />. If two wallpapers refering to 
    ///     the same image paths are added, one of them will be automatically ignored.
    ///   </para>
    ///   <para>
    ///     If the file where <paramref name="imagePath" /> is refering to is not found, no exception will be thrown.
    ///   </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="imagePath" /> is <c>null</c>.
    /// </exception>
    private void AddAsync(Path imagePath) {
      if (imagePath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone("imagePath"));
      }

      BackgroundWorker worker = new BackgroundWorker();

      worker.DoWork += delegate (Object sender, DoWorkEventArgs e) {
        Path imagePathLocal = (Path)e.Argument;
        Wallpaper newWallpaper = new Wallpaper(imagePathLocal);
        
        using (Image wallpaperImage = Image.FromFile(imagePathLocal)) {
          newWallpaper.ImageSize = wallpaperImage.Size;
        }

        e.Result = newWallpaper;
      };

      worker.RunWorkerCompleted += delegate (Object sender, RunWorkerCompletedEventArgs e) {
        if (e.Error != null) {
          // We simply ignore if the file couldn't be found or is invalid / unsupported.
          if ((e.Error is FileNotFoundException) || (e.Error is OutOfMemoryException)) {
            return;
          }

          throw e.Error;
        } 
        
        if (!e.Cancelled) {
          Wallpaper newWallpaperLocal = (Wallpaper)e.Result;

          // Make sure the item wasn't already added by another thread.
          if (this.IndexOfByImagePath(newWallpaperLocal.ImagePath) == -1) {
            base.InsertItem(this.Count, newWallpaperLocal);
          }
        }

        ((BackgroundWorker)sender).Dispose();
      };

      // By using BackgroundWorker we make sure this operation is thread safe.
      worker.RunWorkerAsync(imagePath);
    }

    /// <inheritdoc />
    protected override void InsertItem(Int32 index, Wallpaper item) {
      throw new InvalidOperationException(ExceptionMessages.GetCategoryIsReadOnly());
    }

    /// <inheritdoc />
    protected override void SetItem(Int32 index, Wallpaper item) {
      throw new InvalidOperationException(ExceptionMessages.GetCategoryIsReadOnly());
    }

    /// <inheritdoc />
    protected override void RemoveItem(Int32 index) {
      throw new InvalidOperationException(ExceptionMessages.GetCategoryIsReadOnly());
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      throw new InvalidOperationException(ExceptionMessages.GetCategoryIsReadOnly());
    }
    #endregion

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    private Boolean isDisposed;

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          if (this.fileSystemWatcher != null) {
            this.fileSystemWatcher.Dispose();
          }
        }

        this.isDisposed = true;
      }
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="SynchronizedWallpaperCategory" /> class.
    /// </summary>
    ~SynchronizedWallpaperCategory() {
      this.Dispose(false);
    }
    #endregion
  }
}
