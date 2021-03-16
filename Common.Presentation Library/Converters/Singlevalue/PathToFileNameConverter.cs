// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Common.IO;

namespace Common.Presentation {
  /// <summary>
  ///   Converts a <see cref="Path" /> object's file name to a <see cref="String" />.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ValueConversion(typeof(String), typeof(Path))]
  public class PathToFileNameConverter: IValueConverter {
    #region Property: NoneString
    private String noneString;

    /// <summary>
    ///   Represents the <see cref="String" /> used if <see cref="Path" /> is <see cref="Path.None" />.
    /// </summary>
    public String NoneString {
      get { return this.noneString; }
      set {
        if (value == null) throw new ArgumentNullException();
        this.noneString = value;
      }
    }
    #endregion
    

    #region Method: Constructor
    public PathToFileNameConverter() {
      this.noneString = String.Empty;
    }
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
          return this.NoneString;
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