// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Windows.Threading;
using Common.Diagnostics;
using Microsoft.Win32;

using Common;
using Common.IO;
using Common.Windows;

using WallpaperManager.Data;

namespace WallpaperManager.Application {
  /// <summary>
  ///   Manages the cycling of <see cref="Wallpaper" /> objects.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This class handles the picking of random <see cref="Wallpaper" /> objects and uses a <see cref="WallpaperBuilder" /> 
  ///     instance to build the wallpaper image which is then applied on the Windows Desktop.
  ///   </para>
  ///   <para>
  ///     You have to register at least one delegate with the <see cref="RequestWallpapers">RequestWallpapers Event</see> to
  ///     provide the changer with a collection of <see cref="Wallpaper" /> objects to cycle with.
  ///   </para>
  ///   <para>
  ///     Any Wallpaper Building performed by instances of this class are done by using a <see cref="BackgroundWorker" /> 
  ///     object. Therefore, no blocking should be expected in the calling thread when using instances of this class.
  ///   </para>
  ///   <para>
  ///     For more information about the cycling process of Wallpaper Manager see: 
  ///     <a href="7fc94554-c2fc-4940-b37b-349c6b7fa865.htm">The Cycling Process</a>.
  ///   </para>
  /// </remarks>
  /// <seealso href="7fc94554-c2fc-4940-b37b-349c6b7fa865.htm" target="_self">Wallpaper Cycling Process</seealso>
  /// <seealso href="dd316e3f-9541-46f1-961c-3c057c166f3b.htm" target="_self">Wallpaper Building Process</seealso>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
  /// <seealso cref="RequestWallpapersEventArgs">RequestWallpapersEventArgs Class</seealso>
  /// <seealso cref="RequestWallpapers">RequestWallpapers Event</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperChanger: INotifyPropertyChanged, IDisposable {
    #region Static Fields: buildWallpaperWorker, buildWallpaperWorkerCaller
    /// <summary>
    ///   The <see cref="BackgroundWorker" /> performing the current wallpaper build asynchronously.
    /// </summary>
    private static BackgroundWorker buildWallpaperWorker;

    /// <summary>
    ///   The <see cref="WallpaperChanger" /> which initiated the current wallpaper build.
    /// </summary>
    private static WallpaperChanger buildWallpaperWorkerCaller;
    #endregion

    #region Fields: lastActiveWallpapers, lastCycleTime, wallpaperImagePath, buildWallpaperWorker, random
    /// <summary>
    ///   A collection of the last applied <see cref="Wallpaper" /> objects.
    /// </summary>
    private readonly LastActiveWallpaperCollection lastCycledWallpapers;

    /// <summary>
    ///   The time when the last cycle was performed. <c>null</c> if there was no cycle yet.
    /// </summary>
    private DateTime? lastCycleTime;

    /// <summary>
    ///   The <see cref="Random" /> object used to randomize the cycle process.
    /// </summary>
    private Random random;
    #endregion

    #region Properties: WallpaperBuilder, WallpaperChangeType
    /// <summary>
    ///   <inheritdoc cref="WallpaperBuilder" select='../value/node()' />
    /// </summary>
    private WallpaperBuilderBase wallpaperBuilder;

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperBuilder" /> object.
    /// </summary>
    /// <value>
    ///   The wallpaper builder object.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set this property's value to <c>null</c>.
    /// </exception>
    /// <seealso cref="WallpaperBuilderBase">WallpaperBuilderBase Class</seealso>
    protected WallpaperBuilderBase WallpaperBuilder {
      get { return this.wallpaperBuilder; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the 
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the change type to a value which is not a constant of 
    ///   <see cref="WallpaperManager.Data.WallpaperChangeType" />.
    /// </exception>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    public virtual WallpaperChangeType WallpaperChangeType {
      get {
        if (this.WallpaperBuilder is WallpaperBuilderAll) {
          return WallpaperChangeType.ChangeAll;
        }
        if (this.WallpaperBuilder is WallpaperBuilderAllCloned) {
          return WallpaperChangeType.ChangeAllCloned;
        }
        if (this.WallpaperBuilder is WallpaperBuilderOneByOne) {
          return WallpaperChangeType.ChangeOneByOne;
        }

        return 0;
      }
      set {
        if (!Enum.IsDefined(typeof(WallpaperChangeType), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(WallpaperChangeType), value));
        }

        this.wallpaperBuilder = this.NewWallpaperBuilderByChangeType(value);
        this.OnPropertyChanged("WallpaperChangeType");
      }
    }

    /// <summary>
    ///   Creates a new type derived from <see cref="WallpaperBuilderBase" /> by the given <paramref name="changeType" />.
    /// </summary>
    /// <param name="changeType">
    ///   The <see cref="WallpaperChangeType" /> where the right Wallpaper Builder is guessed of.
    /// </param>
    /// <returns>
    ///   The new Wallpaper Builder or <c>null</c> if the given <paramref name="changeType" /> is unknown by this method.
    /// </returns>
    /// /// <seealso cref="WallpaperManager.Data.WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    private WallpaperBuilderBase NewWallpaperBuilderByChangeType(WallpaperChangeType changeType) {
      switch (changeType) {
        case WallpaperChangeType.ChangeAll:
          return new WallpaperBuilderAll(this.ScreensSettings);
        case WallpaperChangeType.ChangeAllCloned:
          return new WallpaperBuilderAllCloned(this.ScreensSettings);
        case WallpaperChangeType.ChangeOneByOne:
          return new WallpaperBuilderOneByOne(this.ScreensSettings);
      }

