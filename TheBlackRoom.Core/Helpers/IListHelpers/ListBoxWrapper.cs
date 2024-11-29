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
using System.Collections.Generic;


namespace TheBlackRoom.System.Helpers.IListHelpers
{
    /// <summary>
    /// Wrapper around an `IList` to provide basic ListBox functionality,
    /// without any drawing functionality. (Similar to how BindingSource
    /// maintains a position, but this includes scrolling functionality)
    ///
    /// Notes:
    /// - Assumes item heights are all the same size
    ///
    /// Features:
    /// - Maximum (Visible) ItemCount
    /// - Selected Index
    /// - Select Next/Prev/Top/Bottom/Page, CanSelect Next/Prev
    /// - Top Index
    /// - Scroll Next/Prev/Top/Bottom/Page, CanSCroll Next/Prev
    /// - Support for partial display of last item
    /// - Events and methods to map to System.Windows.Forms.Scrollbar control
    ///
    /// TODO:
    /// - If SelectedIndex==0 and TopIndex is >0
    ///   and then SelectPreviousItem() is called:
    ///   Should TopIndex be set to 0?
    ///   (Should all Select*() calls adjust TopIndex,
    ///    even if SelectedIndex doesn't change?)
    /// - Should there be a way to set SelectedIndex without
    ///   adjusting TopIndex?
    /// </summary>
    /// <typeparam name="T">Item Type</typeparam>
    public class ListBoxWrapper<T>
    {
        public ListBoxWrapper() { }
        public ListBoxWrapper(IList<T> ListItems) { }
        public ListBoxWrapper(IList<T> ListItems, float MaximumItemCount) { }
        public ListBoxWrapper(float MaximumItemCount) { }

        /// <summary>
        /// List of items to wrap with a ListBox
        /// </summary>
        public IList<T> ListItems
        {
            get => _ListItems;
            set
            {
                if (_ListItems == value)
                    return;

                _ListItems = value;
                SelectedIndex = -1;

                AdjustScrollParameters();
            }
        }
        private IList<T> _ListItems = null;

        /// <summary>
        /// Maximum number of visible items the ListBoxWrapper can display.
        /// Truncate to an int value to exclude partial items, otherwise
        /// partial items will be displayed
        /// </summary>
        public float MaximumItemCount
        {
            get => _MaximumItemCount;
            set
            {
                if (_MaximumItemCount == value)
                    return;

                _MaximumItemCount = value;

                //Adjust view when resizing
                if (TopIndex > InvisibleItemCount)
                    TopIndex = InvisibleItemCount;

                AdjustScrollParameters();
            }
        }
        private float _MaximumItemCount = 1.0f;

        /// <summary>
        /// Total number of items inside ListBoxWrapper
        /// </summary>
        public int ItemCount => ListItems?.Count ?? 0;

        /// <summary>
        /// Number of items visible in ListBoxWrapper (includes any partially visible items)
        /// </summary>
        public int VisibleItemCount => Math.Min(ItemCount, (int)Math.Ceiling(MaximumItemCount));

        /// <summary>
        /// Number of items fully visible in ListBoxWrapper (excludes partially visible items)
        /// </summary>
        public int VisibleFullItemCount => Math.Min(ItemCount, (int)MaximumItemCount);

        /// <summary>
        /// Number of hidden items in ListBoxWrapper (includes any partially visible items)
        /// </summary>
        protected int InvisibleItemCount => Math.Max(0, ItemCount - VisibleFullItemCount);

        /// <summary>
        /// Currently selected index of ListBoxWrapper, -1 for no selection
        /// </summary>
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                //contrain value to -1, or from 0-ListItemCount-1)
                var constrainedIndex = (ListItems == null) ? -1 :
                    Math.Max(-1, Math.Min(ListItems.Count - 1, value));

                if (_SelectedIndex == constrainedIndex)
                    return;

                _SelectedIndex = constrainedIndex;

