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
using System.Collections.Generic;
using System.Linq;

namespace TheBlackRoom.System.Extensions
{
    public static class IEnumerable_T_Concat
    {
        /// <summary>
        /// Concatinates an element to a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The element to concatenate to the first sequence.</param>
        /// <returns>An IEnumerable<T> that contains the concatenated elements of the input sequences and element</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            //Cast, to ensure the correct Concat is called
            return first.Concat((IEnumerable<TSource>)(new TSource[] { second }));
        }

        /// <summary>
        /// Concatinates an element or elements to a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The element or elements to concatenate to the first sequence.</param>
        /// <returns>An IEnumerable<T> that contains the concatenated elements of the input sequences and element or elements</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, params TSource[] second)
        {
            //Cast, to ensure the correct Concat is called
            return first.Concat((IEnumerable<TSource>)second);
        }
    }
}
