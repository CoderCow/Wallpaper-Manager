// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

namespace WallpaperManager.Data {
  /// <summary>
  ///   Specifies the action which should be executed when a <see cref="Wallpaper" /> is being double clicked.
  /// </summary>
  public enum WallpaperClickAction {
    /// <summary>
    ///   No action should be performed.
    /// </summary>
    NoAction,

    /// <summary>
    ///   The <see cref="Wallpaper" /> should be applied on the Windows Desktop.
    /// </summary>
    ApplyOnDesktop,

    /// <summary>
    ///   The wallpaper configuration window should be shown for the selected <see cref="Wallpaper" />.
    /// </summary>
    ShowConfigurationWindow
  }
}
