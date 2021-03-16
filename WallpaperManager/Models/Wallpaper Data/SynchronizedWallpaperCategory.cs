// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Windows.Threading;
using Common;
using WallpaperManager.Views;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  /// <summary>
  ///   An extended <see cref="WallpaperCategory" /> with the additional feature to watch a given directory of the file
  ///   system
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
  public class SynchronizedWallpaperCategory : WallpaperCategory, IDisposable {
    /// <summary>
    ///   Gets the file extensions for files which are suggested as wallpaper images.
    /// </summary>
    /// <value>
    ///   The file extensions for files which are suggested as wallpaper images.
    /// </value>
    protected static ReadOnlyCollection<string> WallpaperFileExtensions { get; } = new ReadOnlyCollection<string>(new[] {
      "jpg", "jpeg", "jpe", "jfif", "exif", "gif", "png", "tif", "tiff", "bmp", "dib"
    });

    /// <summary>
    ///   Gets the <see cref="Dispatcher" /> used to invoke operations finished by another thread.
    /// </summary>
    /// <value>
    ///   The <see cref="Dispatcher" /> used to invoke operations finished by another thread.
    /// </value>
    public Dispatcher InvokeDispatcher { get; }

    /// <summary>
    ///   Gets the path of the directory being watched.
    /// </summary>
    /// <value>
    ///   The path of the directory being watched.
    /// </value>
    public Path SynchronizedDirectoryPath { get; }

    /// <summary>
    ///   Gets the <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </summary>
    /// <value>
    ///   The <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </value>
    /// <seealso cref="System.IO.FileSystemWatcher">FileSystemWatcher Class</seealso>
    protected FileSystemWatcher FileSystemWatcher { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class with the given
    ///   <see cref="Wallpaper" /> objects added natively.
    /// </summary>
    /// <inheritdoc cref="WallpaperCategory(string, System.Collections.Generic.IEnumerable{WallpaperManager.Models.Wallpaper})"
    ///   select='param[@name="name"]' />
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
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class.
    ///   </summary>
    ///   <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// </overloads>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public SynchronizedWallpaperCategory(string name, Path synchronizedDirectoryPath, IEnumerable<Wallpaper> wallpapers) : base(name) {
      if (!Directory.Exists(synchronizedDirectoryPath)) throw new DirectoryNotFoundException();

      if (wallpapers != null) {
        foreach (Wallpaper wallpaper in wallpapers)
          base.InsertItem(this.Count, wallpaper);
      }

      this.SynchronizedDirectoryPath = synchronizedDirectoryPath;

      // TODO: Crosslayer-Access, try to provide the application's dispatcher via the constructor somehow.
      this.InvokeDispatcher = Application.Current.Dispatcher;

      this.FileSystemWatcher = new FileSystemWatcher();
      this.FileSystemWatcher.BeginInit();
      this.FileSystemWatcher.Path = synchronizedDirectoryPath;
      this.FileSystemWatcher.IncludeSubdirectories = false;
      this.FileSystemWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName);
      this.FileSystemWatcher.EnableRaisingEvents = true;
      this.FileSystemWatcher.Deleted += this.FileSystemWatcher_Deleted;
      this.FileSystemWatcher.Created += this.FileSystemWatcher_Changed;
      this.FileSystemWatcher.Changed += this.FileSystemWatcher_Changed;
      this.FileSystemWatcher.Renamed += this.FileSystemWatcher_Renamed;
      this.FileSystemWatcher.EndInit();

      this.Resynchronize();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="SynchronizedWallpaperCategory" /> class.
    /// </summary>
    /// <inheritdoc cref="SynchronizedWallpaperCategory(string, Path, IEnumerable{Wallpaper})" />
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public SynchronizedWallpaperCategory(string name, Path synchronizedDirectoryPath) :
      this(name, synchronizedDirectoryPath, null) {}

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(SynchronizedWallpaperCategory.WallpaperFileExtensions != null);
      Contract.Invariant(this.InvokeDispatcher != null);
      Contract.Invariant(this.SynchronizedDirectoryPath != Path.None);
      Contract.Invariant(this.FileSystemWatcher != null);
    }

    /// <summary>
    ///   Forces a resynchronization of the containing <see cref="Wallpaper" /> objects with the physical image files on the
    ///   users hard disk.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void Resynchronize() {
      if (this.IsDisposed) throw new ObjectDisposedException("this");

      string[] existingFiles = Directory.GetFiles(this.SynchronizedDirectoryPath);

      // Add wallpapers that are on the hard disk but not in this category.
      for (int i = 0; i < existingFiles.Length; i++) {
        Path existingPath = new Path(existingFiles[i]);

        if (this.IndexOfByImagePath(existingPath) == -1) {
          if (SynchronizedWallpaperCategory.IsImageFileExtensionSupported(existingPath))
            this.AddAsync(existingPath);
        }
      }

      // Remove wallpapers which are in this category but doesn't exist on the hard disk.
      for (int i = this.Count - 1; i >= 0; i--) {
        if ((this[i].ImagePath == Path.None) || (!existingFiles.Contains(this[i].ImagePath)))
          base.RemoveItem(i);
      }
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Changed" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
      Action<Path> code = (filePath) => {
        // Make sure the item doesn't already exist.
        if (this.IndexOfByImagePath(filePath) == -1 && SynchronizedWallpaperCategory.IsImageFileExtensionSupported(filePath))
          this.AddAsync(filePath);
      };

      this.InvokeDispatcher.Invoke(DispatcherPriority.Background, code, new Path(e.FullPath));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Deleted" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e) {
      Action<Path> code = delegate(Path filePath) {
        int wallpaperIndex = this.IndexOfByImagePath(filePath);

        if (wallpaperIndex != -1)
          base.RemoveItem(wallpaperIndex);
      };

      this.InvokeDispatcher.Invoke(DispatcherPriority.Background, code, new Path(e.FullPath));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Renamed" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e) {
      if (e.OldFullPath != e.FullPath) {
        Action<Path, Path> code = (oldPath, newPath) => {
          int wallpaperIndex = this.IndexOfByImagePath(oldPath);

          if (wallpaperIndex != -1)
            this[wallpaperIndex].ImagePath = newPath;
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
    ///   A <see cref="bool" /> indicating whether the file extension of the specified <paramref name="filePath" /> is a
    ///   supported image format or not.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="filePath" /> is <c>null</c>.
    /// </exception>
    private static bool IsImageFileExtensionSupported(Path filePath) {
      if (filePath == Path.None) throw new ArgumentException();

      string fileNameString = filePath.FileName;
      if (!fileNameString.Contains('.'))
        return false;

      return SynchronizedWallpaperCategory.WallpaperFileExtensions.Contains(
        fileNameString.Substring(fileNameString.IndexOf('.') + 1).ToLower(CultureInfo.CurrentCulture));
    }

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
      if (imagePath == Path.None) throw new ArgumentException();

      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += (sender, e) => {
        Path imagePathLocal = (Path)e.Argument;
        Wallpaper newWallpaper = new Wallpaper(imagePathLocal);

        using (Image wallpaperImage = Image.FromFile(imagePathLocal))
          newWallpaper.ImageSize = wallpaperImage.Size;

        e.Result = newWallpaper;
      };

      worker.RunWorkerCompleted += (sender, e) => {
        if (e.Error != null) {
          // We simply ignore if the file couldn't be found or is invalid / unsupported.
          if ((e.Error is FileNotFoundException) || (e.Error is OutOfMemoryException))
            return;

          throw e.Error;
        }

        if (!e.Cancelled) {
          Wallpaper newWallpaperLocal = (Wallpaper)e.Result;

          // Make sure the item wasn't already added by another thread.
          if (this.IndexOfByImagePath(newWallpaperLocal.ImagePath) == -1)
            base.InsertItem(this.Count, newWallpaperLocal);
        }

        ((BackgroundWorker)sender).Dispose();
      };

      // By using BackgroundWorker we make sure this operation is thread safe.
      worker.RunWorkerAsync(imagePath);
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, Wallpaper item) {
      // TODO: Throwing this exception is not allowed here.
      throw new InvalidOperationException("Category is read only.");
    }

    /// <inheritdoc />
    protected override void SetItem(int index, Wallpaper item) {
      // TODO: Throwing this exception is not allowed here.
      throw new InvalidOperationException("Category is read only.");
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index) {
      // TODO: Throwing this exception is not allowed here.
      throw new InvalidOperationException("Category is read only.");
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      // TODO: Throwing this exception is not allowed here.
      throw new InvalidOperationException("Category is read only.");
    }

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    public bool IsDisposed { get; private set; }

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(bool disposing) {
      if (!this.IsDisposed) {
        if (disposing)
          this.FileSystemWatcher?.Dispose();

        this.IsDisposed = true;
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