/*
 * Copyright (c) 2022 Christopher Hayes
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
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class ListView_SetIconSpacing
    {
        /// <summary>
        /// Sets the spacing between icons in list-view controls that have the LVS_ICON style.
        /// See https://docs.microsoft.com/en-us/windows/win32/controls/lvm-seticonspacing
        /// for more information.
        /// </summary>
        public static void SetIconSpacing(this ListView listView, int leftPadding, int topPadding)
        {
            if (listView is null)
                throw new ArgumentNullException(nameof(listView));

            if (!listView.IsHandleCreated)
                throw new ArgumentException("Invalid Handle", nameof(listView));

            NativeMethods.SendMessage(listView.Handle, NativeMethods.LVM_SETICONSPACING,
                IntPtr.Zero, (IntPtr)((topPadding << 16) | leftPadding));
        }
    }
}
