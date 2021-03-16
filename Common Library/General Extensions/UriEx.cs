using System;
using System.Collections.Generic;

namespace Common {
  public static class UriEx {
    public static Uri Clone(this Uri uri) {
      return new Uri(uri.ToString());
    }

    public static T Clone<T>(this IEnumerable<Uri> uris) where T: ICollection<Uri>, new() {
      T newCollection = new T();
      foreach (Uri uri in uris) {
        newCollection.Add(uri.Clone());
      }

      return newCollection;
    }
  }
}