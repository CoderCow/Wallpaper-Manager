using System;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
  public class XmlNodeTypeDefAttribute: Attribute, INamedAttribute, IAssignable {
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


    public XmlNodeTypeDefAttribute(String name, Type type) {
      this.name = name;
      this.type = type;
    }

    #region IAssignable Implementation
    public void AssignTo(Object other) {
      if (other == null) throw new ArgumentNullException();
      if (!(other is XmlNodeTypeDefAttribute)) throw new ArgumentException();

      var otherInstance = (XmlNodeTypeDefAttribute)other;
      otherInstance.name = this.name;
      otherInstance.type = this.type;
    }
    #endregion
  }
}