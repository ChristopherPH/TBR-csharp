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
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Extensions
{
    public static class RichTextBox_RemoveLine
    {
        public static void RemoveLine(this RichTextBox richTextBox,
            int lineNumber, int lineCount = 1)
        {
            //validate parameters
            if ((richTextBox == null) || (lineNumber < 0) || (lineCount <= 0) ||
                (lineNumber >= richTextBox.Lines.Length))
            {
                return;
            }

            //remove all lines
            if ((lineNumber == 0) && (lineCount >= richTextBox.Lines.Length))
            {
                richTextBox.Clear();
                return;
            }

            //get character position of start line
            var removeStart = richTextBox.GetFirstCharIndexFromLine(lineNumber);
            int removeEnd, removeLength;

            //if the end will exceed the total length, then set the
            //count to the rest of the text
            if (lineNumber + lineCount > richTextBox.Lines.Length)
            {
                removeEnd = richTextBox.Text.Length;
            }
            else
            {
                //set the count to be beginning of the line after the count
                removeEnd = richTextBox.GetFirstCharIndexFromLine(lineNumber + lineCount);
            }

            removeLength = removeEnd - removeStart;
            if (removeLength == 0)
                return;

            //Save selection and scroll positions
            var selectionStart = richTextBox.SelectionStart;
            var selectionLength = richTextBox.SelectionLength;
            var scrollPosition = richTextBox.GetScrollPosition();
            var readOnly = richTextBox.ReadOnly;

            //Disable drawing, so that changing the selections doesn't adjust
            //the current position
            richTextBox.DisableRedraw();

            //Select characters to remove, and set them to empty,
            //which preserves formatting. This needs to not be read-only
            //to work.
            richTextBox.ReadOnly = false;
            richTextBox.Select(removeStart, removeLength);
            richTextBox.SelectedText = string.Empty;

            var selectionEnd = selectionStart + selectionLength;

            //Adjust the selection if it is inside or past the removed
            //characters. Check if the selected characters need to be
            //adjusted (if they are inside any of the removal),
            //otherwise the selection can be left alone.
            if (selectionEnd > removeStart)
            {
                if (selectionStart >= removeEnd) //selection is past the removed characters
                {
                    //shift selection by removed characters to leave selection
                    selectionStart -= removeLength;
                }
                else if ((selectionStart >= removeStart) && //selection entirely within removed characters
                    (selectionEnd <= removeEnd))
                {
                    //set the selection to the start of the removed characters
                    selectionStart = removeStart;
                    selectionLength = 0;
                }
                else if ((selectionStart < removeStart) && //selection entirely outside removed characters
                    (selectionEnd > removeEnd))
                {
                    //reduce selection by removed characters to leave selection
                    //before and after removed characters
                    selectionLength -= removeLength;
                }
                else if (selectionStart < removeStart) //selection before to inside removed characters
                {
                    //reduce selection to before removed characters
                    selectionLength = removeStart - selectionStart;
                }
                else if (selectionEnd > removeEnd) //selection inside to past removed characters
                {
                    //shift and reduce selection to whats left beyond removed characters
                    selectionLength = selectionEnd - removeEnd;
                    selectionStart = removeStart;
                }
            }

            //Restore parameters
            richTextBox.ReadOnly = readOnly;

            //Restore selection
            richTextBox.Select(selectionStart, selectionLength);

            //restore scroll position
            //TODO: If the lines removed are out of view, beyond the current scroll position,
            //      or there is not enough lines to scroll, then this will work fine.
            //      Otherwise, this will restore an incorrect position and it needs to be fixed.
            richTextBox.SetScrollPosition(scrollPosition);

            //Restore drawing with the current position
            richTextBox.EnableRedraw(true);
        }
    }
}
