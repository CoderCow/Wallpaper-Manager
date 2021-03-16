using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Xml;

namespace Common.IO.Serialization {
  public class XmlSerializationProvider: XmlSerializationProviderBase {
    #region Property: XmlWriter
    private readonly XmlWriter xmlWriter;

    public XmlWriter XmlWriter {
      get { return this.xmlWriter; }
    }
    #endregion

    #region Contract Invariant Method
    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.XmlWriter != null);
    }
    #endregion


    #region Methods: Constructor
    internal XmlSerializationProvider(
      IXmlSerializerInternal serializer, XmlWriter writer, String elementName, Boolean asAttribute = false, 
      XmlItemDefAttributeCollection itemDefAttributes = null
    ): base(serializer, elementName, asAttribute, itemDefAttributes) {
      if (serializer == null) throw new ArgumentNullException();
      if (writer == null) throw new ArgumentNullException();
      if (String.IsNullOrEmpty(elementName)) throw new ArgumentNullException();
      if (writer.WriteState != WriteState.Closed) throw new ArgumentException();

      this.xmlWriter = writer;
    }
    #endregion

    #region Methods: WriteRoot, WriteAll
    public void WriteRoot(Object instance, String customName = null, Boolean ignoreAttributes = false) {
      if (instance == null) throw new ArgumentNullException();
      if (this.AsAttribute) throw new InvalidOperationException();
      Contract.Requires<InvalidOperationException>(
        this.XmlWriter.WriteState == WriteState.Content ||
        this.XmlWriter.WriteState == WriteState.Element || 
        this.XmlWriter.WriteState == WriteState.Start
      );

      String rootName = customName;
      if (rootName == null)
        rootName = this.ElementName;

      XmlRootAttribute xmlRootAttribute = (this.Serializer.RootAttribute as XmlRootAttribute);

      if (xmlRootAttribute.Comment != null)
        this.XmlWriter.WriteComment(xmlRootAttribute.Comment);

      if (this.XmlWriter.WriteState == WriteState.Start)
        this.XmlWriter.WriteStartDocument();

      this.XmlWriter.WriteStartElement(rootName, xmlRootAttribute.XmlNamespace);

      if (!ignoreAttributes) {
        for (Int32 i = 0; i < this.Serializer.SerializableMembers.Count; i++) {
          XmlSerializableMember member = this.Serializer.SerializableMembers[i];
          if (member.MemberInfo.IsAttribute) {
            this.WriteMember(member, member.GetValue(instance), true);
          }
        }
      }
    }

    public void WriteAll(Object instance) {
      if (instance == null) throw new ArgumentNullException();
      Contract.Requires<InvalidOperationException>(
        this.XmlWriter.WriteState == WriteState.Content ||
        this.XmlWriter.WriteState == WriteState.Element || 
        this.XmlWriter.WriteState == WriteState.Start
      );

      if (this.XmlWriter.WriteState == WriteState.Start || !this.IsRoot)
        this.WriteRoot(instance);

      foreach (XmlSerializableMember member in this.Serializer.SerializableMembers) {
        if (!member.MemberInfo.IsAttribute)
          this.WriteMember(member, member.GetValue(instance), false);
      }
    }
    #endregion

    #region Methods: WriteMember, WriteAttribute, WriteElement
    /// <exception cref="XmlSerializationException">
    ///   The member contains a derived type which is not explicitly defined by an XmlTypeDefAttribute which is required by the 
    ///   current serialization settings.
    /// </exception>
    private void WriteMember(
      XmlSerializableMember member, Object memberValue, Boolean asAttribute, 
      IEnumerable<XmlItemDefAttribute> itemAttributeOverrides = null
    ) {
      XmlItemDefAttributeCollection itemDefAttributes = new XmlItemDefAttributeCollection(member.MemberInfo.ItemDefAttributes);
      if (itemAttributeOverrides != null) {
        itemDefAttributes.AddRange(itemAttributeOverrides);
      }

      String nameToWrite;
      if (memberValue != null) {
        Type objectType = memberValue.GetType();
        Boolean typeIsExplicitlyDefined;
        nameToWrite = member.MemberInfo.GetMemberNameByType(objectType, out typeIsExplicitlyDefined);
        if (!typeIsExplicitlyDefined && this.Settings.RequiresExplicitTypeDefinition) {
          var ex = new XmlSerializationException("The member contains a derived type which is not explicitly defined by an XmlTypeDefAttribute which is required by the current serialization settings.");
          ex.Data.Add("MemberInfo", member.MemberInfo);
          ex.Data.Add("Derived Type", objectType);
          throw ex;
        }
      } else {
        nameToWrite = member.MemberInfo.Name;
      }

      this.WriteNode(nameToWrite, memberValue, asAttribute, itemDefAttributes, member.MemberInfo.Comment);
    }

    /// <exception cref="InvalidOperationException">
    ///   The <see cref="XmlWriter" /> has to be in the write state: <see cref="WriteState.Element" />.
    /// </exception>
    public void WriteAttribute(String name, Object value, XmlItemDefAttributeCollection itemDefAttributes = null) {
      if (String.IsNullOrEmpty(name)) throw new ArgumentNullException();
      if (this.XmlWriter.WriteState == WriteState.Element) throw new InvalidOperationException();
      
      this.WriteNode(name, value, true, itemDefAttributes);
    }

    public void WriteElement(
      String name, Object value, XmlItemDefAttributeCollection itemDefAttributes = null, String comment = null
    ) {
      if (String.IsNullOrEmpty(name)) throw new ArgumentNullException();
      Contract.Requires<InvalidOperationException>(
        this.XmlWriter.WriteState == WriteState.Content ||
        this.XmlWriter.WriteState == WriteState.Element || 
        this.XmlWriter.WriteState == WriteState.Start
      );

      this.WriteNode(name, value, false, itemDefAttributes, comment);
    }
    #endregion

    #region Methods: WriteDefinedNode, WriteNode
    /// <exception cref="ArgumentException">
    ///   No serializable member with the given <paramref name="nodeName" /> found.
    /// </exception>
    public void WriteDefinedNode(String nodeName, Object value, IEnumerable<XmlItemDefAttribute> itemAttributeOverrides = null) {
      if (String.IsNullOrEmpty(nodeName)) throw new ArgumentNullException();
      Contract.Requires<InvalidOperationException>(
        this.XmlWriter.WriteState == WriteState.Content ||
        this.XmlWriter.WriteState == WriteState.Element || 
        this.XmlWriter.WriteState == WriteState.Start
      );

      XmlSerializableMember member;
      if (!this.Serializer.SerializableMembers.TryGetValue(nodeName, out member)) {
        var ex = new ArgumentException("No serializable member with the given node name found.");
        ex.Data.Add("Node Name", nodeName);
        throw ex;
      }

      this.WriteMember(member, value, true, itemAttributeOverrides);
    }

    private void WriteNode(
      String nodeName, Object value, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes, String comment = null
    ) {
      if (String.IsNullOrEmpty(nodeName)) throw new ArgumentNullException();

      if (!asAttribute && comment != null)
        this.XmlWriter.WriteComment(comment);

      Type objectType;
      if (value != null) {
        objectType = value.GetType();
        if (!XmlSerializationProvider.IsNativelySerializable(objectType)) {
          this.Serializer.GetSubSerializer(value.GetType()).SubSerialize(this.XmlWriter, value, nodeName, asAttribute, itemDefAttributes);
          return;
        }
      } else {
        objectType = null;
      }

      this.WriteNativeObject(nodeName, objectType, value, asAttribute, itemDefAttributes);
    }
    #endregion

    #region Methods: WriteNativeObject, IsNativelySerializable
    /// <exception cref="ArgumentException">
    ///   The given <paramref name="type" /> is not natively serializable.
    /// </exception>
    /// <exception cref="XmlSerializationException">
    ///   A type which implements <see cref="IEnumerable" /> can not be serialized as an attribute.
    /// </exception>
    private void WriteNativeObject(
      String nodeName, Type type, Object obj, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes
    ) {
      if (nodeName == null) throw new ArgumentNullException();

      if (!asAttribute)
        this.XmlWriter.WriteStartElement(nodeName);
      else
        this.XmlWriter.WriteStartAttribute(nodeName);

      if (obj == null) {
        if (asAttribute)
          this.XmlWriter.WriteValue(this.Settings.NullAttributeName);
        else
          this.XmlWriter.WriteAttributeString(this.Settings.NullAttributeName, true.ToString(this.Settings.CultureInfo));
      } else if (type.FullName != "System.String" && typeof(IEnumerable).IsAssignableFrom(type)) {
        if (asAttribute)
          throw new XmlSerializationException("A type which implements IEnumerable can not be serialized as an attribute.");

        foreach (Object item in (IEnumerable)obj) {
          Type itemType = item.GetType();
          String itemName;
          XmlItemDefAttribute itemDefAttribute;

          if (itemDefAttributes != null && itemDefAttributes.TryGetAttribute(itemType, out itemDefAttribute)) {
            itemName = itemDefAttribute.Name;
          } else {
            if (this.Settings.RequiresExplicitCollectionItemDefinition) {
              var ex = new XmlSerializationException("A collection contains an item which is not explicitly defined by an XmlItemDefAttribute which is required by the current serialization settings.");
              ex.Data.Add("Collection Type", type);
              ex.Data.Add("Item Type", itemType);
              throw ex;
            }

            itemName = itemType.FullName;
            // Mask nested class' full name if required.
            itemName = itemName.Replace('+', XmlSerializationProviderBase.NestedClassNameMaskCharacter);
          }

          this.WriteElement(itemName, item);
        }
      } else if (type.Name == "KeyValuePair`2" && type.Namespace == "System.Collections.Generic") {
        if (asAttribute)
          throw new XmlSerializationException("The native type KeyValuePair<,> is not serializable as an attribute.");

        Object key = type.GetProperty("Key").GetValue(obj, null);
        Object value = type.GetProperty("Value").GetValue(obj, null);
        this.WriteElement("Key", key);
        this.WriteElement("Value", value);
      } else if (type.IsEnum) {
        this.XmlWriter.WriteString(obj.ToString());
      } else {
        switch (type.FullName) {
          case "System.Drawing.Color":
            this.XmlWriter.WriteString(System.Drawing.ColorTranslator.ToHtml((System.Drawing.Color)obj));
            break;
          /*case "System.Windows.Media.Color":
            System.Windows.Media.Color mediaColor = (System.Windows.Media.Color)obj;
            Color color = Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
            this.XmlWriter.WriteString(System.Drawing.ColorTranslator.ToHtml(color));
            break;*/

          case "System.Boolean":
          case "System.Byte":
          case "System.SByte":
          case "System.Int16":
          case "System.Int32":
          case "System.Int64":
          case "System.UInt16":
          case "System.UInt32":
          case "System.UInt64":
          case "System.Single":
          case "System.Double":
          case "System.Decimal":
          case "System.Char":
          case "System.String":
          case "System.DateTime":
          case "System.DBNull":
            this.XmlWriter.WriteString(((IConvertible)obj).ToString(this.Settings.CultureInfo));
            break;

          case "System.DateTimeOffset":
            this.XmlWriter.WriteString(((System.DateTimeOffset)obj).ToString(this.Settings.CultureInfo));
            break;
         
          /*
          case "System.Windows.Point":
            this.XmlWriter.WriteString(((System.Windows.Point)obj).ToString(this.Settings.CultureInfo));
            break;
          case "System.Windows.Rect":
            this.XmlWriter.WriteString(((System.Windows.Rect)obj).ToString(this.Settings.CultureInfo));
            break;
          case "System.Windows.Media.Matrix":
            this.XmlWriter.WriteString(((System.Windows.Media.Matrix)obj).ToString(this.Settings.CultureInfo));
            break;
          case "System.Windows.Media.Media3D.Matrix3D":
            this.XmlWriter.WriteString(((System.Windows.Media.Media3D.Matrix3D)obj).ToString(this.Settings.CultureInfo));
            break;*/
          case "Common.Percentage":
            this.XmlWriter.WriteString(((Common.Percentage)obj).ToString(this.Settings.CultureInfo));
            break;

          case "System.Drawing.Point":
          case "System.Drawing.PointF":
          case "System.Drawing.Rectangle":
          case "System.Drawing.RectangleF":
          case "System.Object":
          case "System.Version":
          case "System.Guid":
          case "Common.IO.Path":
            this.XmlWriter.WriteString(obj.ToString());
            break;
          default:
            throw new ArgumentException("The given type is not natively serializable.", "type");
        }
      }

      if (!asAttribute) {
        this.XmlWriter.WriteEndElement();
      } else {
        this.XmlWriter.WriteEndAttribute();
      }
    }

    internal static Boolean IsNativelySerializable(Type type) {
      String typeName = type.FullName;

      return (
        type.IsEnum ||
        typeof(IEnumerable).IsAssignableFrom(type) ||
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
        typeName == "Common.Percentage" ||
        typeName == "Common.IO.Path" ||
        typeName == "System.Object" ||
        typeName == "System.Version" ||
        typeName == "System.DateTimeOffset" ||
        typeName == "System.Guid" ||
        (type.Name == "KeyValuePair`2" && type.Namespace == "System.Collections.Generic") ||
        typeName == "System.Drawing.Color" || 
        typeName == "System.Drawing.Point" || 
        typeName == "System.Drawing.PointF" || 
        typeName == "System.Drawing.Rectangle" || 
        typeName == "System.Drawing.RectangleF" ||
        typeName == "System.Windows.Point" ||
        typeName == "System.Windows.Rect" ||
        typeName == "System.Windows.Media.Color" ||
        typeName == "System.Windows.Media.Matrix" ||
        typeName == "System.Windows.Media.Media3D.Matrix3D"
      );
    }
    #endregion
  }
}