                if (_SelectedIndex != -1)
                {
                    /* Adjust TopIndex - The +1 in the first if() accounts for displaying partial
                     * items and allows the TopIndex to go past if needed */
                    if (TopIndex + VisibleFullItemCount < _SelectedIndex + 1)
                        TopIndex = _SelectedIndex - VisibleFullItemCount + 1;
                    else if (_SelectedIndex < TopIndex)
                        TopIndex = _SelectedIndex;
                }

                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private int _SelectedIndex = -1;

        /// <summary>
        /// Currently selected item inside ListBoxWrapper
        /// </summary>
        public T SelectedItem
        {
            get
            {
                if ((ListItems == null) || (SelectedIndex < 0) || (SelectedIndex >= ListItems.Count))
                    return default(T);

                return ListItems[SelectedIndex];
            }
            set => SelectedIndex = ListItems?.IndexOf(value) ?? -1;
        }

        /// <summary>
        /// Returns the given item in the ListBoxWrapper by index
        /// </summary>
        /// <param name="index">Item index</param>
        /// <returns>Item</returns>
        public T this[int index] => ListItems[index];

        /// <summary>
        /// Index of the first visible item of ListBoxWrapper
        /// </summary>
        public int TopIndex
        {
            get => _TopIndex;
            set
            {
                //Top Index range is from 0-InvisibleItemCount
                var constrainedIndex = Math.Max(0, Math.Min(InvisibleItemCount, value));

                if (_TopIndex == constrainedIndex)
                    return;

                _TopIndex = constrainedIndex;

                TopIndexChanged?.Invoke(this, EventArgs.Empty);

                ScrollPositionChanged?.Invoke(this, ScrollPosition);
            }
        }
        private int _TopIndex = 0;

        /// <summary>
        /// Index of last visible item of ListBoxWrapper, which includes any partially visible items
        /// </summary>
        public int BottomIndex => Math.Min(TopIndex + Math.Min(ItemCount, VisibleItemCount), ItemCount);

        /// <summary>
        /// Index of last fully visible item of ListBoxWrapper
        /// </summary>
        public int BottomIndexFull => Math.Min(TopIndex + Math.Min(ItemCount, VisibleFullItemCount), ItemCount);

        /// <summary>
        /// Gets the visible items of ListBoxWrapper (includes any partially visible items)
        /// </summary>
        /// <returns>Visible items</returns>
        public IEnumerable<T> VisibleItems
        {
            get
            {
                for (int ix = TopIndex; ix < BottomIndex; ix++)
                    yield return ListItems[ix];
            }
        }

        /// <summary>
        /// Gets the fully visible items of ListBoxWrapper
        /// </summary>
        /// <returns>Fully visible items</returns>
        public IEnumerable<T> VisibleFullItems
        {
            get
            {
                for (int ix = TopIndex; ix < BottomIndexFull; ix++)
                    yield return ListItems[ix];
            }
        }

        /// <summary>
        /// To be called when items added or removed from ListItems
        /// </summary>
        /// <param name="listItemsCleared">Set to true of the list has been fully cleared</param>
        public virtual void OnListItemsChanged(bool listItemsCleared = false)
        {
            //If the list got smaller than the current selection,
            //change the selection to the end
            if (SelectedIndex >= ItemCount)
                SelectLastItem();

            AdjustScrollParameters();
        }

        #region SelectedIndex helper functions

        /// <summary>
        /// Unselects any selected item in the list
        /// </summary>
        public void UnselectItems() => SelectedIndex = -1;

        /// <summary>
        /// Selects the previous item in the list
        /// </summary>
        public void SelectPreviousItem() => SelectedIndex = Math.Max(0, SelectedIndex - 1);

        /// <summary>
        /// Selects the next item in the list
        /// </summary>
        public void SelectNextItem() => SelectedIndex++;

        /// <summary>
        /// Selects the previous item in the previous page of the list
        /// </summary>
        public void SelectPreviousPageItem() => SelectedIndex = Math.Max(0, SelectedIndex - VisibleFullItemCount + 1);

        /// <summary>
        /// Selects the next item in the next page of the list
        /// </summary>
        public void SelectNextPageItem() => SelectedIndex += VisibleFullItemCount - 1;

