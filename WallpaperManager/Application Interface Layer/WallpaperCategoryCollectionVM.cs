// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

using Common;
using Common.Presentation;

using WallpaperManager.Data;

namespace WallpaperManager.ApplicationInterface {
  /// <commondoc select='WrappingCollectionViewModels/General/summary' params="WrappedType=WallpaperCategoryVM" />
  /// <remarks>
  ///   <para>
  ///     <commondoc select='WrappingCollectionViewModels/General/remarks/node()' params="WrappedType=WallpaperCategoryVM" />
  ///   </para>
  ///   <para>
  ///     <commondoc select='ObserableCollection/General/remarks/node()' />
  ///   </para>
  /// </remarks>
  /// <seealso cref="WallpaperCategoryVM">WallpaperCategoryVM Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperCategoryCollectionVM: ObservableCollection<WallpaperCategoryVM>, IWeakEventListener {
    #region Fields: handleCategoriesCollectionChanged
    /// <summary>
    ///   Indicaties whether the <see cref="ObservableCollection{T}.CollectionChanged" />
    ///   event of the <see cref="WallpaperCategoryCollection">Wallpaper Category Collection</see>
    ///   will be handled or not.
    /// </summary>
    /// <remarks>
    ///   This field should be set to <c>false</c> before an internal change on the 
    ///   <see cref="WallpaperCategory" /> is done and must be set to <c>true</c> 
    ///   again after.
    /// </remarks>
    private Boolean handleCategoriesCollectionChanged;
    #endregion

    #region Properties: Categories, CategoriesAccessor
    /// <summary>
    ///   <inheritdoc cref="Categories" select='../value/node()' />
    /// </summary>
    private ReadOnlyWallpaperCategoryCollection categories;

    /// <summary>
    ///   Gets a read only version of the <see cref="WallpaperCategory" /> collection wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   A read only version of the <see cref="WallpaperCategory" /> collection wrapped by this View Model.
    /// </value>
    public ReadOnlyWallpaperCategoryCollection Categories {
      get {
        if (this.categories == null) {
          this.categories = new ReadOnlyWallpaperCategoryCollection(this.CategoriesAccessor);
        }

        return this.categories;
      }
    }

    /// <summary>
    ///   <inheritdoc cref="Categories" select='../value/node()' />
    /// </summary>
    private readonly WallpaperCategoryCollection categoriesAccessor;

    /// <summary>
    ///   Gets the <see cref="WallpaperCategory" /> collection wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperCategory" /> collection wrapped by this View Model.
    /// </value>
    protected WallpaperCategoryCollection CategoriesAccessor {
      get { return this.categoriesAccessor; }
    }
    #endregion

    #region Property: SelectedCategoryVMIndex
    /// <summary>
    ///   <inheritdoc cref="SelectedCategoryVMIndex" select='../value/node()' />
    /// </summary>
    private Int32 selectedCategoryVMIndex;

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="WallpaperCategory" />.
    /// </summary>
    /// <value>
    ///   The index of the selected <see cref="WallpaperCategory" />.
    /// </value>
    public Int32 SelectedCategoryVMIndex {
      get { return this.selectedCategoryVMIndex; }
      set {
        this.selectedCategoryVMIndex = value;

        if (value != -1) {
          // Select the first item if not empty.
          if (this[value].Count > 0) {
            this[value].SelectedWallpaperVMs = new ReadOnlyCollection<WallpaperVM>(new List<WallpaperVM>() { this[value][0] } );
          }
        }

        this.OnPropertyChanged("SelectedCategoryVMIndex");
        this.OnPropertyChanged("SelectedCategoryVM");
        this.OnPropertyChanged("CanActivateDeactivateSelected");
        this.OnPropertyChanged("CanRenameCategory");
        this.OnPropertyChanged("CanRemoveSelected");
      }
    }
    #endregion

    #region Property: SelectedCategoryVM
    /// <summary>
    ///   Gets the selected <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <value>
    ///   The selected <see cref="WallpaperCategoryVM" />.
    /// </value>
    public WallpaperCategoryVM SelectedCategoryVM {
      get {
        if ((this.Categories.Count > 0) && (this.SelectedCategoryVMIndex != -1)) {
          return this[this.SelectedCategoryVMIndex];
        }

        return null;
      }
    }
    #endregion

