using System;
using System.Windows.Input;

namespace Common.Presentation {
  /// <summary>
  ///   Provides command exception event related data.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  /// <seealso cref="ICommand">ICommand Interface</seealso>
  public class CommandExceptionEventArgs: ExceptionEventArgs {
    #region Property: Command
    /// <summary>
    ///   <inheritdoc cref="Command" select='../value/node()' />
    /// </summary>
    private readonly ICommand command;

    /// <summary>
    ///   Gets the <see cref="ICommand" /> which rised an exception.
    /// </summary>
    /// <value>
    ///   The <see cref="ICommand" /> which rised an exception.
    /// </value>
    /// <seealso cref="ICommand">ICommand Interface</seealso>
    public ICommand Command {
      get { return this.command; }
    }
    #endregion

    #region Method: Constructor, ToString
    /// <summary>
    ///   Initializes a new instance of the <see cref="CommandExceptionEventArgs">CommandExceptionEventArgs Class</see>.
    /// </summary>
    /// <param name="command">
    ///   <inheritdoc cref="Command" select='../value/node()' />
    /// </param>
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="command" /> or <paramref name="exception" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="ICommand">ICommand Interface</seealso>
    public CommandExceptionEventArgs(ICommand command, Exception exception): base(exception) {
      if (command == null)
        throw new ArgumentNullException("command");

      this.command = command;
    }

    /// <inheritdoc />
    public override String ToString() {
      return String.Format("Command: {0}, Exception: {1}", this.Command, this.Exception.Message);
    }
    #endregion
  }
}