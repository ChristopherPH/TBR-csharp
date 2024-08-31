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
using System.Drawing.Drawing2D;

namespace TheBlackRoom.Gfx.Utility
{
    public static class RoundedRectangles
    {
        /// <summary>
        /// Creates a graphics path representing a rectangle with rounded corners
        /// </summary>
        /// <param name="rectangle">Bounding box of rectangle</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>A GraphicsPath object (which must be disposed after use)</returns>
        public static GraphicsPath CreateRoundedRectangle(RectangleF rectangle, float radius)
        {
            var path = new GraphicsPath();

            /* No valid radius, return the rectangle */
            if (radius <= 0)
            {
                path.AddRectangle(rectangle);
                return path;
            }

            var diameter = radius * 2.0f;
            var arc = new RectangleF(rectangle.X, rectangle.Y, diameter, diameter);

            /* Start with top left */
            path.AddArc(arc, 180, 90);

            /* Add top right */
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90);

            /* Add bottom right */
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            /* Add bottom left */
            arc.X = rectangle.Left;
            path.AddArc(arc, 90, 90);

            /* Close */
            path.CloseFigure();

            return path;
        }
    }
}
