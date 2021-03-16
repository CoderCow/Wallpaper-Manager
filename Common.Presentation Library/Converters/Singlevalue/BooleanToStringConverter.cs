using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Presentation {
  /// <summary>
  ///   Converts the defined strings to and from <see cref="Boolean" />.
  /// </summary>
  /// <remarks>
  ///   The string case will be ignored due the comparision.
  /// </remarks>
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(String), typeof(Boolean))]
  public class BooleanToStringConverter: IValueConverter {
    #region Constants and Fields
    /// <summary>
    ///   Represents the string representation for the <c>true</c> value.
    /// </summary>
    private const String DefaultTrueString = "True";

    /// <summary>
    ///   Represents the string representation for the <c>false</c> value.
    /// </summary>
    private const String DefaultFalseString = "False";

    /// <summary>
    ///   <inheritdoc cref="TrueString" select='../value/node()' />
    /// </summary>
    private String trueString = BooleanToStringConverter.DefaultTrueString;

    /// <summary>
    ///   <inheritdoc cref="FalseString" select='../value/node()' />
    /// </summary>
    private String falseString = BooleanToStringConverter.DefaultFalseString;
    #endregion

    #region Properties
    /// <summary>
    ///   Gets or sets the string representation of <c>true</c>.
    /// </summary>
    /// <value>
    ///   The string representation of <c>true</c>.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempt to set a <c>null</c> value.
    /// </exception>
    public String TrueString {
      get { return this.trueString; }
      set {
        if (value == null)
          throw new ArgumentNullException();

        this.trueString = value;
      }
    }

    /// <summary>
    ///   Gets or sets the string representation of <c>false</c>.
    /// </summary>
    /// <value>
    ///   The string representation of <c>false</c>.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempt to set a <c>null</c> value.
    /// </exception>
    public String FalseString {
      get { return this.falseString; }
      set {
        if (value == null)
          throw new ArgumentNullException();

        this.falseString = value;
      }
    }
    #endregion

    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="Boolean" /> value to <see cref="String" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is Boolean) {
        Boolean convValue = (Boolean)value;

        if (convValue) {
          return this.TrueString;
        } else {
          return this.FalseString;
        }
      }

      return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///   Converts a <see cref="String" /> value to <see cref="Boolean" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is String) {
        // Convert to lower cased strings first, because we want to ignore the case when comparing 
        // the strings.
        String convTrueString = this.TrueString.ToLower();
        String convFalseString = this.FalseString.ToLower();
        String convValue = ((String)value).ToLower();

        if (convValue == convTrueString) {
          return true;
        } else {
          if (convValue == convFalseString) {
            return false;
          }
        }
      }

      return DependencyProperty.UnsetValue;
    }
    #endregion
  }
}