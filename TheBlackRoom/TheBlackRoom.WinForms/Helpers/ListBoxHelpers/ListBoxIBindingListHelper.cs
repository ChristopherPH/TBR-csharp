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
using System.Windows.Forms;
using TheBlackRoom.WinForms.Extensions;

namespace TheBlackRoom.WinForms.Helpers.ListBoxHelpers
{
    /// <summary>
    /// Helper to handle updating a ListBox from IBindingList ListChanged events
    ///
    /// Setting the IBindingList as the ListBox DataSource (or using a BindingSource
    /// as a wrapper around the IBindingList) is not only not thread safe, but is
    /// implemented through a CurrencyManager which just ends up refreshing the
    /// entire internal list of the ListBox with any change.
    ///
    /// This helper directly manipulates the ListBox.Items as the IBindingList
    /// changes, trying to minimize the amount of list changes to reduce the
    /// amount of updates.
    ///
    /// As a bonus, the helper will call Invoke() when necessary, allowing the
    /// IBindingList to be updated on a thread.
    /// </summary>
    public class ListBoxIBindingListHelper : IDisposable
    {
        public ListBoxIBindingListHelper(ListBox listBox)
        {
            if (listBox == null)
                throw new ArgumentNullException(nameof(listBox));

            if (listBox.DataSource != null)
                throw new InvalidOperationException("DataSource cannot be set");

            _ListBox = listBox;
            _ListBox.DataSourceChanged += _ListBox_DataSourceChanged;
        }

        public ListBoxIBindingListHelper(ListBox listBox, IBindingList bindingList) : this(listBox)
        {
            SetBindingList(bindingList);
        }

        private ListBox _ListBox;
        private IBindingList _BindingList = null;
        private bool _bound = false;
        private bool disposedValue;

        public void SetBindingList(IBindingList bindingList)
        {
            ClearBindingList();

            if (bindingList == null)
                throw new ArgumentNullException(nameof(bindingList));

            _BindingList = bindingList;
            _BindingList.ListChanged += _BindingList_ListChanged;

            //Clear listbox and add items
            if (_BindingList.Count == 0)
            {
                _ListBox.Items.Clear();
            }
            else
            {
                var itemArray = new object[_BindingList.Count];
                _BindingList.CopyTo(itemArray, 0);

                _ListBox.BeginUpdate();
                _ListBox.Items.Clear();
                _ListBox.Items.AddRange(itemArray);
                _ListBox.EndUpdate();
            }
        }

        private void _ListBox_DataSourceChanged(object sender, EventArgs e)
        {
            if (_ListBox.DataSource != null)
                throw new InvalidOperationException("DataSource cannot be set");
        }

        public void ClearBindingList()
        {
            if (_bound)
            {
                _bound = false;
                _BindingList.ListChanged -= _BindingList_ListChanged;
                _ListBox.Items.Clear();
            }
        }

        private void _BindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    _ListBox.UIThread(() => _ListBox.Items.Insert(e.NewIndex, _BindingList[e.NewIndex]));
                    break;

                case ListChangedType.ItemDeleted:
                    _ListBox.UIThread(() => _ListBox.Items.RemoveAt(e.NewIndex));
                    break;

                case ListChangedType.ItemChanged:
                    if (string.IsNullOrEmpty(e.PropertyDescriptor?.Name)) //multiple properties changed or is a new item
                    {
                        _ListBox.UIThread(() => _ListBox.Items[e.NewIndex] = _BindingList[e.NewIndex]);
                    }
                    else //only a property changed
                    {
                        //TODO: is there a better way to force a change on a single item? Invalidate()? RefreshItem()?
                        _ListBox.UIThread(() => _ListBox.Items[e.NewIndex] = _BindingList[e.NewIndex]);
                    }
                    break;

                case ListChangedType.Reset:
                    
                    //Clear listbox and re-add items
                    if (_BindingList.Count == 0)
                    {
                        _ListBox.UIThread(() => _ListBox.Items.Clear());
                    }
                    else
                    {
                        var itemArray = new object[_BindingList.Count];
                        _BindingList.CopyTo(itemArray, 0);

                        _ListBox.UIThread(() =>
                        {
                            _ListBox.BeginUpdate();
                            _ListBox.Items.Clear();
                            _ListBox.Items.AddRange(itemArray);
                            _ListBox.EndUpdate();
                        });
                    }
                    break;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ClearBindingList();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
