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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Helpers.ListViewHelpers.ListViewOwnerDraw
{
    /// <summary>
    /// Provides data for the ListViewOwnerDrawHelper.OwnerDrawItemTile event.
    /// </summary>
    public class ListViewOwnerDrawItemTileEventArgs : ListViewOwnerDrawItemEventArgs
    {
        public ListViewOwnerDrawItemTileEventArgs(DrawListViewItemEventArgs eventArgs,
            Rectangle textBounds, TextFormatFlags textFlags, Rectangle imageBounds, ImageList imageList,
            Rectangle tileBounds, int tileLines)
            : base(eventArgs, textBounds, textFlags, imageBounds, imageList)
        {
            TileBounds = tileBounds;
            TileLines = tileLines;
        }
        public Rectangle TileBounds { get; }
        public int TileLines { get; }
        public bool TilesIncludeColumnHeaders { get; set; } = false;

        const float LightenAmount = 0.20f;

        public override void DrawText()
        {
            base.DrawText();

            var color = ForeColor;

            //If the color isn't disabled, highlighted, etc, then lighten it
            if (color == EventArgs.Item.ForeColor)
                color = GetTileTextColor(color);

            DrawTileText(color);
        }

        public override void DrawText(Color foreColor)
        {
            base.DrawText(foreColor);

            DrawText(foreColor, true);
        }

        public void DrawText(Color foreColor, bool lighten)
        {
            base.DrawText(foreColor);

            if (lighten)
                DrawTileText(GetTileTextColor(foreColor));
            else
                DrawTileText(foreColor);
        }

        /// <summary>
        /// Draws the background of the item with the specified color and tile text color.
        /// </summary>
        public void DrawText(Color foreColor, Color secondaryColor)
        {
            base.DrawText(foreColor);

            DrawTileText(secondaryColor);
        }

        void DrawTileText(Color foreColor)
        {
            if (TileBounds.IsEmpty || (TileLines < 0))
                return;

            var listView = EventArgs.Item?.ListView;
            if ((listView == null) || (listView.View != View.Tile))
                return;

            var sb = new StringBuilder();

            for (int i = 1; i < TileLines; i++)
            {
                if (TilesIncludeColumnHeaders)
                    sb.AppendFormat("{0}: {1}\n", listView.Columns[i].Text, EventArgs.Item.SubItems[i].Text);
                else
                    sb.AppendLine(EventArgs.Item.SubItems[i].Text);
            }

            TextRenderer.DrawText(EventArgs.Graphics, sb.ToString(),
                EventArgs.Item.Font, TileBounds, foreColor, TextFlags);
        }

        Color GetTileTextColor(Color foreColor)
        {
            if (RGBEquals(foreColor, Color.Black))
                return Color.Gray;


            return ControlPaint.Light(foreColor, LightenAmount);
        }

        bool RGBEquals(Color c1, Color c2)
        {
            return (c1.R == c2.R) && (c1.G == c2.G) && (c1.B == c2.B);
        }
    }
}
