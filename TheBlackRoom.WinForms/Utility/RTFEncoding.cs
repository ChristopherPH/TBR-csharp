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
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TheBlackRoom.WinForms.Utility
{
    /* Notes when encoding into RTF:
     *
     * If the RTF fontsize is not specified, the RTF spec defaults the
     * size at 24 1/2 points.
     *
     * Notes specifically for the WinForms RichTextBox control:
     *
     * - The RTF colour and font tables will persist when setting the
     *   RTF via the `.Rtf` or `.SelectedRtf` properties, and will be
     *   cleared when setting the `.Text` property or by using the
     *   `.Clear()` method.
     *
     * - When setting the `.Rtf` or `.SelectedRtf` properties, and the
     *   RTF string does not contain a font table:
     *   - If the control does not already contain a font table, the
     *     font table is initialized with the `.Font.Name` property
     *     of the RichTextBox control, and the RTF string will be set
     *     to use that font.
     *   - If the control already contains a font table, the last
     *     specified fontname prior to the insert point is used for the
     *     RTF string.
     *
     * - When setting the `.Rtf` or `.SelectedRtf` properties, and the
     *   RTF string does not contain a color table:
     *   - If the RTF string contains a font table:
     *     - The font colour is defaulted to black.
     *   - If the RTF string does not contain a font table:
     *     - If the control does not already contain a colour table,
     *       the colour table is initialized with the `.ForeColor`
     *       property of the RichTextBox control, and the RTF string
     *       will be set to use that colour.
     *     - If the control already contains a colour table, the last
     *       specified colour prior to the insert point is used for the
     *       RTF string.
     *
     * - When setting the `.Rtf` or `.SelectedRtf` properties, and the
     *   RTF string does not contain a font size (\fsN):
     *   - When setting the RTF string using the `.Rtf` property:
     *     The default fontsize of 24 is used.
     *   - When setting the RTF string using the `.SelectedRtf`
     *     property, regardless of the current selection length and
     *     position (so appending to end, inserting at beginning,
     *     replacing text, etc) :
     *     - If the RTF string does not contain a font table:
     *       - If the control does not already contain a font table,
     *          the font size is initialized with the
     *          `.Font.SizeInPoints` property of the RichTextBox
     *          control, and the RTF string will be set to use that
     *          font size.
     *     - If the control already contains a font table, the last
     *       specified fontsize prior to the insert point is used for
     *       the RTF string.
     *
     * Note that the extension method `AppendRtf()` found in the
     * TheBlackRoom.WinForms.Extensions namespace always uses the
     * `.SelectedRtf` property internally to set the RTF string.
     */
    public static class RTFEncoding
    {
        /// <summary>
        /// Formats an markup string to RTF. Markup is similar to BBCode,
        /// however ending tags are not required, and do not need to follow
        /// the same nesting levels if they are used.
        ///
        /// Supports the following markup:
        /// [b]                   - bold on
        /// [/b]                  - bold off
        /// [i]                   - italic on
        /// [/i]                  - italic off
        /// [u] [ul]              - underline on
        /// [/u] [/ul]            - underline off
        /// [font=FontName]       - set font to FontName
        /// [/font]               - set default font, or first font in pre-existing fonttable
        /// [size=FontSize]       - set fontsize
        /// [/size]               - set default font size, if not 0
        /// [color=#RRGGBB]       - set colour from hex values red=RR green=GG blue=BB
        /// [color=rgb:RR/GG/BB]  - set colour from hex values red=RR green=GG blue=BB
        /// [color=Name]          - set colour to Name
        /// [/color]              - set colour to default colour, or first colour in pre-existing colourtable
        /// </summary>
        /// <param name="Message">String to convert to RTF</param>
        /// <param name="DefaultColor">Default colour of returned RTF string, can be empty for no default</param>
        /// <param name="DefaultFont">Default font of returned RTF string, can be empty for no default</param>
        /// <param name="DefaultFontSizeInPoints">Default font size of returned RTF string, can be 0 for no default</param>
        /// <returns>RTF String</returns>
        public static string FormatToRtf(string Message, Color DefaultColor, string DefaultFont, float DefaultFontSizeInPoints)
        {
            if (Message == null)
                return null;

            //Ensure size is valid when converted to twips and back
            //as RichTextBox internally uses twips for font height
            var points = Math.Round(DefaultFontSizeInPoints * 20) / 20;

            //RichTextBox requires halfpoints to have no decimal,
            //so cast to int to drop the decimal after soubling.
            //This allows sizeInPoints to be a 1/2 size (.5).
            var halfPoints = (int)(points * 2);

            //escape rtf control characters
            Message = Message.Replace(@"\", @"\\");
            Message = Message.Replace("{", @"\{");
            Message = Message.Replace("}", @"\}");

            //newline
            Message = Message.Replace(Environment.NewLine, @"\line ");
            Message = Message.Replace("\n", @"\line ");
            Message = Message.Replace("\r", @"\line ");

            //italics
            Message = Message.Replace("[i]", @"\i1 ");
            Message = Message.Replace("[/i]", @"\i0 ");

            //bold
            Message = Message.Replace("[b]", @"\b1 ");
            Message = Message.Replace("[/b]", @"\b0 ");

            //underline
            Message = Message.Replace("[ul]", @"\ul1 ");
            Message = Message.Replace("[/ul]", @"\ul0 ");
            Message = Message.Replace("[u]", @"\ul1 ");
            Message = Message.Replace("[/u]", @"\ul0 ");

            //fonts
            Message = Message.Replace("[/font]", @"\f0 ");

            //font size
            Message = Regex.Replace(Message, @"\[/size\]", x =>
            {
                if (DefaultFontSizeInPoints > 0)
                    return $@"\fs{halfPoints} ";

                return string.Empty;
            });

            //colours
            Message = Message.Replace("[/color]", @"\cf1 ");

            //create colour list
            var colours = new List<Color>();

            if (!DefaultColor.IsEmpty && (DefaultColor != Color.Transparent))
                colours.Add(DefaultColor);

            //create font list
            var fonts = new List<string>();

            if (!string.IsNullOrEmpty(DefaultFont))
                fonts.Add(DefaultFont);


            //color=#rrggbb (hex)
            Message = Regex.Replace(Message, @"\[color=#([0-9A-Fa-f][0-9A-Fa-f])([0-9A-Fa-f][0-9A-Fa-f])([0-9A-Fa-f][0-9A-Fa-f]).*?\]", x =>
            {
                var r = int.Parse(x.Groups[1].Value, global::System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(x.Groups[2].Value, global::System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(x.Groups[3].Value, global::System.Globalization.NumberStyles.HexNumber);
                var c = Color.FromArgb(r, g, b);

                var ix = colours.IndexOf(c);
                if (ix == -1)
                {
                    colours.Add(c);
                    ix = colours.Count - 1;
                }

                //found the colour in the list, the cr code is 1 based
                return $@"\cf{ix + 1} ";
            });

            //color=rgb:rrggbb (hex)
            Message = Regex.Replace(Message, @"\[color=rgb:([0-9A-Fa-f][0-9A-Fa-f])/([0-9A-Fa-f][0-9A-Fa-f])/([0-9A-Fa-f][0-9A-Fa-f])\]", x =>
            {
                var r = int.Parse(x.Groups[1].Value, global::System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(x.Groups[2].Value, global::System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(x.Groups[3].Value, global::System.Globalization.NumberStyles.HexNumber);
                var c = Color.FromArgb(r, g, b);

                var ix = colours.IndexOf(c);
                if (ix == -1)
                {
                    colours.Add(c);
                    ix = colours.Count - 1;
                }

                //found the colour in the list, the cr code is 1 based
                return $@"\cf{ix + 1} ";
            });

            //color=name
            Message = Regex.Replace(Message, @"\[color=([a-zA-Z]+)\]", x =>
            {
                var c = Color.FromName(x.Groups[1].Value);

                var ix = colours.IndexOf(c);
                if (ix == -1)
                {
                    colours.Add(c);
                    ix = colours.Count - 1;
                }

                //found the colour in the list, the cr code is 1 based
                return $@"\cf{ix + 1} ";
            });

            //font=name
            Message = Regex.Replace(Message, @"\[font=([a-zA-Z]+)\]", x =>
            {
                var fontname = x.Groups[1].Value;

                var ix = fonts.IndexOf(fontname);
                if (ix == -1)
                {
                    fonts.Add(fontname);
                    ix = fonts.Count - 1;
                }

                //found the font in the list, the font name is 0 based
                return $@"\f{ix} ";
            });

            //size=points
            Message = Regex.Replace(Message, @"\[size=([+-\.[0-9]+)\]", x =>
            {
                if (float.TryParse(x.Groups[1].Value, out var size))
                    return $@"\fs{(int)(size * 2)} ";

                return string.Empty;
            });

            //build rtf display
            var Header = @"{\rtf1\ansi" + Environment.NewLine;

            var MessagePrefix = string.Empty;

            //font table
            var FontTable = string.Empty;

            if (fonts.Count > 0)
            {
                FontTable = @"{\fonttbl";

                for (int ix = 0; ix < fonts.Count; ix++)
                    FontTable += $@"{{\f{ix} {fonts[ix]};}}";

                FontTable += "}" + Environment.NewLine;

                MessagePrefix += @"\f0";
            }


            //colour table
            var ColourTable = string.Empty;

            if (colours.Count > 0)
            {
                ColourTable = @"{\colortbl ;";

                foreach (var c in colours)
                    ColourTable += $@"\red{c.R}\green{c.G}\blue{c.B};";
                ColourTable += "}" + Environment.NewLine;

                MessagePrefix += @"\cf1";
            }

            //font size
            if (DefaultFontSizeInPoints > 0)
                MessagePrefix += $@"\fs{halfPoints}";

            if (!string.IsNullOrEmpty(MessagePrefix))
                MessagePrefix += " ";

            var Footer = Environment.NewLine + @"}";

            //format RTF
            return $"{Header}{FontTable}{ColourTable}{MessagePrefix}{Message}{Footer}";
        }

        /// <summary>
        /// RTF control codes for generating a newline
        /// </summary>
        public static string RtfNewline => @"{\rtf1\ansi \line}";
    }
}
