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
  public class CollectionPropertyChangedListener<TItem>: IWeakEventListener, IDisposable {
    private readonly INotifyCollectionChanged notifier;
    private readonly IEnumerable collection;

    public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;

    public CollectionPropertyChangedListener(ObservableCollection<TItem> collection): this(collection, collection) {
      Contract.Requires<ArgumentNullException>(collection != null);
    }

    public CollectionPropertyChangedListener(INotifyCollectionChanged notifier, IEnumerable collection) {
      Contract.Requires<ArgumentNullException>(notifier != null);
      Contract.Requires<ArgumentNullException>(collection != null);
      
      this.notifier = notifier;
      this.collection = collection;
      CollectionChangedEventManager.AddListener(notifier, this);

      foreach (INotifyPropertyChanged item in collection)
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
          foreach (Wallpaper newWallpaper in e.NewItems)
            PropertyChangedEventManager.AddListener(newWallpaper, this, string.Empty);
          
          break;
        }
        case NotifyCollectionChangedAction.Remove: {
          foreach (Wallpaper deletedWallpaper in e.OldItems) {
            Contract.Assert(deletedWallpaper != null);
            PropertyChangedEventManager.RemoveListener(deletedWallpaper, this, string.Empty);
          }
          
          break;
        }
        case NotifyCollectionChangedAction.Replace: {
          Contract.Assert(e.NewItems.Count == e.OldItems.Count);

          for (int i = 0; i < e.NewItems.Count; i++) {
            Wallpaper oldWallpaper = (Wallpaper)e.OldItems[i];
            Wallpaper newWallpaper = (Wallpaper)e.NewItems[i];

            PropertyChangedEventManager.RemoveListener(oldWallpaper, this, string.Empty);
            PropertyChangedEventManager.AddListener(newWallpaper, this, string.Empty);
          }

          break;
        }
        case NotifyCollectionChangedAction.Reset: {
          foreach (Wallpaper oldWallpaper in e.OldItems)
            PropertyChangedEventManager.RemoveListener(oldWallpaper, this, string.Empty);
          foreach (Wallpaper newWallpaper in e.NewItems)
            PropertyChangedEventManager.AddListener(newWallpaper, this, string.Empty);
          
          break;
        }
      }
    }

    protected virtual void OnItemPropertyChanged(ItemPropertyChangedEventArgs e) {
      Contract.Requires<ArgumentNullException>(e != null);

      EventHandler<ItemPropertyChangedEventArgs> handler = this.ItemPropertyChanged;
      handler?.Invoke(this, e);
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
