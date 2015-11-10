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

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains overlay text related data.
  /// </summary>
  /// <remarks>
  ///   Wallpaper Overlay Texts are meant to be drawn over a wallpaper when it is applied.
  /// </remarks>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperTextOverlay: ValidatableBase, IWallpaperTextOverlay, ICloneable, IAssignable {
    /// <summary>
    ///   Represents the default font name.
    /// </summary>
    private const string DefaultFontName = "Verdana";

    /// <summary>
    ///   Represents the default font size in points.
    /// </summary>
    private const float DefaultFontSize = 12;

    /// <summary>
    ///   Represents the default <see cref="FontStyle" />.
    /// </summary>
    private const FontStyle DefaultFontStyle = 0;

    /// <summary>
    ///   Represents the default <see cref="Position" /> value.
    /// </summary>
    private const TextOverlayPosition DefaultPosition = TextOverlayPosition.BottomRight;

    /// <summary>
    ///   Gets the default <see cref="ForeColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="ForeColor" /> value.
    /// </value>
    private static Color DefaultForeColor => Color.White;

    /// <summary>
    ///   Gets the default <see cref="BorderColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="BorderColor" /> value.
    /// </value>
    private static Color DefaultBorderColor => Color.Black;

    /// <inheritdoc />
    public string Format { get; set; }

    /// <inheritdoc />
    public string FontName { get; set; }

    /// <inheritdoc />
    public float FontSize { get; set; }

    /// <inheritdoc />
    public FontStyle FontStyle { get; set; }

    /// <inheritdoc />
    public Color ForeColor { get; set; }

    /// <inheritdoc />
    public Color BorderColor { get; set; }

    /// <inheritdoc />
    public int HorizontalOffset { get; set; }

    /// <inheritdoc />
    public int VerticalOffset { get; set; }

    /// <inheritdoc />
    public TextOverlayPosition Position { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperTextOverlay" /> class.
    /// </summary>
    public WallpaperTextOverlay() {
      this.FontName = WallpaperTextOverlay.DefaultFontName;
      this.FontSize = WallpaperTextOverlay.DefaultFontSize;
      this.FontStyle = WallpaperTextOverlay.DefaultFontStyle;
      this.Format = LocalizationManager.GetLocalizedString("OverlayTextData.DefaultFormat");
      this.ForeColor = WallpaperTextOverlay.DefaultForeColor;
      this.BorderColor = WallpaperTextOverlay.DefaultBorderColor;
      this.Position = WallpaperTextOverlay.DefaultPosition;
    }
    
    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.Format))
        if (string.IsNullOrWhiteSpace(this.Format))
          return "This field is mandatory.";
      else if (propertyName == nameof(this.FontName))
        if (string.IsNullOrWhiteSpace(this.FontName))
          return "This field is mandatory.";
      else if (propertyName == nameof(this.FontSize))
        if (this.FontSize <= 0)
          return "Must be a postive number.";
      
      return null;
    }
    #endregion

    // TODO: split up, maybe extract?
    /// <inheritdoc />
    public string GetEvaluatedText(IList<IWallpaper> wallpapers) {
      StringBuilder evaluatedText = new StringBuilder();

      bool inParam = false;
      int lastParamStart = 0;
      for (int i = 0; i < this.Format.Length; i++) {
        char currentChar = this.Format[i];

        if (currentChar == '%') {
          if (inParam) {
            string parameter = this.Format.Substring(lastParamStart + 1, i - lastParamStart - 1);
            parameter = parameter.ToUpperInvariant();

            switch (parameter) {
              case "DATE":
                evaluatedText.Append(DateTime.Today.ToString("dd.MM.yy", CultureInfo.CurrentCulture));
                break;
              case "DATEFMT":
                evaluatedText.Append(DateTime.Today.ToString("MMMM d, yyyy", CultureInfo.CurrentCulture));
                break;
              case "TIME":
                evaluatedText.Append(DateTime.Now.ToString("hh:mm:ss", CultureInfo.CurrentCulture));
                break;
              case "DAYOFWEEK":
                evaluatedText.Append(DateTime.Today.DayOfWeek);
                break;
              case "USERNAME":
                evaluatedText.Append(Environment.UserName);
                break;
              case "MACHINENAME":
                evaluatedText.Append(Environment.MachineName);
                break;
              case "OSVERSION":
                evaluatedText.Append(Environment.OSVersion.VersionString);
                break;
              case "SYSTEMRUNTIME":
                TimeSpan runtime = TimeSpan.FromMilliseconds(Environment.TickCount);

                if (runtime.Days > 0)
                  evaluatedText.Append(string.Format(CultureInfo.CurrentCulture, "{0}.{1:00}:{2:00}:{3:00}", runtime.Days, runtime.Hours, runtime.Minutes, runtime.Seconds));
                else
                  evaluatedText.Append(string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", runtime.Hours, runtime.Minutes, runtime.Seconds));

                break;
              case "SYSTEMRUNTIMEFMT":
                TimeSpan runtime2 = TimeSpan.FromMilliseconds(Environment.TickCount);

                if (runtime2.Days > 0)
                  evaluatedText.Append(string.Format(CultureInfo.CurrentCulture, "{0} days, {1} hours, {2} minutes", runtime2.Days, runtime2.Hours, runtime2.Minutes));
                else
                  evaluatedText.Append(string.Format(CultureInfo.CurrentCulture, "{0} hours, {1} minutes", runtime2.Hours, runtime2.Minutes));

                break;
              case "SYSTEMSTARTTIME":
                TimeSpan runtime3 = TimeSpan.FromMilliseconds(Environment.TickCount);

                evaluatedText.Append(DateTime.Now.Subtract(runtime3).ToString("hh:mm:ss", CultureInfo.CurrentCulture));
                break;
              case "SYSTEMSTARTTIMED":
                TimeSpan runtime4 = TimeSpan.FromMilliseconds(Environment.TickCount);

                evaluatedText.Append(DateTime.Now.Subtract(runtime4).ToString("dd.MM.yy, hh:mm:ss", CultureInfo.CurrentCulture));
                break;
              case "SYSTEMSTARTTIMEDFMT":
                TimeSpan runtime5 = TimeSpan.FromMilliseconds(Environment.TickCount);

                evaluatedText.Append(DateTime.Now.Subtract(runtime5).ToString("MMMM d, yyyy hh:mm:ss", CultureInfo.CurrentCulture));
                break;
              case "LB":
                evaluatedText.AppendLine();
                break;
              default:
                if (parameter.Length > 9 && parameter.StartsWith("WALLPAPER", StringComparison.OrdinalIgnoreCase)) {
                  int screenIndex;

                  if (int.TryParse(parameter.Substring(9), out screenIndex)) {
                    if (screenIndex > wallpapers.Count || screenIndex <= 0)
                      screenIndex = 1;

                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None)
                      evaluatedText.Append(filePath.FileNameWithoutExt);
                  }
                }

                if (parameter.Length > 13 && parameter.StartsWith("WALLPAPERFILE", StringComparison.OrdinalIgnoreCase)) {
                  int screenIndex;

                  if (int.TryParse(parameter.Substring(13), out screenIndex)) {
                    if (screenIndex > wallpapers.Count || screenIndex <= 0)
                      screenIndex = 1;

                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None)
                      evaluatedText.Append(filePath.FileName);
                  }
                }

                if (parameter.Length > 13 && parameter.StartsWith("WALLPAPERPATH", StringComparison.OrdinalIgnoreCase)) {
                  int screenIndex;

                  if (int.TryParse(parameter.Substring(13), out screenIndex)) {
                    if (screenIndex > wallpapers.Count || screenIndex <= 0)
                      screenIndex = 1;

                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None)
                      evaluatedText.Append(filePath);
                  }
                }
                break;
            }

            inParam = false;
          } else {
            inParam = true;
            lastParamStart = i;
          }
        } else {
          if (!inParam)
            evaluatedText.Append(currentChar);
        }
      }

      return evaluatedText.ToString();
    }

    /// <summary>
    ///   Gets the <see cref="FontName" />, <see cref="FontSize" /> and <see cref="FontStyle" /> property from the given
    ///   <paramref name="sourceFont" />.
    /// </summary>
    /// <param name="sourceFont">
    ///   The <see cref="Font" /> where the settings should be get from.
    /// </param>
    public void FontSettingsFromFont(Font sourceFont) {
      this.FontName = sourceFont.Name;
      this.FontSize = sourceFont.Size;
      this.FontStyle = sourceFont.Style;
    }

    /// <summary>
    ///   Creates a new <see cref="Font" /> instance with the <see cref="FontName" />, <see cref="FontSize" /> and
    ///   <see cref="FontStyle" /> properties assigned.
    /// </summary>
    /// <returns>
    ///   A new <see cref="Font" /> instance with the <see cref="FontName" />, <see cref="FontSize" /> and
    ///   <see cref="FontStyle" /> properties assigned.
    /// </returns>
    public Font FontSettingsToFont() {
      return new Font(this.FontName, this.FontSize, this.FontStyle);
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual object Clone() {
      return (WallpaperTextOverlay)this.MemberwiseClone();
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is WallpaperTextOverlay);

      WallpaperTextOverlay otherInstance = (WallpaperTextOverlay)other;
      otherInstance.Format = this.Format;
      otherInstance.FontName = this.FontName;
      otherInstance.FontSize = this.FontSize;
      otherInstance.FontStyle = this.FontStyle;
      otherInstance.ForeColor = this.ForeColor;
      otherInstance.BorderColor = this.BorderColor;
      otherInstance.Position = this.Position;
    }
    #endregion
  }
}