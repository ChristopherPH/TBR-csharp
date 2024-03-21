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

namespace TheBlackRoom.WinForms.Helpers.ListBoxHelpers
{
    public enum ListBoxMouseWheelScrollAmounts
    {
        [Description("Windows Default")]
        Default,

        Disabled,

        [Description("Single Page")]
        SinglePage,

        [Description("Half Page")]
        HalfPage,

        [Description("Third Page")]
        ThirdPage,

        [Description("Quarter Page")]
        QuarterPage,

        [Description("Single Line")]
        SingleLine,
    }

    /// <summary>
    /// Helper class for handling mousewheel scrolling in listboxes
    /// </summary>
    public static class ListBoxMouseWheelScrollHelper
    {
        /// <summary>
        /// Disables listbox smooth scrolling via mousewheel
        /// </summary>
        /// <param name="listBox"></param>
        public static void DisableSmoothScroll(this ListBox listBox)
        {
            listBox.MouseWheel += listBox_MouseWheel;
        }

        private static void listBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ListBoxMouseWheelScroller(sender, e, ListBoxMouseWheelScrollAmounts.Default);
        }

        /// <summary>
        /// Disables listbox smooth scrolling via mousewheel, and replaces scroll amount
        /// with a configurable amount of scroll.
        /// Note: This function is designed to be called from the ListBox MouseWheel event.
        /// </summary>
        /// <param name="sender">ListBox</param>
        /// <param name="e">MouseEventArgs from MouseWheel event</param>
        /// <param name="scrollAmount">Amount to scroll</param>
        public static void ListBoxMouseWheelScroller(object sender, MouseEventArgs e,
            ListBoxMouseWheelScrollAmounts scrollAmount = ListBoxMouseWheelScrollAmounts.Default)
        {
            //NOTE: This only works when the listbox scrollbar does not have focus.
            //      If the listbox scrollbar is clicked and has focus, then the first
            //      mousescroll will still be smooth. After the first scroll, it looks
            //      like the something changes so subsequent scrolls are no longer smooth.
            if ((sender is ListBox listBox) && (e is HandledMouseEventArgs handledArgs))
            {
                //Manually handling the mousewheel scroll disables smooth scrolling
                handledArgs.Handled = true;

                if (e.Delta == 0)
                    return;

                int scrollLines;
                int visibleLines = listBox.Height / listBox.ItemHeight;

                switch (scrollAmount)
                {
                    default:
                    case ListBoxMouseWheelScrollAmounts.Default:
                        scrollLines = SystemInformation.MouseWheelScrollLines;
                        if (scrollLines == -1) // -1 is the "One screen at a time" mouse option
                            scrollLines = visibleLines;
                        break;

                    case ListBoxMouseWheelScrollAmounts.Disabled:
                        return;

                    case ListBoxMouseWheelScrollAmounts.SinglePage:
                        scrollLines = visibleLines;
                        break;

                    case ListBoxMouseWheelScrollAmounts.HalfPage:
                        scrollLines = visibleLines / 2;
                        break;

                    case ListBoxMouseWheelScrollAmounts.ThirdPage:
                        scrollLines = visibleLines / 3;
                        break;

                    case ListBoxMouseWheelScrollAmounts.QuarterPage:
                        scrollLines = visibleLines / 4;
                        break;

                    case ListBoxMouseWheelScrollAmounts.SingleLine:
                        scrollLines = 1;
                        break;
                }

                //ensure at least one line will be scrolled
                if (scrollLines <= 0)
                    scrollLines = 1;

                //scroll listbox using topindex property
                if (e.Delta > 0)
                {
                    if (listBox.TopIndex > 0)
                        listBox.TopIndex -= Math.Min(scrollLines, listBox.TopIndex);
                }
                else if (e.Delta < 0)
                {
                    if (listBox.TopIndex < listBox.Items.Count - 1)
                        listBox.TopIndex += Math.Min(scrollLines, listBox.Items.Count - 1 - listBox.TopIndex);
                }
            }
        }
    }
}
