using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using Path = Common.IO.Path;
using Microsoft.Win32;

namespace Common.Windows {
  /// <summary>
  ///   Provides methods to read and write Windows desktop related configuration entries.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  public class Desktop {
    #region Constants and Fields
    private const String ControlPanelDesktopRegistryKey = @"Control Panel\Desktop";
    private const String WallpaperStyleRegistryValue = @"WallpaperStyle";
    private const String WallpaperTileRegistryValue = @"TileWallpaper";
    private const String WallpaperPathRegistryValue = @"WallPaper";
    #endregion

    #region Methods
    /// <summary>
    ///   Gets the path of the active wallpaper.
    /// </summary>
    /// <remarks>
    ///   The path returned may also be empty or invalid.
    ///   <c>null</c> will be returned if the key or value doesn't exist or isn't of type String.
    /// </remarks>
    /// <returns>
    ///   A <see cref="String" /> representing the active wallpaper path. 
    ///   <c>null</c> if the key or value doesn't exist or isn't of type string.
    /// </returns>
    /// <exception cref="SecurityException">
    ///   Missing framework access rights to read the registry key or its value.
    ///   Or missing windows access rights to read the registry key.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing Windows access rights to read the registry value.
    /// </exception>
    /// <permission cref="RegistryPermission">
    ///   for reading the registry key and value. Associated enumerations: 
    ///   <see cref="RegistryPermissionAccess.Read">RegistryPermissionAccess.Read</see>.
    /// </permission>
    /// <permission cref="SecurityPermission">
    ///   for the ability to access the specified registry key, if it is a remote key. 
    ///   Associated enumeration: 
    ///   <see cref="SecurityPermissionFlag.UnmanagedCode">SecurityPermissionFlag.UnmanagedCode</see>.
    /// </permission>
    public static String GetWallpaperPath() {
      using (RegistryKey desktopRegKey = 
        Registry.CurrentUser.OpenSubKey(Desktop.ControlPanelDesktopRegistryKey, false)
      ) {
        return Desktop.GetRegString(desktopRegKey, Desktop.WallpaperPathRegistryValue);
      }
    }

    /// <summary>
    ///   Sets the path of the active wallpaper.
    /// </summary>
    /// <remarks>
    ///   The path may also be <see cref="string.Empty" /> to indicate that no wallpaper is set.
    /// </remarks>
    /// <param name="wallpaperPath">
    ///   The path of the wallpaper.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when <see cref="wallpaperPath" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   Missing framework access rights to write the registry key or its value.
    ///   Or missing windows access rights to read or write the registry key.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing Windows access rights to read or write the registry values.
    /// </exception>
    /// <permission cref="RegistryPermission">
    ///   for writing the registry key and value. Associated enumerations: 
    ///   <see cref="RegistryPermissionAccess.Write">RegistryPermissionAccess.Write</see>.
    /// </permission>
    /// <permission cref="SecurityPermission">
    ///   for the ability to access the specified registry key, if it is a remote key. 
    ///   Associated enumeration: 
    ///   <see cref="SecurityPermissionFlag.UnmanagedCode">SecurityPermissionFlag.UnmanagedCode</see>.
    /// </permission>
    public static void SetWallpaperPath(String wallpaperPath) {
      if (wallpaperPath == null)
        throw new ArgumentNullException("wallpaperPath");
      
      using (RegistryKey desktopRegKey = 
        Registry.CurrentUser.OpenSubKey(Desktop.ControlPanelDesktopRegistryKey, true)
      ) {
        desktopRegKey.SetValue(Desktop.WallpaperPathRegistryValue, wallpaperPath);
      }
    }

    /// <summary>
    ///   Gets the wallpaper arrangement.
    /// </summary>
    /// <remarks>
    ///   This method returns <c>null</c> if the registry value is invalid or cannot be found.
    /// </remarks>
    /// <returns>
    ///   A <see cref="Nullable{WallpaperArrangement}" />? object representing the current
    ///   wallpaper arrangement. <c>null</c> if the registry value is invalid or cannot be found.
    /// </returns>
    /// <inheritdoc cref="GetWallpaperPath" />
    public static WallpaperArrangement? GetWallpaperArrangement() {
      using (RegistryKey desktopRegKey = 
        Registry.CurrentUser.OpenSubKey(Desktop.ControlPanelDesktopRegistryKey, false)
      ) {

        String wallpaperStyle = Desktop.GetRegString(desktopRegKey, Desktop.WallpaperStyleRegistryValue);
        String wallpaperTile = Desktop.GetRegString(desktopRegKey, Desktop.WallpaperTileRegistryValue);

        if (
          (wallpaperStyle == null) || (wallpaperTile == null) ||
          (wallpaperStyle.Length != 1) || (wallpaperTile.Length != 1)) {
          return null;
        }

        if (wallpaperTile[0] == '1') {
          return WallpaperArrangement.Tile;
        }

        if (wallpaperStyle[0] == '1') {
          return WallpaperArrangement.Center;
        }

        if (wallpaperStyle[0] == '2') {
          return WallpaperArrangement.Stretch;
        }

        return null;
      }
    }

