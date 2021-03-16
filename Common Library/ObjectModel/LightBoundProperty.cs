using System;
using System.Diagnostics.Contracts;

namespace Common.ObjectModel {
  /// <summary>
  ///   Contains data related to a synchronized property.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public struct LightBoundProperty {
    #region Property: SourceName
    /// <summary>
    ///   <inheritdoc cref="SourceName" select='../value/node()' />
    /// </summary>
    private String sourceName;

    /// <summary>
    ///   Gets the name of the property of the source object to be bound.
    /// </summary>
    /// <value>
    ///   The name of the property of the source object to be bound.
    /// </value>
    public String SourceName {
      get { return this.sourceName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (value.Length == 0) throw new ArgumentOutOfRangeException();

        this.sourceName = value;
      }
    }
    #endregion

    #region Property: TargetName
    /// <summary>
    ///   <inheritdoc cref="TargetName" select='../value/node()' />
    /// </summary>
    private String targetName;

    /// <summary>
    ///   Gets or sets the name of the property of the target object to be bound.
    /// </summary>
    /// <value>
    ///   The name of the property of the target object to be bound.
    /// </value>
    public String TargetName {
      get { return this.targetName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (value.Length == 0) throw new ArgumentOutOfRangeException();

        this.targetName = value;
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="LightBoundProperty">LightBoundProperty Structure</see>.
    /// </summary>
    /// <param name="sourceName">
    ///   <inheritdoc cref="SourceName" select='../value/node()' />
    /// </param>
    /// <param name="targetName">
    ///   <inheritdoc cref="TargetName" select='../value/node()' />
    /// </param>
    public LightBoundProperty(String sourceName, String targetName) {
      if (sourceName == null) throw new ArgumentNullException();
      if (sourceName.Length == 0) throw new ArgumentOutOfRangeException();
      if (targetName == null) throw new ArgumentNullException();
      if (targetName.Length == 0) throw new ArgumentOutOfRangeException();

      this.sourceName = sourceName;
      this.targetName = targetName;
    }

    /// <inheritdoc />
    /// <param name="name">
    ///   The name of the property to be bound.
    /// </param>
    public LightBoundProperty(String name) {
      if (name == null) throw new ArgumentNullException();
      if (name.Length == 0) throw new ArgumentOutOfRangeException();

      this.sourceName = name;
      this.targetName = name;
    }
    #endregion
  }
}