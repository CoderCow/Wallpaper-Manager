// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;

namespace WallpaperManager.Models {
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