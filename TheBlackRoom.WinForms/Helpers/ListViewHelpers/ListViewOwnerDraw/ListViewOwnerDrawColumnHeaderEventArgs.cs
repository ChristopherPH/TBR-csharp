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
    /// Provides data for the ListViewOwnerDrawHelper.OwnerDrawColumnHeader event.
    /// </summary>
    public class ListViewOwnerDrawColumnHeaderEventArgs : EventArgs
    {
        public ListViewOwnerDrawColumnHeaderEventArgs(DrawListViewColumnHeaderEventArgs eventArgs,
            Rectangle textBounds, TextFormatFlags textFlags, Rectangle imageBounds, ImageList imageList)
        {
            EventArgs = eventArgs;
            TextBounds = textBounds;
            TextFlags = textFlags;
            ImageBounds = imageBounds;
            ImageList = imageList;
        }

        /// <summary>
        /// True if the header should be drawn automatically. The default is false.
        /// </summary>
        public bool DrawDefault { get; set; } = false;

        public Rectangle TextBounds { get; }
        public TextFormatFlags TextFlags { get; }
        public Rectangle ImageBounds { get; }
        public ImageList ImageList { get; }

        public DrawListViewColumnHeaderEventArgs EventArgs { get; }

        /// <summary>
        /// Draws the background of the column header.
        /// </summary>
        public void DrawBackground()
        {
            EventArgs.DrawBackground();
        }

        /// <summary>
        /// Draws the background of the column header with the specified color.
        /// </summary>
        public void DrawBackground(Color backColor)
        {
            using (var brush = new SolidBrush(backColor))
                EventArgs.Graphics.FillRectangle(brush, EventArgs.Bounds);
        }

        /// <summary>
        /// Draws the image of the column header.
        /// </summary>
        public void DrawImage()
        {
            if ((ImageList == null) || (ImageBounds.IsEmpty))
                return;

            var ix = EventArgs.Header.ImageIndex;
            if ((ix < 0) || (ix >= ImageList.Images.Count))
                return;

            ImageList.Draw(EventArgs.Graphics, ImageBounds.Location, EventArgs.Header.ImageIndex);
        }

        /// <summary>
        /// Draws the text of the column header.
        /// </summary>
        public void DrawText()
        {
            DrawText(EventArgs.ForeColor);
        }

        /// <summary>
        /// Draws the background of the column header with the specified color.
        /// </summary>
        public void DrawText(Color foreColor)
        {
            if (string.IsNullOrEmpty(EventArgs.Header.Text))
                return;

            TextRenderer.DrawText(EventArgs.Graphics, EventArgs.Header.Text,
                EventArgs.Font, TextBounds, foreColor, TextFlags);
        }
    }
}
