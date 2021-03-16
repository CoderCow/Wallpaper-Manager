using System;
using System.Collections.Generic;
using System.Xml;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  internal interface IXmlSerializerInternal: IXmlSerializer {
    XmlSerializableMemberCollection SerializableMembers { get; }
    INamedAttribute RootAttribute { get; }
    Dictionary<Type,IXmlSerializerInternal> SubSerializers { get; }
    Boolean IsSubSerializer { get; }

    void SubSerialize(
      XmlWriter xmlWriter, Object instance, String rootName, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes
    );
    Object SubDeserialize(
      XmlReader xmlReader, String rootName, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes
    );
    IXmlSerializerInternal GetSubSerializer(Type type);
  }
}