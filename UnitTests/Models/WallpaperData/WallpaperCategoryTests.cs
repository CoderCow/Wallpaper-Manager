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

namespace UnitTests {
  public class WallpaperCategoryTests {
    [Fact]
    public void CtorShouldThrowOnNullName() {
      Action construct = () => new WallpaperCategory(null, new WallpaperDefaultSettings(new WallpaperBaseImpl(), null));

      construct.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CtorShouldThrowOnNullDefaultSettings() {
      Action construct = () => new WallpaperCategory("New Category", null);

      construct.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CtorShouldntThrowOnEmptyWallpapers() {
      NewCategory(wallpapers: new List<Wallpaper>());
    }

    [Fact]
    public void CtorShouldSetDefaultSettings() {
      WallpaperDefaultSettings expectedDefaultSettings = new WallpaperDefaultSettings(new WallpaperBaseImpl(), null);
      
      WallpaperCategory sut = NewCategory(defaultSettings: expectedDefaultSettings);

      sut.WallpaperDefaultSettings.Should().Be(expectedDefaultSettings);
    }

    [Fact]
    public void CtorShouldAddWallpapers() {
      Fixture fixture = WallpaperFixture();
      var wallpapers = fixture.Create<List<Wallpaper>>();

      WallpaperCategory sut = NewCategory(wallpapers: wallpapers);

      sut.Wallpapers.Should().BeEquivalentTo(wallpapers);
    }

    [Fact]
    public void ShouldThrowOnNullName() {
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

      sut.Invoking((x) => x.Name = null).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldThrowOnNullDefaultSettings() {
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

      sut.Invoking((x) => x.WallpaperDefaultSettings = null).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData("")]
    public void ShouldReportErrorWhenNameIsTooShort(string nameToTest) {
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooShort"), WallpaperCategory.Name_MinLength);
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("thisisaonesinglechartoolongname")]
    [InlineData("thisisawaaaaaaaaaaaaaaaaaaytoolongname")]
    public void ShouldReportErrorWhenNameIsTooLong(string nameToTest) {
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

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
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

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
      WallpaperCategory sut = TestUtils.WallpaperCategoryFromFixture();

      sut.Name = nameToTest;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.Category.NameContainsInvalidChar");
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldBeActiveAfterCreation() {
      WallpaperCategory sut = NewCategory();

      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeActiveWhenActiveWallpapersAreAddedOnCreation() {
      var wallpapers = NewWallpaperList(3, true);

      WallpaperCategory sut = NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeInactiveWhenInactiveWallpapersAreAddedOnCreation() {
      var wallpapers = NewWallpaperList(3, false);

      WallpaperCategory sut = NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBeNullWhenActiveAndInactiveWallpapersAreAddedOnCreation() {
      var wallpapers = NewWallpaperList(3, null);

      WallpaperCategory sut = NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().Be(null);
    }

    [Theory]
    [InlineData(1, false), InlineData(2, false), InlineData(3, false)]
    public void ShouldStayActiveWhenActiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = NewCategory();

      AddWallpapers(sut, NewWallpaperList(wallpaperCount, true), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, true), InlineData(2, false), InlineData(3, false)]
    public void ShouldBecomeInactiveWhenInactiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = NewCategory();

      AddWallpapers(sut, NewWallpaperList(wallpaperCount, false), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().BeFalse();
    }

    [Theory]
    [InlineData(2, true), InlineData(3, false), InlineData(4, false)]
    public void ShouldBecomeNullWhenInactiveAndActiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = NewCategory();

      AddWallpapers(sut, NewWallpaperList(wallpaperCount, null), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().Be(null);
    }

    [Fact]
    public void ShouldBecomeActiveWhenLastInactiveWasRemoved() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, true),
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, true)
      });
      sut.MonitorEvents();

      sut.Wallpapers.RemoveAt(1);

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeInactiveWhenLastActiveWasRemoved() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, true),
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, false)
      });
      sut.MonitorEvents();

      sut.Wallpapers.RemoveAt(0);

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBecomeActiveWhenCleared() {
      var mixedWallpapers = NewWallpaperList(4, null);
      WallpaperCategory sut = NewCategory(wallpapers: mixedWallpapers);
      sut.MonitorEvents();

      sut.Wallpapers.Clear();

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeActiveWhenEmptied() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, true),
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, false)
      });
      sut.MonitorEvents();

      sut.Wallpapers.RemoveAt(0);
      sut.Wallpapers.RemoveAt(0);
      sut.Wallpapers.RemoveAt(0);

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeActiveWhenLastInactiveIsToggled() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, true),
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, true)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = true;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeInactiveWhenLastActiveIsToggled() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, true),
        NewWallpaper(fixture, false)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = false;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBecomeNullWhenWallpapersAreToggledIntoMixedState() {
      Fixture fixture = WallpaperFixture();
      WallpaperCategory sut = NewCategory(wallpapers: new[] {
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, false),
        NewWallpaper(fixture, false)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = true;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().Be(null);
    }

    #region Helpers
    private static WallpaperCategory NewCategory(string name = "New Category", WallpaperDefaultSettings defaultSettings = null, ICollection<Wallpaper> wallpapers = null) {
      defaultSettings = defaultSettings ?? new WallpaperDefaultSettings(new WallpaperBaseImpl(), null);

      return new WallpaperCategory(name, defaultSettings, wallpapers);
    }

    private static List<Wallpaper> NewWallpaperList(int count, bool? activeStatus) {
      Fixture fixture = WallpaperFixture();

      bool shouldAddMixedStatus = (activeStatus == null);
      bool addActive;
      if (shouldAddMixedStatus)
        addActive = false;
      else
        addActive = activeStatus.Value;
      
      List<Wallpaper> wallpapers = new List<Wallpaper>();
      for (int i = 0; i < count; i++) {
        Wallpaper wallpaper = NewWallpaper(fixture, addActive);
        if (shouldAddMixedStatus)
          addActive = !addActive;

        wallpapers.Add(wallpaper);
      }

      return wallpapers;
    }

    private static Wallpaper NewWallpaper(Fixture fixture, bool isActive) {
      return fixture.Build<Wallpaper>().With((x) => x.IsActivated, isActive).Create();
    }

    private static void AddWallpapers(WallpaperCategory category, IList<Wallpaper> wallpapers, bool lastShouldRaisePropertyChanged) {
      for (int i = 0; i < wallpapers.Count - 1; i++) {
        Wallpaper wallpaper = wallpapers[i];
        category.Wallpapers.Add(wallpaper);
      }

      category.MonitorEvents();
      category.Wallpapers.Add(wallpapers[wallpapers.Count - 1]);

      if (lastShouldRaisePropertyChanged)
        category.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      else 
        category.ShouldNotRaisePropertyChangeFor((x) => x.IsActivated);
    }

    private static Fixture WallpaperFixture() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());

      return fixture;
    }
    #endregion
  }
}
