// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
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
  [DataContract]
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
    [DataMember(Order = 1)]
    public string Format { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 2)]
    public string FontName { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 3)]
    public float FontSize { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 4)]
    public Color ForeColor { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 5)]
    public FontStyle FontStyle { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 6)]
    public Color BorderColor { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 7)]
    public int HorizontalOffset { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 8)]
    public int VerticalOffset { get; set; }

    /// <inheritdoc />
    [DataMember(Order = 9)]
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
      } else if (propertyName == nameof(this.Position)) {
        if (!Enum.IsDefined(typeof(TextOverlayPosition), this.Position))
          return LocalizationManager.GetLocalizedString("Error.FieldIsInvalid");
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