// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Windows.Markup;
using Common.Presentation;
using WallpaperManager.Models;

namespace WallpaperManager.Views {
  /// <summary>
  ///   A <see cref="MarkupExtension" /> used to provide simple access to the application's localization database from the
  ///   graphical user interface.
  /// </summary>
  [MarkupExtensionReturnType(typeof(string))]
  public class LocalizedStringExtension : LocalizedStringExtensionBase {
    /// <summary>
    ///   Initializes a new instance of the <see cref="LocalizedStringExtension" /> class.
    /// </summary>
    public LocalizedStringExtension() : base(null) {}

    /// <inheritdoc />
    public LocalizedStringExtension(string entryName) : base(entryName) {
      if (string.IsNullOrWhiteSpace(entryName)) throw new ArgumentNullException();

      this.Context = LocalizationManager.DefaultContext;
    }

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
    protected override string ProvideString(string context, string entryName) {
      return LocalizationManager.GetLocalizedString(context, entryName);
    }
  }
}