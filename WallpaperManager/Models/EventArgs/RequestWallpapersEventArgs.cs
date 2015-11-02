// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Instanced when a <see cref="WallpaperChanger" /> instance requests <see cref="Wallpaper" /> objects for a
  ///   new cycle.
  /// </summary>
  /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class RequestWallpapersEventArgs : EventArgs {
    /// <summary>
    ///   Gets the collection of <see cref="Wallpaper" /> objects to be provided for cycling.
    /// </summary>
    /// <value>
    ///   The collection of <see cref="Wallpaper" /> objects to be provided for cycling.
    /// </value>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public List<Wallpaper> Wallpapers { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="RequestWallpapersEventArgs" /> class.
    /// </summary>
    public RequestWallpapersEventArgs() {
      this.Wallpapers = new List<Wallpaper>(50);
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.Wallpapers != null);
    }

    /// <inheritdoc />
    public override string ToString() {
      return $"Wallpapers: {this.Wallpapers}";
    }
  }
}