    /// <summary>
    ///   Sets a new wallpaper to be shown on the Windows desktop.
    /// </summary>
    /// <remarks>
    ///   This method was tested on Windows Vista Business and worked fine with Bitmap and Jpeg files.
    ///   However, the API documentation says that the API function used by this method only supports 
    ///   Bitmap files so be carefully when using the method on other operating systems.
    /// </remarks>
    /// <param name="filePath">The path of the image file.</param>
    /// <param name="arrangement">
    ///   The new arrangement of the wallpaper. Set to <c>null</c> to keep the arrangement unchanged.
    /// </param>
    /// <exception cref="SecurityException">
    ///   Missing framework access rights to read the file specified by <see cref="filePath"/>.
    ///   Or missing framework access rights to read or write the registry key or its values.
    ///   Or missing Windows access rights to read or write the registry key.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   <see cref="filePath"/> is set to <c>null</c>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing Windows access rights to read the registry values.
    /// </exception>
    /// <exception cref="Win32Exception">
    ///   The file specified by <see cref="filePath"/> doesnt exist.
    /// </exception>
    /// <inheritdoc cref="SetWallpaperPath" select='permission[@cref="RegistryPermission"]' />
    /// <permission cref="FileIOPermission">
    ///   for reading the wallpaper file. Associated enumerations: 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery">FileIOPermissionAccess.PathDiscovery</see>, 
    ///   <see cref="FileIOPermissionAccess.Read">FileIOPermissionAccess.Read</see>.
    /// </permission>
    public static void SetWallpaper(Path filePath, WallpaperArrangement? arrangement) {
      new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, filePath).Demand();

      Desktop.SetWallpaperInternal(filePath, arrangement, true);
    }

    /// <inheritdoc cref="SetWallpaper(Path, Nullable{WallpaperArrangement})" />
    public static void SetWallpaper(Path filePath) {
      Desktop.SetWallpaper(filePath, null);
    }

    /// <summary>
    ///   Sets no wallpaper on the windows desktop.
    /// </summary>
    /// <inheritdoc 
    ///   cref="SetWallpaper(Path, Nullable{WallpaperArrangement})"
    ///   select='*[@cref!="FileIOPermission"]'
    /// />
    public static void SetWallpaperToNone() {
      Desktop.SetWallpaperInternal("", null, true);
    }

    private static void SetWallpaperInternal(String path, WallpaperArrangement? arrangement, Boolean resetable) {
      if (path == null)
        throw new ArgumentNullException("path");
      
      WallpaperArrangement? oldArrangement = null;
      String oldPath = null;

      // If the path is empty we just want to set the wallpaper to nothing
      if (path.Length != 0) {
        new FileIOPermission(FileIOPermissionAccess.Read, path).Demand();
      }

      if (resetable) {
        try {
          // This will require read rights too, but its necessary to execute this method safety
          oldPath = Desktop.GetWallpaperPath();
        } catch {
          oldPath = null;
        }
      }

      // Do we want to set a new arrangement?
      if (arrangement != null) {
        if (resetable) {
          // This will require read rights too, but its necessary to execute this method safety
          oldArrangement = Desktop.GetWallpaperArrangement();
        }

        Desktop.SetWallpaperArrangement(arrangement.Value);
      }

      Int32 errorOccurred = WinAPI.SystemParametersInfo(
        WinAPI.SPI_SETDESKWALLPAPER, 1, path, WinAPI.SPIF_UPDATEINIFILE | WinAPI.SPIF_SENDWININICHANGE
      );

      // Check if an error occurred
      if (errorOccurred == 0) {
        if (resetable) {
          // Since we had an error when setting the wallpaper, we have to try to reset the registry values now
          if ((arrangement != null) && (oldArrangement != null)) {
            Desktop.SetWallpaperArrangement(oldArrangement.Value);
          }
          if (oldPath != null) {
            Desktop.SetWallpaperInternal(oldPath, null, false);
          }
        }

        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    // This method shouldn't be called standalone. It will take no effect...
    private static void SetWallpaperArrangement(WallpaperArrangement arrangement) {
      using (RegistryKey desktopRegKey = 
        Registry.CurrentUser.OpenSubKey(Desktop.ControlPanelDesktopRegistryKey, true)
      ) {

        switch (arrangement) {
          case WallpaperArrangement.Center:
            desktopRegKey.SetValue(Desktop.WallpaperStyleRegistryValue, "1");
            desktopRegKey.SetValue(Desktop.WallpaperTileRegistryValue, "0");
            break;
          case WallpaperArrangement.Stretch:
            desktopRegKey.SetValue(Desktop.WallpaperStyleRegistryValue, "2");
            desktopRegKey.SetValue(Desktop.WallpaperTileRegistryValue, "0");
            break;
          case WallpaperArrangement.Tile:
            desktopRegKey.SetValue(Desktop.WallpaperStyleRegistryValue, "0");
            desktopRegKey.SetValue(Desktop.WallpaperTileRegistryValue, "1");
            break;
        }
      }
    }

    private static String GetRegString(RegistryKey key, String valueName) {
      if (key == null)
        throw new ArgumentNullException("key");
      if (valueName == null)
        throw new ArgumentNullException("valueName");

      Object value = key.GetValue(valueName, null);

      try {
        if (value is String)
          return (String)value;
        else
          return null;
      } catch (IOException) {
        return null;
      }
    }
    #endregion
  }
}
