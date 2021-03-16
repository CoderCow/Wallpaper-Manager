// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using Common;
using Common.Presentation;
using WallpaperManager.Models;
using Path = Common.IO.Path;

namespace WallpaperManager.ViewModels {
  /// <commondoc select='WrappingCollectionViewModels/General/summary' params="WrappedType=WallpaperVM" />
  /// <remarks>
  ///   <para>
  ///     <commondoc select='WrappingCollectionViewModels/General/remarks/node()' params="WrappedType=WallpaperVM" />
  ///   </para>
  ///   <para>
  ///     <commondoc select='ObserableCollection/General/remarks/node()' />
  ///   </para>
  /// </remarks>
  /// <seealso cref="WallpaperVM">WallpaperVM Class</seealso>
  /// <seealso cref="WallpaperCategoryCollectionVM">WallpaperCategoryCollectionVM Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperCategoryVM : ObservableCollection<WallpaperVM>, IWeakEventListener {
    /// <summary>
    ///   Indicaties whether the <see cref="ObservableCollection{T}.CollectionChanged" />
    ///   event of the <see cref="Category">Wallpaper Category</see> will be or not.
    /// </summary>
    /// <remarks>
    ///   This field should be set to <c>false</c> before an internal change on the
    ///   <see cref="WallpaperCategory" /> is done and must be set to <c>true</c>
    ///   again after.
    /// </remarks>
    private bool handleCategoryCollectionChanged;

    /// <summary>
    ///   <inheritdoc cref="SelectedWallpaperVMs" select='../value/node()' />
    /// </summary>
    private IList<WallpaperVM> selectedWallpaperVMs;

    /// <summary>
    ///   Gets a value indicating whether this category is a <see cref="SynchronizedWallpaperCategory" /> or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this category is a <see cref="SynchronizedWallpaperCategory" />;
    ///   otherwise <c>false</c>.
    /// </value>
    public bool IsSynchronizedCategory {
      get { return (this.Category is SynchronizedWallpaperCategory); }
    }

    /// <summary>
    ///   Gets the <see cref="ViewModels.WallpaperChangerVM" /> instance used to control cycle
    ///   operations performed by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="ViewModels.WallpaperChangerVM" /> instance used to control cycle operations
    ///   performed by this View Model.
    /// </value>
    /// <seealso cref="WallpaperChangerVM">WallpaperChangerVM Class</seealso>
    public WallpaperChangerVM WallpaperChangerVM { get; }

    /// <summary>
    ///   Gets the wrapped category.
    /// </summary>
    /// <value>
    ///   The wrapped category.
    /// </value>
    public WallpaperCategory Category { get; }

    /// <summary>
    ///   Gets or sets the collection of selected <see cref="WallpaperVM">Wallpaper View Models</see>.
    /// </summary>
    /// <value>
    ///   The collection of selected <see cref="WallpaperVM">Wallpaper View Models</see>.
    /// </value>
    public IList<WallpaperVM> SelectedWallpaperVMs {
      get { return this.selectedWallpaperVMs; }
      set {
        this.selectedWallpaperVMs = value;
        this.OnPropertyChanged("SelectedWallpaperVMs");
      }
    }

    /// <summary>
    ///   Gets the delegate invoked to request the configuration of the selected <see cref="WallpaperVM" /> instances.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the configuration of the selected <see cref="WallpaperVM" /> instances.
    /// </value>
    protected Action<WallpaperCategoryVM> RequestConfigureSelected { get; }

    /// <summary>
    ///   Gets the delegate invoked to request the configuration of the <see cref="WallpaperDefaultSettings" />.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the configuration of the <see cref="WallpaperDefaultSettings" />.
    /// </value>
    protected Action<WallpaperCategoryVM> RequestConfigureDefaultSettings { get; }

