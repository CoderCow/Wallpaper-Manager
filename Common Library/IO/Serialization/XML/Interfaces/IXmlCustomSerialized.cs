using System;

namespace Common.IO.Serialization {
  public interface IXmlCustomSerialized {
	  void Serialize(XmlSerializationProvider serializationProvider);
  }
}