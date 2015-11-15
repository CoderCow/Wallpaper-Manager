using System;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models {
  public class ConfigurationTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();
    
    [Fact]
    public void CtorShouldSetDefaultsProperly() {
      Configuration sut = new Configuration();

      sut.AutocycleInterval.Should().Be(Configuration.DefaultAutocycleInterval);
      sut.WallpaperChangeType.Should().Be(Configuration.DefaultWallpaperChangeType);
      sut.WallpaperDoubleClickAction.Should().Be(Configuration.DefaultWallpaperDoubleClickAction);
      sut.TrayIconSingleClickAction.Should().Be(Configuration.DefaultTrayIconSingleClickAction);
      sut.TrayIconDoubleClickAction.Should().Be(Configuration.DefaultTrayIconDoubleClickAction);
      sut.ScreenSettings.Should().NotBeNull();
      sut.ScreenSettings.Should().BeEmpty();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(WallpaperChangeType))]
    public void ShouldThrowWhenChangeTypeIsSetToUndefinedValue(WallpaperChangeType changeType) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.WallpaperChangeType = changeType).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(WallpaperClickAction))]
    public void ShouldThrowWhenWallpaperDoubleClickActionIsSetToUndefinedValue(WallpaperClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.WallpaperDoubleClickAction = clickAction).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(TrayIconClickAction))]
    public void ShouldThrowWhenTraySingleClickActionIsSetToUndefinedValue(TrayIconClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.TrayIconSingleClickAction = clickAction).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(TrayIconClickAction))]
    public void ShouldThrowWhenTrayDoubleClickActionIsSetToUndefinedValue(TrayIconClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.TrayIconDoubleClickAction = clickAction).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoValidEnumData(typeof(WallpaperChangeType))]
    public void ShouldNotThrowWhenChangeTypeIsSetToDefinedValue(WallpaperChangeType changeType) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.WallpaperChangeType = changeType).ShouldNotThrow();
    }

    [Theory]
    [AutoValidEnumData(typeof(WallpaperClickAction))]
    public void ShouldNotThrowWhenWallpaperDoubleClickActionIsSetToDefinedValue(WallpaperClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.WallpaperDoubleClickAction = clickAction).ShouldNotThrow();
    }

    [Theory]
    [AutoValidEnumData(typeof(TrayIconClickAction))]
    public void ShouldNotThrowWhenTraySingleClickActionIsSetToDefinedValue(TrayIconClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.TrayIconSingleClickAction = clickAction).ShouldNotThrow();
    }

    [Theory]
    [AutoValidEnumData(typeof(TrayIconClickAction))]
    public void ShouldNotThrowWhenTrayDoubleClickActionIsSetToDefinedValue(TrayIconClickAction clickAction) {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      sut.Invoking((x) => x.TrayIconDoubleClickAction = clickAction).ShouldNotThrow();
    }

    [Theory]
    [InlineData(9)]
    [InlineData(0)]
    [InlineData(-1)]
    public void ShouldReportErrorWhenAutocycleIntervalIsBelowMinimum(int intervalInSeconds) {
      Configuration sut = this.modelFixtures.Create<Configuration>();

      sut.AutocycleInterval = TimeSpan.FromSeconds(intervalInSeconds);

      sut[nameof(sut.AutocycleInterval)].Should().Be(string.Format(LocalizationManager.GetLocalizedString("Error.Time.Minimum"), Configuration.MinAutocycleIntervalSeconds));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(9999)]
    public void ShouldNotReportErrorWhenFontNameIsValid(int intervalInSeconds) {
      Configuration sut = this.modelFixtures.Create<Configuration>();

      sut.AutocycleInterval = TimeSpan.FromSeconds(intervalInSeconds);

      sut[nameof(sut.AutocycleInterval)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      Configuration sut = this.modelFixtures.Create<Configuration>();
      
      Configuration sutClone = (Configuration)sut.Clone();

      sutClone.Should().BeCloneOf(sut, new[] {nameof(sut.ScreenSettings)});
      sutClone.ScreenSettings.Should().NotBeSameAs(sut.ScreenSettings);
      sutClone.ScreenSettings.Should().HaveCount(sut.ScreenSettings.Count);
      sutClone.ScreenSettings.Values.Should().NotBeSubsetOf(sut.ScreenSettings.Values);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        Configuration target = this.modelFixtures.Create<Configuration>();
        Configuration sut = this.modelFixtures.Create<Configuration>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }
  }
}