    /// <commondoc select='ViewModels/Events/UnhandledCommandException/*' />
    public event EventHandler<CommandExceptionEventArgs> UnhandledCommandException;

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperCategoryVM" /> class.
    /// </summary>
    /// <param name="category">
    ///   The <see cref="WallpaperManager.Models.Wallpaper" /> which should be wrapped by this View Model.
    /// </param>
    /// <param name="wallpaperChangerVM">
    ///   The <see cref="ViewModels.WallpaperChangerVM" /> instance used to control cycle operations
    ///   performed by this View Model.
    /// </param>
    /// <param name="requestConfigureSelected">
    ///   The delegate invoked to request the configuration of the selected <see cref="WallpaperVM" /> instances.
    /// </param>
    /// <param name="requestConfigureDefaultSettings">
    ///   The delegate invoked to request the configuration of the <see cref="WallpaperDefaultSettings" />.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="category" /> or <paramref name="wallpaperChangerVM" /> or
    ///   <paramref name="requestConfigureSelected" /> or <paramref name="requestConfigureDefaultSettings" /> is <c>null</c>.
    /// </exception>
    /// <commondoc select='ViewModels/General/seealso' />
    public WallpaperCategoryVM(
      WallpaperCategory category,
      WallpaperChangerVM wallpaperChangerVM,
      Action<WallpaperCategoryVM> requestConfigureSelected,
      Action<WallpaperCategoryVM> requestConfigureDefaultSettings) {

      this.Category = category;
      this.WallpaperChangerVM = wallpaperChangerVM;
      this.RequestConfigureSelected = requestConfigureSelected;
      this.RequestConfigureDefaultSettings = requestConfigureDefaultSettings;

      CollectionChangedEventManager.AddListener(category, this);

      // Simulate adding of all wallpapers to the category so that a WallpaperVM is created for any Wallpaper instance.
      this.handleCategoryCollectionChanged = true;
      this.Category_CollectionChanged(
        this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, category));

      this.selectedWallpaperVMs = new ReadOnlyCollection<WallpaperVM>(new List<WallpaperVM>());

