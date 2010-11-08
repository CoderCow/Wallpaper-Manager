// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using FontStyle = System.Drawing.FontStyle;

using WallpaperManager.Data;
using WallpaperManager.ApplicationInterface;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   Creates and manages the application's notify icon.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class NotifyIconManager: IDisposable {
    #region Constants: ToolTipText, IconResName
    /// <summary>
    ///   Represents the tool tip text displayed by the <see cref="NotifyIconManager" />.
    /// </summary>
    private const String ToolTipText = "Wallpaper Manager";
    /// <summary>
    ///   Represents the resource path of the main application icon.
    /// </summary>
    private const String IconResName = "WallpaperManager.Presentation_Layer.Resources.Icons.Main.ico";
    #endregion

    #region Constants: ContextMenu_Show_Name, ContextMenu_CycleNext_Name, ContextMenu_Options_Name, ContextMenu_StartCycling_Name, ContextMenu_StopCycling_Name, ContextMenu_Exit_Name
    /// <summary>
    ///   Represents the name of the show menu item in the context menu.
    /// </summary>
    private const String ContextMenu_Show_Name = "Show";

    /// <summary>
    ///   Represents the name of the cycle next menu item in the context menu.
    /// </summary>
    private const String ContextMenu_CycleNext_Name = "CycleNext";

    /// <summary>
    ///   Represents the name of the options menu item in the context menu.
    /// </summary>
    private const String ContextMenu_Options_Name = "Options";

    /// <summary>
    ///   Represents the text of the start cycling menu item in the context menu.
    /// </summary>
    private const String ContextMenu_StartCycling_Name = "StartCycling";

    /// <summary>
    ///   Represents the name of the stop cycling menu item in the context menu.
    /// </summary>
    private const String ContextMenu_StopCycling_Name = "StopCycling";

    /// <summary>
    ///   Represents the name of the exit menu item in the context menu.
    /// </summary>
    private const String ContextMenu_Exit_Name = "Exit";
    #endregion

    #region Constants: ContextMenu_Options_IconResName, ContextMenu_StartCycling_IconResName, ContextMenu_StopCycling_IconResName
    /// <summary>
    ///   Represents the resource path of the configuration icon.
    /// </summary>
    private const String ContextMenu_Options_IconResName = "WallpaperManager.Presentation_Layer.Resources.Icons.Configuration.ico";

    /// <summary>
    ///   Represents the resource path of the start cycling icon.
    /// </summary>
    private const String ContextMenu_StartCycling_IconResName = "WallpaperManager.Presentation_Layer.Resources.Icons.StartCycling.ico";

    /// <summary>
    ///   Represents the resource path of the stop cycling icon.
    /// </summary>
    private const String ContextMenu_StopCycling_IconResName = "WallpaperManager.Presentation_Layer.Resources.Icons.StopCycling.ico";
    #endregion

    #region Fields: clickTimer, currentBoldedItem, ignoreNextSingleClick
    /// <summary>
    ///   The timer used to detect single or double clicks on the notify icon.
    /// </summary>
    private readonly DispatcherTimer clickTimer;

    /// <summary>
    ///   The menu item which is currently bolded.
    /// </summary>
    private ToolStripMenuItem currentBoldedItem;

    /// <summary>
    ///   A <see cref="Boolean" /> indicating whether to ignore the next single click performed on the notify icon.
    /// </summary>
    private Boolean ignoreNextSingleClick;
    #endregion

    #region Property: WrappedNotifyIcon
    /// <summary>
    ///   <inheritdoc cref="WrappedNotifyIcon" select='../value/node()' />
    /// </summary>
    private readonly NotifyIcon wrappedNotifyIcon;

    /// <summary>
    ///   Gets the wrapped <see cref="System.Windows.Forms.NotifyIcon" /> instance.
    /// </summary>
    /// <value>
    ///   The wrapped <see cref="System.Windows.Forms.NotifyIcon" /> instance.
    /// </value>
    protected NotifyIcon WrappedNotifyIcon {
      get { return this.wrappedNotifyIcon; }
    }
    #endregion

    #region Property: ApplicationViewModel
    /// <summary>
    ///   <inheritdoc cref="ApplicationViewModel" select='../value/node()' />
    /// </summary>
    private readonly ApplicationVM applicationViewModel;

    /// <summary>
    ///   Gets the <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </value>
    protected ApplicationVM ApplicationViewModel {
      get { return this.applicationViewModel; }
    }
    #endregion

    #region Property: TrayIconSingleClickAction
    /// <summary>
    ///   <inheritdoc cref="TrayIconSingleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconSingleClickAction;

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconSingleClickAction {
      get { return this.trayIconSingleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(TrayIconClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(TrayIconClickAction), value));
        }

        this.trayIconSingleClickAction = value;
      }
    }
    #endregion

    #region Property: TrayIconDoubleClickAction
    /// <summary>
    ///   <inheritdoc cref="TrayIconDoubleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconDoubleClickAction;

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconDoubleClickAction {
      get { return this.trayIconDoubleClickAction; }
      set {
        if (!Enum.IsDefined(typeof(TrayIconClickAction), value)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetEnumValueInvalid(null, typeof(TrayIconClickAction), value));
        }

        if (this.trayIconDoubleClickAction != value) {
          if (this.isDisposed) {
            return;
          }

          // The double click action has changed, so we have to bold another item too.
          if (this.currentBoldedItem != null) {
            this.currentBoldedItem.Font = new Font("Tahoma", 8);
          }

          this.currentBoldedItem = null;
          String newBoldItemKey = null;
          switch (value) {
            case TrayIconClickAction.ShowMainWindow:
              newBoldItemKey = NotifyIconManager.ContextMenu_Show_Name;
              break;

            case TrayIconClickAction.ShowOptionsWindow:
              newBoldItemKey = NotifyIconManager.ContextMenu_Options_Name;
              break;

            case TrayIconClickAction.CycleNextWallpaper:
              newBoldItemKey = NotifyIconManager.ContextMenu_CycleNext_Name;
              break;
          }

          if (newBoldItemKey != null) {
            this.currentBoldedItem = (this.WrappedNotifyIcon.ContextMenuStrip.Items[newBoldItemKey] as ToolStripMenuItem);

            if (this.currentBoldedItem != null) {
              this.currentBoldedItem.Font = new Font("Tahoma", 8, FontStyle.Bold);
            }
          }
        }

        this.trayIconDoubleClickAction = value;
      }
    }
    #endregion
    

    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="NotifyIconManager" /> class.
    /// </summary>
    /// <param name="applicationViewModel">
    ///   The <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="applicationViewModel" /> is <c>null</c>.
    /// </exception>
    public NotifyIconManager(ApplicationVM applicationViewModel) {
      if (applicationViewModel == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("applicationViewModel"));
      }

      // It was first planned to use the NotifyIcon class of the AvalonLibrary. But since it is bound to
      // a Window object we better use the System.Windows.Forms.NotifyIcon class.
      this.wrappedNotifyIcon = new NotifyIcon();
      this.WrappedNotifyIcon.Text = NotifyIconManager.ToolTipText;
      this.WrappedNotifyIcon.Icon = AppEnvironment.IconFromEmbeddedResource(NotifyIconManager.IconResName);
      this.WrappedNotifyIcon.Visible = true;
      this.WrappedNotifyIcon.MouseClick += this.WrappedNotifyIcon_MouseClick;
      this.WrappedNotifyIcon.MouseDoubleClick += this.WrappedNotifyIcon_DoubleClick;

      this.clickTimer = new DispatcherTimer();
      this.clickTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
      this.clickTimer.Tick += this.ClickTimer_Tick;

      this.applicationViewModel = applicationViewModel;

      #region Create Context Menu
      this.WrappedNotifyIcon.ContextMenuStrip = new ContextMenuStrip();
      this.WrappedNotifyIcon.ContextMenuStrip.ItemClicked += this.ContextMenuStrip_ItemClicked;
      this.WrappedNotifyIcon.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;
      ToolStripMenuItem menuItem;

      // **** Show Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayShowApplication"));
      menuItem.Name = NotifyIconManager.ContextMenu_Show_Name;
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Start Cycling Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayStartAutocycling"));
      menuItem.Name = NotifyIconManager.ContextMenu_StartCycling_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(
        NotifyIconManager.ContextMenu_StartCycling_IconResName
        ).ToBitmap();
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      // **** Stop Cycling Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayStopAutocycling"));
      menuItem.Name = NotifyIconManager.ContextMenu_StopCycling_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(
        NotifyIconManager.ContextMenu_StopCycling_IconResName
        ).ToBitmap();
      menuItem.Visible = false;
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      // **** Cycle Next Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayCycleNext"));
      menuItem.Name = NotifyIconManager.ContextMenu_CycleNext_Name;
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Options Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayOptions"));
      menuItem.Name = NotifyIconManager.ContextMenu_Options_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(
        NotifyIconManager.ContextMenu_Options_IconResName
        ).ToBitmap();
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Exit Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayExit"));
      menuItem.Name = NotifyIconManager.ContextMenu_Exit_Name;
      this.wrappedNotifyIcon.ContextMenuStrip.Items.Add(menuItem);
      #endregion
    }
    #endregion

    /*** Notify Icon Click Handling ***/
    #region Methods: WrappedNotifyIcon_MouseClick, ClickTimer_Tick, WrappedNotifyIcon_DoubleClick
    /// <summary>
    ///   Handles the <see cref="System.Windows.Forms.NotifyIcon.Click" /> event of the a <see cref="NotifyIcon" /> instance.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WrappedNotifyIcon_MouseClick(Object sender, MouseEventArgs  e) {
      if (this.isDisposed) {
        return;
      }

      if (e.Button == MouseButtons.Left && !this.ignoreNextSingleClick) {
        // This will execute the Single Click Action after a short time and will get deactivated if the
        // user performs a doubleclick.
        this.clickTimer.Start();
      }

      this.ignoreNextSingleClick = false;
    }

    /// <summary>
    ///   Handles the <see cref="DispatcherTimer.Tick" /> event of the ClickTimer control.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ClickTimer_Tick(Object sender, EventArgs e) {
      if (this.isDisposed) {
        return;
      }

      // Never change the order of these lines since ExecuteClickAction casts ShowDialog in some cases.
      this.clickTimer.Stop();
      this.ExecuteClickAction(this.TrayIconSingleClickAction);
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Forms.NotifyIcon.DoubleClick" /> event of the 
    ///   <see cref="WrappedNotifyIcon" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WrappedNotifyIcon_DoubleClick(Object sender, MouseEventArgs e) {
      if (this.isDisposed) {
        return;
      }

      if (e.Button == MouseButtons.Left) {
        // This prevents the Single Click Action from being executed, so not both actions get executed.
        this.clickTimer.Stop();

        // Because the MouseClick event will be executed AFTER this DoubleClick event, causing both the single and double click 
        // actions to be executed.
        this.ignoreNextSingleClick = true;

        this.ExecuteClickAction(this.TrayIconDoubleClickAction);
      }
    }

    /// <summary>
    ///   Executes the given click action.
    /// </summary>
    /// <param name="clickAction">
    ///   The click action to be executed.
    /// </param>
    private void ExecuteClickAction(TrayIconClickAction clickAction) {
      switch (clickAction) {
        case TrayIconClickAction.ShowMainWindow:
          this.ApplicationViewModel.ShowMainCommand.Execute(false);
          
          break;
        case TrayIconClickAction.ShowOptionsWindow:
          this.ApplicationViewModel.ShowConfigurationCommand.Execute();

          break;
        case TrayIconClickAction.CycleNextWallpaper:
          this.ApplicationViewModel.WallpaperChangerVM.CycleNextRandomlyCommand.Execute();

          break;
      }
    }
    #endregion

    /*** Context Menu Behavior and Click Handling ***/
    #region Methods: ContextMenuStrip_Opening, ContextMenuStrip_ItemClicked
    /// <summary>
    ///   Handles the <see cref="ToolStripDropDown.Opening" /> event of the 
    ///   <see cref="WrappedNotifyIcon">Wrapped Notify Icon's</see> <see cref="ContextMenuStrip" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ContextMenuStrip_Opening(Object sender, EventArgs e) {
      if (this.isDisposed) {
        return;
      }

      Boolean isAutocycling = this.ApplicationViewModel.WallpaperChangerVM.IsAutocycling;

      this.WrappedNotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StartCycling_Name].Visible = !isAutocycling;
      this.WrappedNotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StopCycling_Name].Visible = isAutocycling;
    }

    /// <summary>
    ///   Handles the <see cref="ToolStrip.ItemClicked" /> event of the 
    ///   <see cref="WrappedNotifyIcon">Wrapped Notify Icon's</see> <see cref="ContextMenuStrip" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ContextMenuStrip_ItemClicked(Object sender, ToolStripItemClickedEventArgs e) {
      if (this.isDisposed) {
        return;
      }

      switch (e.ClickedItem.Name) {
        case NotifyIconManager.ContextMenu_Show_Name:
          this.ExecuteClickAction(TrayIconClickAction.ShowMainWindow);

          break;
        case NotifyIconManager.ContextMenu_CycleNext_Name:
          this.ExecuteClickAction(TrayIconClickAction.CycleNextWallpaper);

          break;
        case NotifyIconManager.ContextMenu_StartCycling_Name:
          this.ApplicationViewModel.WallpaperChangerVM.StartCyclingCommand.Execute();

          e.ClickedItem.Visible = false;
          this.wrappedNotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StopCycling_Name].Visible = true;

          break;
        case NotifyIconManager.ContextMenu_StopCycling_Name:
          this.ApplicationViewModel.WallpaperChangerVM.StopCyclingCommand.Execute();

          e.ClickedItem.Visible = false;
          this.wrappedNotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StartCycling_Name].Visible = true;

          break;
        case NotifyIconManager.ContextMenu_Options_Name:
          this.ExecuteClickAction(TrayIconClickAction.ShowOptionsWindow);

          break;
        case NotifyIconManager.ContextMenu_Exit_Name:
          this.ApplicationViewModel.TerminateApplicationCommand.Execute();

          break;
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
          if (this.WrappedNotifyIcon != null) {
            this.wrappedNotifyIcon.Dispose();
          }
        }
        
        this.isDisposed = true;
      }
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="NotifyIconManager"/> class.
    /// </summary>
    ~NotifyIconManager() {
      this.Dispose(false);
    }
    #endregion
  }
}