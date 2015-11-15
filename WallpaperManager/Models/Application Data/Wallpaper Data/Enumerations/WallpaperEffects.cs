// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Specifies drawing effects for a wallpaper.
  /// </summary>
  [Flags]
  public enum WallpaperEffects {
    /// <summary>
    ///   The wallpaper gets horizontally flipped.
    /// </summary>
    FlipHorizontal = 1,

    /// <summary>
    ///   The wallpaper gets vertically flipped.
    /// </summary>
    FlipVertical = 2,

    /// <summary>
    ///   The wallpaper gets mirrored to its right side.
    /// </summary>
    MirrorRight = 4,

    /// <summary>
    ///   The wallpaper gets mirrored to its left side.
    /// </summary>
    MirrorLeft = 8,

    /// <summary>
    ///   The wallpaper gets mirrored to its top side.
    /// </summary>
    MirrorTop = 16,

    /// <summary>
    ///   The wallpaper gets mirrored to its bottom side.
    /// </summary>
    MirrorBottom = 32
  }
}