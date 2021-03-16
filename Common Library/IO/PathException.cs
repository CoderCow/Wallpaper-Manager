using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Common {
  /// <summary>
  ///   Thrown when an invalid operation is performed by the <see cref="Path" /> structure.
  /// </summary>
  [Serializable]
  public class PathException: Exception {
    #region Property: Path
    /// <summary>
    ///   <inheritdoc cref="Path" select='../value/node()' />
    /// </summary>
    private readonly String path;

    /// <summary>
    ///   Gets the path string where this exception is related to.
    /// </summary>
    /// <value>
    ///   The path string where this exception is related to.
    /// </value>
    public String Path {
      get { return this.path; }
    }
    #endregion


    #region Methods: Contructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="PathException" /> class with the given <paramref name="path" />.
    /// </summary>
    /// <param name="path">
    ///   The path string where this exception is related to.
    /// </param>
    /// <inheritdoc />
    /// 
    /// <overloads>
	  ///   <summary>
    ///     Initializes a new instance of the <see cref="PathException" /> class with the given <paramref name="path" />.
    ///   </summary>
	  /// </overloads>
    public PathException(String path, String message, Exception inner = null): base(message, inner) {
      this.path = path;
    }

    /// <inheritdoc />
    public PathException(String message, Exception inner = null): base(message, inner) {}
    #endregion

    #region ISerializable Implementation
    /// <inheritdoc />
    protected PathException(SerializationInfo info, StreamingContext context): base(info, context) {
      this.path = info.GetString("PathException_Path");
    }
    
    /// <inheritdoc />
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      
      info.AddValue("PathException_Path", this.path);
    }
    #endregion
  }
}