using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;
using Ploeh.AutoFixture;
using UnitTests.Models;
using WallpaperManager.Models;
using Path = Common.IO.Path;

namespace UnitTests {
  public class WallpaperCustomization: ICustomization {
    public void Customize(IFixture fixture) {
      new EssentialCustomizations().Customize(fixture);

      fixture.Register(() => 
        fixture.Build<WallpaperBase>()
        .Without((x) => x.OnlyCycleBetweenStart)
        .Without((x) => x.OnlyCycleBetweenStop)
        .Without((x) => x.BackgroundColor)
        .Create());

      fixture.Register(() => 
        fixture.Build<Wallpaper>()
        .With((x) => x.CycleCountTotal, 100)
        .With((x) => x.CycleCountWeek, 10)
        .With((x) => x.TimeAdded, new DateTime(1999, 1, 1))
        .With((x) => x.TimeLastCycled, new DateTime(2000, 1, 1))
        .Without((x) => x.OnlyCycleBetweenStart)
        .Without((x) => x.OnlyCycleBetweenStop)
        .Without((x) => x.BackgroundColor)
        .Create());

      fixture.Register(() => 
        fixture.Build<AppDataReaderWriterFake>()
        .With((x) => x.UseOutputDataAsInput, true)
        .Create());

      fixture.Register(() =>
        fixture.Build<DisplayStub>()
        .With((x) => x.UniqueDeviceId, string.Concat(@"MONITOR\BNQ78A5\", Guid.NewGuid().ToString(), @"\0004"))
        .With((x) => x.DeviceName, @"\\.\DISPLAY1")
        .Create());

      fixture.Register(() =>
        fixture.Build<DisplayInfoStub>()
        .With((x) => x.Displays, new ReadOnlyCollection<IDisplay>(fixture.CreateMany<IDisplay>(1).ToList()))
        .With((x) => x.IsMultiDisplaySystem, false)
        .Create());

      fixture.Customizations.Add(new StringSpeciemenBuilder());
    }
  }

  public class EssentialCustomizations: ICustomization {
    private static readonly Lazy<Path> existingFilePath = new Lazy<Path>(() => new Path(System.IO.Path.GetTempFileName()), true);

    public void Customize(IFixture fixture) {
      fixture.Register<IApplicationData>(fixture.Create<ApplicationData>);
      fixture.Register<IConfiguration>(fixture.Create<Configuration>);
      fixture.Register<IWallpaperBase>(fixture.Create<WallpaperBase>);
      fixture.Register<IWallpaper>(fixture.Create<Wallpaper>);
      fixture.Register<IWallpaperCategory>(fixture.Create<WallpaperCategory>);
      fixture.Register<ISyncedWallpaperCategory>(fixture.Create<SyncedWallpaperCategory>);
      fixture.Register<IWallpaperDefaultSettings>(fixture.Create<WallpaperDefaultSettings>);
      fixture.Register<IDisplay>(() => fixture.Create<DisplayStub>());
      fixture.Register<IDisplayInfo>(() => fixture.Create<DisplayInfoStub>());
      fixture.Register<IScreenSettings>(fixture.Create<ScreenSettings>);

      fixture.Register(() => existingFilePath.Value);
      fixture.Register(() => new Size(10, 10));
    }
  }
}
