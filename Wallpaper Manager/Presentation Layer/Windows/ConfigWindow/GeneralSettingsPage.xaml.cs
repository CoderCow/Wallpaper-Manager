// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Windows.Controls;

namespace WallpaperManager.Presentation {
  /// <summary>
  ///   The General Settings <see cref="Page" /> used by the <see cref="ConfigWindow">Configuration Window</see>.
  /// </summary>
  /// <seealso cref="ConfigWindow">ConfigWindow Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public partial class GeneralSettingsPage: Page {
    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="GeneralSettingsPage" /> class.
    /// </summary>
    public GeneralSettingsPage() {
      this.InitializeComponent();
    }
    #endregion
  }
}
