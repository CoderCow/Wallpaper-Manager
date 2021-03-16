using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common.Presentation.Controls {
  /// <summary>
  ///   Extended <see cref="TextBox" /> which allows to change the value using mouse-dragging.
  /// </summary>
  /// <typeparam name="ValueType">
  ///   The type of the containing value.
  /// </typeparam>
  /// <inheritdoc />
  public abstract class DragableTextBoxBase<ValueType>: TextBox where ValueType: IComparable {
    #region Constants and Fields
    /// <summary>
    ///   Identifies the <see cref="MinValue" /> dependency property. 
    /// </summary>
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
      "MinValue", 
      typeof(ValueType), 
      typeof(DragableTextBoxBase<ValueType>), 
      new PropertyMetadata(
        DragableTextBoxBase<ValueType>.OnMinValueChanged
      )
    );
    
    /// <summary>
    ///   Identifies the <see cref="MaxValue" /> dependency property. 
    /// </summary>
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
      "MaxValue", 
      typeof(ValueType), 
      typeof(DragableTextBoxBase<ValueType>), 
      new PropertyMetadata(
        DragableTextBoxBase<ValueType>.OnMaxValueChanged
      )
    );

    /// <summary>
    ///   Identifies the <see cref="DragValueSmall" /> dependency property. 
    /// </summary>
    public static readonly DependencyProperty DragValueSmallProperty = DependencyProperty.Register(
      "DragValueSmall", typeof(ValueType), typeof(DragableTextBoxBase<ValueType>)
    );

    /// <summary>
    ///   Identifies the <see cref="DragValueLarge" /> dependency property. 
    /// </summary>
    public static readonly DependencyProperty DragValueLargeProperty = DependencyProperty.Register(
      "DragValueLarge", typeof(ValueType), typeof(DragableTextBoxBase<ValueType>)
    );

    /// <summary>
    ///   <inheritdoc cref="Value" select='../value/node()' />
    /// </summary>
    private ValueType value;

    /// <summary>
    ///   <inheritdoc cref="AllowedInputExpression" select='../value/node()' />
    /// </summary>
    private Regex allowedInputExpression;

    /// <summary>
    ///   The last position of the current drag process.
    /// </summary>
    private Point lastDragPoint;

    /// <summary>
    ///   <c>true</c> if a mouse dragging is actually in process; otherwise <c>false</c>.
    /// </summary>
    private Boolean isDragging;
    #endregion

    #region Events and Properties
    /// <summary>
    ///   Gets or sets the current value.
    /// </summary>
    /// <value>
    ///   The current value.
    /// </value>
    public virtual ValueType Value {
      get { return this.value; }
      protected set {
        this.value = value;

        if (value.CompareTo(this.MaxValue) > 0) {
          this.value = this.MaxValue;
        }

        if (value.CompareTo(this.MinValue) < 0) {
          this.value = this.MinValue;
        }
      }
    }

    /// <summary>
    ///   Gets or sets the minimum allowed value for this text box.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The minimum allowed value for this text box.
    /// </value>
    [Bindable(true)]
    public ValueType MinValue {
      get { return (ValueType)this.GetValue(DragableTextBoxBase<ValueType>.MinValueProperty); }
      set { this.SetValue(DragableTextBoxBase<ValueType>.MinValueProperty, value); }
    }

    /// <summary>
    ///   Gets or sets the maximum allowed value for this text box.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The maximum allowed value for this text box.
    /// </value>
    [Bindable(true)]
    public ValueType MaxValue {
      get { return (ValueType)this.GetValue(DragableTextBoxBase<ValueType>.MaxValueProperty); }
      set { this.SetValue(DragableTextBoxBase<ValueType>.MaxValueProperty, value); }
    }

    /// <summary>
    ///   Gets or sets the small value with which <see cref="Value" /> is changed when dragging.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The small value with which <see cref="Value" /> is changed when dragging.
    /// </value>
    [Bindable(true)]
    public ValueType DragValueSmall {
      get { return (ValueType)this.GetValue(DragableTextBoxBase<ValueType>.DragValueSmallProperty); }
      set { this.SetValue(DragableTextBoxBase<ValueType>.DragValueSmallProperty, value); }
    }

    /// <summary>
    ///   Gets or sets the large value with which <see cref="Value" /> is changed when dragging.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The small value with which <see cref="Value" /> is changed when dragging.
    /// </value>
    [Bindable(true)]
    public ValueType DragValueLarge {
      get { return (ValueType)this.GetValue(DragableTextBoxBase<ValueType>.DragValueLargeProperty); }
      set { this.SetValue(DragableTextBoxBase<ValueType>.DragValueLargeProperty, value); }
    }

    /// <summary>
    ///   Gets or sets the regular expression for the allowed input.
    /// </summary>
    /// <value>
    ///   The regular expression for the allowed input.
    /// </value>
    protected Regex AllowedInputExpression {
      get { return this.allowedInputExpression; }
      set { this.allowedInputExpression = value; }
    }
    #endregion

    #region Routed Event: DragStart
    /// <summary>
    ///   Identifies the <see cref="DragStart" /> <see cref="RoutedEvent">Routed Event</see>.
    /// </summary>
    public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent(
      "DragStart", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragableTextBoxBase<ValueType>)
    );

    /// <summary>
    ///   Occurs when the user starts a drag process.
    /// </summary>
    public event RoutedEventHandler DragStart {
      add { this.AddHandler(DragableTextBoxBase<ValueType>.DragStartEvent, value); } 
      remove { this.RemoveHandler(DragableTextBoxBase<ValueType>.DragStartEvent, value); }
    }

    /// <summary>
    ///  Invoked when an unhandled <see cref="DragStart" /> <see cref="RoutedEvent">Routed Event</see> reaches an element 
    ///  in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    protected virtual void OnDragStartEvent() {
      this.RaiseEvent(new RoutedEventArgs(DragableTextBoxBase<ValueType>.DragStartEvent));
    }
    #endregion

    #region Routed Event: DragEnd
    /// <summary>
    ///   Identifies the <see cref="DragEnd" /> <see cref="RoutedEvent">Routed Event</see>.
    /// </summary>
    public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent(
      "DragEnd", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragableTextBoxBase<ValueType>)
    );

    /// <summary>
    ///   Occurs when the user ends a drag process.
    /// </summary>
    public event RoutedEventHandler DragEnd {
      add { this.AddHandler(DragableTextBoxBase<ValueType>.DragEndEvent, value); } 
      remove { this.RemoveHandler(DragableTextBoxBase<ValueType>.DragEndEvent, value); }
    }

    /// <summary>
    ///  Invoked when an unhandled <see cref="DragEnd" /> <see cref="RoutedEvent">Routed Event</see> reaches an element 
    ///  in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    protected virtual void OnDragEndEvent() {
      this.RaiseEvent(new RoutedEventArgs(DragableTextBoxBase<ValueType>.DragEndEvent));
    }
    #endregion


    #region Methods
    static DragableTextBoxBase() {
      DragableTextBoxBase<ValueType>.TextProperty.OverrideMetadata(
        typeof(DragableTextBoxBase<ValueType>), 
        new FrameworkPropertyMetadata(String.Empty, DragableTextBoxBase<ValueType>.TextPropertyChanged)
      );
    }

    private static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      DragableTextBoxBase<ValueType> instance = (DragableTextBoxBase<ValueType>)sender;
      ValueType newValue;
        
      if (instance.TryParseFromString((String)e.NewValue, out newValue)) {
        instance.value = newValue;
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DragableTextBoxBase&lt;ValueType&gt;"/> class.
    /// </summary>
    protected DragableTextBoxBase() {
      // This updates the validation expression.
      this.Cursor = Cursors.ScrollAll;
      this.isDragging = false;

      DataObject.AddPastingHandler(
        this, 
        new DataObjectPastingEventHandler((sender, e) => {
          if (e.DataObject.GetDataPresent(typeof(String))) {
            String data = e.DataObject.GetData(typeof(String)).ToString();

            if (this.allowedInputExpression.IsMatch(data)) {
              ValueType newValue;

              if (this.TryParseFromString(data, out newValue)) {
                this.value = newValue;
              }

              return;
            }
          }

          e.CancelCommand();
        })
      );
    }

    /// <inheritdoc />
    protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
      e.Handled = !this.allowedInputExpression.IsMatch(e.Text);
      
      base.OnPreviewTextInput(e);
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseButtonEventArgs e) {
      if (e.ChangedButton == MouseButton.Left) {
        this.lastDragPoint = e.GetPosition(this);
        this.Cursor = Cursors.ScrollAll;
        this.isDragging = true;

        this.OnDragStartEvent();
      }

      base.OnMouseDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed) {
        Point mousePosition = e.GetPosition(this);

        // The difference between the last and the current drag position.
        Double usedDragDifference;
        Point dragDifference = new Point(
          mousePosition.X - this.lastDragPoint.X,
          mousePosition.Y - this.lastDragPoint.Y
        );
        
        // Use the value of the greater axis.
        if (Math.Abs(dragDifference.X) > Math.Abs(dragDifference.Y)) {
          usedDragDifference = dragDifference.X;
        } else {
          // We want to drag along the X-Axis but against the Y-Axis.
          usedDragDifference = -dragDifference.Y;
        }
         
        if (Math.Abs(usedDragDifference) >= 5) {
          ValueType dragValue = this.DragValueSmall;

          if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
            dragValue = this.DragValueLarge;
          }

          if (usedDragDifference < 0) {
            this.DecreaseValue(dragValue);
          } else {
            this.IncreaseValue(dragValue);
          }

          this.lastDragPoint = mousePosition;

          // This will hide the caret of the TextBox.
          this.IsReadOnly = true;
        }

        // Do NOT call the base method here because we don't want the selection of the TextBox to be
        // changed whenever the user is actually dragging.
        return;
      }

      base.OnMouseMove(e);
    }

    /// <inheritdoc />
    protected override void OnMouseUp(MouseButtonEventArgs e) {
      if (e.ChangedButton == MouseButton.Left) {
        this.Cursor = Cursors.IBeam;
        this.isDragging = false;
        this.IsReadOnly = false;

        this.OnDragEndEvent();
      }

      base.OnMouseUp(e);
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e) {
      this.Cursor = Cursors.ScrollAll;
      
      base.OnMouseEnter(e);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      ValueType dragValue = this.DragValueSmall;

      if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
        dragValue = this.DragValueLarge;
      }

      if (e.Key == Key.Up) {
        this.IncreaseValue(dragValue);
      } else if (e.Key == Key.Down) {
        this.DecreaseValue(dragValue);
      }

      base.OnKeyDown(e);
    }

    /// <summary>
    ///   Converts a string to the given <typeparamref name="ValueType" />.
    /// </summary>
    /// <param name="stringValue">
    ///   The string value.
    /// </param>
    /// <returns>
    ///   The converted <typeparamref name="ValueType" />.
    /// </returns>
    protected abstract Boolean TryParseFromString(String stringValue, out ValueType value);

    /// <summary>
    ///   Increases the value by the given amount.
    /// </summary>
    /// <param name="increaseBy">
    ///   The amount of how much the value should be increased.
    /// </param>
    protected abstract void IncreaseValue(ValueType increaseBy);

    /// <summary>
    ///   Decreases the value by the given amount.
    /// </summary>
    /// <param name="decreaseBy">
    ///   The amount of how much the value should be decreased.
    /// </param>
    protected abstract void DecreaseValue(ValueType decreaseBy);

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
      DragableTextBoxBase<ValueType> @this = (DragableTextBoxBase<ValueType>)sender;

      /*if (@this.Value.CompareTo(e.NewValue) < 0) {
        @this.Value = (ValueType)e.NewValue;
      }*/
    }

    /// <summary>
    ///   Occurs when <see cref="MaxValue" /> has changed.
    ///   Sets <see cref="Value" /> to <see cref="MaxValueProperty" /> if the new max value is too large.
    /// </summary>
    /// <param name="sender">
    ///   The object of which the property has changed.
    /// </param>
    /// <param name="e">
    ///   The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.
    /// </param>
    private static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      DragableTextBoxBase<ValueType> @this = (DragableTextBoxBase<ValueType>)sender;

      /*if (@this.Value.CompareTo(e.NewValue) > 0) {
        @this.Value = (ValueType)e.NewValue;
      }*/
    }
    #endregion
  }
}
