using System;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class XmlItemDefAttribute: Attribute, INamedAttribute, IAssignable {
    #region Property: Name
	  private String name;

    public String Name {
      get { return this.name; }
      set { this.name = value; }
    }
    #endregion

    #region Property: Type
	  private Type type;

    public Type Type {
      get { return this.type; }
      set {
        if (value == null) throw new ArgumentNullException();
        this.type = value;
      }
    }
    #endregion


	  public XmlItemDefAttribute(String name, Type type) {
	    this.name = name;
	    this.type = type;
	  }

    public void AssignTo(Object other) {
      if (!(other is XmlItemDefAttribute)) throw new ArgumentException();

      XmlItemDefAttribute otherInstance = (XmlItemDefAttribute)other;
      otherInstance.name = this.name;
      otherInstance.type = this.type;
    }
  }
}