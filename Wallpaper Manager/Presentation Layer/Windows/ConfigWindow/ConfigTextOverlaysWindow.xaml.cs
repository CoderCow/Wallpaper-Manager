// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using FormsDialogResult = System.Windows.Forms.DialogResult;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The Text Overlays Configuration Window used to configure <see cref="WallpaperOverlayText">WallpaperOverlayTexts</see>
  ///   by using the <see cref="ConfigTextOverlaysVM">Config Text Overlays View Model</see>.
  /// </summary>
  /// <seealso cref="ConfigTextOverlaysVM">ConfigTextOverlaysVM Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class ConfigTextOverlaysWindow: Window {
    #region Property: ConfigTextOverlaysVM
    /// <summary>
    ///   <inheritdoc cref="ConfigTextOverlaysVM" select='../value/node()' />
    /// </summary>
    private readonly ConfigTextOverlaysVM configTextOverlaysVM;

    /// <summary>
    ///   Gets the <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </value>
    public ConfigTextOverlaysVM ConfigTextOverlaysVM {
      get { return this.configTextOverlaysVM; }
    }
    #endregion

    #region Command: SelectFont
    /// <summary>
    ///   Contains the Select Font <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand SelectFontCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectFontCommand" />
    protected virtual void SelectFontCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows a <see cref="FontDialog" /> and applies the settings with the <see cref="ConfigTextOverlaysVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectFontCommand" />
    protected virtual void SelectFontCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (this.ConfigTextOverlaysVM.SelectedItem != null) {
        using (FontDialog fontDialog = new FontDialog()) {
          fontDialog.Font = this.ConfigTextOverlaysVM.SelectedItem.FontSettingsToFont();
          fontDialog.FontMustExist = true;
          fontDialog.ShowApply = false;
          fontDialog.ShowColor = true;
          fontDialog.Color = this.ConfigTextOverlaysVM.SelectedItem.ForeColor;

          if (fontDialog.ShowDialog() == FormsDialogResult.OK) {
            this.ConfigTextOverlaysVM.SelectedItem.FontSettingsFromFont(fontDialog.Font);
            this.ConfigTextOverlaysVM.SelectedItem.ForeColor = fontDialog.Color;
          }

          // For some reason the Window doesn't get the focus back after the dialog has been shown.
          this.Focus();
        }
      }
    }
    #endregion

    #region Command: SelectForeColor
    /// <summary>
    ///   Contains the Select Fore Color <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand SelectForeColorCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectForeColorCommand" />
    protected virtual void SelectForeColorCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows a <see cref="ColorDialog" /> and applies the settings with the <see cref="ConfigTextOverlaysVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectForeColorCommand" />
    protected virtual void SelectForeColorCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (this.ConfigTextOverlaysVM.SelectedItem != null) {
        using (ColorDialog colorDialog = new ColorDialog()) {
          colorDialog.AnyColor = true;
          colorDialog.AllowFullOpen = true;
          colorDialog.FullOpen = true;
          colorDialog.Color = this.ConfigTextOverlaysVM.SelectedItem.ForeColor;

          if (colorDialog.ShowDialog() == FormsDialogResult.OK) {
            this.ConfigTextOverlaysVM.SelectedItem.ForeColor = colorDialog.Color;
          }
        }
        
        // For some reason the Window doesn't get the focus back if the open file dialog has been shown.
        this.Focus();
      }
    }
    #endregion

    #region Command: SelectBorderColor
    /// <summary>
    ///   Contains the Select Border Color <see cref="RoutedCommand">Command</see>.
    /// </summary>
    public static readonly RoutedCommand SelectBorderColorCommand = new RoutedCommand();

    /// <summary>
    ///   Handles the <see cref="CommandBinding.CanExecute" /> event of a <see cref="CommandBinding" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectBorderColorCommand" />
    protected virtual void SelectBorderColorCommand_CanExecute(Object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    /// <summary>
    ///   Handles the <see cref="CommandBinding.Executed" /> event of a <see cref="CommandBinding" />.
    ///   This method shows a <see cref="ColorDialog" /> and applies the settings with the <see cref="ConfigTextOverlaysVM" />.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.
    /// </param>
    /// <seealso cref="SelectBorderColorCommand" />
    protected virtual void SelectBorderColorCommand_Executed(Object sender, ExecutedRoutedEventArgs e) {
      if (this.ConfigTextOverlaysVM.SelectedItem != null) {
        using (ColorDialog colorDialog = new ColorDialog()) {
          colorDialog.AnyColor = true;
          colorDialog.AllowFullOpen = true;
          colorDialog.FullOpen = true;
          colorDialog.Color = this.ConfigTextOverlaysVM.SelectedItem.BorderColor;

          if (colorDialog.ShowDialog() == FormsDialogResult.OK) {
            this.ConfigTextOverlaysVM.SelectedItem.BorderColor = colorDialog.Color;
          }
        }
        
        // For some reason the Window doesn't get the focus back if the open file dialog has been shown.
        this.Focus();
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigTextOverlaysWindow" /> class.
    /// </summary>
    /// <param name="configTextOverlaysVM">
    ///   The <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="configTextOverlaysVM" /> is <c>null</c>.
    /// </exception>
    public ConfigTextOverlaysWindow(ConfigTextOverlaysVM configTextOverlaysVM) {
      if (configTextOverlaysVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("configTextOverlaysVM"));
      }

      this.configTextOverlaysVM = configTextOverlaysVM;

      this.InitializeComponent();
    }
    #endregion
  }
}