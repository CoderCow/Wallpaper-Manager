// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

using Common;
using Common.IO;

using WallpaperManager.Application;

namespace WallpaperManager.Data {
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
  public class WallpaperOverlayText: INotifyPropertyChanged, ICloneable, IAssignable {
    #region Constants: DefaultFormat, DefaultFontName, DefaultFontSize, DefaultFontStyle, DefaultForeColor, DefaultBorderColor, DefaultPosition
    /// <summary>
    ///   Represents the default <see cref="Format" /> string.
    /// </summary>
    private const String DefaultFormat = "New Overlay Text";

    /// <summary>
    ///   Represents the default font name.
    /// </summary>
    private const String DefaultFontName = "Verdana";

    /// <summary>
    ///   Represents the default font size in points.
    /// </summary>
    private const Single DefaultFontSize = 12;

    /// <summary>
    ///   Represents the default <see cref="FontStyle" />.
    /// </summary>
    private const FontStyle DefaultFontStyle = 0;

    /// <summary>
    ///   Gets the default <see cref="ForeColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="ForeColor" /> value.
    /// </value>
    private static Color DefaultForeColor {
      get { return Color.White; }
    }

    /// <summary>
    ///   Gets the default <see cref="BorderColor" /> value.
    /// </summary>
    /// <value>
    ///   The default <see cref="BorderColor" /> value.
    /// </value>
    private static Color DefaultBorderColor {
      get { return Color.Black; }
    }

    /// <summary>
    ///   Represents the default <see cref="Position" /> value.
    /// </summary>
    private const OverlayTextPosition DefaultPosition = OverlayTextPosition.BottomRight;
    #endregion

    #region Property: Format
    /// <summary>
    ///   <inheritdoc cref="Format" select='../value/node()' />
    /// </summary>
    private String format;
    
    /// <summary>
    ///   Gets or sets the content text.
    /// </summary>
    /// <value>
    ///   The content text.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public String Format {
      get { return this.format; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }
        
