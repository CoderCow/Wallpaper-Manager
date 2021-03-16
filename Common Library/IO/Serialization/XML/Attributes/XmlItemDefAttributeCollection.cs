using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  public class XmlItemDefAttributeCollection: KeyedCollection<Type,XmlItemDefAttribute> {
    public XmlItemDefAttributeCollection() {}

    public XmlItemDefAttributeCollection(IEnumerable<XmlItemDefAttribute> items) {
      this.AddRange(items);
    }

    protected override void SetItem(Int32 index, XmlItemDefAttribute item) {
      XmlItemDefAttribute existingAttribute;
      if (this.TryGetAttribute(item.Type, out existingAttribute)) {
        item.AssignTo(existingAttribute);
      } else {
        base.SetItem(index, item);
      }
    }

    public Boolean TryGetAttribute(Type type, out XmlItemDefAttribute attribute) {
      if (type == null) throw new ArgumentNullException();

      return this.TryGetValue(type, out attribute);
    }

    public Boolean TryGetAttribute(String name, out XmlItemDefAttribute attribute) {
      foreach (XmlItemDefAttribute attributeItem in this.Items) {
        if (attributeItem.Name == name) {
          attribute = attributeItem;
          return true;
        }
      }

      attribute = null;
      return false;
    }

    public Boolean ContainsName(String name) {
      if (String.IsNullOrEmpty(name)) throw new ArgumentNullException();

      foreach (XmlItemDefAttribute attributeItem in this.Items) {
        if (attributeItem.Name == name) {
          return true;
        }
      }

      return false;
    }

    public Boolean ContainsType(Type type) {
      return this.ContainsKey(type);
    }

    protected override Type GetKeyForItem(XmlItemDefAttribute item) {
      return item.Type;
    }
  }
}