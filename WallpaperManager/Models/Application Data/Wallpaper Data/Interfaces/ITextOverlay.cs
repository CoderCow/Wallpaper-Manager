using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common;
using PropertyChanged;

namespace WallpaperManager.Models {
  [ContractClass(typeof(TextOverlayContracts))]
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

  [DoNotNotify]
  [ContractClassFor(typeof(ITextOverlay))]
  internal abstract class TextOverlayContracts: ITextOverlay {
    public abstract float FontSize { get; set; }
    public abstract Color ForeColor { get; set; }
    public abstract FontStyle FontStyle { get; set; }
    public abstract Color BorderColor { get; set; }
    public abstract int HorizontalOffset { get; set; }
    public abstract int VerticalOffset { get; set; }

    public string Format {
      get {
        Contract.Ensures(Contract.Result<string>() != null);
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentNullException>(value != null);
        throw new NotImplementedException();
      }
    }

    public string FontName {
      get {
        Contract.Ensures(Contract.Result<string>() != null);
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentNullException>(value != null);
        throw new NotImplementedException();
      }
    }

    public TextOverlayPosition Position {
      get {
        Contract.Ensures(Enum.IsDefined(typeof(TextOverlayPosition), Contract.Result<TextOverlayPosition>()));
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TextOverlayPosition), value));
        throw new NotImplementedException();
      }
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
    public abstract event PropertyChangedEventHandler PropertyChanged;
  }
}