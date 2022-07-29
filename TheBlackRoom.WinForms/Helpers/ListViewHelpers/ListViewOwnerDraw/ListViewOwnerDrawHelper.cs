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
    /// Helper class for OwnerDrawing a ListView.
    ///
    /// Splits OwnerDrawing each view into a seperate event, provides a DrawDefault option per view
    ///     (and auto sets DrawDefault to true when no event handlers are attached to the event)
    /// </summary>
    public class ListViewOwnerDrawHelper
    {
        public ListViewOwnerDrawHelper(ListView listView)
        {
            if (listView == null)
                throw new ArgumentNullException(nameof(listView));

            listView.OwnerDraw = true;

            listView.DrawColumnHeader += listView_DrawColumnHeader;
            listView.DrawItem += listView_DrawItem;
            listView.DrawSubItem += listView_DrawSubItem;
        }

        /// <summary>
        /// Occurs when the Column Header of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawColumnHeaderEventArgs> OwnerDrawColumnHeader;

        /// <summary>
        /// Occurs when the Large Icon View of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawItemEventArgs> OwnerDrawItemLargeIcon;

        /// <summary>
        /// Occurs when the Small Icon View of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawItemEventArgs> OwnerDrawItemSmallIcon;

        /// <summary>
        /// Occurs when the List View of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawItemEventArgs> OwnerDrawItemList;

        /// <summary>
        /// Occurs when the Tile View of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawItemTileEventArgs> OwnerDrawItemTile;

        /// <summary>
        /// Occurs when the Details View of a System.Windows.Forms.ListView is drawn
        /// </summary>
        public event EventHandler<ListViewOwnerDrawItemDetailsEventArgs> OwnerDrawItemDetails;

        /// <summary>
        /// Occurs when the Background of a System.Windows.Forms.ListView is drawn, when in the
        /// Details View, Full Row Select is enabled, and the current item is selected.
        /// </summary>
        public event EventHandler<ListViewOwnerDrawSelectedItemDetailsFullRowBackgroundEventArgs> OwnerDrawSelectedItemDetailsFullRowBackground;

        protected bool OnDrawListViewColumnHeaderOwnerDrawHelperEventArgs(ListViewOwnerDrawColumnHeaderEventArgs args)
        {
            if (OwnerDrawColumnHeader == null)
                return true;

            OwnerDrawColumnHeader(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawItemLargeIcon(ListViewOwnerDrawItemEventArgs args)
        {
            if (OwnerDrawItemLargeIcon == null)
                return true;

            OwnerDrawItemLargeIcon(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawItemSmallIcon(ListViewOwnerDrawItemEventArgs args)
        {
            if (OwnerDrawItemSmallIcon == null)
                return true;

            OwnerDrawItemSmallIcon(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawItemList(ListViewOwnerDrawItemEventArgs args)
        {
            if (OwnerDrawItemList == null)
                return true;

            OwnerDrawItemList(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawItemTile(ListViewOwnerDrawItemTileEventArgs args)
        {
            if (OwnerDrawItemTile == null)
                return true;

            OwnerDrawItemTile(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawItemDetails(ListViewOwnerDrawItemDetailsEventArgs args)
        {
            if (OwnerDrawItemDetails == null)
                return true;

            OwnerDrawItemDetails(this, args);
            return args.DrawDefault;
        }

        protected bool OnOwnerDrawSelectedItemDetailsFullRowBackground(
            ListViewOwnerDrawSelectedItemDetailsFullRowBackgroundEventArgs args)
        {
            if (OwnerDrawSelectedItemDetailsFullRowBackground == null)
                return true;

            OwnerDrawSelectedItemDetailsFullRowBackground(this, args);
            return args.DrawDefault;
        }

        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (!(sender is ListView lv))
                return;

            //setup arg defaults
            var textBounds = e.Bounds;
            var textFlags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            var imageBounds = Rectangle.Empty;
            var imageList = lv.SmallImageList;

            //if there is an image defined
            if ((imageList != null) && (e.Header.ImageIndex != -1))
            {
                //get the image location and readjust the text bounds to match
                //stock listview
                textBounds.Shrink(6, 0, 0, 0);
                imageBounds = new Rectangle(textBounds.Location, imageList.ImageSize);
                textBounds.Shrink(6, 0, 0, 0);
                textBounds.Shrink(imageList.ImageSize.Width, 0, 0, 0);

                //center image vertically
                imageBounds.Y -= ((imageList.ImageSize.Height / 2) - (e.Bounds.Height / 2));

                //TODO: fix image draw location when center/right alignment
            }

            //Adjust text bounds to match stock listview and setup
            //flags
            if (string.IsNullOrEmpty(e.Header.Text))
            {
                textFlags = TextFormatFlags.Default;
                textBounds = Rectangle.Empty;
            }
            else
            {
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Left:
                        textBounds.Shrink(3, 0, 3, 2);
                        textFlags |= TextFormatFlags.Left;
                        break;

                    case HorizontalAlignment.Center:
                        textBounds.Shrink(3, 0, 3, 2);
                        textFlags |= TextFormatFlags.HorizontalCenter;
                        break;

                    case HorizontalAlignment.Right:
                        textBounds.Shrink(3, 0, 2, 2);
                        textFlags |= TextFormatFlags.Right;
                        break;
                }
            }

            //Adjust text position when clicking the header
            //to make it look like a button
            if (e.State.HasFlag(ListViewItemStates.Selected))
            {
                textBounds.Shrink(1, 2, 0, 0);
            }

            //Trigger event and draw default options if defined
            var args = new ListViewOwnerDrawColumnHeaderEventArgs(e,
                textBounds, textFlags,
                imageBounds, imageList);

            if (OnDrawListViewColumnHeaderOwnerDrawHelperEventArgs(args))
            {
                args.DrawBackground();
                args.DrawImage();
                args.DrawText();
            }
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (!(sender is ListView lv))
                return;

            /* Because of a bug in the underlying Win32 control, the DrawItem event occurs without accompanying
             * DrawSubItem events once per row in the details view when the mouse pointer moves over the row,
             * causing anything painted in a DrawSubItem event handler to be painted over by a custom
             * background drawn in a DrawItem event handler. See the example in the OwnerDraw reference
             * topic for a workaround that invalidates each row when the extra event occurs. An alternative
             * workaround is to put all your custom drawing code in a DrawSubItem event handler and paint the
             * background for the entire item (including subitems) only when the
             * DrawListViewSubItemEventArgs.ColumnIndex value is 0.
             *
             * See remarks:
             * https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.drawitem?view=netframework-4.8#remarks
             * https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.ownerdraw?view=netframework-4.8#examples
             *
             * HACK: Just draw the details background in the DrawItem() event when FullRowSelect
             *       and item is selected, instead of the subitem event.
             */
            if (lv.View == View.Details)
            {
                if (lv.FullRowSelect && e.Item.Selected)
                {
                    var backgroundargs = new ListViewOwnerDrawSelectedItemDetailsFullRowBackgroundEventArgs(e);

                    if (OnOwnerDrawSelectedItemDetailsFullRowBackground(backgroundargs))
                    {
                        backgroundargs.DrawBackground();
                        backgroundargs.DrawFocusRectangle();
                    }
                }

                //Handle View.Details drawing in DrawSubItem(), not here
                return;
            }

            //setup arg defaults
            Rectangle textBounds = e.Bounds;
            TextFormatFlags textFlags = TextFormatFlags.Default;
            ImageList imageList;
            var imageBounds = Rectangle.Empty;
            ListViewOwnerDrawItemEventArgs args;
            bool drawDefault;

            //Adjust text bounds to match stock listview and setup
            //flags and imagelist, image positions. These numbers
            //generated by testing, trial and error
            switch (lv.View)
            {
                case View.SmallIcon:
                    textBounds.Shrink(2);
                    textFlags |= TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
                    imageList = lv.SmallImageList;

                    if (imageList != null)
                    {
                        imageBounds = new Rectangle(e.Bounds.Location, imageList.ImageSize);
                        textBounds.Shrink(imageList.ImageSize.Width, 0, 0, 0);
                    }

                    //Trigger event and get draw default
                    args = new ListViewOwnerDrawItemEventArgs(e,
                        textBounds, textFlags, imageBounds, imageList);
                    drawDefault = OnOwnerDrawItemSmallIcon(args);
                    break;

                case View.LargeIcon:
                    textBounds.Shrink(2, 5, 2, 2);
                    if (!e.State.HasFlag(ListViewItemStates.Focused))
                        textFlags |= TextFormatFlags.HorizontalCenter |
                            TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
                    else
                        textFlags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

                    imageList = lv.LargeImageList;

                    if (imageList != null)
                    {
                        var imageLocation = new Point(
                            e.Bounds.X + (e.Bounds.Width / 2) - (imageList.ImageSize.Width / 2), e.Bounds.Y + 2);
                        imageBounds = new Rectangle(imageLocation, imageList.ImageSize);
                        textBounds.Shrink(0, imageList.ImageSize.Height, 0, 0);
                    }

                    //Trigger event and get draw default
                    args = new ListViewOwnerDrawItemEventArgs(e,
                        textBounds, textFlags, imageBounds, imageList);
                    drawDefault = OnOwnerDrawItemLargeIcon(args);
                    break;

                case View.Tile:
                    textBounds.Shrink(6, 0, 0, 0);
                    textFlags |= TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
                    imageList = lv.LargeImageList;

                    if (imageList != null)
                    {
                        var imageLocation = new Point(
                            e.Bounds.X + 2, e.Bounds.Y + (e.Bounds.Height / 2) - (imageList.ImageSize.Height / 2));
                        imageBounds = new Rectangle(imageLocation, imageList.ImageSize);
                        textBounds.Shrink(imageList.ImageSize.Width, 0, 0, 0);
                    }

                    var sz = TextRenderer.MeasureText(e.Graphics, e.Item.Text, e.Item.Font, textBounds.Size, textFlags);
                    var lines = Math.Max(lv.Columns.Count, 1);

                    if (sz.Height * lines <= textBounds.Height)
                    {
                        textBounds.Shrink(0, (textBounds.Height - (Math.Max(lv.Columns.Count, 1) * sz.Height)) / 2, 0, 0);
                    }
                    else
                    {
                        lines = textBounds.Height / sz.Height;
                    }

                    var tileBounds = textBounds;
                    tileBounds.Shrink(0, sz.Height, 0, 0);
                    textBounds.Height = sz.Height;

                    //Trigger event and get draw default
                    var tileargs = new ListViewOwnerDrawItemTileEventArgs(e,
                        textBounds, textFlags, imageBounds, imageList, tileBounds, lines);
                    args = tileargs;
                    drawDefault = OnOwnerDrawItemTile(tileargs);
                    break;

                case View.List:
                    textBounds.Shrink(2, 0, 2, 0);
                    textFlags |= TextFormatFlags.Left | TextFormatFlags.EndEllipsis |
                        TextFormatFlags.NoPadding;
                    imageList = lv.SmallImageList;

                    if (imageList != null)
                    {
                        imageBounds = new Rectangle(e.Bounds.Location, imageList.ImageSize);
                        textBounds.Shrink(imageList.ImageSize.Width, 0, 0, 0);
                        textFlags |= TextFormatFlags.VerticalCenter;
                    }

                    //Trigger event and get draw default
                    args = new ListViewOwnerDrawItemEventArgs(e,
                        textBounds, textFlags, imageBounds, imageList);
                    drawDefault = OnOwnerDrawItemList(args);
                    break;

                default:
                    return;
            }

            //Trigger event and draw default options if defined
            if (drawDefault)
            {
                args.DrawBackground();
                args.DrawImage();
                args.DrawText();
                args.DrawFocusRectangle();
            }
        }

        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var lv = sender as ListView;

            if (lv.View != View.Details)
                return;

            //setup arg defaults
            Rectangle textBounds = e.Bounds;
            TextFormatFlags textFlags = TextFormatFlags.NoPadding |
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            ImageList imageList = null;
            var imageBounds = Rectangle.Empty;

            //Adjust text bounds to match stock listview and setup
            //flags and imagelist, image positions. These numbers
            //generated by testing
            switch (lv.Columns[e.ColumnIndex].TextAlign)
            {
                case HorizontalAlignment.Left:
                    textBounds.Shrink(6, 2, 2, 2);
                    textFlags |= TextFormatFlags.Left;
                    break;

                case HorizontalAlignment.Center:
                    /*if (e.ColumnIndex == 0)
                        textRect.Shrink(6, 2, 2, 2);
                    else*/
                    textBounds.Shrink(3, 2, 3, 2);
                    textFlags |= TextFormatFlags.HorizontalCenter;
                    break;

                case HorizontalAlignment.Right:
                    /*if (e.ColumnIndex == 0)
                        textRect.Shrink(6, 2, 2, 2);
                    else*/
                    textBounds.Shrink(2, 2, 6, 2);
                    textFlags |= TextFormatFlags.Right;
                    break;
            }

            //Get image only on column 0
            if (e.ColumnIndex == 0)
            {
                imageList = lv.SmallImageList;
                if (imageList != null)
                {
                    imageBounds = new Rectangle(new Point(e.Bounds.X + 4, e.Bounds.Y), imageList.ImageSize);
                    textBounds.Shrink(lv.SmallImageList.ImageSize.Width, 0, 0, 0);
                }
            }

            //Trigger event and draw default options if defined
            var args = new ListViewOwnerDrawItemDetailsEventArgs(e,
                textBounds, textFlags,
                imageBounds, imageList);

            if (OnOwnerDrawItemDetails(args))
            {
                args.DrawBackground();
                args.DrawImage();
                args.DrawText();
                args.DrawFocusRectangle();
            }
        }
    }
}