    #region Property: RequestWallpaperCategoryVM
    /// <summary>
    ///   <inheritdoc cref="RequestWallpaperCategoryVM" select='../value/node()' />
    /// </summary>
    private readonly Func<WallpaperCategory,WallpaperCategoryVM> requestWallpaperCategoryVM;

    /// <summary>
    ///   Gets the delegate invoked to request a <see cref="WallpaperCategoryVM" /> for a <see cref="WallpaperCategory" /> 
    ///   instance.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request a <see cref="WallpaperCategoryVM" /> for a <see cref="WallpaperCategory" /> instance.
    /// </value>
    /// <seealso cref="WallpaperCategoryVM">WallpaperCategoryVM Class</seealso>
    /// <seealso cref="WallpaperCategory">WallpaperCategory Class</seealso>
    protected Func<WallpaperCategory,WallpaperCategoryVM> RequestWallpaperCategoryVM {
      get { return this.requestWallpaperCategoryVM; }
    }
    #endregion

    #region Event: UnhandledCommandException
    /// <commondoc select='ViewModels/Events/UnhandledCommandException/*' />
    public event EventHandler<CommandExceptionEventArgs> UnhandledCommandException;

    /// <commondoc select='ViewModels/Methods/OnUnhandledCommandException/*' />
    protected virtual void OnUnhandledCommandException(CommandExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (this.UnhandledCommandException != null) {
        this.UnhandledCommandException.ReverseInvoke(this, e);
      }

      if (!e.IsHandled) {
        throw e.Exception;
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperCategoryCollectionVM" /> class.
    /// </summary>
    /// <param name="categories">
    ///   The <see cref="WallpaperCategoryCollection" /> which should be wrapped by this View Model.
    /// </param>
    /// <param name="requestWallpaperCategoryVM">
    ///   The delegate invoked to request a <see cref="WallpaperCategoryVM" /> for a <see cref="WallpaperCategory" /> instance.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="categories" /> or <paramref name="requestWallpaperCategoryVM" /> is <c>null</c>.
    /// </exception>
    /// <commondoc select='ViewModels/General/seealso' />
    public WallpaperCategoryCollectionVM(
      WallpaperCategoryCollection categories, Func<WallpaperCategory,WallpaperCategoryVM> requestWallpaperCategoryVM
    ) {
      if (categories == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("categories"));
      }
      if (requestWallpaperCategoryVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestWallpaperCategoryVM"));
      }

      this.categoriesAccessor = categories;
      this.requestWallpaperCategoryVM = requestWallpaperCategoryVM;

      CollectionChangedEventManager.AddListener(categories, this);

      // Simulate add of all wallpapers of the category so that a WallpaperCategoryVM is created for all WallpaperCategory 
      // instances.
      this.handleCategoriesCollectionChanged = true;
      this.Categories_CollectionChanged(
        this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, categories)
      );
    }
    #endregion

    #region Command: AddCategory
    /// <summary>
    ///   <inheritdoc cref="AddCategoryCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand<WallpaperCategory> addCategoryCommand;

