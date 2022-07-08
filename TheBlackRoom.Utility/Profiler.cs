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
using System.Diagnostics;
using System.Runtime.CompilerServices;

/* Usages:
 *
 * myfunction()
 * {
 *    using (new Profiler())
 *    {
 *       //code to profile
 *    }
 * }
 *
 * myfunction(argument arg)
 * {
 *    using (new Profiler("myfunction(argument={0}), arg))
 *    {
 *       //code to profile
 *    }
 * }
 *
 * using (new Profiler("ProfileCallingFunctionName))
 *    FunctionName()
 */

namespace TheBlackRoom.Utility
{
    /// <summary>
    /// Wrapper for System.Diagnostics.Stopwatch that uses the using() pattern to output
    /// how long code takes to execute
    /// </summary>
    public class Profiler : IDisposable
    {
        Stopwatch _stopwatch = new Stopwatch();
        string _format;
        object[] _args;

        public Profiler([CallerMemberName] string profiledFunction = "") :
            this(string.IsNullOrWhiteSpace(profiledFunction) ? null : profiledFunction + "()", null)
        { }

        public Profiler(string format, params object[] args)
        {
            //If the format is blank, set it to this class name
            if (string.IsNullOrWhiteSpace(format))
            {
                this._format = this.GetType().Name;
                this._args = null;
            }
            else
            {
                this._format = format;
                this._args = args;
            }

            Start();
        }

        protected void Start()
        {
            OutputInfo("Profile: ", string.Empty);
            _stopwatch.Start();
        }

        protected void Finish()
        {
            _stopwatch.Stop();
            OutputInfo("Elapsed: ", string.Format(" = {0:n0}ms", _stopwatch.ElapsedMilliseconds));
        }

        public void Dispose()
        {
            Finish();
        }

        private void OutputInfo(string prefix, string postfix)
        {
            var fmt = _format;
            if (!string.IsNullOrEmpty(prefix))
                fmt = prefix + fmt;
            if (!string.IsNullOrEmpty(postfix))
                fmt += postfix;

            if (_args == null)
                Debug.Print(fmt);
            else
                Debug.Print(fmt, _args);
        }
    }
}
