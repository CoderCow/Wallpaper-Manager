using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace UnitTests {
  public class AutoValidEnumData: DataAttribute {
    private readonly Array enumValues;

    public AutoValidEnumData(Type enumType) {
      this.enumValues = enumType.GetEnumValues();
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod) {
      foreach(object value in this.enumValues)
        yield return new[] { value };
    }
  }
}