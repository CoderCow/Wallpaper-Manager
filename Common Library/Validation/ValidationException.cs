using System;
using System.Runtime.Serialization;

namespace Common.Validation {
  /// <summary>
  ///   Thrown when a validation check fails.
  /// </summary>
  public class ValidationException: Exception {
    #region Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException" /> class 
    ///   using the default message.
    /// </summary>
    /// <remarks>
    ///   The message property for this exception will be set to the default constantly 
    ///   unlocalized defined message.
    /// </remarks>
    public ValidationException(): base("Validation failed.") {}

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException" /> class.
    /// </summary>
    /// <param name="message">
    ///   The exception message <see cref="String" /> describing the exception behavior to the user.
    /// </param>
    /// <param name="inner">
    ///   The inner exception reference which caused the throw of this exception.
    /// </param>
    public ValidationException(String message, Exception inner): base(message, inner) {}

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException" /> class.
    /// </summary>
    /// <param name="message">
    ///   The exception message <see cref="String" /> describing the exception behavior to the user.
    /// </param>
    public ValidationException(String message): base(message, null) {}

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException" /> class.
    /// </summary>
    /// <param name="info">
    ///   The data for serializing or deserializing the exception.
    /// </param>
    /// <param name="context">
    ///   The source and destination for the exception.
    /// </param>
    protected ValidationException(SerializationInfo info, StreamingContext context): base(info, context) {}
    #endregion
  }
}
