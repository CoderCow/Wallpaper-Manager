using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class WallpaperBaseTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();
    
    [Fact]
    public void IsDefaultBackgroundColorAssigned() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.BackgroundColor.Should().Be(WallpaperBase.DefaultBackgroundColor);
    }

    [Theory]
    [InlineAutoData(999999, 0)]
    [InlineAutoData(0, -1)]
    [InlineAutoData(1, 0)]
    public void ShouldReportErrorWhenCycleStartTimeTooHigh(int onlyCycleBetweenStart, int onlyCycleBetweenStop) {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.OnlyCycleBetweenStop = new TimeSpan(onlyCycleBetweenStop);
      sut.OnlyCycleBetweenStart = new TimeSpan(onlyCycleBetweenStart);
      
      string expectedResultString = LocalizationManager.GetLocalizedString("Error.Wallpaper.CycleTime.Greater");
      sut[nameof(sut.OnlyCycleBetweenStop)].Should().Be(expectedResultString);
      sut[nameof(sut.OnlyCycleBetweenStart)].Should().Be(expectedResultString);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenCycleStartTimeBelowZero() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.OnlyCycleBetweenStart = new TimeSpan(-1);

      sut[nameof(sut.OnlyCycleBetweenStart)].Should().Be(LocalizationManager.GetLocalizedString("Error.Time.CantBeNegative"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenCycleStopTimeBelowZero() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.OnlyCycleBetweenStart = new TimeSpan(-2); // because start time must be below stop time
      sut.OnlyCycleBetweenStop = new TimeSpan(-1);

      sut[nameof(sut.OnlyCycleBetweenStop)].Should().Be(LocalizationManager.GetLocalizedString("Error.Time.CantBeNegative"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldThrowWhenAssigningToInvalidObject() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.Invoking(x => x.AssignTo(new object())).ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowOnInvalidPlacement() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();

      sut.Invoking(x => x.Placement = (WallpaperPlacement)(-1)).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      WallpaperBase sut = this.modelFixtures.Create<WallpaperBase>();
      sut.DisabledDevices.Add(new Path("C:\\"));
      sut.DisabledDevices.Add(new Path("C:\\File.jpg"));
      sut.DisabledDevices.Add(new Path("SomeFile.png"));
      
      WallpaperBase sutClone = (WallpaperBase)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
      sutClone.DisabledDevices.Should().NotBeSameAs(sut.DisabledDevices);
      sutClone.DisabledDevices.Should().ContainInOrder(sutClone.DisabledDevices);
      sutClone.DisabledDevices.Count.Should().Be(sut.DisabledDevices.Count);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        WallpaperBase sutA = this.modelFixtures.Create<WallpaperBase>();
        WallpaperBase subB = this.modelFixtures.Create<WallpaperBase>();

        subB.AssignTo(sutA);

        sutA.Should().BePropertyValueEqual(subB);
      }
    }
  }
}
