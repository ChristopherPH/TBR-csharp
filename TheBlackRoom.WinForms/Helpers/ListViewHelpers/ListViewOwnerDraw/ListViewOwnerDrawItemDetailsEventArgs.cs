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
    /// Provides data for the ListViewOwnerDrawHelper.OwnerDrawItemDetails event.
    /// </summary>
    public class ListViewOwnerDrawItemDetailsEventArgs : EventArgs
    {
        public ListViewOwnerDrawItemDetailsEventArgs(DrawListViewSubItemEventArgs eventArgs,
                    Rectangle textBounds, TextFormatFlags textFlags, Rectangle imageBounds, ImageList imageList)
        {
            EventArgs = eventArgs;
            TextBounds = textBounds;
            TextFlags = textFlags;
            ImageBounds = imageBounds;
            ImageList = imageList;
        }

        /// <summary>
        /// True if the item should be drawn automatically. The default is false.
        /// </summary>
        public bool DrawDefault { get; set; } = false;

        public Rectangle TextBounds { get; }
        public TextFormatFlags TextFlags { get; }
        public Rectangle ImageBounds { get; }
        public ImageList ImageList { get; }

        public DrawListViewSubItemEventArgs EventArgs { get; }

        public int ColumnIndex => EventArgs.ColumnIndex;

        /// <summary>
        /// Returns the default ForeGround colour of the Item
        /// </summary>
        public Color ForeColor
        {
            get
            {
                var listView = EventArgs.Item?.ListView;

                //We can't test the listview, so do what we can
                if (listView == null)
                    return EventArgs.SubItem.ForeColor;

                //We need to override the selected item if its for a full row or column 0
                if (CheckState(ListViewItemStates.Selected) &&
                    (listView.FullRowSelect || (ColumnIndex == 0)))
                {
                    //Check certain conditions of the item and
                    //listview to return a special colour
                    if (listView.Focused || !listView.Enabled)
                        return SystemColors.HighlightText;

                    if (!listView.HideSelection)
                        return SystemColors.WindowText; //TODO: Verify this is the correct SystemColour
                }

                return EventArgs.SubItem.ForeColor;
            }
        }

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

            var color = EventArgs.SubItem.BackColor;

            //Check certain conditions of the item and
            //listview to return a special colour
            if (CheckState(ListViewItemStates.Selected) &&
                (listView.FullRowSelect || (ColumnIndex == 0)))
            {
                if (listView.Focused || !listView.Enabled)
                {
                    color = SystemColors.Highlight;
                }
                else if (!listView.HideSelection)
                {
                    color = SystemColors.Control; //TODO: Verify this is the correct SystemColour
                }
            }
            else
            {
                if (!listView.Enabled)
                    color = SystemColors.Control; //TODO: Verify this is the correct SystemColour
            }

            DrawBackground(color);
        }

        /// <summary>
        /// Draws the background of the item with the specified color.
        /// </summary>
        public void DrawBackground(Color backColor)
        {
            if (!CanDrawBackgroundAndFocusRectangle)
                return;

            using (var brush = new SolidBrush(backColor))
                EventArgs.Graphics.FillRectangle(brush, EventArgs.Bounds);
        }

        /// <summary>
        /// Draws the image of the item.
        /// </summary>
        public void DrawImage()
        {
            if ((ImageList == null) || (ImageBounds.IsEmpty))
                return;

            var ix = EventArgs.Item.ImageIndex;
            if ((ix < 0) || (ix >= ImageList.Images.Count))
                return;

            ImageList.Draw(EventArgs.Graphics, ImageBounds.Location, EventArgs.Item.ImageIndex);
        }

        /// <summary>
        /// Draws the text of the item.
        /// </summary>
        public void DrawText()
        {
            DrawText(ForeColor);
        }

        /// <summary>
        /// Draws the background of the item with the specified color.
        /// </summary>
        public void DrawText(Color foreColor)
        {
            if (string.IsNullOrEmpty(EventArgs.Item.Text))
                return;

            TextRenderer.DrawText(EventArgs.Graphics, EventArgs.SubItem.Text,
                EventArgs.SubItem.Font, TextBounds, foreColor, TextFlags);
        }

        /// <summary>
        /// Draws the Focus Rectangle around the item.
        /// </summary>
        public void DrawFocusRectangle()
        {
            if (!CanDrawBackgroundAndFocusRectangle)
                return;

            //TODO: can we get the first subitems bounds?
            //if (CheckState(ListViewItemStates.Focused))
            //    EventArgs.DrawFocusRectangle(EventArgs.Item.SubItems[0].Bounds);
        }

        /// <summary>
        /// Checks an item for a state flag
        /// </summary>
        public bool CheckState(ListViewItemStates state)
        {
            //EventArgs.State.HasFlag(...Selected) is always true. Use e.Item.Selected as that is correct.
            if (state == ListViewItemStates.Selected)
                return EventArgs.Item.Selected;

            return EventArgs.ItemState.HasFlag(state);
        }

        /// <summary>
        /// Check if Background or FocusRectangle can be drawn, or if they are handled
        /// by OwnerDrawBackgroundDetails event (ListViewOwnerDrawBackgroundDetailsEventArgs)
        /// </summary>
        public bool CanDrawBackgroundAndFocusRectangle
        {
            get
            {
                var listView = EventArgs.Item?.ListView;
                if (listView == null)
                    return true;

                if (listView.FullRowSelect && CheckState(ListViewItemStates.Selected))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
