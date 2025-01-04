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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheBlackRoom.WinForms.Extensions;

namespace TheBlackRoom.WinForms.Controls
{
    /* Notes on RichTextBox font changing:
     *
     * When the RichTextBox font is set (manually, or from a parent font
     * change), all of the text inside the RichTextBox is set to the new
     * font. This causes any colours or formatting to be lost, which may
     * not be expected.
     *
     * This formatting loss happens within (base).OnFontChanged().
     *
     * When the .Font property is changed, the RichTextBox will update the font
     * of the underlying Win32 control first, and then update the .Net wrapper
     * control after. This means that OnFontChanged() is called after the
     * internal RTF text of the Win32 control has been updated.
     *
     * When the parent font is changed (and the RichTextBox has no font set)
     * the .Font property setter is not called, and so OnFontChanged() is
     * called before the underlying Win32 font has been updated. This means
     * that the internal RTF text of the Win32 control has not been updated
     * inside OnFontChanged().
     */

    /// <summary>
    /// RichTextBox with workarounds for:
    ///     setting ZoomFactor
    ///     losing ZoomFactor when text is cleared
    ///     losing RTF colours and formatting on font change (or parent font change)
    /// </summary>
    public class ExtendedRichTextBox : RichTextBox
    {
        /// <summary>
        /// Saved RTF string from RichTextBox control
        /// </summary>
        string _savedRtf = null;

        /// <summary>
        /// Raised when the RTF Text formatting has been restored, after a Font change
        /// has cleared the formatting
        /// </summary>
        public event EventHandler<EventArgs> TextFormattingRestored;

        /// <summary>
        /// Flag to maintain existing RTF text formatting and colours when the font changes
        /// </summary>
        [DisplayName("Maintain Text Formatting")]
        [Category("Behavior")]
        [Description("Maintain existing RTF text formatting and colours when the font changes")]
        [DefaultValue(true)]
        public bool MaintainTextFormatting { get; set; } = true;

        public override Font Font
        {
            get => base.Font;
            set
            {
                /* When the font is manually set, save the RTF string prior to
                 * updating the font of the underlying Win32 control. This
                 * will ensure the RTF string has not yet been updated with the
                 * new font. */
                if (MaintainTextFormatting)
                    _savedRtf = this.Rtf;

                base.Font = value;
            }
        }


        protected override void OnFontChanged(EventArgs e)
        {
            /* If not maintaining formatting, or there is no text to maintain,
             * then allow the font change to update the rtf font table as
             * normal */
            if (!MaintainTextFormatting || (TextLength == 0))
            {
                //Clear any saved RTF data
                _savedRtf = null;

                //This will set any existing text to the new font and style
                base.OnFontChanged(e);

                return;
            }

            /* Save the RTF string of the RichTextBox if it wasn't previously
             * saved.
             *
             * A manual font change will have previously saved the RTF string
             * with the old font(s). Note that a manual font change will update
             * the RTF string with the new font and font styles, but will
             * interestingly preserve the text colours.
             *
             * If the RTF string wasn't previously saved, OnFontChanged() is
             * likely being called due to a parent font change. This means
             * the RichTextBox RTF string has not yet been updated with the
             * new font so it can be saved before calling base.OnFontChanged().
             *
             * Saving the RTF string allows the formatting to be restored
             * after calling base.OnFontChanged(), which removes existing
             * colours and formatting. */
            var savedRTF = _savedRtf ?? this.Rtf;
            _savedRtf = null;

            var savedSelectionStart = this.SelectionStart;
            var savedSelectionLength = this.SelectionLength;
            var savedScrollPosition = this.GetScrollPosition();


            /* HACK: Save the zoom factor as setting the .Rtf property resets
             *       the ZoomFactor back to 1 */
            var savedZoomFactor = this.ZoomFactor;

            this.DisableRedraw();

            /* Fully clear the RTF before the font change, since it will be
             * restored below, there is no reason to have the RichTextBox
             * reparse the RTF data. This also ensures that setting the .Rtf
             * property with the savedRTF will trigger a full reparse of the
             * RTF string (if the RTF string does not change, then the
             * RichTextBox control ignores the property change. */
            base.Rtf = null;

            // Note: This line here causes the RTF formatting loss
            base.OnFontChanged(e);

            /* Restore the saved RTF. Note that this resets the RTF font sizes
             * to those saved inside the RTF string rather that what is cached
             * inside the RichTextBox. This means that if the RichTextBox was
             * displaying fonts with sizes that were rounded up to the next
             * RTF fontsize, the displayed font size will slightly increase. */
            base.Rtf = savedRTF;

            /* Changing the .Rtf property resets the zoom factor back to 1,
             * but does not update the backing variable, so set it twice to
             * ensure the zoom factor is correct. */
            base.ZoomFactor = 1.0f;
            base.ZoomFactor = savedZoomFactor;

            //Restore selection
            this.Select(savedSelectionStart, savedSelectionLength);

            //Restore scroll position
            this.SetScrollPosition(savedScrollPosition);

            //Restore drawing with the current position
            this.EnableRedraw(true);

            /* Raise an event once the RTF and ZoomFactor have been restored
             * after the font change has finished. */
            TextFormattingRestored?.Invoke(this, EventArgs.Empty);
        }

        public new float ZoomFactor
        {
            get => base.ZoomFactor;
            set
            {
                /* HACK: In order to properly set the zoom factor, set it twice.
                 *       Certain methods will reset the underlying zoom factor
                 *       back to 1, withouth updating the backing variable.
                 *       This results in not being able to restore the zoom
                 *       factor in these cases, as the control thinks it is
                 *       already at the old zoom factor. Known causes of reset:
                 *       - Clear()
                 *       - ResetText()
                 *       - Setting .Text property to string.Empty/null
                 *       - Setting .RTF property
                 */
                base.ZoomFactor = 1.0f;
                base.ZoomFactor = value;
            }
        }

        public new string Rtf
        {
            get => base.Rtf;
            set
            {
                /* HACK: Save and restore the zoom factor, as setting the .Rtf
                 *       internally resets the zoom factor back to 1, and does
                 *       not update the backing variable allowing future changes.
                 * NOTE: Setting the .SelectedRtf property does not reset the
                 *       zoom factor, so it is possible to instead do:
                 *           this.Select(0, TextLength);
                 *           this.SelectedRtf = value;
                 */

                //Save current zoom factor
                var savedZoomFactor = this.ZoomFactor;

                //Set .Rtf, which resets the zoom factor internally to 1
                base.Rtf = value;

                //Set backing variable to 1 to match internal zoom factor
                base.ZoomFactor = 1.0f;

                //Restore saved zoom factor
                base.ZoomFactor = savedZoomFactor;
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                /* HACK: Save and restore the zoom factor, as clearing the text
                 *       internally resets the zoom factor back to 1, and does
                 *       not update the backing variable for future changes.
                 *       Note that both Clear() and ResetText() just set the
                 *       .Text property so this code catches those cases as
                 *       well. */
                if (string.IsNullOrEmpty(value))
                {
                    //Save current zoom factor
                    var savedZoomFactor = ZoomFactor;

                    //Clear text, which resets the zoom factor internally to 1
                    base.Text = value;

                    //Set backing variable to 1 to match internal zoom factor
                    base.ZoomFactor = 1.0f;

                    //Restore saved zoom factor
                    base.ZoomFactor = savedZoomFactor;
                }
                else
                {
                    base.Text = value;
                }
            }
        }
    }
}
