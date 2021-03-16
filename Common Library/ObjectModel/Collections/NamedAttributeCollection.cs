using System;

namespace Common.ObjectModel.Collections {
  public class NamedAttributeCollection: KeyedCollection<String,INamedAttribute> {
    protected override String GetKeyForItem(INamedAttribute item) {
      return item.Name;
    }
  }
}