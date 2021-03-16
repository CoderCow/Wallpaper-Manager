using System;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class XmlNodeAttribute: Attribute, INamedAttribute {
    #region Property: Name
    private String name;

    public String Name {
      get { return this.name; }
      set { this.name = value; }
    }
    #endregion

    #region Property: IsAttribute
    private Boolean isAttribute;

    public Boolean IsAttribute {
      get { return this.isAttribute; }
      set { this.isAttribute = value; }
    }
    #endregion
	  
    #region Property: OrderIndex
    private Int32 orderIndex;

    public Int32 OrderIndex {
      get { return this.orderIndex; }
      set { this.orderIndex = value; }
    }
    #endregion

    #region Property: Comment
    private String comment;

    public String Comment {
      get { return this.comment; }
      set { this.comment = value; }
    }
    #endregion


    public XmlNodeAttribute(String name, Boolean isAttribute): this() {
      this.name = name;
      this.isAttribute = isAttribute;
    }

    public XmlNodeAttribute(String name): this(name, false) {}

    public XmlNodeAttribute() {
      this.orderIndex = -1;
    }
  }
}