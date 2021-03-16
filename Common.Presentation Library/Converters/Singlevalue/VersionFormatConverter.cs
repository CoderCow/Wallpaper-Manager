using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Common.Presentation {
  /// <summary>
  ///   Converts a <see cref="Version" /> instance to and from a <see cref="String" /> instance.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  [ValueConversion(typeof(Version), typeof(String))]
  public class VersionFormatConverter: IValueConverter {
    #region Constants: DefaultStringFormat
    /// <summary>
    ///   Represents the default <see cref="StringFormat" />.
    /// </summary>
    protected const String DefaultStringFormat = @"{0}.{1}.{2}.{3}";
    #endregion

    #region Property: StringFormat
    /// <summary>
    ///   <inheritdoc cref="StringFormat" select='../value/node()' />
    /// </summary>
    private String stringFormat;

    /// <summary>
    ///   Gets or sets the the string representing the format of the converted <see cref="Version" />.
    /// </summary>
    /// <value>
    ///   The the string representing the format of the converted <see cref="Version" />.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public String StringFormat {
      get { return this.stringFormat; }
      set { 
        if (value == null)
          throw new ArgumentNullException();

        this.stringFormat = value; 
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the 
    ///   <see cref="VersionFormatConverter">VersionFormatConverter Class</see> using the given 
    ///   <paramref name="stringFormat" />.
    /// </summary>
    /// <param name="stringFormat">
    ///   <inheritdoc cref="StringFormat" select='../value/node()' />
    /// </param>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the 
    ///     <see cref="VersionFormatConverter">VersionFormatConverter Class</see>.
    ///   </summary>
    /// </overloads>
    public VersionFormatConverter(String stringFormat) {
      if (stringFormat == null) {
        this.stringFormat = VersionFormatConverter.DefaultStringFormat;
      } else {
        this.stringFormat = stringFormat;
      }
    }

    /// <summary>
    ///   Initializes a new instance of the 
    ///   <see cref="VersionFormatConverter">VersionFormatConverter Class</see> using the 
    ///   <see cref="DefaultStringFormat" />.
    /// </summary>
    public VersionFormatConverter(): this(null) {}
    #endregion

    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="Version" /> instance to a <see cref="String" /> instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) {
        return DependencyProperty.UnsetValue;
      }

      Version source = (value as Version);
      if (source == null) {
        return DependencyProperty.UnsetValue;
      }

      return String.Format(this.StringFormat, source.Major, source.Minor, source.Build, source.Revision);
    }

    /// <summary>
    ///   Converts a <see cref="String" /> instance to a <see cref="Version" /> instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    /// <exception cref="NotImplementedException">
    ///   always.
    /// </exception>
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
    #endregion
  }
}