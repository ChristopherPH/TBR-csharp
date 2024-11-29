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
using System.ComponentModel;

namespace TheBlackRoom.Core.Binding
{
    public class BindingListViewEx<T> : BindingListEx<T>, IBindingListView
    {
        #region Filtering
        public bool SupportsFiltering => false;

        public string Filter
        {
            get => "";
            set { }
        }

        public void RemoveFilter()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Advanced Sorting
        public bool SupportsAdvancedSorting => true;

        public ListSortDescriptionCollection SortDescriptions => _SortDescriptions;
        private ListSortDescriptionCollection _SortDescriptions = null;

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            _SortDescriptions = sorts;

            ApplySortInternal(new PropertyComparer(sorts));
        }

        protected override void RemoveSortCore()
        {
            _SortDescriptions = null;

            base.RemoveSortCore();
        }

        public void ApplySort(string propertyName, ListSortDirection direction,
            string propertyName2, ListSortDirection direction2)
        {
            ApplySort(new ListSortDescriptionCollection(new ListSortDescription[]
            {
                new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, false),
                    direction),
                new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertyName2, false),
                    direction2),
            }));
        }

        public void ApplySort(string propertyName, ListSortDirection direction,
            string propertyName2, ListSortDirection direction2,
            string propertyName3, ListSortDirection direction3)
        {
            ApplySort(new ListSortDescriptionCollection(new ListSortDescription[]
            {
                new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, false),
                    direction),
                new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertyName2, false),
                    direction2),
                new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertyName3, false),
                    direction3),
            }));
        }

        /* C# 7 tuples:
         * BindingList<T>.ApplySort(
         *      (nameof(<T>.Name), ListSortDirection.Ascending),
         *      (nameof(<T>.ID), ListSortDirection.Ascending));
         */
        public void ApplySort(params (string propertyName, ListSortDirection direction)[] propertySorts)
        {
            var sorts = new List<ListSortDescription>();

            foreach (var propertySort in propertySorts)
            {
                sorts.Add(new ListSortDescription(
                    TypeDescriptor.GetProperties(typeof(T)).Find(propertySort.propertyName, false),
                    propertySort.direction));
            }

            ApplySort(new ListSortDescriptionCollection(sorts.ToArray()));
        }

        #endregion
    }
}
