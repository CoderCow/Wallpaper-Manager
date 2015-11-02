// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using WallpaperManager.Models;
using WallpaperManager.ViewModels;

namespace WallpaperManager.Views {
  /// <summary>
  ///   Creates and manages the application's notify icon.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class NotifyIconManager : IDisposable {
    /// <summary>
    ///   Represents the tool tip text displayed by the <see cref="NotifyIconManager" />.
    /// </summary>
    private const string ToolTipText = "Wallpaper Manager";

    /// <summary>
    ///   Represents the resource path of the main application icon.
    /// </summary>
    private const string IconResName = "WallpaperManager.Views.Resources.Icons.Main.ico";

    /// <summary>
    ///   Represents the name of the show menu item in the context menu.
    /// </summary>
    private const string ContextMenu_Show_Name = "Show";

    /// <summary>
    ///   Represents the name of the cycle next menu item in the context menu.
    /// </summary>
    private const string ContextMenu_CycleNext_Name = "CycleNext";

    /// <summary>
    ///   Represents the name of the options menu item in the context menu.
    /// </summary>
    private const string ContextMenu_Options_Name = "Options";

    /// <summary>
    ///   Represents the text of the start cycling menu item in the context menu.
    /// </summary>
    private const string ContextMenu_StartCycling_Name = "StartCycling";

    /// <summary>
    ///   Represents the name of the stop cycling menu item in the context menu.
    /// </summary>
    private const string ContextMenu_StopCycling_Name = "StopCycling";

    /// <summary>
    ///   Represents the name of the exit menu item in the context menu.
    /// </summary>
    private const string ContextMenu_Exit_Name = "Exit";

    /// <summary>
    ///   Represents the resource name of the configuration icon.
    /// </summary>
    private const string ContextMenu_Options_IconResName = "WallpaperManager.Views.Resources.Icons.Configuration.ico";

    /// <summary>
    ///   Represents the resource name of the start cycling icon.
    /// </summary>
    private const string ContextMenu_StartCycling_IconResName = "WallpaperManager.Views.Resources.Icons.StartCycling.ico";

    /// <summary>
    ///   Represents the resource name of the stop cycling icon.
    /// </summary>
    private const string ContextMenu_StopCycling_IconResName = "WallpaperManager.Views.Resources.Icons.StopCycling.ico";

    /// <summary>
    ///   The timer used to detect single or double clicks on the notify icon.
    /// </summary>
    private readonly DispatcherTimer clickTimer;

    /// <summary>
    ///   The menu item which is currently bolded.
    /// </summary>
    private ToolStripMenuItem currentBoldedItem;

    /// <summary>
    ///   A <see cref="bool" /> indicating whether to ignore the next single click performed on the notify icon.
    /// </summary>
    private bool ignoreNextSingleClick;

    /// <summary>
    ///   <inheritdoc cref="TrayIconDoubleClickAction" select='../value/node()' />
    /// </summary>
    private TrayIconClickAction trayIconDoubleClickAction;

    /// <summary>
    ///   Gets the wrapped <see cref="System.Windows.Forms.NotifyIcon" /> instance.
    /// </summary>
    /// <value>
    ///   The wrapped <see cref="System.Windows.Forms.NotifyIcon" /> instance.
    /// </value>
    protected NotifyIcon NotifyIcon { get; }

    /// <summary>
    ///   Gets the <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </summary>
    /// <value>
    ///   The <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </value>
    protected ApplicationVM ApplicationViewModel { get; }

    /// <summary>
    ///   Gets or sets the default action when clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconSingleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking the Tray-Icon.
    /// </summary>
    /// <value>
    ///   The default action when double clicking the Tray-Icon.
    /// </value>
    public TrayIconClickAction TrayIconDoubleClickAction {
      get { return this.trayIconDoubleClickAction; }
      set {
        if (this.trayIconDoubleClickAction != value) {
          if (this.isDisposed)
            return;

          // The double click action has changed, so we have to bold another item too.
          if (this.currentBoldedItem != null)
            this.currentBoldedItem.Font = new Font("Tahoma", 8);

          this.currentBoldedItem = null;
          string newBoldItemKey = null;
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
            this.currentBoldedItem = (this.NotifyIcon.ContextMenuStrip.Items[newBoldItemKey] as ToolStripMenuItem);

            if (this.currentBoldedItem != null)
              this.currentBoldedItem.Font = new Font("Tahoma", 8, FontStyle.Bold);
          }
        }

        this.trayIconDoubleClickAction = value;
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NotifyIconManager" /> class.
    /// </summary>
    /// <param name="applicationViewModel">
    ///   The <see cref="ApplicationVM" /> instance representing the main interface with the application.
    /// </param>
    public NotifyIconManager(ApplicationVM applicationViewModel) {
      // It was first planned to use the NotifyIcon class of the AvalonLibrary. But since it is bound to
      // a Window object we better use the System.Windows.Forms.NotifyIcon class.
      this.NotifyIcon = new NotifyIcon();
      this.NotifyIcon.Text = NotifyIconManager.ToolTipText;
      this.NotifyIcon.Icon = AppEnvironment.IconFromEmbeddedResource(NotifyIconManager.IconResName);
      this.NotifyIcon.Visible = true;
      this.NotifyIcon.MouseClick += this.WrappedNotifyIcon_MouseClick;
      this.NotifyIcon.MouseDoubleClick += this.WrappedNotifyIcon_DoubleClick;

      this.clickTimer = new DispatcherTimer();
      this.clickTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
      this.clickTimer.Tick += this.ClickTimer_Tick;

      this.ApplicationViewModel = applicationViewModel;

      #region Create Context Menu
      this.NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
      this.NotifyIcon.ContextMenuStrip.ItemClicked += this.ContextMenuStrip_ItemClicked;
      this.NotifyIcon.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;
      ToolStripMenuItem menuItem;

      // **** Show Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayShowApplication"));
      menuItem.Name = NotifyIconManager.ContextMenu_Show_Name;
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);
      this.NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Start Cycling Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayStartAutocycling"));
      menuItem.Name = NotifyIconManager.ContextMenu_StartCycling_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(NotifyIconManager.ContextMenu_StartCycling_IconResName).ToBitmap();
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      // **** Stop Cycling Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayStopAutocycling"));
      menuItem.Name = NotifyIconManager.ContextMenu_StopCycling_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(NotifyIconManager.ContextMenu_StopCycling_IconResName).ToBitmap();
      menuItem.Visible = false;
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      // **** Cycle Next Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayCycleNext"));
      menuItem.Name = NotifyIconManager.ContextMenu_CycleNext_Name;
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      this.NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Options Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayOptions"));
      menuItem.Name = NotifyIconManager.ContextMenu_Options_Name;
      menuItem.Image = AppEnvironment.IconFromEmbeddedResource(NotifyIconManager.ContextMenu_Options_IconResName).ToBitmap();
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);

