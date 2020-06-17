using System.Text.RegularExpressions;
using UnityEngine;

namespace Extensions.String
{
    public static class StringExtensions
    {
        public static Regex capitalRegex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);


        /// <summary>
        /// Checks how many times a specific char in inside the string
        /// </summary>
        /// <param name="text">String to search through</param>
        /// <param name="c">The char the functions looks for</param>
        /// <returns>The amount of times the char is present inside the string</returns>
        public static int SpecificCharacterCount(this string text, char c)
        {
            int a = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != c) continue;
                a++;
            }

            return a;
        }

        public static bool Matches(this string t1, string t2)
        {
            if (t1.Length > t2.Length)
            {
                return false;
            }

            for (int i = 0; i < t1.Length; i++)
            {
                if (t1[i] != t2[i])
                {
                    return false;
                } 
            }

            return true;
        }

        public static bool MatchesAny(this char c, params char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if(c == chars[i])
                {
                    return true;
                }
            }

            return false;
        }

        public static int ClosestFirstIndexOf(this string text, int maxIndex, char c)
        {
            int index = -1;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == c && i < maxIndex)
                {
                    index = i;
                }
            }

            return index;
        }
    }
}
