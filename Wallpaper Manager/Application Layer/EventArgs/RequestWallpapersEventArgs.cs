// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;

using Common;

using WallpaperManager.Data;

namespace WallpaperManager.Application {
  /// <summary>
  ///   Instanced when a <see cref="WallpaperChanger" /> instance requests <see cref="Wallpaper" /> objects for a 
  ///   new cycle.
  /// </summary>
  /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class RequestWallpapersEventArgs: EventArgs {
    #region Property: Wallpapers
    /// <summary>
    ///   <inheritdoc cref="Wallpapers" select='../value/node()' />
    /// </summary>
    private readonly List<Wallpaper> wallpapers;

    /// <summary>
    ///   Gets the collection of <see cref="Wallpaper" /> objects to be provided for cycling.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="Wallpaper" /> objects to be provided for cycling.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public List<Wallpaper> Wallpapers {
      get { return this.wallpapers; }
    }
    #endregion

    #region Method: Constructor, ToString
    /// <summary>
    ///   Initializes a new instance of the <see cref="RequestWallpapersEventArgs" /> class.
    /// </summary>
    public RequestWallpapersEventArgs() {
      this.wallpapers = new List<Wallpaper>(50);
    }

    /// <inheritdoc />
    public override String ToString() {
      return StringGenerator.FromListKeyed(
        new String[] { "Wallpapers" },
        (IList<Object>)new Object[] { this.Wallpapers.Count }
      );
    }
    #endregion
  }
}