using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Controls;

namespace Common.Presentation {
  /// <summary>
  ///   A rule which checks if a <see cref="String" /> object can be converted to a <see cref="Color" /> object.
  /// </summary>
  /// <remarks>
  ///   The <see cref="Validate(Object, CultureInfo)" /> method uses <see cref="ColorTranslator" /> to parse the value which 
  ///   supports formats like "#AARRGGBB", "#RRGGBB", "#ARGB", "#RGB" or "Colorname".
  /// </remarks>
  /// <threadsafety static="false" instance="false" />
  public class ColorStringValidationRule: ValidationRule {
    #region Property: AllowTransparency
    /// <summary>
    ///   <inheritdoc cref="AllowTransparency" select='../value/node()' />
    /// </summary>
    private Boolean allowTransparency;

    /// <summary>
    ///   Gets or sets a value indicating whether a color string containing transparency information is allowed or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether a color string containing transparency information is allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowTransparency {
      get { return this.allowTransparency; }
      set { this.allowTransparency = value; }
    }
    #endregion

    #region Property: AllowNullAndEmpty
    /// <summary>
    ///   <inheritdoc cref="AllowNullAndEmpty" select='../value/node()' />
    /// </summary>
    private Boolean allowNullAndEmpty;

    /// <summary>
    ///   Gets or sets a value indicating whether <c>null</c> and an empty string is allowed or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether <c>null</c> and an empty string is allowed; otherwise <c>false</c>.
    /// </value>
    public Boolean AllowNullAndEmpty {
      get { return this.allowNullAndEmpty; }
      set { this.allowNullAndEmpty = value; }
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
      if ((this.AllowNullAndEmpty) && (value == null)) {
        return new ValidationResult(true, null);
      }

      String stringValue = (value as String);
      
      if (stringValue is String) {
        try {
          if (stringValue != String.Empty) {
            Color colorValue = ColorTranslator.FromHtml(stringValue);

            if ((!this.AllowTransparency) && (colorValue.A != 255)) {
              return new ValidationResult(
                false, "Transparency is not allowed, \"#RRGGBB\" or \"#RGB\" or a valid color name expected."
              );
            }

            return new ValidationResult(true, null);
          } else {
            if (this.AllowNullAndEmpty) {
              return new ValidationResult(true, null);
            }
          }
        } catch {
          // Simply doing nothing will return the invalid format error message.
          // We can't filter the exception type by ArgumentException and FormatException since for some dumb reason 
          // the FromHtml method throws an exception of type Exception...
        }
      }

      return new ValidationResult(
        false, "Invalid format, \"#AARRGGBB\" or \"#RRGGBB\" or \"#ARGB\" or \"#RGB\" or a valid color name expected."
      );
    }
    #endregion
  }
}
