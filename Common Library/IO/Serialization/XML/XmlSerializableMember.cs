using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Common.IO.Serialization {
  internal class XmlSerializableMember: IComparable<XmlSerializableMember> {
    #region Fields: fieldInfo, propertyInfo
    private readonly FieldInfo fieldInfo;
    private readonly PropertyInfo propertyInfo;
    #endregion

    #region Property: MemberInfo
    private XmlMemberInfo memberInfo;

    public XmlMemberInfo MemberInfo {
      get { return this.memberInfo; }
    }
    #endregion


    #region Static Method: Create
    /// <exception cref="ArgumentException">
    ///   The reflected data are invalid.
    /// </exception>
    public static XmlSerializableMember Create(
      MemberInfo memberInfo, ICollection<Type> typesToCache, XmlSerializationSettings settings
    ) {
      if (memberInfo == null) throw new ArgumentNullException();
      if (typesToCache == null) throw new ArgumentNullException();

      FieldInfo fieldInfo = (memberInfo as FieldInfo);
      PropertyInfo propertyInfo = (memberInfo as PropertyInfo);
      if (fieldInfo == null && propertyInfo == null)
        return null;

      XmlSerializableMember serializableMember = new XmlSerializableMember(fieldInfo, propertyInfo);
      Type memberType;
      if (fieldInfo != null)
        memberType = fieldInfo.FieldType;
      else
        memberType = propertyInfo.PropertyType;

      // This check includes IList, ICollection, IEnumerable<T>, IList<T>, ICollection<T>.
      Type collectionItemType = null;
      Boolean isCollection = (memberType != typeof(String) && typeof(IEnumerable).IsAssignableFrom(memberType));
      Boolean isDictionary = (typeof(IDictionary).IsAssignableFrom(memberType) || typeof(IDictionary<,>).IsAssignableFrom(memberType));
      if (isCollection) {
        if (memberType.IsGenericType) {
          if (!isDictionary) {
            collectionItemType = memberType.GetGenericArguments()[0];
          } else {
            Type[] genericArgumentTypes = memberType.GetGenericArguments();
            // TODO: There may be a better approach for this.
            collectionItemType = Type.GetType(String.Format(
              "System.Collections.Generic.KeyValuePair`2[[{0}],[{1}]]", 
              genericArgumentTypes[0].AssemblyQualifiedName, genericArgumentTypes[1].AssemblyQualifiedName
            ));
          }
        } else {
          if (!isDictionary)
            collectionItemType = typeof(Object);
          else
            collectionItemType = typeof(KeyValuePair<Object,Object>);
        }
      }

      Boolean isSerializable = false;
      String memberName = null;
      Int32 orderIndex = 0;
      String comment = null;
      Boolean isAttribute = false;
      XmlItemDefAttributeCollection itemDefAttributes = new XmlItemDefAttributeCollection();
      XmlTypeDefAttributeCollection typeDefAttributes = new XmlTypeDefAttributeCollection();

      if (!settings.RequiresExplicitXmlMetadataAttributes) {
        // With this settings, any field or property is serializable by default.
        memberName = memberInfo.Name;
        isSerializable = true;
      }

      foreach (Attribute attribute in memberInfo.GetCustomAttributes(true)) {
        if (attribute is XmlNodeAttribute) {
          XmlNodeAttribute nodeAttribute = (XmlNodeAttribute)attribute;

          if (nodeAttribute.Name == null)
            memberName = memberInfo.Name;
          else
            memberName = nodeAttribute.Name;

          if (nodeAttribute.OrderIndex == -1 && settings.RequiresExplicitOrder) {
            var ex = new ArgumentException("The XmlNodeAttribute on the member does not define an explicit order index as required by the current serialization settings.");
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }
          comment = nodeAttribute.Comment;
          orderIndex = nodeAttribute.OrderIndex;
          isAttribute = nodeAttribute.IsAttribute;

          isSerializable = true;

          continue;
        }

        if (attribute is XmlNodeTypeDefAttribute) {
          XmlNodeTypeDefAttribute typeDefAttribute = (XmlNodeTypeDefAttribute)attribute;
          if (memberType == typeDefAttribute.Type) {
            var ex = new ArgumentException("XmlNodeTypeDefAttribute can not be of the same type as the member.");
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }
          if (!memberType.IsAssignableFrom(typeDefAttribute.Type)) {
            var ex = new ArgumentException("XmlNodeTypeDefAttribute defined with a type which is not castable to the member's type.");
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }

          typeDefAttributes.Add(typeDefAttribute);
        }

        if (attribute is XmlItemDefAttribute) {
          if (!isCollection) {
            var ex = new ArgumentException("XmlItemDefAttribute defined for a member of a type that does not implement the IEnumerable or IEnumerable<T> interface.");
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }

          XmlItemDefAttribute itemDefAttribute = (XmlItemDefAttribute)attribute;

          // Check if the type defined by the XmlItemDefAttribute is derived or equal to the collection's item type.
          if (!collectionItemType.IsAssignableFrom(itemDefAttribute.Type)) {
            var ex = new ArgumentException("The type given by the XmlItemDefAttribute is not assignable to the item type of the collection.");
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }

          // Check if we have an item attribute for this type already.
          if (itemDefAttributes.Contains(itemDefAttribute.Type)) {
            var ex = new ArgumentException("Only one XmlItemDefAttribute is allowed for the same item type.");
            ex.Data.Add("Item Type", itemDefAttribute.Type);
            ex.Data.Add("Member Info", memberInfo);
            throw ex;
          }

          itemDefAttributes.Add(itemDefAttribute);
          typesToCache.Add(itemDefAttribute.Type);
        }
      }

      if (!isSerializable) {
        return null;
      }

      if (settings.RequiresExplicitCollectionItemDefinition && isCollection && !itemDefAttributes.ContainsType(collectionItemType)) {
        var ex = new ArgumentException("The collection member does not define an explicit XmlItemDefAttribute for its type as required by the current serialization settings.");
        ex.Data.Add("Member Info", memberInfo);
        ex.Data.Add("Item Type", collectionItemType);
        throw ex;
      }

      Boolean isStatic;
      if (fieldInfo != null) {
        isStatic = fieldInfo.IsStatic;
      } else {
        // We need at least one accessor to check whether the property is static etc.
        MethodInfo[] accessors = propertyInfo.GetAccessors(true);
        if (accessors.Length == 0) {
          var ex = new ArgumentException("A XML-Serializable property requires at least one accessor.");
          ex.Data.Add("Property Info", serializableMember.MemberInfo);
          throw ex;
        }

        MethodInfo accessor = accessors[0];
        if (accessor.IsAbstract) {
          var ex = new ArgumentException("A XML-Serializable property can not be abstract.");
          ex.Data.Add("Property Info", serializableMember.MemberInfo);
          throw ex;
        }

        isStatic = accessor.IsStatic;
      }

      // If any field or property should be serializable, we may not include static members.
      if (isStatic && !settings.RequiresExplicitXmlMetadataAttributes)
        return null;
      
      serializableMember.memberInfo = new XmlMemberInfo(
        memberName, memberType, orderIndex, comment, isAttribute, isStatic, isCollection, collectionItemType, 
        itemDefAttributes, typeDefAttributes
      );

      if (collectionItemType != null) {
        typesToCache.Add(collectionItemType);
      }
      typesToCache.Add(memberType);
      return serializableMember;
    }
    #endregion

    #region Method: Constructor
    private XmlSerializableMember(FieldInfo fieldInfo, PropertyInfo propertyInfo) {
      if (fieldInfo == null && propertyInfo == null) throw new ArgumentNullException();

      this.fieldInfo = fieldInfo;
      this.propertyInfo = propertyInfo;
    }
    #endregion

    #region Methods: GetValue, SetValue
    /// <exception cref="XmlSerializationException">
    ///   An error occurred while getting the value of the field or property.
    /// </exception>
    public Object GetValue(Object instance) {
      if (instance == null) throw new ArgumentNullException();
      Contract.Assert(this.fieldInfo != null || this.propertyInfo != null);

      if (this.fieldInfo != null) {
        try {
          return this.fieldInfo.GetValue(instance);
        } catch (Exception exception) {
          var ex = new XmlSerializationException("The value of the field could not be get. See exception details for more information.", exception);
          ex.Data.Add("Field Info", this.fieldInfo);
          throw ex;
        }
      }

      try {
        return this.propertyInfo.GetValue(instance, null);
      } catch (Exception exception) {
        var ex = new XmlSerializationException("The value of the property could not be get. See exception details for more information.", exception);
        ex.Data.Add("Property Info", this.propertyInfo);
        throw ex;
      }
    }

    /// <exception cref="XmlSerializationException">
    ///   An error occurred while setting the value of the field or property.
    /// </exception>
    public void SetValue(Object instance, Object value) {
      if (instance == null) throw new ArgumentNullException();
      Contract.Assert(this.fieldInfo != null || this.propertyInfo != null);

      if (this.fieldInfo != null) {
        try {
          this.fieldInfo.SetValue(instance, value);
        } catch (Exception exception) {
          var ex = new XmlSerializationException("The value of the field could not be set. See exception details for more information.", exception);
          ex.Data.Add("Field Info", this.fieldInfo);
          throw ex;
        }

        return;
      }

      try {
        this.propertyInfo.SetValue(instance, value, null);
      } catch (Exception exception) {
        var ex = new XmlSerializationException("The value of the property could not be set. See exception details for more information.", exception);
        ex.Data.Add("Property Info", this.propertyInfo);
        throw ex;
      }
    }
    #endregion

    #region Methods: GetCollectionItems, SetListItems
    public ReadOnlyCollection<Object> GetCollectionItems(Object instance) {
      if (instance == null) throw new ArgumentNullException();
      if (!this.MemberInfo.IsCollection) throw new InvalidOperationException();
      Contract.Ensures(Contract.Result<ReadOnlyCollection<Object>>() != null);

      IEnumerable items = (this.GetValue(instance) as IEnumerable);
      ICollection collection = (items as ICollection);

      List<Object> newList;
      if (collection != null)
        newList = new List<Object>(collection.Count);
      else
        newList = new List<Object>();

      foreach (Object item in items)
        newList.Add(item);

      return new ReadOnlyCollection<Object>(newList);
    }

    /// <exception cref="XmlSerializationException">
    ///   An error occurred while getting the items of the field or property.
    /// </exception>
    public void SetListItems(Object instance, IEnumerable<Object> items) {
      if (instance == null) throw new ArgumentNullException();
      if (items == null) throw new ArgumentNullException();
      if (!this.MemberInfo.IsCollection) throw new InvalidOperationException();

      IList collection = (this.GetValue(instance) as IList);
      if (collection == null) {
        var ex = new XmlSerializationException("The member needs to implement IList to have its items being deserialized.");
        if (this.propertyInfo != null)
          ex.Data.Add("Field Info", this.propertyInfo);
        else
          ex.Data.Add("Property Info", this.propertyInfo);

        throw ex;
      }

      foreach (Object item in items)
        collection.Add(item);
    }
    #endregion

    #region IComparable Implementation
    public Int32 CompareTo(XmlSerializableMember other) {
      return this.MemberInfo.CompareTo(other.MemberInfo);
    }
    #endregion

    public override String ToString() {
      return this.MemberInfo.ToString();
    }
  }
}