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
        /// Sets the font size of any existing RTF text inside a RichTextBox,
        /// or sets the default font size if there is no existing RTF text.
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

            //Ensure size is valid when converted to twips and back
            //as RichTextBox internally uses twips for font height
            var points = Math.Round(sizeInPoints * 20) / 20;

            //RichTextBox requires halfpoints to have no decimal,
            //so cast to int to drop the decimal after soubling.
            //This allows sizeInPoints to be a 1/2 size (.5).
            var halfPoints = (int)(points * 2);

            //Save the existing ZoomFactor value, as setting the .Rtf property
            //resets the ZoomFactor back to 1.0f.
            var tmpZoomFactor = richTextBox.ZoomFactor;

            //Replace any instances of "\fsSIZE-IN-HALF-POINTS" that is not
            //preceeded by another "\" with the new size.
            richTextBox.Rtf = Regex.Replace(richTextBox.Rtf,
                    @"(?<!\\)\\fs\d+",
                    m => $@"\fs{halfPoints}");

            //Changing the RTF resets the zoom factor back to 1
            //but does not update the backing variable, so set
            //it twice to ensure the zoom factor is correct.
            richTextBox.ZoomFactor = 1.0f;
            richTextBox.ZoomFactor = tmpZoomFactor;
        }
    }
}
