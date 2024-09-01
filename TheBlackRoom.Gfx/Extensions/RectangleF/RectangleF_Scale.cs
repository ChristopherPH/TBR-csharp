/*
 * Copyright (c) 2024 Christopher Hayes
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Drawing;

namespace TheBlackRoom.Gfx.Extensions
{
    public static class RectangleF_Scale
    {
        /// <summary>
        /// Scales a rectangle to fit within the bounds of another rectangle, while maintaining aspect ratio
        /// </summary>
        /// <param name="SrcRect">Rectangle to scale</param>
        /// <param name="Bounds">Bounds to scale into</param>
        /// <param name="scaleFactor">Saved scale factor</param>
        /// <returns>Scaled rectangle</returns>
        public static RectangleF ScaleAspect(this RectangleF SrcRect, RectangleF Bounds, out float scaleFactor)
        {
            var scaleHeight = Bounds.Height / SrcRect.Height;
            var scaleWidth = Bounds.Width / SrcRect.Width;
            scaleFactor = Math.Min(scaleHeight, scaleWidth);

            var height = SrcRect.Height * scaleFactor;
            var width = SrcRect.Width * scaleFactor;

            var scaledRect = new RectangleF(
                    (Bounds.Width / 2) - (width / 2),
                    (Bounds.Height / 2) - (height / 2),
                    width,
                    height
                );

            return scaledRect;
        }

        /// <summary>
        /// Scales a rectangle to fit within the bounds of another rectangle, while maintaining aspect ratio
        /// </summary>
        /// <param name="SrcRect">Rectangle to scale</param>
        /// <param name="Bounds">Bounds to scale into</param>
        /// <returns>Scaled rectangle</returns>
        public static RectangleF ScaleAspect(this RectangleF SrcRect, RectangleF Bounds)
        {
            return ScaleAspect(SrcRect, Bounds, out var scaleFactor);
        }


        /// <summary>
        /// Aligns a rectangle to another rectangle given an alignment
        /// </summary>
        /// <param name="SrcRect">Rectangle to align</param>
        /// <param name="OtherRect">Rectangle to align to</param>
        /// <param name="Alignmment">Location to align to</param>
        /// <returns>Aligned rectangle</returns>
        public static RectangleF Align(this RectangleF SrcRect, RectangleF OtherRect, ContentAlignment Alignmment)
        {
            switch (Alignmment)
            {
                default:
                case ContentAlignment.TopLeft:
                    return new RectangleF(
                        OtherRect.X,
                        OtherRect.Y,
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.TopCenter:
                    return new RectangleF(
                        OtherRect.X + (OtherRect.Width / 2) - (SrcRect.Width / 2),
                        OtherRect.Y,
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.TopRight:
                    return new RectangleF(
                        OtherRect.Right - SrcRect.Width,
                        OtherRect.Y,
                        SrcRect.Width, SrcRect.Height);


                case ContentAlignment.MiddleLeft:
                    return new RectangleF(
                        OtherRect.X,
                        OtherRect.Y + (OtherRect.Height / 2) - (SrcRect.Height / 2),
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.MiddleCenter:
                    return new RectangleF(
                        OtherRect.X + (OtherRect.Width / 2) - (SrcRect.Width / 2),
                        OtherRect.Y + (OtherRect.Height / 2) - (SrcRect.Height / 2),
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.MiddleRight:
                    return new RectangleF(
                        OtherRect.Right - SrcRect.Width,
                        OtherRect.Y + (OtherRect.Height / 2) - (SrcRect.Height / 2),
                        SrcRect.Width, SrcRect.Height); ;


                case ContentAlignment.BottomLeft:
                    return new RectangleF(
                        OtherRect.X,
                        OtherRect.Bottom - SrcRect.Height,
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.BottomCenter:
                    return new RectangleF(
                        OtherRect.X + (OtherRect.Width / 2) - (SrcRect.Width / 2),
                        OtherRect.Bottom - SrcRect.Height,
                        SrcRect.Width, SrcRect.Height);

                case ContentAlignment.BottomRight:
                    return new RectangleF(
                        OtherRect.Right - SrcRect.Width,
                        OtherRect.Bottom - SrcRect.Height,
                        SrcRect.Width, SrcRect.Height);
            }
        }
    }
}
