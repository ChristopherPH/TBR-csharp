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
using System.Drawing;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Helpers.ListViewHelpers.ListViewOwnerDraw
{
    /// <summary>
    /// Provides data for the ListViewOwnerDrawHelper.OwnerDrawSelectedItemDetailsFullRowBackground event.
    /// </summary>
    public class ListViewOwnerDrawSelectedItemDetailsFullRowBackgroundEventArgs : EventArgs
    {
        public ListViewOwnerDrawSelectedItemDetailsFullRowBackgroundEventArgs(DrawListViewItemEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        /// <summary>
        /// True if the item should be drawn automatically. The default is false.
        /// </summary>
        public bool DrawDefault { get; set; } = false;

        public DrawListViewItemEventArgs EventArgs { get; }

        /// <summary>
        /// Draws the background of the item.
        /// </summary>
        public void DrawBackground()
        {
            var listView = EventArgs.Item?.ListView;

            //We can't test the listview, so do what we can
            if (listView == null)
            {
                EventArgs.DrawBackground();
                return;
            }

            //Check certain conditions of the item and
            //listview to return a special colour
            if (CheckState(ListViewItemStates.Selected))
            {
                if (listView.Focused || !listView.Enabled)
                {
                    DrawBackground(SystemColors.Highlight);
                    return;
                }

                if (!listView.HideSelection)
                {
                    DrawBackground(SystemColors.Control); //TODO: Verify this is the correct SystemColour
                    return;
                }
            }
            else
            {
                if (!listView.Enabled)
                {
                    DrawBackground(SystemColors.Control); //TODO: Verify this is the correct SystemColour
                    return;
                }
            }

            //Draw the background using the default
            EventArgs.DrawBackground();
        }

        /// <summary>
        /// Draws the background of the item with the specified color.
        /// </summary>
        public void DrawBackground(Color backColor)
        {
            using (var brush = new SolidBrush(backColor))
                EventArgs.Graphics.FillRectangle(brush, EventArgs.Bounds);
        }

        /// <summary>
        /// Draws the Focus Rectangle around the item.
        /// </summary>
        public void DrawFocusRectangle()
        {
            if (CheckState(ListViewItemStates.Focused))
                EventArgs.DrawFocusRectangle();
        }

        /// <summary>
        /// Checks an item for a state flag
        /// </summary>
        public bool CheckState(ListViewItemStates state)
        {
            //EventArgs.State.HasFlag(...Selected) is always true. Use e.Item.Selected as that is correct.
            if (state == ListViewItemStates.Selected)
                return EventArgs.Item.Selected;

            return EventArgs.State.HasFlag(state);
        }
    }
}
