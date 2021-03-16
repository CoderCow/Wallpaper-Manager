using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Common.Windows {
  internal static class WinAPI {
    #region Constants and Fields
    public const Int32 SPI_SETDESKWALLPAPER = 20;

    public const Int32 SPIF_UPDATEINIFILE = 0x01;
    public const Int32 SPIF_SENDWININICHANGE = 0x02;
    #endregion

    #region Events and Properties

    #endregion

    #region Methods
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static extern Int32 SystemParametersInfo(
      Int32 uAction, 
      Int32 uParam, 
      String lpvParam, 
      Int32 fuWinIni
    );
    #endregion
  }
}
