using System;

namespace Common.Text {
  /// <threadsafety static="false" instance="false" />
  public struct StringlistParseSettings {
    #region Constants and Fields
    /// <summary>
    ///   <inheritdoc cref="StringSeparator" select='../value/node()' />
    /// </summary>
    private Char stringSeparator;

    /// <summary>
    ///   <inheritdoc cref="ListPrefix" select='../value/node()' />
    /// </summary>
    private Char listPrefix;

    /// <summary>
    ///   <inheritdoc cref="ListSuffix" select='../value/node()' />
    /// </summary>
    private Char listSuffix;

    /// <summary>
    ///   <inheritdoc cref="MaskingChar" select='../value/node()' />
    /// </summary>
    private Char maskingChar;

    /// <summary>
    ///   <inheritdoc cref="AltMaskingChar" select='../value/node()' />
    /// </summary>
    private Char altValueMaskingChar;

    /// <summary>
    ///   <inheritdoc cref="ExplicitPrefixSuffix" select='../value/node()' />
    /// </summary>
    private Boolean explicitPrefixSuffix;

    /// <summary>
    ///   <inheritdoc cref="NamedStringPrefix" select='../value/node()' />
    /// </summary>
    private Char? namedStringPrefix;

    /// <summary>
    ///   <inheritdoc cref="NamedStringValueSeparator" select='../value/node()' />
    /// </summary>
    private Char namedParameterValueSeparator;
    #endregion

    #region Events and Properties
    /// <summary>
    ///   Gets a C# command style setting schema.
    /// </summary>
    /// <value>
    ///   A C# command style setting schema.
    /// </value>
    public static StringlistParseSettings CSharpStyle {
      get {
        return new StringlistParseSettings(
          ',', '(', ')', '"', '\'', null, '=', true
        );
      }
    }

    /// <summary>
    ///   Gets a DOS command style setting schema.
    /// </summary>
    /// <value>
    ///   A DOS command style setting schema.
    /// </value>
    public static StringlistParseSettings DOSStyle {
      get {
        return new StringlistParseSettings(
          ' ', '(', ')', '"', '\'', '-', ' ', false
        );
      }
    }

    /// <summary>
    ///   Gets a Visual Basic 6 command style setting schema.
    /// </summary>
    /// <value>
    ///   A Visual Basic 6 command style setting schema.
    /// </value>
    public static StringlistParseSettings VisualBasicStyle {
      get {
        return new StringlistParseSettings(
          ',', '(', ')', '"', '\'', null, '=', false
        );
      }
    }

    /// <summary>
    ///   Gets or sets the char separating each parameter from the others.
    /// </summary>
    /// <value>
    ///   The char separating each parameter from the others.
    /// </value>
    public Char StringSeparator {
      get { return this.stringSeparator; }
      set { this.stringSeparator = value; }
    }

    /// <summary>
    ///   Gets or sets the character prefixing the begin of the parameter set (after the command name).
    /// </summary>
    /// <value>
    ///   The character prefixing the begin of the parameter set (after the command name).
    /// </value>
    public Char ListPrefix {
      get { return this.listPrefix; }
      set { this.listPrefix = value; }
    }

    /// <summary>
    ///   Gets or sets the character suffixing the end of the parameter set (behind the last parameter).
    /// </summary>
    /// <value>
    ///   The character suffixing the end of the parameter set (behind the last parameter).
    /// </value>
    public Char ListSuffix {
      get { return this.listSuffix; }
      set { this.listSuffix = value; }
    }

    /// <summary>
    ///   Gets or sets the character used to mask a value (to exclude a value from being parsed).
    /// </summary>
    /// <value>
    ///   The character used to mask a value (to exclude a value from being parsed).
    /// </value>
    public Char MaskingChar {
      get { return this.maskingChar; }
      set { this.maskingChar = value; }
    }

    /// <summary>
    ///   Gets or sets the alternative character used to mask a value (to exclude a value from being parsed).
    /// </summary>
    /// <value>
    ///   The alternative character used to mask a value (to exclude a value from being parsed).
    /// </value>
    public Char AltMaskingChar {
      get { return this.altValueMaskingChar; }
      set { this.altValueMaskingChar = value; }
    }

    /// <summary>
    ///   Gets or sets the character used to prefix a named parameter.
    /// </summary>
    /// <value>
    ///   The character used to prefix a named parameter; <c>null</c> if no named parameter prefix is used.
    /// </value>
    public Char? NamedStringPrefix {
      get { return this.namedStringPrefix; }
      set { this.namedStringPrefix = value; }
    }

    /// <summary>
    ///   Gets or sets the character used to separate the named parameter name from its value.
    /// </summary>
    /// <value>
    ///   The character used to separate the named parameter name from its value; <c>null</c> if no
    ///   named parameter-value separator is used.
    /// </value>
    public Char NamedStringValueSeparator {
      get { return this.namedParameterValueSeparator; }
      set { this.namedParameterValueSeparator = value; }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the parameter set has to be explicitly defined by 
    ///   prefixing it with <see cref="ListPrefix" /> and suffixing it with <see cref="ListSuffix" />.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the parameter set has to be explicitly defined by prefixing it with 
    ///   <see cref="ListPrefix" /> and suffixing it with <see cref="ListSuffix" />; 
    ///   otherwise <c>false</c>.
    /// </value>
    public Boolean ExplicitPrefixSuffix {
      get { return this.explicitPrefixSuffix; }
      set { this.explicitPrefixSuffix = value; }
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Initializes a new instance of the <see cref="StringlistParseSettings" /> struct.
    /// </summary>
    /// <param name="stringSeparator">
    ///   <inheritdoc cref="StringSeparator" select='../value/node()' />
    /// </param>
    /// <param name="listPrefix">
    ///   <inheritdoc cref="ListPrefix" select='../value/node()' />
    /// </param>
    /// <param name="listSuffix">
    ///   <inheritdoc cref="ListSuffix" select='../value/node()' />
    /// </param>
    /// <param name="maskingChar">
    ///   <inheritdoc cref="MaskingChar" select='../value/node()' />
    /// </param>
    /// <param name="altMaskingChar">
    ///   <inheritdoc cref="AltMaskingChar" select='../value/node()' />
    /// </param>
    /// <param name="namedStringPrefix">
    ///   <inheritdoc cref="NamedStringPrefix" select='../value/node()' />
    /// </param>
    /// <param name="namedStringValueSeparator">
    ///   <inheritdoc cref="NamedStringValueSeparator" select='../value/node()' />
    /// </param>
    /// <param name="explicitPrefixSuffix">
    ///   <inheritdoc cref="ExplicitPrefixSuffix" select='../value/node()' />
    /// </param>
    public StringlistParseSettings(
      Char stringSeparator, 
      Char listPrefix, 
      Char listSuffix, 
      Char maskingChar,
      Char altMaskingChar,
      Char? namedStringPrefix,
      Char namedStringValueSeparator,
      Boolean explicitPrefixSuffix
    ) {
      this.stringSeparator = stringSeparator;
      this.listPrefix = listPrefix;
      this.listSuffix = listSuffix;
      this.maskingChar = maskingChar;
      this.altValueMaskingChar = altMaskingChar;
      this.namedStringPrefix = namedStringPrefix;
      this.namedParameterValueSeparator = namedStringValueSeparator;
      this.explicitPrefixSuffix = explicitPrefixSuffix;
    }

    /// <inheritdoc />
    public StringlistParseSettings(Char stringSeparator, Char maskingChar, Char altMaskingChar)
      : this(stringSeparator, '(', ')', maskingChar, altMaskingChar, null, ' ', false) {}
    #endregion
  }
}