      return null;
    }
    #endregion

    #region Property: AutocycleInterval
    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set the interval to a value which is lower than <see cref="GeneralConfig.MinAutocycleIntervalSeconds" />.
    /// </exception>
    public TimeSpan AutocycleInterval {
      get { return this.AutocycleTimer.Interval; }
      set {
        if (value.TotalSeconds < GeneralConfig.MinAutocycleIntervalSeconds) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueMustBeGreaterThanValue(
            value.ToString(), GeneralConfig.MinAutocycleIntervalSeconds.ToString(CultureInfo.CurrentCulture)
          ));
        }

        this.AutocycleTimer.Interval = value;

        // Reset Timer if necessary.
        if ((!this.isDisposed) && (this.AutocycleTimer.IsEnabled)) {
          this.ResetAutocycling();
        }
        this.OnPropertyChanged("AutocycleInterval");
      }
    }
    #endregion

    #region Property: LastActiveListSize
    /// <summary>
    ///   <inheritdoc cref="LastActiveListSize" select='../value/node()' />
    /// </summary>
    private Byte lastActiveListSize;

    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a value which is not between <c>1</c> and <see cref="GeneralConfig.LastActiveListSizeMax" />.
    /// </exception>
    public Byte LastActiveListSize {
      get { return this.lastActiveListSize; }
      set {
        if (!value.IsBetween(1, GeneralConfig.LastActiveListSizeMax)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetValueOutOfRange(
            null, value, 1.ToString(CultureInfo.CurrentCulture), 
            GeneralConfig.LastActiveListSizeMax.ToString(CultureInfo.CurrentCulture)
          ));
        }

        this.lastActiveListSize = value;
      }
    }
    #endregion

    #region Property: CycleAfterDisplaySettingsChanged
    /// <summary>
    ///   <inheritdoc cref="CycleAfterDisplaySettingsChanged" select='../value/node()' />
    /// </summary>
    private Boolean cycleAfterDisplaySettingsChanged;

    /// <summary>
    ///   Gets or sets a <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings 
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    public Boolean CycleAfterDisplaySettingsChanged {
      get { return this.cycleAfterDisplaySettingsChanged; }
      set {
        this.cycleAfterDisplaySettingsChanged = value;
        this.OnPropertyChanged("CycleAfterDisplaySettingsChanged");
      }
    }
    #endregion

    #region Property: ScreensSettings
    /// <summary>
    ///   <inheritdoc cref="ScreensSettings" select='../value/node()' />
    /// </summary>
    private ScreenSettingsCollection screensSettings;

    /// <summary>
    ///   Gets or sets a collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single 
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public ScreenSettingsCollection ScreensSettings {
      get { return this.screensSettings; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        this.screensSettings = value;
        this.WallpaperBuilder.ScreensSettings = value;
        this.OnPropertyChanged("ScreensSettings");
      }
    }
    #endregion

    #region Property: AppliedWallpaperFilePath
    /// <summary>
    ///   <inheritdoc cref="AppliedWallpaperFilePath" select='../value/node()' />
    /// </summary>
    private readonly Path appliedWallpaperFilePath;

    /// <summary>
    ///   Gets the <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </summary>
    /// <value>
    ///   The <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </value>
    public Path AppliedWallpaperFilePath {
      get { return this.appliedWallpaperFilePath; }
    }
    #endregion

    #region Properties: AutocycleTimer, IsAutocycling, TimeSpanUntilNextCycle
    /// <summary>
    ///   <inheritdoc cref="AutocycleTimer" select='../value/node()' />
    /// </summary>
    /// <inheritdoc cref="AutocycleTimer" select='seealso' />
    private DispatcherTimer autocycleTimer;

    /// <summary>
    ///   Gets the <see cref="DispatcherTimer" /> used for autocycling the <see cref="Wallpaper" /> instances.
    /// </summary>
    /// <value>
    ///   The <see cref="DispatcherTimer" /> object used for autocycling the <see cref="Wallpaper" /> instances.
    /// </value>
    /// <seealso cref="DispatcherTimer">DispatcherTimer Class</seealso>
    protected DispatcherTimer AutocycleTimer {
      get { return this.autocycleTimer; }
    }

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the <see cref="AutocycleTimer" /> is enabled.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the <see cref="AutocycleTimer" /> is enabled.
    /// </value>
    public Boolean IsAutocycling {
      get { return this.AutocycleTimer.IsEnabled; }
    }

    /// <summary>
    ///   Gets the <see cref="TimeSpan" /> representing the reamining time until the next automatic cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> representing the reamining time until the next automatic cycle.
    /// </value>
    public TimeSpan TimeSpanUntilNextCycle {
      get {
        if (lastCycleTime == null) {
          return TimeSpan.Zero;
        }

        return this.AutocycleTimer.Interval - (DateTime.Now - this.lastCycleTime.Value);
      }
    }
    #endregion

    #region Properties: ActiveWallpapers, ActiveWallpapersAccessor
    /// <summary>
    ///   <inheritdoc cref="ActiveWallpapers" select='../value/node()' />
    /// </summary>
    /// <inheritdoc cref="ActiveWallpapers" select='seealso' />
    private ReadOnlyCollection<Wallpaper> activeWallpapers;

    /// <summary>
    ///   Gets the <see cref="Wallpaper" /> objects which have been recently applied on the Windows Desktop.
    /// </summary>
    /// <value>
    ///   The <see cref="Wallpaper" /> objects which have been recently applied on the Windows Desktop.
    /// </value>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public ReadOnlyCollection<Wallpaper> ActiveWallpapers {
      get { return this.activeWallpapers; }
    }

    /// <summary>
    ///   <inheritdoc cref="ActiveWallpapers" select='../value/node()' />
    /// </summary>
    /// <inheritdoc cref="ActiveWallpapersAccessor" select='seealso' />
    private List<Wallpaper> activeWallpapersAccessor;

    /// <inheritdoc cref="ActiveWallpapers" />
    protected List<Wallpaper> ActiveWallpapersAccessor {
      get { return this.activeWallpapersAccessor; }
      set {
        this.activeWallpapersAccessor = value;
        this.activeWallpapers = new ReadOnlyCollection<Wallpaper>(value);

        this.OnPropertyChanged("ActiveWallpapers");
        this.OnPropertyChanged("ActiveWallpapersAccessor");
      }
    }
    #endregion

    #region Event: RequestWallpapers
    /// <summary>
    ///   Occurs when this instance is about to perform a new cycle and requests <see cref="Wallpaper" /> instances to use
    ///   for this cycle.
    /// </summary>
    /// <seealso cref="RequestWallpapersEventArgs">RequestWallpapersEventArgs Class</seealso>
    public event EventHandler<RequestWallpapersEventArgs> RequestWallpapers;

    /// <summary>
    ///   Called when this instance is about to perform a new cycle and requests <see cref="Wallpaper" /> instances to use
    ///   for this cycle.
    /// </summary>
    /// <remarks>
    ///   This method raises the <see cref="RequestWallpapers">RequestWallpapers Event</see>.
    /// </remarks>
    /// <returns>
    ///   A collection of provided <see cref="Wallpaper" /> instances.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   <see cref="RequestWallpapers" /> is <c>null</c> or no <see cref="Wallpaper" /> objects where provided.
    /// </exception>
    /// <seealso cref="RequestWallpapers">RequestWallpapers Event</seealso>
    /// <seealso cref="RequestWallpapersEventArgs">RequestWallpapersEventArgs Class</seealso>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    protected virtual IList<Wallpaper> OnRequestWallpapers() {
      if (this.RequestWallpapers != null) {
        RequestWallpapersEventArgs e = new RequestWallpapersEventArgs();
        this.RequestWallpapers(this, e);

        return e.Wallpapers;
      }

      throw new InvalidOperationException(ExceptionMessages.GetNoHandlersForRequestWallpapersEvent());
    }
    #endregion

    #region Event: AutocycleException
    /// <summary>
    ///   Occurs when an <see cref="Exception" /> is thrown while autocycling a new wallpaper.
    /// </summary>
    /// <seealso cref="ExceptionEventArgs">ExceptionEventArgs Class</seealso>
    public event EventHandler<ExceptionEventArgs> AutocycleException;

    /// <summary>
    ///   Called when an <see cref="Exception" /> is thrown while autocycling a new wallpaper.
    /// </summary>
    /// <commondoc select='All/Methods/EventRaisers[@Params="ExceptionEventArgs"]/*' params="EventName=AutocycleException" />
    /// <seealso cref="ExceptionEventArgs">ExceptionEventArgs Class</seealso>
    protected virtual void OnAutocycleException(ExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }
      
      if (AutocycleException != null) {
        this.AutocycleException(this, e);

        if (!e.IsHandled) {
          throw new InvalidOperationException(ExceptionMessages.GetExceptionNotHandled("AutocycleException"), e.Exception);
        }
      }
    }
    #endregion

    #region Event: BuildException
    /// <summary>
    ///   Occurs when an <see cref="Exception" /> is thrown while performing the building process in the static 
    ///   <see cref="BackgroundWorker" />.
    /// </summary>
    /// <seealso cref="ExceptionEventArgs">BuildExceptionEventArgs Class</seealso>
    public event EventHandler<ExceptionEventArgs> BuildException;

    /// <summary>
    ///   Called when an <see cref="Exception" /> is thrown while performing the building process in the static 
    ///   <see cref="BackgroundWorker" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventRaisers[@Params="ExceptionEventArgs"]/*' params="EventName=BuildException" />
    protected virtual void OnBuildException(ExceptionEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("e"));
      }

      if (BuildException != null) {
        this.BuildException(this, e);

        if (!e.IsHandled) {
          throw new InvalidOperationException(ExceptionMessages.GetExceptionNotHandled("BuildException"), e.Exception);
        }
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperChanger" /> class.
    /// </summary>
    /// <param name="appliedWallpaperFilePath">
    ///   The <see cref="Path" /> of the file where the generated wallpaper is stored before being applied.
    /// </param>
    /// <param name="screensSettings">
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="appliedWallpaperFilePath" /> or <paramref name="screensSettings" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public WallpaperChanger(Path appliedWallpaperFilePath, ScreenSettingsCollection screensSettings) {
      if (appliedWallpaperFilePath == Path.None) {
        throw new ArgumentNullException(ExceptionMessages.GetPathCanNotBeNone("appliedWallpaperFilePath"));
      }
      if (screensSettings == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("screensSettings"));
      }

      this.appliedWallpaperFilePath = appliedWallpaperFilePath;
      this.screensSettings = screensSettings;

      this.lastActiveListSize = GeneralConfig.LastActiveListSizeMax;
      this.lastCycledWallpapers = new LastActiveWallpaperCollection(this.LastActiveListSize);
      this.activeWallpapersAccessor = new List<Wallpaper>();
      this.activeWallpapers = new ReadOnlyCollection<Wallpaper>(this.activeWallpapersAccessor);

      this.autocycleTimer = new DispatcherTimer(DispatcherPriority.Background);
      this.autocycleTimer.IsEnabled = false;
      this.autocycleTimer.Tick += this.AutocycleTimer_Tick;

      this.wallpaperBuilder = this.NewWallpaperBuilderByChangeType(WallpaperChangeType.ChangeAll);
      SystemEvents.DisplaySettingsChanged += this.System_DisplaySettingsChanged;

      this.random = new Random();
    }
    #endregion

    #region Methods: StartCycling, StopCycling, StopCyclingInternal, ResetAutocycling, AutochangeTimer_Tick, System_DisplaySettingsChanged
    /// <summary>
    ///   Starts the autocycling of wallpapers.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void StartCycling() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if (this.IsAutocycling) {
        return;
      }

      this.AutocycleTimer.Start();
      this.lastCycleTime = DateTime.Now;
      this.OnPropertyChanged("IsAutocycling");
    }

    /// <summary>
    ///   Stops the autocycling of wallpapers.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void StopCycling() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }
      if (!this.IsAutocycling) {
        return;
      }

      this.AutocycleTimer.Stop();
      this.OnPropertyChanged("IsAutocycling");
    }

    /// <summary>
    ///   Resets the internal timer used to autocycle wallpapers.
    /// </summary>
    /// <commondoc select='IDisposable/Methods/All/*' />
    public void ResetAutocycling() {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }

      if (this.AutocycleTimer.IsEnabled) {
        this.AutocycleTimer.Stop();
        this.AutocycleTimer.Start();
        this.lastCycleTime = DateTime.Now;
      }
    }

    /// <summary>
    ///   Handles the <see cref="DispatcherTimer.Tick" /> event of the <see cref="AutocycleTimer" /> and cycles the next random 
    ///   wallpaper.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void AutocycleTimer_Tick(Object sender, EventArgs e) {
      if (this.isDisposed) {
        if (this.AutocycleTimer != null) {
          this.AutocycleTimer.Stop();
        }

        return;
      }

      #if DEBUG
      this.CycleNextRandomly();
      #else
      try {
        this.CycleNextRandomly();
      } catch (Exception exception) {
        this.OnAutocycleException(new ExceptionEventArgs(exception));
      }
      #endif
    }

    /// <summary>
    ///   Handles the <see cref="SystemEvents.DisplaySettingsChanged" /> event of a <see cref="SystemEvents" /> class and
    ///   refreshes interal cached screen data, then performs a cycle if <see cref="CycleAfterDisplaySettingsChanged" /> 
    ///   is <c>true</c>
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void System_DisplaySettingsChanged(Object sender, EventArgs e) {
      if (this.isDisposed) {
        return;
      }

      this.ScreensSettings.RefreshBounds();

      if (this.CycleAfterDisplaySettingsChanged) {
        try {
          this.CycleNextRandomly();
        } catch (Exception exception) {
          this.OnAutocycleException(new ExceptionEventArgs(exception));
        }
      }
    }
    #endregion
    
    #region Methods: CheckWallpaperListIntegrity, CycleNextRandomly, CycleNext
    /// <summary>
    ///   Checks if the given list or the collection returned by the <see cref="RequestWallpapers" /> event contains
    ///   enought <see cref="Wallpaper" /> objects (and matching settings) to perform a successfull cycle.
    /// </summary>
    /// <param name="wallpapersToCheck">
    ///   The collection of <see cref="Wallpaper" /> objects to check. Set to <c>null</c> to have this list being requested by the
    ///   <see cref="RequestWallpapers" /> event.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether the <see cref="Wallpaper" /> objects in the collection are sufficient to
    ///   perform a cycle or not.
    /// </returns>
    /// <seealso cref="RequestWallpapers">RequestWallpapers Event</seealso>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public Boolean CheckWallpaperListIntegrity(IList<Wallpaper> wallpapersToCheck = null) {
      if (wallpapersToCheck == null) {
        wallpapersToCheck = this.OnRequestWallpapers();
      }
      if (wallpapersToCheck.Count == 0) {
        return false;
      }

      Int32 singleScreenWPsNeeded = this.ScreensSettings.RandomCycledScreenCount;
      // If this condition matches, then no check is required since all monitors should display static wallpapers.
      if (singleScreenWPsNeeded == 0) {
        return true;
      }

      Boolean isMultiscreenSystem = (this.ScreensSettings.Count > 1);
      foreach (Wallpaper wallpaper in wallpapersToCheck) {
        if (!wallpaper.IsActivated) {
          continue;
        }

        if (isMultiscreenSystem && wallpaper.IsMultiscreen && wallpaper.EvaluateCycleConditions()) {
          return true;
        }

        if (!wallpaper.IsMultiscreen && wallpaper.EvaluateCycleConditions()) {
          singleScreenWPsNeeded--;

          if (singleScreenWPsNeeded <= 0) {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    ///   Cycles the next wallpaper on the Windows Desktop by picking random <see cref="Wallpaper" /> objects from the 
    ///   collection provided in by the <paramref name="wallpapersToPickFrom" /> parameter or by the 
    ///   <see cref="RequestWallpapers" /> event if the parameter is <c>null</c>.
    /// </summary>
    /// <inheritdoc cref="BuildAndApplyWallpaper(IList{IList{Wallpaper}},Boolean)" select='remarks' />
    /// <param name="wallpapersToPickFrom">
    ///   A collection of <see cref="Wallpaper" /> objects to randomly pick from. If set to <c>null</c>, the 
    ///   <see cref="RequestWallpapers" /> event is used to gather them.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="wallpapersToPickFrom" /> contains a <c>null</c> item.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   There are not enought useable <see cref="Wallpaper" /> instances to cycle or the collection provided by the
    ///   <see cref="RequestWallpapers" /> event contains a <c>null</c> item.
    /// </exception>
    /// <inheritdoc 
    ///   cref="BuildAndApplyWallpaper(IList{IList{Wallpaper}},Boolean)" 
    ///   select='exception[@cref="NotSupportedException"]|exception[@cref="SecurityException"]|exception[@cref="UnauthorizedAccessException"]|permission[@cref="FileIOPermission"]' 
    /// />
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// <seealso cref="RequestWallpapers">RequestWallpapers Event</seealso>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Cycles the next wallpaper on the Windows Desktop by picking random <see cref="Wallpaper" /> objects from the 
    ///     collection provided.
    ///   </summary>
    ///   <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// </overloads>
    public void CycleNextRandomly(IList<Wallpaper> wallpapersToPickFrom) {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }

      Debug.WriteLine("-- Starting random wallpaper picking --");
      Debug.Indent();
      // If all screens should display static wallpapers anyway, we have nearly nothing to do here.
      if (this.ScreensSettings.AllStatic) {
        Debug.WriteLine("Applying by using a dummy wallpaper because all screens should display static wallpapers.");
        // Build by using a dummy wallpaper.
        this.BuildAndApplyWallpaper(new[] { new[] { new Wallpaper() } }, false);
        this.ActiveWallpapersAccessor = new List<Wallpaper>();
        this.CyclePostActions();
        return;
      }

      if (wallpapersToPickFrom != null) {
        if (wallpapersToPickFrom.Contains(null)) {
          throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapersToPickFrom"));
        }
      } else {
        Debug.WriteLine("No wallpapers given, starting a request.");
        wallpapersToPickFrom = this.OnRequestWallpapers();

        if (wallpapersToPickFrom.Contains(null)) {
          throw new InvalidOperationException(ExceptionMessages.GetCollectionContainsNullItem());
        }
      }
      if (wallpapersToPickFrom.Count == 0) {
        throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle());
      }
      Debug.Write(wallpapersToPickFrom.Count);
      Debug.WriteLine(" wallpapers given to pick from.");
      Debug.WriteLine("Performing first pick step...");
      Debug.Flush();

      #region First: Filter, enumerate the wallpapers and accumulate the priority values
      // The highest priority value of multi- and singlescreen wallpapers.
      Byte maxMultiscreenPriority = new Byte();
      Byte maxSinglescreenPriority = new Byte();

      // The total value of all priorities of multi- and singlescreen Wallpapers.
      // This values are required to decide whether to pick one multiscreen- or multiple singlescreen-wallpaper(s) later.
      Int32 sumMultiscreenPriority = new Int32();
      Int32 sumSinglescreenPriority = new Int32();

      // The sum of all wallpaper activated for a specified screen. (e.g. If we need to cycle a wallpaper on the second screen 
      // later, but there are no activated wallpaper for screen 2, then we can't perform a cycle.)
      Int32[] sumSinglescreenByScreen = new Int32[this.ScreensSettings.Count];

      // Filter out any not activated or priority <= 0 wallpapers and also measure some data.
      List<Wallpaper> filteredWallpapers = new List<Wallpaper>(wallpapersToPickFrom.Count);
      for (Int32 i = 0; i < wallpapersToPickFrom.Count; i++) {
        Wallpaper wallpaper = wallpapersToPickFrom[i];

        if (!wallpaper.IsActivated || wallpaper.Priority <= 0) {
          continue;
        }

        if (wallpaper.IsMultiscreen) {
          if (wallpaper.Priority > maxMultiscreenPriority) {
            maxMultiscreenPriority = wallpaper.Priority;
          }

          sumMultiscreenPriority += wallpaper.Priority;
        } else {
          if (wallpaper.Priority > maxSinglescreenPriority) {
            maxSinglescreenPriority = wallpaper.Priority;
          }

          sumSinglescreenPriority += wallpaper.Priority;
            
          for (Int32 x = 0; x < this.ScreensSettings.Count; x++) {
            if (!wallpaper.DisabledScreens.Contains(x)) {
              sumSinglescreenByScreen[x]++;
            }
          }
        }

        filteredWallpapers.Insert(this.random.Next(0, filteredWallpapers.Count + 1), wallpaper);
      }
      #endregion

      // No way to cycle when there are no useable Wallpapers.
      if (filteredWallpapers.Count == 0) {
        throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle(1));
      }

      // Determine how many singlescreen Wallpapers would be required for a cycle using the assigned WallpaperBuilder.
      ReadOnlyCollection<Int32> requiredWallpapersByScreen = this.WallpaperBuilder.RequiredWallpapersByScreen;
      Int32 requiredWallpaperCount = 0;
      for (Int32 i = 0; i < requiredWallpapersByScreen.Count; i++) {
        requiredWallpaperCount += requiredWallpapersByScreen[i];
      }

      #region Decide whether to pick a Multiscreen- or Singlescreen-Wallpaper(s)
      // Indicates whether a Multiscreen-Wallpaper should be picked.
      Boolean multiscreenMode;

      // The decision is simple if there are no Singlescreen- or Multiscreen-Wallpapers (both can't be).
      if (sumSinglescreenPriority == 0 || sumMultiscreenPriority == 0) {
        multiscreenMode = (sumMultiscreenPriority != 0);
      } else {
        // If we have to pick multiple singlescreen Wallpapers, then we should divide their priorities first for a better
        // picking accuracy.
        sumSinglescreenPriority /= requiredWallpaperCount;

        Int32 minSumPriority = this.random.Next(1, sumMultiscreenPriority + sumSinglescreenPriority + 1);
        multiscreenMode = (sumMultiscreenPriority < minSumPriority);
      }

      if (multiscreenMode) {
        requiredWallpaperCount = 1;
      }
      #endregion

      #region Second filerting by single- or multiscreen wallpapers and organizing the filter list.
      Debug.WriteLine("Performing second pick step...");
      Debug.Flush();
      // And filter the list again by single- or multiscreen wallpapers.
      // Note: The filtered list cannot be empty because multiscreenMode cannot be true if there are no multiscreen wallpapers 
      //       and neither be false if there are no singlescreen wallpapers.
      for (Int32 i = 0; i < filteredWallpapers.Count; i++) {
        if (filteredWallpapers[i].IsMultiscreen != multiscreenMode) {
          filteredWallpapers.RemoveAt(i);
          i--;
        }
      }

      // Sort the wallpapers by priority in descending order.
      filteredWallpapers.Sort(new Comparison<Wallpaper>((a, b) => a.Priority.CompareTo(b.Priority)));
      #endregion

      // TODO: This check should not be necessary.
      // No way to cycle when there are not enought usable wallpapers.
      if (filteredWallpapers.Count == 0) {
        throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle(2));
      }
      
      if (!multiscreenMode) {
        if (filteredWallpapers.Count < requiredWallpaperCount) {
          throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle(3));
        }

        for (Int32 i = 0; i < requiredWallpapersByScreen.Count; i++) {
          if (sumSinglescreenByScreen[i] < requiredWallpapersByScreen[i]) {
            throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle(4));
          }
        }
      }

      // Refresh the maximum size of the last activate wallpapers collection since the amount of cycleable Wallpapers may 
      // have changed.
      Int32 lastWallpapersMaximum = (filteredWallpapers.Count * (this.LastActiveListSize / 100));
      if (lastWallpapersMaximum == 0) {
        lastWallpapersMaximum = 3;
      }
      this.lastCycledWallpapers.MaximumSize = lastWallpapersMaximum;
      
      #region Pick the wallpapers by priority.
      Debug.WriteLine("Performing third pick step...");
      Debug.Flush();

      // Enumerate some more information before starting the random picking.
      Byte maxPriority;

      if (multiscreenMode) {
        maxPriority = maxMultiscreenPriority;
      } else {
        maxPriority = maxSinglescreenPriority;
      }

      // We genereally do not want to pick Wallpapers from the last cycles again, but we can't consider the last active 
      // Wallpapers if there are generally too few Wallpapers in the list.
      Int32 lastActiveWallpapersCount = 0;
      foreach (Wallpaper wallpaper in this.lastCycledWallpapers) {
        if (wallpaper.IsMultiscreen == multiscreenMode) {
          lastActiveWallpapersCount++;
        }
      }

      Boolean considerLastActives = (filteredWallpapers.Count >= (lastActiveWallpapersCount + requiredWallpaperCount));

      // The Wallpapers picked for each screen (only 1 items when picking a Multiscreen-Wallpaper.
      List<Wallpaper>[] pickedWallpapersForScreen;

      if (multiscreenMode) {
        pickedWallpapersForScreen = new List<Wallpaper>[1] { new List<Wallpaper>() };
      } else {
        pickedWallpapersForScreen = new List<Wallpaper>[this.ScreensSettings.Count];

        for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
          pickedWallpapersForScreen[i] = new List<Wallpaper>();
        }
      }

      Int32 pickedWallpapersCount = 0;

      // Finally pick the wallpapers.
      Boolean considerConditions = true;
      Boolean considerPriority = true;
      Boolean considerDisabledScreens = (this.WallpaperChangeType != WallpaperChangeType.ChangeAllCloned);
      while (pickedWallpapersCount < requiredWallpaperCount) {
        Byte minPriority = (Byte)this.random.Next(1, maxPriority + 1);
        
        for (Int32 i = 0; i < filteredWallpapers.Count; i++) {
          Wallpaper wallpaper = filteredWallpapers[i];

          if (pickedWallpapersCount >= requiredWallpaperCount) {
            break;
          }

          if (
            (considerLastActives && this.lastCycledWallpapers.Contains(wallpaper)) ||
            (considerConditions && !filteredWallpapers[i].EvaluateCycleConditions())
          ) {
            // We shouldn't remove too many wallpapers.
            if (filteredWallpapers.Count == requiredWallpaperCount) {
              considerLastActives = false;
              considerConditions = false;
              considerPriority = false;
              considerDisabledScreens = false;
            } else {
              filteredWallpapers.RemoveAt(i);
              i--;
              continue;
            }
          }

          if (considerPriority && filteredWallpapers[i].Priority < minPriority) {
            // Keep the wallpaper in the list.
            continue;
          }
          
          if (multiscreenMode) {
            pickedWallpapersForScreen[0].Add(wallpaper);
            pickedWallpapersCount = 1;

            break;
          }

          for (Int32 x = 0; x < this.ScreensSettings.Count; x++) {
            if (considerDisabledScreens && wallpaper.DisabledScreens.Contains(x)) {
              continue;
            }

            if (pickedWallpapersForScreen[x].Count >= requiredWallpapersByScreen[x]) {
              continue;
            }

            pickedWallpapersForScreen[x].Add(wallpaper);
            filteredWallpapers.RemoveAt(i);
            i--;
                
            pickedWallpapersCount++;
            break;
          }
        }

        if (pickedWallpapersCount >= requiredWallpaperCount) {
          break;
        }

        // Note: This exception should never be thrown.
        if (filteredWallpapers.Count == 0) {
          throw new InvalidOperationException(ExceptionMessages.GetNotEnoughtWallpapersToCycle(5));
        }
      }
      #endregion

      // Set the picked wallpapers as last active.
      if (!considerLastActives) {
        this.lastCycledWallpapers.Clear();
      }

      List<Wallpaper> pickedWallpapers = new List<Wallpaper>(requiredWallpaperCount);
      for (Int32 i = 0; i < pickedWallpapersForScreen.Length; i++) {
        this.lastCycledWallpapers.AddRange(pickedWallpapersForScreen[i]);
        pickedWallpapers.AddRange(pickedWallpapersForScreen[i]);
      }

      Debug.Write(pickedWallpapers.Count);
      Debug.WriteLine(" wallpapers picked. Starting build...");
      Debug.Flush();
      Debug.Unindent();
      // Finally build the image and apply it on the Windows Desktop.
      this.BuildAndApplyWallpaper(pickedWallpapersForScreen, !multiscreenMode);

      if (this.WallpaperChangeType == WallpaperChangeType.ChangeOneByOne) {
        // If the change one builder is used, there is a special behavior since only one Wallpaper was picked but at least 2
        // are currently applied.
        if (pickedWallpapers.Count == 1 && this.ActiveWallpapers.Count >= this.ScreensSettings.Count) {
          // Readd all last active Wallpapers to the picked collection because they didn't change, instead of the first one
          // which had just been picked.
          for (Int32 i = this.ScreensSettings.Count - 1; i > 0; i--) {
            pickedWallpapers.Insert(0, this.ActiveWallpapers[i]);
          }
        }
      }
      this.ActiveWallpapersAccessor = pickedWallpapers;

      this.CyclePostActions();
    }

    /// <summary>
    ///   Cycles the next wallpaper on the Windows Desktop by picking random <see cref="Wallpaper" /> objects from the 
    ///   collection provided by the <see cref="RequestWallpapers" /> event.
    /// </summary>
    /// <inheritdoc 
    ///   cref="CycleNextRandomly(IList{Wallpaper})" 
    ///   select='remarks|exception[@cref="InvalidOperationException"]|exception[@cref="NotSupportedException"]|exception[@cref="ObjectDisposedException"]' 
    /// />
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// <seealso cref="RequestWallpapers">RequestWallpapers Event</seealso>
    public void CycleNextRandomly() {
      this.CycleNextRandomly(null);
    }

    /// <summary>
    ///   Cycles the next wallpaper by using the given <see cref="Wallpaper" /> objects and applies them on the Window Desktop.
    /// </summary>
    /// <inheritdoc cref="CycleNextRandomly()" select='remarks' />
    /// <param name="wallpapersToUse">
    ///   The <see cref="Wallpaper" /> objects to use for the cycle.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpapersToUse" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="wallpapersToUse" /> contains a <c>null</c> item.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="wallpapersToUse" /> is empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   There are not enought useable <see cref="Wallpaper" /> objects provided to perform a cycle.
    /// </exception>
    /// <inheritdoc cref="CycleNextRandomly()" select='exception[@cref="NotSupportedException"]' />
    /// <commondoc select='IDisposable/Methods/All/*' />
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public void CycleNext(IList<Wallpaper> wallpapersToUse) {
      if (this.isDisposed) {
        throw new ObjectDisposedException(ExceptionMessages.GetThisObjectIsDisposed());
      }

      Debug.WriteLine("-- Starting non-random wallpaper picking --");
      Debug.Indent();

      // First, we have nearly nothing to do here if all screens should display static wallpapers anyway.
      if (this.ScreensSettings.AllStatic) {
        Debug.WriteLine("Applying by using a dummy wallpaper because all screens should display static wallpapers.");

        // Build by using a dummy wallpaper.
        this.BuildAndApplyWallpaper(new[] { new[] { new Wallpaper() } }, false);
        this.ActiveWallpapersAccessor = new List<Wallpaper>();
        this.CyclePostActions();
        return;
      }

      if (wallpapersToUse == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapersToUse"));
      }
      if (wallpapersToUse.Count == 0) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCollectionIsEmpty("wallpapersToUse"));
      }
      if (wallpapersToUse.Contains(null)) {
        throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapersToUse"));
      }

      #region Look if there are enought wallpapers in the list to perform a cycle.
      Boolean containsSinglescreen = false;
      Boolean containsMultiscreen = false;
      foreach (Wallpaper wallpaper in wallpapersToUse) {
        if (wallpaper == null) {
          throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapersToUse"));
        }

        if (!wallpaper.IsMultiscreen) {
          containsSinglescreen = true;
        } else {
          containsMultiscreen = true;
        }
      }

      if ((containsSinglescreen && containsMultiscreen) || (!containsSinglescreen && !containsMultiscreen)) {
        throw new InvalidOperationException();
      }
      #endregion

      // The Wallpapers picked from the wallpapersToUse collection.
      List<Wallpaper> wallpapersToUseFinally = new List<Wallpaper>();
      #region Pick the Wallpapers we need.
      if (containsMultiscreen) {
        Debug.WriteLine("1 wallpaper picked. Starting build...");
        Debug.Unindent();
        Debug.Flush();
        this.BuildAndApplyWallpaper(new[] { new[] { wallpapersToUse[0] } }, false);
        wallpapersToUseFinally.Add(wallpapersToUse[0]);
      } else {
        ReadOnlyCollection<Int32> requiredWallpapersByScreen = this.WallpaperBuilder.RequiredWallpapersByScreen;
        List<Wallpaper>[] wallpapersToUseByScreen = new List<Wallpaper>[this.ScreensSettings.Count];
        Int32 wallpaperIndexCounter = 0;

        for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
          wallpapersToUseByScreen[i] = new List<Wallpaper>();

          for (Int32 x = 0; x < requiredWallpapersByScreen[i]; x++) {
            wallpapersToUseByScreen[i].Add(wallpapersToUse[wallpaperIndexCounter]);
            wallpapersToUseFinally.Add(wallpapersToUse[wallpaperIndexCounter]);

            // If there are too less Wallpapers in the collection, add the last Wallpaper multiple times.
            if (wallpaperIndexCounter < wallpapersToUse.Count - 1) {
              wallpaperIndexCounter++;
            }
          }
        }

        Debug.Write(wallpapersToUseFinally.Count);
        Debug.WriteLine(" wallpapers picked. Starting build...");
        Debug.Unindent();
        Debug.Flush();
        this.BuildAndApplyWallpaper(wallpapersToUseByScreen, true);
      }
      #endregion

      // Store the new Wallpapers in the cache, making sure not to pick them on the next cycles again.
      this.lastCycledWallpapers.AddRange(wallpapersToUseFinally);

      if (this.WallpaperChangeType == WallpaperChangeType.ChangeOneByOne) {
        // If the change one builder is used, there is a special behavior since only one Wallpaper was picked but at least 2
        // are currently applied.
        if ((wallpapersToUseFinally.Count == 1) && (this.ActiveWallpapers.Count >= this.ScreensSettings.Count)) {
          // Readd all last active Wallpapers to the picked collection because they didn't change, instead of the first one
          // which had just been picked.
          for (Int32 i = this.ScreensSettings.Count - 1; i > 0; i--) {
            wallpapersToUseFinally.Insert(0, this.ActiveWallpapers[i]);
          }
        }
      }
      this.ActiveWallpapersAccessor = wallpapersToUseFinally;

      this.CyclePostActions();
    }

    /// <summary>
    ///   Performs actions generally done after a cycle.
    /// </summary>
    private void CyclePostActions() {
      Debug.WriteLine("Running cycle post actions.");

      // Restart the autochange timer to avoid that the timer cycles right after the user may have just cycled manually.
      if (this.IsAutocycling) {
        this.StopCycling();
        this.StartCycling();
      }

      this.lastCycleTime = DateTime.Now;
    }
    #endregion

    #region Methods: BuildAndApplyWallpaper
    /// <summary>
    ///   Applies one or multiple <see cref="Wallpaper" /> instances on the user's desktop.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The wallpaper is applied by writing it to the given <see cref="AppliedWallpaperFilePath" /> and calling 
    ///     Common Library methods to set it on the Windows Desktop.
    ///   </para>
    ///   <para>
    ///     The Wallpaper Building Process is performed by using a static <see cref="BackgroundWorker" /> object, therefore no 
    ///     performance issues should be expected when calling this method. However it will also execute asynchronous:
    ///     Multiple calls of this method at once can lead in a possible <see cref="NotSupportedException" />,
    ///     because this class is not able to handle multiple asynchronous wallpaper builds at the same time. To prevent this 
    ///     exception on an <see cref="AutocycleTimer" /> tick, calling this method will always reset the timer if active so 
    ///     that it can not tick after this method has just been called.
    ///   </para>
    /// </remarks>
    /// <param name="wallpapersToUse">
    ///   The <see cref="Wallpaper" /> instances to use for the building process.<br />
    ///   This collection should contain as much sub collections of <see cref="Wallpaper" /> instances as required by 
    ///   the current <see cref="WallpaperBuilder" />.
    /// </param>
    /// <param name="applyMultiple">
    ///   Indicates whether one multiscreen wallpaper or mutiple singlescreen wallpapers should be build and applied.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="wallpapersToUse" /> is empty or contains a <c>null</c> item.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpapersToUse" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///   Another asynchronous Wallpaper Build Process is already in progress.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   Missing <see cref="FileIOPermissionAccess.PathDiscovery" /> or <see cref="FileIOPermissionAccess.Write" /> for the 
    ///   given path.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing file system access rights to write the file.
    /// </exception>
    /// <permission cref="FileIOPermission">
    ///   to write the file contents. Associated enumerations: 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery" /> and 
    ///   <see cref="FileIOPermissionAccess.Write" />.
    /// </permission>
    protected void BuildAndApplyWallpaper(IList<IList<Wallpaper>> wallpapersToUse, Boolean applyMultiple) {
      if (wallpapersToUse == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapersToUse"));
      }
      if (wallpapersToUse.Count == 0) {
        throw new ArgumentException(ExceptionMessages.GetCollectionIsEmpty("wallpapersToUse"));
      }
      if (wallpapersToUse.Contains(null)) {
        throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapersToUse"));
      }

      // Is there already an asynchronous build in progress?
      if ((WallpaperChanger.buildWallpaperWorker != null) && (WallpaperChanger.buildWallpaperWorker.IsBusy)) {
        throw new NotSupportedException(ExceptionMessages.GetCyclingAlreadyInProgress());
      }

      WallpaperChanger.buildWallpaperWorker = new BackgroundWorker();
      WallpaperChanger.buildWallpaperWorkerCaller = this;
      List<Wallpaper>[] wallpapersToUseSync = new List<Wallpaper>[wallpapersToUse.Count];

      Debug.WriteLine("Preparing build with the following wallpapers:");
      Debug.Indent();
      for (Int32 i = 0; i < wallpapersToUse.Count; i++) {
        wallpapersToUseSync[i] = new List<Wallpaper>(wallpapersToUse[i].Count);
        
        // Clone all Wallpaper-objects of the collection to make them thread safe for the background worker's thread.
        foreach (Wallpaper wallpaper in wallpapersToUse[i]) {
          Wallpaper clonedWallpaper = (Wallpaper)wallpaper.Clone();
          DebugHelper.WriteObjectPropertyData(clonedWallpaper);
          wallpapersToUseSync[i].Add(clonedWallpaper);
        }
      }
      Debug.Unindent();
      Debug.Flush();

      WallpaperChanger.buildWallpaperWorker.DoWork += delegate(Object sender, DoWorkEventArgs e) {
        Object[] args = (Object[])e.Argument;
        IList<Wallpaper>[] wallpapersToUseLocal = (IList<Wallpaper>[])args[0];
        Boolean applyMultipleLocal = (Boolean)args[1];
        Image wallpaperImage;
        
        Debug.WriteLine("[BuildThread] Building wallpaper...");
        Debug.Flush();
        // Use the current WallpaperBuilder to create the wallpaper image file.);
        if (!applyMultipleLocal) {
          wallpaperImage = this.WallpaperBuilder.CreateMultiscreenFromSingle(wallpapersToUseLocal[0][0], 1.0f, true);
        } else {
          wallpaperImage = this.WallpaperBuilder.CreateMultiscreenFromMultiple(wallpapersToUseLocal, 1.0f, true);
        }

        Debug.WriteLine("[BuildThread] Saving wallpaper image...");
        Debug.Flush();
        wallpaperImage.Save(this.AppliedWallpaperFilePath, ImageFormat.Bmp);
        Debug.WriteLine("[BuildThread] Applying wallpaper on Windows Desktop...");
        Debug.Flush();
        Desktop.SetWallpaper(this.AppliedWallpaperFilePath, WallpaperArrangement.Tile);
      };

      WallpaperChanger.buildWallpaperWorker.RunWorkerCompleted += delegate(Object sender, RunWorkerCompletedEventArgs e) {
        try {
          if (e.Cancelled) {
            return;
          }
          if (e.Error != null) {
            throw e.Error;
          }
          Debug.WriteLine("Build completed successfully.");
        } finally {
          ((BackgroundWorker)sender).Dispose();
          WallpaperChanger.buildWallpaperWorker = null;
          WallpaperChanger.buildWallpaperWorkerCaller = null;
          Debug.WriteLine("-- Build worker disposed --");
          Debug.Flush();

          // Explicitly release pending managed resources of the cycle and build.
          GC.Collect();
        }
      };

      Debug.WriteLine("-- Starting build worker --");
      WallpaperChanger.buildWallpaperWorker.RunWorkerAsync(new Object[] { wallpapersToUse, applyMultiple });
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

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    private Boolean isDisposed;

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          if (this.autocycleTimer != null) {
            this.autocycleTimer.Stop();
            this.autocycleTimer = null;
          }
          
          if ((WallpaperChanger.buildWallpaperWorkerCaller == this) && (WallpaperChanger.buildWallpaperWorker != null)) {
            WallpaperChanger.buildWallpaperWorker.CancelAsync();
            WallpaperChanger.buildWallpaperWorker.Dispose();
          }
        }
      }

      this.isDisposed = true;
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="WallpaperChanger" /> class.
    /// </summary>
    ~WallpaperChanger() {
      this.Dispose(false);
    }
    #endregion
  }
}
