// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
namespace WallpaperManager.Models {
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
