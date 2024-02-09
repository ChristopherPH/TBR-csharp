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
using System.ComponentModel;

namespace TheBlackRoom.System.Binding
{
    /// <summary>
    /// Wrapper around a BindingList, that provides a filtered view
    /// of the items in the wrapped BindingList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingListFilter<T> : BindingList<T>
    {
        /* TODO:
         *  One can Add()/Clear() the filtered list and it should be fully read only
         *  The source list could not be of type <T>, this should be enforced
         *    (The constructor / source could check, there is the listhelper that gets the
         *     list type, or instead have the constructor work on
         *     BindingList<T> (and BindingListEx<T>) and cast those to IBindingList
         *     when saving this.Source.
         */
        public BindingListFilter()
        {
            AllowNew = false;
            AllowRemove = false;
            AllowEdit = false;
        }

        public BindingListFilter(IBindingList Source) : this()
        {
            this.Source = Source;
        }

        public BindingListFilter(IBindingList Source, Func<T, bool> Filter) : this(Source)
        {
            this.Filter = Filter;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBindingList Source
        {
            get => _Source;
            set
            {
                if (_Source == value)
                    return;

                if (_Source != null)
                {
                    _Source.ListChanged -= _Source_ListChanged;

                    if (_Source is IBindingListEx sourceEx)
                        sourceEx.DeletingItem -= _Source_DeletingItem;
                }

                _Source = value;

                if (_Source != null)
                {
                    _Source.ListChanged += _Source_ListChanged;

                    if (_Source is IBindingListEx sourceEx)
                        sourceEx.DeletingItem += _Source_DeletingItem;
                }

                Refilter();
            }
        }
        private IBindingList _Source;


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<T, bool> Filter
        {
            get => _Filter;
            set
            {
                if (_Filter == value)
                    return;

                _Filter = value;
                Refilter();
            }
        }
        private Func<T, bool> _Filter;

        /// <summary>
        /// Forces a refilter of the BindingList
        /// </summary>
        public void Refilter()
        {
            //no source, clear working list (which raises an event)
            if (_Source == null)
            {
                Clear();
                return;
            }

            //save value of RaiseListChangedEvents
            var savedRaiseListChangedEvents = this.RaiseListChangedEvents;

            //Inhibit list changed events
            RaiseListChangedEvents = false;

            this.Clear(); //no event

            //No filter, so just add all items and be done
            if (_Filter == null)
            {
                foreach (var item in _Source)
                {
                    if (item is T tItem)
                    {
                        _Source.Add(tItem); //no event raised
                    }
                }
            }
            else //add filtered items only
            {
                foreach (var item in _Source)
                {
                    if (item is T tItem)
                    {
                        try
                        {
                            if (_Filter(tItem))
                            {
                                this.Add(tItem); //no event raised
                            }
                        }
                        catch { /* EVERYTHING */ }
                    }
                }
            }

            RaiseListChangedEvents = savedRaiseListChangedEvents;
            ResetBindings(); //allow event
        }

        private void _Source_ListChanged(object sender, ListChangedEventArgs e)
        {
            int ix;

            if (_Source == null)
                return;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ix = AddSourceItemToFilteredList(e.NewIndex);
                    if (ix != -1)
                        this.Insert(ix, (T)_Source[e.NewIndex]);
                    break;

                case ListChangedType.ItemDeleted:
                    //If we don't know the item that has been removed,
                    //Then we need to reset the entire list to re-filter
                    //otherwise this is handled by _Source_DeletingItem
                    if (!(_Source is IBindingListEx))
                        Refilter();
                    break;

                case ListChangedType.ItemChanged:
                    //Entire item has changed, and since we can't get
                    //a reference to the old item to test if its in
                    //the filtered list, we need to just refilter the
                    //whole list. (isFiltered will always be false)
                    if (string.IsNullOrEmpty(e.PropertyDescriptor?.Name))
                    {
                        Refilter();
                        return;
                    }

                    //Item property has changed, need to determine
                    // if it needs to be filtered or not
                    var item = (T)_Source[e.NewIndex];
                    var shouldBeFiltered = _Filter(item);
                    var isFiltered = this.Contains(item);

                    if (shouldBeFiltered == isFiltered)
                        return; //filter state didn't change

                    //is filtered, should not be filtered, so remove
                    //the item
                    if (isFiltered)
                    {
                        AllowRemove = true;
                        this.Remove(item);
                        AllowRemove = false;
                    }
                    else //add to list
                    {
                        ix = AddSourceItemToFilteredList(e.NewIndex);
                        if (ix != -1)
                            this.Insert(ix, (T)_Source[e.NewIndex]);
                    }
                    break;

                case ListChangedType.Reset:
                    Refilter();
                    return;
            }
        }


        /// <summary>
        /// Adds an item from the source list to the filtered list, maintaining order
        /// </summary>
        /// <param name="SourceIndex"></param>
        /// <returns>true if the item was added to the filtered list</returns>
        private int AddSourceItemToFilteredList(int SourceIndex)
        {
            if ((SourceIndex < 0) || (SourceIndex >= _Source.Count))
                return -1;

            var srcItem = (T)_Source[SourceIndex];

            //no filter, just add put the item in the filtered
            //list where it exists in the source list, as the
            //indexes will match between the two
            if (_Filter == null)
                return SourceIndex;

            //Filter exists and the added item is filtered,
            //So no need to add it to the filtered list
            if (!_Filter(srcItem))
                return -1;

            //An item was added to end of the source list,
            //put the item at end of the filtered list
            if (SourceIndex == _Source.Count - 1)
                return this.Count;

            //An item was inserted at beginning of the source list,
            //insert the item at beginning of the filtered list
            if (SourceIndex == 0)
                return 0;


            //HACK: There is probably a much better way to do this
            //      so we can investigate at some point.
            //      With this implementation, we can work backwards,
            //      forwards, we can filter or call contains()...

            //An item was inserted somewhere in the middle of the
            //source list. However, depending on the filter, the item
            //might actually need to be inserted at the beginning or
            //placed at the end of the list (or inserted somewhere in
            //the middle).
            //
            //Work backwards from where the object was inserted,
            //looking for the next item that also exists in the
            //

            if (SourceIndex < (_Source.Count / 2)) //less items, work backwards from index
            {
                for (var ix = SourceIndex - 1; ix >= 0; ix--)
                {
                    //Check if the given source item exists in the filtered list
                    var filterIx = this.IndexOf((T)_Source[ix]);

                    //found item in filtered list, insert after it
                    if (filterIx != -1)
                        return filterIx + 1;
                }

                //item not added yet, insert it at beginning
                return 0;
            }
            else //work forwards from index
            {
                for (var ix = SourceIndex + 1; ix < _Source.Count; ix++)
                {
                    var filterIx = this.IndexOf((T)_Source[ix]);

                    //found item in filtered list, insert before it
                    if (filterIx != -1)
                        return filterIx;
                }

                //item not added yet, add it to end
                return this.Count;
            }
        }

        private void _Source_DeletingItem(object sender, ListChangedEventArgs e)
        {
            //source is an BindingListEx, so we can get the item that is
            //about to be removed, and remove it from the filter list
            AllowRemove = true;
            this.Remove((T)_Source[e.NewIndex]);
            AllowRemove = false;
        }
    }
}
