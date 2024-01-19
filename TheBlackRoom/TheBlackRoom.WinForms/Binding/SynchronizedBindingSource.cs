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

namespace TheBlackRoom.WinForms.Binding
{
    /// <summary>
    /// BindingSource that ensures updates on the datasource are invoked if required
    /// </summary>
    public class SynchronizedBindingSource : BindingSource
    {
        private ISynchronizeInvoke _invoker;

        /// <summary>
        /// Initializes a new instance of the BindingSource class to the default property values.
        /// </summary>
        /// <param name="invoker">Object (control or form) to invoke datasource updates on</param>
        public SynchronizedBindingSource(ISynchronizeInvoke invoker) : base()
        {
            _invoker = invoker;
        }

        /// <summary>
        /// Initializes a new instance of the BindingSource class and adds the BindingSource to the specified container.
        /// </summary>
        /// <param name="dataSource">The data source for the BindingSource.</param>
        /// <param name="dataMember">The specific column or list name within the data source to bind to.</param>
        /// <param name="invoker">Object (control or form) to invoke datasource updates on</param>
        public SynchronizedBindingSource(object dataSource, string dataMember, ISynchronizeInvoke invoker)
            : base(dataSource, dataMember)
        {
            _invoker = invoker;
        }

        /// <summary>
        /// Initializes a new instance of the BindingSource class with the specified data source and data member.
        /// </summary>
        /// <param name="container">The IContainer to add the current BindingSource to.</param>
        /// <param name="invoker">Object (control or form) to invoke datasource updates on</param>
        public SynchronizedBindingSource(IContainer container, ISynchronizeInvoke invoker) : base(container)
        {
            _invoker = invoker;
        }

        /// <summary>
        /// Raises the ListChanged event and invokes if required
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnListChanged(e);
            else
                _invoker.Invoke((Action)(() => base.OnListChanged(e)), null);
        }

        /* From the best I can tell tracing through the winforms binding code
         * in the .NET reference source, the currency manager only looks at
         * OnListChanged events to trigger bound control updates, so this is
         * the only method that I overrode.
         *
         * However, the controls such as the listbox bind directly against the
         * CurrencyManager so things like manually changing the currency manager
         * positon (which BindingList.Position map to) from a thread will still
         * fail to update the control. This includes MoveNext()/MovePrevious()
         * etc.
         *
         * I think its too much to create a SynchronizedCurrencyManager here,
         * as this class is really only for binding to a datasource
         * that is updated via a thread (and mainly for display only).
         *
         * I suppose one might want an external source or thread to
         * update the selection, so we can create some Invoke* Methods
         * helpers.
         *
         * And, might as well add some other On*Changed events that are
         * proxies from the CurrencyManager in case they are useful.
         */

        protected override void OnPositionChanged(EventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnPositionChanged(e);
            else
                _invoker.Invoke((Action)(() => base.OnPositionChanged(e)), null);
        }

        protected override void OnCurrentChanged(EventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnCurrentChanged(e);
            else
                _invoker.Invoke((Action)(() => base.OnCurrentChanged(e)), null);
        }

        protected override void OnCurrentItemChanged(EventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnCurrentItemChanged(e);
            else
                _invoker.Invoke((Action)(() => base.OnCurrentItemChanged(e)), null);
        }

        protected override void OnBindingComplete(BindingCompleteEventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnBindingComplete(e);
            else
                _invoker.Invoke((Action)(() => base.OnBindingComplete(e)), null);
        }

        protected override void OnDataError(BindingManagerDataErrorEventArgs e)
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                base.OnDataError(e);
            else
                _invoker.Invoke((Action)(() => base.OnDataError(e)), null);
        }

        /// <summary>
        /// Gets or sets the bindingsource position via Invoke()
        /// </summary>
        public int InvokePosition
        {
            get => Position;
            set
            {
                if ((_invoker == null) || !_invoker.InvokeRequired)
                    Position = value;
                else
                    _invoker.Invoke((Action)(() => Position = value), null);
            }
        }

        /// <summary>
        /// Calls BindingSource.MoveNext via Invoke()
        /// </summary>
        public void InvokeMoveNext()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                MoveNext();
            else
                _invoker.Invoke((Action)(() => MoveNext()), null);
        }

        /// <summary>
        /// Calls BindingSource.MovePrevious via Invoke()
        /// </summary>
        public void InvokeMovePrevious()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                MovePrevious();
            else
                _invoker.Invoke((Action)(() => MovePrevious()), null);
        }

        /// <summary>
        /// Calls BindingSource.MoveFirst via Invoke()
        /// </summary>
        public void InvokeMoveFirst()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                MoveFirst();
            else
                _invoker.Invoke((Action)(() => MoveFirst()), null);
        }

        /// <summary>
        /// Calls BindingSource.MoveLast via Invoke()
        /// </summary>
        public void InvokeMoveLast()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                MoveLast();
            else
                _invoker.Invoke((Action)(() => MoveLast()), null);
        }

        /// <summary>
        /// Calls BindingSource.AddNew via Invoke()
        /// </summary>
        public void InvokeAddNew()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                AddNew();
            else
                _invoker.Invoke((Action)(() => AddNew()), null);
        }

        /// <summary>
        /// Calls BindingSource.EndEdit via Invoke()
        /// </summary>
        public void InvokeEndEdit()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                EndEdit();
            else
                _invoker.Invoke((Action)(() => EndEdit()), null);
        }

        /// <summary>
        /// Calls BindingSource.CancelEdit via Invoke()
        /// </summary>
        public void InvokeCancelEdit()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                CancelEdit();
            else
                _invoker.Invoke((Action)(() => CancelEdit()), null);
        }

        /// <summary>
        /// Calls BindingSource.SuspendBinding via Invoke()
        /// </summary>
        public void InvokeSuspendBinding()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                SuspendBinding();
            else
                _invoker.Invoke((Action)(() => SuspendBinding()), null);
        }

        /// <summary>
        /// Calls BindingSource.ResumeBinding via Invoke()
        /// </summary>
        public void InvokeResumeBinding()
        {
            if ((_invoker == null) || !_invoker.InvokeRequired)
                ResumeBinding();
            else
                _invoker.Invoke((Action)(() => ResumeBinding()), null);
        }
    }
}
