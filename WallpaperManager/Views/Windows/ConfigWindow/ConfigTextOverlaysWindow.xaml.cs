// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WallpaperManager.Models;
using WallpaperManager.ViewModels;
using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace WallpaperManager.Views {
  /// <summary>
  ///   The Text Overlays Configuration Window used to configure <see cref="WallpaperTextOverlay">WallpaperOverlayTexts</see>
  ///   by using the <see cref="ConfigTextOverlaysVM">Config Text Overlays View Model</see>.
  /// </summary>
  /// <seealso cref="ConfigTextOverlaysVM">ConfigTextOverlaysVM Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class ConfigTextOverlaysWindow : Window {
    /// <summary>
    ///   Gets the <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </value>
    public ConfigTextOverlaysVM ConfigTextOverlaysVM { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigTextOverlaysWindow" /> class.
    /// </summary>
    /// <param name="configTextOverlaysVM">
    ///   The <see cref="ConfigTextOverlaysVM" /> instance used as interface to communicate with the application.
    /// </param>
    public ConfigTextOverlaysWindow(ConfigTextOverlaysVM configTextOverlaysVM) {
      this.ConfigTextOverlaysVM = configTextOverlaysVM;
      this.InitializeComponent();
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.ConfigTextOverlaysVM != null);
      Contract.Invariant(ConfigTextOverlaysWindow.SelectFontCommand != null);
      Contract.Invariant(ConfigTextOverlaysWindow.SelectForeColorCommand != null);
      Contract.Invariant(ConfigTextOverlaysWindow.SelectBorderColorCommand != null);
    }

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
    protected virtual void SelectFontCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
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
    protected virtual void SelectFontCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
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
    protected virtual void SelectForeColorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
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
    protected virtual void SelectForeColorCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      if (this.ConfigTextOverlaysVM.SelectedItem != null) {
        using (ColorDialog colorDialog = new ColorDialog()) {
          colorDialog.AnyColor = true;
          colorDialog.AllowFullOpen = true;
          colorDialog.FullOpen = true;
          colorDialog.Color = this.ConfigTextOverlaysVM.SelectedItem.ForeColor;

          if (colorDialog.ShowDialog() == FormsDialogResult.OK)
            this.ConfigTextOverlaysVM.SelectedItem.ForeColor = colorDialog.Color;
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
    protected virtual void SelectBorderColorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
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
    protected virtual void SelectBorderColorCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      if (this.ConfigTextOverlaysVM.SelectedItem != null) {
        using (ColorDialog colorDialog = new ColorDialog()) {
          colorDialog.AnyColor = true;
          colorDialog.AllowFullOpen = true;
          colorDialog.FullOpen = true;
          colorDialog.Color = this.ConfigTextOverlaysVM.SelectedItem.BorderColor;

          if (colorDialog.ShowDialog() == FormsDialogResult.OK)
            this.ConfigTextOverlaysVM.SelectedItem.BorderColor = colorDialog.Color;
        }

        // For some reason the Window doesn't get the focus back if the open file dialog has been shown.
        this.Focus();
      }
    }
    #endregion
  }
}