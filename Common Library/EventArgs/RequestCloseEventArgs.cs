using System;

namespace Common {
  /// <summary>
  ///   Provides Request Close event related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class RequestCloseEventArgs: EventArgs {
    #region Property: Result
    /// <summary>
    ///   <inheritdoc cref="Result" select='../value/node()' />
    /// </summary>
    private readonly Boolean? result;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating the reason why closing is requested.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating the reason why closing is requested. <c>null</c> if no reason is given at all.
    ///   Default value: <c>null</c>.
    /// </value>
    public Boolean? Result {
      get { return this.result; }
    }
    #endregion


    #region Method: Constructor, ToString
    /// <summary>
    ///   Initializes a new instance of the <see cref="RequestCloseEventArgs">RequestCloseEventArgs Class</see>.
    /// </summary>
    /// <param name="result">
    ///   <inheritdoc cref="Result" select='../value/node()' />
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="$ArgumentNameLC" /> is <c>null</c>.
    /// </exception>
    public RequestCloseEventArgs(Boolean? result = null) {
      this.result = result;
    }

    /// <inheritdoc />
    public override String ToString() {
      if (this.Result != null) {
        return String.Format("Result: {0}", this.Result);
      }

      return "Result: null";
    }
    #endregion
  }
}