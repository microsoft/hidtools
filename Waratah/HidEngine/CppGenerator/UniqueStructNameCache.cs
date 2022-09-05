// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Cache of unique type/variable CPP names.
    /// </summary>
    public static class UniqueStructNameCache
    {
        private static Dictionary<string, int> uniqueNames = new Dictionary<string, int>();

        /// <summary>
        /// Resets the cache.  Should be called before process a struct.
        /// </summary>
        public static void Reset()
        {
            uniqueNames.Clear();
        }

        /// <summary>
        /// Generates a unique id for a given name.
        /// </summary>
        /// <param name="name">Name to ensure uniqueness from.</param>
        /// <returns>Unique id for a given name.</returns>
        public static string GenerateUniqueName(string name)
        {
            // Ensure all elements are named uniquely.
            if (uniqueNames.ContainsKey(name))
            {
                uniqueNames[name]++;
            }
            else
            {
                uniqueNames[name] = 0;
            }

            int uniqueNameSuffix = uniqueNames[name];
            string nameIdSuffix = (uniqueNameSuffix == 0) ? string.Empty : uniqueNameSuffix.ToString();

            string suffixedName = $"{name}{nameIdSuffix}";

            // Validate the (new) name (which may now include a suffix), doesn't collide with any other name.
            if (uniqueNames[name] == 0)
            {
                return suffixedName;
            }
            else
            {
                // Collision found with new name, so try adding another suffix.
                return GenerateUniqueName(suffixedName);
            }
        }
    }
}
