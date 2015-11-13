using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Common.IO;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  
  public class WallpaperTests {
    [Fact]
    public void CtorShouldSetImagePath() {
      Path testPath = new Path("SomeFile.png");

      Wallpaper sut = new Wallpaper(testPath);

      sut.ImagePath.Should().Be(testPath);
    }

    [Fact]
    public void CtorShouldThrowOnInvalidImagePath() {
      Action construct = () => new Wallpaper(Path.Invalid);

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldAssignDefaultBackgroundColor() {
      Wallpaper sut = WallpaperFromFixture();

      sut.BackgroundColor.Should().Be(WallpaperBase.DefaultBackgroundColor);
    }

    [Fact]
    public void ShouldThrowOnInvalidImagePath() {
      Wallpaper sut = WallpaperFromFixture();

      sut.Invoking((x) => x.ImagePath = Path.Invalid).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-999)]
    public void ShouldThrowOnInvalidTotalCycleCount(int valueToTest) {
      Wallpaper sut = WallpaperFromFixture();

      sut.Invoking((x) => x.CycleCountTotal = valueToTest).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-999)]
    public void ShouldThrowOnInvalidWeekCycleCount(int valueToTest) {
      Wallpaper sut = WallpaperFromFixture();

      sut.Invoking((x) => x.CycleCountWeek = valueToTest).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999)]
    public void ShouldntThrowOnValidCycleCount(int valueToTest) {
      Wallpaper sut = WallpaperFromFixture();

      sut.Invoking((x) => x.CycleCountTotal = valueToTest);
    }

    [Fact]
    public void ShouldIndicateWhenImageSizeWasResolved() {
      Wallpaper sut = WallpaperFromFixture();

      sut.ImageSize = new Size(10, 10);

      sut.IsImageSizeResolved.Should().BeTrue();
    }

    [Fact]
    public void ShouldIndicateWhenImageSizeWasNotResolved() {
      Wallpaper sut = WallpaperFromFixture();

      sut.ImageSize = null;

      sut.IsImageSizeResolved.Should().BeFalse();
    }

    [Fact]
    public void ShouldIndicateWhenImageSizeWasNotResolved2() {
      Wallpaper sut = WallpaperFromFixture();

      sut.IsImageSizeResolved.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, -1)]
    public void ShouldThrowOnImageSizeIsZeroOrLess(int width, int height) {
      Wallpaper sut = WallpaperFromFixture();

      sut.Invoking((x) => x.ImageSize = new Size(width, height)).ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 1)]
    [InlineData(1, 10)]
    [InlineData(999, 999)]
    public void ShouldNotThrowWhenImageSizeIsGreaterThanZero(int width, int height) {
      Wallpaper sut = WallpaperFromFixture();

      sut.ImageSize = new Size(width, height);
    }

    [Fact]
    public void ShouldReportErrorWhenCycleTimeIsBelowAddTime() {
      Wallpaper sut = WallpaperFromFixture();

      sut.TimeAdded = new DateTime(2000, 1, 1);
      sut.TimeLastCycled = new DateTime(1999, 1, 1);

      sut[nameof(sut.TimeAdded)].Should().Be(LocalizationManager.GetLocalizedString("Error.Wallpaper.AddedCycledTimeInvalid"));
      sut[nameof(sut.TimeLastCycled)].Should().Be(LocalizationManager.GetLocalizedString("Error.Wallpaper.AddedCycledTimeInvalid"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1000, 2000)]
    public void ShouldNotReportErrorWhenCycleTimeIsEqualOrGreater(int addedTimestamp, int cycleTimestamp) {
      Wallpaper sut = WallpaperFromFixture();

      sut.TimeAdded = DateTime.FromFileTimeUtc(addedTimestamp);
      sut.TimeLastCycled = DateTime.FromFileTimeUtc(cycleTimestamp);

      sut[nameof(sut.TimeAdded)].Should().BeNullOrEmpty();
      sut[nameof(sut.TimeLastCycled)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      Wallpaper sut = WallpaperFromFixture();
      sut.DisabledScreens.Add(1);
      sut.DisabledScreens.Add(3);
      sut.DisabledScreens.Add(6);
      var originalDisabledScreens = sut.DisabledScreens;
      
      Wallpaper sutClone = (Wallpaper)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.DisabledScreens.Should().NotBeSameAs(sut.DisabledScreens);
      sutClone.DisabledScreens.Should().ContainInOrder(sutClone.DisabledScreens);
      sutClone.DisabledScreens.Count.Should().Be(sut.DisabledScreens.Count);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        Fixture fixture = WallpaperFixture();
        Wallpaper sutA = fixture.Create<Wallpaper>();
        Wallpaper sutB = fixture.Create<Wallpaper>();

        sutB.AssignTo(sutA);

        sutA.Should().BePropertyValueEqual(sutB);
      }
    }

    private static Wallpaper WallpaperFromFixture() {
      Fixture fixture = WallpaperFixture();
      return fixture.Create<Wallpaper>();
    }

    private static Fixture WallpaperFixture() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());

      return fixture;
    }
  }
}