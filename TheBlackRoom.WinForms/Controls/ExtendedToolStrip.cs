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
using System.ComponentModel;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Controls
{
    /// <summary>
    /// ToolStrip with workarounds for:
    ///     Toolstrip requiring form focus before allowing buttons to be clicked
    /// </summary>
    public class ExtendedToolStrip : ToolStrip
    {
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Allows toolstrip buttons to receive a mouse click when the toolstrip is on an inactive form")]
        public bool AllowInactiveFormSingleClick { get; set; } = true;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                /* AllowInactiveFormSingleClick
                 * Allows toolstrip buttons to receive a mouse click when the toolstrip is on an inactive form
                 */
                case NativeMethods.WM_MOUSEACTIVATE:
                    if (AllowInactiveFormSingleClick && CanFocus && !Focused)
                        Focus();
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
