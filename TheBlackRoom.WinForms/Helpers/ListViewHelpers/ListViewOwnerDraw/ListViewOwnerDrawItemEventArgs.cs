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
    /// Provides data for the ListViewOwnerDrawHelper.OwnerDrawItem[List/SmallIcon/LargeIcon] event.
    /// </summary>
    public class ListViewOwnerDrawItemEventArgs : EventArgs
    {
        public ListViewOwnerDrawItemEventArgs(DrawListViewItemEventArgs eventArgs,
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

        public DrawListViewItemEventArgs EventArgs { get; }

        /// <summary>
        /// Returns the default ForeGround colour of the Item
        /// </summary>
        public virtual Color ForeColor
        {
            get
            {
                var listView = EventArgs.Item?.ListView;

                //We can't test the listview, so do what we can
                if (listView == null)
                    return EventArgs.Item.ForeColor;

                if (CheckState(ListViewItemStates.Selected))
                {
                    //Check certain conditions of the item and
                    //listview to return a special colour
                    if (listView.Focused || !listView.Enabled)
                        return SystemColors.HighlightText;

                    if (!listView.HideSelection)
                        return SystemColors.WindowText; //TODO: Verify this is the correct SystemColour
                }

                return EventArgs.Item.ForeColor;
            }
        }

        /// <summary>
        /// Draws the background of the item.
        /// </summary>
        public virtual void DrawBackground()
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

            EventArgs.DrawBackground();
        }

        /// <summary>
        /// Draws the background of the item with the specified color.
        /// </summary>
        public virtual void DrawBackground(Color backColor)
        {
            using (var brush = new SolidBrush(backColor))
                EventArgs.Graphics.FillRectangle(brush, EventArgs.Bounds);
        }

        /// <summary>
        /// Draws the image of the item.
        /// </summary>
        public virtual void DrawImage()
        {
            if ((ImageList == null) || ImageBounds.IsEmpty)
                return;

            var ix = EventArgs.Item.ImageIndex;
            if ((ix < 0) || (ix >= ImageList.Images.Count))
                return;

            ImageList.Draw(EventArgs.Graphics, ImageBounds.Location, EventArgs.Item.ImageIndex);
        }

        /// <summary>
        /// Draws the text of the item.
        /// </summary>
        public virtual void DrawText()
        {
            DrawText(ForeColor);
        }

        /// <summary>
        /// Draws the background of the item with the specified color.
        /// </summary>
        public virtual void DrawText(Color foreColor)
        {
            if (string.IsNullOrEmpty(EventArgs.Item.Text))
                return;

            TextRenderer.DrawText(EventArgs.Graphics, EventArgs.Item.Text,
                EventArgs.Item.Font, TextBounds, foreColor, TextFlags);
        }

        /// <summary>
        /// Draws the Focus Rectangle around the item.
        /// </summary>
        public virtual void DrawFocusRectangle()
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
