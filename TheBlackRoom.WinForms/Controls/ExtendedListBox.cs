/*
 * Copyright (c) 2024 Christopher Hayes
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
using System.Drawing;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Controls
{
    /// <summary>
    /// Extended ListBox
    ///
    /// Workarounds:
    ///   Stop flicker on OwnerDraw listboxes and reduces number of draw events
    /// </summary>
    public class ExtendedListBox : ListBox
    {
        public ExtendedListBox()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public override DrawMode DrawMode
        {
            get => base.DrawMode;
            set
            {
                if (base.DrawMode == value)
                    return;

                base.DrawMode = value;

                switch (value)
                {
                    default:
                    case DrawMode.Normal:
                        SetStyle(ControlStyles.UserPaint, false);
                        break;

                    case DrawMode.OwnerDrawFixed:
                    case DrawMode.OwnerDrawVariable:
                        /* Enable UserPaint option to be able to ignore
                         * erase background */
                        SetStyle(ControlStyles.UserPaint, true);
                        break;
                }

                UpdateStyles();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            /* When ControlStyles.UserPaint is set to true, OnPaint() does no
             * drawing. Not drawing therefore means not erasing the background,
             * which seems to be the root causes of the ListBox flicker.
             * Since an OwnerDraw ListBox relies on OnDrawItem() to handle all
             * item drawing, OnPaint() can be just call OnDrawItem() for each
             * visible item to mimic the internal draw logic.
             * Additional checks are made to only call OnDrawItem() when
             * required, which reduces the number of draw events. */
            switch (DrawMode)
            {
                case DrawMode.OwnerDrawFixed:
                case DrawMode.OwnerDrawVariable:

                    /* Draw background only where needed */
                    using (var backgroundBrush = new SolidBrush(BackColor))
                        e.Graphics.FillRectangle(backgroundBrush, e.ClipRectangle);

                    /* Draw each item */
                    for (int ix = 0; ix < Items.Count; ix++)
                    {
                        var itemRect = GetItemRectangle(ix);

                        /* Don't draw the item if its not in the clip rectangle */
                        if (!e.ClipRectangle.IntersectsWith(itemRect))
                                continue;

                        /* Determine if the item is selected or not */
                        bool selected;

                        switch (SelectionMode)
                        {
                            default:
                            case SelectionMode.None:
                                selected = false;
                                break;

                            case SelectionMode.One:
                                selected = (SelectedIndex == ix);
                                break;

                            case SelectionMode.MultiSimple:
                            case SelectionMode.MultiExtended:
                                selected = SelectedIndices.Contains(ix);
                                break;
                        }

                        /* Generate draw args, call OnDrawItem() and allow
                         * OwnerDraw to take over drawing */
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, itemRect,
                            ix, selected ? DrawItemState.Selected : DrawItemState.Default,
                            ForeColor, BackColor));
                    }
                    break;
            }

            /* If not OwnerDraw, then this will do a regular draw. Either way,
             * this will fire the Paint event. */
            base.OnPaint(e);
        }
    }
}
