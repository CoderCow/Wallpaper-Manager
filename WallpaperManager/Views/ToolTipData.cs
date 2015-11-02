// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WallpaperManager.Views {
  /// <summary>
  ///   Holds <see cref="ToolTip" /> related information which are usually displayed by a <see cref="DataTemplate" />.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ToolTipData : Common.Presentation.ToolTipData {
    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.NoteText != null);
    }

    #region Dependency Property: IsMultiscreenFeature
    /// <summary>
    ///   Identifies the <see cref="IsMultiscreenFeature" /> <see cref="DependencyProperty" />.
    /// </summary>
    public static readonly DependencyProperty IsMultiscreenFeatureProperty = DependencyProperty.Register(
      "IsMultiscreenFeature", typeof(bool), typeof(ToolTipData), new PropertyMetadata(false));

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether content will be replaced with the information that
    ///   this feature is only available on multiscreen systems if the running system is no multiscreen system.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether content will be replaced with the information that this feature is
    ///   only available on multiscreen systems if the running system is no multiscreen system.
    /// </value>
    [Bindable(true)]
    public bool IsMultiscreenFeature {
      get { return (bool)this.GetValue(ToolTipData.IsMultiscreenFeatureProperty); }
      set { this.SetValue(ToolTipData.IsMultiscreenFeatureProperty, value); }
    }
    #endregion

    #region Dependency Property: NoteIcon
    /// <summary>
    ///   Identifies the <see cref="NoteIcon" /> <see cref="DependencyProperty" />.
    /// </summary>
    public static readonly DependencyProperty NoteIconProperty = DependencyProperty.Register(
      "NoteIcon", typeof(ImageSource), typeof(ToolTipData), new PropertyMetadata(null));

    /// <summary>
    ///   Gets or sets the note icon of the tooltip.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The tooltip's note icon.
    /// </value>
    [Bindable(true)]
    public ImageSource NoteIcon {
      get { return (ImageSource)this.GetValue(ToolTipData.NoteIconProperty); }
      set { this.SetValue(ToolTipData.NoteIconProperty, value); }
    }
    #endregion

    #region Dependency Property: NoteText
    /// <summary>
    ///   Identifies the <see cref="NoteText" /> <see cref="DependencyProperty" />.
    /// </summary>
    public static readonly DependencyProperty NoteTextProperty = DependencyProperty.Register(
      "NoteText", typeof(string), typeof(ToolTipData), new PropertyMetadata(""));

    /// <summary>
    ///   Gets or sets the tooltip's note text.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The tooltip's note text.
    /// </value>
    [Bindable(true)]
    public string NoteText {
      get { return (string)this.GetValue(ToolTipData.NoteTextProperty); }
      set { this.SetValue(ToolTipData.NoteTextProperty, value); }
    }
    #endregion
  }
}