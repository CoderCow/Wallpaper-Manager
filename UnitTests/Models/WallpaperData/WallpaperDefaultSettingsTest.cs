using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class WallpaperDefaultSettingsTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldSetBaseSettings() {
      IWallpaperBase baseSettings = this.modelFixtures.Create<WallpaperBase>();
      IDisplayInfo displayInfo = this.modelFixtures.Create<DisplayInfo>();
      
      WallpaperDefaultSettings sut = new WallpaperDefaultSettings(displayInfo, baseSettings);

      sut.Settings.Should().Be(baseSettings);
    }

    [Fact]
    public void CtorShouldThrowOnNoDisplayInfo() {
      IWallpaperBase baseSettings = this.modelFixtures.Create<WallpaperBase>();
      Action construct = () => new WallpaperDefaultSettings(null, baseSettings);

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldApplySettingsProperlyWithoutAutoDetermination() {
      IWallpaper target = this.modelFixtures.Create<Wallpaper>();
      WallpaperDefaultSettings sut = this.modelFixtures.Create<WallpaperDefaultSettings>();
      sut.AutoDeterminePlacement = false;
      sut.AutoDetermineIsMultiscreen = false;

      sut.ApplyToWallpaper(target);

      sut.Settings.Should().BePropertyValueEqual(target);
    }

    [Theory]
    [InlineData(10, 10, WallpaperPlacement.Tile)]
    [InlineData(10, 1080, WallpaperPlacement.Tile)]
    [InlineData(1240, 10, WallpaperPlacement.Tile)]
    [InlineData(640, 480, WallpaperPlacement.Tile)]
    [InlineData(641, 481, WallpaperPlacement.Uniform)]
    [InlineData(1280, 1024, WallpaperPlacement.Uniform)]
    public void ShouldAutoDeterminePlacementForSingleScreenSystemProperly(int wallpaperWidth, int wallpaperHeight, WallpaperPlacement expectedPlacement) {
      IWallpaper target = this.modelFixtures.Create<Wallpaper>();
      target.ImageSize = new Size(wallpaperWidth, wallpaperHeight);
      WallpaperDefaultSettings sut = this.modelFixtures.Create<WallpaperDefaultSettings>();
      sut.Settings.IsMultiscreen = false;
      sut.AutoDeterminePlacement = true;
      sut.AutoDetermineIsMultiscreen = false;

      sut.ApplyToWallpaper(target);

      target.Placement.Should().Be(expectedPlacement);
      target.IsMultiscreen.Should().BeFalse();
    }

    [Theory]
    [InlineData(10, 10, WallpaperPlacement.Tile, false)]
    [InlineData(640, 480, WallpaperPlacement.Tile, false)]
    [InlineData(640 * 1.5, 480, WallpaperPlacement.Tile, false)]
    [InlineData(640 * 1.5 + 1, 480, WallpaperPlacement.UniformToFill, true)]
    [InlineData(640 * 2, 480, WallpaperPlacement.UniformToFill, true)]
    public void ShouldAutoDetermineForMultiScreenSystemProperly(int wallpaperWidth, int wallpaperHeight, WallpaperPlacement expectedPlacement, bool expectedIsMultiscreen) {
      IWallpaper target = this.modelFixtures.Create<Wallpaper>();
      target.IsMultiscreen = !expectedIsMultiscreen;
      target.ImageSize = new Size(wallpaperWidth, wallpaperHeight);

      WallpaperDefaultSettings sut = this.DefaultSettingsWithMultiscreenDisplayInfo();
      sut.Settings.IsMultiscreen = !expectedIsMultiscreen;
      sut.AutoDeterminePlacement = true;
      sut.AutoDetermineIsMultiscreen = true;

      sut.ApplyToWallpaper(target);

      target.Placement.Should().Be(expectedPlacement);
      target.IsMultiscreen.Should().Be(expectedIsMultiscreen);
    }

    [Theory]
    [InlineData(10, 10, false)]
    [InlineData(640, 480, false)]
    [InlineData(640 * 1.5, 480, false)]
    [InlineData(640 * 1.5 + 1, 480, true)]
    [InlineData(640 * 2, 480, true)]
    public void ShouldAutoDetermineMultiScreenButNotPlacement(int wallpaperWidth, int wallpaperHeight, bool expectedIsMultiscreen) {
      IWallpaper target = this.modelFixtures.Create<Wallpaper>();
      target.IsMultiscreen = !expectedIsMultiscreen;
      target.ImageSize = new Size(wallpaperWidth, wallpaperHeight);

      WallpaperDefaultSettings sut = this.DefaultSettingsWithMultiscreenDisplayInfo();
      sut.Settings.IsMultiscreen = !expectedIsMultiscreen;
      sut.AutoDeterminePlacement = false;
      sut.AutoDetermineIsMultiscreen = true;

      sut.ApplyToWallpaper(target);

      target.Placement.Should().Be(sut.Settings.Placement);
      target.IsMultiscreen.Should().Be(expectedIsMultiscreen);
    }

    [Fact]
    public void ShouldReportErrorWhenSettingsIsInvalid() {
      WallpaperDefaultSettings sut = this.modelFixtures.Create<WallpaperDefaultSettings>();

      sut.Settings = null;

      sut[nameof(sut.Settings)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldNotReportErrorWhenSettingsIsValid() {
      WallpaperDefaultSettings sut = this.modelFixtures.Create<WallpaperDefaultSettings>();

      sut[nameof(sut.Settings)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      WallpaperDefaultSettings sut = 
        this.modelFixtures.Build<WallpaperDefaultSettings>()
        .With((x) => x.Settings, this.modelFixtures.Create<Wallpaper>()) // Note: WallpaperBaseImpl does not implement clone.
        .Create();
      
      var sutClone = (WallpaperDefaultSettings)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.Settings.Should().NotBeSameAs(sut.Settings);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        WallpaperDefaultSettings sut = this.modelFixtures.Create<WallpaperDefaultSettings>();
        WallpaperDefaultSettings target = this.modelFixtures.Create<WallpaperDefaultSettings>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }

    #region Helpers
    private WallpaperDefaultSettings DefaultSettingsWithMultiscreenDisplayInfo() {
      IDisplay primaryDisplay =
        this.modelFixtures.Build<DisplayStub>()
          .With((x) => x.IsPrimary, true)
          .With((x) => x.Bounds, new Rectangle(0, 0, 640, 480))
          .Create();
      IDisplayInfo displayInfo =
        this.modelFixtures.Build<DisplayInfoStub>()
          .With((x) => x.IsMultiDisplaySystem, true)
          .With((x) => x.PrimaryDisplay, primaryDisplay)
          .With((x) => x.Displays, new ReadOnlyCollection<IDisplay>(new IDisplay[] {}))
          .Create();

      return new WallpaperDefaultSettings(displayInfo, this.modelFixtures.Create<WallpaperBase>());
    }
    #endregion
  }
}
