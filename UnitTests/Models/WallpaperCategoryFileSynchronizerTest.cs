using System;
using System.IO;
using System.IO.Fakes;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Moq;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;
using Path = Common.IO.Path;

namespace UnitTests.Models {
  public class WallpaperCategoryFileSynchronizerTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void ShouldAddNewWallpaperFiles() {
      using (ShimsContext.Create()) {
        var wallpaperCategory = this.modelFixtures.Create<IWallpaperCategory>();
        var directoryShim = new ShimDirectoryInfo {
          GetFiles = () => new FileInfo[0],
          ExistsGet = () => true
        };
        var dispatcher = new DispatcherFake();
        var sut = new WallpaperCategoryFileSynchronizerFake(wallpaperCategory, directoryShim.Instance, dispatcher);
        var fileToAdd = new Path("C:\\Wallpaper.png");

        sut.FakeAddFile(fileToAdd);

        dispatcher.BeginInvokeCount.Should().Be(1);
        wallpaperCategory.Count.Should().Be(1);
        wallpaperCategory.Wallpapers[0].ImagePath.Should().Be(fileToAdd);
      }
    }
  }
}
