using System;
using System.Collections;
using System.Diagnostics.Contracts;

using Common.Text;

namespace Common.IO.Serialization {
  public class XmlMemberInfo: IComparable<XmlMemberInfo> {
    #region Property: Name
    private readonly String name;
	  
    public String Name {
      get { return this.name; }
    }
    #endregion

    #region Property: Type
    private readonly Type type;
	  
    public Type Type {
      get { return this.type; }
    }
    #endregion

    #region Property: OrderIndex
    private readonly Int32 orderIndex;
	  
    public Int32 OrderIndex {
      get { return this.orderIndex; }
    }
    #endregion

    #region Property: Comment
    private readonly String comment;

    public String Comment {
      get { return this.comment; }
    }
    #endregion

    #region Property: IsAttribute
    private readonly Boolean isAttribute;
	  
    public Boolean IsAttribute {
      get { return this.isAttribute; }
    }
    #endregion

    #region Property: IsStatic
    private readonly Boolean isStatic;
	  
    public Boolean IsStatic {
      get { return this.isStatic; }
    }
    #endregion

    #region Property: IsCollection
    private readonly Boolean isCollection;

    public Boolean IsCollection {
      get { return this.isCollection; }
    }
    #endregion
    
    #region Property: CollectionItemType
    private readonly Type collectionItemType;

    public Type CollectionItemType {
      get { return this.collectionItemType; }
    }
    #endregion

    #region Property: ItemDefAttributes
    private readonly XmlItemDefAttributeCollection itemDefAttributes;

    public XmlItemDefAttributeCollection ItemDefAttributes {
      get { return this.itemDefAttributes; }
    }
    #endregion

    #region Property: TypeDefAttributes
    private readonly XmlTypeDefAttributeCollection typeDefAttributes;

    public XmlTypeDefAttributeCollection TypeDefAttributes {
      get { return this.typeDefAttributes; }
    }
    #endregion


    #region Method: Constructor
    internal XmlMemberInfo(
      String name, Type type, Int32 orderIndex, String comment, Boolean isAttribute, Boolean isStatic, Boolean isCollection, 
      Type collectionItemType, XmlItemDefAttributeCollection itemDefAttributes, XmlTypeDefAttributeCollection typeDefAttributes
    ) {
      if (name == null) throw new ArgumentNullException();
      if (type == null) throw new ArgumentNullException();
      if (itemDefAttributes == null) throw new ArgumentNullException();
      if (typeDefAttributes == null) throw new ArgumentNullException();
      if (orderIndex < -1) throw new ArgumentOutOfRangeException();

      this.name = name;
      this.type = type;
      this.orderIndex = orderIndex;
      this.comment = comment;
      this.isAttribute = isAttribute;
      this.isStatic = isStatic;
      this.isCollection = isCollection;
      this.collectionItemType = collectionItemType;
      this.itemDefAttributes = itemDefAttributes;
      this.typeDefAttributes = typeDefAttributes;
    }
    #endregion

    #region Methods: GetMemberNameByType, TryGetMemberTypeByName
    public String GetMemberNameByType(Type type, out Boolean isExplicitlyDefined) {
      if (type == null) throw new ArgumentNullException();
      Contract.Ensures(Contract.Result<String>() != null);

      isExplicitlyDefined = true;
      if (type == this.type)
        return this.Name;

      XmlNodeTypeDefAttribute typeDefAttribute;
      if (this.TypeDefAttributes.TryGetAttribute(type, out typeDefAttribute))
        return typeDefAttribute.Name;

      // Mask nested class' full name if required.
      isExplicitlyDefined = false;
      return type.FullName.Replace('+', XmlSerializationProviderBase.NestedClassNameMaskCharacter);
    }

    public Boolean TryGetMemberTypeByName(String name, out Type type) {
      if (name == null) throw new ArgumentNullException();

      if (name == this.Name) {
        type = this.Type;
        return true;
      }

      XmlNodeTypeDefAttribute typeDefAttribute;
      if (this.TypeDefAttributes.TryGetAttribute(name, out typeDefAttribute)) {
        type = typeDefAttribute.Type;
        return true;
      }

      type = null;
      return false;
    }
    #endregion

    #region IComparable Implementation
    public Int32 CompareTo(XmlMemberInfo other) {
      if (this.OrderIndex == -1 && other.OrderIndex == -1)
        return 0;
      if (this.OrderIndex == -1)
        return -1;
      if (other.OrderIndex == -1)
        return +1;

      return this.OrderIndex.CompareTo(other.OrderIndex);
    }
    #endregion

    public override String ToString() {
      return StringGenerator.FromListKeyed(new[] { "Name", "IsAttribute" }, new Object[] { this.Name, this.IsAttribute });
    }
  }
}