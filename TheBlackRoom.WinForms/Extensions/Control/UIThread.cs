/*
 * Copyright (c) 2022 Christopher Hayes
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
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class Control_UIThread
    {
        /// <summary>
        /// Runs a delegate on the UI thread if needed, to avoid cross thread exceptions
        /// </summary>
        /// <returns>true if delegate called, false if not</returns>
        public static bool UIThread(this Control ctrl, MethodInvoker action)
        {
            if (ctrl is null)
                throw new ArgumentNullException(nameof(ctrl));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            //If InvokeRequired is true, then run the action with invoke. It is possible that
            //the controls handle has not been created, but InvokeRequired will search parent
            //controls if needed. See:
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invokerequired?view=netframework-4.8#remarks
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(action);
                return true;
            }

            //If InvokeRequired is false, we need to ensure the object can still perform the action().
            //If is possible that the control was created on a different thread but the control's handle has not yet been created.

            //If the control is disposed, any updates will throw an ObjectDisposedException,
            //so might as well just ignore the action().
            if (ctrl.IsDisposed)
                return false;

            //Object has no handle yet. We have a few options:
            // - Ignore the action(), return false
            // - Force the handle creation, although this might end up with the control being created
            //   on the wrong thread. { var force_handle_creation = ctrl.Handle; }
            //   Note that when referencing the handle property, if the handle has not yet been created,
            //   referencing this property will force the handle to be created.
            // - Force the developer to ensure there is a handle on the UI thread, or a parent handle, etc
            if (!ctrl.IsHandleCreated)
            {
                //If we are on the UI thread, then just force the handle creation
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.messageloop?view=netframework-4.8
                if (Application.MessageLoop)
                {
                    //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.handle?view=netframework-4.8#remarks
                    //This might throw an InvalidOperationException for a crossthread exception
                    var force_handle_creation = ctrl.Handle;
                }
                else
                {
                    //Force the calling function to ensure there is a valid handle
                    throw new InvalidOperationException("Control has no handle");
                }
            }

            action();
            return true;
        }
    }
}
