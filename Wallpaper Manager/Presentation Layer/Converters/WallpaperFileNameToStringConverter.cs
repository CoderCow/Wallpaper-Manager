// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Common.IO;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Converts a <see cref="Path" /> object's file name to a <see cref="String" />.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ValueConversion(typeof(String), typeof(Path))]
  public class WallpaperFileNameToStringConverter: IValueConverter {
    #region Constants: NoneString
    /// <summary>
    ///   Represents the <see cref="String" /> used if no file name can be resolved.
    /// </summary>
    public const String NoneString = "-no image-";
    #endregion
    

    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="Path" /> object's file name to a <see cref="String" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is Path) {
        Path path = (Path)value;

        if (path == Path.None) {
          return WallpaperFileNameToStringConverter.NoneString;
        }

        return path.FileName.ToString();
      }
        
      return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///   Converts a <see cref="String" /> value to <see cref="Path" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
    #endregion
  }
}