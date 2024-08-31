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
using TheBlackRoom.Gfx.Utility;

namespace TheBlackRoom.Gfx.Extensions
{
    public static class Graphics_RoundedRect
    {
        /// <summary>
        /// Draws a rounded rectangle
        /// </summary>
        /// <param name="graphics">Graphics object to draw upon</param>
        /// <param name="pen">Pen that determines the color, width, and style of the rectangle</param>
        /// <param name="rectangle">Bounding box of rectangle</param>
        /// <param name="radius">Corner radius</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen,
            RectangleF rectangle, float radius)
        {
            if ((graphics == null) || (pen == null))
                return;

            using (var rr = RoundedRectangles.CreateRoundedRectangle(rectangle, radius))
                graphics.DrawPath(pen, rr);
        }

        /// <summary>
        /// Fills a rounded rectangle
        /// </summary>
        /// <param name="graphics">Graphics object to draw upon</param>
        /// <param name="brush">Brush that determines the characteristics of the fill</param>
        /// <param name="rectangle">Bounding box of rectangle</param>
        /// <param name="radius">Corner radius</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush,
            RectangleF rectangle, float radius)
        {
            if ((graphics == null) || (brush == null))
                return;

            using (var rr = RoundedRectangles.CreateRoundedRectangle(rectangle, radius))
                graphics.FillPath(brush, rr);
        }
    }
}
