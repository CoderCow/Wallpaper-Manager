using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  public class LegacyAppDataReader: IApplicationDataReader {
    private readonly Path inputFileName;

    /// <param name="inputFileName">
    ///   The <see cref="Path" /> of the XML-file to be read from.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="inputFileName" /> is <c>Path.Invalid</c>.
    /// </exception>
    public LegacyAppDataReader(Path inputFileName) {
      Contract.Requires<ArgumentException>(inputFileName != Path.Invalid);

      this.inputFileName = inputFileName;
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
    /// <seealso cref="Path">Path Class</seealso>
    public IApplicationData Read() {
      try {
        using (FileStream fileStream = new FileStream(this.inputFileName, FileMode.Open)) {
          XmlDocument document = new XmlDocument();
          document.Load(fileStream);

          if ((document.DocumentElement == null) || (document.DocumentElement.Name != Configuration.RootNodeName))
            throw new XmlException("The configuration file has an invalid root element.");

          XmlAttribute versionAttribute = document.DocumentElement.Attributes["Version"];
          Version configVersion;
          if ((versionAttribute == null) || (!Version.TryParse(versionAttribute.InnerText, out configVersion)))
            throw new XmlException("The configuration file has an invalid root element.");

          Configuration configuration = new Configuration();
          /*
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

              #region <OverlayTexts>
              XmlElement overlayTextsElement = screenSettingsElement["OverlayTexts"];
              if (overlayTextsElement != null) {
                foreach (XmlElement overlayTextElement in overlayTextsElement) {
                  if (overlayTextElement.Name != "OverlayText")
                    continue;
                  TextOverlay textOverlay = new TextOverlay();

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
                screenSettings.StaticWallpaper = (Wallpaper)GetWallpaperDataFromXmlElement(staticWallpaperElement, typeof(Wallpaper));
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
                wallpaperCategoryElement.Name != "WatchedCategory")
                continue;

              element = wallpaperCategoryElement["Name"];
              if (element == null)
                continue;
              string categoryName = element.InnerText;

              WallpaperDefaultSettings defaultSettings = null;
              element = wallpaperCategoryElement["WallpaperDefaultSettings"];
              if (element != null)
                defaultSettings = (WallpaperDefaultSettings)GetWallpaperDataFromXmlElement(element, typeof(WallpaperDefaultSettings));

              #region <Wallpapers>
              List<Wallpaper> wallpapers;

              XmlElement wallpapersElement = wallpaperCategoryElement["Wallpapers"];
              if (wallpapersElement != null) {
                wallpapers = new List<Wallpaper>(wallpapersElement.ChildNodes.Count);

                foreach (XmlElement wallpaperElement in wallpapersElement) {
                  if (wallpaperElement.Name != "Wallpaper")
                    continue;

                  Wallpaper wallpaper = (Wallpaper)GetWallpaperDataFromXmlElement(wallpaperElement, typeof(Wallpaper));
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
          */

          return new ApplicationData(null, null);
        }
      } catch (IOException) {
        throw;
      } catch (Exception ex) {
        throw new IOException("An I/O error has occured. See inner exception for details.", ex);
      }
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

      /*
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
      }*/

      return settings;
    }
  }
}
