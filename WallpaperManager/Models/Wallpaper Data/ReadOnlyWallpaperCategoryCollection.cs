// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.ObjectModel;

namespace WallpaperManager.Models {
  /// <summary>
  ///   A read only wrapper for a <see cref="WallpaperCategoryCollection" />.
  /// </summary>
  /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
  /// <seealso cref="WallpaperCategory">WallpaperCategory Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class ReadOnlyWallpaperCategoryCollection: ReadOnlyObservableCollection<WallpaperCategory> {
    #region Properties: Items, AllWallpapersCount
    /// <summary>
    ///   Gets the collection wrapped by this <see cref="ReadOnlyCollection{T}" />.
    /// </summary>
    /// <value>
    ///   The collection wrapped by this <see cref="ReadOnlyCollection{T}" />.
    /// </value>
    /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
    protected new WallpaperCategoryCollection Items {
      get { return (WallpaperCategoryCollection)base.Items; }
    }

    /// <inheritdoc cref="WallpaperCategoryCollection.AllWallpapersCount" />
    public Int32 AllWallpapersCount {
      get { return this.Items.AllWallpapersCount; }
    }
    #endregion


    #region Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ReadOnlyWallpaperCategoryCollection" /> class.
    /// </summary>
    /// <param name="categories">
    ///   The <see cref="WallpaperCategoryCollection" /> instance to be wrapped.
    /// </param>
    /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
    /// <seealso cref="WallpaperCategory">WallpaperCategory Class</seealso>
    public ReadOnlyWallpaperCategoryCollection(WallpaperCategoryCollection categories): base(categories) {
      if (categories == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("categories"));
      }
    }
    #endregion
  }
}
