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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Binding
{
    /// <summary>
    /// Displays the contents of a DataSource in a list format
    ///
    /// Reason: In theory, this control should raise less DrawItem
    ///         events compared to a standard ListBox, when:
    ///         (DrawMode == OwnerDrawFixed) && (DataSource != null)
    ///
    /// Features:
    /// - DoubleBuffered
    /// - DrawModes of Normal and OwnerDrawFixed (with DrawItem event)
    /// - SelectedIndexChanged event
    /// - Single item selection via mouse
    /// - Basic keyboard item selection
    /// - Scrollbar
    /// - Mouse wheel scrolling
    /// - BeginUpdate()/EndUpdate()
    /// </summary>
    public class BoundListControl : ListControl
    {
        public BoundListControl()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.Selectable, true);

            this.BackColor = SystemColors.Window;

            SetVisibleItemCounts();
            AddScrollBar();
        }


        /// <summary>
        /// Items contained in list control, null when no datasource
        /// </summary>
        private List<object> _Items = null;

        /// <summary>
        /// Number of items in list control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ItemCount => _Items?.Count ?? 0;

        /// <summary>
        /// Read only view of items in list control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ReadOnlyCollection<object> Items { get; private set; }

        /// <summary>
        /// Maximum number of fully visible items the listcontrol can display
        /// </summary>
        protected int MaximumVisibleItemCount { get; private set; } = 0;

        /// <summary>
        /// Number of items fully visible on list control
        /// </summary>
        protected int VisibleItemCount { get; private set; } = 0;


        /// <summary>
        /// Number of items hidden on list control, includes partially hidden items
        /// </summary>
        protected int InvisibleItemCount { get; private set; } = 0;


        protected override bool AllowSelection => true;

        /// <summary>
        /// Interior padding of control
        /// </summary>
        protected Padding InteriorPadding = new Padding(2, 2, 2, 2);

        protected override Size DefaultSize => new Size(200, 400);

        private int updateCount = 0;

        /// <summary>
        /// Raised when DrawModeFixed is true (OwnerDraw)
        /// </summary>
        public event EventHandler<DrawItemEventArgs> DrawItem;

        /// <summary>
        /// Raised when the selected index changes
        /// </summary>
        public event EventHandler<EventArgs> SelectedIndexChanged;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                var tmpValue = (_Items == null) ? -1 :
                    Math.Max(-1, Math.Min(_Items.Count - 1, value));

                if (_SelectedIndex == tmpValue)
                    return;

                var oldIndex = _SelectedIndex;
                _SelectedIndex = tmpValue;

                //invalidating 2 rectangles at once, will combine then into a single
                //rectangle encompassing both rectangles (union, even though no intersection)
                InvalidateListIndex(oldIndex);
                Update(); //force a paint between invalidates
                InvalidateListIndex(_SelectedIndex);

                OnSelectedIndexChanged(EventArgs.Empty);

                /* Note: Setting CurrencyManager.Position will always call EndCurrentEdit(),
                 *       even if the values are the same, so avoid ending edits */
                if ((DataManager != null) && (DataManager.Position != _SelectedIndex))
                    DataManager.Position = _SelectedIndex;

                /* Adjust TopIndex - The + 1 in the first if() accounts for displaying partial
                 * items and allows the TopIndex to go past if needed */
                if (TopIndex + VisibleItemCount < SelectedIndex + 1)
                    TopIndex = SelectedIndex - VisibleItemCount + 1;
                else if (SelectedIndex < TopIndex)
                    TopIndex = SelectedIndex;
            }
        }
        private int _SelectedIndex = -1;


        [Category("Behavior")]
        [DefaultValue(13)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                //don't allow item height to be set when in normal mode
                if (DrawMode == DrawMode.Normal)
                    return;

                if (value <= 0)
                    return;

                _itemHeight = value;

                var oldCount = SetVisibleItemCounts();
                SetScrollbarRange();
                SyncScrollbarPosition();

                if (oldCount != VisibleItemCount)
                    Invalidate();
            }
        }
        private int _itemHeight = 13;

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            var sz = TextRenderer.MeasureText("Wy", this.Font);

            //bypass the Property setter to allow changes to
            //item height when the DrawMode is normal
            if (_itemHeight != sz.Height)
            {
                _itemHeight = sz.Height;

                var oldCount = SetVisibleItemCounts();
                SetScrollbarRange();
                SyncScrollbarPosition();

                if (oldCount != VisibleItemCount)
                    Invalidate();
            }
        }

        private int SetVisibleItemCounts()
        {
            var oldCount = VisibleItemCount;

            MaximumVisibleItemCount = (ClientSize.Height - InteriorPadding.Top - InteriorPadding.Bottom) / ItemHeight;

            VisibleItemCount = Math.Min(MaximumVisibleItemCount, ItemCount);
            InvisibleItemCount = Math.Max(0, ItemCount - VisibleItemCount);

            return oldCount;
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopIndex
        {
            get => _TopIndex;
            set
            {
                var tmpValue = Math.Max(0, Math.Min(InvisibleItemCount, value));

                if (_TopIndex == tmpValue)
                    return;

                _TopIndex = tmpValue;
                SyncScrollbarPosition();
                Invalidate();
            }
        }
        private int _TopIndex = 0;


        [Category("Behavior")]
        [DefaultValue(DrawMode.Normal)]
        public DrawMode DrawMode
        {
            get => _DrawMode;
            set
            {
                if (value == DrawMode.OwnerDrawVariable)
                    return;

                _DrawMode = value;
                Invalidate();
            }
        }
        private DrawMode _DrawMode = DrawMode.Normal;

        [Category("Appearance")]
        [DefaultValue(typeof(Color), nameof(SystemColors.Window))]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }


        [Category("Appearance")]
        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle BorderStyle
        {
            get => _BorderStyle;
            set
            {
                _BorderStyle = value;
                Invalidate();
            }
        }
        BorderStyle _BorderStyle = BorderStyle.Fixed3D;

        protected override void RefreshItem(int index)
        {
            InvalidateListIndex(index);
        }

        protected override void RefreshItems()
        {
            base.RefreshItems();
            Invalidate();
        }

        protected override void SetItemCore(int index, object value)
        {
            if ((value == null) || (_Items == null) ||
                (index < 0) || (index >= _Items.Count))
                return;

            _Items[index] = value;

            InvalidateListIndex(index);
        }

        protected override void SetItemsCore(IList items)
        {
            if (_Items == null)
            {
                _Items = new List<object>();
                Items = new ReadOnlyCollection<object>(_Items);
            }
            else
            {
                _Items.Clear();
            }

            for (int i = 0; i < items.Count; i++)
                _Items.Add(items[i]);

            SetVisibleItemCounts();
            SetScrollbarRange();

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var oldCount = SetVisibleItemCounts();

            //Adjust view when resizing
            if (TopIndex > InvisibleItemCount)
                TopIndex = InvisibleItemCount;

            SetScrollbarPosition();
            SetScrollbarRange();

            if (oldCount != VisibleItemCount)
            {
                if (oldCount < VisibleItemCount)
                {
                    //only invalidate portion of listcontrol from the old top to the bottom of control
                    var oldTop = ((TopIndex + oldCount) * ItemHeight) + InteriorPadding.Top;
                    var itemBounds = new Rectangle(0, oldTop, ClientRectangle.Width, ClientRectangle.Height - oldTop);
                    Invalidate(itemBounds);
                }
            }
            else
            {
                //only invalidate portion of listcontrol without items, include outside of list padding
                var emptyTop = ((TopIndex + VisibleItemCount) * ItemHeight) + InteriorPadding.Top;
                var emptyBounds = new Rectangle(0, emptyTop, ClientRectangle.Width, ClientRectangle.Height - emptyTop);

                Invalidate(emptyBounds);
            }
        }

        protected void InvalidateListIndex(int index)
        {
            //ensure valid item
            if ((index < 0) || (index >= ItemCount))
                return;

            //items above topindex are hidden
            if (index < TopIndex)
                return;

            //items below topindex plus full page are hidden
            //add in +1 to account for partial items
            if (index > TopIndex + VisibleItemCount + 1)
                return;

            var bounds = new Rectangle(
                InteriorPadding.Left,
                InteriorPadding.Top,
                ClientRectangle.Width - InteriorPadding.Left - InteriorPadding.Right,
                ClientRectangle.Height - InteriorPadding.Top - InteriorPadding.Bottom);

            if (_scrollbar.Visible)
                bounds.Width -= (SystemInformation.VerticalScrollBarWidth);

            var itemTop = ((index - TopIndex) * ItemHeight) + InteriorPadding.Top;
            var itemBounds = new Rectangle(bounds.X, itemTop, bounds.Width, ItemHeight);

            Invalidate(itemBounds);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var bounds = new Rectangle(
                InteriorPadding.Left,
                InteriorPadding.Top,
                ClientRectangle.Width - InteriorPadding.Left - InteriorPadding.Right,
                ClientRectangle.Height - InteriorPadding.Top - InteriorPadding.Bottom);

            if (_scrollbar.Visible)
                bounds.Width -= (SystemInformation.VerticalScrollBarWidth);

            if (DesignMode)
            {
                TextRenderer.DrawText(e.Graphics, this.Name, this.Font,
                    bounds.Location, this.ForeColor, this.BackColor);
            }
            else
            {
                for (int itemIndex = TopIndex, y = bounds.Top; itemIndex < TopIndex + VisibleItemCount + 1; itemIndex++, y += ItemHeight)
                {
                    if (itemIndex >= ItemCount)
                        continue;

                    var itemBounds = new Rectangle(bounds.X, y, bounds.Width, ItemHeight);
                    if (!itemBounds.IntersectsWith(e.ClipRectangle))
                        continue;

                    DrawItemState state = DrawItemState.NoFocusRect;

                    if (itemIndex == SelectedIndex)
                        state |= DrawItemState.Selected;

                    if (!this.Enabled)
                        state |= DrawItemState.Disabled;

                    if (DrawMode == DrawMode.OwnerDrawFixed)
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, itemBounds, itemIndex,
                            state, this.ForeColor, this.BackColor));
                    }
                    else if (DrawMode == DrawMode.Normal)
                    {
                        var itemBackColour = this.BackColor;
                        var itemForeColour = this.ForeColor;

                        if (state.HasFlag(DrawItemState.Selected))
                        {
                            itemBackColour = SystemColors.Highlight;
                            itemForeColour = SystemColors.HighlightText;
                        }

                        if (state.HasFlag(DrawItemState.Disabled))
                        {
                            itemForeColour = Color.Gray;
                        }

                        using (var brush = new SolidBrush(itemBackColour))
                            e.Graphics.FillRectangle(brush, itemBounds);

                        var itemText = GetItemText(_Items[itemIndex]);

                        TextRenderer.DrawText(e.Graphics, itemText, this.Font, itemBounds, itemForeColour, itemBackColour,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
                    }
                }
            }

            switch (BorderStyle)
            {
                case BorderStyle.None:
                    if (DesignMode)
                    {
                        //TODO: Create and call OnPaintAdornments()
                    }
                    break;

                case BorderStyle.FixedSingle:
                    ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                        SystemColors.WindowFrame, ButtonBorderStyle.Solid);
                    break;


                case BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle);
                    break;
            }
        }

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            DrawItem.Invoke(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();

            if (e.Button == MouseButtons.Left)
            {
                var itemIndex = ((e.Y - InteriorPadding.Top) / ItemHeight) + TopIndex;

                if ((itemIndex < 0) || (itemIndex >= ItemCount))
                    return;

                SelectedIndex = itemIndex;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            var scrollLines = SystemInformation.MouseWheelScrollLines;

            //-1 is the "One screen at a time" mouse option
            if (scrollLines == -1)
                scrollLines = ClientSize.Height / ItemHeight;

            if (scrollLines <= 0)
                scrollLines = 1;

            if (e.Delta > 0)
            {
                if (TopIndex > 0)
                    TopIndex -= Math.Min(scrollLines, TopIndex);
            }
            else if (e.Delta < 0)
            {
                if (TopIndex < _Items.Count - 1)
                    TopIndex += Math.Min(scrollLines, _Items.Count - 1 - TopIndex);
            }
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            if (this.DataManager == null)
            {
                _Items = null;
                Items = null;
                SelectedIndex = -1;
                SetVisibleItemCounts();
                SetScrollbarRange();
                Invalidate();
            }

        }

        protected override void OnDisplayMemberChanged(EventArgs e)
        {
            base.OnDisplayMemberChanged(e);

            //Update position as per ListBox
            if (this.DataManager != null)
                SelectedIndex = this.DataManager.Position;

            Invalidate();
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        public void BeginUpdate()
        {
            if (!IsHandleCreated)
                return;

            if (updateCount == 0)
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, 0, 0);

            updateCount++;
        }

        public void EndUpdate()
        {
            if (updateCount > 0)
            {
                updateCount--;

                if (updateCount == 0)
                {
                    NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, -1, 0);
                    Invalidate();
                }
            }
        }

        internal class NativeMethods
        {

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, int wParam, int lParam);

            public const int WM_SETREDRAW = 0xB;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetScrollbarPosition();
        }

        VScrollBar _scrollbar = null;

        private void AddScrollBar()
        {
            _scrollbar = new VScrollBar();
            _scrollbar.Minimum = 0;
            _scrollbar.SmallChange = 1;

            _scrollbar.ValueChanged += _scrollbar_ValueChanged;

            SetScrollbarPosition();
            SetScrollbarRange();

            this.Controls.Add(_scrollbar);
        }

        private void SetScrollbarPosition()
        {
            _scrollbar.Top = 1;
            _scrollbar.Left = this.ClientSize.Width - _scrollbar.Width - 1;
            _scrollbar.Height = this.ClientSize.Height - 2;
        }

        private void SetScrollbarRange()
        {
            if (InvisibleItemCount == 0)
            {
                if (_scrollbar.Visible != false)
                {
                    _scrollbar.Visible = false;
                    Invalidate();
                }
            }
            else
            {
                if (_scrollbar.Visible != true)
                {
                    _scrollbar.Visible = true;
                    Invalidate();
                }

                if (_scrollbar.Value > InvisibleItemCount * ItemHeight)
                    _scrollbar.Value = InvisibleItemCount * ItemHeight;

                _scrollbar.Maximum = ItemCount * ItemHeight;
                _scrollbar.SmallChange = ItemHeight;
                _scrollbar.LargeChange = Math.Max(0, this.ClientRectangle.Height - InteriorPadding.Top - InteriorPadding.Bottom);
            }
        }

        private void SyncScrollbarPosition()
        {
            _scrollbar.Value = TopIndex * ItemHeight;
        }

        private void _scrollbar_ValueChanged(object sender, EventArgs e)
        {
            TopIndex = (_scrollbar.Value + ItemHeight - 1) / ItemHeight;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Left:
                    SelectedIndex = Math.Max(0, SelectedIndex - 1);
                    break;

                case Keys.Down:
                case Keys.Right:
                    SelectedIndex++;
                    break;

                case Keys.PageUp:
                    SelectedIndex = Math.Max(0, SelectedIndex - VisibleItemCount - 1);
                    break;

                case Keys.PageDown:
                    SelectedIndex += VisibleItemCount - 1;
                    break;

                case Keys.Home:
                    SelectedIndex = 0;
                    break;

                case Keys.End:
                    SelectedIndex = ItemCount;
                    break;
            }
        }
    }
}
