using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Xml;

using Common.ObjectModel.Collections;
using Common.Validation;

namespace Common.IO.Serialization {
  using SubSerializerCollection = Dictionary<Type,IXmlSerializerInternal>;

  public class XmlSerializer<T>: IXmlSerializer, IXmlSerializer<T>, IXmlSerializerInternal {
    public const String DefaultNullAttributeName = "Null";

    #region Property: TargetType
    private readonly Type targetType;

    public Type TargetType {
      get { return this.targetType; }
    }
    #endregion

    #region Property: Settings
    private readonly XmlSerializationSettings settings;

    public XmlSerializationSettings Settings {
      get { return this.settings; }
    }
    #endregion

    #region Property: IsCustomSerialized
    private Boolean isCustomSerialized;

    private Boolean IsCustomSerialized {
      get { return this.isCustomSerialized; }
    }
    #endregion

    #region Contract Invariant Method
    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.RootAttribute != null);
    }
    #endregion


    #region Methods: Constructors, ValidateAndGetRootAttribute, ReflectTypeData
    // This constructor is used by the derived non-generic serializer.
    /// <exception cref="ArgumentException">
    ///   The given type is not serializable or the <see cref="XmlSerializationSettings" /> are invalid.
    /// </exception>
    protected XmlSerializer(
      Type targetType, XmlSerializationSettings settings = default(XmlSerializationSettings), 
      XmlRootAttribute rootAttributeOverride = null, IEnumerable<Type> typesToCache = null,
      Boolean isGeneric = true
    ) {
      if (targetType == null) throw new ArgumentNullException();

      this.targetType = targetType;

      this.settings = settings;
      if (this.settings.Equals(default(XmlSerializationSettings)))
        this.settings = XmlSerializationSettings.Default;

      try {
        this.Settings.Validate();
      } catch (ValidationException exception) {
        throw new ArgumentException(
          String.Format(
            CultureInfo.CurrentCulture, 
            "The settings are not valid because: {0}. See inner exception for more details.", exception.Message
          ), "settings", exception);
      }

      if (this.Settings.NullAttributeName == null)
        this.settings.NullAttributeName = XmlSerializer.DefaultNullAttributeName;

      if (this.Settings.CultureInfo == null)
        this.settings.CultureInfo = CultureInfo.CurrentCulture;

      // Reflect basic type data and check if its possible to serialize/deserialize this type at all.
      try {
        this.ValidateAndGetRootAttribute(this.settings, rootAttributeOverride, out this.rootAttribute);
      } catch (XmlSerializationException exception) {
        String paramName = isGeneric ? "T" : "targetType";

        throw new ArgumentException(
          String.Format(
            CultureInfo.CurrentCulture, 
            "The target type is invalid because: {0}. See inner exception for more details.", 
            exception.Message
          ), paramName, exception);
      }

      this.subSerializers = new SubSerializerCollection(10);

      // Enumerate reflection data of each member.
      try {
        this.ReflectTypeData(typesToCache);
      } catch (XmlSerializationException exception) {
        String paramName = isGeneric ? "T" : "targetType";

        throw new ArgumentException(
          String.Format(
            CultureInfo.CurrentCulture, 
            "The target type is invalid because: {0}. See inner exception for more details.", 
            exception.Message
          ), paramName, exception);
      }
    }

    // This constructor is used for the root serializer created by the user.
    /// <exception cref="ArgumentException">
    ///   The given type is not serializable or the <see cref="XmlSerializationSettings" /> are invalid.
    /// </exception>
    public XmlSerializer(
      XmlSerializationSettings settings = default(XmlSerializationSettings), XmlRootAttribute rootAttributeOverride = null, 
      IEnumerable<Type> typesToCache = null
    ): this(typeof(T), settings, rootAttributeOverride, typesToCache) {}

    /// <remarks>
    ///   This constructor is used to create internal sub serializers.
    /// </remarks>
    /// <inheritdoc />
    private XmlSerializer(Type targetType, SubSerializerCollection subSerializers, XmlSerializationSettings settings) {
      if (targetType == null) throw new ArgumentNullException();
      if (subSerializers == null) throw new ArgumentNullException();

      this.targetType = targetType;
      
      // Reflect basic type data and check if its possible to serialize/deserialize this type at all.
      try {
        this.ValidateAndGetRootAttribute(settings, null, out this.rootAttribute);
      } catch (XmlSerializationException exception) {
        throw new ArgumentException(
          String.Format(
            CultureInfo.CurrentCulture, 
            "The target type is invalid because: {0}. See inner exception for more details.", 
            exception.Message
          ), "targetType", exception);
      }

      this.isSubSerializer = true;
      this.subSerializers = subSerializers;
      this.settings = settings;

      // Enumerate reflection data of each member.
      try {
        this.ReflectTypeData();
      } catch (XmlSerializationException exception) {
        throw new ArgumentException(
          String.Format(
            CultureInfo.CurrentCulture, 
            "The target type is not serializable because: {0}. See inner exception for more details.", 
            exception.Message
          ), "targetType", exception);
      }
    }

    /// <exception cref="ArgumentException">
    ///   <see cref="T" /> is no class or struct or is an abstract class.
    /// </exception>
    private void ValidateAndGetRootAttribute(
      XmlSerializationSettings settings, XmlRootAttribute rootAttributeOverride, out XmlRootAttribute rootAttribute
    ) {
      Contract.Ensures(Contract.ValueAtReturn<XmlRootAttribute>(out rootAttribute) != null);

      if (!this.TargetType.IsClass && !this.TargetType.IsValueType)
        throw new XmlSerializationException("Type argument T has to be either a class or struct type.");

      if (this.TargetType.IsAbstract)
        throw new XmlSerializationException("Type argument T can not be an abstract class.");

      if (this.TargetType.IsSubclassOf(typeof(Enum)))
        throw new XmlSerializationException("Type argument T can not be an enumeration.");

      if (rootAttributeOverride != null) {
        rootAttribute = rootAttributeOverride;
      } else {
        // Reflect the root attribute.
        rootAttribute = null;
        foreach(Object attribute in this.TargetType.GetCustomAttributes(false)) {
          XmlRootAttribute foundAttribute = (attribute as XmlRootAttribute);

          if (foundAttribute != null) {
            rootAttribute = foundAttribute;
            break;
          }
        }
      }

      if (rootAttribute == null) {
        if (!settings.RequiresExplicitXmlMetadataAttributes) {
          rootAttribute = new XmlRootAttribute();
        } else {
          var ex = new XmlSerializationException("The type is not serializable because it does not define an XmlRootAttribute.");
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }
      }

      // Get a name for the root attribute if necessary.
      if (this.rootAttribute.Name == null) {
        if (!XmlReader.IsName(this.TargetType.Name)) {
          var ex = new XmlSerializationException(
            "The XmlRootAttribute does not define a custom name and the name of the class is not a valid name for an xml element.");
          ex.Data.Add("Class Name", this.TargetType.Name);
          throw ex;
        }

        this.rootAttribute.Name = this.TargetType.Name;
      } else {
        if (!XmlReader.IsName(this.rootAttribute.Name)) {
          var ex = new XmlSerializationException("The XmlRootAttribute does not define a valid name for an xml element.");
          ex.Data.Add("Root Name", this.rootAttribute.Name);
          throw ex;
        }
      }
    }

    /// <exception cref="ArgumentException">
    ///   The given type implements the <see cref="IXmlCustomSerialized" /> interface but does not define a constructor accepting 
    ///   <see cref="XmlDeserializationProvider" /> as parameter.
    /// </exception>
    private void ReflectTypeData(IEnumerable<Type> typesToCache = null) {
      if (this.TargetType == null) throw new InvalidOperationException();

      this.isCustomSerialized = (this.TargetType.GetInterface("IXmlCustomSerialized", false) != null);

      if (this.IsCustomSerialized) {
        if (this.TargetType.GetConstructor(new[] { typeof(XmlDeserializationProvider) }) == null) {
          var ex = new XmlSerializationException("The type implements the IXmlCustomSerialized interface but does not define a constructor accepting XmlDeserializationProvider as parameter.");
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }
      } else if (!this.TargetType.IsValueType) { // A value type will always provide a parameterless constructor.
        if (this.Settings.AllowsDeserialization && this.TargetType.GetConstructor(new Type[] {}) == null) {
          var ex = new XmlSerializationException("The type has to implement a parameterless constructor to be deserialized.");
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }
      }

      // The types where reflection data has to be cached (where sub serializers have to be created) for.
      List<Type> typesToCacheNew = new List<Type>(20);
      if (typesToCache != null) {
        foreach (Type typeToCache in typesToCache) {
          typesToCacheNew.Add(typeToCache);
        }
      }

      // Reflect Members
      MemberInfo[] memberInfos = this.TargetType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      this.serializableMembers = new XmlSerializableMemberCollection();

      foreach (MemberInfo memberInfo in memberInfos) {
        XmlSerializableMember member = XmlSerializableMember.Create(memberInfo, typesToCacheNew, this.Settings);

        if (member != null)
          this.serializableMembers.Add(member);
      }
      // Sort the members by their OrderIndex value.
      this.serializableMembers.Sort();

      for (Int32 i = 0; i < typesToCacheNew.Count; i++) {
        Type typeToCache = typesToCacheNew[i];
        
        if (
          typeToCache != this.TargetType && typeToCache != null && !this.subSerializers.ContainsKey(typeToCache) && 
          !XmlSerializationProvider.IsNativelySerializable(typeToCache)
        ) {
          try {
            this.subSerializers.Add(typeToCache, new XmlSerializer<Object>(typeToCache, this.subSerializers, this.Settings));
          } catch (Exception exception) {
            var ex = new XmlSerializationException(
              "The type is not serializable. See inner exception for more details.", exception);
            ex.Data.Add("Type", typeToCache);
            throw ex;
          }
        }
      }
    }
    #endregion

    #region Methods: Serialize
    /// <exception cref="XmlSerializationException">
    ///   An error occurred while serializing the given <paramref name="instance" />.
    /// </exception>
    private void Serialize(XmlWriter xmlWriter, Object instance, XmlSerializationProvider provider) {
      if (xmlWriter == null) throw new ArgumentNullException();
      if (instance == null) throw new ArgumentNullException();
      if (provider == null) throw new ArgumentNullException();
      if (!this.TargetType.IsAssignableFrom(instance.GetType())) throw new ArgumentException();
      Contract.Requires<ArgumentException>(
        xmlWriter.WriteState == WriteState.Content ||
        xmlWriter.WriteState == WriteState.Element || 
        xmlWriter.WriteState == WriteState.Start
      );
      

      if (!this.IsCustomSerialized) {
        provider.WriteAll(instance);
      } else {
        try {
          ((IXmlCustomSerialized)instance).Serialize(provider);
        } catch (Exception exception) {
          var ex = new XmlSerializationException("An error occurred while serializing the type. See inner exception for more details.", exception);
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }
      }

      if (xmlWriter.WriteState != WriteState.Start && xmlWriter.WriteState != WriteState.Closed) {
        if (!this.isSubSerializer)
          xmlWriter.WriteEndDocument();
        else
          xmlWriter.WriteEndElement();
      }
    }

    /// <exception cref="XmlSerializationException">
    ///   An error occurred while serializing the given <paramref name="instance" />.
    /// </exception>
    public void Serialize(XmlWriter xmlWriter, T instance) {
      if (xmlWriter == null) throw new ArgumentNullException();
      if (instance == null) throw new ArgumentNullException();

      this.Serialize(xmlWriter, instance, new XmlSerializationProvider(this, xmlWriter, this.RootAttribute.Name));
    }

    /// <inheritdoc />
    public void Serialize(XmlWriter xmlWriter, Object instance) {
      if (xmlWriter == null) throw new ArgumentNullException();
      if (instance == null) throw new ArgumentNullException();
      if (!this.TargetType.IsAssignableFrom(instance.GetType())) throw new ArgumentException();

      this.Serialize(xmlWriter, instance, new XmlSerializationProvider(this, xmlWriter, this.RootAttribute.Name));
    }
    #endregion

    #region Methods: Deserialize
    /// <exception cref="XmlSerializationException">
    ///   An error occurred while deserializing the type.
    /// </exception>
    private Object Deserialize(XmlReader xmlReader, XmlDeserializationProvider xmlDeserializationProvider) {
      if (xmlReader == null) throw new ArgumentNullException();
      if (xmlDeserializationProvider == null) throw new ArgumentNullException();

      Object instance;
      if (!this.IsCustomSerialized) {
        try {
          instance = Activator.CreateInstance(this.TargetType);
        } catch (Exception exception) {
          var ex = new XmlSerializationException("The parameterless contructor of the type threw an exception. See inner exception for more details.", exception);
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }

        xmlDeserializationProvider.ReadAll(instance);
      } else {
        try {
          instance = Activator.CreateInstance(this.TargetType, xmlDeserializationProvider);
        } catch (Exception exception) {
          var ex = new XmlSerializationException("An error occurred while deserializing the type by its custom deserialization constructor. See inner exception for more details.", exception);
          ex.Data.Add("Type Name", this.TargetType.FullName);
          throw ex;
        }
      }

      if (xmlReader.NodeType == XmlNodeType.EndElement) {
        xmlReader.ReadEndElement();
      }

      return instance;
    }

    public T Deserialize(XmlReader xmlReader) {
      if (xmlReader == null) throw new ArgumentNullException();

      return (T)this.Deserialize(xmlReader, new XmlDeserializationProvider(this, xmlReader, this.RootAttribute.Name));
    }
    #endregion

    #region IXmlSerializer Implementation
    /// <inheritdoc />
    Object IXmlSerializer.Deserialize(XmlReader xmlReader) {
      if (xmlReader == null) throw new ArgumentNullException();

      return this.Deserialize(xmlReader, new XmlDeserializationProvider(this, xmlReader, this.RootAttribute.Name));
    }
    #endregion

    #region IXmlSerializerInternal Implementation
    #region Property: SerializableMembers
    private XmlSerializableMemberCollection serializableMembers;

    XmlSerializableMemberCollection IXmlSerializerInternal.SerializableMembers {
      get { return this.serializableMembers; }
    }
    #endregion

    #region Property: RootAttribute
    private readonly XmlRootAttribute rootAttribute;

    INamedAttribute IXmlSerializerInternal.RootAttribute {
      get { return this.rootAttribute; }
    }

    protected XmlRootAttribute RootAttribute {
      get { return this.rootAttribute; }
    }
    #endregion

    #region Property: SubSerializeres
    private readonly SubSerializerCollection subSerializers;

    SubSerializerCollection IXmlSerializerInternal.SubSerializers {
      get { return this.subSerializers; }
    }
    #endregion

    #region Property: IsSubSerializer
    private readonly Boolean isSubSerializer;

    Boolean IXmlSerializerInternal.IsSubSerializer {
      get { return this.isSubSerializer; }
    }
    #endregion

    #region Methods: SubSerialize, SubDeserialize, GetSubSerializer
    /// <exception cref="XmlSerializationException">
    ///   The type requires to implement <see cref="IXmlCustomSerialized" /> to be serialized as an attribute.
    /// </exception>
    void IXmlSerializerInternal.SubSerialize(
      XmlWriter xmlWriter, Object instance, String rootName, Boolean asAttribute, 
      XmlItemDefAttributeCollection itemDefAttributes
    ) {
      if (xmlWriter == null) throw new ArgumentNullException();
      if (instance == null) throw new ArgumentNullException();
      if (!this.TargetType.IsAssignableFrom(instance.GetType())) throw new ArgumentException();
      if (String.IsNullOrEmpty(rootName)) throw new ArgumentNullException();
      if (asAttribute && !this.IsCustomSerialized) {
        var ex = new XmlSerializationException("The type requires to implement IXmlCustomSerialized to be serialized as an attribute.");
        ex.Data.Add("Type Name", this.TargetType.FullName);
        throw ex;
      }

      this.Serialize(xmlWriter, instance, new XmlSerializationProvider(this, xmlWriter, rootName, asAttribute, itemDefAttributes));
    }

    /// <exception cref="XmlSerializationException">
    ///   The type requires to implement <see cref="IXmlCustomSerialized" /> to be deserialized as an attribute.
    /// </exception>
    Object IXmlSerializerInternal.SubDeserialize(
      XmlReader xmlReader, String rootName, Boolean asAttribute, XmlItemDefAttributeCollection itemDefAttributes
      ) {
      if (xmlReader == null) throw new ArgumentNullException();
      if (String.IsNullOrEmpty(rootName)) throw new ArgumentNullException();
      if (asAttribute && !this.IsCustomSerialized) {
        var ex = new XmlSerializationException("The type requires to implement IXmlCustomSerialized to be deserialized as an attribute.");
        ex.Data.Add("Type Name", this.TargetType.FullName);
        throw ex;
      }

      return this.Deserialize(xmlReader, new XmlDeserializationProvider(this, xmlReader, rootName, asAttribute, itemDefAttributes));
    }

    IXmlSerializerInternal IXmlSerializerInternal.GetSubSerializer(Type type) {
      if (type == null) throw new ArgumentNullException();
      Contract.Ensures(Contract.Result<IXmlSerializerInternal>() != null);

      IXmlSerializerInternal serializer;
      if (type == this.TargetType) {
        serializer = this;
      } else if (!this.subSerializers.TryGetValue(type, out serializer)) {
        serializer = new XmlSerializer<Object>(type, this.subSerializers, this.Settings);
      }

      return serializer;
    }
    #endregion
    #endregion

    #region ToString
    public override String ToString() {
      return String.Concat("TargetType = ", this.TargetType.FullName);
    }
    #endregion
  }

  public class XmlSerializer: XmlSerializer<Object> {
    // This constructor is used for the derived non-generic serializer only.
    /// <exception cref="ArgumentException">
    ///   The given type is not serializable or the <see cref="XmlSerializationSettings" /> are invalid.
    /// </exception>
    public XmlSerializer(
      Type targetType, XmlSerializationSettings settings = default(XmlSerializationSettings), 
      XmlRootAttribute rootAttributeOverride = null, IEnumerable<Type> typesToCache = null
    ): base(targetType, settings, rootAttributeOverride, typesToCache, false) {}
  }
}