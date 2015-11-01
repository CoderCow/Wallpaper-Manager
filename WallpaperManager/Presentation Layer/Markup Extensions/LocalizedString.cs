// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Diagnostics.Contracts;
using System.Windows.Markup;

using Common.Presentation;

using WallpaperManager.Data;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   A <see cref="MarkupExtension" /> used to provide simple access to the application's localization database from the
  ///   graphical user interface.
  /// </summary>
  [MarkupExtensionReturnType(typeof(String))]
  public class LocalizedStringExtension: LocalizedStringExtensionBase {
    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="LocalizedStringExtension" /> class.
    /// </summary>
    public LocalizedStringExtension(): base(null) {}

    /// <inheritdoc />
    public LocalizedStringExtension(String entryName): base(entryName) {
      Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(entryName));

      this.Context = LocalizationManager.DefaultContext;
    }
    #endregion
    
    #region Method: ProvideString
    /// <summary>
    ///   Provides an requested localized string.
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
    protected override String ProvideString(String context, String entryName) {
      return LocalizationManager.GetLocalizedString(context, entryName);
    }
    #endregion
  }
}