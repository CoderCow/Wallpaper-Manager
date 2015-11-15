// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Specifies the cycle-method used when random cycling singlescreen wallpapers on multiple screens.
  /// </summary>
  public enum WallpaperChangeType {
    /// <summary>
    ///   Changes the wallpaper of any screen to multiple random wallpapers.
    /// </summary>
    ChangeAll,

    /// <summary>
    ///   Changes the wallpaper of any screen to one random wallpaper.
    /// </summary>
    ChangeAllCloned,

    /// <summary>
    ///   Changes the wallpaper of one screen to one random wallpaper and proceeds this operation
    ///   on the next screen when cycling again.
    /// </summary>
    ChangeOneByOne
  }
}