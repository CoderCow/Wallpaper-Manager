// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Text;
using Common;
using Common.IO;
using Common.Presentation;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains overlay text related data.
  /// </summary>
  /// <remarks>
  ///   Wallpaper Overlay Texts are meant to be drawn over a wallpaper when it is applied.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [ImplementPropertyChanged]
  public class TextOverlay: ValidatableBase, ITextOverlay, ICloneable, IAssignable {
    /// <summary>
    ///   Represents the default font name.
    /// </summary>
    public const string DefaultFontName = "Verdana";

    /// <summary>
    ///   Represents the default font size in points.
    /// </summary>
    public const float DefaultFontSize = 12;

    /// <summary>
    ///   Represents the default <see cref="FontStyle" />.
    /// </summary>
    public const FontStyle DefaultFontStyle = 0;

    /// <summary>
    ///   Represents the default <see cref="Position" /> value.
    /// </summary>
    public const TextOverlayPosition DefaultPosition = TextOverlayPosition.BottomRight;

    /// <summary>
    ///   Gets the default <see cref="ForeColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="ForeColor" /> value.
    /// </value>
    public static Color DefaultForeColor => Color.White;

    /// <summary>
    ///   Gets the default <see cref="BorderColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="BorderColor" /> value.
    /// </value>
    public static Color DefaultBorderColor => Color.Black;

    /// <inheritdoc />
    public string Format { get; set; }

    /// <inheritdoc />
    public string FontName { get; set; }

    /// <inheritdoc />
    public float FontSize { get; set; }

    /// <inheritdoc />
    public Color ForeColor { get; set; }

    /// <inheritdoc />
    public FontStyle FontStyle { get; set; }

    /// <inheritdoc />
    public Color BorderColor { get; set; }

    /// <inheritdoc />
    public int HorizontalOffset { get; set; }

    /// <inheritdoc />
    public int VerticalOffset { get; set; }

    /// <inheritdoc />
    public TextOverlayPosition Position { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="TextOverlay" /> class.
    /// </summary>
    public TextOverlay() {
      this.FontName = DefaultFontName;
      this.FontSize = DefaultFontSize;
      this.FontStyle = DefaultFontStyle;
      this.Format = LocalizationManager.GetLocalizedString("OverlayTextData.DefaultFormat");
      this.ForeColor = DefaultForeColor;
      this.BorderColor = DefaultBorderColor;
      this.Position = DefaultPosition;
    }
    
    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.Format)) {
        if (string.IsNullOrWhiteSpace(this.Format))
          return LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      } else if (propertyName == nameof(this.FontName)) {
        if (string.IsNullOrWhiteSpace(this.FontName))
          return LocalizationManager.GetLocalizedString("Error.FieldIsMandatory");
      } else if (propertyName == nameof(this.FontSize)) {
        if (this.FontSize <= 0)
          return LocalizationManager.GetLocalizedString("Error.Number.MustBePositive");
      }

      return null;
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual object Clone() {
      return (TextOverlay)this.MemberwiseClone();
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is TextOverlay);

      TextOverlay otherInstance = (TextOverlay)other;
      otherInstance.Format = this.Format;
      otherInstance.FontName = this.FontName;
      otherInstance.FontSize = this.FontSize;
      otherInstance.FontStyle = this.FontStyle;
      otherInstance.ForeColor = this.ForeColor;
      otherInstance.BorderColor = this.BorderColor;
      otherInstance.Position = this.Position;
      otherInstance.HorizontalOffset = this.HorizontalOffset;
      otherInstance.VerticalOffset = this.VerticalOffset;
    }
    #endregion
  }
}