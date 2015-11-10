using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IWallpaperTextOverlayContracts))]
  public interface IWallpaperTextOverlay : ICloneable, IAssignable {
    /// <summary>
    ///   Gets or sets the content text.
    /// </summary>
    /// <value>
    ///   The content text.
    /// </value>
    string Format { get; set; }

    /// <summary>
    ///   Gets or sets the content text's font name.
    /// </summary>
    /// <value>
    ///   The content text's font name.
    /// </value>
    string FontName { get; set; }

    /// <summary>
    ///   Gets or sets the content text's font size in points.
    /// </summary>
    /// <value>
    ///   The content text's font size in points.
    /// </value>
    float FontSize { get; set; }

    /// <summary>
    ///   Gets or sets the content text's font style.
    /// </summary>
    /// <value>
    ///   The content text's font style.
    /// </value>
    FontStyle FontStyle { get; set; }

    /// <summary>
    ///   Gets or sets the content text's color.
    /// </summary>
    /// <value>
    ///   The content text's color.
    /// </value>
    Color ForeColor { get; set; }

    /// <summary>
    ///   Gets or sets the content text's border color.
    /// </summary>
    /// <value>
    ///   The content text's border color.
    /// </value>
    Color BorderColor { get; set; }

    /// <summary>
    ///   Gets or sets the content text's horizontal offset.
    /// </summary>
    /// <value>
    ///   The content text's horizontal offset.
    /// </value>
    int HorizontalOffset { get; set; }

    /// <summary>
    ///   Gets or sets the content text's vertical offset.
    /// </summary>
    /// <value>
    ///   The content text's vertical offset.
    /// </value>
    int VerticalOffset { get; set; }

    /// <summary>
    ///   Gets or sets the position where the text should be displayed on the screen.
    /// </summary>
    /// <value>
    ///   The position where the text should be displayed on the screen.
    /// </value>
    /// <seealso cref="TextOverlayPosition">TextOverlayPosition Enumeration</seealso>
    TextOverlayPosition Position { get; set; }

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
    string GetEvaluatedText(IList<IWallpaper> wallpapers);
  }

  [ContractClassFor(typeof(IWallpaperTextOverlay))]
  internal abstract class IWallpaperTextOverlayContracts: IWallpaperTextOverlay {
    public abstract string Format { get; set; }
    public abstract string FontName { get; set; }
    public abstract float FontSize { get; set; }
    public abstract FontStyle FontStyle { get; set; }
    public abstract Color ForeColor { get; set; }
    public abstract Color BorderColor { get; set; }
    public abstract int HorizontalOffset { get; set; }
    public abstract int VerticalOffset { get; set; }
    public abstract TextOverlayPosition Position { get; set; }
    public abstract string GetEvaluatedText(IList<IWallpaper> wallpapers);

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(Enum.IsDefined(typeof(FontStyle), this.FontStyle));
      Contract.Invariant(Enum.IsDefined(typeof(TextOverlayPosition), this.Position));
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}