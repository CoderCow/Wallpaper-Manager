using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  public class WallpaperTextOverlayTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldSetDefaultValues() {
      WallpaperTextOverlay sut = new WallpaperTextOverlay();

      sut.FontName.Should().Be(WallpaperTextOverlay.DefaultFontName);
      sut.FontSize.Should().Be(WallpaperTextOverlay.DefaultFontSize);
      sut.FontStyle.Should().Be(WallpaperTextOverlay.DefaultFontStyle);
      sut.Format.Should().Be(LocalizationManager.GetLocalizedString("OverlayTextData.DefaultFormat"));
      sut.ForeColor.Should().Be(WallpaperTextOverlay.DefaultForeColor);
      sut.BorderColor.Should().Be(WallpaperTextOverlay.DefaultBorderColor);
      sut.Position.Should().Be(WallpaperTextOverlay.DefaultPosition);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void ShouldReportErrorWhenFormatTextIsInvalid(string formatString) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();

      sut.Format = formatString;

      sut[nameof(sut.Format)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldThrowWhenFormatIsSetToNull() {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();
      
      sut.Invoking((x) => x.Format = null).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldThrowWhenFontNameIsSetToNull() {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();
      
      sut.Invoking((x) => x.FontName = null).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(TextOverlayPosition))]
    public void ShouldThrowWhenPositionIsSetToUndefinedValue(TextOverlayPosition position) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();
      
      sut.Invoking((x) => x.Position = position).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoValidEnumData(typeof(TextOverlayPosition))]
    public void ShouldNotThrowWhenFontStyleIsSetToDefinedValue(TextOverlayPosition position) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();
      
      sut.Invoking((x) => x.Position = position).ShouldNotThrow();
    }

    [Theory]
    [InlineData("x")]
    [InlineData("  xy  ")]
    [InlineData("  x\r\ny\tz  ")]
    [InlineData("x %test%")]
    [InlineData("thisisaveryveryveryveryveryveryveryveryveryveryveryveryveryverylongstring")]
    public void ShouldNotReportErrorWhenFormatTextIsValid(string formatString) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();

      sut.Format = formatString;

      sut[nameof(sut.Format)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void ShouldReportErrorWhenFontNameIsInvalid(string fontName) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();

      sut.FontName = fontName;

      sut[nameof(sut.FontName)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("x")]
    [InlineData("  xy  ")]
    [InlineData("  x\r\ny\tz  ")]
    [InlineData("x %test%")]
    [InlineData("Verdana")]
    [InlineData("thisisaveryveryveryveryveryveryveryveryveryveryveryveryveryverylongstring")]
    public void ShouldNotReportErrorWhenFontNameIsValid(string fontName) {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();

      sut.FontName = fontName;

      sut[nameof(sut.FontName)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();
      
      WallpaperTextOverlay sutClone = (WallpaperTextOverlay)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        WallpaperTextOverlay target = this.modelFixtures.Create<WallpaperTextOverlay>();
        WallpaperTextOverlay sut = this.modelFixtures.Create<WallpaperTextOverlay>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }
  }
}
