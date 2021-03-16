using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Common.Drawing {
  /// <summary>
  ///   Provides extension methods for <see cref="Graphics" /> objects.
  /// </summary>
  /// <threadsafety static="false" />
  public static class GraphicsExtensions {
    #region Methods: DrawImageEx
    public static void DrawImageEx(this Graphics destGraphics, Image image, Rectangle destRect) {
      
    }

    public static void DrawImageEx(
      this Graphics destGraphics, Image image, Rectangle destRect, Int32 horizontalScale, Int32 verticalScale
    ) {
      
    }

    /// <summary>
    ///   Draws an image object centered into the given rectangle.
    /// </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     If the image is too large to fit into the destination rectangle it will overflow. Use 
    ///     <see cref="Graphics.SetClip(Rectangle)" /> before executing this method to draw it clipped.
    ///   </note>
    /// </remarks>
    /// <param name="destGraphics">The destination graphics object.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="destRectangle">The destination rectangle.</param>
    public static void DrawImageCentered(this Graphics destGraphics, Image image, Rectangle destRectangle) {
      destGraphics.DrawImage(image, destRectangle.Center(image.Width, image.Height));
    }

    /// <summary>
    ///   Draws an image object uniformed into the given rectangle.
    /// </summary>
    /// <inheritdoc cref="DrawImageCentered" />
    public static void DrawImageUniformed(this Graphics destGraphics, Image image, Rectangle destRectangle) {
      destGraphics.DrawImage(image, destRectangle.Uniform(image.Width, image.Height));
    }

    /// <summary>
    ///   Draws an image object uniform to fill into the given rectangle.
    /// </summary>
    /// <inheritdoc cref="DrawImageCentered" />
    public static void DrawImageUniformedToFill(this Graphics destGraphics, Image image, Rectangle destRectangle) {
      destGraphics.DrawImage(image, destRectangle.UniformToFill(image.Width, image.Height));
    }

    /// <summary>
    ///   Draws an image object tiled into the given rectangle.
    /// </summary>
    /// <inheritdoc cref="DrawImageCentered" />
    public static void DrawImageTiled(this Graphics destGraphics, Image image, Rectangle destRectangle) {
      for (Int32 x = destRectangle.Left; x < destRectangle.Right; x += image.Width) {
        for (Int32 y = destRectangle.Top; y < destRectangle.Bottom; y += image.Height) {
          destGraphics.DrawImage(image, x, y, image.Width, image.Height);
        }
      }
    }
    #endregion
  }
}