        /// <summary>
        /// Selects the first item in the list
        /// </summary>
        public void SelectFirstItem() => SelectedIndex = 0;

        /// <summary>
        /// Selects the last item in the list
        /// </summary>
        public void SelectLastItem() => SelectedIndex = ItemCount - 1;

        /// <summary>
        /// Flag indicating there is a previous item to select
        /// </summary>
        public bool CanSelectPreviousItem => SelectedIndex > 0;

        /// <summary>
        /// Flag indicating there is a next item to select
        /// </summary>
        public bool CanSelectNextItem => SelectedIndex < ItemCount - 1;

        #endregion

        #region TopIndex helper methods

        /// <summary>
        /// Scrolls the ListBox so the previous item is visible
        /// </summary>
        public void ScrollPreviousItem() => TopIndex--;

        /// <summary>
        /// Scrolls the ListBox so the next item is visible
        /// </summary>
        public void ScrollNextItem() => TopIndex++;

        /// <summary>
        /// Scrolls the ListBox so the previous page of items are visible
        /// </summary>
        public void ScrollPreviousPageItem() => TopIndex -= (int)MaximumItemCount;

        /// <summary>
        /// Scrolls the ListBox so the next page of items are visible
        /// </summary>
        public void ScrollNextPageItem() => TopIndex += (int)MaximumItemCount;

        /// <summary>
        /// Scrolls the ListBox so the first item is visible
        /// </summary>
        public void ScrollFirstItem() => TopIndex = 0;

        /// <summary>
        /// Scrolls the ListBox so the last item is visible
        /// </summary>
        public void ScrollLastItem() => TopIndex = ItemCount - 1;

        /// <summary>
        /// Flag indicating the ListBoxWrapper can scroll
        /// </summary>
        public bool CanScroll => InvisibleItemCount != 0;

        /// <summary>
        /// Flag indicating there are previous items to scroll to
        /// </summary>
        public bool CanScrollPreviousItem => (ItemCount > 0) && (TopIndex > 0);

        /// <summary>
        /// Flag indicating there are next items to scroll to
        /// </summary>
        public bool CanScrollNextItem => (ItemCount > 0) && (TopIndex < InvisibleItemCount);
        #endregion

