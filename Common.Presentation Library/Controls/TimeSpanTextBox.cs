using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace Common.Presentation.Controls {
  /// <inheritdoc />
  public class TimeSpanTextBox: DragableTextBoxBase<TimeSpan> {
    #region Events and Properties
    /// <inheritdoc />
    public override TimeSpan Value {
      get { return base.Value; }
      protected set {
        base.Value = value;

        this.Text = this.Value.ToString();
      }
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Initializes static members of the <see cref="TimeSpanTextBox" /> class.
    /// </summary>
    static TimeSpanTextBox() {
      //TimeTextBox.TextProperty.OverrideMetadata(
      //  typeof(TimeTextBox), new FrameworkPropertyMetadata("00:00:00")
      //);

      TimeSpanTextBox.MinValueProperty.OverrideMetadata(
        typeof(TimeSpanTextBox), new PropertyMetadata(new TimeSpan(0, 0, 0))
      );
      TimeSpanTextBox.MaxValueProperty.OverrideMetadata(
        typeof(TimeSpanTextBox), new PropertyMetadata(new TimeSpan(23, 59, 59))
      );

      TimeSpanTextBox.DragValueSmallProperty.OverrideMetadata(
        typeof(TimeSpanTextBox), new PropertyMetadata(new TimeSpan(0, 1, 0))
      );
      TimeSpanTextBox.DragValueLargeProperty.OverrideMetadata(
        typeof(TimeSpanTextBox), new PropertyMetadata(new TimeSpan(0, 10, 0))
      );
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="TimeSpanTextBox" /> class.
    /// </summary>
    public TimeSpanTextBox() {
      this.AllowedInputExpression = new Regex("[0-9:.]+");
    }

    /// <inheritdoc />
    protected override Boolean TryParseFromString(String stringValue, out TimeSpan value) {
      return TimeSpan.TryParse(stringValue, out value);
    }

    /// <inheritdoc />
    protected override void IncreaseValue(TimeSpan increaseBy) {
      this.Value += increaseBy;
    }

    /// <inheritdoc />
    protected override void DecreaseValue(TimeSpan decreaseBy) {
      this.Value -= decreaseBy;
    }
    #endregion
  }
}
