using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Common {
  public static class CollectionEx {
    public static T Clone<T>(this IEnumerable<ICloneable> cloneableList) where T: IList, new() {
      if (cloneableList == null) throw new ArgumentNullException();

      T cloned = new T();
      foreach (ICloneable cloneableItem in cloneableList) {
        cloned.Add(cloneableItem.Clone());
      }

      return cloned;
    }

    public static T Clone<T>(this IEnumerable<Uri> cloneableList) where T: IList, new() {
      if (cloneableList == null) throw new ArgumentNullException();

      T cloned = new T();
      foreach (Uri cloneableItem in cloneableList) {
        cloned.Add(cloneableItem.Clone());
      }

      return cloned;
    }
  }
}