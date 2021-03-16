using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Common.IO.Serialization {
  public class XmlDeserializationProvider: XmlSerializationProviderBase {
    #region Property: XmlReader
    private readonly XmlReader xmlReader;

    public XmlReader XmlReader {
      get { return this.xmlReader; }
    }
    #endregion

    #region Contract Invariant Method
    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.XmlReader != null);
    }
    #endregion
    

    #region Method: Constructor
    internal XmlDeserializationProvider(
      IXmlSerializerInternal serializer, XmlReader reader, String elementName, Boolean asAttribute = false, 
      XmlItemDefAttributeCollection itemDefAttributes = null
    ): base(serializer, elementName, asAttribute, itemDefAttributes) {
      if (serializer == null) throw new ArgumentNullException();
      if (reader == null) throw new ArgumentNullException();
      if (String.IsNullOrEmpty(elementName)) throw new ArgumentNullException();
      if (reader.ReadState == ReadState.Closed || reader.ReadState == ReadState.Error) throw new ArgumentException();

      this.xmlReader = reader;
    }
    #endregion

    #region Methods: ReadRoot, ReadAll
    public void ReadToRoot(String customName = null) {
      Contract.Requires<InvalidOperationException>(
        this.XmlReader.NodeType == XmlNodeType.None || this.XmlReader.NodeType == XmlNodeType.Element
      );

      while (this.XmlReader.NodeType != XmlNodeType.Element && 
        this.XmlReader.Read()) {}

      String rootName = customName;
      if (rootName == null) {
        rootName = this.ElementName;
      }
        
      if (this.XmlReader.LocalName != rootName) {
        if (customName == null) {
          var ex = new XmlSerializationException("Root element name does not match the name defined by the metadata.");
          ex.Data.Add("Read Name", this.XmlReader.LocalName);
          ex.Data.Add("Expected Name", rootName);
          throw ex;
        } else {
          var ex = new XmlSerializationException("Root element name does not match the name given by the customName parameter.");
          ex.Data.Add("Read Name", this.XmlReader.LocalName);
          ex.Data.Add("Expected Name", rootName);
          throw ex;
        }
      }
    }

    /// <exception cref="XmlSerializationException">
    ///   Root element name does not match the name defined by the metadata.
    /// </exception>
    public void ReadRoot(
      Object instance, String customName = null, Boolean ignoreAttributes = false
    ) {
      if (instance == null) throw new ArgumentNullException();
      if (!this.AsAttribute) throw new InvalidOperationException();
      Contract.Requires<InvalidOperationException>(
        this.XmlReader.NodeType == XmlNodeType.None || this.XmlReader.NodeType == XmlNodeType.Element
      );

      this.ReadToRoot(customName);

      if (ignoreAttributes) {
        while (this.XmlReader.MoveToNextAttribute()) {}
      } else {
        while (this.XmlReader.MoveToNextAttribute()) {
          for (Int32 i = 0; i < this.Serializer.SerializableMembers.Count; i++) {
            XmlSerializableMember member = this.Serializer.SerializableMembers[i];

            if (member.MemberInfo.IsAttribute) {
              Type memberType;
              if (member.MemberInfo.TryGetMemberTypeByName(this.XmlReader.LocalName, out memberType)) {
                member.SetValue(instance, this.ReadAttributeObject(memberType));
              }
            }
          }
        }
      }
    }

    public void ReadAll(Object instance) {
      if (instance == null) throw new ArgumentNullException();
      Contract.Requires<InvalidOperationException>(
        this.XmlReader.NodeType == XmlNodeType.None || this.XmlReader.NodeType == XmlNodeType.Element
      );

      if (this.XmlReader.NodeType == XmlNodeType.None || !this.IsRoot) {
        this.ReadRoot(instance);
      }

      if (!this.XmlReader.IsEmptyElement) {
        while (this.XmlReader.NodeType != XmlNodeType.EndElement && this.XmlReader.Read()) {
          if (this.XmlReader.NodeType != XmlNodeType.Element) {
            continue;
          }

          XmlMemberInfo memberInfo;
          if (!this.TryGetCurrentMember(out memberInfo)) {
            continue;
          }

          XmlSerializableMember member;
          if (this.Serializer.SerializableMembers.TryGetValue(memberInfo.Name, out member)) {
            if (!member.MemberInfo.IsAttribute && !member.MemberInfo.IsStatic) {
              member.SetValue(instance, this.ReadMemberContent(member.MemberInfo));
            }
          }
        }
      }
    }
    #endregion

    #region Methods: ReadNextElement, ReadMemberContent, TryGetCurrentMember
    public Boolean ReadNextElement() {
      while (this.XmlReader.Read()) {
        if (this.XmlReader.NodeType == XmlNodeType.Element)
          return true;
      }

      return false;
    }

    public T ReadMemberContent<T>(XmlMemberInfo memberInfo, IEnumerable<XmlItemDefAttribute> itemDefAttributeOverrides = null) {
      if (this.XmlReader.ReadState != ReadState.Interactive) throw new InvalidOperationException();
      if (this.XmlReader.NodeType != XmlNodeType.Element) throw new InvalidOperationException();

      return (T)this.ReadMemberContent(memberInfo, itemDefAttributeOverrides);
    }

    private Object ReadMemberContent(XmlMemberInfo memberInfo, IEnumerable<XmlItemDefAttribute> itemDefAttributeOverrides = null) {
      XmlItemDefAttributeCollection itemDefAttributes = new XmlItemDefAttributeCollection(memberInfo.ItemDefAttributes);
      if (itemDefAttributeOverrides != null) {
        itemDefAttributes.AddRange(itemDefAttributeOverrides);
      }

      return this.ReadElementContent(memberInfo.Type, itemDefAttributes);
    }

    public Boolean TryGetCurrentMember(out XmlMemberInfo memberInfo) {
      if (this.XmlReader.ReadState != ReadState.Interactive) throw new InvalidOperationException();
      if (this.XmlReader.NodeType != XmlNodeType.Element) throw new InvalidOperationException();
      Contract.Ensures(
        (Contract.Result<Boolean>() && Contract.ValueAtReturn(out memberInfo) != null) || 
        (!Contract.Result<Boolean>() && Contract.ValueAtReturn(out memberInfo) == null));

      for (Int32 i = 0; i < this.Serializer.SerializableMembers.Count; i++) {
        XmlSerializableMember member = this.Serializer.SerializableMembers[i];

        Type memberType;
        if (member.MemberInfo.TryGetMemberTypeByName(this.XmlReader.LocalName, out memberType)) {
          memberInfo = member.MemberInfo;
          return true;
        }
      }

      memberInfo = null;
      return false;
    }
    #endregion

    #region Method: ReadAttributeObject
    public T ReadAttributeObject<T>(XmlItemDefAttributeCollection itemDefAttributes = null) {
      if (this.XmlReader.ReadState != ReadState.Interactive) throw new InvalidOperationException();
      if (this.XmlReader.NodeType != XmlNodeType.Attribute) throw new InvalidOperationException();

      return (T)this.ReadAttributeObject(typeof(T), itemDefAttributes);
    }

    private Object ReadAttributeObject(Type deserializedType, XmlItemDefAttributeCollection itemDefAttributes = null) {
      if (XmlDeserializationProvider.IsNativelyDeserializable(deserializedType)) {
        return this.ReadNativeObject(deserializedType, true, itemDefAttributes);
      }

      return this.Serializer.GetSubSerializer(deserializedType).SubDeserialize(
        this.XmlReader, this.XmlReader.LocalName, true, itemDefAttributes
      );
    }
    #endregion

    #region Method: ReadElementContent
    public T ReadElementContent<T>(XmlItemDefAttributeCollection itemDefAttributes = null) {
      if (this.XmlReader.ReadState != ReadState.Interactive) throw new InvalidOperationException();
      if (this.XmlReader.NodeType != XmlNodeType.Element) throw new InvalidOperationException();

      return (T)this.ReadElementContent(typeof(T), itemDefAttributes);
    }

    private Object ReadElementContent(Type deserializedType, XmlItemDefAttributeCollection itemDefAttributes = null) {
      if (XmlDeserializationProvider.IsNativelyDeserializable(deserializedType)) {
        return this.ReadNativeObject(deserializedType, false, itemDefAttributes);
      }

      return this.Serializer.GetSubSerializer(deserializedType).SubDeserialize(
        this.XmlReader, this.XmlReader.LocalName, false, itemDefAttributes
      );
    }
    #endregion

    #region Methods: ReadNativeObject, IsNativelyDeserializable
    /// <exception cref="ArgumentException">
    ///   The given <paramref name="type" /> is not natively deserializable.
    /// </exception>
    /// <exception cref="XmlSerializationException">
    ///   A collection can not be deserialized as an attribute.
    /// </exception>
    private Object ReadNativeObject(Type type, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes) {
      // Check for null value.
      if (asAttribute) {
        if (this.XmlReader.Value == this.Settings.NullAttributeName)
          return null;
      } else {
        if (this.XmlReader.GetAttribute(this.Settings.NullAttributeName) != null)
          return null;
      }

      if (typeof(System.Collections.IList).IsAssignableFrom(type) || typeof(System.Collections.IDictionary).IsAssignableFrom(type)) {
        if (asAttribute)
          throw new XmlSerializationException("A collection can not be deserialized as an attribute.");

        Contract.Assert(this.XmlReader.NodeType == XmlNodeType.Element);
        
        Object collection;
        Boolean isDictionary = typeof(System.Collections.IDictionary).IsAssignableFrom(type);
        if (type.IsClass) {
          ConstructorInfo constructorInfo = type.GetConstructor(new Type[] {});
          if (constructorInfo == null) {
            var ex = new XmlSerializationException("An empty constructor is required to deserialize the given IList.");
            ex.Data.Add("Collection Type", type);
            throw ex;
          }

          try {
            collection = constructorInfo.Invoke(new Object[] {});
          } catch (Exception exception) {
            var ex = new XmlSerializationException("Parameterless constructor of deserializable collection threw exception. See inner exception for more details.", exception);
            ex.Data.Add("Collection Type", type);
            throw ex;
          }
        } else {
          // Value type constructor, will never throw exceptions.
          collection = Activator.CreateInstance(type);
        }
        
        if (!this.XmlReader.IsEmptyElement) {
          while (this.XmlReader.Read() && this.XmlReader.NodeType != XmlNodeType.EndElement) {
            if (this.XmlReader.NodeType != XmlNodeType.Element)
              continue;

            XmlItemDefAttribute attribute;
            if (!itemDefAttributes.TryGetAttribute(this.XmlReader.LocalName, out attribute)) {
              if (this.Settings.RequiresExplicitCollectionItemDefinition) {
                var ex = new XmlSerializationException("The collection-node in the XML-Data contains a sub-node with a name which is not explictly defined by an XmlItemDefAttribute which is required by the current serialization settings.");
                ex.Data.Add("Collection Type", type);
                ex.Data.Add("Node Name", this.XmlReader.LocalName);
                throw ex;
              }
            }

            if (!isDictionary) {
              ((System.Collections.IList)collection).Add(this.ReadElementContent(attribute.Type));
            } else {
              Object key = null;
              Object value = null;
              Type[] genericArgumentTypes = type.GetGenericArguments();
              while (this.XmlReader.Read() && this.XmlReader.NodeType != XmlNodeType.EndElement) {
                if (this.XmlReader.NodeType != XmlNodeType.Element)
                  continue;

                if (this.XmlReader.Name == "Key")
                  key = this.ReadElementContent(genericArgumentTypes[0]);
                else if (this.XmlReader.Name == "Value")
                  value = this.ReadElementContent(genericArgumentTypes[1]);
              }

              if (this.XmlReader.NodeType == XmlNodeType.EndElement)
                this.XmlReader.ReadEndElement();

              try {
                ((System.Collections.IDictionary)collection).Add(key, value);
              } catch (Exception exception) {
                var ex = new XmlSerializationException("Add method of IDictionary implementing collection threw exception. See inner exception for more details.", exception);
                ex.Data.Add("Collection Type", type);
                throw ex;
              }
            }
          }

          if (this.XmlReader.NodeType == XmlNodeType.EndElement)
            this.XmlReader.ReadEndElement();
        }

        return collection;
      }

      if (type.Name == "KeyValuePair`2" && type.Namespace == "System.Collections.Generic") {
        if (asAttribute)
          throw new XmlSerializationException("The native type KeyValuePair<,> is not deserializable as attribute.");

        Object key = null;
        Object value = null;
        Type[] genericArgumentTypes = type.GetGenericArguments();
        while (this.XmlReader.Read() && this.XmlReader.NodeType != XmlNodeType.EndElement) {
          if (this.XmlReader.NodeType != XmlNodeType.Element)
            continue;

          if (this.XmlReader.Name == "Key")
            key = this.ReadElementContent(genericArgumentTypes[0]);
          else if (this.XmlReader.Name == "Value")
            value = this.ReadElementContent(genericArgumentTypes[1]);
        }

        if (this.XmlReader.NodeType == XmlNodeType.EndElement)
          this.XmlReader.ReadEndElement();

        return Activator.CreateInstance(type, key, value);
      }

      String fullTextValue;
      if (!asAttribute)
        fullTextValue = this.XmlReader.ReadElementString();
      else
        fullTextValue = this.XmlReader.Value;

      try {
        if (type.IsEnum)
          return Enum.Parse(type, fullTextValue);
        
        switch (type.FullName) {
          case "System.String":
            return fullTextValue;
          case "System.Boolean":
            return Boolean.Parse(fullTextValue);
          case "System.Byte":
            return Byte.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.SByte":
            return SByte.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Int16":
            return Int16.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Int32":
            return Int32.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Int64":
            return Int64.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.UInt16":
            return UInt16.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.UInt32":
            return UInt32.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.UInt64":
            return UInt64.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Single":
            return Single.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Double":
            return Double.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Decimal":
            return Decimal.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Char":
            return Char.Parse(fullTextValue);
          case "System.DateTime":
            return DateTime.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.DBNull":
            return DBNull.Value;
          case "System.Version":
            return new Version(fullTextValue);
          case "System.DateTimeOffset":
            return DateTimeOffset.Parse(fullTextValue, this.Settings.CultureInfo);
          case "System.Guid":
            return new Guid(fullTextValue);

          case "System.Drawing.Color":
            return System.Drawing.ColorTranslator.FromHtml(fullTextValue);
          case "System.Drawing.Point":
            String[] pointData = fullTextValue.Split(',');
            return new System.Drawing.Point(Int32.Parse(pointData[0]), Int32.Parse(pointData[1]));
          case "System.Drawing.PointF":
            String[] pointFData = fullTextValue.Split(',');
            return new System.Drawing.PointF(Single.Parse(pointFData[0]), Single.Parse(pointFData[1]));
          case "System.Drawing.Rectangle":
            String[] rectangleData = fullTextValue.Split(',');
            return new System.Drawing.Rectangle(
              Int32.Parse(rectangleData[0]), Int32.Parse(rectangleData[1]),
              Int32.Parse(rectangleData[2]), Int32.Parse(rectangleData[3])
              );
          case "System.Drawing.RectangleF":
            String[] rectangleFData = fullTextValue.Split(',');
            return new System.Drawing.RectangleF(
              Single.Parse(rectangleFData[0]), Single.Parse(rectangleFData[1]),
              Single.Parse(rectangleFData[2]), Single.Parse(rectangleFData[3])
              );

          /*
          case "System.Windows.Point":
            return System.Windows.Point.Parse(fullTextValue);
          case "System.Windows.Rect":
            return System.Windows.Rect.Parse(fullTextValue);
          case "System.Windows.Media.Color":
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(fullTextValue);
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
          case "System.Windows.Media.Matrix":
            return System.Windows.Media.Matrix.Parse(fullTextValue);
          case "System.Windows.Media.Media3D.Matrix3D":
            return System.Windows.Media.Media3D.Matrix3D.Parse(fullTextValue);*/

          case "Common.Percentage":
            return Percentage.Parse(fullTextValue, this.Settings.CultureInfo);
          case "Common.IO.Path":
            return new Path(fullTextValue);
        }
      } catch (Exception exception) {
        var ex = new XmlSerializationException("Parsing of value failed.", exception);
        ex.Data.Add("Type", type);
        ex.Data.Add("Value", fullTextValue);
        throw ex;
      }

      var excep = new ArgumentException("The given type is not natively deserializable.", "type");
      excep.Data.Add("Type", type);
      throw excep;
    }

    internal static Boolean IsNativelyDeserializable(Type type) {
      String typeName = type.FullName;

      return (
        type.IsEnum ||
        (typeof(System.Collections.IList).IsAssignableFrom(type) && !typeof(IXmlCustomSerialized).IsAssignableFrom(type)) || 
        typeName == "System.String" ||
        typeName == "System.Boolean" ||
        typeName == "System.Byte" ||
        typeName == "System.SByte" ||
        typeName == "System.Int16" ||
        typeName == "System.Int32" ||
        typeName == "System.Int64" ||
        typeName == "System.UInt16" ||
        typeName == "System.UInt32" ||
        typeName == "System.UInt64" ||
        typeName == "System.Single" ||
        typeName == "System.Double" ||
        typeName == "System.Decimal" ||
        typeName == "System.Char" ||
        typeName == "System.String" ||
        typeName == "System.DateTime" ||
        typeName == "System.DBNull" ||
        typeName == "System.Object" ||
        typeName == "Common.Percentage" ||
        typeName == "Common.IO.Path" ||
        typeName == "System.Version" ||
        typeName == "System.DateTimeOffset" ||
        typeName == "System.Guid" || 
        typeName == "System.Drawing.Color" || 
        typeName == "System.Drawing.Point" || 
        typeName == "System.Drawing.PointF" || 
        typeName == "System.Drawing.Rectangle" || 
        typeName == "System.Drawing.RectangleF" ||
        (type.Name == "KeyValuePair`2" && type.Namespace == "System.Collections.Generic") || 
        (typeof(System.Collections.IDictionary).IsAssignableFrom(type) && !typeof(IXmlCustomSerialized).IsAssignableFrom(type))
      );
    }
    #endregion
  }
}