      this.NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

      // **** Exit Menu Item ****
      menuItem = new ToolStripMenuItem(LocalizationManager.GetLocalizedString("Menu.TrayExit"));
      menuItem.Name = NotifyIconManager.ContextMenu_Exit_Name;
      this.NotifyIcon.ContextMenuStrip.Items.Add(menuItem);
      #endregion
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.NotifyIcon != null);
      Contract.Invariant(this.ApplicationViewModel != null);
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconSingleClickAction));
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconDoubleClickAction));
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Forms.NotifyIcon.Click" /> event of the a
    ///   <see cref="System.Windows.Forms.NotifyIcon" /> instance.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WrappedNotifyIcon_MouseClick(object sender, MouseEventArgs e) {
      if (this.isDisposed)
        return;

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
    private void ClickTimer_Tick(object sender, EventArgs e) {
      if (this.isDisposed)
        return;

      // Never change the order of these lines since ExecuteClickAction casts ShowDialog in some cases.
      this.clickTimer.Stop();
      this.ExecuteClickAction(this.TrayIconSingleClickAction);
    }

    /// <summary>
    ///   Handles the <see cref="System.Windows.Forms.NotifyIcon.DoubleClick" /> event of the
    ///   <see cref="NotifyIcon" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void WrappedNotifyIcon_DoubleClick(object sender, MouseEventArgs e) {
      if (this.isDisposed)
        return;

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

    /// <summary>
    ///   Handles the <see cref="ToolStripDropDown.Opening" /> event of the
    ///   <see cref="NotifyIcon">Wrapped Notify Icon's</see> <see cref="ContextMenuStrip" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ContextMenuStrip_Opening(object sender, EventArgs e) {
      if (this.isDisposed)
        return;

      bool isAutocycling = this.ApplicationViewModel.WallpaperChangerVM.IsAutocycling;

      this.NotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StartCycling_Name].Visible = !isAutocycling;
      this.NotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StopCycling_Name].Visible = isAutocycling;
    }

    /// <summary>
    ///   Handles the <see cref="ToolStrip.ItemClicked" /> event of the
    ///   <see cref="NotifyIcon">Wrapped Notify Icon's</see> <see cref="ContextMenuStrip" />.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      if (this.isDisposed)
        return;

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
          this.NotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StopCycling_Name].Visible = true;

          break;
        case NotifyIconManager.ContextMenu_StopCycling_Name:
          this.ApplicationViewModel.WallpaperChangerVM.StopCyclingCommand.Execute();

          e.ClickedItem.Visible = false;
          this.NotifyIcon.ContextMenuStrip.Items[NotifyIconManager.ContextMenu_StartCycling_Name].Visible = true;

          break;
        case NotifyIconManager.ContextMenu_Options_Name:
          this.ExecuteClickAction(TrayIconClickAction.ShowOptionsWindow);

          break;
        case NotifyIconManager.ContextMenu_Exit_Name:
          this.ApplicationViewModel.TerminateApplicationCommand.Execute();

          break;
      }
    }

    #region IDisposable Implementation
    /// <commondoc select='IDisposable/Fields/isDisposed/*' />
    private bool isDisposed;

    /// <commondoc select='IDisposable/Methods/Dispose[@Params="Boolean"]/*' />
    protected virtual void Dispose(bool disposing) {
      if (!this.isDisposed) {
        if (disposing)
          this.NotifyIcon?.Dispose();

        this.isDisposed = true;
      }
    }

    /// <commondoc select='IDisposable/Methods/Dispose[not(@*)]/*' />
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="NotifyIconManager" /> class.
    /// </summary>
    ~NotifyIconManager() {
      this.Dispose(false);
    }
    #endregion
  }
}