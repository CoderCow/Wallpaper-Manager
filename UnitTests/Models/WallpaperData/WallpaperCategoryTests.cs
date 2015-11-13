using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models.WallpaperData {
  public class WallpaperCategoryTests {
    [Fact]
    public void CtorShouldThrowOnNullName() {
      Action construct = () => new WallpaperCategory(null, new WallpaperDefaultSettings(new WallpaperBaseImpl()));

      construct.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CtorShouldThrowOnNullDefaultSettings() {
      Action construct = () => new WallpaperCategory("New Category", null);

      construct.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CtorShouldntThrowOnEmptyWallpapers() {
      NewCategory(wallpapers: new List<IWallpaper>());
    }

    [Fact]
    public void CtorShouldAddWallpapers() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());
      var wallpapers = fixture.Create<List<IWallpaper>>();

      WallpaperCategory category = NewCategory(wallpapers: wallpapers);

      category.Wallpapers.Should().BeEquivalentTo(wallpapers);
    }

    [Fact]
    public void ShouldThrowOnNullName() {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Invoking((x) => x.Name = null).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldThrowOnNullDefaultSettings() {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Invoking((x) => x.WallpaperDefaultSettings = null).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData("")]
    public void ShouldReportErrorWhenNameIsTooShort(string nameToTest) {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooShort"), WallpaperCategory.Name_MinLength);
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("thisisaonesinglechartoolongname")]
    [InlineData("thisisawaaaaaaaaaaaaaaaaaaytoolongname")]
    public void ShouldReportErrorWhenNameIsTooLong(string nameToTest) {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooLong"), WallpaperCategory.Name_MaxLength);
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("  test")]
    [InlineData("t   ")]
    public void ShouldReportErrorWhenNameStartsOrEndsWithSpaces(string nameToTest) {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.Category.NameStartsOrEndsWithSpace");
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\t")]
    [InlineData("\b")]
    [InlineData("\a")]
    [InlineData("\v")]
    [InlineData("\f")]
    [InlineData("\x7F")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("[Name]")]
    [InlineData("sdf\tfddsf\basd")]
    [InlineData("\tabc")]
    [InlineData("abc\n")]
    public void ShouldReportErrorWhenNameContainsInvalidChars(string nameToTest) {
      WallpaperCategory sut = WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.Category.NameContainsInvalidChar");
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    private static WallpaperCategory NewCategory(string name = "New Category", WallpaperDefaultSettings defaultSettings = null, ICollection<IWallpaper> wallpapers = null) {
      defaultSettings = defaultSettings ?? new WallpaperDefaultSettings(new WallpaperBaseImpl());

      return new WallpaperCategory(name, defaultSettings, wallpapers);
    }

    #region Helpers
    private static WallpaperCategory WallpaperCategoryFromFixture() {
      Fixture fixture = WallpaperFixture();
      return fixture.Create<WallpaperCategory>();
    }

    private static Fixture WallpaperFixture() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());

      return fixture;
    }
    #endregion
  }
}
