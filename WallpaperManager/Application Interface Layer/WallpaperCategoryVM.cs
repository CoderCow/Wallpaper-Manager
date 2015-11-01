// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Windows;

using Common;
using Common.Presentation;
using Path = Common.IO.Path;

using WallpaperManager.Data;

namespace WallpaperManager.ApplicationInterface {
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
  public class WallpaperCategoryVM: ObservableCollection<WallpaperVM>, IWeakEventListener {
    #region Fields: handleCategoryCollectionChanged
    /// <summary>
    ///   Indicaties whether the <see cref="ObservableCollection{T}.CollectionChanged" />
    ///   event of the <see cref="Category">Wallpaper Category</see> will be or not.
    /// </summary>
    /// <remarks>
    ///   This field should be set to <c>false</c> before an internal change on the 
    ///   <see cref="WallpaperCategory" /> is done and must be set to <c>true</c> 
    ///   again after.
    /// </remarks>
    private Boolean handleCategoryCollectionChanged;
    #endregion

    #region Property: IsSynchronizedCategory
    /// <summary>
    ///   Gets a value indicating whether this category is a <see cref="SynchronizedWallpaperCategory" /> or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this category is a <see cref="SynchronizedWallpaperCategory" />; 
    ///   otherwise <c>false</c>.
    /// </value>
    public Boolean IsSynchronizedCategory {
      get { return (this.Category is SynchronizedWallpaperCategory); }
    }
    #endregion

    #region Property: WallpaperChangerVM
    /// <summary>
    ///   <inheritdoc cref="WallpaperChangerVM" select='../value/node()' />
    /// </summary>
    private readonly WallpaperChangerVM wallpaperChangerVM;

    /// <summary>
    ///   Gets the <see cref="WallpaperManager.ApplicationInterface.WallpaperChangerVM" /> instance used to control cycle 
    ///   operations performed by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperManager.ApplicationInterface.WallpaperChangerVM" /> instance used to control cycle operations 
    ///   performed by this View Model.
    /// </value>
    /// <seealso cref="WallpaperChangerVM">WallpaperChangerVM Class</seealso>
    public WallpaperChangerVM WallpaperChangerVM {
      get { return this.wallpaperChangerVM; }
    }
    #endregion

    #region Property: Category
    /// <summary>
    ///   <inheritdoc cref="Category" select='../value/node()' />
    /// </summary>
    private readonly WallpaperCategory category;

    /// <summary>
    ///   Gets the wrapped category.
    /// </summary>
    /// <value>
    ///   The wrapped category.
    /// </value>
    public WallpaperCategory Category {
      get { return this.category; }
    }
    #endregion

    #region Property: SelectedWallpaperVMs
    /// <summary>
    ///   <inheritdoc cref="SelectedWallpaperVMs" select='../value/node()' />
    /// </summary>
    private IList<WallpaperVM> selectedWallpaperVMs;

    /// <summary>
    ///   Gets or sets the collection of selected <see cref="WallpaperVM">Wallpaper View Models</see>.
    /// </summary>
    /// <value>
    ///   The collection of selected <see cref="WallpaperVM">Wallpaper View Models</see>.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    public IList<WallpaperVM> SelectedWallpaperVMs {
      get { return this.selectedWallpaperVMs; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        this.selectedWallpaperVMs = value;
        this.OnPropertyChanged("SelectedWallpaperVMs");
      }
    }
    #endregion

    #region Property: RequestConfigureSelected
    /// <summary>
    ///   <inheritdoc cref="RequestConfigureSelected" select='../value/node()' />
    /// </summary>
    private readonly Action<WallpaperCategoryVM> requestConfigureSelected;

    /// <summary>
    ///   Gets the delegate invoked to request the configuration of the selected <see cref="WallpaperVM" /> instances.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the configuration of the selected <see cref="WallpaperVM" /> instances.
    /// </value>
    protected Action<WallpaperCategoryVM> RequestConfigureSelected {
      get { return this.requestConfigureSelected; }
    }
    #endregion

    #region Property: RequestConfigureDefaultSettings
    /// <summary>
    ///   <inheritdoc cref="RequestConfigureDefaultSettings" select='../value/node()' />
    /// </summary>
    private readonly Action<WallpaperCategoryVM> requestConfigureDefaultSettings;

