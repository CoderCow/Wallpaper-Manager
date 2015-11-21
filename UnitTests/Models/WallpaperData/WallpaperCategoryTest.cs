using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Common.Windows;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class WallpaperCategoryTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldntThrowOnEmptyWallpapers() {
      NewCategory(wallpapers: new List<IWallpaper>());
    }

    [Fact]
    public void CtorShouldSetDefaultSettings() {
      IWallpaperDefaultSettings expectedDefaultSettings = this.modelFixtures.Create<WallpaperDefaultSettings>();

      WallpaperCategory sut = this.NewCategory(defaultSettings: expectedDefaultSettings);

      sut.WallpaperDefaultSettings.Should().Be(expectedDefaultSettings);
    }

    [Fact]
    public void CtorShouldAddWallpapers() {
      var wallpapers = this.modelFixtures.Create<List<IWallpaper>>();

      WallpaperCategory sut = this.NewCategory(wallpapers: wallpapers);

      sut.Wallpapers.Should().BeEquivalentTo(wallpapers);
    }

    [Fact]
    public void ShouldRemainActiveWhenEmpty() {
      WallpaperCategory sut = new WallpaperCategory();
      sut.MonitorEvents();
      
      sut.IsActivated = false;

      sut.IsActivated.Should().BeTrue();
      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
    }

    [Fact]
    public void ShouldReportErrorWhenNameIsNull() {
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

      sut.Name = null;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenDefaultSettingsIsNull() {
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

      sut.WallpaperDefaultSettings = null;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      sut[nameof(sut.WallpaperDefaultSettings)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldNotReportErrorWhenDefaultSettingsIsNotNull() {
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

      sut[nameof(sut.WallpaperDefaultSettings)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    public void ShouldReportErrorWhenNameIsTooShort(string nameToTest) {
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

      sut.Name = nameToTest;

      string expectedErrorMsg = string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooShort"), WallpaperCategory.Name_MinLength);
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("thisisaonesinglechartoolongname")]
    [InlineData("thisisawaaaaaaaaaaaaaaaaaaytoolongname")]
    public void ShouldReportErrorWhenNameIsTooLong(string nameToTest) {
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

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
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

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
      WallpaperCategory sut = this.modelFixtures.Create<WallpaperCategory>();

      sut.Name = nameToTest;

      string expectedErrorMsg = LocalizationManager.GetLocalizedString("Error.Category.NameContainsInvalidChar");
      sut[nameof(sut.Name)].Should().Be(expectedErrorMsg);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldBeActiveAfterCreation() {
      WallpaperCategory sut = this.NewCategory();

      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeActiveWhenActiveWallpapersAreAddedOnCreation() {
      List<IWallpaper> wallpapers = this.NewWallpaperList(3, true);

      WallpaperCategory sut = this.NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBeInactiveWhenInactiveWallpapersAreAddedOnCreation() {
      List<IWallpaper> wallpapers = this.NewWallpaperList(3, false);

      WallpaperCategory sut = this.NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBeNullWhenActiveAndInactiveWallpapersAreAddedOnCreation() {
      List<IWallpaper> wallpapers = this.NewWallpaperList(3, null);

      WallpaperCategory sut = this.NewCategory(wallpapers: wallpapers);

      sut.IsActivated.Should().Be(null);
    }

    [Theory]
    [InlineData(1, false), InlineData(2, false), InlineData(3, false)]
    public void ShouldStayActiveWhenActiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = this.NewCategory();

      AddWallpapers(sut, this.NewWallpaperList(wallpaperCount, true), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, true), InlineData(2, false), InlineData(3, false)]
    public void ShouldBecomeInactiveWhenInactiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = this.NewCategory();

      AddWallpapers(sut, this.NewWallpaperList(wallpaperCount, false), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().BeFalse();
    }

    [Theory]
    [InlineData(2, true), InlineData(3, false), InlineData(4, false)]
    public void ShouldBecomeNullWhenInactiveAndActiveWallpapersAreAddedAfterCreation(int wallpaperCount, bool lastAddShouldRaisePropertyChanged) {
      WallpaperCategory sut = this.NewCategory();

      AddWallpapers(sut, this.NewWallpaperList(wallpaperCount, null), lastAddShouldRaisePropertyChanged);

      sut.IsActivated.Should().Be(null);
    }

    [Fact]
    public void ShouldBecomeActiveWhenLastInactiveWasRemoved() {
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(true),
        this.NewWallpaper(false),
        this.NewWallpaper(true)
      });
      sut.MonitorEvents();

      sut.Wallpapers.RemoveAt(1);

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeInactiveWhenLastActiveWasRemoved() {
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(true),
        this.NewWallpaper(false),
        this.NewWallpaper(false)
      });
      sut.MonitorEvents();

      sut.Wallpapers.RemoveAt(0);

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBecomeActiveWhenCleared() {
      List<IWallpaper> mixedWallpapers = this.NewWallpaperList(4, null);
      WallpaperCategory sut = this.NewCategory(wallpapers: mixedWallpapers);
      sut.MonitorEvents();

      sut.Wallpapers.Clear();

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeActiveWhenEmptied() {
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(true),
        this.NewWallpaper(false),
        this.NewWallpaper(false)
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
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(true),
        this.NewWallpaper(false),
        this.NewWallpaper(true)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = true;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeTrue();
    }

    [Fact]
    public void ShouldBecomeInactiveWhenLastActiveIsToggled() {
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(false),
        this.NewWallpaper(true),
        this.NewWallpaper(false)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = false;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().BeFalse();
    }

    [Fact]
    public void ShouldBecomeNullWhenWallpapersAreToggledIntoMixedState() {
      WallpaperCategory sut = this.NewCategory(wallpapers: new[] {
        this.NewWallpaper(false),
        this.NewWallpaper(false),
        this.NewWallpaper(false)
      });
      sut.MonitorEvents();

      sut.Wallpapers[1].IsActivated = true;

      sut.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      sut.IsActivated.Should().Be(null);
    }

    #region Helpers
    private WallpaperCategory NewCategory(string name = "New Category", IWallpaperDefaultSettings defaultSettings = null, ICollection<IWallpaper> wallpapers = null) {
      defaultSettings = defaultSettings ?? new WallpaperDefaultSettings(this.modelFixtures.Create<IDisplayInfo>(), new WallpaperBase());

      return new WallpaperCategory(name, defaultSettings, wallpapers);
    }

    private List<IWallpaper> NewWallpaperList(int count, bool? activeStatus) {
      bool shouldAddMixedStatus = (activeStatus == null);
      bool addActive;
      if (shouldAddMixedStatus)
        addActive = false;
      else
        addActive = activeStatus.Value;

      var wallpapers = new List<IWallpaper>();
      for (int i = 0; i < count; i++) {
        IWallpaper wallpaper = this.NewWallpaper(addActive);
        if (shouldAddMixedStatus)
          addActive = !addActive;

        wallpapers.Add(wallpaper);
      }

      return wallpapers;
    }

    private IWallpaper NewWallpaper(bool isActive) {
      return this.modelFixtures.Build<Wallpaper>().With((x) => x.IsActivated, isActive).Create();
    }

    private static void AddWallpapers(WallpaperCategory category, IList<IWallpaper> wallpapers, bool lastShouldRaisePropertyChanged) {
      for (int i = 0; i < wallpapers.Count - 1; i++) {
        IWallpaper wallpaper = wallpapers[i];
        category.Wallpapers.Add(wallpaper);
      }

      category.MonitorEvents();
      category.Wallpapers.Add(wallpapers[wallpapers.Count - 1]);

      if (lastShouldRaisePropertyChanged)
        category.ShouldRaisePropertyChangeFor((x) => x.IsActivated);
      else
        category.ShouldNotRaisePropertyChangeFor((x) => x.IsActivated);
    }
    #endregion
  }
}