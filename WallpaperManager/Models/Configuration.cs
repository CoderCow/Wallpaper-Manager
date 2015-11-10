// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Common;
using Common.Windows;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  // TODO: Add zoom functionality
  /// <summary>
  ///   The application's user-defined configuration data holder.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Use the <see cref="Configuration.Read(Stream)">Read</see> and <see cref="Configuration.Write(Stream)">Write</see>
  ///     methods provided by this class to write and read the data in a stream or file.
  ///   </para>
  ///   <para>
  ///     This class supports reading of version 1.0, 1.1 and 1.2 Wallpaper Manager configuration files.
  ///   </para>
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  public class Configuration : ValidatableBase, IConfiguration, ICloneable, IAssignable {
    /// <summary>
    ///   Represents the version number of the configuration file for backward compatibility.
    /// </summary>
    protected const string DataVersion = "1.2";

    /// <summary>
    ///   Represents the name of the root node of the XML-data.
    /// </summary>
    public const string RootNodeName = "WallpaperManagerConfiguration";

    /// <summary>
    ///   Represents the XML namespace of the root node of the XML-data.
    /// </summary>
    public const string XmlNamespace = "http://www.WallpaperManager.de.vu";

    /// <summary>
    ///   Represents the value name of the autostart registry key.
    /// </summary>
    private const string AutostartEntryRegKeyName = "Wallpaper Manager";

    /// <summary>
    ///   Represents the minimum auto cycle interval value.
    /// </summary>
    /// <seealso cref="AutocycleInterval">AutocycleInterval Property</seealso>
    public const int MinAutocycleIntervalSeconds = 10;

    /// <summary>
    ///   Represents the maxium last active list size value in percentage to the overall count of wallpapers.
    /// </summary>
    /// <seealso cref="LastActiveListSize">LastActiveListSize Property</seealso>
    public const byte LastActiveListSizeMax = 80;

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application will start when the user logs in.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application will start when the user logs in.
    /// </value>
    /// <remarks>
    ///   This value is not saved into the configuration file. Its directly accessed from the registry.
    /// </remarks>
    public bool StartWithWindows {
      get { return Autostart.CurrentUserEntries.ContainsKey(Configuration.AutostartEntryRegKeyName); }
      set {
        if (value) {
          // Make sure the entry really doesn't exist.
          if (!this.StartWithWindows)
            Autostart.CurrentUserEntries.Add(Configuration.AutostartEntryRegKeyName, Assembly.GetExecutingAssembly().Location);
        } else {
          // If the entry does exist.
          if (this.StartWithWindows)
            Autostart.CurrentUserEntries.Remove(Configuration.AutostartEntryRegKeyName);
        }
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application should do one cycle right after it has been
    ///   started.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application should do one cycle right after it has been started.
    /// </value>
    public bool CycleAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the application should terminate right after it had been
    ///   started.
    /// </summary>
    /// <remarks>
    ///   If <c>true</c>, this property requires <see cref="CycleAfterStartup" /> also to be <c>true</c> to take effect.
    /// </remarks>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the application should terminate right after it had been started.
    /// </value>
    public bool TerminateAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main view will be shown after startup.
    /// </value>
    public bool MinimizeAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the auto cycle timer will be started immedeately.
    /// </value>
    public bool StartAutocyclingAfterStartup { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="WallpaperChangeType" /> defining how non-multiscreen wallpapers are built.
    /// </summary>
    /// <value>
    ///   The change type for singlescreen wallpapers. <c>0</c> if the internal builder has no representation in the
    ///   <see cref="WallpaperChangeType" /> enumeration.
    /// </value>
    /// <seealso cref="WallpaperChangeType">WallpaperChangeType Enumeration</seealso>
    public WallpaperChangeType WallpaperChangeType { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </summary>
    /// <value>
    ///   The <see cref="TimeSpan" /> to wait between each auto cycle.
    /// </value>
    public TimeSpan AutocycleInterval { get; set; }

    /// <summary>
    ///   Gets or sets the percentage value indicating how large the last active list should be.
    /// </summary>
    /// <value>
    ///   The percentage value indicating how large the last active list should be.
    /// </value>
    public byte LastActiveListSize { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings
    ///   have changed.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the next wallpaper should be cycled if the display settings have changed.
    /// </value>
    public bool CycleAfterDisplaySettingsChanged { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the main window will always be minimized and be shown in the
    ///   Windows Task Bar even if the close button is clicked.
    /// </value>
    public bool MinimizeOnClose { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="bool" /> indicating whether to show the remaining time for the next random cycle
    ///   as overlay icon in the Windows Task Bar Windows 7 only.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether to show the remaining time for the next random cycle as overlay icon
    ///   in the Windows Task Bar Windows 7 only.
    /// </value>
    public bool DisplayCycleTimeAsIconOverlay { get; set; }

    /// <summary>
    ///   Gets or sets the default action when double clicking a wallpaper.
    /// </summary>
    /// <value>
    ///   The default action when double clicking a wallpaper.
    /// </value>
    public WallpaperClickAction WallpaperDoubleClickAction { get; set; }

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
    public TrayIconClickAction TrayIconDoubleClickAction { get; set; }

    /// <summary>
    ///   Gets or sets a collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single
    ///   screen.
    /// </summary>
    /// <value>
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </value>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public ScreenSettingsCollection ScreensSettings { get; set; }

    // TODO: Extract this. The global configuration should have its own file.
    /// <summary>
    ///   Gets the <see cref="WallpaperCategoryCollection" /> holding the
    ///   <see cref="WallpaperCategory">Wallpaper wallpaperCategories</see> which's <see cref="Wallpaper" /> instances should
    ///   be cycled.
    /// </summary>
    /// <value>
    ///   The <see cref="WallpaperCategoryCollection" /> holding the <see cref="WallpaperCategory" /> instances
    ///   which's <see cref="Wallpaper" /> instances should be cycled.
    /// </value>
    /// <seealso cref="WallpaperCategoryCollection">WallpaperCategoryCollection Class</seealso>
    public WallpaperCategoryCollection WallpaperCategories { get; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Configuration" /> class.
    /// </summary>
    public Configuration() {
      this.CycleAfterStartup = false;
      this.TerminateAfterStartup = false;
      this.StartAutocyclingAfterStartup = false;
      this.MinimizeOnClose = false;
      this.CycleAfterDisplaySettingsChanged = false;
      this.AutocycleInterval = new TimeSpan(0, 0, 30);
      this.LastActiveListSize = 30;
      this.WallpaperChangeType = WallpaperChangeType.ChangeAll;
      this.WallpaperDoubleClickAction = WallpaperClickAction.ShowConfigurationWindow;
      this.TrayIconSingleClickAction = TrayIconClickAction.NoAction;
      this.TrayIconDoubleClickAction = TrayIconClickAction.ShowMainWindow;
      this.ScreensSettings = new ScreenSettingsCollection();
      this.WallpaperCategories = new WallpaperCategoryCollection();
    }

    #region Overrides of ValidatableBase
    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(Enum.IsDefined(typeof(WallpaperChangeType), this.WallpaperChangeType));
      Contract.Invariant(Enum.IsDefined(typeof(WallpaperClickAction), this.WallpaperDoubleClickAction));
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconSingleClickAction));
      Contract.Invariant(Enum.IsDefined(typeof(TrayIconClickAction), this.TrayIconDoubleClickAction));
      Contract.Invariant(this.ScreensSettings != null);
      Contract.Invariant(this.WallpaperCategories != null);
    }
    #endregion

    // TODO: Use XmlSerializer for reading and writing the config.
    // TODO: Extract Read/Write functionality into a new class (and interface perhaps)
    /*/// <summary>
    ///   Creates a new <see cref="Configuration" /> instance by reading the data from a <see cref="Stream" />.
    /// </summary>
    /// <remarks>
    ///   This method reads Wallpaper Manager configuration files (version 1.0, 1.1 and 1.2) by using
    ///   <see cref="XmlDocument" />.
    /// </remarks>
    /// <param name="sourceStream">
    ///   The source <see cref="Stream" /> to read from.
    /// </param>
    /// <returns>
    ///   A new <see cref="Configuration" /> instance containing the read data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="sourceStream" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="IOException">
    ///   <paramref name="sourceStream" /> is not readable.
    /// </exception>
    /// <exception cref="XmlException">
    ///   There is a load or parse error in the XML-Data.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   Error while deserializing. Refer to the object returned by the  <see cref="Exception.InnerException" /> property
    ///   for details.
    /// </exception>
    /// <overloads>
    ///   <summary>
    ///     Creates a new <see cref="Configuration" /> instance by reading the data from a data source.
    ///   </summary>
    ///   <returns>
    ///     A new <see cref="Configuration" /> instance containing the read data.
    ///   </returns>
    /// </overloads>
    /// <seealso cref="Stream">Stream Class</seealso>
    public static Configuration Read(Stream sourceStream) {
      Contract.Requires<ArgumentNullException>(sourceStream != null);
      Contract.Requires<IOException>(sourceStream.CanRead);

      XmlDocument document = new XmlDocument();
      document.Load(sourceStream);

      if ((document.DocumentElement == null) || (document.DocumentElement.Name != Configuration.RootNodeName))
        throw new XmlException("The configuration file has an invalid root element.");

      XmlAttribute versionAttribute = document.DocumentElement.Attributes["Version"];
      Version configVersion;
      if ((versionAttribute == null) || (!Version.TryParse(versionAttribute.InnerText, out configVersion)))
        throw new XmlException("The configuration file has an invalid root element.");

      Configuration configuration = new Configuration();

      #region <General>
      XmlElement generalElement = document.DocumentElement["General"];
      if (generalElement == null)
        throw new XmlException("The configuration file does not contain expected element 'General'.");

      XmlElement element = generalElement["CycleAfterStartup"];
      if (element != null)
        configuration.CycleAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["TerminateAfterStartup"];
      if (element != null)
        configuration.TerminateAfterStartup = bool.Parse(element.InnerText);

      element = (generalElement["MinimizeAfterStart"] ?? generalElement["MinimizeAfterStartup"]);
      if (element != null)
        configuration.MinimizeAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["StartAutoCyclingAfterStartup"];
      if (element != null)
        configuration.StartAutocyclingAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["WallpaperChangeType"];
      if (element != null)
        configuration.WallpaperChangeType = (WallpaperChangeType)Enum.Parse(typeof(WallpaperChangeType), element.InnerText);

      element = generalElement["AutoCycleInterval"];
      if (element != null)
        configuration.AutocycleInterval = TimeSpan.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = generalElement["LastActiveListSize"];
      if (element != null)
        configuration.LastActiveListSize = byte.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = generalElement["CycleAfterDisplaySettingsChanged"];
      if (element != null)
        configuration.CycleAfterDisplaySettingsChanged = bool.Parse(element.InnerText);

      element = generalElement["MinimizeOnClose"];
      if (element != null)
        configuration.MinimizeOnClose = bool.Parse(element.InnerText);

      element = generalElement["DisplayCycleTimeAsIconOverlay"];
      if (element != null)
        configuration.DisplayCycleTimeAsIconOverlay = bool.Parse(element.InnerText);

      element = generalElement["WallpaperDoubleClickAction"];
      if (element != null)
        configuration.WallpaperDoubleClickAction = (WallpaperClickAction)Enum.Parse(typeof(WallpaperClickAction), element.InnerText);

      element = generalElement["TrayIconSingleClickAction"];
      if (element != null)
        configuration.TrayIconSingleClickAction = (TrayIconClickAction)Enum.Parse(typeof(TrayIconClickAction), element.InnerText);

      element = generalElement["TrayIconDoubleClickAction"];
      if (element != null)
        configuration.TrayIconDoubleClickAction = (TrayIconClickAction)Enum.Parse(typeof(TrayIconClickAction), element.InnerText);
      #endregion

      #region <ScreensSettings>
      XmlElement screensSettingsElement = document.DocumentElement["ScreensSettings"];
      if (screensSettingsElement != null) {
        int settingsCounter = 0;
        var screensSettings = new List<ScreenSettings>();

        foreach (XmlElement screenSettingsElement in screensSettingsElement) {
          if (screenSettingsElement.Name != "ScreenSettings")
            continue;

          // Make sure there aren't too many screen settings in the configuration.
          if (settingsCounter >= Screen.AllScreens.Length)
            break;

          ScreenSettings screenSettings = new ScreenSettings(settingsCounter);
          element = screenSettingsElement["CycleRandomly"];
          if (element != null)
            screenSettings.CycleRandomly = bool.Parse(element.InnerText);

          element = screenSettingsElement["MarginLeft"];
          if (element != null)
            screenSettings.Margins.MarginLeft = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

          element = screenSettingsElement["MarginRight"];
          if (element != null)
            screenSettings.Margins.MarginRight = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

          element = screenSettingsElement["MarginTop"];
          if (element != null)
            screenSettings.Margins.MarginTop = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

          element = screenSettingsElement["MarginBottom"];
          if (element != null)
            screenSettings.Margins.MarginBottom = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

          #region <OverlayTexts>
          XmlElement overlayTextsElement = screenSettingsElement["OverlayTexts"];
          if (overlayTextsElement != null) {
            foreach (XmlElement overlayTextElement in overlayTextsElement) {
              if (overlayTextElement.Name != "OverlayText")
                continue;
              WallpaperTextOverlay textOverlay = new WallpaperTextOverlay();

              element = overlayTextElement["Format"];
              if (element != null)
                textOverlay.Format = element.InnerText;

              element = overlayTextElement["Position"];
              if (element != null)
                textOverlay.Position = (TextOverlayPosition)Enum.Parse(typeof(TextOverlayPosition), element.InnerText);

              element = overlayTextElement["FontName"];
              if (element != null)
                textOverlay.FontName = element.InnerText;

              element = overlayTextElement["FontSize"];
              if (element != null)
                textOverlay.FontSize = float.Parse(element.InnerText, CultureInfo.InvariantCulture);

              element = overlayTextElement["FontStyle"];
              if (element != null)
                textOverlay.FontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), element.InnerText);

              element = overlayTextElement["ForeColor"];
              if (element != null)
                textOverlay.ForeColor = ColorTranslator.FromHtml(element.InnerText);

              element = overlayTextElement["BorderColor"];
              if (element != null)
                textOverlay.BorderColor = ColorTranslator.FromHtml(element.InnerText);

              element = overlayTextElement["HorizontalOffset"];
              if (element != null)
                textOverlay.HorizontalOffset = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

              element = overlayTextElement["VerticalOffset"];
              if (element != null)
                textOverlay.VerticalOffset = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

              screenSettings.TextOverlays.Add(textOverlay);
            }
          }
          #endregion

          #region <StaticWallpaper>
          XmlElement staticWallpaperElement = screenSettingsElement["StaticWallpaper"];
          if (staticWallpaperElement != null)
            screenSettings.StaticWallpaper = (Wallpaper)Configuration.GetWallpaperDataFromXmlElement(staticWallpaperElement, typeof(Wallpaper));
          #endregion

          screensSettings.Add(screenSettings);
          settingsCounter++;
        }

        configuration.ScreensSettings = new ScreenSettingsCollection(screensSettings);
      }
      #endregion

      #region <WallpaperCategories>
      XmlElement wallpaperCategoriesElement = document.DocumentElement["WallpaperCategories"];
      if (wallpaperCategoriesElement != null) {
        foreach (XmlElement wallpaperCategoryElement in wallpaperCategoriesElement) {
          if (
            wallpaperCategoryElement.Name != "Category" &&
            wallpaperCategoryElement.Name != "SynchronizedFolder" &&
            wallpaperCategoryElement.Name != "WatchedCategory" /* Version 1.1 name for synchronized folders. *//*
            )
            continue;

          element = wallpaperCategoryElement["Name"];
          if (element == null)
            continue;
          string categoryName = element.InnerText;

          WallpaperDefaultSettings defaultSettings = null;
          element = wallpaperCategoryElement["WallpaperDefaultSettings"];
          if (element != null)
            defaultSettings = (WallpaperDefaultSettings)Configuration.GetWallpaperDataFromXmlElement(element, typeof(WallpaperDefaultSettings));

          #region <Wallpapers>
          List<Wallpaper> wallpapers;

          XmlElement wallpapersElement = wallpaperCategoryElement["Wallpapers"];
          if (wallpapersElement != null) {
            wallpapers = new List<Wallpaper>(wallpapersElement.ChildNodes.Count);

            foreach (XmlElement wallpaperElement in wallpapersElement) {
              if (wallpaperElement.Name != "Wallpaper")
                continue;

              Wallpaper wallpaper = (Wallpaper)Configuration.GetWallpaperDataFromXmlElement(wallpaperElement, typeof(Wallpaper));
              wallpapers.Add(wallpaper);
            }
          } else
            wallpapers = new List<Wallpaper>(0);
          #endregion

          bool isSynchronizedFolder = ((wallpaperCategoryElement.Name == "SynchronizedFolder") || (wallpaperCategoryElement.Name == "WatchedCategory"));
          WallpaperCategory category;
          if (isSynchronizedFolder) {
            element = (wallpaperCategoryElement["SynchronizedFolderPath"] ?? wallpaperCategoryElement["WatchedDirectoryPath"]);
            if (element == null)
              continue;
            Path synchronizedDirPath = new Path(element.InnerText);

            if (!Directory.Exists(synchronizedDirPath))
              continue;

            category = new WallpaperCategoryFileSynchronizer(categoryName, synchronizedDirPath, wallpapers);
          } else
            category = new WallpaperCategory(categoryName, wallpapers);

          category.WallpaperDefaultSettings = defaultSettings;
          configuration.WallpaperCategories.Add(category);
        }
      }
      #endregion

      return configuration;
    }

    /// <summary>
    ///   Creates a new <see cref="Configuration" /> instance by reading the data from a XML-file.
    /// </summary>
    /// <inheritdoc cref="Read(Stream)" select='remarks' />
    /// <param name="filePath">
    ///   The <see cref="Path" /> of the XML-file to be read from.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="filePath" /> is <c>Path.None</c>.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   <paramref name="filePath" /> points to a file which does not exist.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///   <paramref name="filePath" /> points to a file in a directory which can not be found.
    ///   Like a directory on a unmapped drive.
    /// </exception>
    /// <exception cref="IOException">
    ///   The stream has been closed while reading.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   The caller does not have the required permission.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing file-system related access rights to read from the file located at <paramref name="filePath" />.
    /// </exception>
    /// <inheritdoc cref="Read(Stream)" select='exception[@cref="XmlException"]|exception[@cref="InvalidOperationException"]' />
    /// <inheritdoc cref="Read(Stream)" select='returns' />
    /// <seealso cref="Path">Path Class</seealso>
    public static Configuration Read(Path filePath) {
      Contract.Requires<ArgumentException>(filePath != Path.None);

      using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        return Configuration.Read(fileStream);
    }

    /// <summary>
    ///   Gets wallpaper related settings from a given <see cref="XmlElement" />.
    /// </summary>
    /// <param name="element">
    ///   The <see cref="XmlElement" /> to get the data from.
    /// </param>
    /// <param name="wallpaperSettingsType">
    ///   The type of the wallpaper settings to read.
    /// </param>
    /// <returns>
    ///   An instance of a type inherited from <see cref="WallpaperBase" /> containing the data get from the
    ///   <see cref="XmlElement" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="element" /> or <paramref name="wallpaperSettingsType" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="XmlException">
    ///   The XML Data are invalid.
    /// </exception>
    protected static WallpaperBase GetWallpaperDataFromXmlElement(XmlElement element, Type wallpaperSettingsType) {
      Contract.Requires<ArgumentNullException>(element != null);
      Contract.Requires<ArgumentNullException>(wallpaperSettingsType != null);

      WallpaperBase settings = null;
      WallpaperDefaultSettings defaultSettings = null;
      XmlElement subElement;

      if (wallpaperSettingsType == typeof(Wallpaper)) {
        subElement = element["ImagePath"];
        if (subElement != null) {
          Wallpaper wallpaper;

          if (subElement.InnerText.Length > 0)
            wallpaper = new Wallpaper(new Path(subElement.InnerText));
          else
            wallpaper = new Wallpaper();

          wallpaper.SuggestIsMultiscreen = false;
          wallpaper.SuggestPlacement = false;
          settings = wallpaper;
        }
      } else if (wallpaperSettingsType == typeof(WallpaperDefaultSettings)) {
        defaultSettings = new WallpaperDefaultSettings();
        settings = defaultSettings;
      }

      if (settings == null)
        throw new XmlException("A wallpaper setting node is missing.");

      subElement = element["IsActivated"];
      if (subElement != null)
        settings.IsActivated = bool.Parse(subElement.InnerText);

      subElement = element["IsMultiscreen"];
      if (subElement != null)
        settings.IsMultiscreen = bool.Parse(subElement.InnerText);

      subElement = element["Priority"];
      if (subElement != null)
        settings.Priority = byte.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["OnlyCycleBetweenStart"];
      if (subElement != null)
        settings.OnlyCycleBetweenStart = TimeSpan.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["OnlyCycleBetweenStop"];
      if (subElement != null)
        settings.OnlyCycleBetweenStop = TimeSpan.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["Placement"];
      if (subElement != null)
        settings.Placement = (WallpaperPlacement)Enum.Parse(typeof(WallpaperPlacement), subElement.InnerText);

      subElement = element["HorizontalOffset"];
      if (subElement != null) {
        int horizontalOffset = int.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

        subElement = element["VerticalOffset"];
        if (subElement != null)
          settings.Offset = new Point(horizontalOffset, int.Parse(subElement.InnerText, CultureInfo.InvariantCulture));
      }

      subElement = element["HorizontalScale"];
      if (subElement != null) {
        int horizontalScale = int.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

        subElement = element["VerticalScale"];
        if (subElement != null)
          settings.Offset = new Point(horizontalScale, int.Parse(subElement.InnerText, CultureInfo.InvariantCulture));
      }

      subElement = element["Effects"];
      if (subElement != null)
        settings.Effects = (WallpaperEffects)Enum.Parse(typeof(WallpaperEffects), subElement.InnerText);

      subElement = element["BackgroundColor"];
      if (subElement != null)
        settings.BackgroundColor = ColorTranslator.FromHtml(subElement.InnerText);

      subElement = element["DisabledScreens"];
      if (subElement != null) {
        string[] disabledScreens = subElement.InnerText.Split(',');

        for (int i = 0; i < disabledScreens.Length; i++) {
          string disabledScreenString = disabledScreens[i].Trim();

          if (disabledScreens[i].Length != 0)
            settings.DisabledScreens.Add(int.Parse(disabledScreenString, CultureInfo.InvariantCulture));
        }
      }

      if (defaultSettings != null) {
        subElement = element["AutoDetermineIsMultiscreen"];
        if (subElement != null)
          defaultSettings.AutoDetermineIsMultiscreen = bool.Parse(subElement.InnerText);

        subElement = element["AutoDeterminePlacement"];
        if (subElement != null)
          defaultSettings.AutoDeterminePlacement = bool.Parse(subElement.InnerText);
      }

      return settings;
    }

    /// <summary>
    ///   Writes the configuration data in the XML-format into the given given file using serialization.
    /// </summary>
    /// <remarks>
    ///   This method uses XML-Serialization to write version 2.0 Wallpaper Manager configuration data.
    /// </remarks>
    /// <param name="destStream">
    ///   The destination <see cref="Stream" /> to write into.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="destStream" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="IOException">
    ///   <paramref name="destStream" /> is not writeable.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   Error while serializing. Refer to the object returned by the <see cref="Exception.InnerException" /> property for
    ///   details.
    /// </exception>
    /// <seealso cref="Path">Path Class</seealso>
    public void Write(Stream destStream) {
      Contract.Requires<ArgumentNullException>(destStream != null);
      Contract.Requires<IOException>(destStream.CanWrite);

      XmlDocument document = new XmlDocument();
      XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
      document.AppendChild(declaration);

      XmlElement currentElement = document.CreateElement(Configuration.RootNodeName);
      currentElement.SetAttribute("Version", Configuration.DataVersion);
      currentElement.SetAttribute("xmlns", "http://www.WallpaperManager.de.vu/Configuration");
      document.AppendChild(currentElement);

      #region <General>
      XmlElement generalElement = document.CreateElement("General");
      document.DocumentElement.AppendChild(generalElement);

      currentElement = document.CreateElement("CycleAfterStartup");
      currentElement.InnerText = this.CycleAfterStartup.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("TerminateAfterStartup");
      currentElement.InnerText = this.TerminateAfterStartup.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("MinimizeAfterStart");
      currentElement.InnerText = this.MinimizeAfterStartup.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("StartAutoCyclingAfterStartup");
      currentElement.InnerText = this.StartAutocyclingAfterStartup.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("WallpaperChangeType");
      currentElement.InnerText = this.WallpaperChangeType.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("AutoCycleInterval");
      currentElement.InnerText = this.AutocycleInterval.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("LastActiveListSize");
      currentElement.InnerText = this.LastActiveListSize.ToString(CultureInfo.InvariantCulture);
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("CycleAfterDisplaySettingsChanged");
      currentElement.InnerText = this.CycleAfterDisplaySettingsChanged.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("MinimizeOnClose");
      currentElement.InnerText = this.MinimizeOnClose.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("DisplayCycleTimeAsIconOverlay");
      currentElement.InnerText = this.DisplayCycleTimeAsIconOverlay.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("WallpaperDoubleClickAction");
      currentElement.InnerText = this.WallpaperDoubleClickAction.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("TrayIconSingleClickAction");
      currentElement.InnerText = this.TrayIconSingleClickAction.ToString();
      generalElement.AppendChild(currentElement);

      currentElement = document.CreateElement("TrayIconDoubleClickAction");
      currentElement.InnerText = this.TrayIconDoubleClickAction.ToString();
      generalElement.AppendChild(currentElement);
      #endregion

      #region <ScreensSettings>
      XmlElement screensSettingsElement = document.CreateElement("ScreensSettings");
      document.DocumentElement.AppendChild(screensSettingsElement);

      foreach (ScreenSettings screenSettings in this.ScreensSettings) {
        XmlElement screenSettingsElement = document.CreateElement("ScreenSettings");
        screensSettingsElement.AppendChild(screenSettingsElement);

        currentElement = document.CreateElement("CycleRandomly");
        currentElement.InnerText = screenSettings.CycleRandomly.ToString();
        screenSettingsElement.AppendChild(currentElement);

        currentElement = document.CreateElement("MarginLeft");
        currentElement.InnerText = screenSettings.Margins.MarginLeft.ToString(CultureInfo.InvariantCulture);
        screenSettingsElement.AppendChild(currentElement);

        currentElement = document.CreateElement("MarginRight");
        currentElement.InnerText = screenSettings.Margins.MarginRight.ToString(CultureInfo.InvariantCulture);
        screenSettingsElement.AppendChild(currentElement);

        currentElement = document.CreateElement("MarginTop");
        currentElement.InnerText = screenSettings.Margins.MarginTop.ToString(CultureInfo.InvariantCulture);
        screenSettingsElement.AppendChild(currentElement);

        currentElement = document.CreateElement("MarginBottom");
        currentElement.InnerText = screenSettings.Margins.MarginBottom.ToString(CultureInfo.InvariantCulture);
        screenSettingsElement.AppendChild(currentElement);

        #region <OverlayTexts>
        XmlElement overlayTextsElement = document.CreateElement("OverlayTexts");
        screenSettingsElement.AppendChild(overlayTextsElement);

        foreach (WallpaperTextOverlay overlayText in screenSettings.TextOverlays) {
          XmlElement overlayTextElement = document.CreateElement("OverlayText");
          overlayTextsElement.AppendChild(overlayTextElement);

          currentElement = document.CreateElement("Format");
          currentElement.InnerText = overlayText.Format;
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("Position");
          currentElement.InnerText = overlayText.Position.ToString();
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("FontName");
          currentElement.InnerText = overlayText.FontName;
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("FontSize");
          currentElement.InnerText = overlayText.FontSize.ToString(CultureInfo.InvariantCulture);
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("FontStyle");
          currentElement.InnerText = overlayText.FontStyle.ToString();
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("ForeColor");
          currentElement.InnerText = ColorTranslator.ToHtml(overlayText.ForeColor);
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("BorderColor");
          currentElement.InnerText = ColorTranslator.ToHtml(overlayText.BorderColor);
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("HorizontalOffset");
          currentElement.InnerText = overlayText.HorizontalOffset.ToString(CultureInfo.InvariantCulture);
          overlayTextElement.AppendChild(currentElement);

          currentElement = document.CreateElement("VerticalOffset");
          currentElement.InnerText = overlayText.VerticalOffset.ToString(CultureInfo.InvariantCulture);
          overlayTextElement.AppendChild(currentElement);
        }
        #endregion

        currentElement = document.CreateElement("StaticWallpaper");
        Configuration.AddWallpaperDataToXmlElement(document, currentElement, screenSettings.StaticWallpaper);
        screenSettingsElement.AppendChild(currentElement);
      }
      #endregion

      #region <WallpaperCategories>
      XmlElement categoriesElement = document.CreateElement("WallpaperCategories");
      document.DocumentElement.AppendChild(categoriesElement);

      foreach (WallpaperCategory wallpaperCategory in this.WallpaperCategories) {
        WallpaperCategoryFileSynchronizer synchronizedWallpaperCategory = wallpaperCategory as WallpaperCategoryFileSynchronizer;
        XmlElement categoryElement;

        if (synchronizedWallpaperCategory != null)
          categoryElement = document.CreateElement("SynchronizedFolder");
        else
          categoryElement = document.CreateElement("Category");
        categoriesElement.AppendChild(categoryElement);

        currentElement = document.CreateElement("Name");
        currentElement.InnerText = wallpaperCategory.Name;
        categoryElement.AppendChild(currentElement);

        if (synchronizedWallpaperCategory != null) {
          currentElement = document.CreateElement("SynchronizedFolderPath");
          currentElement.InnerText = synchronizedWallpaperCategory.DirectoryPath;
          categoryElement.AppendChild(currentElement);
        }

        currentElement = document.CreateElement("WallpaperDefaultSettings");
        Configuration.AddWallpaperDataToXmlElement(document, currentElement, wallpaperCategory.WallpaperDefaultSettings);
        categoryElement.AppendChild(currentElement);

        XmlElement wallpapersElement = document.CreateElement("Wallpapers");
        categoryElement.AppendChild(wallpapersElement);
        foreach (Wallpaper wallpaper in wallpaperCategory) {
          currentElement = document.CreateElement("Wallpaper");
          Configuration.AddWallpaperDataToXmlElement(document, currentElement, wallpaper);
          wallpapersElement.AppendChild(currentElement);
        }
      }
      #endregion

      document.Save(destStream);
    }

    /// <summary>
    ///   Writes the configuration data in the XML-format into the given given file using serialization.
    /// </summary>
    /// <inheritdoc cref="Write(Stream)" select='remarks' />
    /// <param name="filePath">
    ///   The <see cref="Path" /> of the XML-file to be read from.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="filePath" /> is <c>Path.None</c>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///   <paramref name="filePath" /> points to a file in a directory which can not be found.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   The caller does not have the required permission.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///   Missing file-system related access rights to write the file located at <paramref name="filePath" />.
    /// </exception>
    /// <inheritdoc cref="Write(Stream)" select='exception[@cref="IOException"]|exception[@cref="InvalidOperationException"]' />
    /// <seealso cref="Path">Path Class</seealso>
    public void Write(Path filePath) {
      Contract.Requires<ArgumentException>(filePath != Path.None);

      using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        this.Write(fileStream);
    }

    /// <summary>
    ///   Adds wallpaper related settings to a given <see cref="XmlElement" />.
    /// </summary>
    /// <param name="document">
    ///   The <see cref="XmlDocument" /> to add the data for.
    /// </param>
    /// <param name="element">
    ///   The <see cref="XmlElement" /> to add the data to.
    /// </param>
    /// <param name="wallpaperBase">
    ///   The wallpaper settings to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="document" /> or <paramref name="element" /> or <paramref name="wallpaperBase" /> is
    ///   <c>null</c>.
    /// </exception>
    protected static void AddWallpaperDataToXmlElement(XmlDocument document, XmlElement element, WallpaperBase wallpaperBase) {
      Contract.Requires<ArgumentNullException>(document != null);
      Contract.Requires<ArgumentNullException>(element != null);
      Contract.Requires<ArgumentNullException>(wallpaperBase != null);

      Wallpaper wallpaperSettings = (wallpaperBase as Wallpaper);
      WallpaperDefaultSettings defaultSettings = (wallpaperBase as WallpaperDefaultSettings);
      XmlElement currentElement;

      if (wallpaperSettings != null) {
        currentElement = document.CreateElement("ImagePath");
        if (wallpaperSettings.ImagePath != Path.None)
          currentElement.InnerText = wallpaperSettings.ImagePath;
        element.AppendChild(currentElement);
      }

      currentElement = document.CreateElement("IsActivated");
      currentElement.InnerText = wallpaperBase.IsActivated.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("IsMultiscreen");
      currentElement.InnerText = wallpaperBase.IsMultiscreen.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("Priority");
      currentElement.InnerText = wallpaperBase.Priority.ToString(CultureInfo.InvariantCulture);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("OnlyCycleBetweenStart");
      currentElement.InnerText = wallpaperBase.OnlyCycleBetweenStart.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("OnlyCycleBetweenStop");
      currentElement.InnerText = wallpaperBase.OnlyCycleBetweenStop.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("Placement");
      currentElement.InnerText = wallpaperBase.Placement.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("HorizontalOffset");
      currentElement.InnerText = wallpaperBase.Offset.X.ToString(CultureInfo.InvariantCulture);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("VerticalOffset");
      currentElement.InnerText = wallpaperBase.Offset.Y.ToString(CultureInfo.InvariantCulture);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("HorizontalScale");
      currentElement.InnerText = wallpaperBase.Scale.X.ToString(CultureInfo.InvariantCulture);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("VerticalScale");
      currentElement.InnerText = wallpaperBase.Scale.Y.ToString(CultureInfo.InvariantCulture);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("Effects");
      currentElement.InnerText = wallpaperBase.Effects.ToString();
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("BackgroundColor");
      currentElement.InnerText = ColorTranslator.ToHtml(wallpaperBase.BackgroundColor);
      element.AppendChild(currentElement);

      currentElement = document.CreateElement("DisabledScreens");
      StringBuilder disabledScreensString = new StringBuilder(wallpaperBase.DisabledScreens.Count * 2);
      for (int i = 0; i < wallpaperBase.DisabledScreens.Count; i++) {
        if (i > 0)
          disabledScreensString.Append(',');

        disabledScreensString.Append(wallpaperBase.DisabledScreens[i]);
      }
      currentElement.InnerText = disabledScreensString.ToString();
      element.AppendChild(currentElement);

      if (defaultSettings != null) {
        currentElement = document.CreateElement("AutoDetermineIsMultiscreen");
        currentElement.InnerText = defaultSettings.AutoDetermineIsMultiscreen.ToString(CultureInfo.InvariantCulture);
        element.AppendChild(currentElement);

        currentElement = document.CreateElement("AutoDeterminePlacement");
        currentElement.InnerText = defaultSettings.AutoDeterminePlacement.ToString(CultureInfo.InvariantCulture);
        element.AppendChild(currentElement);
      }
    }*/

    /// <inheritdoc />
    public object Clone() {
      Configuration clone = (Configuration)this.MemberwiseClone();
      clone.ScreensSettings = (ScreenSettingsCollection)this.ScreensSettings.Clone();

      return clone;
    }

    /// <inheritdoc />
    public virtual void AssignTo(object other) {
      Contract.Requires<ArgumentException>(other is Configuration);

      Configuration otherInstance = (Configuration)other;
      otherInstance.CycleAfterStartup = this.CycleAfterStartup;
      otherInstance.TerminateAfterStartup = this.TerminateAfterStartup;
      otherInstance.StartAutocyclingAfterStartup = this.StartAutocyclingAfterStartup;
      otherInstance.MinimizeAfterStartup = this.MinimizeAfterStartup;
      otherInstance.WallpaperChangeType = this.WallpaperChangeType;
      otherInstance.AutocycleInterval = this.AutocycleInterval;
      otherInstance.LastActiveListSize = this.LastActiveListSize;
      otherInstance.CycleAfterDisplaySettingsChanged = this.CycleAfterDisplaySettingsChanged;
      otherInstance.MinimizeOnClose = this.MinimizeOnClose;
      otherInstance.DisplayCycleTimeAsIconOverlay = this.DisplayCycleTimeAsIconOverlay;
      otherInstance.WallpaperDoubleClickAction = this.WallpaperDoubleClickAction;
      otherInstance.TrayIconSingleClickAction = this.TrayIconSingleClickAction;
      otherInstance.TrayIconDoubleClickAction = this.TrayIconDoubleClickAction;
      otherInstance.ScreensSettings = (ScreenSettingsCollection)this.ScreensSettings.Clone();
    }
  }
}