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

namespace TheBlackRoom.System.Extensions
{
    public static class BindingList_AddRange
    {
        /// <summary>
        /// Adds the elements of the specified collection to the BindingList.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingList">The BindingList to add elements to</param>
        /// <param name="collection">The collection whose elements should be added the BindingList</param>
        /// <param name="inhibitItemAddedEvent">Flag indicating if ListChanged.ItemAdded events should be
        /// inhibited as elements are Added to the BindingList</param>
        /// <param name="raiseResetEvent">Flag indicating if a ListChanged.Reset event should be raised
        /// when all ements are Added to the BindingList</param>
        /// <exception cref="ArgumentNullException">bindingList is null or collection is null</exception>
        public static void AddRange<T>(this BindingList<T> bindingList, IEnumerable<T> collection,
            bool inhibitItemAddedEvent = false, bool raiseResetEvent = false)
        {
            if (bindingList == null)
                throw new ArgumentNullException(nameof(bindingList));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            //Save current state of RaiseListChangedEvents
            var savedRaiseListChangedEvents = bindingList.RaiseListChangedEvents;

            //Turn off RaiseListChangedEvents for add if flag is set
            if (inhibitItemAddedEvent)
                bindingList.RaiseListChangedEvents = false;

            //Add elements to bindinglist
            foreach (var element in collection)
                bindingList.Add(element);

            //Turn off RaiseListChangedEvents for reset if flag is set, otherwise restore state
            if (raiseResetEvent)
                bindingList.RaiseListChangedEvents = savedRaiseListChangedEvents;
            else
                bindingList.RaiseListChangedEvents = false;

            //Perform a list reset
            bindingList.ResetBindings();

            //Restore state of RaiseListChangedEvents
            bindingList.RaiseListChangedEvents = savedRaiseListChangedEvents;
        }
    }
}
