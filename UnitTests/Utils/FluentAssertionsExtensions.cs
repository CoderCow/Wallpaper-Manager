using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace UnitTests {
  internal static class FluentAssertionsExtensions {
    public static void BePropertyValueEqual(this ObjectAssertions assertions, object other, ICollection<string> excludedProperties = null) {
      Contract.Requires<ArgumentNullException>(other != null);

      Action<PropertyInfo, object, object> comparator = (propertyInfo, subjectPropertyValue, otherPropertyValue) => {
        if (excludedProperties != null && excludedProperties.Contains(propertyInfo.Name))
          return;
        if (!subjectPropertyValue.Equals(otherPropertyValue))
          throw new Exception($"The properties \"{propertyInfo.Name}\" did not return equal values.");
      };
      CompareProperties(assertions, other, comparator);
    }

    public static void BeCloneOf(this ObjectAssertions assertions, object other, ICollection<string> excludedProperties = null) {
      Contract.Requires<ArgumentNullException>(other != null);

      Action<PropertyInfo, object, object> comparator = (propertyInfo, subjectPropertyValue, otherPropertyValue) => {
        Contract.Assert(propertyInfo != null);

        if (excludedProperties != null && excludedProperties.Contains(propertyInfo.Name))
          return;
        bool isSimpleType = propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string);
        bool areEqual = subjectPropertyValue.Equals(otherPropertyValue);
        if (isSimpleType && !areEqual)
          throw new Exception($"The properties \"{propertyInfo.Name}\" did not return equal values.");
        else if (!isSimpleType && areEqual)
          throw new Exception($"The properties \"{propertyInfo.Name}\" are referencing the same instance.");
      };
      CompareProperties(assertions, other, comparator);
    }

    private static void CompareProperties(ObjectAssertions assertions, object other, Action<PropertyInfo, object, object> comparator) {
      object subject = assertions.Subject;
      Type subjectType = subject.GetType();

      foreach (PropertyInfo property in subjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
        if (property.IsIndexer())
          continue;

        object subjectPropertyValue = property.GetValue(subject);
        object otherPropertyValue = property.GetValue(other);
        comparator(property, subjectPropertyValue, otherPropertyValue);
      }
    }
  }
}
