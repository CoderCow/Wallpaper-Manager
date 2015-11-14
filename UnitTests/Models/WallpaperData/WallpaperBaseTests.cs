using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
  public class WallpaperBaseTests {
    [Fact]
    public void IsDefaultBackgroundColorAssigned() {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.BackgroundColor.Should().Be(WallpaperBase.DefaultBackgroundColor);
    }

    [Theory]
    [InlineAutoData(999999, 0)]
    [InlineAutoData(0, -1)]
    [InlineAutoData(1, 0)]
    public void ShouldReportErrorWhenCycleStartTimeTooHigh(int onlyCycleBetweenStart, int onlyCycleBetweenStop) {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.OnlyCycleBetweenStop = new TimeSpan(onlyCycleBetweenStop);
      sut.OnlyCycleBetweenStart = new TimeSpan(onlyCycleBetweenStart);
      
      string expectedResultString = LocalizationManager.GetLocalizedString("Error.Wallpaper.CycleTime.Greater");
      sut[nameof(sut.OnlyCycleBetweenStop)].Should().Be(expectedResultString);
      sut[nameof(sut.OnlyCycleBetweenStart)].Should().Be(expectedResultString);
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenCycleStartTimeBelowZero() {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.OnlyCycleBetweenStart = new TimeSpan(-1);

      sut[nameof(sut.OnlyCycleBetweenStart)].Should().Be(LocalizationManager.GetLocalizedString("Error.Time.CantBeNegative"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenCycleStopTimeBelowZero() {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.OnlyCycleBetweenStart = new TimeSpan(-2); // because start time must be below stop time
      sut.OnlyCycleBetweenStop = new TimeSpan(-1);

      sut[nameof(sut.OnlyCycleBetweenStop)].Should().Be(LocalizationManager.GetLocalizedString("Error.Time.CantBeNegative"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldThrowWhenAssigningToInvalidObject() {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.Invoking(x => x.AssignTo(new object())).ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowOnInvalidPlacement() {
      WallpaperBase sut = TestUtils.WallpaperBaseFromFixture();

      sut.Invoking(x => x.Placement = (WallpaperPlacement)(-1)).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        Fixture fixture = TestUtils.WallpaperFixture();
        WallpaperBaseImpl sutA = fixture.Create<WallpaperBaseImpl>();
        WallpaperBaseImpl subB = fixture.Create<WallpaperBaseImpl>();

        subB.AssignTo(sutA);

        sutA.Should().BePropertyValueEqual(subB);
      }
    }
  }
}
