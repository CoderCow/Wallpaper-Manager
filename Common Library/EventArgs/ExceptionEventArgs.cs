using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Common.Validation;

namespace Common {
  /// <summary>
  ///   Provides exception event related data.
  /// </summary>
  public class ExceptionEventArgs: EventArgs {
    #region Property: Exception
    /// <inheritdoc cref="Exception" select='../value/node()' />
    private readonly Exception exception;

    /// <summary>
    ///   Gets the exception object.
    /// </summary>
    /// <value>
    ///   The exception object.
    /// </value>
    public Exception Exception {
      get { return this.exception; }
    }
    #endregion

    #region Property: IsHandled
    /// <summary>
    ///   <inheritdoc cref="IsHandled" select='../value/node()' />
    /// </summary>
    private Boolean isHandled;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the exception has been handled or not.
    /// </summary>
    /// <value>
    ///   Indicates whether the exception has been handled or not.
    /// </value>
    public Boolean IsHandled {
      get { return this.isHandled; }
      set { this.isHandled = value; }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ExceptionEventArgs">ExceptionEventArgs Class</see>.
    /// </summary>
    /// <param name="exception">
    ///   <inheritdoc cref="Exception" select='../value/node()' />
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="exception" /> is <c>null</c>.
    /// </exception>
    public ExceptionEventArgs(Exception exception) {
      if (exception == null)
        throw new ArgumentNullException("exception");

      this.exception = exception;
    }
    #endregion
  }
}
