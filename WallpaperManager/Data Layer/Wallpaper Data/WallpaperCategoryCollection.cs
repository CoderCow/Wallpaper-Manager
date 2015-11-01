// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Represents a collection of <see cref="WallpaperCategory" /> objects.
  /// </summary>
  /// <commondoc select='ObservableCollections/General/*' />
  /// <seealso cref="Wallpaper" />
  /// <seealso cref="WallpaperCategory" />
  /// <threadsafety static="true" instance="false" />
  public class WallpaperCategoryCollection: ObservableCollection<WallpaperCategory>, IWeakEventListener {
    #region Constants: DataVersion
    /// <summary>
    ///   Represents the version number contained in the serialization info for backward compatibility.
    /// </summary>
    protected const String DataVersion = "2.0";
    #endregion

    #region Property: AllWallpapersCount
    /// <summary>
    ///   Gets the total count of all Wallpapers.
    /// </summary>
    /// <value>
    ///   Total count of all Wallpapers.
    /// </value>
    public Int32 AllWallpapersCount {
      get { 
        Int32 count = 0;

        foreach (WallpaperCategory category in this) {
          count += category.Count;
        }

        return count;
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperCategoryCollection" /> class.
    /// </summary>
    public WallpaperCategoryCollection() {}
    #endregion
    
    #region Methods: GetAllWallpapers, IsWallpaperInAnyCategory
    /// <summary>
    ///   Gets a new collection of all <see cref="Wallpaper" /> instances hold by all <see cref="WallpaperCategory" /> instances.
    /// </summary>
    /// <returns>
    ///   A new collection of all <see cref="Wallpaper" /> instances hold by all <see cref="WallpaperCategory" /> instances.
    /// </returns>
    public IList<Wallpaper> GetAllWallpapers() {
      List<Wallpaper> allWallpapers = new List<Wallpaper>(this.AllWallpapersCount);

      foreach (WallpaperCategory category in this) {
        foreach (Wallpaper wallpaper in category) {
          allWallpapers.Add(wallpaper);
        }
      }

      return allWallpapers;
    }

    /// <summary>
    ///   Determines whether a <see cref="Wallpaper" /> object is in one of the <see cref="WallpaperCategory" /> instances or not.
    /// </summary>
    /// <param name="wallpaper">
    ///   The <see cref="Wallpaper" /> object to check for.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether one of the <see cref="WallpaperCategory" /> instances contains a given
    ///   <see cref="Wallpaper" /> object or not.
    /// </returns>
    protected Boolean IsWallpaperInAnyCategory(Wallpaper wallpaper) {
      foreach (WallpaperCategory category in this) {
        if (category.Contains(wallpaper)) {
          return true;
        }
      }

      return false;
    }
    #endregion

    #region Method: InsertItem, RemoveItem, SetItem, ClearItems, TryGetItem
    /// <inheritdoc />
    protected override void InsertItem(Int32 index, WallpaperCategory item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      base.InsertItem(index, item);
      
      CollectionChangedEventManager.AddListener(item, this);
      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <inheritdoc />
    protected override void RemoveItem(Int32 index) {
      WallpaperCategory category = this[index];

      base.RemoveItem(index);
      CollectionChangedEventManager.RemoveListener(category, this);

      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <inheritdoc />
    protected override void SetItem(Int32 index, WallpaperCategory item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      WallpaperCategory oldItem = this.TryGetItem(index);

      base.SetItem(index, item);

      if (oldItem != null) {
        CollectionChangedEventManager.RemoveListener(oldItem, this);
      }
      CollectionChangedEventManager.AddListener(item, this);
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      WallpaperCategory[] removedWallpapers = new WallpaperCategory[this.Count];
      this.CopyTo(removedWallpapers, 0);

      base.ClearItems();

      for (Int32 i = 0; i < removedWallpapers.Length; i++) {
        CollectionChangedEventManager.RemoveListener(removedWallpapers[i], this);
      }

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
    private WallpaperCategory TryGetItem(Int32 index) {
      if ((index > 0) && (index < this.Count)) {
        return this[index];
      }

      return null;
    }
    #endregion

    #region Methods: Item_CollectionChanged, OnPropertyChanged
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
    private void Item_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      this.OnPropertyChanged("AllWallpapersCount");
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(CollectionChangedEventManager)) {
        this.Item_CollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion
  }
}
