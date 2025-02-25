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
    /// <summary>
    /// Event data for Raw Keyboard Input
    /// </summary>
    public class RawInputKeyboardEventArgs : EventArgs
    {
        public RawInputKeyboardEventArgs(Keys Key, RawInputKeyStates KeyState, ushort ScanCode,
            ushort Flags, uint Message, ulong ExtraInformation)
        {
            this.Key = Key;
            this.KeyState = KeyState;
            this.ScanCode = ScanCode;
            this.Flags = Flags;
            this.Message = Message;
            this.ExtraInformation = ExtraInformation;
        }

        /// <summary>
        /// Legacy virtual key code
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// Raw Input Key State (Up, Down)
        /// </summary>
        public RawInputKeyStates KeyState { get; }

        /// <summary>
        /// Keyboard scan code
        /// </summary>
        public ushort ScanCode { get; }

        /// <summary>
        /// Scan code flags
        /// </summary>
        public ushort Flags { get; }

        /// <summary>
        /// Legacy windows keyboard message (WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, WM_SYSKEYUP)
        /// </summary>
        public uint Message { get; }

        /// <summary>
        /// The device-specific additional information for the event
        /// </summary>
        public ulong ExtraInformation { get; }

        /// <summary>
        /// Flag indicating key is up (unless a fake shift)
        /// </summary>
        public bool FlagsKeyBreak => (Flags & RawInput.NativeMethods.RI_KEY_BREAK) == RawInput.NativeMethods.RI_KEY_BREAK;

        /// <summary>
        /// Flag indicating scan code has the E0 prefix
        /// </summary>
        public bool FlagsE0 => (Flags & RawInput.NativeMethods.RI_KEY_E0) == RawInput.NativeMethods.RI_KEY_E0;

        /// <summary>
        /// Flag indicating scan code has the E1 prefix
        /// </summary>
        public bool FlagsE1 => (Flags & RawInput.NativeMethods.RI_KEY_E1) == RawInput.NativeMethods.RI_KEY_E1;
    }
}
