using System;
using System.Globalization;
using System.Windows.Data;

namespace Common.Presentation {
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(Object), typeof(Boolean), ParameterType = typeof(Object))]
  public class IsValueAsBooleanConverter: IValueConverter {
    #region Property: IsNot
    /// <summary>
    ///   <inheritdoc cref="IsNot" select='../value/node()' />
    /// </summary>
    private Boolean isNot = false;
    
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the value should be checked for non-equality.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the value should be checked for non-equality.
    /// </value>
    public Boolean IsNot {
      get { return this.isNot; }
      set { this.isNot = value; }
    }
    #endregion

    #region IValueConverter Implementation
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) {
        return (value == parameter);
      }

      Boolean equals = false;
      String stringParam = (parameter as String);
      if (stringParam != null) {
        Boolean boolParam;
        Int32 intParam;
        if (Boolean.TryParse(stringParam, out boolParam)) {
          equals = value.Equals(boolParam);
        } else if (Int32.TryParse(stringParam, out intParam)) {
          equals = value.Equals(intParam);
        }
      } else {
        equals = (value.Equals(parameter));
      }
      
      if (this.IsNot) {
        return !equals;
      } else {
        return equals;
      }
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      return parameter;
    }
    #endregion
  }
}
