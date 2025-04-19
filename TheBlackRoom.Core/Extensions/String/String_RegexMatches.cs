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
using System.Text.RegularExpressions;

namespace TheBlackRoom.Core.Extensions
{
    public static class String_RegexMatches
    {
        /// <summary>
        /// Matches the input string against the specified regular expression pattern
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="pattern">Regular Expression Pattern</param>
        /// <param name="options">Optional regular expression options</param>
        /// <returns>true if match was found, false if not</returns>
        public static bool RegexMatches(this string input, string pattern,
               RegexOptions options = RegexOptions.None)
        {
            return Regex.Match(input, pattern, options).Success;
        }

        /// <summary>
        /// Matches the input string against the specified regular expression pattern
        /// and returns the first match.
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="pattern">Regular Expression Pattern with 1 capture group</param>
        /// <param name="match">1st match</param>
        /// <param name="options">Optional regular expression options</param>
        /// <returns>true if 1 match was found, false if not</returns>
        public static bool RegexMatches(this string input, string pattern, out string match,
               RegexOptions options = RegexOptions.None)
        {
            var m = Regex.Match(input, pattern, options);

            if (m.Success && m.Groups.Count >= 2)
            {
                match = m.Groups[1].Value;
                return true;
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Matches the input string against the specified regular expression pattern
        /// and returns the first two matches.
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="pattern">Regular Expression Pattern with 2 capture groups</param>
        /// <param name="match1">1st match</param>
        /// <param name="match2">2nd match</param>
        /// <param name="options">Optional regular expression options</param>
        /// <returns>true if 2 matches were found, false if not</returns>
        public static bool RegexMatches(this string input, string pattern, out string match1,
            out string match2, RegexOptions options = RegexOptions.None)
        {
            var m = Regex.Match(input, pattern, options);

            if (m.Success && m.Groups.Count >= 3)
            {
                match1 = m.Groups[1].Value;
                match2 = m.Groups[2].Value;
                return true;
            }

            match1 = null;
            match2 = null;
            return false;
        }

        /// <summary>
        /// Matches the input string against the specified regular expression pattern
        /// and returns the first three matches.
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="pattern">Regular Expression Pattern with 3 capture groups</param>
        /// <param name="match1">1st match</param>
        /// <param name="match2">2nd match</param>
        /// <param name="match3">3rd match</param>
        /// <param name="options">Optional regular expression options</param>
        /// <returns>true if 3 matches were found, false if not</returns>
        public static bool RegexMatches(this string input, string pattern, out string match1,
            out string match2, out string match3, RegexOptions options = RegexOptions.None)
        {
            var m = Regex.Match(input, pattern, options);

            if (m.Success && m.Groups.Count >= 4)
            {
                match1 = m.Groups[1].Value;
                match2 = m.Groups[2].Value;
                match3 = m.Groups[3].Value;
                return true;
            }

            match1 = null;
            match2 = null;
            match3 = null;
            return false;
        }

        /// <summary>
        /// Matches the input string against the specified regular expression pattern
        /// and returns the first three matches.
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="pattern">Regular Expression Pattern with 3 capture groups</param>
        /// <param name="match1">1st match</param>
        /// <param name="match2">2nd match</param>
        /// <param name="match3">3rd match</param>
        /// <param name="match3">4th match</param>
        /// <param name="options">Optional regular expression options</param>
        /// <returns>true if 4 matches were found, false if not</returns>
        public static bool RegexMatches(this string input, string pattern, out string match1,
            out string match2, out string match3, out string match4,
            RegexOptions options = RegexOptions.None)
        {
            var m = Regex.Match(input, pattern, options);

            if (m.Success && m.Groups.Count >= 5)
            {
                match1 = m.Groups[1].Value;
                match2 = m.Groups[2].Value;
                match3 = m.Groups[3].Value;
                match4 = m.Groups[4].Value;
                return true;
            }

            match1 = null;
            match2 = null;
            match3 = null;
            match4 = null;
            return false;
        }
    }
}
