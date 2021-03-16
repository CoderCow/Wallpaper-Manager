using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Presentation {
  /// <summary>
  ///   Applies the <see cref="OperationKind" /> with the given value using the given parameter and returns the operation's 
  ///   result.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(Decimal), typeof(Decimal), ParameterType = typeof(Decimal))]
  public class ArithmeticOperationConverter: IValueConverter {
    #region Property: OperationKind
    /// <summary>
    ///   <inheritdoc cref="OperationKind" select='../value/node()' />
    /// </summary>
    private ArithmeticOperation operationKind;

    /// <summary>
    ///   Gets or sets the kind of arithmetic operation.
    /// </summary>
    /// <value>
    ///   The kind of arithmetic operation.
    /// </value>
    public ArithmeticOperation OperationKind {
      get { return this.operationKind; }
      set { this.operationKind = value; }
    }
    #endregion


    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Applies the <see cref="OperationKind" /> with the given <paramref name="value" /> using the given 
    ///   <paramref name="parameter" />and returns the operation's result.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      Decimal decimalValue = System.Convert.ToDecimal(value, culture);
      Decimal decimalParam = System.Convert.ToDecimal(parameter, culture);
      
      switch (this.OperationKind) {
        case ArithmeticOperation.Addition:
          return decimalValue + decimalParam;
        case ArithmeticOperation.Subtraction:
          return decimalValue - decimalParam;
        case ArithmeticOperation.Multiplication:
          return decimalValue * decimalParam;
        case ArithmeticOperation.Division:
          return decimalValue / decimalParam;
        default:
          return DependencyProperty.UnsetValue;
      }
    }

    /// <summary>
    ///   Applies the reverse <see cref="OperationKind" /> with the given <paramref name="value" /> using the given 
    ///   <paramref name="parameter" />and returns the operation's result.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      Decimal decimalValue = System.Convert.ToDecimal(value, culture);
      Decimal decimalParam = System.Convert.ToDecimal(parameter, culture);

      // We have to do the reversed operation here.
      switch (this.OperationKind) {
        case ArithmeticOperation.Addition:
          return decimalValue - decimalParam;
        case ArithmeticOperation.Subtraction:
          return decimalValue + decimalParam;
        case ArithmeticOperation.Multiplication:
          return decimalValue / decimalParam;
        case ArithmeticOperation.Division:
          return decimalValue * decimalParam;
        default:
          return DependencyProperty.UnsetValue;
      }
    }
    #endregion
  }
}
