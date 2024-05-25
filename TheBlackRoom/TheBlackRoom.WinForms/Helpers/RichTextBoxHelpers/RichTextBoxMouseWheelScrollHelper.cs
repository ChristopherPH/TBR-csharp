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
using System.Drawing;
using System.Windows.Forms;
using TheBlackRoom.WinForms.Extensions;

namespace TheBlackRoom.WinForms.Helpers.RichTextBoxHelpers
{
    public enum RichTextBoxMouseWheelScrollAmounts
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
    /// Helper class for handling mousewheel scrolling in richtextboxes
    /// </summary>
    public static class RichTextBoxMouseWheelScrollHelper
    {
        /// <summary>
        /// Disables richtextbox smooth scrolling via mousewheel
        /// </summary>
        /// <param name="richTextBox"></param>
        public static void DisableSmoothScroll(this RichTextBox richTextBox)
        {
            richTextBox.MouseWheel += richTextBox_MouseWheel;
        }

        private static void richTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            RichTextBoxMouseWheelScroller(sender, e,
                RichTextBoxMouseWheelScrollAmounts.Default);
        }

        /// <summary>
        /// Disables richtextbox smooth scrolling via mousewheel, and replaces scroll amount
        /// with a configurable amount of scroll.
        /// Note: This function is designed to be called from the RichTextBox MouseWheel event.
        /// </summary>
        /// <param name="sender">RichTextBox</param>
        /// <param name="e">MouseEventArgs from MouseWheel event</param>
        /// <param name="scrollAmount">Amount to scroll</param>
        public static void RichTextBoxMouseWheelScroller(object sender, MouseEventArgs e,
            RichTextBoxMouseWheelScrollAmounts scrollAmount = RichTextBoxMouseWheelScrollAmounts.Default,
            bool allowMouseWheelZoom = true)
        {
            //HACK: remove RichTextBox smooth scrolling, replace with a configurable amount of scroll
            if ((sender is RichTextBox richTextBox) && (e is HandledMouseEventArgs handledArgs))
            {
                //handle ctrl-mouse zoom
                if (Control.ModifierKeys.HasFlag(Keys.Control))
                {
                    //if mouse zoom is disabled, mark this event as handled but don't do anything, 
                    //otherwise let the default behaviour happen
                    if (!allowMouseWheelZoom)
                        handledArgs.Handled = true;

                    return;
                }

                //we handle all the scroll wheel movement from here on
                handledArgs.Handled = true;

                if (e.Delta == 0)
                    return;

                var topIndex = richTextBox.GetCharIndexFromPosition(new Point(1, 1));
                var bottomIndex = richTextBox.GetCharIndexFromPosition(new Point(1, richTextBox.Height - 1));
                var topLine = richTextBox.GetLineFromCharIndex(topIndex);
                var bottomLine = richTextBox.GetLineFromCharIndex(bottomIndex);
                var visibleLines = bottomLine - topLine;

                int scrollLines;

                switch (scrollAmount)
                {
                    default:
                    case RichTextBoxMouseWheelScrollAmounts.Default:
                        scrollLines = SystemInformation.MouseWheelScrollLines;
                        if (scrollLines == -1) // -1 is the "One screen at a time" mouse option
                            scrollLines = visibleLines;
                        break;

                    case RichTextBoxMouseWheelScrollAmounts.Disabled:
                        return;

                    case RichTextBoxMouseWheelScrollAmounts.SinglePage:
                        scrollLines = visibleLines;
                        break;

                    case RichTextBoxMouseWheelScrollAmounts.HalfPage:
                        scrollLines = visibleLines / 2;
                        break;

                    case RichTextBoxMouseWheelScrollAmounts.ThirdPage:
                        scrollLines = visibleLines / 3;
                        break;

                    case RichTextBoxMouseWheelScrollAmounts.QuarterPage:
                        scrollLines = visibleLines / 4;
                        break;

                    case RichTextBoxMouseWheelScrollAmounts.SingleLine:
                        scrollLines = 1;
                        break;
                }

                //ensure at least one line will be scrolled
                if (scrollLines <= 0)
                    scrollLines = 1;

                //scroll richtextbox manually
                //HACK: we can't scroll by a set number of lines, so just send that many messages
                if (e.Delta > 0)
                {
                    for (int i = 0; i < scrollLines; i++)
                        richTextBox.ScrollLineUp();
                }
                else if (e.Delta < 0)
                {
                    for (int i = 0; i < scrollLines; i++)
                        richTextBox.ScrollLineDown();
                }
            }
        }
    }
}
