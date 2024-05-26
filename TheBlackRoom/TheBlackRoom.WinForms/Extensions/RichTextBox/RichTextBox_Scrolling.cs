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
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class RichTextBox_Scrolling
    {
        /// <summary>
        /// Gets the current scroll position of a rich text box
        /// </summary>
        /// <param name="richTextBox">Rich Text Box to get scroll position from</param>
        public static Point GetScrollPosition(this RichTextBox richTextBox)
        {
            if (richTextBox == null)
                return Point.Empty;

            NativeMethods.POINT nPoint = new NativeMethods.POINT();

            NativeMethods.SendMessage(richTextBox.Handle,
                NativeMethods.EM_GETSCROLLPOS, IntPtr.Zero, ref nPoint);

            return new Point(nPoint.X, nPoint.Y);
        }

        /// <summary>
        /// Sets the current scroll position of a rich text box
        /// </summary>
        /// <param name="richTextBox">Rich Text Box to scroll</param>
        public static void SetScrollPosition(this RichTextBox richTextBox, Point scrollPosition)
        {
            if (richTextBox == null)
                return;

            NativeMethods.POINT nPoint = new NativeMethods.POINT()
            {
                X = scrollPosition.X,
                Y = scrollPosition.Y,
            };

            NativeMethods.SendMessage(richTextBox.Handle,
                NativeMethods.EM_SETSCROLLPOS, IntPtr.Zero, ref nPoint);
        }

        /// <summary>
        /// Checks if the rich text box scroll position is at the bottom
        /// </summary>
        /// <param name="richTextBox">Rich Text Box to check</param>
        /// <returns>true if the scroll position is at the bottom, false if not</returns>
        public static bool GetScrollPositionIsBottom(this RichTextBox richTextBox)
        {
            //No text box, the scroll position must be at the bottom
            if (richTextBox == null)
                return true;

            //No text, the scroll position must be at the bottom
            if (richTextBox.Text.Length == 0)
                return true;

            //Get the pixel position of the last character in the text box
            var position = richTextBox.GetPositionFromCharIndex(richTextBox.Text.Length - 1);

            //As long as the pixel position of the last character doesn't
            //exceed the control height, the text box is at the bottom
            return position.Y < richTextBox.Height;
        }
    }
}
