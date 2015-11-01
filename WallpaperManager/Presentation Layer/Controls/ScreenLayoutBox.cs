// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Drawing = System.Drawing;
using Screen = System.Windows.Forms.Screen;

using Common.Drawing;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Displays all monitors in a relative aligned layout.
  /// </summary>
  [ContentProperty("ScreenBackground")]
  public class ScreenLayoutBox: UserControl {
    #region Fields: screens, screensBackground
    /// <summary>
    ///   The containing radio buttons (screens).
    /// </summary>
    private readonly List<RadioButton> screens;

    /// <summary>
    ///   The background rectangle behind the radio buttons (screens).
    /// </summary>
    private readonly Rectangle screensBackground;
    #endregion

    #region Static Property: ScreenRadioButtonStyleKey
    /// <inheritdoc cref="ScreenRadioButtonStyleKey" select='../value/node()' />
    private static readonly ResourceKey screenRadioButtonStyleKey = new ComponentResourceKey(
      typeof(ScreenLayoutBox), "ScreenRadioButtonStyleKey"
    );

    /// <summary>
    ///   Gets the <see cref="Style" /> applied to screen borders.
    /// </summary>
    /// <value>
    ///   A resource key that represents the default style for the screen borders.
    /// </value>
    public static ResourceKey ScreenRadioButtonStyleKey {
      get { return ScreenLayoutBox.screenRadioButtonStyleKey; }
    }
    #endregion

    #region Static Property: ScreenCheckBoxStyleKey
    /// <inheritdoc cref="ScreenCheckBoxStyleKey" select='../value/node()' />
    private static readonly ResourceKey screenCheckBoxStyleKey = new ComponentResourceKey(
      typeof(ScreenLayoutBox), "ScreenCheckBoxStyleKey"
    );

    /// <summary>
    ///   Gets the <see cref="Style" /> applied to screen check boxes.
    /// </summary>
    /// <value>
    ///   A resource key that represents the default style for the screen check boxes.
    /// </value>
    public static ResourceKey ScreenCheckBoxStyleKey {
      get { return ScreenLayoutBox.screenCheckBoxStyleKey; }
    }
    #endregion

    #region Dependency Property: SelectedScreenIndex
    /// <summary>
    ///   Identifies the <see cref="SelectedScreenIndex" /> Dependency Property. 
    /// </summary>
    public static readonly DependencyProperty SelectedScreenIndexProperty = DependencyProperty.Register(
      "SelectedScreenIndex", 
      typeof(Int32), 
      typeof(ScreenLayoutBox), 
      new FrameworkPropertyMetadata(
        0, FrameworkPropertyMetadataOptions.AffectsRender, ScreenLayoutBox.OnSelectedScreenIndexChanged
      )
    );

    /// <summary>
    ///   Gets or sets the index of the selected screen.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The index of the selected screen.
    /// </value>
    [Bindable(true)]
    public Int32 SelectedScreenIndex {
      get { return (Int32)this.GetValue(ScreenLayoutBox.SelectedScreenIndexProperty); }
      set { this.SetValue(ScreenLayoutBox.SelectedScreenIndexProperty, value); }
    }

    /// <commondoc select='DependencyObject/Methods/DependencyPropertyChangedHandler/*' params='Property=SelectedScreenIndex' />
    private static void OnSelectedScreenIndexChanged(
      DependencyObject sender, DependencyPropertyChangedEventArgs e
    ) {
      ScreenLayoutBox @this = (ScreenLayoutBox)sender;
      Int32 value = (Int32)e.NewValue;
      
      if ((value > 0) && (value < @this.screens.Count)) {
        @this.screens[value].IsChecked = true;
      }
    }
    #endregion

    #region Dependency Property: ScreenBackground
    /// <summary>
    ///   Identifies the <see cref="ScreenBackground" /> dependency property. 
    /// </summary>
    public static readonly DependencyProperty ScreenBackgroundProperty = DependencyProperty.Register(
      "ScreenBackground", 
      typeof(ImageSource), 
      typeof(ScreenLayoutBox), 
      new FrameworkPropertyMetadata(
        null, FrameworkPropertyMetadataOptions.AffectsRender, ScreenLayoutBox.OnScreenBackgroundChanged
      )
    );

    /// <summary>
    ///   Gets or sets the background brush behind all screens.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The background brush behind all screens.
    /// </value>
    [Bindable(true)]
    public ImageSource ScreenBackground {
      get { return (ImageSource)this.GetValue(ScreenLayoutBox.ScreenBackgroundProperty); }
      set { this.SetValue(ScreenLayoutBox.ScreenBackgroundProperty, value); }
    }

    /// <commondoc select='DependencyObject/Methods/DependencyPropertyChangedHandler/*' params='Property=ScreenBackground' />
    private static void OnScreenBackgroundChanged(
      DependencyObject sender, DependencyPropertyChangedEventArgs e
    ) {
      ((ScreenLayoutBox)sender).screensBackground.Fill = new ImageBrush((ImageSource)e.NewValue);
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Controls.Primitives.ToggleButton.Checked" /> event of a 
    ///   <see cref="System.Windows.Controls.Primitives.ToggleButton" /> instance and updates the 
    ///   <see cref="SelectedScreenIndex" /> dependency property.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Screen_Checked(Object sender, RoutedEventArgs e) {
      RadioButton screen = (sender as RadioButton);

      if ((screen != null) && (screen.IsChecked != null) && (screen.IsChecked.Value)) {
        this.SelectedScreenIndex = this.screens.IndexOf(screen);
      }
    }
    #endregion

    #region Dependency Property: ScreensCheckStatus
    /// <summary>
    ///   Identifies the <see cref="ScreensCheckStatus" /> Dependency Property.
    /// </summary>
    public static readonly DependencyProperty ScreensCheckStatusProperty = DependencyProperty.Register(
      "ScreensCheckStatus", 
      typeof(ObservableCollection<Boolean?>), 
      typeof(ScreenLayoutBox), 
      new FrameworkPropertyMetadata(
        null, FrameworkPropertyMetadataOptions.AffectsRender, ScreenLayoutBox.OnScreensCheckStatusChanged, null, true
      )
    );
    
    /// <summary>
    ///   Gets or sets the collection containing the check status of the screen check boxes.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The collection containing the check status of the screen check boxes.
    /// </value>
    [Bindable(true)]
    public ObservableCollection<Boolean?> ScreensCheckStatus {
      get { return (ObservableCollection<Boolean?>)this.GetValue(ScreenLayoutBox.ScreensCheckStatusProperty); }
      set { this.SetValue(ScreenLayoutBox.ScreensCheckStatusProperty, value); }
    }
    
    /// <commondoc select='DependencyObject/Methods/DependencyPropertyChangedHandler/*' params='Property=ScreensCheckStatus' />
    private static void OnScreensCheckStatusChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e) {
      ((ScreenLayoutBox)instance).GenerateScreenBorders();
    }
    #endregion

    #region Property: ShowScreenNumbers
    /// <summary>
    ///   <inheritdoc cref="ShowScreenNumbers" select='../value/node()' />
    /// </summary>
    private Boolean showScreenNumbers;

    /// <summary>
    ///   Gets or sets a value indicating whether to display the screen numbers or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the screen numbers are displayed; otherwise <c>false</c>.
    /// </value>
    public Boolean ShowScreenNumbers {
      get { return this.showScreenNumbers; }
      set { 
        this.showScreenNumbers = value;
        this.GenerateScreenBorders();
      }
    }
    #endregion

    #region Property: ShowScreenCheckBoxes
    /// <summary>
    ///   <inheritdoc cref="ShowScreenNumbers" select='../value/node()' />
    /// </summary>
    private Boolean showScreenCheckBoxes;

    /// <summary>
    ///   Gets or sets a value indicating whether to display the screen <see cref="CheckBox">Check Boxes</see> or not.
    /// </summary>
    /// <value>
    ///   A value indicating whether to display the screen <see cref="CheckBox">Check Boxes</see> or not.
    /// </value>
    public Boolean ShowScreenCheckBoxes {
      get { return this.showScreenCheckBoxes; }
      set { 
        this.showScreenCheckBoxes = value;
        this.GenerateScreenBorders();
      }
    }
    #endregion

    #region Property: Content
    /// <inheritdoc cref="System.Windows.Controls.ContentControl.Content" />  
    protected new Canvas Content {
      get { return (Canvas)base.Content; }
    }
    #endregion


    #region Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenLayoutBox" /> class.
    /// </summary>
    public ScreenLayoutBox() {
      base.Content = new Canvas();
      this.screens = new List<RadioButton>();
      this.SetResourceReference(ScreenLayoutBox.BackgroundProperty, SystemColors.ControlLightBrushKey);
      this.showScreenNumbers = true;

      // The viewbox which will be placed behind the screens to show their background.
      this.screensBackground = new Rectangle();
      this.Content.Children.Add(this.screensBackground);
    }
    #endregion

    #region Methods: GenerateScreenBorders, ScreenCheckBox_Click, OnRenderSizeChanged
    /// <summary>
    ///   Generates the <see cref="RadioButton" /> controls and aligns the <see cref="screensBackground" /> <see cref="Canvas" />.
    /// </summary>
    private void GenerateScreenBorders() {
      // Remove old borders.
      foreach (RadioButton screen in this.screens) {
        this.Content.Children.Remove(screen);
      }
      this.screens.Clear();

      // The control's measure has to be set before the screens can be generated.
      if ((this.ActualWidth > 0) && (this.ActualHeight > 0)) {
        Drawing.Rectangle allScreensBounds = new Drawing.Rectangle();

        for (Int32 i = 0; i < Screen.AllScreens.Length; i++) {
          allScreensBounds = Drawing.Rectangle.Union(Screen.AllScreens[i].Bounds, allScreensBounds);
        }

        Double horizontalFactor = (this.Content.ActualWidth / allScreensBounds.Width);
        Double verticalFactor = (this.Content.ActualHeight / allScreensBounds.Height);
        Double scaleFactor;

        if (horizontalFactor < verticalFactor) {
          scaleFactor = horizontalFactor;
        } else {
          scaleFactor = verticalFactor;
        }

        allScreensBounds = allScreensBounds.ScaleFull(scaleFactor);

        Int32 xAdd = 0;
        Int32 yAdd = 0;
        if (allScreensBounds.X < 0) {
          xAdd = Math.Abs(allScreensBounds.X);
        }
        if (allScreensBounds.Y < 0) {
          yAdd = Math.Abs(allScreensBounds.Y);
        }

        // Put the fullBounds rectangle into the center.
        allScreensBounds.X = (Int32)((this.Content.ActualWidth - allScreensBounds.Width) / 2.00);
        allScreensBounds.Y = (Int32)((this.Content.ActualHeight - allScreensBounds.Height) / 2.00);
        
        // Set the position and size of the preview ViewBox.
        Canvas.SetLeft(this.screensBackground, allScreensBounds.X + 4);
        Canvas.SetTop(this.screensBackground, allScreensBounds.Y + 4);
        this.screensBackground.Width = allScreensBounds.Width - 7;
        this.screensBackground.Height = allScreensBounds.Height - 7;

        // Create preview screen border objects.
        for (Int32 i = 0; i < Screen.AllScreens.Length; i++) {
          Drawing.Rectangle screenBounds = Screen.AllScreens[i].Bounds;
          screenBounds = screenBounds.ScaleFull(scaleFactor);
          screenBounds.X += allScreensBounds.X + xAdd;
          screenBounds.Y += allScreensBounds.Y + yAdd;

          RadioButton screen = new RadioButton();
          Canvas.SetLeft(screen, screenBounds.Left);
          Canvas.SetTop(screen, screenBounds.Top);
          screen.Width = screenBounds.Width;
          screen.Height = screenBounds.Height;
          screen.SetResourceReference(
            FrameworkElement.StyleProperty, ScreenLayoutBox.ScreenRadioButtonStyleKey
            );
          screen.IsChecked = (i == this.SelectedScreenIndex);
          screen.Checked += this.Screen_Checked;

          DockPanel contentPanel = new DockPanel();

          if (this.ShowScreenCheckBoxes) {
            CheckBox screenCheckBox = new CheckBox() {
              HorizontalAlignment = HorizontalAlignment.Left,
              Tag = i
            };
            if ((this.ScreensCheckStatus != null) && (i < this.ScreensCheckStatus.Count)) {
              screenCheckBox.IsChecked = this.ScreensCheckStatus[i];
            }
            screenCheckBox.Click += this.ScreenCheckBox_Click;
            screenCheckBox.SetResourceReference(FrameworkElement.StyleProperty, ScreenLayoutBox.ScreenCheckBoxStyleKey);
            DockPanel.SetDock(screenCheckBox, Dock.Left);

            contentPanel.Children.Add(screenCheckBox);
          }

          if (this.ShowScreenNumbers) {
            contentPanel.Children.Add(new TextBlock() {
              Text = (i + 1).ToString(CultureInfo.CurrentCulture), 
              HorizontalAlignment = HorizontalAlignment.Center, 
              VerticalAlignment = VerticalAlignment.Center
            });
          }

          screen.Content = contentPanel;
          this.screens.Add(screen);
          
          this.Content.Children.Add(screen);
        }
      }
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Controls.Primitives.ButtonBase.Click" /> event of a 
    ///   <see cref="System.Windows.Controls.Primitives.ToggleButton" /> instance and updates the <see cref="ScreensCheckStatus" /> 
    ///   dependency property.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ScreenCheckBox_Click(Object sender, RoutedEventArgs e) {
      CheckBox checkBox = sender as CheckBox;
      if (checkBox == null) {
        return;
      }

      Int32 screenIndex = (Int32)checkBox.Tag;

      if ((this.ScreensCheckStatus != null) && (screenIndex < this.ScreensCheckStatus.Count)) {
        this.ScreensCheckStatus[screenIndex] = checkBox.IsChecked;
      }
    }

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      base.OnRenderSizeChanged(sizeInfo);

      this.GenerateScreenBorders();
    }
    #endregion
  }
}