    /// <summary>
    ///   Gets the Add Category <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Add Category <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="AddCategoryCommand_CanExecute">AddCategoryCommand_CanExecute Method</seealso>
    /// <seealso cref="AddCategoryCommand_Execute">AddCategoryCommand_Execute Method</seealso>
    public DelegateCommand<WallpaperCategory> AddCategoryCommand {
      get {
        if (this.addCategoryCommand == null) {
          this.addCategoryCommand = new DelegateCommand<WallpaperCategory>(
            this.AddCategoryCommand_Execute, this.AddCategoryCommand_CanExecute
          );
        }

        return this.addCategoryCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="AddCategoryCommand" /> can be executed.
    /// </summary>
    /// <param name="wallpaperCategory">
    ///   The <see cref="WallpaperCategory" /> object to add.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="AddCategoryCommand" />
    protected Boolean AddCategoryCommand_CanExecute(WallpaperCategory wallpaperCategory) {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="AddCategoryCommand" /> is executed and adds the given <see cref="WallpaperCategory" /> object.
    /// </summary>
    /// <param name="wallpaperCategory">
    ///   The <see cref="WallpaperCategory" /> object to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpaperCategory" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="AddCategoryCommand" />
    protected void AddCategoryCommand_Execute(WallpaperCategory wallpaperCategory) {
      if (wallpaperCategory == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperCategory"));
      }

      try {
        this.Add(this.RequestWallpaperCategoryVM(wallpaperCategory));
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.AddCategoryCommand, exception));
      }
    }
    #endregion

    #region Command: ActivateDeactivateSelected
    /// <summary>
    ///   <inheritdoc cref="ActivateDeactivateSelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand activateDeactivateSelectedCommand;

    /// <summary>
    ///   Gets the Activate Deactivate Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Activate Deactivate Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ActivateDeactivateSelectedCommand_CanExecute">ActivateDeactivateSelectedCommand_CanExecute Method</seealso>
    /// <seealso cref="ActivateDeactivateSelectedCommand_Execute">ActivateDeactivateSelectedCommand_Execute Method</seealso>
    public DelegateCommand ActivateDeactivateSelectedCommand {
      get {
        if (this.activateDeactivateSelectedCommand == null) {
          this.activateDeactivateSelectedCommand = new DelegateCommand(
            this.ActivateDeactivateSelectedCommand_Execute, this.ActivateDeactivateSelectedCommand_CanExecute
          );
        }
    
        return this.activateDeactivateSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ActivateDeactivateSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ActivateDeactivateSelectedCommand" />
    protected Boolean ActivateDeactivateSelectedCommand_CanExecute() {
      return (this.SelectedCategoryVM != null);
    }

    /// <summary>
    ///   Called when <see cref="ActivateDeactivateSelectedCommand" /> is executed.
    ///   Toggles the <see cref="WallpaperSettingsBase.IsActivated" /> state of the <see cref="SelectedCategoryVM" />.
    /// </summary>
    /// <seealso cref="ActivateDeactivateSelectedCommand" />
    protected void ActivateDeactivateSelectedCommand_Execute() {
      try {
        if (this.SelectedCategoryVM.Category.IsActivated == null) {
          this.SelectedCategoryVM.Category.IsActivated = false;
        } else {
          this.SelectedCategoryVM.Category.IsActivated = !(this.SelectedCategoryVM.Category.IsActivated);
        }
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.AddCategoryCommand, exception));
      }
    }
    #endregion

    #region Command: RenameSelected
    /// <summary>
    ///   <inheritdoc cref="RenameSelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand<String> renameSelectedCommand;

    /// <summary>
    ///   Gets the Rename Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Rename Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RenameSelectedCommand_CanExecute">RenameSelectedCommand_CanExecute Method</seealso>
    /// <seealso cref="RenameSelectedCommand_Execute">RenameSelectedCommand_Execute Method</seealso>
    public DelegateCommand<String> RenameSelectedCommand {
      get {
        if (this.renameSelectedCommand == null) {
          this.renameSelectedCommand = new DelegateCommand<String>(
            this.RenameSelectedCommand_Execute, this.RenameSelectedCommand_CanExecute
          );
        }
    
        return this.renameSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RenameSelectedCommand" /> can be executed.
    /// </summary>
    /// <param name="newName">
    ///   The new name of the selected <see cref="WallpaperCategoryVM" />.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RenameSelectedCommand" />
    protected Boolean RenameSelectedCommand_CanExecute(String newName) {
      return (this.SelectedCategoryVM != null);
    }

    /// <summary>
    ///   Called when <see cref="RenameSelectedCommand" /> is executed.
    ///   Renames the selected <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <param name="newName">
    ///   The new name of the selected <see cref="WallpaperCategoryVM" />.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The string parameter contains invalid characters for a <see cref="WallpaperCategory" /> name.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The string parameter is a <c>null</c> string.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The string parameter is of an invalid length (min. <see cref="WallpaperCategory.Name_MinLength" />, max. 
    ///   <see cref="WallpaperCategory.Name_MaxLength" /> characters).
    /// </exception>
    /// <seealso cref="RenameSelectedCommand" />
    protected void RenameSelectedCommand_Execute(String newName) {
      try {
        this.SelectedCategoryVM.Category.Name = newName;
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.AddCategoryCommand, exception));
      }
    }
    #endregion

    #region Command: RemoveSelected
    /// <summary>
    ///   <inheritdoc cref="RemoveSelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand removeSelectedCommand;

    /// <summary>
    ///   Gets the Remove Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Remove Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RemoveSelectedCommand_CanExecute">RemoveSelectedCommand_CanExecute Method</seealso>
    /// <seealso cref="RemoveSelectedCommand_Execute">RemoveSelectedCommand_Execute Method</seealso>
    public DelegateCommand RemoveSelectedCommand {
      get {
        if (this.removeSelectedCommand == null) {
          this.removeSelectedCommand = new DelegateCommand(
            this.RemoveSelectedCommand_Execute, this.RemoveSelectedCommand_CanExecute
          );
        }
    
        return this.removeSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveSelectedCommand" />
    protected Boolean RemoveSelectedCommand_CanExecute() {
      return (this.SelectedCategoryVM != null);
    }

    /// <summary>
    ///   Called when <see cref="RemoveSelectedCommand" /> is executed and removes the selected 
    ///   <see cref="WallpaperCategoryVM" />.
    /// </summary>
    /// <seealso cref="RemoveSelectedCommand" />
    protected void RemoveSelectedCommand_Execute() {
      try {
        this.RemoveAt(this.SelectedCategoryVMIndex);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.AddCategoryCommand, exception));
      }
    }
    #endregion

    #region Methods: InsertItem, SetItem, RemoveItem, ClearItems
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="item" /> is <c>null</c>.
    /// </exception>
    protected override void InsertItem(Int32 index, WallpaperCategoryVM item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoriesCollectionChanged = false;
      try {
        this.CategoriesAccessor.Add(item.Category);
      } finally {
        this.handleCategoriesCollectionChanged = true;
      }

      base.InsertItem(index, item);

      this.SelectedCategoryVMIndex = index;
    }

    /// <inheritdoc />
    protected override void SetItem(Int32 index, WallpaperCategoryVM item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoriesCollectionChanged = false;
      try {
        this.CategoriesAccessor[index] = item.Category;
      } finally {
        this.handleCategoriesCollectionChanged = true;
      }

      base.SetItem(index, item);
    }

    /// <inheritdoc />
    protected override void RemoveItem(Int32 index) {
      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoriesCollectionChanged = false;
      try {
        this.CategoriesAccessor.RemoveAt(index);
      } finally {
        this.handleCategoriesCollectionChanged = true;
      }

      base.RemoveItem(index);

      if (index < this.Categories.Count) {
        this.SelectedCategoryVMIndex = index;
      } else {
        // Also sets -1 if there are no more categories
        this.SelectedCategoryVMIndex = this.Categories.Count - 1;
      }
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoriesCollectionChanged = false;
      try {
        this.CategoriesAccessor.Clear();
      } finally {
        this.handleCategoriesCollectionChanged = true;
      }

      base.ClearItems();
    }
    #endregion

    #region Methods: Categories_CollectionChanged, OnPropertyChanged
    /// <summary>
    ///   Handles the <see cref="ObservableCollection{T}.CollectionChanged" /> event of the wrapped 
    ///   <see cref="Categories">Wallpaper Category Collection instance</see>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The <see cref="Delegate" /> represented by the 
    ///   <see cref="RequestWallpaperCategoryVM">RequestWallpaperCategoryVM Property</see> returned <c>null</c> or a
    ///   <see cref="WallpaperCategoryVM" /> which does not wrap the requested <see cref="WallpaperCategory" />.
    /// </exception>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    private void Categories_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      // No need to observe our own internal changes on the Category Collection.
      if (!this.handleCategoriesCollectionChanged) {
        return;
      }

      if (
        (e.Action == NotifyCollectionChangedAction.Add) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)
      ) {
        // Loop through added items.
        for (Int32 i = 0; i < e.NewItems.Count; i++) {
          WallpaperCategoryVM requestedVM = this.RequestWallpaperCategoryVM((WallpaperCategory)e.NewItems[i]);
          if (requestedVM == null) {
            throw new InvalidOperationException(ExceptionMessages.GetRequestReturnedNull("RequestWallpaperCategoryVM"));
          }
          if (requestedVM.Category != e.NewItems[i]) {
            throw new InvalidOperationException(ExceptionMessages.GetVMRequestReturnedWrongWrapper(
              "RequestWallpaperCategoryVM", typeof(WallpaperCategoryVM), typeof(WallpaperCategory)
            ));
          }

          base.InsertItem(this.Count, requestedVM);
        }
      }

      if (
        (e.Action == NotifyCollectionChangedAction.Remove) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)
      ) {
        // Loop through removed items.
        foreach (WallpaperCategory category in e.OldItems) {
          for (Int32 i = 0; i < this.Count; i++) {
            if (this[i].Category == category) {
              base.RemoveItem(i);

              break;
            }
          }
        }
      }
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(CollectionChangedEventManager)) {
        this.Categories_CollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion
  }
}
 