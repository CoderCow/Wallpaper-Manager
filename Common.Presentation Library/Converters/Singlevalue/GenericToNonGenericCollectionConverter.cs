// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Presentation {
  /// <threadsafety static="true" instance="false" />
  public class GenericToNonGenericCollectionConverter<GenericCollectionItemType>: IValueConverter {
    #region Methods: Convert, ConvertBack
    /// <summary>
    ///   Converts a <see cref="IList{GenericCollectionItemType}" /> instance to an <see cref="IList" /> instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      IList<GenericCollectionItemType> genericList = (value as IList<GenericCollectionItemType>);
      if (genericList == null) {
        return DependencyProperty.UnsetValue;
      }

      ArrayList nonGenericList = new ArrayList(genericList.Count);
      foreach (GenericCollectionItemType item in genericList) {
        nonGenericList.Add(item);
      }

      return nonGenericList;
    }

    /// <summary>
    ///   Converts a <see cref="IList" /> instance to a <see cref="ReadOnlyCollection{Wallpaper}" />
    ///   instance.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      IList nonGenericList = (value as IList);
      if (nonGenericList == null) {
        return DependencyProperty.UnsetValue;
      }
      
      List<GenericCollectionItemType> genericList = new List<GenericCollectionItemType>(nonGenericList.Count);
      foreach (Object item in nonGenericList) {
        Contract.Assert(item is GenericCollectionItemType);
        genericList.Add((GenericCollectionItemType)item);
      }
      
      return new Collection<GenericCollectionItemType>(genericList);
    }
    #endregion
  }
}
