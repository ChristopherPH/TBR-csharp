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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Keyboard
{
    public partial class RawInput : IDisposable
    {
        private bool disposedValue;
        IntPtr _notificationHandle = IntPtr.Zero;

        public RawInput(IWin32Window window, bool focusedOnly = true)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            if (window.Handle == IntPtr.Zero)
                throw new ArgumentException(nameof(window.Handle));

            //Create keyboard raw input device
            var devices = new NativeMethods.RAWINPUTDEVICE[1]
            {
                new NativeMethods.RAWINPUTDEVICE()
                {
                    usUsagePage = NativeMethods.HID_USAGE_PAGE_GENERIC,
                    usUsage = NativeMethods.HID_USAGE_GENERIC_KEYBOARD,
                    dwFlags = focusedOnly ? 0 : NativeMethods.RIDEV_INPUTSINK,
                    hwndTarget = window.Handle,
                }
            };

            var cbSize = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTDEVICE));

            //Add keyboard raw input device
            if (!NativeMethods.RegisterRawInputDevices(devices, devices.Length, cbSize))
                throw new InvalidOperationException("RegisterRawInputDevices failed");

            //Register for device notifications
            var ptr = IntPtr.Zero;

            var devBroadcastHeader = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE()
            {
                dbch_size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)),
                dbch_devicetype = NativeMethods.DBT_DEVTYP_DEVICEINTERFACE,
                dbcc_classguid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030"),
            };

            try
            {
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(devBroadcastHeader));
                Marshal.StructureToPtr(devBroadcastHeader, ptr, false);
                _notificationHandle = NativeMethods.RegisterDeviceNotification(window.Handle,
                    ptr, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        /// <summary>
        /// Raised when raw keyboard input is available
        /// </summary>
        public event EventHandler<RawInputKeyboardEventArgs> RawInputKeyboard;

        /// <summary>
        /// Call to process raw input messages
        /// </summary>
        /// <param name="msg"></param>
        public void HandleWndProc(Message msg)
        {
            if (msg.Msg == NativeMethods.WM_INPUT)
            {
                //Get raw input message
                var rawInput = new NativeMethods.RAWINPUT();
                var cbSize = Marshal.SizeOf(typeof(NativeMethods.RAWINPUT));
                var cbSizeHeader = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER));

                if (NativeMethods.GetRawInputData(msg.LParam, NativeMethods.RID_INPUT,
                    out rawInput, ref cbSize, cbSizeHeader) != -1)
                {
                    HandleRawInputKeyboard(rawInput.data.keyboard);
                }
            }
        }

        /// <summary>
        /// Handles raw keyboard input message
        /// </summary>
        /// <param name="keyboard"></param>
        private void HandleRawInputKeyboard(NativeMethods.RAWKEYBOARD keyboard)
        {
            //Check for overrun
            if (keyboard.VKey == NativeMethods.KEYBOARD_OVERRUN_MAKE_CODE)
                return;

            //Convert virtual key to keys enum
            var key = (Keys)keyboard.VKey;

            //TODO: Convert makecode to scan code as per API documentation
            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawkeyboard#remarks
            var scanCode = keyboard.MakeCode;

            /* Fake shift keys have an incorrect break flag and make code, so use
             * the message member to determine key down/up state instead of break flag.
             * The make code is that of the key that triggered the fake shift, instead
             * of the make code being the left or right shift key. */
            RawInputKeyStates keyState;

            switch (keyboard.Message)
            {
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    keyState = RawInputKeyStates.Down;
                    break;

                case NativeMethods.WM_KEYUP:
                case NativeMethods.WM_SYSKEYUP:
                    keyState = RawInputKeyStates.Up;
                    break;

                default:
                    return;
            }

            //Raise raw input event
            RawInputKeyboard?.Invoke(this, new RawInputKeyboardEventArgs(key,
                keyState, scanCode, keyboard.Flags, keyboard.Message,
                keyboard.ExtraInformation));
        }

        protected virtual void OnRawInputKeyboard(RawInputKeyboardEventArgs args)
        {
            RawInputKeyboard?.Invoke(this, args);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (_notificationHandle != IntPtr.Zero)
                {
                    NativeMethods.UnregisterDeviceNotification(_notificationHandle);
                    _notificationHandle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        ~RawInput()
        {
            Dispose(disposing: false);
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
