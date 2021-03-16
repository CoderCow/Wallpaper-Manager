using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Common.IO.Serialization {
  public abstract class XmlSerializationProviderBase {
    internal const Char NestedClassNameMaskCharacter = '-';

    #region Property: Serializer
    private readonly IXmlSerializerInternal serializer;

    internal IXmlSerializerInternal Serializer {
      get { return this.serializer; }
    }
    #endregion

    #region Property: Settings
    public XmlSerializationSettings Settings {
      get { return this.Serializer.Settings; }
    }
    #endregion

    #region Property: AsAttribute
    private readonly Boolean asAttribute;

    public Boolean AsAttribute {
      get { return this.asAttribute; }
    }
    #endregion

    #region Property: IsRoot
    public Boolean IsRoot {
      get { return !this.Serializer.IsSubSerializer; }
    }
    #endregion

    #region Property: ElementName
    private readonly String elementName;

    public String ElementName {
      get { return this.elementName; }
    }
    #endregion

    #region Property: ItemDefAttributes
    private readonly XmlItemDefAttributeCollection itemDefAttributes;

    public XmlItemDefAttributeCollection ItemDefAttributes {
      get { return this.itemDefAttributes; }
    }
    #endregion

    #region Contract Invariant Method
    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.serializer != null);
      Contract.Invariant(!(this.asAttribute && this.IsRoot));
    }
    #endregion


    #region Method: Constructor
    internal XmlSerializationProviderBase(
      IXmlSerializerInternal serializer, String elementName, Boolean asAttribute = false, XmlItemDefAttributeCollection itemDefAttributes = null
    ) {
      if (serializer == null) throw new ArgumentNullException();
      if (String.IsNullOrEmpty(elementName)) throw new ArgumentNullException();

      this.serializer = serializer;
      this.elementName = elementName;
      this.asAttribute = asAttribute;
      this.itemDefAttributes = itemDefAttributes;
    }
    #endregion

    #region Methods: GetMemberInfo, GetMemberInfos
    public XmlMemberInfo GetMemberInfo(String name) {
      XmlSerializableMember member;
      if (this.Serializer.SerializableMembers.TryGetValue(name, out member)) {
        return member.MemberInfo;
      }

      return null;
    }

    public IList<XmlMemberInfo> GetMemberInfos() {
      Contract.Ensures(Contract.Result<IList<XmlMemberInfo>>() != null);

      return this.Serializer.SerializableMembers.ToMemberInfos();
    }
    #endregion

    public override String ToString() {
      return String.Concat("Type = ", this.Serializer.TargetType.FullName);
    }
  }
}