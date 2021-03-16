using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text;

namespace Common {
  /// <summary>
  ///   Represents a collection of application arguments.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class AppArgumentCollection: ReadOnlyCollection<String> {
    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="AppArgumentCollection" /> class by enumerating for arguments through 
    ///   a given object.
    /// </summary>
    /// <param name="arguments">
    ///   The arguments which should be hold by this collection.
    /// </param>
    public AppArgumentCollection(IEnumerable<String> arguments = null): base(new List<String>(arguments)) {
      if (arguments == null) throw new ArgumentNullException();

      Contract.Assert(!this.Items.Contains(null));
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="AppArgumentCollection" /> class by taking the arguments passed to
    ///   the current process.
    /// </summary>
    public AppArgumentCollection(): base(new List<String>()) {
      ((List<String>)this.Items).AddRange(Environment.GetCommandLineArgs());

      Contract.Assert(!this.Items.Contains(null));
    }
    #endregion

    #region Method: ContainsArgument
    /// <summary>
    ///   Checks whether a given argument exists.
    /// </summary>
    /// <param name="argument">
    ///   The argument to check for.
    /// </param>
    /// <param name="ignoreCase">
    ///   Indicates whether the case should be ignored when checking for the argument.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the argument exists or not.
    /// </returns>
    public Boolean ContainsArgument(String argument, Boolean ignoreCase = true) {
      if (argument == null) throw new ArgumentNullException();

      String ucWord = argument.ToUpperInvariant();

      for (Int32 i = 0; i < this.Items.Count; i++) {
        Contract.Assert(this.Items[i] != null);

        if (this.Items[i].ToUpperInvariant() == ucWord) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    ///   Checks whether a given argument exists and returns the following argument which is expected to be a parameter of
    ///   the argument being searched for.
    /// </summary>
    /// <param name="argument">
    ///   The argument to check for.
    /// </param>
    /// <param name="parameter">
    ///   The following argument expected as an parameter of the found argument. <c>null</c> if the found argument is the
    ///   last argument or if the argument was not found at all.
    /// </param>
    /// <param name="ignoreCase">
    ///   Indicates whether the case should be ignored when checking for the argument.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the argument exists or not.
    /// </returns>
    public Boolean ContainsArgument(String argument, out String parameter, Boolean ignoreCase = true) {
      String ucWord = argument.ToUpperInvariant();

      for (Int32 i = 0; i < this.Items.Count; i++) {
        Contract.Assert(this.Items[i] != null);

        if (this.Items[i].ToUpperInvariant() == ucWord) {
          if (i + 1 != this.Items.Count) {
            parameter = this.Items[i + 1];
          } else {
            parameter = null;
          }

          return true;
        }
      }

      parameter = null;
      return false;
    }
    #endregion

    #region Methods: ToString
    public override String ToString() {
      if (this.Items.Count == 0) {
        return "-no arguments-";
      }

      StringBuilder builder = new StringBuilder(this.Items.Count * 10);

      for (Int32 i = 0; i < this.Items.Count; i++) {
        if (i > 0) {
          builder.Append(' ');
        }

        String argument = this.Items[i];
        Contract.Assert(argument != null);

        Boolean needsQuoting = argument.IndexOfAny(new[] { ' ', '\t' }) > 0;
        if (needsQuoting) {
          builder.Append('"');
        }

        builder.Append(argument);

        if (needsQuoting) {
          builder.Append('"');
        }
      }

      return builder.ToString();
    }
    #endregion
  }
}