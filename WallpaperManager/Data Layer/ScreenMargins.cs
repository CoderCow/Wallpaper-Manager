// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Common;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Contains margin definitions (left, right, top and bottom) for a screen.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class ScreenMargins: INotifyPropertyChanged, ICloneable, IAssignable {
    #region Property: Left
    /// <summary>
    ///   <inheritdoc cref="Left" select='../value/node()' />
    /// </summary>
    private Int32 left;

    /// <summary>
    ///   Gets or sets the left border value in pixels.
    /// </summary>
    /// <value>
    ///   The left border value in pixels.
    /// </value>
    public Int32 Left {
      get { return this.left; }
      set {
        this.left = value;
        this.OnPropertyChanged("Left");
      }
    }
    #endregion

    #region Property: Right
    /// <summary>
    ///   <inheritdoc cref="Right" select='../value/node()' />
    /// </summary>
    private Int32 right;

    /// <summary>
    ///   Gets or sets the right border value in pixels.
    /// </summary>
    /// <value>
    ///   The right border value in pixels.
    /// </value>
    public Int32 Right {
      get { return this.right; }
      set {
        this.right = value;
        this.OnPropertyChanged("Right");
      }
    }
    #endregion

    #region Property: Top
    /// <summary>
    ///   <inheritdoc cref="Top" select='../value/node()' />
    /// </summary>
    private Int32 top;

    /// <summary>
    ///   Gets or sets the top border value in pixels.
    /// </summary>
    /// <value>
    ///   The top border value in pixels.
    /// </value>
    public Int32 Top {
      get { return this.top; }
      set {
        this.top = value;
        this.OnPropertyChanged("Top");
      }
    }
    #endregion

    #region Property: Bottom
    /// <summary>
    ///   <inheritdoc cref="Bottom" select='../value/node()' />
    /// </summary>
    private Int32 bottom;

    /// <summary>
    ///   Gets or sets the bottom border value in pixels.
    /// </summary>
    /// <value>
    ///   The bottom border value in pixels.
    /// </value>
    public Int32 Bottom {
      get { return this.bottom; }
      set {
        this.bottom = value;
        this.OnPropertyChanged("Bottom");
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenMargins" /> class.
    /// </summary>
    public ScreenMargins() {}
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public Object Clone() {
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
    public void AssignTo(Object other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      ScreenMargins otherInstance = (other as ScreenMargins);
      if (otherInstance == null) {
        throw new ArgumentException(ExceptionMessages.GetTypeIsNotCastable("Object", "ScreenMargins", "other"));
      }

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
    protected virtual void OnPropertyChanged(String propertyName) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion

    #region Methods: ToString
    /// <inheritdoc />
    public override String ToString() {
      return StringGenerator.FromListKeyed(
        new String[] { "Left", "Right", "Top", "Bottom" },
        (IList<Object>)new Object[] { this.Left, this.Right, this.Top, this.Bottom }
      );
    }
    #endregion
  }
}