        #region Scrolling
        private void AdjustScrollParameters()
        {
            var args = new ScrollParametersEventArgs()
            {
                CanScroll = CanScroll,
                ItemCount = ItemCount,
                VisibleItemCount = VisibleItemCount,
                VisibleFullItemCount = VisibleFullItemCount,
            };

            if ((ScrollParameters.CanScroll != args.CanScroll) ||
                (ScrollParameters.ItemCount != args.ItemCount) ||
                (ScrollParameters.VisibleItemCount != args.VisibleItemCount) ||
                (ScrollParameters.VisibleFullItemCount != args.VisibleFullItemCount))
            {
                ScrollParameters = args;
                ScrollParametersChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Returns the current ScrollParameters
        /// </summary>
        public ScrollParametersEventArgs ScrollParameters { get; private set; }
            = new ScrollParametersEventArgs();

        /// <summary>
        /// Returns the current ScrollPosition
        /// </summary>
        public ScrollPositionEventArgs ScrollPosition => new ScrollPositionEventArgs()
        {
            Position = TopIndex,
        };

        /// <summary>
        /// Sets the scroll position using a scaled item height and scroll value
        /// </summary>
        /// <param name="Value">ScrollEventArgs.NewValue or System.Windows.Forms.ScrollBar.Value property</param>
        /// <param name="ItemHeight">Item Height</param>
        public void SetPositionFromScrollBarValue(float Value, float ItemHeight)
        {
            if (ItemHeight > 0)
                TopIndex = (int)((Value + ItemHeight - 1) / ItemHeight);
        }

        /// <summary>
        /// Occurs when the SelectedIndex property has changed
        /// </summary>
        public event EventHandler<EventArgs> SelectedIndexChanged;

        /// <summary>
        /// Occurs when the TopIndex property has changed
        /// </summary>
        public event EventHandler<EventArgs> TopIndexChanged;

        /// <summary>
        /// Occurs when the ScrollParameters property has changed
        /// </summary>
        public event EventHandler<ScrollParametersEventArgs> ScrollParametersChanged;

        /// <summary>
        /// Occurs when the ScrollPosition property has changed
        /// </summary>
        public event EventHandler<ScrollPositionEventArgs> ScrollPositionChanged;

        #endregion
    }

    /// <summary>
    /// Provides data for the ScrollParameters event of the ListBoxWrapper,
    /// and methods to map ScrollParameters properties to the properties of a
    /// System.Windows.Forms.ScrollBar
    /// </summary>
    public class ScrollParametersEventArgs : EventArgs
    {
        const int Minimum = 0;
        const int SmallChange = 1;

        /// <summary>
        /// Flag indicating if an ListBoxWrapper can be scrolled
        /// </summary>
        public bool CanScroll { get; set; } = false;

        /// <summary>
        /// The total number of items
        /// </summary>
        public int ItemCount { get; set; } = 1;

        /// <summary>
        /// The number of visible items
        /// </summary>
        public int VisibleItemCount { get; set; } = 0;

        /// <summary>
        /// The number of fully visible items
        /// </summary>
        public int VisibleFullItemCount { get; set; } = 0;

        /// <summary>
        /// Returns a value usable for setting the System.Windows.Forms.ScrollBar.Minimum property
        /// </summary>
        /// <param name="ItemHeight">ListBox Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.Minimum property</returns>
        public int GetScrollBarMinimum(float ItemHeight) => (int)Math.Round(Minimum * ItemHeight);

        /// <summary>
        /// Returns a value usable for setting the System.Windows.Forms.ScrollBar.Maximum property
        /// </summary>
        /// <param name="ItemHeight">ListBox Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.Maximum property</returns>
        public int GetScrollBarMaximum(float ItemHeight) => (int)Math.Round(ItemCount * ItemHeight);

        /// <summary>
        /// Returns a value usable for setting the System.Windows.Forms.ScrollBar.SmallChange property
        /// </summary>
        /// <param name="ItemHeight">ListBox Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.SmallChange property</returns>
        public int GetScrollBarSmallChange(float ItemHeight) => (int)Math.Round(SmallChange * ItemHeight);

        /// <summary>
        /// Returns a value usable for setting the System.Windows.Forms.ScrollBar.LargeChange property,
        /// which scrolls the ListBoxWrapper by a full page
        /// </summary>
        /// <param name="ItemHeight">ListBox Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.LargeChange property</returns>
        public int GetScrollBarLargeChange(float ItemHeight) =>
            (int)Math.Round(VisibleFullItemCount * ItemHeight);

        /// <summary>
        /// Returns a value usable for setting the System.Windows.Forms.ScrollBar.LargeChange property,
        /// which scrolls the ListBoxWrapper by a full page less one item
        /// </summary>
        /// <param name="ItemHeight">ListBox Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.LargeChange property</returns>
        public int GetScrollBarLargeChangeAlt(float ItemHeight) => (int)Math.Round(
            Math.Max(VisibleFullItemCount - 1, 1) * ItemHeight);
    }

    /// <summary>
    /// Provides data for the ScrollPosition event of the ListBoxWrapper
    /// and methods to map ScrollPosition properties to the properties of a
    /// System.Windows.Forms.ScrollBar
    /// </summary>
    public class ScrollPositionEventArgs : EventArgs
    {
        /// <summary>
        /// The normalized position of the scrollbar
        /// </summary>
        public float Position { get; set; } = 0;

        /// <summary>
        /// Returns the scroll position scaled by the item height
        /// </summary>
        /// <param name="ItemHeight">Item Height</param>
        /// <returns>Value to use for the System.Windows.Forms.ScrollBar.Value property</returns>
        public int GetScrollBarValue(float ItemHeight) => (int)(Position * ItemHeight);
    }
}
