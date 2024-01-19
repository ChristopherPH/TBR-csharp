/*
 * Copyright (c) 2023 Christopher Hayes
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
using System.ComponentModel;

namespace TheBlackRoom.System.Binding
{
    /// <summary>
    /// Provides a BindingList that:
    ///  Supports searching
    ///  Supports sorting (when sorted, new items will be added/inserted in sort order)
    ///  Adds events: DeletingItem, Clearing
    ///  Adds methods: AddRange, Find, ApplySort, RemoveSort
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class BindingListEx<T> : BindingList<T>, IBindingListEx
    {
        #region Constructores
        /// <inheritdoc />
        public BindingListEx() { }

        /// <inheritdoc />
        public BindingListEx(IList<T> list) : base(list) { }
        #endregion

        #region New Constructores
        /// <summary>
        /// Initializes a new instance of the BindingList class using default values
        /// nd adds the elements of the specified collection to the BindingList.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added the BindingList</param>
        public BindingListEx(IEnumerable<T> collection)
        {
            AddRange(collection);
        }
        #endregion

        #region Searching
        protected override bool SupportsSearchingCore => true;

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            for (int ix = 0; ix < Count; ix++)
                if (Equals(prop.GetValue(this[ix]), key))
                    return ix;

            return -1;
        }
        #endregion

        #region Sorting

        //Only support sorting if the underlying collection is a list
        protected override bool SupportsSortingCore => Items is List<T>;

        protected override PropertyDescriptor SortPropertyCore => _sortProperty;
        private PropertyDescriptor _sortProperty = null;

        protected override ListSortDirection SortDirectionCore => _sortDirection;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        protected override bool IsSortedCore => _sortComparer != null;
        private IComparer<T> _sortComparer = null;

        /// <summary>
        /// Flag to ignore sort order when adding an item via AddNew()
        /// </summary>
        private bool addItemUnsorted = false;

        /// <summary>
        /// Index of item added via AddNew()
        /// </summary>
        int addNewCoreItemIndex = -1;


        protected override void ApplySortCore(PropertyDescriptor propertyDescriptor,
            ListSortDirection listSortDirection)
        {
            //Save the sort parameters
            _sortProperty = propertyDescriptor;
            _sortDirection = listSortDirection;

            ApplySortInternal(new PropertyComparer(propertyDescriptor, listSortDirection));
        }

        protected override void RemoveSortCore()
        {
            _sortProperty = null;
            _sortDirection = ListSortDirection.Ascending;

            //if the list was sorted, remove the comparer
            //and reset the list
            if (_sortComparer != null)
            {
                _sortComparer = null;

                //Note: The original order was never saved,
                //so there isn't much point to reset the list
                //ResetBindings();
            }
        }

        protected virtual void ApplySortInternal(IComparer<T> sortComparer)
        {
            if (sortComparer == null)
                throw new ArgumentNullException(nameof(sortComparer));

            //need a List<T> for calling Sort()
            if (!(Items is List<T> list))
                return;

            try
            {
                //sort the list
                list.Sort(sortComparer);
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is ArgumentException)
            {
                //sort failed
                RemoveSortCore();
                return;
            }

            //Sort succeeded, raise a ListChanged event with
            //the Reset enumeration as per documentation.
            ResetBindings();

            //save the sort comparer
            _sortComparer = sortComparer;
        }

        protected override void SetItem(int index, T item)
        {
            //if an item is replaced in the list with
            //a different item, and the list is sorted,
            //then move the item if needed to maintain
            //the sort order
            if (IsSortedCore)
            {
                //Item is not yet added and list is sorted,
                //so GetSortedIndex() is valid to call
                var ix = GetSortedIndex(item);

                //Item needs to move, remove and re-insert
                //in new position
                if (ix != index)
                {
                    base.RemoveItem(index);

                    if (ix > index)
                        ix--;

                    base.InsertItem(ix, item);
                    return;
                }
            }

            base.SetItem(index, item);
        }

        protected override void InsertItem(int index, T item)
        {
            //if an item is added or inserted and the list is sorted,
            //then check if the new item needs to move to a different
            //index to keep the sort
            if (IsSortedCore)
            {
                //If the item was added via addNewCore(), don't
                //change the index and keep it at the end of the
                //list.
                if (!addItemUnsorted)
                    index = GetSortedIndex(item);

                addItemUnsorted = false;
            }

            base.InsertItem(index, item);
        }

        protected override object AddNewCore()
        {
            /* Work around to support ICancelAddNew on DataBound controls such
             * as the DataGridView:
             *
             * The DataGridView adds items to the BindingList via AddNew()
             * (which is not overridable, but AddNewCore() is) and expects
             * the newly added item to be the last item. It then allows the
             * item to be modified before being committed (EndNew()) or rolled
             * back (CancelNew()).
             *
             * The item can be sorted once it is committed in EndNew().
             *
             * AddNewCore() calls Add(), which calls InsertItem(), so
             * the flag addItemUnsorted can be set before calling
             * AddNewCore() which will ensure the item will not be
             * sorted during InsertItem().
             */
            addItemUnsorted = IsSortedCore;

            //Add new item
            var newItem =  base.AddNewCore();

            //Save index of new item (should be the last item)
            addNewCoreItemIndex = (newItem != null) ? IndexOf((T)newItem) : -1;

            return newItem;
        }

        public override void EndNew(int itemIndex)
        {
            //Commit the new item
            base.EndNew(itemIndex);

            //If the item committed is the same item that was added
            //via AddNew(), it is at the end of the list and the list
            //needs to be sorted.
            if ((addNewCoreItemIndex != -1) && (addNewCoreItemIndex == itemIndex))
            {
                addNewCoreItemIndex = -1;

                if (IsSortedCore)
                {
                    //TODO: The following code fails, but it would be
                    //      nicer to not have to fully reset the list
                    //      in the mean time, do a full sort.
                    //
                    //remove the item before calling GetSortedIndex()
                    //to ensure the list is still correctly sorted
                    //var item = this[itemIndex];
                    //base.RemoveItem(itemIndex);
                    //var ix = GetSortedIndex(item);
                    //base.InsertItem(ix, item);

                    //Resort the list using the existing sorter
                    ApplySortInternal(_sortComparer);
                }
            }
        }

        public override void CancelNew(int itemIndex)
        {
            //Note that base.CancelNew() calls EndNew(), so clear the
            //item index before calling Cancel(), to ensure that it
            //does not trigger a sort.
            if ((addNewCoreItemIndex != -1) && (addNewCoreItemIndex == itemIndex))
                addNewCoreItemIndex = -1;

            //Cancel the new item
            base.CancelNew(itemIndex);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            //if the list is sorted, and an item's property changed
            //that matches the sort property, then resort the list
            if (IsSortedCore &&
                (e.ListChangedType == ListChangedType.ItemChanged) &&
                (e.PropertyDescriptor?.Name == _sortProperty.Name))
            {
                //in a transaction, don't sort the list as EndNew()
                //will do that.
                if ((addNewCoreItemIndex != -1) && (addNewCoreItemIndex == e.NewIndex))
                {
                    base.OnListChanged(e);
                    return;
                }

                //remove the item before calling GetSortedIndex()
                //to ensure the list is still correctly sorted
                var item = this[e.NewIndex];
                base.RemoveItem(e.NewIndex);
                var ix = GetSortedIndex(item);
                base.InsertItem(ix, item);

                return;
            }

            base.OnListChanged(e);
        }

        /// <summary>
        /// Gets the index to add the given item at to keep the item sorted
        /// Note that the list must be sorted for this function to work.
        /// Note that duplicated items will be sorted, but may return an index
        ///   before or after the duplicated item (position within sort is undefined)
        /// </summary>
        /// <param name="item">Item to determine sorted index</param>
        /// <returns>-1 on error, index to insert item on success</returns>
        private int GetSortedIndex(T item)
        {
            if (!(Items is List<T> list)) //for BinarySearch()
                return -1;

            if (_sortComparer == null)
                return -1;

            if (list.Count == 0)
                return 0;

            if (_sortComparer.Compare(this[Count - 1], item) <= 0)
                return Count;

            if (_sortComparer.Compare(this[0], item) >= 0)
                return 0;

            var ix = list.BinarySearch(item, _sortComparer);
            if (ix < 0)
                ix = ~ix;

            return ix;
        }
        #endregion

        #region Property Comparer
        protected class PropertyComparer : IComparer<T>
        {
            ListSortDescriptionCollection _sorts;

            public PropertyComparer(PropertyDescriptor propertyDescriptor, ListSortDirection listSortDirection)
            {
                if (propertyDescriptor == null)
                    throw new ArgumentNullException(nameof(propertyDescriptor));

                _sorts = new ListSortDescriptionCollection(new ListSortDescription[] {
                    new ListSortDescription(propertyDescriptor, listSortDirection) });
            }

            public PropertyComparer(ListSortDescriptionCollection sorts)
            {
                if (sorts == null)
                    throw new ArgumentNullException(nameof(sorts));
                if (sorts.Count == 0)
                    throw new ArgumentException(nameof(sorts));

                _sorts = sorts;
            }

            public int Compare(T x, T y)
            {
                foreach (ListSortDescription sort in _sorts)
                {
                    var rc = Compare(x, y, sort);

                    if (rc != 0)
                        return rc;
                }

                return 0;
            }

            public static int Compare(T x, T y, ListSortDescription sort)
            {
                var xval = (x == null) ? null : (sort.PropertyDescriptor?.GetValue(x) as IComparable);
                var yval = (y == null) ? null : (sort.PropertyDescriptor?.GetValue(y) as IComparable);

                if (xval == null)
                    return (yval == null) ? 0 : -1;
                else if (yval == null)
                    return 1;

                if (sort.SortDirection == ListSortDirection.Ascending)
                    return xval.CompareTo(yval);
                else
                    return yval.CompareTo(xval);
            }
        }
        #endregion

        #region Public versions of IBindingList Explicit Methods

        /// <summary>
        /// Returns the index of the first item where the value of the propertyName parameter equals the value of the key parameter.
        /// </summary>
        /// <param name="propertyName">The PropertyName to search on.</param>
        /// <param name="key">The value of the property parameter to search for</param>
        /// <returns>The index of the matching item, or -1 if not found</returns>
        /// <exception cref="ArgumentException">PropertyName not found</exception>
        public int Find(string propertyName, object key)
        {
            var pd = TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, false);

            if (pd == null)
                throw new ArgumentException($"Property '{propertyName}' cannot be found");

            return (this as IBindingList).Find(pd, key);
        }

        /// <summary>
        /// Sorts the list based on a PropertyName and a ListSortDirection.
        /// </summary>
        /// <param name="propertyName">The PropertyName to sort by</param>
        /// <param name="direction">One of the ListSortDirection values.</param>
        /// <exception cref="ArgumentException">PropertyName not found</exception>
        public void ApplySort(string propertyName, ListSortDirection direction)
        {
            var pd = TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, false);

            if (pd == null)
                throw new ArgumentException($"Property '{propertyName}' cannot be found");

            (this as IBindingList).ApplySort(pd, direction);
        }

        /// <summary>
        /// Removes any sort applied using ApplySort()
        /// </summary>
        public void RemoveAppliedSort()
        {
            //call explicit implementation, note that this method
            //must not be named the same as the base interface
            //method otherwise it calls itself recursively.
            (this as IBindingList).RemoveSort();
        }
        #endregion

        #region New Public Methods
        /// <summary>
        /// Adds the elements of the specified collection to the BindingList.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added the BindingList</param>
        /// <exception cref="ArgumentNullException">collection is null.</exception>
        public void AddRange(IEnumerable<T> collection, bool RaiseAddItem = false)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var savedRaiseListChangedEvents = this.RaiseListChangedEvents;

            if (RaiseAddItem == false)
                this.RaiseListChangedEvents = false;

            foreach (var item in collection)
                Add(item);

            if (!RaiseAddItem)
            {
                this.RaiseListChangedEvents = savedRaiseListChangedEvents;
                ResetBindings();
            }
        }
        #endregion

        #region New Public Events

        /// <summary>
        /// Raised immediately before an item is deleted.
        /// ListChangedEventArgs.NewIndex contains the index of the item to be deleted.
        /// </summary>
        public event EventHandler<ListChangedEventArgs> DeletingItem;

        protected override void RemoveItem(int index)
        {
            //Raise event before item removal
            if (this.RaiseListChangedEvents)
                DeletingItem?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));

            base.RemoveItem(index);
        }

        /// <summary>
        /// Raised immediately before the list is cleared.
        /// </summary>
        public event EventHandler<EventArgs> Clearing;

        protected override void ClearItems()
        {
            //Raise event before list is cleared
            if (this.RaiseListChangedEvents)
                Clearing?.Invoke(this, EventArgs.Empty);

            base.ClearItems();
        }
        #endregion

        #region New Public Properties
        /// <summary>
        /// Returns the type of items contained in the binding list
        /// </summary>
        public Type ListType => typeof(T);
        #endregion
    }
}
