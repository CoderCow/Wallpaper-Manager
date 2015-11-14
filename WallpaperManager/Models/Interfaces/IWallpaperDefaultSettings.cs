// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Diagnostics.Contracts;
using Common;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines data to be applied to newly added wallpapers.
  /// </summary>
  [ContractClass(typeof(IWallpaperDefaultSettingsContracts))]
  public interface IWallpaperDefaultSettings : ICloneable, IAssignable {
    WallpaperBase Settings { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperBase.IsMultiscreen" /> property should be
    ///   determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperBase.IsMultiscreen" /> property should be determined
    ///   automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDetermineIsMultiscreen { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperBase.Placement" />
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperBase.Placement" /> property should be
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    bool AutoDeterminePlacement { get; set; }
  }

  [ContractClassFor(typeof(IWallpaperDefaultSettings))]
  internal abstract class IWallpaperDefaultSettingsContracts: IWallpaperDefaultSettings {
    public abstract bool AutoDetermineIsMultiscreen { get; set; }
    public abstract bool AutoDeterminePlacement { get; set; }

    public WallpaperBase Settings {
      get {
        Contract.Ensures(Contract.Result<WallpaperBase>() != null);
        throw new NotImplementedException();
      }
      set {
        Contract.Requires<ArgumentNullException>(value != null);
        throw new NotImplementedException();
      }
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}