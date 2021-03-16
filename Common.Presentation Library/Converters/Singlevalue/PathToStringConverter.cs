using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Common.IO;

namespace Common.Presentation {
  /// <threadsafety static="true" instance="false" />
  [ValueConversion(typeof(String), typeof(Path))]
  public class PathToStringConverter: IValueConverter {
    #region Constants and Fields
    /// <summary>
    ///   Represents the string representation for the <c>true</c> value.
    /// </summary>
    private const String DefaultNoneString = "None";

    /// <summary>
    ///   Represents the string representation for the <c>false</c> value.
    /// </summary>
    private const String DefaultPathFormatString = "{0}";

    /// <summary>
    ///   <inheritdoc cref="NoneString" select='../value/node()' />
    /// </summary>
    private String noneString = PathToStringConverter.DefaultNoneString;

    /// <summary>
    ///   <inheritdoc cref="PathFormatString" select='../value/node()' />
    /// </summary>
    private String pathFormatString = PathToStringConverter.DefaultPathFormatString;
    #endregion

    #region Properties
    public String NoneString {
      get { return this.noneString; }
      set {
        if (value == null)
          throw new ArgumentNullException();

        this.noneString = value;
      }
    }

    public String PathFormatString {
      get { return this.pathFormatString; }
      set {
        if (value == null)
          throw new ArgumentNullException();

        this.pathFormatString = value;
      }
    }
    #endregion

    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="Path" /> value to <see cref="String" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is Path) {
        Path path = (Path)value;

        if (path == Path.None) {
          return this.NoneString;
        }

        // Note: Leave the redundant ToString() here for performance related reasons.
        return String.Format(this.PathFormatString, path.ToString());
      }
        
      return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///   Converts a <see cref="String" /> value to <see cref="Path" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      String pathString = value as String;

      if (pathString != null) {
        if (pathString == this.NoneString) {
          return Path.None;
        }

        return new Path(pathString);
      }

      return DependencyProperty.UnsetValue;
    }
    #endregion
  }
}