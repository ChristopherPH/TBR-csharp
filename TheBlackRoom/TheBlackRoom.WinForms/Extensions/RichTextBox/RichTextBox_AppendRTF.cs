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
using System.Drawing;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class RichTextBox_AppendRTF
    {
        /// <summary>
        /// Appends RTF text to the current RTF of a rich text box, and scrolls
        /// the rich text box to the end
        /// </summary>
        public static void AppendRtf(this RichTextBox richTextBox, string Rtf, bool scrollToEnd = true)
        {
            if (richTextBox == null)
                return;

            if (string.IsNullOrEmpty(Rtf))
                return;

            //Variables used to save selections and positions when not scrolling
            int selectionStart = 0, selectionLength = 0;
            Point scrollPosition = Point.Empty;

            //Save selection and scroll positions
            if (!scrollToEnd)
            {
                selectionStart = richTextBox.SelectionStart;
                selectionLength = richTextBox.SelectionLength;
                scrollPosition = richTextBox.GetScrollPosition();
            }

            //Disable drawing, so that changing the selections doesn't adjust
            //the current position
            richTextBox.DisableRedraw();

            //put selection at the end of the text as that is where
            //the RTF will be inserted
            richTextBox.Select(richTextBox.TextLength, 0);

            //Append rtf text (technically, insert rtf text at selection point)
            try
            {
                richTextBox.SelectedRtf += Rtf;
            }
            catch (ArgumentException)
            {
                //File format is not valid.' means invalid RTF, and might be just text
            }
            finally
            {
                if (scrollToEnd)
                {
                    //put selection at the end of the text, scrolling the rich text box
                    richTextBox.Select(richTextBox.TextLength, 0);
                    richTextBox.ScrollToBottom();
                }
                else
                {
                    //restore selection and scroll position
                    richTextBox.Select(selectionStart, selectionLength);
                    richTextBox.SetScrollPosition(scrollPosition);
                }
            }

            //Restore drawing with the current position
            richTextBox.EnableRedraw(true);
        }
    }
}
