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
using System.Runtime.InteropServices;

namespace TheBlackRoom.WinForms
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] uint wMsg,
            [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] uint wMsg,
            [In] IntPtr wParam, [In] int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] uint wMsg,
            [In] int wParam, [In] int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] uint wMsg,
            [In] IntPtr wParam, ref POINT lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect([In] IntPtr hWnd, [Out] out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public int cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        public enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }

        /// <summary>
        /// Scroll Bar Orientation
        /// </summary>
        public enum SBOrientation : int
        {
            SB_HORZ = 0x0,
            SB_VERT = 0x1,
            SB_CTL = 0x2,
            SB_BOTH = 0x3
        }

        public const int WM_USER = 0x0400;

        public const int WM_SETREDRAW = 0x000B;

        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_CONTEXTMENU = 0x007B;

        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x20A;

        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;

        public const int EM_SCROLL = 0x00B5;

        public const int SB_LINEUP = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_ENDSCROLL = 8;

        /* ListView */
        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETHEADER = LVM_FIRST + 31;
        public const int LVM_SETICONSPACING = LVM_FIRST + 53;
        public const int LVM_SETVIEW = LVM_FIRST + 142;

        /* RichTextBox Constants
         * https://referencesource.microsoft.com/#system.windows.forms/winforms/managed/system/winforms/RichTextBoxConstants.cs
         */

        public const int EM_GETEVENTMASK = NativeMethods.WM_USER + 59;
        public const int EM_SETEVENTMASK = NativeMethods.WM_USER + 69;

        // Message for getting and restoring scroll pos
        public const int EM_GETSCROLLPOS = NativeMethods.WM_USER + 221;
        public const int EM_SETSCROLLPOS = NativeMethods.WM_USER + 222;


        /* Scrolling */
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo([In] IntPtr hWnd, int nBar,
            ref SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo([In] IntPtr hWnd, int nBar,
            ref SCROLLINFO lpsi, bool redraw);

        /* Misc */
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public extern static int SetWindowTheme([In] IntPtr hWnd,
            string pszSubAppName, string pszSubIdList);
    }
}
