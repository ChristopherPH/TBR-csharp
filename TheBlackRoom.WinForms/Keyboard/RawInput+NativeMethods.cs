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

namespace TheBlackRoom.WinForms.Keyboard
{
    public partial class RawInput
    {
        internal static class NativeMethods
        {
            public const int WM_INPUT = 0x00FF;
            public const int WM_KEYDOWN = 0x100;
            public const int WM_KEYUP = 0x101;
            public const int WM_SYSKEYDOWN = 0x104;
            public const int WM_SYSKEYUP = 0x105;
            public const int RID_INPUT = 0x10000003;
            public const uint RIDEV_INPUTSINK = 0x00000100;
            public const int KEYBOARD_OVERRUN_MAKE_CODE = 0xFF;
            public const int RI_KEY_MAKE = 0x00;    //Down
            public const int RI_KEY_BREAK = 0x01;   //Up
            public const int RI_KEY_E0 = 0x02;
            public const int RI_KEY_E1 = 0x04;

            //https://learn.microsoft.com/en-us/windows-hardware/drivers/hid/hid-usages#usage-page
            public const int HID_USAGE_PAGE_GENERIC = 0x01;
            public const int HID_USAGE_GENERIC_KEYBOARD = 0x06;

            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerrawinputdevices
            [DllImport("user32.dll")]
            public static extern bool RegisterRawInputDevices([In] RAWINPUTDEVICE[] pRawInputDevices,
                [In] int uiNumDevices, [In] int cbSize);

            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdata
            [DllImport("user32.dll")]
            public static extern int GetRawInputData([In] IntPtr hRawInput, [In] uint uiCommand,
                [Out] out RAWINPUT pData, ref int pcbSize, [In] int cbSizeHeader);


            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputdevice
            [StructLayout(LayoutKind.Sequential)]
            public struct RAWINPUTDEVICE
            {
                public ushort usUsagePage;
                public ushort usUsage;
                public uint dwFlags;
                public IntPtr hwndTarget;
            }

            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputheader
            [StructLayout(LayoutKind.Sequential)]
            public struct RAWINPUTHEADER
            {
                public uint dwType;
                public uint dwSize;
                public IntPtr hDevice;
                public IntPtr wParam;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RAWINPUT
            {
                public RAWINPUTHEADER header;
                public RAWDATA data;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct RAWDATA
            {
                [FieldOffset(0)]
                public RAWMOUSE mouse;

                [FieldOffset(0)]
                public RAWKEYBOARD keyboard;

                [FieldOffset(0)]
                public RAWHID hid;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RAWKEYBOARD
            {
                public ushort MakeCode;
                public ushort Flags;
                public ushort Reserved;
                public ushort VKey;
                public uint Message;
                public ulong ExtraInformation;
            }


            [StructLayout(LayoutKind.Explicit)]
            public struct RAWMOUSE
            {
                [FieldOffset(0)]
                public ushort usFlags;
                [FieldOffset(4)]
                public uint ulButtons;
                [FieldOffset(4)]
                public ushort usButtonFlags;
                [FieldOffset(6)]
                public ushort usButtonData;
                [FieldOffset(8)]
                public ulong ulRawButtons;
                [FieldOffset(12)]
                public long lLastX;
                [FieldOffset(16)]
                public long lLastY;
                [FieldOffset(20)]
                public ulong ulExtraInformation;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RAWHID
            {
                public uint dwSizeHid;
                public uint dwCount;
                public byte bRawData;
            }

            public const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
            public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

            //https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerdevicenotificationa
            [DllImport("user32.dll")]
            public static extern IntPtr RegisterDeviceNotification([In] IntPtr hRecipient, [In] IntPtr NotificationFilter, [In] uint Flags);

            [DllImport("user32.dll")]
            public static extern bool UnregisterDeviceNotification([In] IntPtr Handle);

            //https://learn.microsoft.com/en-us/windows/win32/api/dbt/ns-dbt-dev_broadcast_deviceinterface_a
            [StructLayout(LayoutKind.Sequential)]
            public struct DEV_BROADCAST_DEVICEINTERFACE
            {
                public int dbch_size;
                public uint dbch_devicetype;
                public uint dbch_reserved;
                public Guid dbcc_classguid;
                public char dbcc_name;
            }
        }
    }
}
