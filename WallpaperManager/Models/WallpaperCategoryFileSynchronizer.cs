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
  public class WallpaperCategoryFileSynchronizer : IDisposable {
    /// <summary>
    ///   Gets the file extensions for files which are suggested as wallpaper images.
    /// </summary>
    /// <value>
    ///   The file extensions for files which are suggested as wallpaper images.
    /// </value>
    protected static ReadOnlyCollection<string> WallpaperFileExtensions { get; } = new ReadOnlyCollection<string>(new[] {
      "jpg", "jpeg", "jpe", "jfif", "exif", "gif", "png", "tif", "tiff", "bmp", "dib"
    });

    public IWallpaperCategory WallpaperCategory { get; }

    /// <summary>
    ///   Gets the <see cref="Dispatcher" /> used to invoke operations in another thread.
    /// </summary>
    /// <value>
    ///   The <see cref="Dispatcher" /> used to invoke operations in another thread.
    /// </value>
    public IDispatcher InvokeDispatcher { get; }

    /// <summary>
    ///   Gets the directory being watched.
    /// </summary>
    /// <value>
    ///   The directory being watched.
    /// </value>
    public DirectoryInfo Directory { get; }

    /// <summary>
    ///   Gets the <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </summary>
    /// <value>
    ///   The <see cref="FileSystemWatcher" /> instance used to observe the file system's directory.
    /// </value>
    /// <seealso cref="System.IO.FileSystemWatcher">FileSystemWatcher Class</seealso>
    protected FileSystemWatcher FileSystemWatcher { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperCategoryFileSynchronizer" /> class with the given
    ///   <see cref="IWallpaperCategory" />.
    /// </summary>
    /// <inheritdoc cref="WallpaperCategory(string, IEnumerable{IWallpaper})"
    ///   select='param[@name="name"]' />
    /// <param name="directory">
    ///   The path of the directory to be watched.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="name" /> or <paramref name="directory" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///   The directory where <paramref name="directory" /> is refering to doesn't exist.
    /// </exception>
    /// <permission cref="SecurityAction.LinkDemand">
    ///   for full trust for the immediate caller. This member cannot be used by partially trusted code.
    /// </permission>
    /// <seealso cref="IWallpaperCategory">IWallpaperCategory Interface</seealso>
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="WallpaperCategoryFileSynchronizer" /> class.
    ///   </summary>
    ///   <seealso cref="IWallpaperCategory">IWallpaperCategory Interface</seealso>
    /// </overloads>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public WallpaperCategoryFileSynchronizer(IWallpaperCategory wallpaperCategory, DirectoryInfo directory, IDispatcher invokeDispatcher) {
      Contract.Requires<ArgumentNullException>(wallpaperCategory != null);
      Contract.Requires<ArgumentException>(directory != null);
      Contract.Requires<DirectoryNotFoundException>(directory.Exists);
      Contract.Requires<ArgumentNullException>(invokeDispatcher != null);

      this.WallpaperCategory = wallpaperCategory;
      this.Directory = directory;
      this.InvokeDispatcher = invokeDispatcher;

      this.FileSystemWatcher = this.CreateFileSystemWatcher();
      this.FileSystemWatcher.BeginInit();
      this.FileSystemWatcher.Path = directory.FullName;
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

    protected virtual FileSystemWatcher CreateFileSystemWatcher() {
      return new FileSystemWatcher();
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(WallpaperCategoryFileSynchronizer.WallpaperFileExtensions != null);
      Contract.Invariant(this.InvokeDispatcher != null);
      Contract.Invariant(this.Directory != null);
      Contract.Invariant(this.FileSystemWatcher != null);
    }

    /// <summary>
    ///   Forces a resynchronization of the containing <see cref="Wallpaper" /> objects with the physical image files on the
    ///   users hard disk.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void Resynchronize() {
      Contract.Requires<ObjectDisposedException>(!this.IsDisposed);

      FileInfo[] files = this.Directory.GetFiles();
      HashSet<string> filePaths = new HashSet<string>(files.Select((file) => file.FullName));

      // Add wallpapers not in the category yet.
      foreach (FileInfo file in files) {
        Path filePath = new Path(file.FullName);

        if (WallpaperCategoryFileSynchronizer.IsImageFileExtensionSupported(filePath))
          this.AddIfNotExist(filePath);
      }
  
      // Remove wallpapers which are in this category but don't exist on the hard disk.
      foreach (IWallpaper wallpaper in this.WallpaperCategory.Wallpapers)
        if (!filePaths.Contains(wallpaper.ImagePath))
          this.WallpaperCategory.Wallpapers.Remove(wallpaper);
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Changed" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
      Path filePath = new Path(e.FullPath);
      if (!IsImageFileExtensionSupported(filePath)) 
        return;
      
      this.InvokeDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.AddIfNotExist(filePath)));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Deleted" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e) {
      Path filePath = new Path(e.FullPath);

      this.InvokeDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
        IWallpaper existingWallpaper = this.WallpaperCategory.Wallpapers.FirstOrDefault(wp => wp.ImagePath == filePath);
        if (existingWallpaper != null)
          this.WallpaperCategory.Wallpapers.Remove(existingWallpaper);
      }));
    }

    /// <summary>
    ///   Handles the <see cref="System.IO.FileSystemWatcher.Renamed" /> event of a <see cref="System.IO.FileSystemWatcher" />
    ///   object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e) {
      if (e.OldFullPath == e.FullPath)
        return;

      Path oldFilePath = new Path(e.OldFullPath);
      Path newFilePath = new Path(e.FullPath);

      this.InvokeDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
        IWallpaper existingWallpaper = this.WallpaperCategory.Wallpapers.FirstOrDefault(wp => wp.ImagePath == oldFilePath);
        if (existingWallpaper != null)
          existingWallpaper.ImagePath = newFilePath;
        else
          this.WallpaperCategory.Wallpapers.Add(new Wallpaper(newFilePath));
      }));
    }

    private void AddIfNotExist(Path filePath) {
      if (!this.WallpaperCategory.Wallpapers.Any(wp => wp.ImagePath == filePath))
        this.WallpaperCategory.Wallpapers.Add(new Wallpaper(filePath));
    }

    // TODO: extract into another component
    /*/// <summary>
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
      Contract.Requires<ArgumentException>(imagePath != Path.Invalid);

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
    }*/

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
      Contract.Requires<ArgumentException>(filePath != Path.Invalid);

      string fileNameString = filePath.FileName;
      if (!fileNameString.Contains('.'))
        return false;

      return WallpaperCategoryFileSynchronizer.WallpaperFileExtensions.Contains(
        fileNameString.Substring(fileNameString.IndexOf('.') + 1).ToLower(CultureInfo.CurrentCulture));
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
    ///   Finalizes an instance of the <see cref="WallpaperCategoryFileSynchronizer" /> class.
    /// </summary>
    ~WallpaperCategoryFileSynchronizer() {
      this.Dispose(false);
    }
    #endregion
  }
}