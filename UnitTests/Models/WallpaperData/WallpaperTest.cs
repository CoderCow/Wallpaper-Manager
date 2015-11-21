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
  public class WallpaperTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldSetImagePath() {
      Path testPath = new Path("SomeFile.png");

      Wallpaper sut = new Wallpaper(testPath);

      sut.ImagePath.Should().Be(testPath);
    }

    [Fact]
    public void CtorShouldAssignDefaultBackgroundColor() {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.BackgroundColor.Should().Be(WallpaperBase.DefaultBackgroundColor);
    }

    [Fact]
    public void ShouldIndicateWhenImageSizeWasResolved() {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.ImageSize = new Size(10, 10);

      sut.IsImageSizeResolved.Should().BeTrue();
    }

    [Fact]
    public void ShouldIndicateWhenImageSizeWasNotResolved() {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.ImageSize = null;

      sut.IsImageSizeResolved.Should().BeFalse();
    }

    [Fact]
    public void ShouldReportErrorWhenCycleTimeIsBelowAddTime() {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

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
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.TimeAdded = DateTime.FromFileTimeUtc(addedTimestamp);
      sut.TimeLastCycled = DateTime.FromFileTimeUtc(cycleTimestamp);

      sut[nameof(sut.TimeAdded)].Should().BeNullOrEmpty();
      sut[nameof(sut.TimeLastCycled)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(false, 0, 0)]
    [InlineData(false, 1, -9999)]
    [InlineData(false, -1, 1)]
    [InlineData(false, 1, -1)]
    [InlineData(false, -10, -10)]
    public void ShouldReportErrorWhenImageSizeIsInvalid(bool isVullValue, int width, int height) {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();
      Size? valueToTest = null;
      if (!isVullValue)
        valueToTest = new Size(width, height);

      sut.ImageSize = valueToTest;

      sut[nameof(sut.ImageSize)].Should().Be(LocalizationManager.GetLocalizedString("Error.Image.CantBeNegativeSize"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(true, 0, 0)]
    [InlineData(false, 1, 1)]
    [InlineData(false, 9999, 1)]
    [InlineData(false, 1, 9999)]
    public void ShouldNotReportErrorWhenImageSizeIsValid(bool isVullValue, int width, int height) {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();
      Size? valueToTest = null;
      if (!isVullValue)
        valueToTest = new Size(width, height);

      sut.ImageSize = valueToTest;

      sut[nameof(sut.ImageSize)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-999)]
    public void ShouldReportErrorWhenCountsAreInvalid(int valueToTest) {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.CycleCountTotal = valueToTest;
      sut.CycleCountWeek = valueToTest;

      sut[nameof(sut.CycleCountTotal)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsInvalid"));
      sut[nameof(sut.CycleCountWeek)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsInvalid"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999)]
    public void ShouldNotReportErrorWhenCountsAreValid(int valueToTest) {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

      sut.CycleCountTotal = valueToTest;
      sut.CycleCountWeek = valueToTest;

      sut[nameof(sut.CycleCountTotal)].Should().BeNullOrEmpty();
      sut[nameof(sut.CycleCountWeek)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      Wallpaper sut = this.modelFixtures.Create<Wallpaper>();
      sut.DisabledDevices.Add(new Path("C:\\"));
      sut.DisabledDevices.Add(new Path("C:\\File.jpg"));
      sut.DisabledDevices.Add(new Path("SomeFile.png"));
      
      Wallpaper sutClone = (Wallpaper)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.DisabledDevices.Should().NotBeSameAs(sut.DisabledDevices);
      sutClone.DisabledDevices.Should().ContainInOrder(sutClone.DisabledDevices);
      sutClone.DisabledDevices.Count.Should().Be(sut.DisabledDevices.Count);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        Wallpaper target = this.modelFixtures.Create<Wallpaper>();
        Wallpaper sut = this.modelFixtures.Create<Wallpaper>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }
  }
}