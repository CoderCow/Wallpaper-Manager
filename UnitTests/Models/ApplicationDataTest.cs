using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models {
  public class ApplicationDataTest {
    [Fact]
    public void CtorShouldThrowOnNullArgument() {
      Action constructA = () => new ApplicationData(null, new ObservableCollection<IWallpaperCategory>());
      Action constructB = () => new ApplicationData(new Configuration(), null);

      constructA.ShouldThrow<Exception>();
      constructB.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldSetParametersProperly() {
      IConfiguration configuration = new Configuration();
      ObservableCollection<IWallpaperCategory> categories = new ObservableCollection<IWallpaperCategory>();
      ApplicationData sut = new ApplicationData(configuration, categories);

      sut.Configuration.Should().Be(configuration);
      sut.WallpaperCategories.Should().BeSameAs(categories);
    }
  }
}
