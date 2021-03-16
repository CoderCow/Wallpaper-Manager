using System;
using System.Runtime.Serialization;

namespace Common.IO.Serialization {
  [Serializable]
  public class XmlSerializationException: SerializationException {
    public XmlSerializationException(String message, Exception inner): base(message, inner) {}
    public XmlSerializationException(String message): base(message, null) {}
    public XmlSerializationException(): base("An error occurred when serializing or deserializing an object.") {}
  }
}