      PropertyChangedEventManager.AddListener(wallpaperChangerVM, this, string.Empty);
      this.WallpaperChangerVM_PropertyChanged(this, new PropertyChangedEventArgs("ActiveWallpapers"));
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        this.WallpaperChangerVM_PropertyChanged(sender, (PropertyChangedEventArgs)e);
        return true;
      }

      if (managerType == typeof(CollectionChangedEventManager)) {
        this.Category_CollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.WallpaperChangerVM != null);
      Contract.Invariant(this.Category != null);
      Contract.Invariant(this.SelectedWallpaperVMs != null);
      Contract.Invariant(!this.SelectedWallpaperVMs.Contains(null));
      Contract.Invariant(this.RequestConfigureSelected != null);
      Contract.Invariant(this.RequestConfigureDefaultSettings != null);
      Contract.Invariant(this.ConfigureDefaultSettingsCommand != null);
      Contract.Invariant(this.ActivateDeactivateSelectedCommand != null);
      Contract.Invariant(this.OpenFolderOfSelectedCommand != null);
      Contract.Invariant(this.ApplySelectedCommand != null);
      Contract.Invariant(this.ConfigureSelectedCommand != null);
      Contract.Invariant(this.RemoveSelectedCommand != null);
      Contract.Invariant(this.RemoveSelectedPhysicallyCommand != null);
    }

    /// <summary>
    ///   Copies an image file the selected folder of the selected
    ///   <see cref="SynchronizedWallpaperCategory" />.
    /// </summary>
    /// <param name="imagePath">
    ///   <c>true</c> to overwrite an already existing image file; otherwise <c>false</c>.
    /// </param>
    /// <param name="overwriteOnExist">
    ///   A <see cref="bool" /> indicating whether the image file should be overwritten if it exists already.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="imagePath" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    ///   <paramref name="overwriteOnExist" /> is <c>false</c> and the file does already exist or
    ///   an I/O error has occurred.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The wrapped <see cref="WallpaperCategory" /> is no <see cref="SynchronizedWallpaperCategory" />.
    /// </exception>
    /// <inheritdoc cref="System.IO.File.Copy(string, string)" />
    public virtual void AddSynchronizedImage(Path imagePath, bool overwriteOnExist) {
      if (imagePath == Path.None) throw new ArgumentException();
      if (!this.IsSynchronizedCategory) throw new InvalidOperationException();

      SynchronizedWallpaperCategory synchronizedCategory = (this.Category as SynchronizedWallpaperCategory);
      if (synchronizedCategory != null)
        File.Copy(imagePath, Path.Concat(synchronizedCategory.SynchronizedDirectoryPath, imagePath.FileName), overwriteOnExist);
    }

    /// <commondoc select='ViewModels/Methods/OnUnhandledCommandException/*' />
    protected virtual void OnUnhandledCommandException(CommandExceptionEventArgs e) {
      if (e == null) throw new ArgumentNullException();

      this.UnhandledCommandException?.ReverseInvoke(this, e);
    }

    /// <summary>
    ///   Checks if more than zero wallpapers are selected and if the selection contains no <c>null</c> values and throws an
    ///   exception otherwise.
    /// </summary>
    protected void CheckProperWallpaperSelection() {
      if (this.SelectedWallpaperVMs.Count == 0)
        throw new InvalidOperationException("No wallpaper selected.");
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, WallpaperVM item) {
      // TODO: Throwing this exception is not allowed here.
      if (item == null) throw new ArgumentNullException();

      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoryCollectionChanged = false;
      try {
        this.Category.Add(item.Wallpaper);
      } finally {
        this.handleCategoryCollectionChanged = true;
      }

      base.InsertItem(index, item);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, WallpaperVM item) {
      // TODO: Throwing this exception is not allowed here.
      if (item == null) throw new ArgumentNullException();

      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoryCollectionChanged = false;
      try {
        this.Category[index] = item.Wallpaper;
      } finally {
        this.handleCategoryCollectionChanged = true;
      }

      base.SetItem(index, item);
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index) {
      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoryCollectionChanged = false;
      try {
        this.Category.RemoveAt(index);
      } finally {
        this.handleCategoryCollectionChanged = true;
      }

      base.RemoveItem(index);
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      // If we change the Category internally we don't have to observe our own changes.
      this.handleCategoryCollectionChanged = false;
      try {
        this.Category.Clear();
      } finally {
        this.handleCategoryCollectionChanged = true;
      }

      base.ClearItems();
    }

    /// <summary>
    ///   Handles the <see cref="ObservableCollection{T}.CollectionChanged" /> event of the wrapped
    ///   <see cref="Category">Wallpaper Category instance</see>.
    /// </summary>
    /// <param name="sender">
    ///   The source of the event.
    /// </param>
    /// <param name="e">
    ///   The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.
    /// </param>
    private void Category_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      // No need to observe our own internal changes on the Category.
      if (!this.handleCategoryCollectionChanged)
        return;

      if (
        (e.Action == NotifyCollectionChangedAction.Add) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)) {
        // Loop through added items.
        for (int i = 0; i < e.NewItems.Count; i++)
          base.InsertItem(this.Count, new WallpaperVM((Wallpaper)e.NewItems[i]));
      }

      if (
        (e.Action == NotifyCollectionChangedAction.Remove) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)) {
        // Loop through removed items.
        foreach (Wallpaper wallpaper in e.OldItems) {
          for (int i = 0; i < this.Count; i++) {
            if (this[i].Wallpaper == wallpaper) {
              base.RemoveItem(i);

              break;
            }
          }
        }
      }
    }

    /// <summary>
    ///   Handles the <see cref="ViewModels.WallpaperChangerVM.PropertyChanged" /> event of a
    ///   <see cref="WallpaperChangerVM" /> instance and updates the <see cref="WallpaperVM.IsApplied">IsApplied Property</see>
    ///   of the child <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    private void WallpaperChangerVM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "ActiveWallpapers") {
        foreach (WallpaperVM wallpaperVM in this)
          wallpaperVM.IsApplied = (this.WallpaperChangerVM.ActiveWallpapers.Contains(wallpaperVM.Wallpaper));
      }
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    #region Command: ConfigureDefaultSettings
    /// <summary>
    ///   <inheritdoc cref="ConfigureDefaultSettingsCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand configureDefaultSettingsCommand;

    /// <summary>
    ///   Gets the Configure Default Settings <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Configure Default Settings <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ConfigureDefaultSettingsCommand_CanExecute">ConfigureDefaultSettingsCommand_CanExecute Method</seealso>
    /// <seealso cref="ConfigureDefaultSettingsCommand_Execute">ConfigureDefaultSettingsCommand_Execute Method</seealso>
    public DelegateCommand ConfigureDefaultSettingsCommand {
      get {
        if (this.configureDefaultSettingsCommand == null)
          this.configureDefaultSettingsCommand = new DelegateCommand(this.ConfigureDefaultSettingsCommand_Execute, this.ConfigureDefaultSettingsCommand_CanExecute);

        return this.configureDefaultSettingsCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ConfigureDefaultSettingsCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ConfigureDefaultSettingsCommand" />
    protected bool ConfigureDefaultSettingsCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ConfigureDefaultSettingsCommand" /> is executed and requests the
    ///   <see cref="WallpaperDefaultSettings" /> of the wrapped <see cref="WallpaperCategory" /> instance to be configured.
    /// </summary>
    /// <seealso cref="ConfigureDefaultSettingsCommand" />
    protected void ConfigureDefaultSettingsCommand_Execute() {
      try {
        this.RequestConfigureDefaultSettings(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ConfigureDefaultSettingsCommand, exception));
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
            this.ActivateDeactivateSelectedCommand_Execute, this.ActivateDeactivateSelectedCommand_CanExecute);
        }

        return this.activateDeactivateSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ActivateDeactivateSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ActivateDeactivateSelectedCommand" />
    protected bool ActivateDeactivateSelectedCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="ActivateDeactivateSelectedCommand" /> is executed.
    ///   Toggles the <see cref="WallpaperSettingsBase.IsActivated" /> state of all <see cref="SelectedWallpaperVMs" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty or contains a <c>null</c> item.
    /// </exception>
    /// <seealso cref="ActivateDeactivateSelectedCommand" />
    protected void ActivateDeactivateSelectedCommand_Execute() {
      this.CheckProperWallpaperSelection();

      bool isActivated = true;
      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null) {
          if (!wallpaperVM.Wallpaper.IsActivated) {
            isActivated = false;
            break;
          }
        }
      }

      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null)
          wallpaperVM.Wallpaper.IsActivated = (!isActivated);
      }
    }
    #endregion

    #region Command: OpenFolderOfSelected
    /// <summary>
    ///   <inheritdoc cref="OpenFolderOfSelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand openFolderOfSelectedCommand;

    /// <summary>
    ///   Gets the Open Folder of Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Open Folder of Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="OpenFolderOfSelectedCommand_CanExecute">OpenFolderOfSelectedCommand_CanExecute Method</seealso>
    /// <seealso cref="OpenFolderOfSelectedCommand_Execute">OpenFolderOfSelectedCommand_Execute Method</seealso>
    public DelegateCommand OpenFolderOfSelectedCommand {
      get {
        if (this.openFolderOfSelectedCommand == null) {
          this.openFolderOfSelectedCommand = new DelegateCommand(
            this.OpenFolderOfSelectedCommand_Execute, this.OpenFolderOfSelectedCommand_CanExecute);
        }

        return this.openFolderOfSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="OpenFolderOfSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="OpenFolderOfSelectedCommand" />
    protected bool OpenFolderOfSelectedCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="OpenFolderOfSelectedCommand" /> is executed.
    ///   Opens the containing folder of the <see cref="SelectedWallpaperVMs" /> in Windows Explorer.
    /// </summary>
    /// <permission cref="SecurityAction.LinkDemand">
    ///   for full trust for the immediate caller. This member cannot be used by partially trusted code.
    /// </permission>
    /// <seealso cref="OpenFolderOfSelectedCommand" />
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    protected void OpenFolderOfSelectedCommand_Execute() {
      try {
        foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
          if (wallpaperVM != null) {
            using (Process explorer = new Process()) {
              explorer.StartInfo.FileName = "explorer.exe";
              explorer.StartInfo.Arguments = string.Concat(@"/e,/select,", wallpaperVM.Wallpaper.ImagePath);
              explorer.Start();
            }
          }
        }
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.OpenFolderOfSelectedCommand, exception));
      }
    }
    #endregion

    #region Command: ApplySelected
    /// <summary>
    ///   <inheritdoc cref="ApplySelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand applySelectedCommand;

    /// <summary>
    ///   Gets the Apply Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Apply Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ApplySelectedCommand_CanExecute">ApplySelectedCommandCommand_CanExecute Method</seealso>
    /// <seealso cref="ApplySelectedCommand_Execute">ApplySelectedCommand_Execute Method</seealso>
    public DelegateCommand ApplySelectedCommand {
      get {
        if (this.applySelectedCommand == null)
          this.applySelectedCommand = new DelegateCommand(this.ApplySelectedCommand_Execute, this.ApplySelectedCommand_CanExecute);

        return this.applySelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ApplySelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty.
    /// </exception>
    /// <seealso cref="ApplySelectedCommand" />
    protected bool ApplySelectedCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="ApplySelectedCommand" /> is executed.
    ///   Applies the <see cref="SelectedWallpaperVMs" /> as wallpaper on the Windows Desktop.
    /// </summary>
    /// <seealso cref="ApplySelectedCommand" />
    protected void ApplySelectedCommand_Execute() {
      if (this.SelectedWallpaperVMs.Count == 0)
        return;

      var wallpapersToUse = new List<Wallpaper>(this.SelectedWallpaperVMs.Count);
      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null)
          wallpapersToUse.Add(wallpaperVM.Wallpaper);
      }

      this.WallpaperChangerVM.CycleNextCommand.Execute(wallpapersToUse);
    }
    #endregion

    #region Command: ConfigureSelected
    /// <summary>
    ///   <inheritdoc cref="ConfigureSelectedCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand configureSelectedCommand;

    /// <summary>
    ///   Gets the Configure Selected <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Configure Selected <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="ConfigureSelectedCommand_CanExecute">ConfigureSelectedCommand_CanExecute Method</seealso>
    /// <seealso cref="ConfigureSelectedCommand_Execute">ConfigureSelectedCommand_Execute Method</seealso>
    public DelegateCommand ConfigureSelectedCommand {
      get {
        if (this.configureSelectedCommand == null)
          this.configureSelectedCommand = new DelegateCommand(this.ConfigureSelectedCommand_Execute, this.ConfigureSelectedCommand_CanExecute);

        return this.configureSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ConfigureSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ConfigureSelectedCommand" />
    protected bool ConfigureSelectedCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ConfigureSelectedCommand" /> is executed and requests the selected <see cref="WallpaperVM" />
    ///   instances to be configured.
    /// </summary>
    /// <seealso cref="ConfigureSelectedCommand" />
    protected void ConfigureSelectedCommand_Execute() {
      try {
        this.RequestConfigureSelected(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ConfigureSelectedCommand, exception));
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
            this.RemoveSelectedCommand_Execute, this.RemoveSelectedCommand_CanExecute);
        }

        return this.removeSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveSelectedCommand" />
    protected bool RemoveSelectedCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0 && !this.IsSynchronizedCategory);
    }

    /// <summary>
    ///   Called when <see cref="RemoveSelectedCommand" /> is executed.
    ///   Removes the selected <see cref="SelectedWallpaperVMs" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty.
    /// </exception>
    /// <seealso cref="RemoveSelectedCommand" />
    protected void RemoveSelectedCommand_Execute() {
      this.CheckProperWallpaperSelection();

      for (int i = 0; i < this.SelectedWallpaperVMs.Count; i++) {
        if (this.SelectedWallpaperVMs[i] != null) {
          this.Remove(this.SelectedWallpaperVMs[i]);
          i--;
        }
      }
    }
    #endregion

    #region Command: RemoveSelectedPhysically
    /// <summary>
    ///   <inheritdoc cref="RemoveSelectedPhysicallyCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand removeSelectedPhysicallyCommand;

    /// <summary>
    ///   Gets the Remove Selected Physically <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Remove Selected Physically <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="RemoveSelectedPhysicallyCommand_CanExecute">RemoveSelectedPhysicallyCommand_CanExecute Method</seealso>
    /// <seealso cref="RemoveSelectedPhysicallyCommand_Execute">RemoveSelectedPhysicallyCommand_Execute Method</seealso>
    public DelegateCommand RemoveSelectedPhysicallyCommand {
      get {
        if (this.removeSelectedPhysicallyCommand == null) {
          this.removeSelectedPhysicallyCommand = new DelegateCommand(
            this.RemoveSelectedPhysicallyCommand_Execute, this.RemoveSelectedPhysicallyCommand_CanExecute);
        }

        return this.removeSelectedPhysicallyCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveSelectedPhysicallyCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveSelectedPhysicallyCommand" />
    protected bool RemoveSelectedPhysicallyCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="RemoveSelectedPhysicallyCommand" /> is executed.
    ///   Removes the selected <see cref="SelectedWallpaperVMs" /> from the hard disk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing to a non
    ///   existing file.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing a file which
    ///   is currently is in use.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing a file which
    ///   is write protected or the user does not have the required rights to access it.
    /// </exception>
    /// <seealso cref="RemoveSelectedPhysicallyCommand" />
    protected void RemoveSelectedPhysicallyCommand_Execute() {
      this.CheckProperWallpaperSelection();

      var selectedVMs = new List<WallpaperVM>(this.SelectedWallpaperVMs);
      for (int i = 0; i < selectedVMs.Count; i++) {
        if (selectedVMs[i] != null) {
          if (!File.Exists(selectedVMs[i].Wallpaper.ImagePath))
            throw new FileNotFoundException(selectedVMs[i].Wallpaper.ImagePath);

          try {
            File.Delete(selectedVMs[i].Wallpaper.ImagePath);
          } catch (DirectoryNotFoundException exception) {
            throw new FileNotFoundException(selectedVMs[i].Wallpaper.ImagePath, exception);
          }

          if (!this.IsSynchronizedCategory)
            this.Remove(selectedVMs[i]);
        }
      }
    }
    #endregion
  }
}