using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Linq;

namespace Common.Text {
  /// <summary>
  ///   Provides methods to generated several kinds of strings.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public static class StringGenerator {
    #region Constants: DefaultNullString
    private const String DefaultNullString = "null";
    #endregion

    #region Methods: FromList, FromListKeyed, ToString
    public static String FromList(
      IList<Object> list, Boolean ignoreNull = false, IFormatProvider formatProvider = null,
      String listFormat = "{0}", String itemFormat = "{0}", String separator = ", ", 
      String nullString = StringGenerator.DefaultNullString
    ) {
      if (list == null) throw new ArgumentNullException();
      if (listFormat == null) throw new ArgumentNullException();
      if (itemFormat == null) throw new ArgumentNullException();
      if (separator == null) throw new ArgumentNullException();
      if (nullString == null) throw new ArgumentNullException();

      if (formatProvider == null) {
        formatProvider = CultureInfo.CurrentCulture;
      }

      if (list.Count == 0) {
        if (listFormat == "{0}") {
          return String.Empty;
        }

        return String.Format(formatProvider, listFormat, String.Empty);
      }

      StringBuilder builder = new StringBuilder();
      for (Int32 i = 0; i < list.Count; i++) {
        Object item = list[i];
        if (item != null && ignoreNull) {
          continue;
        }

        if (i != 0) {
          builder.Append(separator);
        }

        if (item != null) {
          if (itemFormat == "{0}") {
            builder.Append(item);
          } else {
            builder.AppendFormat(formatProvider, itemFormat, item);
          }
        } else {
          builder.Append(nullString);
        }
      }

      if (listFormat == "{0}") {
        return builder.ToString();
      }
      
      return String.Format(formatProvider, listFormat, builder);
    }

    public static String FromList(
      IList list, Boolean ignoreNull = false, IFormatProvider formatProvider = null,
      String listFormat = "{0}", String itemFormat = "{0}", String separator = ", ", 
      String nullString = StringGenerator.DefaultNullString
    ) {
      if (list == null) throw new ArgumentNullException();
      if (listFormat == null) throw new ArgumentNullException();
      if (itemFormat == null) throw new ArgumentNullException();
      if (separator == null) throw new ArgumentNullException();
      if (nullString == null) throw new ArgumentNullException();

      IList<Object> genList = new List<Object>(list.Count);
      foreach (Object item in list) {
        genList.Add(item);
      }

      return StringGenerator.FromList(genList, ignoreNull, formatProvider, listFormat, itemFormat, separator, nullString);
    }

    public static String FromListKeyed(
      IList<String> keys, IList<Object> values, Boolean ignoreNull = false, IFormatProvider formatProvider = null,
      String listFormat = "{0}", String itemFormat = "{0} = {1}", String separator = ", ", 
      String nullString = StringGenerator.DefaultNullString
    ) {
      if (keys.Count != values.Count) throw new ArgumentException();

      var list = new Dictionary<String,Object>(keys.Count);
      for (Int32 i = 0; i < keys.Count; i++) {
        list.Add(keys[i], values[i]);
      }

      return StringGenerator.FromListKeyed(
        list, ignoreNull, formatProvider, listFormat, itemFormat, separator, nullString);
    }

    public static String FromListKeyed(
      IDictionary<String,Object> list, Boolean ignoreNull = false, IFormatProvider formatProvider = null,
      String listFormat = "{0}", String itemFormat = "{0} = {1}", String separator = ", ", 
      String nullString = StringGenerator.DefaultNullString
    ) {
      if (list == null) throw new ArgumentNullException();
      if (listFormat == null) throw new ArgumentNullException();
      if (itemFormat == null) throw new ArgumentNullException();
      if (separator == null) throw new ArgumentNullException();
      if (nullString == null) throw new ArgumentNullException();

      if (formatProvider == null)
        formatProvider = CultureInfo.CurrentCulture;

      if (list.Count == 0) {
        if (listFormat == "{0}")
          return String.Empty;

        return String.Format(formatProvider, listFormat, String.Empty);
      }

      StringBuilder builder = new StringBuilder();
      Int32 i = 0;
      foreach (KeyValuePair<String,Object> item in list) {
        if (item.Key == null || (item.Value == null && ignoreNull))
          continue;

        if (i != 0)
          builder.Append(separator);

        if (item.Value != null)
          builder.AppendFormat(formatProvider, itemFormat, item.Key, item.Value);
        else
          builder.AppendFormat(formatProvider, itemFormat, item.Key, nullString);

        i++;
      }
    
      if (listFormat == "{0}")
        return builder.ToString();

      return String.Format(formatProvider, listFormat, builder);
    }

    public static String ToString(Object value) {
      if (value != null) {
        return value.ToString();
      }

      return StringGenerator.DefaultNullString;
    }
    #endregion
  }
}