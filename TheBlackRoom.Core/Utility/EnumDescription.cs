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

namespace TheBlackRoom.System.Utility
{
    /// <summary>
    /// Utility class to parse enum descriptions back into enums
    /// </summary>
    public static class EnumDescription
    {
        /// <summary>
        /// Converts an enum [Description("...")] attribute to an equivalent enumerated object.
        /// </summary>
        /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
        public static bool TryParse<T>(string value, out T result) where T : Enum
        {
            return TryParse<T>(value, false, out result);
        }

        /// <summary>
        /// Converts an enum [Description("...")] attribute to an equivalent enumerated object.
        /// </summary>
        /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
        public static bool TryParse<T>(string value, bool ignoreCase, out T result) where T : Enum
        {
            result = default(T);

            var enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.Name} is not an enum");

            //check each description for a match
            foreach (var field in enumType.GetFields())
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr == null)
                    continue;

                if (attr.Description.Equals((string)value,
                    ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                {
                    result = (T)Enum.Parse(enumType, field.Name);
                    return true;
                }
            }

            //no match, try to parse value as an enum
            try
            {
                result = (T)Enum.Parse(enumType, value, ignoreCase);
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is OverflowException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts an enum [Description("...")] attribute to an equivalent enumerated object.
        /// </summary>
        /// <returns>
        /// An object of type <paramref name="enumType"/> whose value is represented by <paramref name="value"/>.
        /// </returns>
        public static object Parse(Type enumType, string value)
        {
            return Parse(enumType, value, false);
        }

        /// <summary>
        /// Converts an enum [Description("...")] attribute to an equivalent enumerated object.
        /// </summary>
        /// <returns>
        /// An object of type <paramref name="enumType"/> whose value is represented by <paramref name="value"/>.
        /// </returns>
        public static object Parse(Type enumType, string value, bool ignoreCase)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.Name} is not an enum");

            //check each description for a match
            foreach (var field in enumType.GetFields())
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr == null)
                    continue;

                if (attr.Description.Equals((string)value,
                    ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                {
                    return Enum.Parse(enumType, field.Name);
                }
            }

            //no match, try to parse value as an enum
            return Enum.Parse(enumType, value, ignoreCase);
        }
    }
}
