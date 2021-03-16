using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Presentation {
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(Boolean), typeof(Boolean))]
  public class NotBooleanConverter: IValueConverter {
    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <c>true</c> value to a <c>false</c> value and other way round.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is Boolean) {
        return (!(Boolean)value);
      }

      if (value is Boolean?) {
        return (!((Boolean?)value).Value);
      }

      return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///   Converts a <c>true</c> value to a <c>false</c> value and other way round.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      return this.Convert(value, targetType, parameter, culture);
    }
    #endregion
  }
}
