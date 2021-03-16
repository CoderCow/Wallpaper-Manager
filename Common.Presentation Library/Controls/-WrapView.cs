using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Common.Presentation.Controls {
  // Source: Microsoft MSDN Examples (Modified)
  /*public class WrapView: ViewBase {
    #region Constants and Fields
    public static readonly DependencyProperty ItemContainerStyleProperty = ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(WrapView));
    public static readonly DependencyProperty ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(typeof(WrapView));
    public static readonly DependencyProperty ItemWidthProperty = WrapPanel.ItemWidthProperty.AddOwner(typeof(WrapView));
    public static readonly DependencyProperty ItemHeightProperty = WrapPanel.ItemHeightProperty.AddOwner(typeof(WrapView));
    public static readonly DependencyProperty OrientationProperty = WrapPanel.OrientationProperty.AddOwner(typeof(WrapView));
    private static ResourceKey wrapViewStyleKey = null;
    #endregion

    #region Events and Properties
    public static ResourceKey WrapViewStyleKey {
      get {
        if (WrapView.wrapViewStyleKey == null) {
          WrapView.wrapViewStyleKey = new ComponentResourceKey(typeof(WrapView), "WrapViewStyleKey");
        }
        Debug.WriteLine(WrapView.wrapViewStyleKey);
        return WrapView.wrapViewStyleKey;
      }
    }

    public Style ItemContainerStyle {
      get { return (Style)this.GetValue(WrapView.ItemContainerStyleProperty); }
      set { this.SetValue(WrapView.ItemContainerStyleProperty, value); }
    }

    public DataTemplate ItemTemplate {
      get { return (DataTemplate)this.GetValue(WrapView.ItemTemplateProperty); }
      set { this.SetValue(WrapView.ItemTemplateProperty, value); }
    }

    public Double ItemWidth {
      get { return (Double)this.GetValue(WrapView.ItemWidthProperty); }
      set { this.SetValue(WrapView.ItemWidthProperty, value); }
    }

    public Double ItemHeight {
      get { return (Double)this.GetValue(WrapView.ItemHeightProperty); }
      set { this.SetValue(WrapView.ItemHeightProperty, value); }
    }

    public Orientation Orientation {
      get { return (Orientation)this.GetValue(WrapView.OrientationProperty); }
      set { this.SetValue(WrapView.OrientationProperty, value); }
    }

    protected override Object DefaultStyleKey {
      get { return WrapView.WrapViewStyleKey; }
    }
    #endregion
  }*/
}