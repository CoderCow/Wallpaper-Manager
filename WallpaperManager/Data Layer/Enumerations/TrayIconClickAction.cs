// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System.Windows.Forms;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Specifies the action which should be executed when a click action is performed on the <see cref="NotifyIcon" />.
  /// </summary>
  public enum TrayIconClickAction {
    /// <summary>
    ///   No action should be performed.
    /// </summary>
    NoAction,

    /// <summary>
    ///   The main window should be displayed.
    /// </summary>
    ShowMainWindow,

    /// <summary>
    ///   The next wallpaper should be applied on the Windows Desktop.
    /// </summary>
    CycleNextWallpaper,

    /// <summary>
    ///   The options window should be displayed.
    /// </summary>
    ShowOptionsWindow,
  }
}
