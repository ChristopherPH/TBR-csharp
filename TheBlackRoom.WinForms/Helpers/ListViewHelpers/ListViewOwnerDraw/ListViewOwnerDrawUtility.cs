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

namespace TheBlackRoom.WinForms.Helpers.ListViewHelpers.ListViewOwnerDraw
{
    public static class ListViewOwnerDrawUtility
    {
        public static void Inflate(this ref Rectangle srcRect, int leftAmount,
            int topAmount, int rightAmount, int bottomAmount)
        {
            if (leftAmount != 0)
            {
                srcRect.X -= leftAmount;
                srcRect.Width += leftAmount;
            }

            if (topAmount != 0)
            {
                srcRect.Y -= topAmount;
                srcRect.Height += topAmount;
            }

            srcRect.Width += rightAmount;
            srcRect.Height += bottomAmount;
        }

        public static void Inflate(this ref Rectangle srcRect, int amount)
        {
            srcRect.Inflate(amount, amount);
        }

        public static void Shrink(this ref Rectangle srcRect, int leftAmount,
            int topAmount, int rightAmount, int bottomAmount)
        {
            srcRect.Inflate(-leftAmount, -topAmount, -rightAmount, -bottomAmount);
        }

        public static void Shrink(this ref Rectangle srcRect, int amount)
        {
            srcRect.Inflate(-amount, -amount);
        }

        public static void Shrink(this ref Rectangle srcRect, int x, int y)
        {
            srcRect.Inflate(-x, -y);
        }
    }
}
