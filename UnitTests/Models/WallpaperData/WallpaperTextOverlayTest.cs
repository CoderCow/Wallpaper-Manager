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
      TextOverlay sut = new TextOverlay();

      sut.FontName.Should().Be(TextOverlay.DefaultFontName);
      sut.FontSize.Should().Be(TextOverlay.DefaultFontSize);
      sut.FontStyle.Should().Be(TextOverlay.DefaultFontStyle);
      sut.Format.Should().Be(LocalizationManager.GetLocalizedString("OverlayTextData.DefaultFormat"));
      sut.ForeColor.Should().Be(TextOverlay.DefaultForeColor);
      sut.BorderColor.Should().Be(TextOverlay.DefaultBorderColor);
      sut.Position.Should().Be(TextOverlay.DefaultPosition);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void ShouldReportErrorWhenFormatTextIsInvalid(string formatString) {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();

      sut.Format = formatString;

      sut[nameof(sut.Format)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldThrowWhenFormatIsSetToNull() {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();
      
      sut.Invoking((x) => x.Format = null).ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldThrowWhenFontNameIsSetToNull() {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();
      
      sut.Invoking((x) => x.FontName = null).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoInvalidEnumData(typeof(TextOverlayPosition))]
    public void ShouldThrowWhenPositionIsSetToUndefinedValue(TextOverlayPosition position) {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();
      
      sut.Invoking((x) => x.Position = position).ShouldThrow<Exception>();
    }

    [Theory]
    [AutoValidEnumData(typeof(TextOverlayPosition))]
    public void ShouldNotThrowWhenFontStyleIsSetToDefinedValue(TextOverlayPosition position) {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();
      
      sut.Invoking((x) => x.Position = position).ShouldNotThrow();
    }

    [Theory]
    [InlineData("x")]
    [InlineData("  xy  ")]
    [InlineData("  x\r\ny\tz  ")]
    [InlineData("x %test%")]
    [InlineData("thisisaveryveryveryveryveryveryveryveryveryveryveryveryveryverylongstring")]
    public void ShouldNotReportErrorWhenFormatTextIsValid(string formatString) {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();

      sut.Format = formatString;

      sut[nameof(sut.Format)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void ShouldReportErrorWhenFontNameIsInvalid(string fontName) {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();

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
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();

      sut.FontName = fontName;

      sut[nameof(sut.FontName)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldCreateProperClones() {
      TextOverlay sut = this.modelFixtures.Create<TextOverlay>();
      
      TextOverlay sutClone = (TextOverlay)sut.Clone();

      sutClone.Should().BeCloneOf(sut);
    }

    [Fact]
    public void ShouldAssignAllProperties() {
      for (int i = 0; i < 10; i++) {
        TextOverlay target = this.modelFixtures.Create<TextOverlay>();
        TextOverlay sut = this.modelFixtures.Create<TextOverlay>();

        sut.AssignTo(target);

        target.Should().BePropertyValueEqual(sut);
      }
    }
  }
}
