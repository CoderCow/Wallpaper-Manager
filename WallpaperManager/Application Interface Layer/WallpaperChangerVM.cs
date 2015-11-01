// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using Common;
using Common.Presentation;

using WallpaperManager.Data;
using WallpaperManager.Business;

namespace WallpaperManager.ApplicationInterface {
  /// <commondoc select='WrappingViewModels/General/*' params="WrappedType=WallpaperChanger" />
  /// <threadsafety static="true" instance="false" />
  public class WallpaperChangerVM: IWeakEventListener, INotifyPropertyChanged {
    #region Property: WallpaperChanger
    /// <summary>
    ///   <inheritdoc cref="WallpaperChanger" select='../value/node()' />
    /// </summary>
    private readonly WallpaperChanger wallpaperChanger;

    /// <summary>
    ///   Gets the <see cref="WallpaperChanger" /> wrapped by this View Model.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperChanger" /> wrapped by this View Model.
    /// </value>
    /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
    protected WallpaperChanger WallpaperChanger {
      get { return this.wallpaperChanger; }
    }
    #endregion

    #region Property: IsAutocycling
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.IsAutocycling" />
    public Boolean IsAutocycling {
      get { return this.WallpaperChanger.IsAutocycling; }
    }
    #endregion

    #region Property: TimeSpanUntilNextCycle
    /// <summary>
    ///   Gets the <see cref="TimeSpan" /> until the next automatic cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> until the next automatic cycle.
    /// </value>
    public TimeSpan TimeSpanUntilNextCycle {
      get { return this.WallpaperChanger.TimeSpanUntilNextCycle; }
    }
    #endregion

