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
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class TextBoxBase_Scrolling
    {
        /// <summary>
        /// Scrolls a Text Box up one line
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollLineUp(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_LINEUP, 0);
        }

        /// <summary>
        /// Scrolls a Text Box down one line
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollLineDown(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_LINEDOWN, 0);
        }

        /// <summary>
        /// Scrolls a Text Box up one page
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollPageUp(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_PAGEUP, 0);
        }

        /// <summary>
        /// Scrolls a Text Box down one page
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollPageDown(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_PAGEDOWN, 0);
        }

        /// <summary>
        /// Scrolls a Text Box to the top
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollToTop(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_TOP, 0);
        }

        /// <summary>
        /// Scrolls a Text Box to the bottom
        /// </summary>
        /// <param name="textBox">Text Box to scroll</param>
        public static void ScrollToBottom(this TextBoxBase textBox)
        {
            if (textBox != null)
                NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL,
                    NativeMethods.SB_BOTTOM, 0);
        }
    }
}
