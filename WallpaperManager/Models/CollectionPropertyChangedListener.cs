using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WallpaperManager.Models;

namespace WallpaperManager {
  /// <seealso cref="IWeakEventListener">IWeakEventListener Interface</seealso>
  /// <threadsafety static="true" instance="false" />
  public class CollectionPropertyChangedListener<TItem>: IWeakEventListener, IDisposable where TItem: INotifyPropertyChanged {
    private readonly INotifyCollectionChanged notifier;
    private readonly IEnumerable collection;
    private bool resetHasHappened;

    public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;

    public CollectionPropertyChangedListener(ObservableCollection<TItem> collection): this(collection, collection) {
      Contract.Requires<ArgumentNullException>(collection != null);
    }

    public CollectionPropertyChangedListener(INotifyCollectionChanged notifier, IEnumerable<TItem> collection) {
      Contract.Requires<ArgumentNullException>(notifier != null);
      Contract.Requires<ArgumentNullException>(collection != null);
      
      this.notifier = notifier;
      this.collection = collection;
      CollectionChangedEventManager.AddListener(notifier, this);
      
      foreach (TItem item in collection)
        PropertyChangedEventManager.AddListener(item, this, string.Empty);
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        if (!this.IsDisposed) {
          TItem item = (TItem)sender;
          PropertyChangedEventArgs propertyChangedArgs = (PropertyChangedEventArgs)e;

          this.OnItemPropertyChanged(new ItemPropertyChangedEventArgs(item, propertyChangedArgs.PropertyName));
        }

        return true;
      } else if (managerType == typeof(CollectionChangedEventManager)) {
        if (!this.IsDisposed)
          this.Collection_Changed(sender, (NotifyCollectionChangedEventArgs)e);

        return true;
      }

      return false;
    }
    #endregion

    private void Collection_Changed(object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add: {
          foreach (Wallpaper newItem in e.NewItems)
            if (newItem != null)
              PropertyChangedEventManager.AddListener(newItem, this, string.Empty);
          
          break;
        }
        case NotifyCollectionChangedAction.Remove: {
          foreach (Wallpaper deletedItem in e.OldItems)
            if (deletedItem != null)
              PropertyChangedEventManager.RemoveListener(deletedItem, this, string.Empty);
          
          break;
        }
        case NotifyCollectionChangedAction.Replace: {
          Contract.Assert(e.NewItems.Count == e.OldItems.Count);

          for (int i = 0; i < e.NewItems.Count; i++) {
            Wallpaper oldItem = (Wallpaper)e.OldItems[i];
            Wallpaper newItem = (Wallpaper)e.NewItems[i];

            if (oldItem != null)
              PropertyChangedEventManager.RemoveListener(oldItem, this, string.Empty);

            if (newItem != null)
              PropertyChangedEventManager.AddListener(newItem, this, string.Empty);
          }

          break;
        }
        case NotifyCollectionChangedAction.Reset: {
          foreach (Wallpaper item in this.collection)
            if (item != null)
              PropertyChangedEventManager.AddListener(item, this, string.Empty);
          
          this.resetHasHappened = true;
          break;
        }
      }
    }

    protected virtual void OnItemPropertyChanged(ItemPropertyChangedEventArgs e) {
      Contract.Requires<ArgumentNullException>(e != null);

      bool itemExists;
      // TODO: Bad implementation. Improve this in the future
      if (this.resetHasHappened) {
        itemExists = false;
        // As NotifyCollectionChangedAction.Reset is issued, we have no way to know which items have been previously in the
        // collection, thus this temporary workaround is required.
        foreach (TItem item in this.collection) {
          if (item.Equals(e.Item)) {
            itemExists = true;
            break;
          }
        }
      } else {
        itemExists = true;
      }

      if (itemExists) {
        EventHandler<ItemPropertyChangedEventArgs> handler = this.ItemPropertyChanged;
        handler?.Invoke(this, e);
      } else if (e.Item != null) {
        PropertyChangedEventManager.RemoveListener(e.Item, this, string.Empty);
      }
    }

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    public bool IsDisposed { get; private set; }

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(bool disposing) {
      if (!this.IsDisposed && disposing) {
        CollectionChangedEventManager.RemoveListener(this.notifier, this);

        if (this.collection != null)
          foreach (INotifyPropertyChanged item in this.collection)
            PropertyChangedEventManager.RemoveListener(item, this, string.Empty);
      }

      this.IsDisposed = true;
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="Views.Application" /> class.
    /// </summary>
    ~CollectionPropertyChangedListener() {
      this.Dispose(false);
    }
    #endregion

    public class ItemPropertyChangedEventArgs: PropertyChangedEventArgs {
      public TItem Item { get; }

      public ItemPropertyChangedEventArgs(TItem item, string propertyName) : base(propertyName) {
        this.Item = item;
      }
    }
  }
}
