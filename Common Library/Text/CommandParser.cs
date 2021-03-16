using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Text {
  /// <threadsafety static="false" instance="false" />
  public class CommandParser {
    #region Property: ParseSettings
    /// <summary>
    ///   <inheritdoc cref="ParseSettings" select='../value/node()' />
    /// </summary>
    private readonly CommandParseSettings parseSettings;

    /// <summary>
    ///   Gets the settings for command parsing.
    /// </summary>
    /// <value>
    ///   The settings for command parsing.
    /// </value>
    public CommandParseSettings ParseSettings {
      get { return this.parseSettings; }
    }
    #endregion

    #region Property: ParameterParser
    /// <summary>
    ///   <inheritdoc cref="ParameterParser" select='../value/node()' />
    /// </summary>
    private readonly StringlistParser parameterParser;

    /// <summary>
    ///   Gets the parser used to parse the command's parameter set.
    /// </summary>
    /// <value>
    ///   The parser used to parse the command's parameter set.
    /// </value>
    protected StringlistParser ParameterParser {
      get { return this.parameterParser; }
    }
    #endregion


    #region Methods: Constructor
    public CommandParser(CommandParseSettings parseSettings) {
      this.parseSettings = parseSettings;
      this.parameterParser = new StringlistParser(parseSettings.ParameterParseSettings);
    }
    #endregion

    #region Methods: Parse, ThrowParseException
    public virtual void Parse(
      String commandString, 
      out String commandName, 
      out List<String> parameters, 
      out Dictionary<String, String> namedParameters
    ) {
      StringBuilder commandNameBuilder = new StringBuilder();

      // True if the loop is already inside of the command name; otherwise false.
      Boolean inCommandName = false;
      Int32 i;

      // At first, we have to parse the command name until the parameter set starts.
      for (i = 0; i < commandString.Length; i++) {
        Char currentChar = commandString[i];
        
        // Make sure we aren't already inside the command name
        if ((currentChar != ' ') && (!inCommandName)) {
          inCommandName = true;

          // This is the first char of the command name, check for the prefix.
          if (this.ParseSettings.CommandPrefix != null) {
            if (currentChar == this.ParseSettings.CommandPrefix) {
              // Don't include the command prefix in the name.
              continue;
            } else {
              this.ThrowParseException("Command prefix missing.");
            }
          }
        }

        if (inCommandName) {
          if (currentChar == ' ') {
            if (this.ParseSettings.ParameterParseSettings.ExplicitPrefixSuffix) {
              this.ThrowParseException(String.Format(
                "Whitespaces are not allowed in command names. Forgot {0}{1}?",
                this.ParseSettings.ParameterParseSettings.ListPrefix,
                this.ParseSettings.ParameterParseSettings.ListSuffix
              ));
            }

            // End of command name.
            break;
          }

          if (currentChar == this.ParseSettings.ParameterParseSettings.ListPrefix) {
            // End of command name.
            break;
          }

          commandNameBuilder.Append(currentChar);
        }
      }

      if (commandNameBuilder.Length == 0) {
        this.ThrowParseException("Command name missing.");
      }

      Int32 parameterSetStartIndex = i;
      Int32 parameterSetEndIndex = 0;

      if (this.ParseSettings.CommandSuffix != null) {
        // If a suffix is defined, we have to find the end of the command.
        for (i = commandString.Length - 1; i >= 0; i--) {
          if (commandString[i] == this.ParseSettings.CommandSuffix) {
            break;
          }
        }

        parameterSetEndIndex = i;

        if (parameterSetEndIndex < parameterSetStartIndex) {
          this.ThrowParseException("Command suffix missing.");
        }
      } else {
        parameterSetEndIndex = commandString.Length;
      }

      commandName = commandNameBuilder.ToString();

      try {
        this.ParameterParser.Parse(
          commandString.Substring(parameterSetStartIndex, parameterSetEndIndex - parameterSetStartIndex),
          out parameters, out namedParameters
        );
      } catch (ArgumentException exception) {
        throw new ArgumentException("Error parsing parameters: " + exception.Message);
      }
    }

    private void ThrowParseException(String reason) {
      throw new ArgumentException(String.Format("Error while parsing command string: {0}", reason));
    }
    #endregion
  }
}
