using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class ScreenSettingsTest {
    private readonly Fixture concreteValidModels = TestUtils.WallpaperFixture();

    [Fact]
    public void TextOverlaysShouldNotBeNull() {
      ScreenSettings sut = new ScreenSettings();

      sut.TextOverlays.Should().NotBeNull();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      ScreenSettings sut = this.concreteValidModels.Create<ScreenSettings>();
      
      var sutClone = (ScreenSettings)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.StaticWallpaper.Should().NotBeSameAs(sut.StaticWallpaper);
      sutClone.TextOverlays.Should().NotBeSameAs(sut.TextOverlays);
      sutClone.TextOverlays.Should().NotContain(sut.TextOverlays);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        ScreenSettings sut = this.concreteValidModels.Create<ScreenSettings>();
        ScreenSettings target = this.concreteValidModels.Create<ScreenSettings>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }
  }
}
