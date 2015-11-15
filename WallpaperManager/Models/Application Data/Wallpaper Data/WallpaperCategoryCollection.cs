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
  public class WallpaperCategoryCollection {
    /// <summary>
    ///   Represents the version number contained in the serialization info for backward compatibility.
    /// </summary>
    protected const string DataVersion = "2.0";

    /// <summary>
    ///   Used to listen for property changes on all wallpapers in this category.
    /// </summary>
    private readonly CollectionPropertyChangedListener<WallpaperCategory> categoriesPropertyChangedListener;

    public ObservableCollection<WallpaperCategory> Categories { get; }

    /// <summary>
    ///   Gets the total count of all Wallpapers.
    /// </summary>
    /// <value>
    ///   Total count of all Wallpapers.
    /// </value>
    public int WallpaperCount { get; private set; }

    public WallpaperCategoryCollection() {
      this.Categories = new ObservableCollection<WallpaperCategory>();
      this.categoriesPropertyChangedListener = new CollectionPropertyChangedListener<WallpaperCategory>(this.Categories);
      this.categoriesPropertyChangedListener.ItemPropertyChanged += this.Category_PropertyChanged;
    }

    private void Category_PropertyChanged(object sender, CollectionPropertyChangedListener<WallpaperCategory>.ItemPropertyChangedEventArgs e) {
      WallpaperCategory changedCategory = e.Item;

      if (e.PropertyName == nameof(changedCategory.Count)) {
        int count = 0;
        foreach (WallpaperCategory category in this.Categories)
          count += category.Count;

        this.WallpaperCount = count;
      }
    }

    /// <summary>
    ///   Enumerates through all wallpapers in all categories.
    /// </summary>
    /// <returns>
    ///   An enumerable of all wallpapers.
    /// </returns>
    public IEnumerable<Wallpaper> EnumerateAllWallpapers() {
      foreach (WallpaperCategory category in this.Categories)
        foreach (Wallpaper wallpaper in category.Wallpapers)
          yield return wallpaper;
    }

    /*/// <summary>
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
    }*/
  }
}