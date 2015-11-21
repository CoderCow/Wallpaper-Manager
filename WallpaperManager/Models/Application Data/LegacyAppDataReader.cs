using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Common.Windows;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  /// <summary>
  ///  Reads Wallpaper Manager 1.x and 2.x configuration files.
  /// </summary>
  public class LegacyAppDataReader: IApplicationDataReader {
    /// <summary>
    ///   Represents the name of the root node of the XML-data.
    /// </summary>
    public const string RootNodeName = "WallpaperManagerConfiguration";

    /// <summary>
    ///   Represents the XML namespace of the root node of the XML-data.
    /// </summary>
    public const string XmlNamespace = "http://www.WallpaperManager.de.vu";

    private readonly Path inputFileName;
    private readonly IDisplayInfo displayInfo;

    /// <param name="inputFileName">
    ///   The <see cref="Path" /> of the XML-file to be read from.
    /// </param>
    /// <param name="displayInfo">
    ///   The display information provider used to verify and correct screen settings.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="inputFileName" /> is <c>Path.Invalid</c>.
    /// </exception>
    public LegacyAppDataReader(Path inputFileName, IDisplayInfo displayInfo) {
      Contract.Requires<ArgumentException>(inputFileName != Path.Invalid);
      Contract.Requires<ArgumentNullException>(displayInfo != null);

      this.inputFileName = inputFileName;
      this.displayInfo = displayInfo;
    }

    /// <summary>
    ///   Creates a new <see cref="Configuration" /> instance by reading the data from a XML-file..
    /// </summary>
    /// <remarks>
    ///   This method reads Wallpaper Manager configuration files (version 1.0, 1.1 and 1.2) by using
    ///   <see cref="XmlDocument" />.
    /// </remarks>
    /// <returns>
    ///   A new <see cref="IApplicationData" /> object containing the read data.
    /// </returns>
    /// <inheritdoc />
    public IApplicationData Read() {
      try {
        using (FileStream fileStream = new FileStream(this.inputFileName, FileMode.Open)) {
          XmlDocument document = new XmlDocument();
          document.Load(fileStream);

          if ((document.DocumentElement == null) || (document.DocumentElement.Name != RootNodeName))
            throw new XmlException("The configuration file has an invalid root element.");

          XmlAttribute versionAttribute = document.DocumentElement.Attributes["Version"];
          Version configVersion;
          if ((versionAttribute == null) || (!Version.TryParse(versionAttribute.InnerText, out configVersion)))
            throw new XmlException("The configuration file has an invalid root element.");

          XmlElement generalElement = document.DocumentElement["General"];
          if (generalElement == null)
            throw new XmlException("The configuration file does not contain expected element 'General'.");

          IConfiguration resultingConfiguration = ConfigurationFromXmlElement(generalElement);

          XmlElement screensSettingsElement = document.DocumentElement["ScreenSettings"];
          if (screensSettingsElement != null)
            resultingConfiguration.ScreenSettings = this.ScreenSettingDictionaryFromXmlElement(screensSettingsElement);
          
          List<IWallpaperCategory> resultingCategories;
          Dictionary<IWallpaperCategory, Path> resultingCategoryFolderAssociations;
          XmlElement wallpaperCategoriesElement = document.DocumentElement["WallpaperCategories"];
          if (wallpaperCategoriesElement != null) {
            resultingCategories = this.WallpaperCategoryCollectionFromXmlElement(wallpaperCategoriesElement, out resultingCategoryFolderAssociations);
          } else {
            resultingCategories = new List<IWallpaperCategory>(0);
            resultingCategoryFolderAssociations = new Dictionary<IWallpaperCategory, Path>();
          }
          
          return new ApplicationData(resultingConfiguration, new ObservableCollection<IWallpaperCategory>(resultingCategories), resultingCategoryFolderAssociations);
        }
      } catch (IOException) {
        throw;
      } catch (Exception ex) {
        throw new IOException("An I/O error has occured. See inner exception for details.", ex);
      }
    }

    private static IConfiguration ConfigurationFromXmlElement(XmlElement generalElement) {
      IConfiguration resultingConfiguration = new Configuration();

      XmlElement element = generalElement["CycleAfterStartup"];
      if (element != null)
        resultingConfiguration.CycleAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["TerminateAfterStartup"];
      if (element != null)
        resultingConfiguration.TerminateAfterStartup = bool.Parse(element.InnerText);

      element = (generalElement["MinimizeAfterStart"] ?? generalElement["MinimizeAfterStartup"]);
      if (element != null)
        resultingConfiguration.MinimizeAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["StartAutoCyclingAfterStartup"];
      if (element != null)
        resultingConfiguration.StartAutocyclingAfterStartup = bool.Parse(element.InnerText);

      element = generalElement["WallpaperChangeType"];
      if (element != null)
        resultingConfiguration.WallpaperChangeType = (WallpaperChangeType)Enum.Parse(typeof(WallpaperChangeType), element.InnerText);

      element = generalElement["AutoCycleInterval"];
      if (element != null)
        resultingConfiguration.AutocycleInterval = TimeSpan.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = generalElement["CycleAfterDisplaySettingsChanged"];
      if (element != null)
        resultingConfiguration.CycleAfterDisplaySettingsChanged = bool.Parse(element.InnerText);

      element = generalElement["MinimizeOnClose"];
      if (element != null)
        resultingConfiguration.MinimizeOnClose = bool.Parse(element.InnerText);

      element = generalElement["DisplayCycleTimeAsIconOverlay"];
      if (element != null)
        resultingConfiguration.DisplayCycleTimeAsIconOverlay = bool.Parse(element.InnerText);

      element = generalElement["WallpaperDoubleClickAction"];
      if (element != null)
        resultingConfiguration.WallpaperDoubleClickAction = (WallpaperClickAction)Enum.Parse(typeof(WallpaperClickAction), element.InnerText);

      element = generalElement["TrayIconSingleClickAction"];
      if (element != null)
        resultingConfiguration.TrayIconSingleClickAction = (TrayIconClickAction)Enum.Parse(typeof(TrayIconClickAction), element.InnerText);

      element = generalElement["TrayIconDoubleClickAction"];
      if (element != null)
        resultingConfiguration.TrayIconDoubleClickAction = (TrayIconClickAction)Enum.Parse(typeof(TrayIconClickAction), element.InnerText);

      return resultingConfiguration;
    }

    private Dictionary<string, IScreenSettings> ScreenSettingDictionaryFromXmlElement(XmlElement screensSettingsElement) {
      var resultingScreenSettings = new Dictionary<string, IScreenSettings>();

      int settingsCounter = 0;
      foreach (XmlElement screenSettingsElement in screensSettingsElement) {
        if (screenSettingsElement.Name != "ScreenSettings")
          continue;

        // ScreenSettings in the old configuration file were identified by screen indexes only, thus if there are more displays configured than actually connected to the computer
        // we can just skip the remaining settings.
        int screenIndex = settingsCounter;
        if (screenIndex >= this.displayInfo.Displays.Count)
          break;

        IScreenSettings screenSettings = this.ScreenSettingsFromXmlElement(screenSettingsElement);
        string deviceId = this.displayInfo.Displays[screenIndex].UniqueDeviceId;
        resultingScreenSettings.Add(deviceId, screenSettings);

        settingsCounter++;
      }

      return resultingScreenSettings;
    }

    private IScreenSettings ScreenSettingsFromXmlElement(XmlElement screenSettingsElement) {
      IScreenSettings screenSettings = new ScreenSettings();
      XmlElement element = screenSettingsElement["CycleRandomly"];
      if (element != null)
        screenSettings.CycleRandomly = bool.Parse(element.InnerText);

      element = screenSettingsElement["MarginLeft"];
      if (element != null)
        screenSettings.MarginLeft = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = screenSettingsElement["MarginRight"];
      if (element != null)
        screenSettings.MarginRight = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = screenSettingsElement["MarginTop"];
      if (element != null)
        screenSettings.MarginTop = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

      element = screenSettingsElement["MarginBottom"];
      if (element != null)
        screenSettings.MarginBottom = int.Parse(element.InnerText, CultureInfo.InvariantCulture);

      XmlElement overlayTextsElement = screenSettingsElement["OverlayTexts"];
      if (overlayTextsElement != null) {
        foreach (XmlElement overlayTextElement in overlayTextsElement) {
          if (overlayTextElement.Name != "OverlayText")
            continue;

          ITextOverlay textOverlay = OverlayTextFromXmlElement(overlayTextsElement);
          screenSettings.TextOverlays.Add(textOverlay);
        }
      }

      XmlElement staticWallpaperElement = screenSettingsElement["StaticWallpaper"];
      if (staticWallpaperElement != null)
        screenSettings.StaticWallpaper = this.WallpaperFromXmlElement(staticWallpaperElement);

      return screenSettings;
    }

    private static ITextOverlay OverlayTextFromXmlElement(XmlElement overlayTextElement) {
      ITextOverlay textOverlay = new TextOverlay();

      XmlElement element = overlayTextElement["Format"];
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

      return textOverlay;
    }

    private List<IWallpaperCategory> WallpaperCategoryCollectionFromXmlElement(XmlElement wallpaperCategoriesElement, out Dictionary<IWallpaperCategory, Path> categoryFolderAssociations) {
      List<IWallpaperCategory> categories = new List<IWallpaperCategory>(wallpaperCategoriesElement.ChildNodes.Count);
      categoryFolderAssociations = new Dictionary<IWallpaperCategory, Path>();

      foreach (XmlElement wallpaperCategoryElement in wallpaperCategoriesElement) {
        bool isSynchronizedFolder = ((wallpaperCategoryElement.Name == "SynchronizedFolder") || (wallpaperCategoryElement.Name == "WatchedCategory"));
        if (wallpaperCategoryElement.Name != "Category" && !isSynchronizedFolder)
          continue;

        XmlElement element = wallpaperCategoryElement["Name"];
        if (element == null)
          continue;

        string categoryName = element.InnerText;

        IWallpaperDefaultSettings defaultSettings = null;
        element = wallpaperCategoryElement["WallpaperDefaultSettings"];
        if (element != null)
          defaultSettings = this.WallpaperDefaultSettingsFromXmlElement(element);

        List<IWallpaper> wallpapers;
        XmlElement wallpapersElement = wallpaperCategoryElement["Wallpapers"];
        if (wallpapersElement != null) {
          wallpapers = new List<IWallpaper>(wallpapersElement.ChildNodes.Count);

          foreach (XmlElement wallpaperElement in wallpapersElement) {
            if (wallpaperElement.Name != "Wallpaper")
              continue;

            IWallpaper wallpaper = this.WallpaperFromXmlElement(wallpaperElement);
            wallpapers.Add(wallpaper);
          }
        } else
          wallpapers = new List<IWallpaper>(0);

        IWallpaperCategory category;
        if (isSynchronizedFolder) {
          element = (wallpaperCategoryElement["SynchronizedFolderPath"] ?? wallpaperCategoryElement["WatchedDirectoryPath"]);
          Contract.Assert(element != null);

          Path synchronizedDirPath = new Path(element.InnerText);
          if (!Directory.Exists(synchronizedDirPath))
            continue;

          category = new WallpaperCategory(categoryName, defaultSettings, wallpapers);
          categoryFolderAssociations.Add(category, synchronizedDirPath);
        } else
          category = new WallpaperCategory(categoryName, defaultSettings, wallpapers);

        category.WallpaperDefaultSettings = defaultSettings;
      }

      categories.Capacity = categories.Count;
      return categories;
    }

    /// <summary>
    ///   Gets wallpaper related settings from a given <see cref="XmlElement" />.
    /// </summary>
    /// <param name="xmlElement">
    ///   The <see cref="XmlElement" /> to get the data from.
    /// </param>
    /// <returns>
    ///   An instance of a type inherited from <see cref="WallpaperBase" /> containing the data get from the
    ///   <see cref="XmlElement" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="xmlElement" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="XmlException">
    ///   The XML Data are invalid.
    /// </exception>
    private IWallpaper WallpaperFromXmlElement(XmlElement xmlElement) {
      XmlElement subElement = xmlElement["ImagePath"];
      IWallpaper wallpaper = new Wallpaper(new Path(subElement.InnerText));

      this.AssignWallpaperBaseFromXmlElement(xmlElement, wallpaper);

      return wallpaper;
    }

    private IWallpaperDefaultSettings WallpaperDefaultSettingsFromXmlElement(XmlElement xmlElement) {
      Wallpaper baseSettings = new Wallpaper(new Path("dummypath"));
      this.AssignWallpaperBaseFromXmlElement(xmlElement, baseSettings);

      IWallpaperDefaultSettings resultingSettings = new WallpaperDefaultSettings(baseSettings, this.displayInfo);
    
      XmlElement subElement;
      subElement = xmlElement["AutoDetermineIsMultiscreen"];
      if (subElement != null)
        resultingSettings.AutoDetermineIsMultiscreen = bool.Parse(subElement.InnerText);

      subElement = xmlElement["AutoDeterminePlacement"];
      if (subElement != null)
        resultingSettings.AutoDeterminePlacement = bool.Parse(subElement.InnerText);

      return resultingSettings;
    }

    private void AssignWallpaperBaseFromXmlElement(XmlElement element, IWallpaperBase wallpaperBase) {
      XmlElement subElement;
      subElement = element["IsActivated"];
      if (subElement != null)
        wallpaperBase.IsActivated = bool.Parse(subElement.InnerText);

      subElement = element["IsMultiscreen"];
      if (subElement != null)
        wallpaperBase.IsMultiscreen = bool.Parse(subElement.InnerText);

      subElement = element["Priority"];
      if (subElement != null)
        wallpaperBase.Priority = byte.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["OnlyCycleBetweenStart"];
      if (subElement != null)
        wallpaperBase.OnlyCycleBetweenStart = TimeSpan.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["OnlyCycleBetweenStop"];
      if (subElement != null)
        wallpaperBase.OnlyCycleBetweenStop = TimeSpan.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

      subElement = element["Placement"];
      if (subElement != null)
        wallpaperBase.Placement = (WallpaperPlacement)Enum.Parse(typeof(WallpaperPlacement), subElement.InnerText);

      subElement = element["HorizontalOffset"];
      if (subElement != null) {
        int horizontalOffset = int.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

        subElement = element["VerticalOffset"];
        if (subElement != null)
          wallpaperBase.Offset = new Point(horizontalOffset, int.Parse(subElement.InnerText, CultureInfo.InvariantCulture));
      }

      subElement = element["HorizontalScale"];
      if (subElement != null) {
        int horizontalScale = int.Parse(subElement.InnerText, CultureInfo.InvariantCulture);

        subElement = element["VerticalScale"];
        if (subElement != null)
          wallpaperBase.Offset = new Point(horizontalScale, int.Parse(subElement.InnerText, CultureInfo.InvariantCulture));
      }

      subElement = element["Effects"];
      if (subElement != null)
        wallpaperBase.Effects = (WallpaperEffects)Enum.Parse(typeof(WallpaperEffects), subElement.InnerText);

      subElement = element["BackgroundColor"];
      if (subElement != null)
        wallpaperBase.BackgroundColor = ColorTranslator.FromHtml(subElement.InnerText);

      subElement = element["DisabledScreens"];
      if (subElement != null) {
        string[] disabledScreens = subElement.InnerText.Split(',');

        for (int i = 0; i < disabledScreens.Length; i++) {
          string disabledScreenString = disabledScreens[i].Trim();

          if (disabledScreenString.Length > 0) {
            int disabledScreenIndex = int.Parse(disabledScreenString, CultureInfo.InvariantCulture);
            if (disabledScreenIndex < this.displayInfo.Displays.Count)
              wallpaperBase.DisabledDevices.Add(this.displayInfo.Displays[disabledScreenIndex].UniqueDeviceId);
          }
        }
      }
    }
  }
}
