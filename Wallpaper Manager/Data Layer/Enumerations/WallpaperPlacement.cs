// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

namespace WallpaperManager.Data {
  /// <summary>
  ///   Specifies how a wallpaper should be placed when drawn on a screen.
  /// </summary>
  public enum WallpaperPlacement {
    /// <summary>
    ///   The wallpaper image gets resized (keeping its aspect ratio) to fit in the screen.
    /// </summary>
    Uniform = 0,

    /// <summary>
    ///   The wallpaper image gets resized (keeping its aspect ratio) to fill the screen completely.
    /// </summary>
    UniformToFill = 1,

    /// <summary>
    ///   The wallpaper image gets resized (keeping its aspect ratio) to fill the screen.
    /// </summary>
    Stretch = 2,

    /// <summary>
    ///   The wallpaper keeps its original size and gets placed into the center of the screen.
    /// </summary>
    Center = 3,

    /// <summary>
    ///   The wallpaper is tiled to fill the whole screen.
    /// </summary>
    Tile = 4,
  }
}
