// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WallpaperManager.Models;

namespace WallpaperManager.Views {
  /// <summary>
  ///   Converts a <see cref="WallpaperPlacement" /> value to a string.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  [ValueConversion(typeof(WallpaperPlacement), typeof(string))]
  public class WallpaperPlacementConverter : IValueConverter {
    /// <summary>
    ///   Represents the string representation for the <see cref="WallpaperPlacement.Uniform" /> value.
    /// </summary>
    private const string UniformString = "Uniform";

    /// <summary>
    ///   Represents the string representation for the <see cref="WallpaperPlacement.UniformToFill" /> value.
    /// </summary>
    private const string UniformToFillString = "Uniform to fill";

    /// <summary>
    ///   Represents the string representation for the <see cref="WallpaperPlacement.Stretch" /> value.
    /// </summary>
    private const string StretchString = "Stretch";

    /// <summary>
    ///   Represents the string representation for the <see cref="WallpaperPlacement.Center" /> value.
    /// </summary>
    private const string CenterString = "Center";

    /// <summary>
    ///   Represents the string representation for the <see cref="WallpaperPlacement.Tile" /> value.
    /// </summary>
    private const string TileString = "Tile";

    /// <summary>
    ///   Converts a <see cref="WallpaperPlacement" /> value to a string.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert" />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value == null)
        return DependencyProperty.UnsetValue;

      switch ((WallpaperPlacement)value) {
        case WallpaperPlacement.Uniform:
          return WallpaperPlacementConverter.UniformString;
        case WallpaperPlacement.UniformToFill:
          return WallpaperPlacementConverter.UniformToFillString;
        case WallpaperPlacement.Stretch:
          return WallpaperPlacementConverter.StretchString;
        case WallpaperPlacement.Center:
          return WallpaperPlacementConverter.CenterString;
        case WallpaperPlacement.Tile:
          return WallpaperPlacementConverter.TileString;
        default:
          return DependencyProperty.UnsetValue;
      }
    }

    /// <summary>
    ///   Converts a string value to <see cref="WallpaperPlacement" />.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack" />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}