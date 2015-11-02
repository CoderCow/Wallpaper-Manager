// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace WallpaperManager.Views {
  /// <summary>
  ///   The General Settings <see cref="Page" /> used by the <see cref="ConfigWindow">Configuration Window</see>.
  /// </summary>
  /// <seealso cref="ConfigWindow">ConfigWindow Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class GeneralSettingsPage : Page {
    /// <summary>
    ///   Initializes a new instance of the <see cref="GeneralSettingsPage" /> class.
    /// </summary>
    public GeneralSettingsPage() {
      this.InitializeComponent();
    }
  }
}