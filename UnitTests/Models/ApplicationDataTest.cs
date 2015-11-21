using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models {
  public class ApplicationDataTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldSetParametersProperly() {
      IConfiguration configuration = new Configuration();
      var categories = new ObservableCollection<IWallpaperCategory>();
      ApplicationData sut = new ApplicationData(configuration, categories);

      sut.Configuration.Should().Be(configuration);
      sut.WallpaperCategories.Should().BeSameAs(categories);
    }

    [Fact]
    public void ShouldReportErrorWhenConfigurationIsInvalid() {
      ApplicationData sut = this.modelFixtures.Create<ApplicationData>();

      sut.Configuration = null;

      sut[nameof(sut.Configuration)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldNotReportErrorWhenConfigurationIsValid() {
      ApplicationData sut = this.modelFixtures.Create<ApplicationData>();

      sut[nameof(sut.Configuration)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldReportErrorWhenCategoriesIsInvalid() {
      ApplicationData sut = this.modelFixtures.Create<ApplicationData>();

      sut.WallpaperCategories = null;

      sut[nameof(sut.WallpaperCategories)].Should().Be(LocalizationManager.GetLocalizedString("Error.FieldIsMandatory"));
      sut.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldNotReportErrorWhenCategoriesIsValid() {
      ApplicationData sut = this.modelFixtures.Create<ApplicationData>();

      sut[nameof(sut.WallpaperCategories)].Should().BeNullOrEmpty();
      sut.Error.Should().BeNullOrEmpty();
    }
  }
}
