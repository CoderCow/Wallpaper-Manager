using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using WallpaperManager.Models;

namespace UnitTests {
  public class WallpaperCustomization: ICustomization {
    public void Customize(IFixture fixture) {
      fixture.Register(() => 
        fixture.Build<WallpaperBaseImpl>()
        .Without((x) => x.OnlyCycleBetweenStart)
        .Without((x) => x.OnlyCycleBetweenStop)
        .Create());

      fixture.Register(() => 
        fixture.Build<Wallpaper>()
        .With((x) => x.CycleCountTotal, 100)
        .With((x) => x.CycleCountWeek, 10)
        .With((x) => x.TimeAdded, new DateTime(1999, 1, 1))
        .With((x) => x.TimeLastCycled, new DateTime(2000, 1, 1))
        .With((x) => x.ImageSize, new Size(10, 10))
        .Without((x) => x.OnlyCycleBetweenStart)
        .Without((x) => x.OnlyCycleBetweenStop)
        .Create());

      fixture.Register(() => 
        fixture.Build<WallpaperCategory>()
        .Without((x) => x.Wallpapers)
        .Create());

      fixture.Register(() => new Size(10, 10));

      fixture.Register<WallpaperBase>(fixture.Create<WallpaperBaseImpl>);
      fixture.Register<IWallpaperBase>(fixture.Create<WallpaperBaseImpl>);
      fixture.Register<IWallpaper>(fixture.Create<Wallpaper>);
      fixture.Register<IWallpaperCategory>(fixture.Create<WallpaperCategory>);
      fixture.Register<IWallpaperDefaultSettings>(fixture.Create<WallpaperDefaultSettings>);

      fixture.Customizations.Add(new StringSpeciemenBuilder());
      fixture.Customizations.Add(new FullPathSpeciemenBuilder());
    }
  }
}
