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
using System.Drawing;

namespace TheBlackRoom.Gfx.Extensions
{
    public static class Rectangle_Scale
    {
        /// <summary>
        /// Scales a rectangle to fit within the bounds of another rectangle, while maintaining aspect ratio
        /// </summary>
        /// <param name="SrcRect">Rectangle to scale</param>
        /// <param name="Bounds">Bounds to scale into</param>
        /// <param name="scaleFactor">Saved scale factor</param>
        /// <returns>Scaled rectangle</returns>
        public static Rectangle ScaleAspect(this Rectangle SrcRect, Rectangle Bounds, out float scaleFactor)
        {
            return Rectangle.Round(RectangleF_Scale.ScaleAspect(SrcRect, (RectangleF)Bounds, out scaleFactor));
        }

        /// <summary>
        /// Scales a rectangle to fit within the bounds of another rectangle, while maintaining aspect ratio
        /// </summary>
        /// <param name="SrcRect">Rectangle to scale</param>
        /// <param name="Bounds">Bounds to scale into</param>
        /// <returns>Scaled rectangle</returns>
        public static Rectangle ScaleAspect(this Rectangle SrcRect, Rectangle Bounds)
        {
            return Rectangle.Round(RectangleF_Scale.ScaleAspect(SrcRect, (RectangleF)Bounds));
        }

        /// <summary>
        /// Aligns a rectangle to another rectangle given an alignment
        /// </summary>
        /// <param name="SrcRect">Rectangle to align</param>
        /// <param name="OtherRect">Rectangle to align to</param>
        /// <param name="Alignmment">Location to align to</param>
        /// <returns>Aligned rectangle</returns>
        public static Rectangle Align(this Rectangle SrcRect, Rectangle OtherRect, ContentAlignment Alignmment)
        {
            return Rectangle.Round(RectangleF_Scale.Align(SrcRect, (RectangleF)OtherRect, Alignmment));
        }
    }
}
