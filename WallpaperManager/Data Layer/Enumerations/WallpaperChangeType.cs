// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

namespace WallpaperManager.Data {
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
