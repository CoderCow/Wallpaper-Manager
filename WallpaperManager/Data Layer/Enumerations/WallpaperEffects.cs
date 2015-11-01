// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;

namespace WallpaperManager.Data {
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
    MirrorBottom = 32,
  }
}
