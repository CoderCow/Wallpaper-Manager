using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;
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
        .Without((x) => x.BackgroundColor)
        .Create());

      fixture.Register(() => 
        fixture.Build<WallpaperCategory>()
        .Without((x) => x.Wallpapers)
        .Create());

      fixture.Register(() => new Size(10, 10));

      fixture.Register<WallpaperBase>(fixture.Create<WallpaperBaseImpl>);
      fixture.Register<IWallpaperBase>(fixture.Create<WallpaperBase>);
      fixture.Register<IWallpaper>(fixture.Create<Wallpaper>);
      fixture.Register<IWallpaperCategory>(fixture.Create<WallpaperCategory>);
      fixture.Register<IWallpaperDefaultSettings>(fixture.Create<WallpaperDefaultSettings>);

      fixture.Register<IDisplay>(() =>
        fixture.Build<DisplayStub>()
        .With((x) => x.UniqueDeviceId, string.Concat(@"MONITOR\BNQ78A5\", Guid.NewGuid().ToString(), @"\0004"))
        .With((x) => x.DeviceName, @"\\.\DISPLAY1")
        .Create());

      fixture.Register<IDisplayInfo>(() =>
        fixture.Build<DisplayInfoStub>()
        .With((x) => x.Displays, new ReadOnlyCollection<IDisplay>(fixture.CreateMany<IDisplay>(1).ToList()))
        .With((x) => x.IsMultiDisplaySystem, false)
        .Create());

      fixture.Customizations.Add(new StringSpeciemenBuilder());
      fixture.Customizations.Add(new FullPathSpeciemenBuilder());
    }
  }
}
