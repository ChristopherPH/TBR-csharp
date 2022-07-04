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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Controls
{
    /// <summary>
    /// Extended ListView
    ///
    /// New Events:
    ///   ViewChanged
    /// </summary>
    public class ExtendedListView : ListView
    {
        /// <summary>
        /// Occurs when the View property has changed
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the View property has changed.")]
        public event EventHandler<ViewEventArgs> ViewChanged;

        protected virtual void OnViewChanged(ViewEventArgs e)
        {
            ViewChanged?.Invoke(this, e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case NativeMethods.LVM_SETVIEW:
                    OnViewChanged(new ViewEventArgs((View)m.WParam));
                    break;
            }
        }
    }
}
