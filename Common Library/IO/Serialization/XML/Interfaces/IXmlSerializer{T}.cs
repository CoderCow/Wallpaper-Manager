using System;
using System.Xml;

namespace Common.IO.Serialization {
  public interface IXmlSerializer<T>: IXmlSerializer {
    void Serialize(XmlWriter xmlWriter, T instance);
    new T Deserialize(XmlReader xmlReader);
  }
}