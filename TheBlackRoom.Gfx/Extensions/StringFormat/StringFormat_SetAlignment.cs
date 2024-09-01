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
    public static class StringFormat_SetAlignment
    {
        /// <summary>
        /// Sets the alignment of a StringFormat object based on a ContentAlignment
        /// </summary>
        public static void SetAlignment(this StringFormat format, ContentAlignment alignment)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    format.LineAlignment = StringAlignment.Near;
                    format.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.TopCenter:
                    format.LineAlignment = StringAlignment.Near;
                    format.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.TopRight:
                    format.LineAlignment = StringAlignment.Near;
                    format.Alignment = StringAlignment.Far;
                    break;

                case ContentAlignment.MiddleLeft:
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.MiddleCenter:
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.MiddleRight:
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Far;
                    break;

                case ContentAlignment.BottomLeft:
                    format.LineAlignment = StringAlignment.Far;
                    format.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.BottomCenter:
                    format.LineAlignment = StringAlignment.Far;
                    format.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.BottomRight:
                    format.LineAlignment = StringAlignment.Far;
                    format.Alignment = StringAlignment.Far;
                    break;
            }
        }
    }
}
