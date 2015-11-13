// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common.IO;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines general wallpaper related data.
  /// </summary>
  [ContractClass(typeof(IWallpaperContracts))]
  public interface IWallpaper : IWallpaperBase {
    bool IsImageSizeResolved { get; }

    /// <summary>
    ///   Gets or sets the path of the image file of this wallpaper.
    /// </summary>
    /// <value>
    ///   The path of the image file of this wallpaper.
    /// </value>
    Path ImagePath { get; set; }

    /// <summary>
    ///   Gets or sets the size of the image where <see cref="ImagePath" /> is reffering to.
    /// </summary>
    /// <value>
    ///   The size of the image where <see cref="ImagePath" /> is reffering to.
    /// </value>
    Size? ImageSize { get; set; }

    DateTime TimeLastCycled { get; set; }
    DateTime TimeAdded { get; set; }
    int CycleCountWeek { get; set; }
    int CycleCountTotal { get; set; }
  }

  [ContractClassFor(typeof(IWallpaper))]
  internal abstract class IWallpaperContracts : IWallpaper {
    public abstract bool IsActivated { get; set; }
    public abstract bool IsMultiscreen { get; set; }
    public abstract byte Priority { get; set; }
    public abstract TimeSpan OnlyCycleBetweenStart { get; set; }
    public abstract TimeSpan OnlyCycleBetweenStop { get; set; }
    public abstract WallpaperPlacement Placement { get; set; }
    public abstract Point Offset { get; set; }
    public abstract Point Scale { get; set; }
    public abstract WallpaperEffects Effects { get; set; }
    public abstract Color BackgroundColor { get; set; }
    public abstract ICollection<int> DisabledScreens { get; }
    public abstract bool IsBlank { get; set; }
    public abstract bool SuggestIsMultiscreen { get; set; }
    public abstract bool SuggestPlacement { get; set; }
    public abstract DateTime TimeLastCycled { get; set; }
    public abstract DateTime TimeAdded { get; set; }

    public bool IsImageSizeResolved { 
      get {
        Contract.Ensures(Contract.Result<bool>() == (this.ImageSize != null));
        throw new NotImplementedException();
      }
    }

    public Path ImagePath {
      get { throw new NotImplementedException(); }
      set {
        Contract.Requires(value != Path.Invalid);
        throw new NotImplementedException();
      }
    }

    public Size? ImageSize {
      get { throw new NotImplementedException(); }
      set {
        Contract.Requires(value == null || (value.Value.Width > 0 && value.Value.Height > 0));
        throw new NotImplementedException();
      }
    }

    public int CycleCountWeek {
      get { throw new NotImplementedException(); }
      set {
        Contract.Requires(value >= 0);
        throw new NotImplementedException();
      }
    }

    public int CycleCountTotal {
      get { throw new NotImplementedException(); }
      set {
        Contract.Requires(value >= 0);  
        throw new NotImplementedException();
      }
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}