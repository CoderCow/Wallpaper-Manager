using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;

namespace Common.ObjectModel {
  public class CollectionSynchronizer<SourceItemType, TargetItemType>: IWeakEventListener, IDisposable {
    #region Property: SourceCollection, TargetCollection
    private readonly IList<SourceItemType> sourceCollection;

    public IList<SourceItemType> SourceCollection {
      get { return this.sourceCollection; }
    }

    private readonly IList<TargetItemType> targetCollection;

    public IList<TargetItemType> TargetCollection {
      get { return this.targetCollection; }
    }
    #endregion

    #region Property: IsEnabled
    private Boolean isEnabled;

    public Boolean IsEnabled {
      get { return this.isEnabled; }
      set {
        if (this.isDisposed) throw new ObjectDisposedException("this");

        this.isEnabled = value;
      }
    }
    #endregion

    #region Property: GetTargetInstanceFunction
    private readonly Func<SourceItemType, TargetItemType> getTargetInstanceFunction;

    protected Func<SourceItemType, TargetItemType> GetTargetInstanceFunction {
      get { return this.getTargetInstanceFunction; }
    }
    #endregion


    #region Method: Constructor
    public CollectionSynchronizer(
      IList<SourceItemType> sourceCollection, IList<TargetItemType> targetCollection, 
      Func<SourceItemType, TargetItemType> getTargetInstanceFunction = null
    ) {
      if (sourceCollection == null) throw new ArgumentNullException();
      if (!(sourceCollection is INotifyCollectionChanged)) throw new ArgumentException();
      if (targetCollection == null) throw new ArgumentNullException();
      if (!targetCollection.IsReadOnly) throw new ArgumentException();

      this.sourceCollection = sourceCollection;
      this.targetCollection = targetCollection;
      this.getTargetInstanceFunction = getTargetInstanceFunction;

      this.isEnabled = true;
      CollectionChangedEventManager.AddListener((INotifyCollectionChanged)sourceCollection, this);
    }
    #endregion

    #region Methods: SourceCollection_CollectionChanged
    private void SourceCollection_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add:
          for (Int32 i = 0; i < e.NewItems.Count; i++) {
            if (this.getTargetInstanceFunction != null) {
              this.targetCollection.Add(this.getTargetInstanceFunction((SourceItemType)e.NewItems[i]));
            } else {
              this.targetCollection.Add((TargetItemType)e.NewItems[i]);
            }
          }

          break;
        case NotifyCollectionChangedAction.Remove:
          for (Int32 i = 0; i < e.NewItems.Count; i++) {
            if (this.getTargetInstanceFunction != null) {
              this.targetCollection.Add(this.getTargetInstanceFunction((SourceItemType)e.NewItems[i]));
            } else {
              this.targetCollection.Add((TargetItemType)e.NewItems[i]);
            }
          }

          break;
      }
    }
    #endregion

    #region IWeakEventListener Implementation
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(CollectionChangedEventManager)) {
        this.SourceCollection_CollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);

        return true;
      }

      return false;
    }
    #endregion

    #region IDisposable Implementation
    [ContractPublicPropertyName("IsDisposed")]
    private Boolean isDisposed;

    public Boolean IsDisposed {
      get { return this.isDisposed; }
    }

    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          this.isEnabled = false;
          CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged)this.sourceCollection, this);
        }
      }

      this.isDisposed = true;
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~CollectionSynchronizer() {
      this.Dispose(false);
    }
    #endregion
  }
}