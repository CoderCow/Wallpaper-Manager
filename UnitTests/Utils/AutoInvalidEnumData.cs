using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Xunit2;
using Xunit.Sdk;

namespace UnitTests {
  public class AutoInvalidEnumData: DataAttribute {
    private readonly int min;
    private readonly int max;

    public AutoInvalidEnumData(Type enumType) {
      this.min = int.MinValue;
      this.max = int.MaxValue;

      foreach (int value in enumType.GetEnumValues()) {
        this.min = Math.Min(value, min);
        this.max = Math.Max(value, max);
      }
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod) {
      yield return new object[] { this.min - 1 };
      yield return new object[] { this.max + 1 };
    }
  }
}
