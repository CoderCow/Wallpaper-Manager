using System;
using System.Globalization;
using System.Windows.Controls;

namespace Common.Presentation {
  /// <summary>
  ///   A rule which checks a <see cref="TimeSpan" /> object in the definable ways.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  public class TimeSpanValidationRule: ValidationRule {
    #region Property: MinValue
    /// <summary>
    ///   <inheritdoc cref="MinValue" select='../value/node()' />
    /// </summary>
    private TimeSpan minValue;

    /// <summary>
    ///   Gets or sets the type of number to be checked for.
    /// </summary>
    /// <value>
    ///   The type of number to be checked for.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is greater than <see cref="MaxValue" />.
    /// </exception>
    public TimeSpan MinValue {
      get { return this.minValue; }
      set {
        if (value > this.MaxValue)
          throw new ArgumentOutOfRangeException("MaxValue");

        this.minValue = value;
      }
    }
    #endregion

    #region Property: MaxValue
    /// <summary>
    ///   <inheritdoc cref="MaxValue" select='../value/node()' />
    /// </summary>
    private TimeSpan maxValue;

    /// <summary>
    ///   Gets or sets the type of number to be checked for.
    /// </summary>
    /// <value>
    ///   The type of number to be checked for.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is lower than <see cref="MinValue" />.
    /// </exception>
    public TimeSpan MaxValue {
      get { return this.maxValue; }
      set {
        if (value < this.MinValue)
          throw new ArgumentOutOfRangeException("MinValue");

        this.maxValue = value;
      }
    }
    #endregion

    #region Methods: Constructor
    /// <summary>
    ///   Initializes static members of the <see cref="NumberValidationRule" /> class.
    /// </summary>
    public TimeSpanValidationRule() {
      this.minValue = TimeSpan.Zero;
      this.maxValue = TimeSpan.MaxValue;
    }
    #endregion

    #region Methods: Validate
    /// <summary>
    ///   Performs validation checks on the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="String" /> or <see cref="TimeSpan" /> value from the binding target to check.
    /// </param>
    /// <param name="cultureInfo">
    ///   The culture to use in this rule.
    /// </param>
    /// <returns>
    ///   A <see cref="ValidationResult" /> object.
    /// </returns>
    public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
      String stringValue = (value as String);
      TimeSpan timeSpanValue;
      
      if (stringValue is String) {
        if (!TimeSpan.TryParse(stringValue, out timeSpanValue)) {
          return new ValidationResult(false, String.Format("The value has an invalid format."));
        }
      } else if (value is TimeSpan) {
        timeSpanValue = (TimeSpan)value;
      } else {
        return new ValidationResult(false, "Invalid type, String or Decimal expected.");
      }

      if (timeSpanValue < this.MinValue) {
        return new ValidationResult(false, String.Format("The time-span must be at greater than {0}.", this.MinValue));
      }

      if (timeSpanValue > this.MaxValue) {
        return new ValidationResult(false, String.Format("The time-span must be at lower than {0}.", this.MaxValue));
      }

      return new ValidationResult(true, null);
    }
    #endregion
  }
}
