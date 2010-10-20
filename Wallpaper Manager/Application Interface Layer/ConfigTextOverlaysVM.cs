// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Common.Presentation;

using WallpaperManager.Data;

namespace WallpaperManager.ApplicationInterface {
  /// <commondoc select='WrappingCollectionViewModels/General/*' params="WrappedType=WallpaperOverlayText" />
  /// <threadsafety static="true" instance="false" />
  public class ConfigTextOverlaysVM: INotifyPropertyChanged {
    #region Property: OverlayTexts
    /// <summary>
    ///   <inheritdoc cref="OverlayTextCollection" select='../value/node()' />
    /// </summary>
    private readonly ObservableCollection<WallpaperOverlayText> overlayTextCollection;

    /// <summary>
    ///   Gets the <see cref="WallpaperOverlayText" /> collection instance wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperOverlayText" /> collection instance wrapped by this View Model.
    /// </value>
    public ObservableCollection<WallpaperOverlayText> OverlayTextCollection {
      get { return this.overlayTextCollection; }
    }
    #endregion

    #region Property: SelectedItemIndex
    /// <summary>
    ///   <inheritdoc cref="SelectedItemIndex" select='../value/node()' />
    /// </summary>
    private Int32 selectedItemIndex;

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="WallpaperOverlayText" /> item.
    /// </summary>
    /// <value>
    ///   The index of the selected <see cref="WallpaperOverlayText" /> item.
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
    ///   Gets the selected <see cref="WallpaperOverlayText" /> item.
    /// </summary>
    /// <value>
    ///   The selected <see cref="WallpaperOverlayText" /> item.
    /// </value>
    public WallpaperOverlayText SelectedItem {
      get {
        if ((this.OverlayTextCollection.Count > 0) && (this.SelectedItemIndex != -1)) {
          return this.OverlayTextCollection[this.SelectedItemIndex];
        }

        return null;
      }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConfigTextOverlaysVM" /> class.
    /// </summary>
    /// <param name="overlayTextCollection">
    ///   The <see cref="WallpaperOverlayText" /> collection instance wrapped by this View Model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="overlayTextCollection" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperOverlayText">WallpaperOverlayText Class</seealso>
    public ConfigTextOverlaysVM(ObservableCollection<WallpaperOverlayText> overlayTextCollection) {
      if (overlayTextCollection == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("overlayTextCollection"));
      }

      this.overlayTextCollection = overlayTextCollection;
    }
    #endregion

    #region Command: AddOverlayText
    /// <summary>
    ///   <inheritdoc cref="AddOverlayTextCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand addOverlayTextCommand;

    /// <summary>
    ///   Gets the Add OverlayText <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Add OverlayText <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="AddOverlayTextCommand_CanExecute">AddOverlayTextCommand_CanExecute Method</seealso>
    /// <seealso cref="AddOverlayTextCommand_Execute">AddOverlayTextCommand_Execute Method</seealso>
    /// <seealso cref="WallpaperOverlayText">WallpaperOverlayText Class</seealso>
    public DelegateCommand AddOverlayTextCommand {
      get {
        if (this.addOverlayTextCommand == null) {
          this.addOverlayTextCommand = new DelegateCommand(
            this.AddOverlayTextCommand_Execute, this.AddOverlayTextCommand_CanExecute
          );
        }
    
        return this.addOverlayTextCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="AddOverlayTextCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="AddOverlayTextCommand" />
    protected Boolean AddOverlayTextCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="AddOverlayTextCommand" /> is executed.
    ///   Adds a new <see cref="WallpaperOverlayText" /> to the wrapped <see cref="OverlayTextCollection" />.
    /// </summary>
    /// <seealso cref="AddOverlayTextCommand" />
    protected void AddOverlayTextCommand_Execute() {
      this.OverlayTextCollection.Add(new WallpaperOverlayText());
      this.SelectedItemIndex = this.OverlayTextCollection.Count - 1;
    }
    #endregion

    #region Command: RemoveOverlayText
    /// <summary>
    ///   <inheritdoc cref="RemoveOverlayTextCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand removeOverlayTextCommand;

    /// <summary>
    ///   Gets the Remove Overlay Text <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Remove Overlay Text <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RemoveOverlayTextCommand_CanExecute">RemoveOverlayTextCommand_CanExecute Method</seealso>
    /// <seealso cref="RemoveOverlayTextCommand_Execute">RemoveOverlayTextCommand_Execute Method</seealso>
    /// <seealso cref="WallpaperOverlayText">WallpaperOverlayText Class</seealso>
    public DelegateCommand RemoveOverlayTextCommand {
      get {
        if (this.removeOverlayTextCommand == null) {
          this.removeOverlayTextCommand = new DelegateCommand(
            this.RemoveOverlayTextCommand_Execute, this.RemoveOverlayTextCommand_CanExecute
          );
        }
    
        return this.removeOverlayTextCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveOverlayTextCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveOverlayTextCommand" />
    /// <seealso cref="WallpaperOverlayText">WallpaperOverlayText Class</seealso>
    protected Boolean RemoveOverlayTextCommand_CanExecute() {
      return (this.SelectedItem != null);
    }

    /// <summary>
    ///   Called when <see cref="RemoveOverlayTextCommand" /> is executed.
    ///   Removes the selected <see cref="WallpaperOverlayText" /> from the wrapped <see cref="OverlayTextCollection" />.
    /// </summary>
    /// <seealso cref="RemoveOverlayTextCommand" />
    /// <seealso cref="WallpaperOverlayText">WallpaperOverlayText Class</seealso>
    protected void RemoveOverlayTextCommand_Execute() {
      this.OverlayTextCollection.RemoveAt(this.SelectedItemIndex);
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
