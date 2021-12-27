// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Optimize TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "optimize", typeof(bool))]
    public class OptimizeTag : BaseTag
    {
        private OptimizeTag(bool value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OptimizeTag"/> is asserted.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="OptimizeTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="OptimizeTag"/>.</param>
        /// <returns>New <see cref="OptimizeTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static OptimizeTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(OptimizeTag), rawTag))
            {
                return null;
            }

            bool value = (bool)(rawTag.Value);

            return new OptimizeTag(value, rawTag);
        }
    }
}