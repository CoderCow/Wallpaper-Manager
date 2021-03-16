using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

using Microsoft.Win32;

namespace Common.Windows {
  public class Autostart {
    #region Constants and Fields
    private static AutostartEntryCollection currentUserEntries;
    private static AutostartEntryCollection localMachineEntries;

    private const String AutostartKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    #endregion

    #region Events and Properties
    public static AutostartEntryCollection CurrentUserEntries {
      get {
        if (Autostart.currentUserEntries == null) {
          RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
            Autostart.AutostartKeyPath, 
            RegistryKeyPermissionCheck.ReadWriteSubTree
          );

          if (registryKey == null)
            throw new IOException(String.Concat(@"HKEY_CURRENT_USER\", Autostart.AutostartKeyPath));

          Autostart.currentUserEntries = new AutostartEntryCollection(registryKey, false);
        }

        return Autostart.currentUserEntries;
      }
    }

    public static AutostartEntryCollection LocalMachineEntries {
      get {
        if (Autostart.localMachineEntries == null) {
          RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
            Autostart.AutostartKeyPath, 
            RegistryKeyPermissionCheck.ReadWriteSubTree
          );

          if (registryKey == null) {
            throw new IOException(String.Concat(@"HKEY_LOCAL_MACHINE\", Autostart.AutostartKeyPath));
          }


          Autostart.localMachineEntries = new AutostartEntryCollection(registryKey, false);
        }

        return Autostart.localMachineEntries;
      }
    }
    #endregion

    #region Methods
    
    #endregion
  }
}
