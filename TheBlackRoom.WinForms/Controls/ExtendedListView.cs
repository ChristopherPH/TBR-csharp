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
    ///   ColumnRightClick
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

        protected virtual void OnColumnRightClick(ColumnClickEventArgs e)
        {
            ColumnRightClick?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a column header is right clicked
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a column header is right clicked.")]
        public event ColumnClickEventHandler ColumnRightClick;

        /// <summary>
        /// Checks WM_CONTEXTMENU to determine if click was on a header, and
        /// triggers ColumnRightClick event if so.
        /// </summary>
        /// <returns>True if click was on the header region</returns>
        private bool HandleColumnRightClick(Message m)
        {
            if (!this.IsHandleCreated)
                return false;

            /* The listview has 2 window handles, the main
             * handle, and the header handle. If the clicked
             * window is on the main window, ignore the click.
             * Otherwise, we must be on the header.
             * We could also use LVM_GETHEADER message to get
             * the handle to the header from the listview handle.
             */
            if (m.WParam == this.Handle)
                return false;

            if (!NativeMethods.GetWindowRect(m.WParam, out var rect))
                return true;

            var currentPosition = 0;

            /* Check each column position to see if the click
             * occured on that column.
             */
            foreach (var column in this.Columns
                .OfType<ColumnHeader>()
                .OrderBy(x => x.DisplayIndex))
            {
                currentPosition += column.Width;

                //Check if the header position has exceeded the click location
                if (currentPosition > (MousePosition.X - rect.Left))
                {
                    OnColumnRightClick(new ColumnClickEventArgs(this.Columns.IndexOf(column)));
                    break;
                }
            }

            return true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case NativeMethods.LVM_SETVIEW:
                    OnViewChanged(new ViewEventArgs((View)m.WParam));
                    break;

                case NativeMethods.WM_CONTEXTMENU:
                    HandleColumnRightClick(m);
                    break;
            }
        }
    }
}
