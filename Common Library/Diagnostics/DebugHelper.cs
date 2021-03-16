using System;
using System.Diagnostics;
using System.Reflection;

namespace Common.Diagnostics {
  public static class DebugHelper {
    [Conditional("DEBUG")]
    public static void WriteObjectPropertyData(Object obj) {
      Type objectType = obj.GetType();
      Debug.Write("-- ");
      Debug.Write(objectType.FullName);
      Debug.WriteLine(" Data --");
      Debug.Indent();

      foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
        if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0) {
          Debug.Write(propertyInfo.Name);
          Debug.Write(" = ");
          Debug.WriteLine(propertyInfo.GetValue(obj, null).ToString());
        }
      }

      Debug.Unindent();
      Debug.WriteLine("------------");
    }
  }
}