// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.ObjectModel;

namespace WallpaperManager.Data {
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
