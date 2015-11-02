// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Represents a collection of <see cref="WallpaperCategory" /> objects.
  /// </summary>
  /// <commondoc select='ObservableCollections/General/*' />
  /// <seealso cref="Wallpaper" />
  /// <seealso cref="WallpaperCategory" />
  /// <threadsafety static="true" instance="false" />
  public class WallpaperCategoryCollection : ObservableCollection<WallpaperCategory>, IWeakEventListener {
    /// <summary>
    ///   Represents the version number contained in the serialization info for backward compatibility.
    /// </summary>
    protected const string DataVersion = "2.0";

    /// <summary>
    ///   Gets the total count of all Wallpapers.
    /// </summary>
    /// <value>
    ///   Total count of all Wallpapers.
    /// </value>
    public int AllWallpapersCount {
      get {
        int count = 0;

        foreach (WallpaperCategory category in this)
          count += category.Count;

        return count;
      }
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(CollectionChangedEventManager)) {
        this.Item_CollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion

    /// <summary>
    ///   Gets a new collection of all <see cref="Wallpaper" /> instances hold by all <see cref="WallpaperCategory" />
    ///   instances.
    /// </summary>
    /// <returns>
    ///   A new collection of all <see cref="Wallpaper" /> instances hold by all <see cref="WallpaperCategory" /> instances.
    /// </returns>
    public IList<Wallpaper> GetAllWallpapers() {
      var allWallpapers = new List<Wallpaper>(this.AllWallpapersCount);

      foreach (WallpaperCategory category in this) {
        foreach (Wallpaper wallpaper in category)
          allWallpapers.Add(wallpaper);
      }

      return allWallpapers;
    }

    /// <summary>
    ///   Determines whether a <see cref="Wallpaper" /> object is in one of the <see cref="WallpaperCategory" /> instances or
    ///   not.
    /// </summary>
    /// <param name="wallpaper">
    ///   The <see cref="Wallpaper" /> object to check for.
    /// </param>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether one of the <see cref="WallpaperCategory" /> instances contains a given
    ///   <see cref="Wallpaper" /> object or not.
    /// </returns>
    protected bool IsWallpaperInAnyCategory(Wallpaper wallpaper) {
      foreach (WallpaperCategory category in this) {
        if (category.Contains(wallpaper))
          return true;
      }

      return false;
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, WallpaperCategory item) {
      // TODO: Throwing this exception is not allowed here.
      Contract.Requires<ArgumentNullException>(item != null);

      base.InsertItem(index, item);

      CollectionChangedEventManager.AddListener(item, this);
      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index) {
      WallpaperCategory category = this[index];

      base.RemoveItem(index);
      CollectionChangedEventManager.RemoveListener(category, this);

      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <inheritdoc />
    protected override void SetItem(int index, WallpaperCategory item) {
      // TODO: Throwing this exception is not allowed here.
      Contract.Requires<ArgumentNullException>(item != null);

      WallpaperCategory oldItem = this.TryGetItem(index);

      base.SetItem(index, item);

      if (oldItem != null)
        CollectionChangedEventManager.RemoveListener(oldItem, this);
      CollectionChangedEventManager.AddListener(item, this);
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      var removedWallpapers = new WallpaperCategory[this.Count];
      this.CopyTo(removedWallpapers, 0);

      base.ClearItems();

      for (int i = 0; i < removedWallpapers.Length; i++)
        CollectionChangedEventManager.RemoveListener(removedWallpapers[i], this);

      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <summary>
    ///   Tries to get the <see cref="WallpaperCategory" /> instance with the specified index.
    /// </summary>
    /// <param name="index">
    ///   A zero-based index.
    /// </param>
    /// <returns>
    ///   <c>null</c> if the index is invalid or the item is <c>null</c>; otherwise the
    ///   <see cref="WallpaperCategory" /> instance with the given index.
    /// </returns>
    private WallpaperCategory TryGetItem(int index) {
      if ((index > 0) && (index < this.Count))
        return this[index];

      return null;
    }

    /// <summary>
    ///   Handles the <see cref="ObservableCollection{T}.CollectionChanged" /> event of any
    ///   <see cref="WallpaperCategory" /> in this collection.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.
    /// </param>
    private void Item_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
  }
}