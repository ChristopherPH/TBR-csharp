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
using System.Drawing;

namespace TheBlackRoom.Gfx.Extensions
{
    public static class Rectangle_Slice
    {
        /// <summary>
        /// Enlarges this System.Drawing.Rectangle by the specified amount.
        /// </summary>
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

        /// <summary>
        /// Enlarges this System.Drawing.Rectangle by the specified amount.
        /// </summary>
        public static void Inflate(this ref Rectangle srcRect, int amount)
        {
            srcRect.Inflate(amount, amount);
        }

        /// <summary>
        /// Shrinks this System.Drawing.Rectangle by the specified amount.
        /// </summary>
        public static void Shrink(this ref Rectangle srcRect, int leftAmount,
            int topAmount, int rightAmount, int bottomAmount)
        {
            srcRect.Inflate(-leftAmount, -topAmount, -rightAmount, -bottomAmount);
        }

        /// <summary>
        /// Shrinks this System.Drawing.Rectangle by the specified amount.
        /// </summary>
        public static void Shrink(this ref Rectangle srcRect, int amount)
        {
            srcRect.Inflate(-amount, -amount);
        }

        /// <summary>
        /// Shrinks this System.Drawing.Rectangle by the specified amount.
        /// </summary>
        public static void Shrink(this ref Rectangle srcRect, int x, int y)
        {
            srcRect.Inflate(-x, -y);
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceLeft(this Rectangle SrcRect, int Amount, out Rectangle Remainder)
        {
            if (SrcRect.IsEmpty || (Amount < 0))
            {
                Remainder = Rectangle.Empty;
                return Rectangle.Empty;
            }

            if (Amount >= SrcRect.Width)
            {
                Remainder = Rectangle.Empty;
                return SrcRect;
            }

            //Rectangles are structs so we can just copy them
            Remainder = SrcRect;
            Remainder.X += Amount;
            Remainder.Width -= Amount;

            var rc = SrcRect;
            rc.Width = Amount;

            return rc;
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Percent">0f - 1f</param>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceLeftPercent(this Rectangle SrcRect, float Percent, out Rectangle Remainder)
        {
            return SrcRect.SliceLeft((int)(SrcRect.Width * Percent), out Remainder);
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceRight(this Rectangle SrcRect, int Amount, out Rectangle Remainder)
        {
            if (SrcRect.IsEmpty || (Amount < 0))
            {
                Remainder = Rectangle.Empty;
                return Rectangle.Empty;
            }

            if (Amount >= SrcRect.Width)
            {
                Remainder = Rectangle.Empty;
                return SrcRect;
            }

            //Rectangles are structs so we can just copy them
            Remainder = SrcRect;
            Remainder.Width -= Amount;

            var rc = SrcRect;
            rc.X += SrcRect.Width - Amount;
            rc.Width = Amount;

            return rc;
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Percent">0f - 1f</param>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceRightPercent(this Rectangle SrcRect, float Percent, out Rectangle Remainder)
        {
            return SrcRect.SliceRight((int)(SrcRect.Width * Percent), out Remainder);
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceTop(this Rectangle SrcRect, int Amount, out Rectangle Remainder)
        {
            if (SrcRect.IsEmpty || (Amount < 0))
            {
                Remainder = Rectangle.Empty;
                return Rectangle.Empty;
            }

            if (Amount >= SrcRect.Height)
            {
                Remainder = Rectangle.Empty;
                return SrcRect;
            }

            //Rectangles are structs so we can just copy them
            Remainder = SrcRect;
            Remainder.Y += Amount;
            Remainder.Height -= Amount;

            var rc = SrcRect;
            rc.Height = Amount;

            return rc;
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Percent">0f - 1f</param>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceTopPercent(this Rectangle SrcRect, float Percent, out Rectangle Remainder)
        {
            return SrcRect.SliceTop((int)(SrcRect.Height * Percent), out Remainder);
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceBottom(this Rectangle SrcRect, int Amount, out Rectangle Remainder)
        {
            if (SrcRect.IsEmpty || (Amount < 0))
            {
                Remainder = Rectangle.Empty;
                return Rectangle.Empty;
            }

            if (Amount >= SrcRect.Height)
            {
                Remainder = Rectangle.Empty;
                return SrcRect;
            }

            //Rectangles are structs so we can just copy them
            Remainder = SrcRect;
            Remainder.Height -= Amount;

            var rc = SrcRect;
            rc.Y += SrcRect.Height - Amount;
            rc.Height = Amount;

            return rc;
        }

        /// <summary>
        /// Removes a slice of a rectangle
        /// </summary>
        /// <param name="Percent">0f - 1f</param>
        /// <param name="Remainder">Leftover rectangle after removing the slice</param>
        /// <returns>Sliced rectangle</returns>
        public static Rectangle SliceBottomPercent(this Rectangle SrcRect, float Percent, out Rectangle Remainder)
        {
            return SrcRect.SliceBottom((int)(SrcRect.Height * Percent), out Remainder);
        }
    }
}
