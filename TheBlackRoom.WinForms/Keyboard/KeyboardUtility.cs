/*
 * Copyright (c) 2025 Christopher Hayes
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

namespace TheBlackRoom.WinForms.Keyboard
{
    public static class KeyboardUtility
    {
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public const uint KF_SCANCODE_MASK = 0x00FF;
        public const uint KF_FLAGS_MASK = 0xFF00;

        //https://learn.microsoft.com/en-us/windows/win32/inputdev/about-keyboard-input#keystroke-message-flags
        [Flags]
        public enum KeystrokeFlags
        {
            /// <summary>
            /// Manipulates the extended key flag.
            /// </summary>
            KF_EXTENDED = 0x0100,

            /// <summary>
            /// Manipulates the dialog mode flag, which indicates whether a dialog box is active.
            /// </summary>
            KF_DLGMODE = 0x0800,

            /// <summary>
            /// Manipulates the menu mode flag, which indicates whether a menu is active.
            /// </summary>
            KF_MENUMODE = 0x1000,

            /// <summary>
            /// Manipulates the context code flag.
            /// </summary>
            KF_ALTDOWN = 0x2000,

            /// <summary>
            /// Manipulates the previous key state flag.
            /// </summary>
            KF_REPEAT = 0x4000,

            /// <summary>
            /// Manipulates the transition state flag.
            /// </summary>
            KF_UP = 0x8000,
        }

        /// <summary>
        /// Parses information from a keyboard message (WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, WM_SYSKEYUP)
        /// </summary>
        /// <param name="msg">Windows message to parse</param>
        /// <param name="modifierKeys">Control.ModifierKeys if available, or Keys.None</param>
        /// <param name="keyData">Parsed keyCode or keyData</param>
        /// <param name="repeatCount">Parsed repeat count</param>
        /// <param name="flags">Parsed flags</param>
        /// <param name="scanCode">Parsed scan code</param>
        /// <returns>true if message parsed</returns>
        public static bool ParseKeyboardMessage(Message msg, Keys modifierKeys,
            out Keys keyData, out ushort repeatCount, out KeystrokeFlags flags,
            out uint scanCode)
        {
            switch (msg.Msg)
            {
                case WM_KEYDOWN:
                case WM_KEYUP:
                case WM_SYSKEYDOWN:
                case WM_SYSKEYUP:
                    break;

                default:
                    keyData = Keys.None;
                    repeatCount = 0;
                    flags = (KeystrokeFlags)0;
                    scanCode = 0;
                    return false;
            }

            //key is wparam
            keyData = (Keys)msg.WParam | modifierKeys;

            //lparam loword is repeat count
            repeatCount = unchecked((ushort)(ulong)msg.LParam);

            //lparam hiword is flags and scancode
            ushort hiword = unchecked((ushort)((ulong)msg.LParam >> 16));

            flags = (KeystrokeFlags)(hiword & KF_FLAGS_MASK);
            scanCode = (hiword & KF_SCANCODE_MASK);

            return true;
        }
    }
}
