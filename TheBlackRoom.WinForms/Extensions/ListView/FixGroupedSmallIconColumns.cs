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
using System.Linq;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class ListView_FixGroupedSmallIconColumns
    {
        /// <summary>
        /// Fix small icon view display issues when grouped, by setting
        /// column 0 width to the largest text display. Note that this
        /// will cause all items to be aligned to the largest item which
        /// may not be desired.
        /// However, it will also fix issues with items not displaying
        /// when scrolling, due to the size of other items.
        /// </summary>
        public static void FixGroupedSmallIconColumns(this ListView listView)
        {
            float smallIconWidth = 0;

            //get width of largest item
            using (var g = listView.CreateGraphics())
                smallIconWidth = listView.Items
                    .OfType<ListViewItem>()
                    .Max(x => g.MeasureString(x.Text, x.Font ?? listView.Font).Width);

            //add in size of icon if exists
            if (listView.SmallImageList != null)
                smallIconWidth += listView.SmallImageList.ImageSize.Width;

            //add column 0 to be able to set the width, if it doesn't exist
            if (listView.Columns.Count == 0)
                listView.Columns.Add(string.Empty, (int)Math.Ceiling(smallIconWidth));
            else
                listView.Columns[0].Width = (int)Math.Ceiling(smallIconWidth);

            //trigger a redisplay of the view
            listView.ArrangeIcons(ListViewAlignment.SnapToGrid);
        }
    }
}
