// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Holds <see cref="ToolTip" /> related information which are usually displayed by a <see cref="DataTemplate" />.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ToolTipData: Common.Presentation.ToolTipData {
    #region Dependency Property: IsMultiscreenFeature
    /// <summary>
    ///   Identifies the <see cref="IsMultiscreenFeature" /> <see cref="DependencyProperty" />. 
    /// </summary>
    public static readonly DependencyProperty IsMultiscreenFeatureProperty = DependencyProperty.Register(
      "IsMultiscreenFeature", typeof(Boolean), typeof(ToolTipData), new PropertyMetadata(false)
    );
    
    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether content will be replaced with the information that
    ///   this feature is only available on multiscreen systems if the running system is no multiscreen system.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether content will be replaced with the information that this feature is 
    ///   only available on multiscreen systems if the running system is no multiscreen system.
    /// </value>
    [Bindable(true)]
    public Boolean IsMultiscreenFeature {
      get { return (Boolean)this.GetValue(ToolTipData.IsMultiscreenFeatureProperty); }
      set { this.SetValue(ToolTipData.IsMultiscreenFeatureProperty, value); }
    }
    #endregion

    #region Dependency Property: NoteIcon
    /// <summary>
    ///   Identifies the <see cref="NoteIcon" /> <see cref="DependencyProperty" />. 
    /// </summary>
    public static readonly DependencyProperty NoteIconProperty = DependencyProperty.Register(
      "NoteIcon", typeof(ImageSource), typeof(ToolTipData), new PropertyMetadata(null)
    );

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
      "NoteText", typeof(String), typeof(ToolTipData), new PropertyMetadata("")
    );

    /// <summary>
    ///   Gets or sets the tooltip's note text.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The tooltip's note text.
    /// </value>
    [Bindable(true)]
    public String NoteText {
      get { return (String)this.GetValue(ToolTipData.NoteTextProperty); }
      set { this.SetValue(ToolTipData.NoteTextProperty, value); }
    }
    #endregion
  }
}
