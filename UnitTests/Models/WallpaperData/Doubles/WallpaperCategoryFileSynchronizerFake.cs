using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.Fakes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Moq;
using WallpaperManager.Models;
using Path = Common.IO.Path;

namespace UnitTests.Models {
  public class WallpaperCategoryFileSynchronizerFake : WallpaperCategoryFileSynchronizer {
    private Mock<FileSystemWatcher> watcherMock;

    public WallpaperCategoryFileSynchronizerFake(IWallpaperCategory wallpaperCategory, DirectoryInfo directory, IDispatcher invokeDispatcher) : 
      base(wallpaperCategory, directory, invokeDispatcher) {}

    protected override FileSystemWatcher CreateFileSystemWatcher() {
      this.watcherMock = new ShimFileSystemWatcher();

      return this.watcherMock.Object;
    }

    public void FakeAddFile(Path filePath) {
      var args = new FileSystemEventArgs(WatcherChangeTypes.Created, filePath, filePath.FileName);

      this.watcherMock.Raise((x) => x.Created += null, args);
      this.watcherMock.Raise((x) => x.Changed += null, args);
    }
  }
}
