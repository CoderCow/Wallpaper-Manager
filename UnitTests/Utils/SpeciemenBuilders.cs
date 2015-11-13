using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace UnitTests {
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
