// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Provides methods to register and access localization databases.
  /// </summary>
  public static class LocalizationManager {
    #region Constants: DefaultContext
    /// <summary>
    ///   Represents the default context used to localized the strings.
    /// </summary>
    public const String DefaultContext = "Wallpaper Manager";
    #endregion

    #region Fields: databases
    /// <summary>
    ///   The dictionary holding the <see cref="ResourceManager" /> instances identified by their localization context.
    /// </summary>
    private static readonly Dictionary<String,ResourceManager> databases;
    #endregion

    #region Constructor
    static LocalizationManager() {
      LocalizationManager.databases = new Dictionary<String,ResourceManager>();
      LocalizationManager.RegisterContext(
        "Wallpaper Manager", new ResourceManager("WallpaperManager.Presentation_Layer.Resources.Localization.LocalizationData", Assembly.GetAssembly(typeof(LocalizationManager)))
      );
    }
    #endregion

    #region Methods: RegisterContext, GetLocalizedString
    /// <summary>
    ///   Registers a new localization context by using the given <see cref="ResourceManager" /> as database accessor.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///   The given <paramref name="context" /> is invalid.
    /// </exception>
    public static void RegisterContext(String context, ResourceManager resourceManager) {
      Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(context));
      Contract.Requires<ArgumentNullException>(resourceManager != null);

      LocalizationManager.databases.Add(context, resourceManager);
    }

    /// <summary>
    ///   Gets a localized string from the database registered witht he given localization context.
    /// </summary>
    /// <param name="context">
    ///   The context database where the localized string should be get from.
    /// </param>
    /// <param name="entryName">
    ///   The name of the entry in the localization database.
    /// </param>
    /// <returns>
    ///   The localized string.
    /// </returns>
    public static String GetLocalizedString(String context, String entryName) {
      Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(context));
      Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(entryName));

      ResourceManager resourceManager;
      if (!LocalizationManager.databases.TryGetValue(context, out resourceManager)) {
        throw new ArgumentException(
          String.Concat("The given serialization context is not registered with a database.\nContext: ", context), "context"
        );
      }

      String localizedString = resourceManager.GetString(entryName, CultureInfo.CurrentCulture);
      if (localizedString == null) {
        throw new InvalidOperationException(String.Format(
          "The given localization entry name could not be found either in the selected and the neutral localization database.\nEntry Name: {0}\nContext: {1}", 
          entryName, context
        ));
      }

      return localizedString;
    }

    /// <inheritdoc />
    public static String GetLocalizedString(String entryName) {
      Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(entryName));

      return LocalizationManager.GetLocalizedString(LocalizationManager.DefaultContext, entryName);
    }
    #endregion
  }
}