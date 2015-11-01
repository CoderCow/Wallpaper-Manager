// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using WallpaperManager.Data;

namespace WallpaperManager.ApplicationInterface {
  /// <summary>
  ///   Specifies the configuration mode for the wallpaper configuration window.
  /// </summary>
  public enum ConfigWallpaperMode {
    /// <summary>
    ///   Defines that one or more <see cref="Wallpaper" /> objects will be configured.
    /// </summary>
    ConfigureWallpapers,

    /// <summary>
    ///   Defines that default settings for recently added <see cref="Wallpaper" /> objects will be configured.
    /// </summary>
    ConfigureDefaultSettings,

    /// <summary>
    ///   Defines that one static <see cref="Wallpaper" /> will be configured.
    /// </summary>
    ConfigureStaticWallpaper
  }
}
