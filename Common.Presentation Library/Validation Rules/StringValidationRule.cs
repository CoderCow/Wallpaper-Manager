using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Common.Presentation {
  /// <summary>
  ///   A rule which checks a <see cref="String" /> object in the definable ways.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  public class StringValidationRule: ValidationRule {
    #region Static Property: CommonCharSet
    /// <summary>
    ///   <inheritdoc cref="CommonCharSet" select='../value/node()' />
    /// </summary>
    private static readonly ReadOnlyCollection<Char> commonCharSet;

    /// <summary>
    ///   Gets a collection of common characters (ASCII index 32 to 126, Ä, ä, Ö, ö, Ü, ü, ß, ', §, °).
    /// </summary>
    /// <value>
    ///   A collection of common characters (ASCII index 32 to 126, Ä, ä, Ö, ö, Ü, ü, ß, ', §, °).
    /// </value>
    public static ReadOnlyCollection<Char> CommonCharSet {
      get { return StringValidationRule.commonCharSet; }
    }
    #endregion

    #region Property: AllowsNull
    /// <summary>
    ///   <inheritdoc cref="AllowNull" select='../value/node()' />
    /// </summary>
    private Boolean allowNull;

    /// <summary>
    ///   Gets or sets a value indicating whether <c>null</c> is allowed or not.
    ///   Default value: <c>false</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether <c>null</c> is allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowNull {
      get { return this.allowNull; }
      set { this.allowNull = value; }
    }
    #endregion

    #region Property: AllowEmpty
    /// <summary>
    ///   Gets or sets a value indicating whether an <see cref="String.Empty">empty string</see> is allowed or not.
    ///   Default value: <c>true</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether an <see cref="String.Empty">empty string</see> is allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowEmpty {
      get { return (this.AllowedMinLength == 0); }
      set {
        if (value) {
          this.AllowedMinLength = 0;
        } else {
          this.AllowedMinLength = 1;
        }
      }
    }
    #endregion

    #region Property: AllowWhitespacesOnly
    /// <summary>
    ///   <inheritdoc cref="AllowWhitespacesOnly" select='../value/node()' />
    /// </summary>
    private Boolean allowWhitespacesOnly;

    /// <summary>
    ///   Gets or sets a value indicating whether only whitespaces are allowed in a string or not.
    ///   Default value: <c>true</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether only whitespaces are allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowWhitespacesOnly {
      get { return this.allowWhitespacesOnly; }
      set { this.allowWhitespacesOnly = value; }
    }
    #endregion

    #region Property: AllowWhitespacesAtStart
    /// <summary>
    ///   <inheritdoc cref="AllowWhitespacesAtStart" select='../value/node()' />
    /// </summary>
    private Boolean allowWhitespacesAtStart;

    /// <summary>
    ///   Gets or sets a value indicating whether whitespaces at the start of a string are allowed or not.
    ///   Default value: <c>true</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether whitespaces at the start of a string are allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowWhitespacesAtStart {
      get { return this.allowWhitespacesAtStart; }
      set { this.allowWhitespacesAtStart = value; }
    }
    #endregion

    #region Property: AllowWhitespacesAtEnd
    /// <summary>
    ///   <inheritdoc cref="AllowWhitespacesAtEnd" select='../value/node()' />
    /// </summary>
    private Boolean allowWhitespacesAtEnd;

    /// <summary>
    ///   Gets or sets a value indicating whether whitespaces at the end of a string are allowed or not.
    ///   Default value: <c>true</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether whitespaces at the end of a string are allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowWhitespacesAtEnd {
      get { return this.allowWhitespacesAtEnd; }
      set { this.allowWhitespacesAtEnd = value; }
    }
    #endregion

    #region Property: AllowedMinLength
    /// <summary>
    ///   <inheritdoc cref="AllowedMinLength" select='../value/node()' />
    /// </summary>
    private Int32 allowedMinLength;

    /// <summary>
    ///   Gets or sets the allowed minimum length of a string.
    ///   Default value: <c>0</c>.
    /// </summary>
    /// <value>
    ///   The allowed minimum length of a string.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is below <c>0</c> or greater than <see cref="AllowedMaxLength" />.
    /// </exception>
    public Int32 AllowedMinLength {
      get { return this.allowedMinLength; }
      set {
        if (value < 0)
          throw new ArgumentOutOfRangeException();
        if ((this.AllowedMaxLength != 0) && (value > this.AllowedMaxLength))
          throw new ArgumentOutOfRangeException();

        this.allowedMinLength = value;
      }
    }
    #endregion

    #region Property: AllowedMaxLength
    /// <summary>
    ///   <inheritdoc cref="AllowedMaxLength" select='../value/node()' />
    /// </summary>
    private Int32 allowedMaxLength;

    /// <summary>
    ///   Gets or sets the allowed maximum length of a string.
    ///   Default value: <c>0</c> (unlimited).
    /// </summary>
    /// <remarks>
    ///   Set to <c>0</c> to set no limit for the <see cref="String" />'s length.
    /// </remarks>
    /// <value>
    ///   The allowed maximum length of a string.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is lower than <see cref="AllowedMinLength" />.
    /// </exception>
    public Int32 AllowedMaxLength {
      get { return this.allowedMaxLength; }
      set {
        if (value < this.AllowedMinLength)
          throw new ArgumentOutOfRangeException();

        this.allowedMaxLength = value;
      }
    }
    #endregion

    #region Property: AllowedCharacters
    /// <summary>
    ///   <inheritdoc cref="AllowedCharacters" select='../value/node()' />
    /// </summary>
    private readonly Collection<Char> allowedCharacters;

    /// <summary>
    ///   Gets a collection of characters allowed in the string.
    ///   Default value: <see cref="StringValidationRule.Char32To126CharSet" />.
    /// </summary>
    /// <value>
    ///   A collection of characters allowed in the string.
    /// </value>
    public Collection<Char> AllowedCharacters {
      get { return this.allowedCharacters; }
    }
    #endregion

    #region Property: DisallowedCharacters
    /// <summary>
    ///   <inheritdoc cref="DisallowedCharacters" select='../value/node()' />
    /// </summary>
    private readonly Collection<Char> disallowedCharacters;

    /// <summary>
    ///   Gets a collection of characters allowed in the string.
    /// </summary>
    /// <value>
    ///   A collection of characters allowed in the string.
    /// </value>
    public Collection<Char> DisallowedCharacters {
      get { return this.disallowedCharacters; }
    }
    #endregion

    #region Methods: Constructors
    /// <summary>
    ///   Initializes static members of the <see cref="StringValidationRule" /> class.
    /// </summary>
    static StringValidationRule() {
      Char[] commonChars = new Char[105];
      for (Int32 i = 0; i < 95; i++) {
        commonChars[i] = Encoding.ASCII.GetChars(new Byte[] { (Byte)(i + 32) })[0];
      }

      commonChars[95] = 'Ä';
      commonChars[96] = 'ä';
      commonChars[97] = 'Ö';
      commonChars[98] = 'ö';
      commonChars[99] = 'Ü';
      commonChars[100] = 'ü';
      commonChars[101] = 'ß';
      commonChars[102] = '\'';
      commonChars[103] = '§';
      commonChars[104] = '°';

      StringValidationRule.commonCharSet = new ReadOnlyCollection<Char>(commonChars);
    }

    public StringValidationRule() {
      this.allowWhitespacesOnly = true;
      this.allowWhitespacesAtStart = true;
      this.allowWhitespacesAtEnd = true;
      this.allowedMinLength = 1;
      this.allowedMaxLength = 0;
      this.allowedCharacters = new Collection<Char>();
      this.disallowedCharacters = new Collection<Char>();

      for (Int32 i = 0; i < StringValidationRule.CommonCharSet.Count; i++) {
        this.AllowedCharacters.Add(StringValidationRule.CommonCharSet[i]);
      }
    }
    #endregion

    #region Methods: Validate
    /// <summary>
    ///   Performs validation checks on the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="String" /> value from the binding target to check.
    /// </param>
    /// <param name="cultureInfo">
    ///   The culture to use in this rule.
    /// </param>
    /// <returns>
    ///   A <see cref="ValidationResult" /> object.
    /// </returns>
    public override ValidationResult Validate(Object value, CultureInfo cultureInfo) {
      if ((this.AllowNull) && (value == null)) {
        return new ValidationResult(true, null);
      }

      String stringValue = (value as String);
      
      if (stringValue is String) {
        if ((!this.AllowEmpty) && (stringValue.Length == 0)) {
          return new ValidationResult(false, "The value can not be empty.");
        }
        if ((!this.AllowWhitespacesOnly) && (stringValue.Trim().Length == 0)) {
          return new ValidationResult(false, "The value can not contain whitespaces only.");
        }
        if ((!this.AllowWhitespacesAtStart) && (stringValue.Length != 0) && (stringValue[0] == ' ')) {
          return new ValidationResult(false, "Whitespaces at the start are not allowed.");
        }
        if ((!this.AllowWhitespacesAtEnd) && (stringValue.Length != 0) && (stringValue[stringValue.Length - 1] == ' ')) {
          return new ValidationResult(false, "Whitespaces at the end are not allowed.");
        }
        if (stringValue.Length < this.AllowedMinLength) {
          return new ValidationResult(
            false, String.Format("The value has to be at least {0} characters long.", this.AllowedMinLength)
          );
        }
        if ((this.AllowedMaxLength > 0) && (stringValue.Length > this.AllowedMaxLength)) {
          return new ValidationResult(
            false, String.Format("The value can be at most {0} characters long.", this.AllowedMinLength)
          );
        }
        
        // Check the string for invalid/valid characters.
        for (Int32 i = 0; i < stringValue.Length; i++) {
          Char currentChar = stringValue[i];
          Boolean allowed = (
            (!this.DisallowedCharacters.Contains(currentChar)) && 
            (this.AllowedCharacters.Contains(currentChar))
          );
          
          if (!allowed) {
            if (!Char.IsControl(currentChar)) {
              if (Char.IsWhiteSpace(currentChar)) {
                return new ValidationResult(false, "The value may not contain whitespaces.");
              } else {
                return new ValidationResult(false, String.Format("The value may not contain \"{0}\".", currentChar));
              }
            } else {
              // It's a control character, so we can't print it.
              if ((currentChar == '\n') || (currentChar == '\r')) {
                return new ValidationResult(false, "The value has to be on a single line.");
              } else {
                return new ValidationResult(false, "The value contains invalid characters.");
              }
            }
          }
        }

        return new ValidationResult(true, null);
      }

      return new ValidationResult(false, "Invalid type, String expected.");
    }
    #endregion
  }
}
