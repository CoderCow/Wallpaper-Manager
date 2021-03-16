using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Data;

namespace Common.Presentation {
  public class StringFormatConverter: IMultiValueConverter {
    #region IMultiValueConverter Implementation
    public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture) {
      if (parameter == null) throw new ArgumentNullException();
      if (!(parameter is String)) throw new ArgumentException();

      return String.Format(culture, (String)parameter, values);
    }

    public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
    #endregion
  }
}