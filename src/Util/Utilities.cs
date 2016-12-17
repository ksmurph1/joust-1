using System.Collections.Generic;
using System.Linq;
using System;
namespace Util
{
    public sealed class Utilities
    {
        public static string LongestCommonSubstring(string stringA, string stringB)
        {
            List<string> allSubstrings = new List<string>();
            for (int substringLength = stringA.Length - 1; substringLength > 0; substringLength--)
            {
                for (int offset = 0; (substringLength + offset) < stringA.Length; offset++)
                {
                    string currentSubstring = stringA.Substring(offset, substringLength);
                    if (!System.String.IsNullOrWhiteSpace(currentSubstring) && !allSubstrings.Contains(currentSubstring))
                    {
                        allSubstrings.Add(currentSubstring);
                    }
                }
            }

            return allSubstrings.OrderBy(subStr => subStr).ThenByDescending(subStr => subStr.Length).
                   Where(subStr => stringB.Contains(subStr)).DefaultIfEmpty(String.Empty).First();
        }
    }
}