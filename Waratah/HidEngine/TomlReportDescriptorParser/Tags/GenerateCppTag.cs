// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a GenerateCpp TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "generateCpp", typeof(bool))]
    public class GenerateCppTag : BaseTag
    {
        private GenerateCppTag(bool value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GenerateCppTag"/> is asserted.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="GenerateCppTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="GenerateCppTag"/>.</param>
        /// <returns>New <see cref="GenerateCppTag"/> or null if it cannot be parsed as one.</returns>
        public static GenerateCppTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(GenerateCppTag), rawTag))
            {
                return null;
            }

            bool value = (bool)(rawTag.Value);

            return new GenerateCppTag(value, rawTag);
        }
    }
}