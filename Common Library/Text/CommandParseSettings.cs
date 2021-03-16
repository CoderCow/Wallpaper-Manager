using System;

namespace Common.Text {
  /// <threadsafety static="false" instance="false" />
  public struct CommandParseSettings {
    #region Constants and Fields
    /// <summary>
    ///   <inheritdoc cref="CommandPrefix" select='../value/node()' />
    /// </summary>
    private readonly Char? commandPrefix;

    /// <summary>
    ///   <inheritdoc cref="CommandSuffix" select='../value/node()' />
    /// </summary>
    private readonly Char? commandSuffix;

    /// <summary>
    ///   <inheritdoc cref="ParameterParseSettings" select='../value/node()' />
    /// </summary>
    private readonly StringlistParseSettings parameterParseSettings;
    #endregion

    #region Events and Properties
    /// <summary>
    ///   Gets a C# command style setting schema.
    /// </summary>
    /// <value>
    ///   A C# command style setting schema.
    /// </value>
    public static CommandParseSettings CSharpStyle {
      get {
        return new CommandParseSettings(
          null, ';', StringlistParseSettings.CSharpStyle
        );
      }
    }

    /// <summary>
    ///   Gets a DOS command style setting schema.
    /// </summary>
    /// <value>
    ///   A DOS command style setting schema.
    /// </value>
    public static CommandParseSettings DOSStyle {
      get {
        return new CommandParseSettings(
          null, null, StringlistParseSettings.DOSStyle
        );
      }
    }

    /// <summary>
    ///   Gets a Visual Basic 6 command style setting schema.
    /// </summary>
    /// <value>
    ///   A Visual Basic 6 command style setting schema.
    /// </value>
    public static CommandParseSettings VisualBasicStyle {
      get {
        return new CommandParseSettings(
          null, null, StringlistParseSettings.VisualBasicStyle
        );
      }
    }

    /// <summary>
    ///   Gets the char prefixing the command name.
    /// </summary>
    /// <value>
    ///   The char prefixing the command name.
    /// </value>
    public Char? CommandPrefix {
      get { return this.commandPrefix; }
    }

    /// <summary>
    ///   Gets the char suffixing the command line.
    /// </summary>
    /// <value>
    ///   The char suffixing the command line.
    /// </value>
    public Char? CommandSuffix {
      get { return this.commandSuffix; }
    }

    /// <summary>
    ///   Gets the settings for parameter parsing.
    /// </summary>
    /// <value>
    ///   The settings for parameter parsing.
    /// </value>
    public StringlistParseSettings ParameterParseSettings {
      get { return this.parameterParseSettings; }
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Initializes a new instance of the <see cref="CommandParseSettings" /> struct.
    /// </summary>
    /// <param name="commandPrefix">
    ///   <inheritdoc cref="CommandPrefix" select='../value/node()' />
    /// </param>
    /// <param name="commandSuffix">
    ///   <inheritdoc cref="CommandSuffix" select='../value/node()' />
    /// </param>
    /// <param name="parameterParseSettings">
    ///   <inheritdoc cref="ParameterParseSettings" select='../value/node()' />
    /// </param>
    public CommandParseSettings(
      Char? commandPrefix, Char? commandSuffix, StringlistParseSettings parameterParseSettings
    ) {
      this.commandPrefix = commandPrefix;
      this.commandSuffix = commandSuffix;
      this.parameterParseSettings = parameterParseSettings;
    }
    #endregion
  }
}
