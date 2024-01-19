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
using System.ComponentModel;

namespace TheBlackRoom.System.Binding
{
    /// <summary>
    /// Provides the features required to support both complex and simple
    /// scenarios when binding to a data source.
    /// </summary>
    public interface IBindingListEx : IBindingList
    {
        /// <summary>
        /// Raised immediately before an item is deleted.
        /// ListChangedEventArgs.NewIndex contains the index of the item to be deleted.
        /// </summary>
        event EventHandler<ListChangedEventArgs> DeletingItem;

        /// <summary>
        /// Raised immediately before the list is cleared.
        /// </summary>
        event EventHandler<EventArgs> Clearing;

        /// <summary>
        /// Returns the type of items contained in the binding list
        /// </summary>
        Type ListType { get; }
    }
}
