using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  public class XmlTypeDefAttributeCollection: KeyedCollection<Type,XmlNodeTypeDefAttribute> {
    public XmlTypeDefAttributeCollection() {}

    public XmlTypeDefAttributeCollection(IEnumerable<XmlNodeTypeDefAttribute> items) {
      this.AddRange(items);
    }

    public Boolean TryGetAttribute(Type type, out XmlNodeTypeDefAttribute attribute) {
      return this.TryGetValue(type, out attribute);
    }

    public Boolean TryGetAttribute(String name, out XmlNodeTypeDefAttribute attribute) {
      foreach (XmlNodeTypeDefAttribute attributeItem in this.Items) {
        if (attributeItem.Name == name) {
          attribute = attributeItem;
          return true;
        }
      }

      attribute = null;
      return false;
    }

    protected override void SetItem(Int32 index, XmlNodeTypeDefAttribute item) {
      XmlNodeTypeDefAttribute existingAttribute;
      if (this.TryGetAttribute(item.Type, out existingAttribute)) {
        item.AssignTo(existingAttribute);
      } else {
        base.SetItem(index, item);
      }
    }

    protected override Type GetKeyForItem(XmlNodeTypeDefAttribute item) {
      return item.Type;
    }
  }
}