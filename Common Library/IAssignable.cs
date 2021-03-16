using System;

namespace Common {
  /// <summary>
  ///   Supports assigning, which sets all members of one instance equal with the members of another instance.
  /// </summary>
  public interface IAssignable {
    #region Methods: AssignTo
    /// <summary>
    ///   Assigns all members of the current instance to the given instance.
    /// </summary>
    /// <remarks>
    ///   This method will <c>not</c> clone the members but simply assign them by using the = operator.
    /// </remarks>
    /// <param name="other">
    ///   The other instance to assign the members to.
    /// </param>
    void AssignTo(Object other);
    #endregion
  }
}
