// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

namespace WallpaperManager.Data {
  /// <summary>
  ///   Specifies the position of an overlay text on the screen.
  /// </summary>
  public enum OverlayTextPosition {
    /// <summary>
    ///   The text will be placed in the upper left corner of the screen.
    /// </summary>
    TopLeft,

    /// <summary>
    ///   The text will be centered in the upper border of the screen.
    /// </summary>
    TopMiddle,

    /// <summary>
    ///   The text will be placed in the upper right corner of the screen.
    /// </summary>
    TopRight,

    /// <summary>
    ///   The text will be placed in the lower left corner of the screen.
    /// </summary>
    BottomLeft,

    /// <summary>
    ///   The text will be centered in the lower border of the screen.
    /// </summary>
    BottomMiddle,

    /// <summary>
    ///   The text will be placed in the lower right corner of the screen.
    /// </summary>
    BottomRight,
  }
}
