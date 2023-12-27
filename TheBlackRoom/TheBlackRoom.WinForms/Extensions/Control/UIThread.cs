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
        /// Configurable option to throw exceptions if the action/delegate
        /// fails to run due to a no valid window handle
        /// </summary>
        public static bool ThrowExceptionsWhenNoHandle { get; set; } = true;

        /// <summary>
        /// Configurable option to force handle creation when possible
        /// </summary>
        public static bool ForceHandleCreation { get; set; } = false;

        /// <summary>
        /// Runs an action/delegate on the controls creation thread if
        /// required, as to avoid cross thread exceptions.
        /// </summary>
        /// <returns>true if action/delegate called, false if not</returns>
        public static bool UIThread(this Control ctrl, MethodInvoker action)
        {
            if (ctrl is null)
                throw new ArgumentNullException(nameof(ctrl));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            //If the control is disposed, any updates will throw an ObjectDisposedException,
            //so might as well just ignore the action().
            if (ctrl.IsDisposed)
                return false;

            //If InvokeRequired is true, then run the action with invoke. It is possible that
            //the controls handle has not been created, but InvokeRequired will search parent
            //controls if needed. See:
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invokerequired?view=netframework-4.8#remarks
            if (ctrl.InvokeRequired)
            {
                //A parent control/form has a handle (as InvokeRequired==true), but the current control does not.
                //This could be due to the control or its form being invisible.
                if (!ctrl.IsHandleCreated)
                {
                    //We need to invoke, but the current thread is the UI thread. This means the action will run on a thread other
                    //than the UI thread, meaning the parent control was likely created on the wrong thread.
                    if (Application.MessageLoop)
                    {
                        if (ThrowExceptionsWhenNoHandle)
                            throw new InvalidOperationException($"Not invoking action on control as its parent is not on the UI thread ({ctrl.Name})");
                        else
                            return false;
                    }

                    //No handle, there isn't much we can really do here. We could fail,
                    //but it is probably fine to run the action as it will run on the parent
                    //controls creation thread. We just hope that it ends up on the UI thread
                    //so the control is created correctly, and that the control plays nicely
                    //with getting modified when it has no handle.
                }

                ctrl.Invoke(action);
                return true;
            }

            //If InvokeRequired is false, we need to ensure the object can still perform the action().
            //It is possible that the control was created on a different thread from its parent,
            //but the control's handle has not yet been created.

            //Object has no handle yet. This could be due to the control or its form being
            //invisible, or external events causing functions to be run on the control before
            //the form has fully been created. We have a few options:
            // - Ignore the action(), return false
            // - Force the handle creation, although this might end up with the control being created
            //   on the wrong thread if not careful
            //   Note that when referencing the handle property, if the handle has not yet been created,
            //   referencing this property will force the handle to be created.
            // - Force the developer to ensure there is a handle on the UI thread, or a parent handle, etc
            //   via an Exception
            if (!ctrl.IsHandleCreated)
            {
                //If we are on the UI thread, then just force the handle creation
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.messageloop?view=netframework-4.8
                if (Application.MessageLoop)
                {
                    //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.handle?view=netframework-4.8#remarks

                    //We could be nice and create the handle for the control since we are on the UI thread,
                    //although the control will do it eventually, and we probably don't want to create handles
                    //when the control isn't visible anyway
                    if (ForceHandleCreation)
                    {
                        //Note that we need to assign the handle to a variable to ensure it does not get optimized out
                        var force_handle_creation = ctrl.Handle;
                    }
                }
                else
                {
                    //Not on UI thread, fail
                    if (ThrowExceptionsWhenNoHandle)
                        throw new InvalidOperationException($"Not performing action on control as it has no handle and is not on the UI thread ({ctrl.Name})");
                    else
                        return false;
                }
            }

            //No need to invoke, just run the action
            action();
            return true;
        }
    }
}
