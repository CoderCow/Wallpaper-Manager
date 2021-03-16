using System;
using System.Xml;

namespace Common.IO.Serialization {
  public interface IXmlSerializer {
    Type TargetType { get; }
    XmlSerializationSettings Settings { get; }

    void Serialize(XmlWriter xmlWriter, Object instance);
    Object Deserialize(XmlReader xmlReader);
  }
}