// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine
{
    using System;

    /// <summary>
    /// Extension to System.String.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Extension Class")]
    public static class StringExtension
    {
        /// <summary>
        /// Replaces all occurrences of a string (a) within this string, with another string (b).
        /// </summary>
        /// <param name="self">This string.</param>
        /// <param name="oldString">Substring to replace.</param>
        /// <param name="newString">String to replace oldString substring with.</param>
        /// <param name="firstOccurrenceOnly">Whether to replace only the first occurrence of oldString.</param>
        /// <returns>String with occurrences replaced.</returns>
        /// https://stackoverflow.com/questions/12368480/how-do-i-replace-the-first-occurrence-of-a-substring-in-net
        public static String Replace(this string self, string oldString, string newString, bool firstOccurrenceOnly = false)
        {
            if (!firstOccurrenceOnly)
            {
                return self.Replace(oldString, newString);
            }

            int pos = self.IndexOf(oldString);
            if (pos < 0)
            {
                return self;
            }
            else
            {
                return self.Substring(0, pos) + newString + self.Substring(pos + oldString.Length);
            }
        }
    }
}
