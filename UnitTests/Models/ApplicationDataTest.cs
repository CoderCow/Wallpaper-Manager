using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using FluentAssertions;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models {
  public class ApplicationDataTest {
    [Fact]
    public void CtorShouldThrowOnNullArgument() {
      Action constructA = () => new ApplicationData(null, new ObservableCollection<IWallpaperCategory>(), new Dictionary<IWallpaperCategory, Path>());
      Action constructB = () => new ApplicationData(new Configuration(), null, new Dictionary<IWallpaperCategory, Path>());
      Action constructC = () => new ApplicationData(new Configuration(), new ObservableCollection<IWallpaperCategory>(), null);

      constructA.ShouldThrow<Exception>();
      constructB.ShouldThrow<Exception>();
      constructC.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldSetParametersProperly() {
      IConfiguration configuration = new Configuration();
      var categories = new ObservableCollection<IWallpaperCategory>();
      var categoryWatchedFolderAssociations = new Dictionary<IWallpaperCategory, Path>();
      ApplicationData sut = new ApplicationData(configuration, categories, categoryWatchedFolderAssociations);

      sut.Configuration.Should().Be(configuration);
      sut.WallpaperCategories.Should().BeSameAs(categories);
    }
  }
}
