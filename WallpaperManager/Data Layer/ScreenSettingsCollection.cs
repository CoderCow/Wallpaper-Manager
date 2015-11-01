// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using Common;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Represents a collection of <see cref="ScreenSettings" /> objects.
  /// </summary>
  /// <remarks>
  ///   This collection is read only. It always contains the same count of <see cref="ScreenSettings" /> objects as screens
  ///   are available on the current computer.
  /// </remarks>
  /// <threadsafety static="false" instance="false" />
  public class ScreenSettingsCollection: ReadOnlyCollection<ScreenSettings>, ICloneable, IAssignable {
    #region Property: RandomCycledScreenCount
    /// <summary>
    ///   Gets the number of <see cref="ScreenSettings" /> objects where no static wallpaper should be cycled on.
    /// </summary>
    /// <value>
    ///   The number of <see cref="ScreenSettings" /> objects where no static wallpaper should be cycled on.
    /// </value>
    public Int32 RandomCycledScreenCount {
      get {
        Int32 count = 0;

        foreach (ScreenSettings screenSetting in this) {
          if (screenSetting.CycleRandomly) {
            count++;
          }
        }

        return count;
      }
    }
    #endregion

    #region Property: AllStatic
    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether all monitors should cycle static wallpapers or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether all monitors should cycle static wallpapers or not.
    /// </value>
    public Boolean AllStatic {
      get {
        Boolean allStatic = true;

        foreach (ScreenSettings screenSetting in this) {
          if (screenSetting.CycleRandomly) {
            allStatic = false;
            break;
          }
        }

        return allStatic;
      }
    }
    #endregion


    #region Methods: Constructor, RefreshBounds
    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenSettingsCollection" /> class.
    /// </summary>
    public ScreenSettingsCollection(): base(new ScreenSettings[Screen.AllScreens.Length]) {
      for (Int32 i = 0; i < Screen.AllScreens.Length; i++) {
        this.Items[i] = new ScreenSettings(i);
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ScreenSettingsCollection" /> class with a given collection of 
    ///   <see cref="ScreenSettings" /> instances.
    /// </summary>
    /// <param name="screenSettings">
    ///   A collection of <see cref="ScreenSettings" /> instances to be initially added.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The <paramref name="screenSettings" /> collection contains an amount of items which is not equal to the amount of
    ///   screens.
    /// </exception>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public ScreenSettingsCollection(IList<ScreenSettings> screenSettings): base(new ScreenSettings[Screen.AllScreens.Length]) {
      if (screenSettings.Count != Screen.AllScreens.Length) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCollectionItemsNotEqualToScreenCount("screenSettings"));
      }

      for (Int32 i = 0; i < Screen.AllScreens.Length; i++) {
        this.Items[i] = screenSettings[i];
      }
    }

    /// <summary>
    ///   Refreshes the cached <see cref="ScreenSettings.Bounds" /> and recalculates 
    ///   <see cref="ScreenSettings.BoundsWithMargin" /> of all <see cref="ScreenSettings" /> instances in this collection.
    /// </summary>
    public void RefreshBounds() {
      foreach (ScreenSettings screenSettings in this) {
        screenSettings.RefreshBounds();
      }
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <summary>
    ///   Assigns all member values of this instance to the respective members of the given instance.
    /// </summary>
    /// <param name="other">
    ///   The target instance to assign to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="other" /> is not castable to the <see cref="ScreenSettingsCollection" /> type.
    /// </exception>
    public void AssignTo(Object other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      ScreenSettingsCollection otherInstance = (ScreenSettingsCollection)other;
      if (otherInstance == null) {
        throw new ArgumentException(ExceptionMessages.GetTypeIsNotCastable("Object", "ScreenSettingsCollection", "other"));
      }

      for (Int32 i = 0; i < this.Count; i++) {
        if (i >= otherInstance.Count) {
          otherInstance.Items.Add(this[i]);
        } else {
          this[i].AssignTo(otherInstance[i]);
        }
      }
    }

    /// <inheritdoc />
    public Object Clone() {
      List<ScreenSettings> clonedScreensSettings = new List<ScreenSettings>(this.Count);

      for (Int32 i = 0; i < this.Count; i++) {
        clonedScreensSettings.Add((ScreenSettings)this[i].Clone());
      }

      return new ScreenSettingsCollection(clonedScreensSettings);
    }
    #endregion
  }
}
