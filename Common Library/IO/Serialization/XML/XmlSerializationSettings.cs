using System;
using System.Globalization;

using Common.Validation;

namespace Common.IO.Serialization {
  public struct XmlSerializationSettings {
    #region Static Property: Default
    /// <summary>
    ///   Gets a set of settings which are recommended to serialize/deserialize types in general.
    /// </summary>
    public static XmlSerializationSettings Default {
      get {
        return new XmlSerializationSettings {
          CultureInfo = CultureInfo.CurrentUICulture,
          RequiresExplicitOrder = false,
          RequiresExplicitTypeDefinition = true,
          RequiresExplicitCollectionItemDefinition = true,
          RequiresExplicitXmlMetadataAttributes = true,
          AllowsSerialization = true,
          AllowsDeserialization = true
        };
      }
    }
    #endregion

    #region Static Property: AnonymousType
    /// <summary>
    ///   Gets a set of settings which are recommended to serialize anonymous types. 
    ///   Note that this setting causes the serializer to not allow deserializations at all.
    /// </summary>
    public static XmlSerializationSettings AnonymousType {
      get {
        return new XmlSerializationSettings {
          CultureInfo = CultureInfo.CurrentUICulture,
          RequiresExplicitOrder = false,
          RequiresExplicitTypeDefinition = false,
          RequiresExplicitCollectionItemDefinition = false,
          RequiresExplicitXmlMetadataAttributes = false,
          AllowsSerialization = true,
          AllowsDeserialization = false
        };
      }
    }
    #endregion

    #region Property: CultureInfo
    private CultureInfo cultureInfo;

    /// <summary>
    ///   Defines the culture to be used when formatting numbers and other culture dependend data. This setting is set to
    ///   <see cref="System.Globalization.CultureInfo.CurrentUICulture" /> by default.
    /// </summary>
    public CultureInfo CultureInfo {
      get { return this.cultureInfo; }
      set { this.cultureInfo = value; }
    }
    #endregion
    
    #region Property: RequiresExplicitOrder
    private Boolean requiresExplicitOrder;

    /// <summary>
    ///   Indicates whether an order index value has to be explicitly defined for each node or not.
    /// </summary>
    public Boolean RequiresExplicitOrder {
      get { return this.requiresExplicitOrder; }
      set { this.requiresExplicitOrder = value; }
    }
    #endregion

    #region Property: RequiresExplicitTypeDefinition
    private Boolean requiresExplicitTypeDefinition;

    /// <summary>
    ///   Indicates whether explicit type definitions by <see cref="XmlNodeTypeDefAttribute" /> instances are required or not.
    /// </summary>
    public Boolean RequiresExplicitTypeDefinition {
      get { return this.requiresExplicitTypeDefinition; }
      set { this.requiresExplicitTypeDefinition = value; }
    }
    #endregion

    #region Property: RequiresExplicitCollectionItemDefinition
    private Boolean requiresExplicitCollectionItemDefinition;

    /// <summary>
    ///   Indicates whether explicit type definitions by <see cref="XmlItemDefAttribute" /> instances are required or not.
    /// </summary>
    public Boolean RequiresExplicitCollectionItemDefinition {
      get { return this.requiresExplicitCollectionItemDefinition; }
      set { this.requiresExplicitCollectionItemDefinition = value; }
    }
    #endregion

    #region Property: RequiresExplicitXmlMetadataAttributes
    private Boolean requiresExplicitXmlMetadataAttributes;

    /// <summary>
    ///   Indicates whether explicit <see cref="XmlRootAttribute" /> and <see cref="XmlNodeAttribute" /> attributes 
    ///   for each serializable type and members are required or if any public type, property and field should be serialized.
    /// </summary>
    /// <remarks>
    ///   This value is <c>true</c> by default and its not recommended to set it to <c>false</c> because the 
    ///   <see cref="XmlSerializer{T}" /> is designed to work with the opt-in behavior. However, a good case
    ///   to set this property to <c>false</c> is a one-way serialization behavior such as a serialization of an
    ///   anonymous type.
    /// </remarks>
    public Boolean RequiresExplicitXmlMetadataAttributes {
      get { return this.requiresExplicitXmlMetadataAttributes; }
      set { this.requiresExplicitXmlMetadataAttributes = value; }
    }
    #endregion

    #region Property: AllowsSerialization
    private Boolean allowsSerialization;

    /// <summary>
    ///   Indicates whether the <see cref="XmlSerializer{T}" /> supports serialization for the given type at all.
    ///   Set this property to <c>true</c> if you plan to perform one-way deserializations of the type only.
    /// </summary>
    public Boolean AllowsSerialization {
      get { return this.allowsSerialization; }
      set { this.allowsSerialization = value; }
    }
    #endregion

    #region Property: AllowsDeserialization
    private Boolean allowsDeserialization;

    /// <summary>
    ///   Indicates whether the <see cref="XmlSerializer{T}" /> supports deserialization for the given type at all.
    ///   Set this property to <c>true</c> if you plan to perform one-way serializations of the type only, for example
    ///   if the type neither does implement <see cref="IXmlCustomSerialized" /> or a parameterless constructor.
    /// </summary>
    public Boolean AllowsDeserialization {
      get { return this.allowsDeserialization; }
      set { this.allowsDeserialization = value; }
    }
    #endregion

    #region Property: NullAttributeName
    private String nullAttributeName;

    /// <summary>
    ///   If a serializable member returns a <c>null</c> value or an <c>null</c> value has to be deserialized, this
    ///   setting defines the attribute name of an element or the value of an serializable attribute to indicate a 
    ///   null value.
    /// </summary>
    public String NullAttributeName {
      get { return this.nullAttributeName; }
      set { this.nullAttributeName = value; }
    }
    #endregion


    #region Methods: Validate
    /// <summary>
    ///   Checkes whether the settings are compatible with each other and throws an exception if not.
    /// </summary>
    public void Validate() {
      if (!this.AllowsSerialization && !this.AllowsDeserialization)
        throw new ValidationException("The XmlSerializationSettings should allow at least one way of serialization.");

      if (!this.RequiresExplicitXmlMetadataAttributes)
        if (this.RequiresExplicitOrder || this.RequiresExplicitTypeDefinition || 
          this.RequiresExplicitCollectionItemDefinition)
            throw new ValidationException(
              "RequiresExplicitXmlMetadataAttributes setting can not be false while RequiresExplicitOrder, " + 
              "RequiresExplicitTypeDefinition or RequiresExplicitCollectionItemDefinition is true");
    }
    #endregion
  }
}