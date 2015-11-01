// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Common.Presentation;

using WallpaperManager.Models;

namespace WallpaperManager.ViewModels {
  /// <commondoc select='WrappingCollectionViewModels/General/*' params="WrappedType=WallpaperTextOverlay" />
  /// <threadsafety static="true" instance="false" />
  public class ConfigTextOverlaysVM: INotifyPropertyChanged {
    #region Property: TextOverlays
    /// <summary>
    ///   <inheritdoc cref="TextOverlays" select='../value/node()' />
    /// </summary>
    private readonly ObservableCollection<WallpaperTextOverlay> textOverlays;

    /// <summary>
    ///   Gets the <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </value>
    public ObservableCollection<WallpaperTextOverlay> TextOverlays {
      get { return this.textOverlays; }
    }
    #endregion

    #region Property: SelectedItemIndex
    /// <summary>
    ///   <inheritdoc cref="SelectedItemIndex" select='../value/node()' />
    /// </summary>
    private Int32 selectedItemIndex;

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="WallpaperTextOverlay" /> item.
    /// </summary>
    /// <value>
    ///   The index of the selected <see cref="WallpaperTextOverlay" /> item.
    /// </value>
    public Int32 SelectedItemIndex {
      get { return this.selectedItemIndex; }
      set {
        this.selectedItemIndex = value;
        this.OnPropertyChanged("SelectedItemIndex");
        this.OnPropertyChanged("SelectedItem");
      }
    }
    #endregion

    #region Property: SelectedItem
    /// <summary>
    ///   Gets the selected <see cref="WallpaperTextOverlay" /> item.
    /// </summary>
    /// <value>
    ///   The selected <see cref="WallpaperTextOverlay" /> item.
    /// </value>
    public WallpaperTextOverlay SelectedItem {
      get {
        if ((this.TextOverlays.Count > 0) && (this.SelectedItemIndex != -1)) {
          return this.TextOverlays[this.SelectedItemIndex];
        }

        return null;
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigTextOverlaysVM" /> class.
    /// </summary>
    /// <param name="textOverlays">
    ///   The <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="textOverlays" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    public ConfigTextOverlaysVM(ObservableCollection<WallpaperTextOverlay> textOverlays) {
      if (textOverlays == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("TextOverlays"));
      }

      this.textOverlays = textOverlays;
    }
    #endregion

    #region Command: AddTextOverlay
    /// <summary>
    ///   <inheritdoc cref="AddTextOverlayCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand addTextOverlayCommand;

    /// <summary>
    ///   Gets the Add OverlayText <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Add OverlayText <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="AddTextOverlayCommand_CanExecute">AddTextOverlayCommand_CanExecute Method</seealso>
    /// <seealso cref="AddTextOverlayCommand_Execute">AddTextOverlayCommand_Execute Method</seealso>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    public DelegateCommand AddTextOverlayCommand {
      get {
        if (this.addTextOverlayCommand == null) {
          this.addTextOverlayCommand = new DelegateCommand(
            this.AddTextOverlayCommand_Execute, this.AddTextOverlayCommand_CanExecute
          );
        }
    
        return this.addTextOverlayCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="AddTextOverlayCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="AddTextOverlayCommand" />
    protected Boolean AddTextOverlayCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="AddTextOverlayCommand" /> is executed.
    ///   Adds a new <see cref="WallpaperTextOverlay" /> to the wrapped <see cref="TextOverlays" />.
    /// </summary>
    /// <seealso cref="AddTextOverlayCommand" />
    protected void AddTextOverlayCommand_Execute() {
      this.TextOverlays.Add(new WallpaperTextOverlay());
      this.SelectedItemIndex = this.TextOverlays.Count - 1;
    }
    #endregion

    #region Command: RemoveTextOverlay
    /// <summary>
    ///   <inheritdoc cref="RemoveTextOverlayCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand removeTextOverlayCommand;

    /// <summary>
    ///   Gets the Remove Overlay Text <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Remove Overlay Text <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RemoveTextOverlayCommand_CanExecute">RemoveTextOverlayCommand_CanExecute Method</seealso>
    /// <seealso cref="RemoveTextOverlayCommand_Execute">RemoveTextOverlayCommand_Execute Method</seealso>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    public DelegateCommand RemoveTextOverlayCommand {
      get {
        if (this.removeTextOverlayCommand == null) {
          this.removeTextOverlayCommand = new DelegateCommand(
            this.RemoveTextOverlayCommand_Execute, this.RemoveTextOverlayCommand_CanExecute
          );
        }
    
        return this.removeTextOverlayCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveTextOverlayCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveTextOverlayCommand" />
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    protected Boolean RemoveTextOverlayCommand_CanExecute() {
      return (this.SelectedItem != null);
    }

    /// <summary>
    ///   Called when <see cref="RemoveTextOverlayCommand" /> is executed.
    ///   Removes the selected <see cref="WallpaperTextOverlay" /> from the wrapped <see cref="TextOverlays" />.
    /// </summary>
    /// <seealso cref="RemoveTextOverlayCommand" />
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    protected void RemoveTextOverlayCommand_Execute() {
      this.TextOverlays.RemoveAt(this.SelectedItemIndex);
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
