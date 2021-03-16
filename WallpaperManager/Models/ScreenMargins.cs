// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Common;
using Common.Text;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains margin definitions (left, right, top and bottom) for a screen.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ScreenMargins : INotifyPropertyChanged, ICloneable, IAssignable {
    /// <summary>
    ///   <inheritdoc cref="Bottom" select='../value/node()' />
    /// </summary>
    private int bottom;

    /// <summary>
    ///   <inheritdoc cref="Left" select='../value/node()' />
    /// </summary>
    private int left;

    /// <summary>
    ///   <inheritdoc cref="Right" select='../value/node()' />
    /// </summary>
    private int right;

    /// <summary>
    ///   <inheritdoc cref="Top" select='../value/node()' />
    /// </summary>
    private int top;

    /// <summary>
    ///   Gets or sets the left border value in pixels.
    /// </summary>
    /// <value>
    ///   The left border value in pixels.
    /// </value>
    public int Left {
      get { return this.left; }
      set {
        this.left = value;
        this.OnPropertyChanged("Left");
      }
    }

    /// <summary>
    ///   Gets or sets the right border value in pixels.
    /// </summary>
    /// <value>
    ///   The right border value in pixels.
    /// </value>
    public int Right {
      get { return this.right; }
      set {
        this.right = value;
        this.OnPropertyChanged("Right");
      }
    }

    /// <summary>
    ///   Gets or sets the top border value in pixels.
    /// </summary>
    /// <value>
    ///   The top border value in pixels.
    /// </value>
    public int Top {
      get { return this.top; }
      set {
        this.top = value;
        this.OnPropertyChanged("Top");
      }
    }

    /// <summary>
    ///   Gets or sets the bottom border value in pixels.
    /// </summary>
    /// <value>
    ///   The bottom border value in pixels.
    /// </value>
    public int Bottom {
      get { return this.bottom; }
      set {
        this.bottom = value;
        this.OnPropertyChanged("Bottom");
      }
    }

    /// <inheritdoc />
    public override string ToString() {
      return StringGenerator.FromListKeyed(
        new[] {"Left", "Right", "Top", "Bottom"},
        (IList<object>)new object[] {this.Left, this.Right, this.Top, this.Bottom});
    }

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public object Clone() {
      return this.MemberwiseClone();
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
    ///   <paramref name="other" /> is not castable to the <see cref="ScreenMargins" /> type.
    /// </exception>
    public void AssignTo(object other) {
      if (other == null) throw new ArgumentNullException();
      if (!(other is ScreenMargins)) throw new ArgumentException();

      ScreenMargins otherInstance = (ScreenMargins)other;
      otherInstance.Left = this.Left;
      otherInstance.Top = this.Top;
      otherInstance.Right = this.Right;
      otherInstance.Bottom = this.Bottom;
    }
    #endregion

    #region INotifyPropertyChanged Implementation
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}