using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace Common.Presentation {
  /// <summary>
  ///   Contains tooltip related data supposed to be used within a <see cref="DataTemplate" />.
  /// </summary>
  public class ToolTipData: DependencyObject {
    #region Dependency Property: Title
    /// <summary>
    ///   Identifies the <see cref="Title" /> <see cref="DependencyProperty">Dependency Property</see>. 
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      "Title", typeof(String), typeof(ToolTipData), new PropertyMetadata(null)
    );

    /// <summary>
    ///   Gets or sets the tooltip's title text.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The tooltip's title text.
    /// </value>
    [Bindable(true)]
    public String Title {
      get { return (String)this.GetValue(ToolTipData.TitleProperty); }
      set { this.SetValue(ToolTipData.TitleProperty, value); }
    }
    #endregion

    #region Dependency Property: Content
    /// <summary>
    ///   Identifies the <see cref="Content" /> <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
      "Content", typeof(String), typeof(ToolTipData), new PropertyMetadata(null)
    );

    /// <summary>
    ///   Gets or sets the tooltip's content.
    ///   This is a <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    /// <value>
    ///   The tooltip's content.
    /// </value>
    [Bindable(true)]
    public String Content {
      get { return (String)this.GetValue(ToolTipData.ContentProperty); }
      set { this.SetValue(ToolTipData.ContentProperty, value); }
    }
    #endregion

    #region Dependency Property: MaxWidth
    /// <summary>
    ///   Identifies the <see cref="MaxWidth" /> <see cref="DependencyProperty">Dependency Property</see>.
    /// </summary>
    public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
      "MaxWidth", typeof(Double), typeof(ToolTipData), new PropertyMetadata(null) 
    );
    #endregion
  }
}

