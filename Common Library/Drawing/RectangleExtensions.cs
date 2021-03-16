using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Common.Drawing {
  /// <summary>
  ///   Provides extension methods for <see cref="Rectangle">Rectangles</see>.
  /// </summary>
  /// <threadsafety static="false" />
  public static class RectangleExtensions {
    #region Methods
    /// <summary>
    ///   Creates a new <see cref="Rectangle" /> scaled by the given <paramref name="factor"/>.
    /// </summary>
    /// <param name="source">The source <see cref="Rectangle" />.</param>
    /// <param name="factor">The scale factor.</param>
    /// <returns>
    ///   A new <see cref="Rectangle" /> scaled by the given <paramref name="factor" />.
    /// </returns>
    public static Rectangle Scale(this Rectangle source, Double factor) {
      source.Width = (Int32)(source.Width * factor);
      source.Height = (Int32)(source.Height * factor);

      return source;
    }

    /// <summary>
    ///   Creates a new <see cref="Rectangle" /> multiplicated by the given <paramref name="factor"/>.
    /// </summary>
    /// <param name="source">The source <see cref="Rectangle" />.</param>
    /// <param name="factor">The multiplication factor.</param>
    /// <returns>
    ///   A new <see cref="Rectangle" /> multiplicated by the given <paramref name="factor" />.
    /// </returns>
    public static Rectangle ScaleFull(this Rectangle source, Double factor) {
      source.X = (Int32)(source.X * factor);
      source.Y = (Int32)(source.Y * factor);
      source.Width = (Int32)(source.Width * factor);
      source.Height = (Int32)(source.Height * factor);

      return source;
    }

    /// <summary>
    ///   Creates a new <see cref="Rectangle" /> centered inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </summary>
    /// <param name="source">The source <see cref="Rectangle" />.</param>
    /// <param name="contentWidth">The width value of new <see cref="Rectangle" />.</param>
    /// <param name="contentHeight">The height value of new <see cref="Rectangle" />.</param>
    /// <returns>
    ///   A new <see cref="Rectangle" /> centered inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </returns>
    public static Rectangle Center(this Rectangle source, Int32 contentWidth, Int32 contentHeight) {
      Rectangle centered = new Rectangle();
      
      centered.X = (source.X + ((source.Width / 2) - (contentWidth / 2)));
      centered.Y = (source.Y + ((source.Height / 2) - (contentHeight / 2)));
      centered.Width = contentWidth;
      centered.Height = contentHeight;

      return centered;
    }

    private static Rectangle UniformInternal(Rectangle source, Int32 contentWidth, Int32 contentHeight, Boolean toFill) {
      Double horizontalFactor = ((Double)source.Width / contentWidth);
      Double verticalFactor = ((Double)source.Height / contentHeight);
      Double scaleFactor;

      if (!toFill) {
        if (horizontalFactor < verticalFactor) {
          scaleFactor = horizontalFactor;
        } else {
          scaleFactor = verticalFactor;
        }
      } else {
        if (horizontalFactor > verticalFactor) {
          scaleFactor = horizontalFactor;
        } else {
          scaleFactor = verticalFactor;
        }
      }
      
      return source.Center((Int32)(contentWidth * scaleFactor), (Int32)(contentHeight * scaleFactor));
    }

    /// <summary>
    ///   Creates a new <see cref="Rectangle" /> stretched (with keeping ratio) inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </summary>
    /// <inheritdoc cref="Center" />
    /// <returns>
    ///   A new <see cref="Rectangle" /> stretched (with keeping ratio) inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </returns>
    public static Rectangle Uniform(this Rectangle source, Int32 contentWidth, Int32 contentHeight) {
      return RectangleExtensions.UniformInternal(source, contentWidth, contentHeight, false);
    }
    #endregion

    /// <summary>
    ///   Creates a new <see cref="Rectangle" /> uniform to fill (with keeping ratio) inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </summary>
    /// <inheritdoc cref="Center" />
    /// <returns>
    ///   A new <see cref="Rectangle" /> uniform to fill (with keeping ratio) inside the 
    ///   <paramref name="source">source</paramref> <see cref="Rectangle" />.
    /// </returns>
    public static Rectangle UniformToFill(this Rectangle source, Int32 contentWidth, Int32 contentHeight) {
      return RectangleExtensions.UniformInternal(source, contentWidth, contentHeight, true);
    }
  }
}
