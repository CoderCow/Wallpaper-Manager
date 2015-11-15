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
    private readonly Fixture concreteValidModels = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldSetBaseSettings() {
      IWallpaperBase baseSettings = this.concreteValidModels.Create<WallpaperBase>();
      IDisplayInfo displayInfo = this.concreteValidModels.Create<DisplayInfo>();
      
      WallpaperDefaultSettings sut = new WallpaperDefaultSettings(baseSettings, displayInfo);

      sut.Settings.Should().Be(baseSettings);
    }

    [Fact]
    public void CtorShouldThrowOnNoBaseSettings() {
      IDisplayInfo displayInfo = this.concreteValidModels.Create<DisplayInfo>();
      Action construct = () => new WallpaperDefaultSettings(null, displayInfo);

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldThrowOnNoDisplayInfo() {
      IWallpaperBase baseSettings = this.concreteValidModels.Create<WallpaperBase>();
      Action construct = () => new WallpaperDefaultSettings(baseSettings, null);

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldApplySettingsProperlyWithoutAutoDetermination() {
      IWallpaper target = this.concreteValidModels.Create<Wallpaper>();
      WallpaperDefaultSettings sut = this.concreteValidModels.Create<WallpaperDefaultSettings>();
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
      IWallpaper target = this.concreteValidModels.Create<Wallpaper>();
      target.ImageSize = new Size(wallpaperWidth, wallpaperHeight);
      WallpaperDefaultSettings sut = this.concreteValidModels.Create<WallpaperDefaultSettings>();
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
      IWallpaper target = this.concreteValidModels.Create<Wallpaper>();
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
      IWallpaper target = this.concreteValidModels.Create<Wallpaper>();
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
    public void ShouldCreateProperClones() {
      WallpaperDefaultSettings sut = 
        this.concreteValidModels.Build<WallpaperDefaultSettings>()
        .With((x) => x.Settings, this.concreteValidModels.Create<Wallpaper>()) // Note: WallpaperBaseImpl does not implement clone.
        .Create();
      
      var sutClone = (WallpaperDefaultSettings)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.Settings.Should().NotBeSameAs(sut.Settings);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        WallpaperDefaultSettings sut = this.concreteValidModels.Create<WallpaperDefaultSettings>();
        WallpaperDefaultSettings target = this.concreteValidModels.Create<WallpaperDefaultSettings>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }

    #region Helpers
    private WallpaperDefaultSettings DefaultSettingsWithMultiscreenDisplayInfo() {
      IDisplay primaryDisplay =
        this.concreteValidModels.Build<DisplayStub>()
          .With((x) => x.IsPrimary, true)
          .With((x) => x.Bounds, new Rectangle(0, 0, 640, 480))
          .Create();
      IDisplayInfo displayInfo =
        this.concreteValidModels.Build<DisplayInfoStub>()
          .With((x) => x.IsMultiDisplaySystem, true)
          .With((x) => x.PrimaryDisplay, primaryDisplay)
          .With((x) => x.Displays, new ReadOnlyCollection<IDisplay>(new IDisplay[] {}))
          .Create();

      return new WallpaperDefaultSettings(this.concreteValidModels.Create<WallpaperBase>(), displayInfo);
    }
    #endregion
  }
}
