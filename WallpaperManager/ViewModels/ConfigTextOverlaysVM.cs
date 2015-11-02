// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Common;
using Common.Presentation;
using WallpaperManager.Models;

namespace WallpaperManager.ViewModels {
  /// <commondoc select='WrappingCollectionViewModels/General/*' params="WrappedType=WallpaperTextOverlay" />
  /// <threadsafety static="true" instance="false" />
  public class ConfigTextOverlaysVM : INotifyPropertyChanged {
    /// <summary>
    ///   <inheritdoc cref="SelectedItemIndex" select='../value/node()' />
    /// </summary>
    private int selectedItemIndex;

    /// <summary>
    ///   Gets the selected <see cref="WallpaperTextOverlay" /> item.
    /// </summary>
    /// <value>
    ///   The selected <see cref="WallpaperTextOverlay" /> item.
    /// </value>
    public WallpaperTextOverlay SelectedItem {
      get {
        if (this.SelectedItemIndex != -1)
          return this.TextOverlays[this.SelectedItemIndex];

        return null;
      }
    }

    /// <summary>
    ///   Gets the <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </value>
    public ObservableCollection<WallpaperTextOverlay> TextOverlays { get; }

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="WallpaperTextOverlay" /> item.
    /// </summary>
    /// <value>
    ///   The index of the selected <see cref="WallpaperTextOverlay" /> item.
    /// </value>
    public int SelectedItemIndex {
      get { return this.selectedItemIndex; }
      set {
        this.selectedItemIndex = value;
        this.OnPropertyChanged("SelectedItemIndex");
        this.OnPropertyChanged("SelectedItem");
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigTextOverlaysVM" /> class.
    /// </summary>
    /// <param name="textOverlays">
    ///   The <see cref="WallpaperTextOverlay" /> collection instance wrapped by this View Model.
    /// </param>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    public ConfigTextOverlaysVM(ObservableCollection<WallpaperTextOverlay> textOverlays) {
      this.TextOverlays = textOverlays;
      this.selectedItemIndex = -1;
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.SelectedItemIndex.IsBetween(-1, this.TextOverlays.Count - 1));
      Contract.Invariant(this.TextOverlays != null);
      Contract.Invariant(this.AddTextOverlayCommand != null);
      Contract.Invariant(this.RemoveTextOverlayCommand != null);
    }

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
        if (this.addTextOverlayCommand == null)
          this.addTextOverlayCommand = new DelegateCommand(this.AddTextOverlayCommand_Execute, this.AddTextOverlayCommand_CanExecute);

        return this.addTextOverlayCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="AddTextOverlayCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="AddTextOverlayCommand" />
    protected bool AddTextOverlayCommand_CanExecute() {
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
        if (this.removeTextOverlayCommand == null)
          this.removeTextOverlayCommand = new DelegateCommand(this.RemoveTextOverlayCommand_Execute, this.RemoveTextOverlayCommand_CanExecute);

        return this.removeTextOverlayCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveTextOverlayCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveTextOverlayCommand" />
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    protected bool RemoveTextOverlayCommand_CanExecute() {
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
    protected virtual void OnPropertyChanged(string propertyName) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}