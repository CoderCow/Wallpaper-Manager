// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Provides methods to register and access localization databases.
  /// </summary>
  public static class LocalizationManager {
    /// <summary>
    ///   Represents the default context used to localized the strings.
    /// </summary>
    public const string DefaultContext = "Wallpaper Manager";

    /// <summary>
    ///   The dictionary holding the <see cref="ResourceManager" /> instances identified by their localization context.
    /// </summary>
    private static readonly Dictionary<string, ResourceManager> databases;

    static LocalizationManager() {
      LocalizationManager.databases = new Dictionary<string, ResourceManager>();
      LocalizationManager.RegisterContext(
        "Wallpaper Manager", new ResourceManager("WallpaperManager.Locale.LocalizationData", Assembly.GetAssembly(typeof(LocalizationManager))));
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private static void CheckInvariants() {
      Contract.Invariant(LocalizationManager.databases != null);
    }

    /// <summary>
    ///   Registers a new localization context by using the given <see cref="ResourceManager" /> as database accessor.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///   The given <paramref name="context" /> is invalid.
    /// </exception>
    public static void RegisterContext(string context, ResourceManager resourceManager) {
      Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(context));
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
    public static string GetLocalizedString(string context, string entryName) {
      Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(context));
      Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(entryName));

      ResourceManager resourceManager;
      if (!LocalizationManager.databases.TryGetValue(context, out resourceManager))
        throw new ArgumentException(string.Concat("The given serialization context is not registered with a database.\nContext: ", context), nameof(context));

      string localizedString = resourceManager.GetString(entryName, CultureInfo.CurrentCulture);
      if (localizedString == null) {
        throw new InvalidOperationException(string.Format(
          "The given localization entry name could not be found either in the selected and the neutral localization database.\nEntry Name: {0}\nContext: {1}", entryName, context));
      }

      return localizedString;
    }

    /// <inheritdoc />
    public static string GetLocalizedString(string entryName) {
      Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(entryName));

      return LocalizationManager.GetLocalizedString(LocalizationManager.DefaultContext, entryName);
    }
  }
}