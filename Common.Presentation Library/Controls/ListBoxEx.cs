using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Common.Presentation.Controls {
  /// <summary>
  ///   Extended <see cref="ListBox" /> control with a bindable 
  ///   <see cref="SelectedItems">SelectedItems property</see>.
  /// </summary>
  /// <remarks>
  ///   This extended version of the <see cref="ListBox" /> control allows to bind a collection to the 
  ///   <see cref="SelectedItems" /> property.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  public class ListBoxEx: ListBox {
    #region Constants and Fields
    /// <summary>
    ///   Identifies the <see cref="SelectedItems" /> dependency property. 
    /// </summary>
    /// <value>
    ///   The identifier for the <see cref="SelectedItems" /> dependency property.
    /// </value>
    public static readonly new DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(ListBoxEx), new PropertyMetadata(null, ListBoxEx.OnSelectedItemsChanged));
    private Boolean isUpdatingSelection;
    private Boolean isUpdatingSelectedItems;
    #endregion

    #region Events and Properties
    /// <summary>
    ///   Gets or sets the currently selected items. This is a dependency property.
    /// </summary>
    /// <value>
    ///   A collection of the currently selected items.
    /// </value>
    /// <returns>
    ///   Returns a collection of the currently selected items.
    /// </returns>
    public new IList SelectedItems {
      get { return (IList)this.GetValue(ListBoxEx.SelectedItemsProperty); }
      set { this.SetValue(ListBoxEx.SelectedItemsProperty, value); }
    }
    #endregion

    #region Methods
    private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      ((ListBoxEx)sender).OnSelectedItemsChanged();
    }

    /// <summary>
    ///   Invoked when the <see cref="SelectedItems" /> dependency property is changed.
    /// </summary>
    protected virtual void OnSelectedItemsChanged() {
      // We dont need to update the selection because the SelectedItems property has been set internally.
      if (this.isUpdatingSelectedItems) {
        return;
      }

      if (this.SelectionMode != SelectionMode.Single) {
        // Indicate that the SelectedItems property does'nt have to be updated while we change the selections.
        this.isUpdatingSelection = true;

        this.SelectedIndex = 0;

        IList items = this.SelectedItems;
        if (items != null) {
          foreach (Object item in items) {
            Int32 itemIndex = this.Items.IndexOf(item);

            if (itemIndex != -1) {
              if (this.Items[itemIndex] is ListBoxItem) {
                ((ListBoxItem)this.Items[itemIndex]).IsSelected = true;
              }
            }
          } 
        }

        this.isUpdatingSelection = false;
      }
    }

    /// <inheritdoc />
    protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
      base.OnSelectionChanged(e);

      // We dont need to set the SelectedItems property if we are actually selecting the items.
      if (!this.isUpdatingSelection) {
        // Indicate that the selection does'nt need to be changed when setting the SelectedItems property internally.
        this.UpdateSelection();
      }
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);

      // Since the ListBox auto selects the first item, we have to update the selection right 
      // after initialization.
      this.UpdateSelection();
    }

    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      base.OnItemsChanged(e);

      if (this.Items.Count > 0) {
        this.ScrollIntoView(this.Items[0]);
      }
    }

    private void UpdateSelection() {
      this.isUpdatingSelectedItems = true;
      this.SelectedItems = base.SelectedItems;
      this.isUpdatingSelectedItems = false;
    }
    #endregion
  }
}
