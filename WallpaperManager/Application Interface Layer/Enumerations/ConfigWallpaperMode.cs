// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

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
