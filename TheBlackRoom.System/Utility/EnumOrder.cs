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
using System.Linq;

namespace TheBlackRoom.System.Utility
{
    public static class EnumOrder
    {
        /// <summary>
        /// Gets enum values in declaration order
        /// </summary>
        public static Array GetOrderedValues(Type EnumType)
        {
            if (EnumType == null) throw new ArgumentNullException(nameof(EnumType));
            if (!EnumType.IsEnum) throw new ArgumentException(nameof(EnumType));

            return EnumType.GetFields()
                .Where(x => x.IsStatic)
                .OrderBy(x => x.MetadataToken)
                .Select(x => x.GetValue(null))
                .ToArray();
        }

        /// <summary>
        /// Compares enum values using declaration order
        /// </summary>
        public static int CompareOrderedValues<T>(T x, T y) where T : Enum
        {
            var type = typeof(T);

            var fx = type.GetField(x.ToString());
            var fy = type.GetField(y.ToString());

            if ((fx != null) && (fy != null))
                return fx.MetadataToken.CompareTo(fy.MetadataToken);
            else if (fx != null)
                return 1;
            else if (fy != null)
                return -1;
            else
                return 0;
        }
    }
}
