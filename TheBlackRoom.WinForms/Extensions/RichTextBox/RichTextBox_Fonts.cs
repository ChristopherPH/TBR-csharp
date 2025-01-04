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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class RichTextBox_Fonts
    {
        /// <summary>
        /// Sets the font size of any RTF text inside a RichTextBox, without
        /// modifying other font parameters.
        /// </summary>
        /// <param name="richTextBox">>Rich Text Box to set font parameters of</param>
        /// <param name="sizeInPoints">Font size to set, in points</param>
        /// <param name="selectionOnly">Flag indicating only selected text should be set</param>
        public static void SetRtfFontSize(this RichTextBox richTextBox, float sizeInPoints,
            bool selectionOnly)
        {
            //Validate parameters
            if (richTextBox is null)
                throw new ArgumentNullException(nameof(richTextBox));

            if (!richTextBox.IsHandleCreated)
                throw new ArgumentException("Invalid Handle", nameof(richTextBox));

            if (sizeInPoints <= 0)
                throw new ArgumentException("Invalid Font Size", nameof(sizeInPoints));

            //Setup font parameters
            var charformat2 = new NativeMethods.CHARFORMAT2W()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.CHARFORMAT2W)),
                dwMask = NativeMethods.CFM_MASK.CFM_SIZE,
                yHeight = (int)(sizeInPoints * 20)
            };

            //Send message to richtextbox
            NativeMethods.SendMessage(richTextBox.Handle, NativeMethods.EM_SETCHARFORMAT,
                selectionOnly ? NativeMethods.SCF_SELECTION : NativeMethods.SCF_ALL,
                ref charformat2);
        }
    }
}
