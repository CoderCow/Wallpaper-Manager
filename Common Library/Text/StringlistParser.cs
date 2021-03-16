using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Text {
  /// <threadsafety static="false" instance="false" />
  public class StringlistParser {
    #region Constants and Fields
    private readonly StringlistParseSettings parseSettings;
    #endregion

    #region Events and Properties
    public StringlistParseSettings ParseSettings {
      get { return this.parseSettings; }
    }
    #endregion

    #region Methods
    public StringlistParser(StringlistParseSettings parseSettings) {
      this.parseSettings = parseSettings;

      this.CheckStringlistParseSettings(parseSettings);
    }

    private void CheckStringlistParseSettings(StringlistParseSettings settings) {
      if (
        (settings.StringSeparator == settings.ListPrefix) || 
        (settings.StringSeparator == settings.ListSuffix) || 
        (settings.StringSeparator == settings.MaskingChar) ||
        (settings.StringSeparator == settings.AltMaskingChar) || (
          (settings.NamedStringPrefix != null) &&
          (settings.AltMaskingChar == settings.NamedStringPrefix)
        )
      ) {
        throw new ArgumentException(
          "The string-separator can not be equal with list-prefix, list-suffix, masking-char, alternative masking-char or named-string-prefix."
        );
      }

      if (
        (settings.MaskingChar == settings.ListPrefix) || 
        (settings.MaskingChar == settings.ListSuffix) || 
        (settings.MaskingChar == settings.NamedStringValueSeparator) || (
          (settings.NamedStringPrefix != null) &&
          (settings.MaskingChar == settings.NamedStringPrefix)
        )
      ) {
        throw new ArgumentException(
          "The masking-char can not be equal with list-prefix, list-suffix, named-string-value-separator or named-string-prefix."
        );
      }

      if (
        (settings.AltMaskingChar == settings.ListPrefix) || 
        (settings.AltMaskingChar == settings.ListSuffix) || 
        (settings.AltMaskingChar == settings.NamedStringValueSeparator) || (
          (settings.NamedStringPrefix != null) &&
          (settings.AltMaskingChar == settings.NamedStringPrefix)
        )
      ) {
        throw new ArgumentException(
          "The alternative masking-char can not be equal with list-prefix, list-suffix, named-string-value-separator or named-string-prefix."
        );
      }

      if (
        (settings.NamedStringPrefix != null) && (
          (settings.NamedStringPrefix == settings.ListPrefix) || 
          (settings.NamedStringPrefix == settings.ListSuffix) || 
          (settings.NamedStringPrefix == settings.NamedStringValueSeparator)
        )
      ) {
        throw new ArgumentException(
          "The named-string-prefix can not be equal with list-prefix, list-suffix or named-string-value-separator."
        );
      }

      if (
        (settings.AltMaskingChar == settings.ListPrefix) || 
        (settings.AltMaskingChar == settings.ListSuffix)
      ) {
        throw new ArgumentException(
          "The named-string-value-separator can not be equal with list-prefix or list-suffix."
        );
      }

      if (settings.ListPrefix == ' ') {
        throw new ArgumentException("List-prefix can not be a whitespace.");
      }

      if (settings.ListSuffix == ' ') {
        throw new ArgumentException("List-suffix can not be a whitespace.");
      }

      if (settings.NamedStringPrefix == ' ') {
        throw new ArgumentException("Named-string-prefix can not be a whitespace.");
      }

      if ((settings.MaskingChar == ' ') || (settings.AltMaskingChar == ' ')) {
        throw new ArgumentException("Masking-char or alternative masking-char can not be a whitespace.");
      }

      if ((settings.NamedStringPrefix == null) && (settings.NamedStringValueSeparator == ' ')) {
        throw new ArgumentException(
          "The prefix for named-strings can not be null if named-string value separator is a whitespace."
        );
      }
    }

    public virtual void Parse(
      String stringList, out List<String> strings, out Dictionary<String, String> namedStrings
    ) {
      // Parse position definitions.
      //      (MyParameter1, MyParameter2, MyNamedParameter=MyValue)
      // ^ to ^
      const Int32 PP_Prefix = 1;

      // ( MyParameter1 , MyParameter2,MyParameter3,MyNamedParameter = MyValue)
      //  ^                    to                  ^
      const Int32 PP_String = 2;

      // (MyParameter1,MyParameter2,MyParameter3,MyNamedParameter=MyValue)
      //                                         ^               ^
      const Int32 PP_NamedString = 3;

      // (MyParameter1,MyParameter2,MyParameter3,MyNamedParameter=MyValue)
      //                                                          ^      ^
      const Int32 PP_NamedStringValue = 4;
      // (MyParameter1,MyParameter2,MyParameter3,MyNamedParameter=MyValue)           
      //                                                                  ^   to    ^
      const Int32 PP_End = 5;

      Int32 parsePosition = PP_Prefix;
      Char? maskingWith = null;
      Char? lastStringMaskChar = null;
      Boolean lastStringWasMasked = false;
      String lastNamedStringName = null;

      // The first part will be the command's name.
      StringBuilder currentString = new StringBuilder();

      strings = new List<String>();
      namedStrings = new Dictionary<String, String>();

      for (Int32 i = 0; ((i < stringList.Length) && (parsePosition != PP_End)); i++) {
        Char currentChar = stringList[i];
        Boolean isLastChar = (i == stringList.Length - 1);

        switch (parsePosition) {
          case PP_Prefix:
            // e.g.     #<parameters>)
            //          <parameters>
            //      (<parameters>)
            //      <parameters>
            if (currentChar != ' ') {
              if (currentChar != this.ParseSettings.ListPrefix) {
                if (!this.ParseSettings.ExplicitPrefixSuffix) {
                  // Let PP_String reparse this character.
                  i--;
                } else {
                  this.ThrowParseException(
                    String.Format("List-prefix {0} not found.", this.ParseSettings.ListPrefix)
                  );
                }
              }
              
              parsePosition = PP_String;
              continue;
            }

            break;
          case PP_String:
          case PP_NamedString:
          case PP_NamedStringValue:
            // e.g. MyParameter1,MyParameter2)
            //         MyParameter  )  
            //      "MyPar"a"meter"
            //      MyParameter)
            //      MyParameter,"",,  ,   MyParameter3 ,A= , Muh    
            //      )
            if (maskingWith != null) {
              if (currentChar != maskingWith) {
                if (isLastChar) {
                  this.ThrowParseException(String.Format("Value mask {0} is never closed.", maskingWith));
                }

                currentString.Append(currentChar);
                continue;
              } else {
                lastStringMaskChar = maskingWith;
                maskingWith = null;
                lastStringWasMasked = true;

                if (!isLastChar) {
                  continue;
                }
              }
            }

            if (
              (isLastChar) &&
              (this.ParseSettings.ExplicitPrefixSuffix) &&
              (currentChar != this.ParseSettings.ListSuffix)
            ) {
              this.ThrowParseException("List-suffix not found.");
            }

            if (
              (this.ParseSettings.NamedStringPrefix != null) && 
              (currentChar == this.ParseSettings.NamedStringPrefix)
            ) {
              for (Int32 n = 0; n < currentString.Length; n++) {
                if (currentString[n] != ' ') {
                  this.ThrowParseException(String.Format(
                    "Only whitespaces are allowed before {0}", this.ParseSettings.NamedStringPrefix
                  ));
                }
              }

              parsePosition = PP_NamedString;
              currentString = new StringBuilder();
              continue;
            }

            if (
              (isLastChar) ||
              (currentChar == this.ParseSettings.ListSuffix) ||
              (currentChar == this.ParseSettings.StringSeparator) || (
                (currentChar == this.ParseSettings.NamedStringValueSeparator) &&
                (parsePosition != PP_NamedStringValue) && (
                  (parsePosition == PP_NamedString) || (
                    (parsePosition == PP_String) &&
                    (this.ParseSettings.NamedStringValueSeparator != ' ')
                  )
                )
              )
            ) {
              if (
                (isLastChar) &&
                (currentChar != this.ParseSettings.ListSuffix) &&
                (currentChar != this.ParseSettings.StringSeparator) &&
                (currentChar != this.ParseSettings.NamedStringValueSeparator) &&
                (currentChar != lastStringMaskChar)
              ) {
                currentString.Append(currentChar);
              }

              if (!lastStringWasMasked) {
                if ((currentChar == this.ParseSettings.ListSuffix) && (strings.Count == 0)) {
                  // This muste be a stringlist prefix, suffix string, so ignore this possible parameter.
                  continue;
                }

                if (currentString.Length == 0) {
                  if (this.ParseSettings.StringSeparator != ' ') {
                    this.ThrowParseException("A string value can not be empty.");
                  } else {
                    // If the string separator is a whitespace, then simply ignore this possible parameter.

                    if (currentChar == this.ParseSettings.ListSuffix) {
                      parsePosition = PP_End;
                    }

                    continue;
                  }
                }

                Boolean onlyWhitespaces = true;
                for (Int32 n = 0; n < currentString.Length; n++) {
                  if (currentString[n] != ' ') {
                    onlyWhitespaces = false;
                  }
                }

                if (onlyWhitespaces) {
                  this.ThrowParseException("A string value can not contain whitespaces only.");
                }
              }

              String finalizedString = currentString.ToString();
              currentString = new StringBuilder();

              if (!lastStringWasMasked) {
                finalizedString = finalizedString.Trim();
              }

              if (parsePosition == PP_String) {
                if (
                  (currentChar != this.ParseSettings.NamedStringValueSeparator) ||
                  (this.ParseSettings.NamedStringValueSeparator == this.ParseSettings.StringSeparator)
                ) {
                  strings.Add(finalizedString);
                } else {
                  if (this.ParseSettings.NamedStringPrefix == null) {
                    parsePosition = PP_NamedString;
                  } else {
                    this.ThrowParseException(String.Format(
                      "Invalid {0} after string, if the string should be a named string put a {1} before it.", 
                      this.ParseSettings.NamedStringValueSeparator, this.ParseSettings.NamedStringPrefix
                    ));
                  }
                }
              }

              if (parsePosition == PP_NamedStringValue) {
                namedStrings.Add(lastNamedStringName, finalizedString);
                parsePosition = PP_String;
              }
              
              if (parsePosition == PP_NamedString) {
                if (currentChar == this.ParseSettings.NamedStringValueSeparator) {
                  if (!isLastChar) {
                    lastNamedStringName = finalizedString;

                    parsePosition = PP_NamedStringValue;
                  } else {
                    this.ThrowParseException(
                      String.Format("No value for named string \"{0}\" defined.", finalizedString)
                    );
                  }
                } else {
                  // This must be an empty named string.
                  namedStrings.Add(finalizedString, String.Empty);

                  parsePosition = PP_String;
                }
              }

              if (currentChar == this.ParseSettings.ListSuffix) {
                parsePosition = PP_End;
              }

              lastStringWasMasked = false;
              lastStringMaskChar = null;
              continue;
            }

            if (
              (currentChar == this.ParseSettings.MaskingChar) || 
              (currentChar == this.ParseSettings.AltMaskingChar)
            ) {
              maskingWith = currentChar;
              currentString = new StringBuilder();
              continue;
            }

            if (!lastStringWasMasked) {
              currentString.Append(currentChar);
            }

            break;
        }
      }
    }

    private void ThrowParseException(String reason) {
      throw new ArgumentException(String.Format("Error while parsing command string: {0}", reason));
    }
    #endregion
  }
}
