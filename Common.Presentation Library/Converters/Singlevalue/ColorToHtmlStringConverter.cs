using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Presentation {
  /// <summary>
  ///   Converts a <see cref="Color" /> value to and from a <see cref="String" /> value.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(Color), typeof(String))]
  public class ColorToHtmlStringConverter: IValueConverter {
    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="Color" /> value to a <see cref="String" /> value.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if ((value != null) && (value is Color)) {
        return ColorTranslator.ToHtml((Color)value);
      }

      return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///   Converts a <see cref="String" /> value to a <see cref="Color" /> value.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      String convValue = (value as String);

      if (convValue != null) {
        return ColorTranslator.FromHtml(convValue);
      }

      return DependencyProperty.UnsetValue;
    }
    #endregion
  }
}