    /// <summary>
    ///   Gets the delegate invoked to request the configuration of the <see cref="WallpaperDefaultSettings" />.
    /// </summary>
    /// <value>
    ///   The delegate invoked to request the configuration of the <see cref="WallpaperDefaultSettings" />.
    /// </value>
    protected Action<WallpaperCategoryVM> RequestConfigureDefaultSettings {
      get { return this.requestConfigureDefaultSettings; }
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
    ///   Initializes a new instance of the <see cref="WallpaperCategoryVM" /> class.
    /// </summary>
    /// <param name="category">
    ///   The <see cref="WallpaperManager.Data.Wallpaper" /> which should be wrapped by this View Model.
    /// </param>
    /// <param name="wallpaperChangerVM">
    ///   The <see cref="WallpaperManager.ApplicationInterface.WallpaperChangerVM" /> instance used to control cycle operations 
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
      Action<WallpaperCategoryVM> requestConfigureDefaultSettings
    ) {
      if (category == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("category"));
      }
      if (requestConfigureSelected == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestConfigureSelected"));
      }
      if (requestConfigureDefaultSettings == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("requestConfigureDefaultSettings"));
      }
      if (wallpaperChangerVM == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperChangerVM"));
      }

      this.category = category;
      this.wallpaperChangerVM = wallpaperChangerVM;
      this.requestConfigureSelected = requestConfigureSelected;
      this.requestConfigureDefaultSettings = requestConfigureDefaultSettings;

      CollectionChangedEventManager.AddListener(category, this);

      // Simulate adding of all wallpapers to the category so that a WallpaperVM is created for any Wallpaper instance.
      this.handleCategoryCollectionChanged = true;
      this.Category_CollectionChanged(
        this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, category)
      );

      this.selectedWallpaperVMs = new ReadOnlyCollection<WallpaperVM>(new List<WallpaperVM>());

      PropertyChangedEventManager.AddListener(wallpaperChangerVM, this, String.Empty);
      this.WallpaperChangerVM_PropertyChanged(this, new PropertyChangedEventArgs("ActiveWallpapers"));
    }
    #endregion

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
        if (this.configureDefaultSettingsCommand == null) {
          this.configureDefaultSettingsCommand = new DelegateCommand(this.ConfigureDefaultSettingsCommand_Execute, this.ConfigureDefaultSettingsCommand_CanExecute);
        }

        return this.configureDefaultSettingsCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ConfigureDefaultSettingsCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ConfigureDefaultSettingsCommand" />
    protected Boolean ConfigureDefaultSettingsCommand_CanExecute() {
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
      #if DEBUG
      if (this.SelectedWallpaperVMs.Count == 0) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionIsEmpty("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      if (this.SelectedWallpaperVMs.Contains(null)) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionContainsNullItem("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      #endif

      Boolean isActivated = true;
      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null) {
          if (!wallpaperVM.Wallpaper.IsActivated) {
            isActivated = false;
            break;
          }
        }
      }

      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null) {
          wallpaperVM.Wallpaper.IsActivated = (!isActivated);
        }
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
            this.OpenFolderOfSelectedCommand_Execute, this.OpenFolderOfSelectedCommand_CanExecute
          );
        }
    
        return this.openFolderOfSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="OpenFolderOfSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="OpenFolderOfSelectedCommand" />
    protected Boolean OpenFolderOfSelectedCommand_CanExecute() {
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
              explorer.StartInfo.Arguments = String.Concat(@"/e,/select,", wallpaperVM.Wallpaper.ImagePath);
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
        if (this.applySelectedCommand == null) {
          this.applySelectedCommand = new DelegateCommand(this.ApplySelectedCommand_Execute, this.ApplySelectedCommand_CanExecute);
        }
    
        return this.applySelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ApplySelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty or contains a <c>null</c> item.
    /// </exception>
    /// <seealso cref="ApplySelectedCommand" />
    protected Boolean ApplySelectedCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="ApplySelectedCommand" /> is executed.
    ///   Applies the <see cref="SelectedWallpaperVMs" /> as wallpaper on the Windows Desktop.
    /// </summary>
    /// <seealso cref="ApplySelectedCommand" />
    protected void ApplySelectedCommand_Execute() {
      if (this.SelectedWallpaperVMs.Count == 0) {
        return;
      }

      List<Wallpaper> wallpapersToUse = new List<Wallpaper>(this.SelectedWallpaperVMs.Count);
      foreach (WallpaperVM wallpaperVM in this.SelectedWallpaperVMs) {
        if (wallpaperVM != null) {
          wallpapersToUse.Add(wallpaperVM.Wallpaper);
        }
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
        if (this.configureSelectedCommand == null) {
          this.configureSelectedCommand = new DelegateCommand(this.ConfigureSelectedCommand_Execute, this.ConfigureSelectedCommand_CanExecute);
        }

        return this.configureSelectedCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="ConfigureSelectedCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="ConfigureSelectedCommand" />
    protected Boolean ConfigureSelectedCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="ConfigureSelectedCommand" /> is executed and requests the selected <see cref="WallpaperVM" /> 
    ///   instances to be configured.
    /// </summary>
    /// <seealso cref="ConfigureSelectedCommand" />
    protected void ConfigureSelectedCommand_Execute() {
      #if DEBUG
      this.RequestConfigureSelected(this);
      #else
      try {
        this.RequestConfigureSelected(this);
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.ConfigureSelectedCommand, exception));
      }
      #endif
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
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0 && !this.IsSynchronizedCategory);
    }

    /// <summary>
    ///   Called when <see cref="RemoveSelectedCommand" /> is executed.
    ///   Removes the selected <see cref="SelectedWallpaperVMs" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty or contains a <c>null</c> item.
    /// </exception>
    /// <seealso cref="RemoveSelectedCommand" />
    protected void RemoveSelectedCommand_Execute() {
      #if DEBUG
      if (this.SelectedWallpaperVMs.Count == 0) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionIsEmpty("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      if (this.SelectedWallpaperVMs.Contains(null)) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionContainsNullItem("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      #endif

      for (Int32 i = 0; i < this.SelectedWallpaperVMs.Count; i++) {
        if (this.SelectedWallpaperVMs[i] != null) {
          this.Remove(this.SelectedWallpaperVMs[i]);
          
          // Because the selected wallpapers collection changes if we delete one item, 
          // we need to reset the loop index.
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
            this.RemoveSelectedPhysicallyCommand_Execute, this.RemoveSelectedPhysicallyCommand_CanExecute
          );
        }
    
        return this.removeSelectedPhysicallyCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="RemoveSelectedPhysicallyCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="RemoveSelectedPhysicallyCommand" />
    protected Boolean RemoveSelectedPhysicallyCommand_CanExecute() {
      return (this.Category != null && this.SelectedWallpaperVMs.Count > 0);
    }

    /// <summary>
    ///   Called when <see cref="RemoveSelectedPhysicallyCommand" /> is executed.
    ///   Removes the selected <see cref="SelectedWallpaperVMs" /> from the hard disk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   DEBUG only: <see cref="SelectedWallpaperVMs" /> is empty or contains a <c>null</c> item.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing to a non 
    ///   existing file.
    /// </exception>
    /// <exception cref="IOException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing a file which 
    ///   is currently is in use.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   <see cref="SelectedWallpaperVMs" /> contains a <see cref="Wallpaper" /> with an image path referencing a file which 
    ///   is write protected or the user does not have the required rights to access it.
    /// </exception>
    /// <seealso cref="RemoveSelectedPhysicallyCommand" />
    protected void RemoveSelectedPhysicallyCommand_Execute() {
      #if DEBUG
      if (this.SelectedWallpaperVMs.Count == 0) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionIsEmpty("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      if (this.SelectedWallpaperVMs.Contains(null)) {
        throw new InvalidOperationException(
          ExceptionMessages.GetCollectionContainsNullItem("SelectedWallpaperVMs", VariableType.Property)
        );
      }
      #endif

      List<WallpaperVM> selectedVMs = new List<WallpaperVM>(this.SelectedWallpaperVMs);
      for (Int32 i = 0; i < selectedVMs.Count; i++) {
        if (selectedVMs[i] != null) {
          if (!File.Exists(selectedVMs[i].Wallpaper.ImagePath)) {
            throw new FileNotFoundException(ExceptionMessages.GetFileNotFound(selectedVMs[i].Wallpaper.ImagePath));
          }

          try {
            File.Delete(selectedVMs[i].Wallpaper.ImagePath);
          } catch (DirectoryNotFoundException exception) {
            throw new FileNotFoundException(
              ExceptionMessages.GetFileNotFound(selectedVMs[i].Wallpaper.ImagePath), exception
            );
          }

          if (!this.IsSynchronizedCategory) {
            this.Remove(selectedVMs[i]);
          }
        }
      }
    }
    #endregion

    #region Methods: AddSynchronizedImage
    /// <summary>
    ///   Copies an image file the selected folder of the selected 
    ///   <see cref="SynchronizedWallpaperCategory" />.
    /// </summary>
    /// <param name="imagePath">
    ///   <c>true</c> to overwrite an already existing image file; otherwise <c>false</c>.
    /// </param>
    /// <param name="overwriteOnExist">
    ///   A <see cref="Boolean" /> indicating whether the image file should be overwritten if it exists already.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="imagePath" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="IOException">
    ///   <paramref name="overwriteOnExist" /> is <c>false</c> and the file does already exist or 
    ///   an I/O error has occurred.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The wrapped <see cref="WallpaperCategory" /> is no <see cref="SynchronizedWallpaperCategory" />.
    /// </exception>
    /// <inheritdoc cref="File.Copy(String, String)" />
    public virtual void AddSynchronizedImage(Path imagePath, Boolean overwriteOnExist) {
      if (imagePath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone("imagePath"));
      }
      if (!this.IsSynchronizedCategory) {
        throw new InvalidOperationException(ExceptionMessages.GetCategoryIsNoSynchronizedFolder());
      }

      SynchronizedWallpaperCategory synchronizedCategory = (this.Category as SynchronizedWallpaperCategory);
      if (synchronizedCategory != null) {
        File.Copy(imagePath, Path.Concat(synchronizedCategory.SynchronizedDirectoryPath, imagePath.FileName), overwriteOnExist);
      }
    }
    #endregion

    #region Methods: InsertItem, SetItem, RemoveItem, ClearItems
    /// <inheritdoc />
    protected override void InsertItem(Int32 index, WallpaperVM item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

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
    protected override void SetItem(Int32 index, WallpaperVM item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

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
    protected override void RemoveItem(Int32 index) {
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
    #endregion

    #region Methods: Category_CollectionChanged, WallpaperChangerVM_PropertyChanged, OnPropertyChanged
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
    private void Category_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      // No need to observe our own internal changes on the Category.
      if (!this.handleCategoryCollectionChanged) {
        return;
      }

      if (
        (e.Action == NotifyCollectionChangedAction.Add) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)
      ) {
        // Loop through added items.
        for (Int32 i = 0; i < e.NewItems.Count; i++) {
          base.InsertItem(this.Count, new WallpaperVM((Wallpaper)e.NewItems[i]));
        }
      }

      if (
        (e.Action == NotifyCollectionChangedAction.Remove) ||
        (e.Action == NotifyCollectionChangedAction.Replace) ||
        (e.Action == NotifyCollectionChangedAction.Reset)
      ) {
        // Loop through removed items.
        foreach (Wallpaper wallpaper in e.OldItems) {
          for (Int32 i = 0; i < this.Count; i++) {
            if (this[i].Wallpaper == wallpaper) {
              base.RemoveItem(i);

              break;
            }
          }
        }
      }
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperManager.ApplicationInterface.WallpaperChangerVM.PropertyChanged" /> event of a 
    ///   <see cref="WallpaperChangerVM" /> instance and updates the <see cref="WallpaperVM.IsApplied">IsApplied Property</see> 
    ///   of the child <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers/*' />
    private void WallpaperChangerVM_PropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "ActiveWallpapers") {
        foreach (WallpaperVM wallpaperVM in this) {
          wallpaperVM.IsApplied = (this.WallpaperChangerVM.ActiveWallpapers.Contains(wallpaperVM.Wallpaper));
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
  }
}
