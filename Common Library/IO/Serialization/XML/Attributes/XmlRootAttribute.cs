using System;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  /// <summary>
  ///   Defines metadata for a class or structure to control the serialization process by using <see cref="XmlSerializer" />.
  /// </summary>
  /// <seealso cref="XmlSerializer{T}">XmlSerializer Class</seealso>
  /// <seealso cref="Attribute">Attribute Class</seealso>
  /// <threadsafety static="true" instance="false" />
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  public class XmlRootAttribute: Attribute, INamedAttribute {
    #region Property: Name
    /// <summary>
    ///   <inheritdoc cref="Name" select='../value/node()' />
    /// </summary>
    private String name;

    /// <summary>
    ///   Gets or sets the name which should be used when an instance of this type is serialized.
    /// </summary>
    /// <value>
    ///   The name which should be used when an instance of this type is serialized.
    /// </value>
    /// <remarks>
    ///   Set this property to <c>null</c> to indicate that the name of the type where this attribute is applied to should be
    ///   used.
    /// </remarks>
    public String Name {
      get { return this.name; }
      set {  this.name = value; }
    }
    #endregion

    #region Property: XmlNamespace
    /// <summary>
    ///   <inheritdoc cref="XmlNamespace" select='../value/node()' />
    /// </summary>
    private String xmlNamespace;

    /// <summary>
    ///   Gets or sets the XML-Namespace of the root node.
    /// </summary>
    /// <value>
    ///   The XML-Namespace of the root node.
    /// </value>
    public String XmlNamespace {
      get { return this.xmlNamespace; }
      set {
        if (value != null && !XmlUtil.IsValidAttributeValue(value)) throw new ArgumentException();
        this.xmlNamespace = value;
      }
    }
    #endregion

    #region Property: Comment
    private String comment;

    public String Comment {
      get { return this.comment; }
      set { this.comment = value; }
    }
    #endregion


    #region Methods: Constructors
    public XmlRootAttribute() {}

    public XmlRootAttribute(String name, String xmlNamespace) {
      this.name = name;
      this.xmlNamespace = xmlNamespace;
    }

    public XmlRootAttribute(String name): this(name, null) {}
    #endregion
  }
}