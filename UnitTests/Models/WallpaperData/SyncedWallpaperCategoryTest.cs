using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO.Fakes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models.WallpaperData {
  class SyncedWallpaperCategoryTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldThrowOnInvalidPath() {
      Action construct = () => new SyncedWallpaperCategory(null, this.modelFixtures.Create<WallpaperDefaultSettings>(), Path.Invalid);

      construct.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ShouldReportErrorWhenDirectoryDoesntExist() {
      Path directoryToSync = new Path("C:\\ThisFolderMustNotPhysicallyExist");

      using (ShimsContext.Create()) {
        SyncedWallpaperCategory sut = new SyncedWallpaperCategory(this.modelFixtures.Create<string>(), this.modelFixtures.Create<WallpaperDefaultSettings>(), directoryToSync);

        ShimDirectory.ExistsString = directoryPath => false;

        string expectedErrorMsg = string.Format(LocalizationManager.GetLocalizedString("Error.Path.DirectoryNotFound"), directoryToSync);
        sut[nameof(sut.DirectoryPath)].Should().Be(expectedErrorMsg);
        sut.Error.Should().NotBeNullOrEmpty();
      }
    }

    [Fact]
    public void ShouldNotReportErrorWhenDirectoryDoesExist() {
      Path directoryToSync = new Path("C:\\ThisFolderMustNotPhysicallyExist");

      using (ShimsContext.Create()) {
        SyncedWallpaperCategory sut = new SyncedWallpaperCategory(this.modelFixtures.Create<string>(), this.modelFixtures.Create<WallpaperDefaultSettings>(), directoryToSync);

        ShimDirectory.ExistsString = directoryPath => false;

        sut[nameof(sut.DirectoryPath)].Should().BeNullOrEmpty();
        sut.Error.Should().BeNullOrEmpty();
      }
    }
  }
}
