using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO.Fakes;
using Common.IO;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class SyncedWallpaperCategoryTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void ShouldReportErrorWhenDirectoryIsInvalid() {
      Path directoryToSync = Path.Invalid;
      SyncedWallpaperCategory sut = new SyncedWallpaperCategory(this.modelFixtures.Create<string>(), this.modelFixtures.Create<WallpaperDefaultSettings>(), directoryToSync);

      sut.DirectoryPath = directoryToSync;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.FieldIsInvalid");
      sut[nameof(sut.DirectoryPath)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
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

        ShimDirectory.ExistsString = directoryPath => true;

        sut[nameof(sut.DirectoryPath)].Should().BeNullOrEmpty();
        sut.Error.Should().BeNullOrEmpty();
      }
    }
  }
}
