// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Windows.Forms;

namespace WallpaperManager.Models {
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
    ShowOptionsWindow
  }
}