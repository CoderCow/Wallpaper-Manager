using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Common.Presentation.Controls {
  /// <inheritdoc />
  public class NumericTextBox: DragableTextBoxBase<Double> {
    #region Constants and Fields
    /// <summary>
    ///   <inheritdoc cref="AllowsRationalNumber" select='../value/node()' />
    /// </summary>
    private Boolean allowsRationalNumber;
    #endregion

    #region Events and Properties
    /// <summary>
    ///   Gets or sets a value indicating whether this TextBox accepts rational numbers.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this TextBox accepts rational numbers; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowsRationalNumber {
      get { return this.allowsRationalNumber; }
      set {
        this.allowsRationalNumber = value;
        this.UpdateInputExpression();
      }
    }

    /// <inheritdoc />
    public override Double Value {
      get { return base.Value; }
      protected set {
        base.Value = value;

        this.Text = this.Value.ToString();
      }
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Initializes static members of the <see cref="NumericTextBox" /> class.
    /// </summary>
    static NumericTextBox() {
      NumericTextBox.MinValueProperty.OverrideMetadata(
        typeof(NumericTextBox), new PropertyMetadata(0.00, NumericTextBox.OnMinValueChanged)
      );
      NumericTextBox.MaxValueProperty.OverrideMetadata(
        typeof(NumericTextBox), new PropertyMetadata(Double.MaxValue)
      );

      NumericTextBox.DragValueSmallProperty.OverrideMetadata(
        typeof(NumericTextBox), new PropertyMetadata(1.00)
      );
      NumericTextBox.DragValueLargeProperty.OverrideMetadata(
        typeof(NumericTextBox), new PropertyMetadata(10.00)
      );
    }

    /// <summary>
    ///   Occurs when <see cref="MinValue" /> has changed.
    ///   Sets <see cref="Value" /> to <see cref="MinValueProperty" /> if the new min value is too small.
    /// </summary>
    /// <param name="sender">
    ///   The object of which the property has changed.
    /// </param>
    /// <param name="e">
    ///   The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.
    /// </param>
    private static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      NumericTextBox instance = (NumericTextBox)sender;

      instance.UpdateInputExpression();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericTextBox" /> class.
    /// </summary>
    public NumericTextBox() {
      // This updates the validation expression.
      this.AllowsRationalNumber = false;
      this.Cursor = Cursors.ScrollAll;
    }

    /// <inheritdoc />
    protected override Boolean TryParseFromString(String stringValue, out Double value) {
      return Double.TryParse(stringValue, out value);
    }

    /// <inheritdoc />
    protected override void IncreaseValue(Double increaseBy) {
      this.Value += increaseBy;
    }

    /// <inheritdoc />
    protected override void DecreaseValue(Double decreaseBy) {
      this.Value -= decreaseBy;
    }

    protected virtual void UpdateInputExpression() {
      if (this.AllowsRationalNumber) {
        // Allows integer and rational numbers.
        // Note: This expression is not localized.
        if (this.MinValue < 0) {
          this.AllowedInputExpression = new Regex(@"[0-9.\-]+");
        } else {
          this.AllowedInputExpression = new Regex(@"[0-9.]+");
        }
      } else {
        // Allows numbers only.
        if (this.MinValue < 0) {
          this.AllowedInputExpression = new Regex(@"[0-9\-]+");
        } else {
          this.AllowedInputExpression = new Regex(@"[0-9]+");
        }
      }
    }
    #endregion
  }
}
