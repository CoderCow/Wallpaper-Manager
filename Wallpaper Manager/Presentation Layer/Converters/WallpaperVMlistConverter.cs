// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Converts the <see cref="ReadOnlyCollection{WallpaperVM}" /> to and from 
  ///   <see cref="IList" />.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(ReadOnlyCollection<WallpaperVM>), typeof(IList))]
  public class WallpaperVMlistConverter: IValueConverter {
    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="ReadOnlyCollection{WallpaperVM}" /> instance to an <see cref="IList" />
    ///   instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) {
        return DependencyProperty.UnsetValue;
      }

      ReadOnlyCollection<WallpaperVM> list = (ReadOnlyCollection<WallpaperVM>)value;
      List<WallpaperVM> convertedList = new List<WallpaperVM>(list.Count);

      foreach (WallpaperVM wallpaperVM in list) {
        convertedList.Add(wallpaperVM);
      }

      return convertedList;
    }

    /// <summary>
    ///   Converts a <see cref="IList" /> instance to a <see cref="ReadOnlyCollection{Wallpaper}" />
    ///   instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) {
        return DependencyProperty.UnsetValue;
      }

      IList list = (IList)value;
      List<WallpaperVM> convertedList = new List<WallpaperVM>(list.Count);

      foreach (WallpaperVM wallpaperVM in list) {
        convertedList.Add(wallpaperVM);
      }
      
      return new ReadOnlyCollection<WallpaperVM>(convertedList);
    }
    #endregion
  }
}
