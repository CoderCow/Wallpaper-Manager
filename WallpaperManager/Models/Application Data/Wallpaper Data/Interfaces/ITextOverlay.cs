using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  public interface ITextOverlay : ICloneable, IAssignable, INotifyPropertyChanged {
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
  }
}