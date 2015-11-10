using System;
using System.Drawing;
using System.Reflection;
using Common.IO;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using WallpaperManager.Models;
using Xunit;

namespace UnitTests {
  
  public class WallpaperTests {
    [Fact]
    public void CtorSetsImagePath() {
      Fixture fixture = new Fixture();
      fixture.Customize(new WallpaperCustomization());
      
      Wallpaper wallpaper = fixture.Create<Wallpaper>();

      Path testPath = new Path("SomeFile.png");
      Wallpaper wallpaper2 = new Wallpaper(testPath);

      Assert.Equal(wallpaper2.ImagePath, testPath);
    }
  }

  public class WallpaperCustomization: ICustomization {
    public void Customize(IFixture fixture) {
      fixture.Register(() => 
        fixture.Build<Wallpaper>()
        .Without((x) => x.OnlyCycleBetweenStart)
        .Without((x) => x.OnlyCycleBetweenStop));

      fixture.Register(() => new Size(10, 10));

      fixture.Customizations.Add(new StringSpeciemenBuilder());
      fixture.Customizations.Add(new FullPathSpeciemenBuilder());
    }
  }

  public class StringSpeciemenBuilder: ISpecimenBuilder {
    public object Create(object request, ISpecimenContext context) {
      PropertyInfo property = request as PropertyInfo;
      if (property != null && property.PropertyType == typeof(string))
        return property.Name + context.Create<int>();
      else
        return new NoSpecimen(request);
    }
  }

  public class FullPathSpeciemenBuilder: ISpecimenBuilder {
    public object Create(object request, ISpecimenContext context) {
      PropertyInfo property = request as PropertyInfo;
      if (property != null && property.PropertyType == typeof(Path))
        return new Path($"C:\\{property.Name}.ext");
      else
        return new NoSpecimen(request);
    }
  }
}