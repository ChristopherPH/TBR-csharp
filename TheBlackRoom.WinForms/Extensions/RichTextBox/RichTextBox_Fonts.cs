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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class RichTextBox_Fonts
    {
        /// <summary>
        /// Sets the Font Size of any existing RTF Text inside a Rich Text Box
        /// </summary>
        /// <param name="richTextBox">>Rich Text Box to set font size of</param>
        /// <param name="sizeInPoints">Font size to set, in points</param>
        public static void SetRTFFontSize(this RichTextBox richTextBox, float sizeInPoints)
        {
            if (richTextBox is null)
                throw new ArgumentNullException(nameof(richTextBox));

            if (!richTextBox.IsHandleCreated)
                throw new ArgumentException("Invalid Handle", nameof(richTextBox));

            if (sizeInPoints <= 0)
                throw new ArgumentException("Invalid Font Size", nameof(sizeInPoints));

            //No existing text, nothing to do
            if (richTextBox.TextLength == 0)
                return;

            //Save the existing ZoomFactor value, as setting the .Rtf property
            //resets the ZoomFactor back to 1.0f.
            var tmpZoomFactor = richTextBox.ZoomFactor;

            //Replace any instances of "\fsSIZE-IN-HALF-POINTS" that is not
            //preceeded by another "\" with the new size. Convert the new
            //size to half points as required by the RTF spec, rounding up
            //and formatting to no decimal places.
            richTextBox.Rtf = Regex.Replace(richTextBox.Rtf,
                    @"(?<!\\)\\fs\d+",
                    m => $@"\fs{(sizeInPoints * 2):0}");

            //Restore the pre-existing ZoomFactor value after setting the .Rtf
            //property. Sometimes the underlying ZoomFactor changes but the
            //property doesn't, so set the ZoomFactor twice to fix it.
            richTextBox.ZoomFactor = 1.0f;
            richTextBox.ZoomFactor = tmpZoomFactor;
        }
    }
}
