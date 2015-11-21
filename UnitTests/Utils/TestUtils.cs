using Ploeh.AutoFixture;
using UnitTests;
using WallpaperManager.Models;

namespace UnitTests {
  internal class TestUtils {
    public static WallpaperCategory WallpaperCategoryFromFixture(int withWallpaperCount, bool activationStatus = true, Fixture fixture = null) {
      if (fixture == null)
        fixture = WallpaperFixture();

      WallpaperCategory category = fixture.Create<WallpaperCategory>();

      for (int i = 0; i < withWallpaperCount; i++) {
        Wallpaper wallpaper = fixture.Build<Wallpaper>().With((x) => x.IsActivated, activationStatus).Create();
        category.Wallpapers.Add(wallpaper);
      }

      return category;
    }

    public static WallpaperCategory WallpaperCategoryFromFixture(Fixture fixture = null) {
      if (fixture == null)
        fixture = WallpaperFixture();

      return fixture.Create<WallpaperCategory>();
    }

    public static WallpaperDefaultSettings WallpaperDefaultSettingsFromFixture(Fixture fixture = null) {
      if (fixture == null)
        fixture = WallpaperFixture();

      return fixture.Create<WallpaperDefaultSettings>();
    }

    public static WallpaperBase WallpaperBaseFromFixture(Fixture fixture = null) {
      if (fixture == null)
        fixture = WallpaperFixture();

      return fixture.Create<WallpaperBase>();
    }

    public static Wallpaper WallpaperFromFixture(Fixture fixture = null) {
      if (fixture == null)
        fixture = WallpaperFixture();

      return fixture.Create<Wallpaper>();
    }

    public static Fixture WallpaperFixture() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());

      return fixture;
    }
  }
}