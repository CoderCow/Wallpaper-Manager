using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using Common.Windows;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Ploeh.AutoFixture;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests.Models {
  public class AppDataReaderWriterTest {
    private readonly Fixture modelFixtures = TestUtils.WallpaperFixture();

    [Fact]
    public void CtorShouldThrowOnInvalidInputFilePath() {
      Action construct = () => new AppDataReaderWriter(Path.Invalid, this.modelFixtures.Create<IDisplayInfo>());

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldThrowOnInvalidDisplayInfo() {
      Action construct = () => new AppDataReaderWriter(new Path("C:\\"), null);

      construct.ShouldThrow<Exception>();
    }

    [Fact]
    public void CtorShouldNotThrowOnValidParams() {
      new AppDataReaderWriter(new Path("C:\\"), this.modelFixtures.Create<IDisplayInfo>());
    }

    [Theory]
    [InlineData(0, 0, 0, false)]
    [InlineData(0, 0, 1, false)]
    [InlineData(1, 0, 4, false)]
    [InlineData(4, 1, 2, false)]
    [InlineData(4, 5, 0, false)]
    [InlineData(4, 1, 2, true)]
    [InlineData(4, 5, 0, true)]
    [InlineData(10, 30, 7, true)]
    public void ShouldSerializeAndDeserializeProperly(int categoryCount, int wallpapersPerCategory, int screenSettingCount, bool tryToProduceInvalidModels) {
      Fixture fixtureToUse = this.modelFixtures;
      if (tryToProduceInvalidModels) {
        fixtureToUse = new Fixture();
        fixtureToUse.Customize(new EssentialCustomizations());
      }

      AppDataReaderWriterFake sut = this.modelFixtures.Create<AppDataReaderWriterFake>();
      var configuration = CreateConfigurationTestData(fixtureToUse, screenSettingCount);
      var categories = new ObservableCollection<IWallpaperCategory>(CreateCategoryTestData(fixtureToUse, categoryCount, wallpapersPerCategory));
      IApplicationData appData = new ApplicationData(configuration, categories);

      sut.Write(appData);
      IApplicationData deserializedAppData = sut.Read();
      
      appData.Should().BeDeepPropertyEqual(deserializedAppData);
    }

    private static IConfiguration CreateConfigurationTestData(Fixture fixture, int screenSettingCount) {
      var screenSettings = new Dictionary<string, IScreenSettings>();
      for (int i = 0; i < screenSettingCount; i++) {
        string deviceUniqueId = string.Concat(@"MONITOR\BNQ78A5\", Guid.NewGuid().ToString(), @"\0004");
        screenSettings.Add(deviceUniqueId, fixture.Create<IScreenSettings>());
      }

      return fixture.Build<IConfiguration>()
        .With((x) => x.ScreenSettings, screenSettings)
        .Create<Configuration>();
    }

    private static IEnumerable<IWallpaperCategory> CreateCategoryTestData(Fixture fixture, int categoryCount, int wallpapersPerCategory) {
      for (int i = 0; i < categoryCount; i++) {
        yield return fixture.Build<IWallpaperCategory>()
          .Do((x) => x.Wallpapers.AddMany(() => fixture.Create<IWallpaper>(), wallpapersPerCategory))
          .Create<WallpaperCategory>();
      }
    }
  }
}
