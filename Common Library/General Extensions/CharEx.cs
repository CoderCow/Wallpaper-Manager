using System;
using System.Text;

namespace Common {
  /// <summary>
  ///   Defines a set of extension methods for <see cref="Char" />, <see cref="Char[]" /> and generic collections 
  ///   of <see cref="Char" />.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  public static class CharEx {
    #region Methods
    public static String Join(this Char[] chars, String separator) {
      StringBuilder joined = new StringBuilder(chars.Length * separator.Length);

      for (Int32 i = 0; i < chars.Length; i++) {
        if (i != 0) {
          joined.Append(separator);
        }
        
        joined.Append(chars[i]);
      }

      return joined.ToString();
    }

    public static String Join(this Char[] chars, Char separator) {
      return CharEx.Join(chars, separator.ToString());
    }
    #endregion
  }
}
