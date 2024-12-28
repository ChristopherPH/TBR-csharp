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
using TheBlackRoom.WinForms.Extensions;

namespace TheBlackRoom.WinForms.Controls
{
    /// <summary>
    /// RichTextBox with workarounds for:
    ///     setting ZoomFactor
    ///     losing ZoomFactor on clear
    ///     losing RTF colourtable and formatting on font change or parent font change
    /// </summary>
    public class ExtendedRichTextBox : RichTextBox
    {
        protected override void OnFontChanged(EventArgs e)
        {
            /* If there is no text, then use Clear() to remove the internal
             * RTF font table. This will force the next RTF text that is
             * appended to use the Font.SizeInPoints property. */
            if (this.TextLength == 0)
            {
                this.Clear();
                base.OnFontChanged(e);
                return;
            }

            //HACK: save the RTF as changing the font (or having the parent form's font change)
            //      causes the RTF to lose colour and formatting
            var savedRTF = this.Rtf;
            var savedSelectionStart = this.SelectionStart;
            var savedSelectionLength = this.SelectionLength;
            var scrollPosition = this.GetScrollPosition();

            //HACK: save the zoom factor as setting the RTF sets it back to 1
            var tmpZoomFactor = this.ZoomFactor;

            this.DisableRedraw();

            //this line here causes the RTF formatting loss
            base.OnFontChanged(e);

            //reset to the saved RTF
            this.Rtf = savedRTF;

            //Changing the RTF resets the zoom factor back to 1
            //but does not update the backing variable, so set
            //it twice to ensure the zoom factor is correct.
            base.ZoomFactor = 1.0f;
            base.ZoomFactor = tmpZoomFactor;

            //Restore selection
            this.Select(savedSelectionStart, savedSelectionLength);

            //Restore scroll position
            this.SetScrollPosition(scrollPosition);

            //Restore drawing with the current position
            this.EnableRedraw(true);
        }

        public new float ZoomFactor
        {
            get => base.ZoomFactor;
            set
            {
                //HACK: in order to properly set the zoom factor, set it twice.
                //      sometimes the underlying zoom factor changes but the property
                //      doesn't, for example, calling Clear().
                base.ZoomFactor = 1.0f;
                base.ZoomFactor = value;
            }
        }

        public new void Clear()
        {
            //HACK: save and restore the zoom factor as Clear()
            //      resets the zoom factor back to 1
            var tmpZoomFactor = ZoomFactor;

            base.Clear();

            //Clearing the control resets the zoom factor back to 1
            //but does not update the backing variable, so set
            //it twice to ensure the zoom factor is correct.
            base.ZoomFactor = 1.0f;
            base.ZoomFactor = tmpZoomFactor;
        }
    }
}