    #region Property: ActiveWallpapers
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.ActiveWallpapers" />
    public ReadOnlyCollection<Wallpaper> ActiveWallpapers {
      get { return this.WallpaperChanger.ActiveWallpapers; }
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


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperChangerVM" /> class.
    /// </summary>
    /// <param name="wallpaperChanger">
    ///   The <see cref="WallpaperChanger" /> wrapped by this View Model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpaperChanger" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperChanger">WallpaperChanger Class</seealso>
    public WallpaperChangerVM(WallpaperChanger wallpaperChanger) {
      if (wallpaperChanger == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperChanger"));
      }

      this.wallpaperChanger = wallpaperChanger;
      PropertyChangedEventManager.AddListener(this.WallpaperChanger, this, String.Empty);
    }
    #endregion

    #region Command: StartCycling
    /// <summary>
    ///   <inheritdoc cref="StartCyclingCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand startCyclingCommand;

    /// <summary>
    ///   Gets the Start Cycling <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Start Cycling <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="StartCyclingCommand_CanExecute">StartCyclingCommand_CanExecute Method</seealso>
    /// <seealso cref="StartCyclingCommand_Execute">StartCyclingCommand_Execute Method</seealso>
    public DelegateCommand StartCyclingCommand {
      get {
        if (this.startCyclingCommand == null) {
          this.startCyclingCommand = new DelegateCommand(this.StartCyclingCommand_Execute, this.StartCyclingCommand_CanExecute);
        }

        return this.startCyclingCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="StartCyclingCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="StartCyclingCommand" />
    protected Boolean StartCyclingCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="StartCyclingCommand" /> is executed.
    ///   Starts the automatic cycling of wallpapers.
    /// </summary>
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.StartCycling" select='exception' />
    /// <seealso cref="StartCyclingCommand" />
    protected void StartCyclingCommand_Execute() {
      if (!this.WallpaperChanger.CheckWallpaperListIntegrity()) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(
          this.StartCyclingCommand, new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle())
        ));

        return;
      }

      #if DEBUG
      this.WallpaperChanger.StartCycling();
      #else
      try {
        this.WallpaperChanger.StartCycling();
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.StartCyclingCommand, exception));
      }
      #endif
    }
    #endregion

    #region Command: StopCycling
    /// <summary>
    ///   <inheritdoc cref="StopCyclingCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand stopCyclingCommand;

    /// <summary>
    ///   Gets the Stop Cycling <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Stop Cycling <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="StopCyclingCommand_CanExecute">StopCyclingCommand_CanExecute Method</seealso>
    /// <seealso cref="StopCyclingCommand_Execute">StopCyclingCommand_Execute Method</seealso>
    public DelegateCommand StopCyclingCommand {
      get {
        if (this.stopCyclingCommand == null) {
          this.stopCyclingCommand = new DelegateCommand(this.StopCyclingCommand_Execute, this.StopCyclingCommand_CanExecute);
        }

        return this.stopCyclingCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="StopCyclingCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="StopCyclingCommand" />
    protected Boolean StopCyclingCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="StopCyclingCommand" /> is executed.
    ///   Stops the automatic cycling of wallpapers.
    /// </summary>
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.StopCycling" select='exception' />
    /// <seealso cref="StopCyclingCommand" />
    protected void StopCyclingCommand_Execute() {
      #if DEBUG
      this.WallpaperChanger.StopCycling();
      #else
      try {
        this.WallpaperChanger.StopCycling();
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.StopCyclingCommand, exception));
      }
      #endif
    }
    #endregion

    #region Command: CycleNextRandomly
    /// <summary>
    ///   <inheritdoc cref="CycleNextRandomlyCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand cycleNextRandomlyCommand;

    /// <summary>
    ///   Gets the Cycle Next Randomly <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Cycle Next Randomly <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="CycleNextRandomlyCommand_CanExecute">CycleNextRandomlyCommand_CanExecute Method</seealso>
    /// <seealso cref="CycleNextRandomlyCommand_Execute">CycleNextRandomlyCommand_Execute Method</seealso>
    public DelegateCommand CycleNextRandomlyCommand {
      get {
        if (this.cycleNextRandomlyCommand == null) {
          this.cycleNextRandomlyCommand = new DelegateCommand(
            this.CycleNextRandomlyCommand_Execute, this.CycleNextRandomlyCommand_CanExecute
          );
        }

        return this.cycleNextRandomlyCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="CycleNextRandomlyCommand" /> can be executed.
    /// </summary>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="CycleNextRandomlyCommand" />
    protected Boolean CycleNextRandomlyCommand_CanExecute() {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="CycleNextRandomlyCommand" /> is executed.
    ///   Starts the automatic cycling of wallpapers.
    /// </summary>
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.CycleNextRandomly(IList{Wallpaper})" select='exception' />
    /// <seealso cref="CycleNextRandomlyCommand" />
    protected void CycleNextRandomlyCommand_Execute() {
      #if DEBUG
      this.WallpaperChanger.CycleNextRandomly();
      #else
      try {
        this.WallpaperChanger.CycleNextRandomly();
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.CycleNextRandomlyCommand, exception));
      }
      #endif
    }
    #endregion

    #region Command: CycleNext
    /// <summary>
    ///   <inheritdoc cref="CycleNextCommand" select='../value/node()' />
    /// </summary>
    private DelegateCommand<IList<Wallpaper>> cycleNextCommand;

    /// <summary>
    ///   Gets the Cycle Next <see cref="DelegateCommand" />.
    /// </summary>
    /// <value>
    ///   The Cycle Next <see cref="DelegateCommand" />.
    /// </value>
    /// <seealso cref="CycleNextCommand_CanExecute">CycleNextCommand_CanExecute Method</seealso>
    /// <seealso cref="CycleNextCommand_Execute">CycleNextCommand_Execute Method</seealso>
    public DelegateCommand<IList<Wallpaper>> CycleNextCommand {
      get {
        if (this.cycleNextCommand == null) {
          this.cycleNextCommand = new DelegateCommand<IList<Wallpaper>>(
            this.CycleNextCommand_Execute, this.CycleNextCommand_CanExecute
          );
        }

        return this.cycleNextCommand;
      }
    }

    /// <summary>
    ///   Determines if <see cref="CycleNextCommand" /> can be executed.
    /// </summary>
    /// <param name="wallpapersToUse">
    ///   The <see cref="Wallpaper" /> objects to use for the cycle.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the command can be executed or not.
    /// </returns>
    /// <seealso cref="CycleNextCommand" />
    protected Boolean CycleNextCommand_CanExecute(IList<Wallpaper> wallpapersToUse) {
      return true;
    }

    /// <summary>
    ///   Called when <see cref="CycleNextCommand" /> is executed.
    ///   Starts the automatic cycling of wallpapers.
    /// </summary>
    /// <param name="wallpapersToUse">
    ///   The <see cref="Wallpaper" /> objects to use for the cycle.
    /// </param>
    /// <inheritdoc cref="WallpaperManager.Business.WallpaperChanger.CycleNextRandomly(IList{Wallpaper})" select='exception' />
    /// <seealso cref="CycleNextCommand" />
    protected void CycleNextCommand_Execute(IList<Wallpaper> wallpapersToUse) {
      #if DEBUG
      if (wallpapersToUse != null) {
        this.WallpaperChanger.CycleNext(wallpapersToUse);
      } else {
        this.WallpaperChanger.CycleNextRandomly();
      }
      #else
      try {
        if (wallpapersToUse != null) {
          this.WallpaperChanger.CycleNext(wallpapersToUse);
        } else {
          this.WallpaperChanger.CycleNextRandomly();
        }
      } catch (Exception exception) {
        this.OnUnhandledCommandException(new CommandExceptionEventArgs(this.CycleNextCommand, exception));
      }
      #endif
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        switch (((PropertyChangedEventArgs)e).PropertyName) {
          case "IsAutocycling":
            this.OnPropertyChanged("IsAutocycling");
            break;
          case "ActiveWallpapers":
            this.OnPropertyChanged("ActiveWallpapers");
            break;
          case "TimeSpanUntilNextCycle":
            this.OnPropertyChanged("TimeSpanUntilNextCycle");
            break;
        }
        return true;
      }

      return false;
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