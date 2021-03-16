using System;
using System.Globalization;
using System.Windows.Controls;

namespace Common.Presentation {
  /// <summary>
  ///   A rule which checks a number object in the definable ways.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  // TODO: When porting to .NET 4.0, implement a generic type and change MinValue, MaxValue to that gen. type.
  public class NumberValidationRule: ValidationRule {
    #region Property: AllowsRationalNumber
    /// <summary>
    ///   <inheritdoc cref="AllowsRationalNumber" select='../value/node()' />
    /// </summary>
    private Boolean allowsRationalNumber;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether rational numbers are valid or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether rational numbers are valid or not.
    /// </value>
    public Boolean AllowsRationalNumber {
      get { return this.allowsRationalNumber; }
      set { this.allowsRationalNumber = value; }
    }
    #endregion

    #region Property: MinValue
    /// <summary>
    ///   <inheritdoc cref="MinValue" select='../value/node()' />
    /// </summary>
    private Decimal minValue;

    /// <summary>
    ///   Gets or sets the type of number to be checked for.
    /// </summary>
    /// <value>
    ///   The type of number to be checked for.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is greater than <see cref="MaxValue" />.
    /// </exception>
    public Decimal MinValue {
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
    private Decimal maxValue;

    /// <summary>
    ///   Gets or sets the type of number to be checked for.
    /// </summary>
    /// <value>
    ///   The type of number to be checked for.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is lower than <see cref="MinValue" />.
    /// </exception>
    public Decimal MaxValue {
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
    public NumberValidationRule() {
      this.minValue = 0;
      this.maxValue = Decimal.MaxValue;
    }
    #endregion

    #region Methods: Validate
    /// <summary>
    ///   Performs validation checks on the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="String" /> or <see cref="Decimal" /> value from the binding target to check.
    /// </param>
    /// <param name="cultureInfo">
    ///   The culture to use in this rule.
    /// </param>
    /// <returns>
    ///   A <see cref="ValidationResult" /> object.
    /// </returns>
    public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
      String stringValue = (value as String);
      Decimal decimalValue;
      
      if (stringValue is String) {
        if (!Decimal.TryParse(stringValue, out decimalValue)) {
          return new ValidationResult(false, String.Format("The value has an invalid format."));
        }
      } else if (value is Decimal) {
        decimalValue = (Decimal)value;
      } else {
        return new ValidationResult(false, "Invalid type, String or Decimal expected.");
      }

      if (decimalValue < this.MinValue) {
        return new ValidationResult(false, String.Format("The value must be at greater than {0}.", this.MinValue));
      }

      if (decimalValue > this.MaxValue) {
        return new ValidationResult(false, String.Format("The value must be at lower than {0}.", this.MaxValue));
      }

      return new ValidationResult(true, null);
    }
    #endregion
  }
}