        this.format = value;
        this.OnPropertyChanged("Format");
      }
    }
    #endregion

    #region Property: FontName
    /// <summary>
    ///   <inheritdoc cref="FontName" select='../value/node()' />
    /// </summary>
    private String fontName;

    /// <summary>
    ///   Gets or sets the content text's font name.
    /// </summary>
    /// <value>
    ///   The content text's font name.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public String FontName {
      get { return this.fontName; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }
        if (value.Trim().Length == 0) {
          throw new ArgumentNullException(ExceptionMessages.GetStringIsEmptyOrClear());
        }

        this.fontName = value;
        this.OnPropertyChanged("FontName");
      }
    }
    #endregion

    #region Property: FontSize
    /// <summary>
    ///   <inheritdoc cref="FontSize" select='../value/node()' />
    /// </summary>
    private Single fontSize;

    /// <summary>
    ///   Gets or sets the content text's font size in points.
    /// </summary>
    /// <value>
    ///   The content text's font size in points.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public Single FontSize {
      get { return this.fontSize; }
      set {
        this.fontSize = value;
        this.OnPropertyChanged("FontSize");
      }
    }
    #endregion

    #region Property: FontStyle
    /// <summary>
    ///   <inheritdoc cref="FontStyle" select='../value/node()' />
    /// </summary>
    private FontStyle fontStyle;

    /// <summary>
    ///   Gets or sets the content text's font style.
    /// </summary>
    /// <value>
    ///   The content text's font style.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public FontStyle FontStyle {
      get { return this.fontStyle; }
      set {
        this.fontStyle = value;
        this.OnPropertyChanged("FontStyle");
      }
    }
    #endregion

    #region Property: ForeColor
    /// <summary>
    ///   <inheritdoc cref="ForeColor" select='../value/node()' />
    /// </summary>
    private Color foreColor;

    /// <summary>
    ///   Gets or sets the content text's color.
    /// </summary>
    /// <value>
    ///   The content text's color.
    /// </value>
    public Color ForeColor {
      get { return this.foreColor; }
      set {
        this.foreColor = value;
        this.OnPropertyChanged("ForeColor");
      }
    }
    #endregion

    #region Property: BorderColor
    /// <summary>
    ///   <inheritdoc cref="BorderColor" select='../value/node()' />
    /// </summary>
    private Color borderColor;

    /// <summary>
    ///   Gets or sets the content text's border color.
    /// </summary>
    /// <value>
    ///   The content text's border color.
    /// </value>
    public Color BorderColor {
      get { return this.borderColor; }
      set {
        this.borderColor = value;
        this.OnPropertyChanged("BorderColor");
      }
    }
    #endregion

    #region Properties: Offset, HorizontalOffset, VerticalOffset
    /// <summary>
    ///   <inheritdoc cref="Offset" select='../value/node()' />
    /// </summary>
    private Point offset;

    /// <summary>
    ///   Gets or sets the content text's horizontal and vertical offset.
    /// </summary>
    /// <value>
    ///   The content text's horizontal and vertical offset.
    /// </value>
    public Point Offset {
      get { return this.offset; }
      set {
        this.offset = value;
        this.OnPropertyChanged("Offset");
      }
    }

    /// <summary>
    ///   Gets or sets the content text's horizontal offset.
    /// </summary>
    /// <value>
    ///   The content text's horizontal offset.
    /// </value>
    public Int32 HorizontalOffset {
      get { return this.offset.X; }
      set {
        this.offset = new Point(value, this.Offset.Y);
        this.OnPropertyChanged("Offset");
      }
    }

    /// <summary>
    ///   Gets or sets the content text's vertical offset.
    /// </summary>
    /// <value>
    ///   The content text's vertical offset.
    /// </value>
    public Int32 VerticalOffset {
      get { return this.offset.Y; }
      set {
        this.offset = new Point(this.Offset.X, value);
        this.OnPropertyChanged("Offset");
      }
    }
    #endregion

    #region Property: Position
    /// <summary>
    ///   <inheritdoc cref="Position" select='../value/node()' />
    /// </summary>
    private OverlayTextPosition position;

    /// <summary>
    ///   Gets or sets the position where the text should be displayed on the screen.
    /// </summary>
    /// <value>
    ///   The position where the text should be displayed on the screen.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is no constant defined by the <see cref="OverlayTextPosition" /> enumeration.
    /// </exception>
    /// <seealso cref="OverlayTextPosition">OverlayTextPosition Enumeration</seealso>
    public OverlayTextPosition Position {
      get { return this.position; }
      set {
        if (!Enum.IsDefined(typeof(OverlayTextPosition), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(OverlayTextPosition), value));
        }

        this.position = value;
        this.OnPropertyChanged("Position");
      }
    }
    #endregion

    
    #region Methods: Constructor, GetEvaluatedText
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperOverlayText" /> class.
    /// </summary>
    public WallpaperOverlayText() {
      this.fontName = WallpaperOverlayText.DefaultFontName;
      this.fontSize = WallpaperOverlayText.DefaultFontSize;
      this.fontStyle = WallpaperOverlayText.DefaultFontStyle;
      this.format = WallpaperOverlayText.DefaultFormat;
      this.foreColor = WallpaperOverlayText.DefaultForeColor;
      this.borderColor = WallpaperOverlayText.DefaultBorderColor;
      this.position = WallpaperOverlayText.DefaultPosition;
    }

    /// <summary>
    ///   Returns the format string with replaced parameter values.
    /// </summary>
    /// <param name="wallpapers">
    ///   The collection of <see cref="Wallpaper" /> objects which are actually being applied.
    /// </param>
    /// <returns>
    ///   The format string with replaced parameter values.
    /// </returns>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public String GetEvaluatedText(IList<Wallpaper> wallpapers) {
      StringBuilder evaluatedText = new StringBuilder();
      
      Boolean inParam = false;
      Int32 lastParamStart = 0;
      for (Int32 i = 0; i < this.Format.Length; i++) {
        Char currentChar = this.Format[i];

        if (currentChar == '%') {
          if (inParam) {
            String parameter = this.Format.Substring(lastParamStart + 1, i - lastParamStart - 1);
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
                evaluatedText.Append(DateTime.Today.DayOfWeek.ToString());
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
                
                if (runtime.Days > 0) {
                  evaluatedText.Append(String.Format(
                    CultureInfo.CurrentCulture, 
                    "{0}.{1:00}:{2:00}:{3:00}", runtime.Days, runtime.Hours, runtime.Minutes, runtime.Seconds)
                  );
                } else {
                  evaluatedText.Append(String.Format(
                    CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", runtime.Hours, runtime.Minutes, runtime.Seconds)
                  );
                }
                break;
              case "SYSTEMRUNTIMEFMT":
                TimeSpan runtime2 = TimeSpan.FromMilliseconds(Environment.TickCount);
                
                if (runtime2.Days > 0) {
                  evaluatedText.Append(String.Format(
                    CultureInfo.CurrentCulture, "{0} days, {1} hours, {2} minutes", runtime2.Days, runtime2.Hours, runtime2.Minutes)
                  );
                } else {
                  evaluatedText.Append(String.Format(
                    CultureInfo.CurrentCulture, "{0} hours, {1} minutes", runtime2.Hours, runtime2.Minutes)
                  );
                }
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
                if ((parameter.Length > 9) && (parameter.StartsWith("WALLPAPER", StringComparison.OrdinalIgnoreCase))) {
                  Int32 screenIndex;

                  if (Int32.TryParse(parameter.Substring(9), out screenIndex)) {
                    if (screenIndex >= wallpapers.Count) {
                      screenIndex = 1;
                    }

                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None) {
                      evaluatedText.Append(filePath.FileNameWithoutExt);
                    }
                  }
                }
                if ((parameter.Length > 13) && (parameter.StartsWith("WALLPAPERFILE", StringComparison.OrdinalIgnoreCase))) {
                  Int32 screenIndex;

                  if (Int32.TryParse(parameter.Substring(13), out screenIndex)) {
                    if (screenIndex >= wallpapers.Count) {
                      screenIndex = 1;
                    }
                    
                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None) {
                      evaluatedText.Append(filePath.FileName);
                    }
                  }
                }
                if ((parameter.Length > 13) && (parameter.StartsWith("WALLPAPERPATH", StringComparison.OrdinalIgnoreCase))) {
                  Int32 screenIndex;

                  if (Int32.TryParse(parameter.Substring(13), out screenIndex)) {
                    if (screenIndex >= wallpapers.Count) {
                      screenIndex = 1;
                    }

                    Path filePath = wallpapers[screenIndex - 1].ImagePath;
                    if (filePath != Path.None) {
                      evaluatedText.Append(filePath);
                    }
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
          if (!inParam) {
            evaluatedText.Append(currentChar);
          }
        }
      }

      return evaluatedText.ToString();
    }
    #endregion
    
    #region Methods: FontSettingsFromFont, FontSettingsToFont
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
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public virtual Object Clone() {
      return new WallpaperOverlayText() {
        Format = this.Format,
        FontName = this.FontName,
        FontSize = this.FontSize,
        FontStyle = this.FontStyle,
        ForeColor = this.ForeColor,
        BorderColor = this.BorderColor,
        Offset = this.Offset,
        Position = this.Position,
      };
    }

    /// <summary>
    ///   Assigns all member values of this instance to the respective members of the given instance.
    /// </summary>
    /// <param name="other">
    ///   The target instance to assign to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="other" /> is not castable to the <see cref="WallpaperOverlayText" /> type.
    /// </exception>
    public virtual void AssignTo(Object other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      WallpaperOverlayText otherInstance = other as WallpaperOverlayText;
      if (otherInstance == null) {
        throw new ArgumentException(ExceptionMessages.GetTypeIsNotCastable("Object", "WallpaperOverlayText", "other"));
      }

      otherInstance.Format = this.Format;
      otherInstance.FontName = this.FontName;
      otherInstance.FontSize = this.FontSize;
      otherInstance.FontStyle = this.FontStyle;
      otherInstance.ForeColor = this.ForeColor;
      otherInstance.BorderColor = this.BorderColor;
      otherInstance.Offset = this.Offset;
      otherInstance.Position = this.Position;
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}
