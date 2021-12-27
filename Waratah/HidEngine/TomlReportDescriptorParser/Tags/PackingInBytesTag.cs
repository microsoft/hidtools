// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Packing TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "packingInBytes", typeof(Int64))]
    public class PackingInBytesTag : BaseTag
    {
        private PackingInBytesTag(Int32 value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="PackingInBytesTag"/>.
        /// </summary>
        public Int32 Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="PackingInBytesTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="PackingInBytesTag"/>.</param>
        /// <returns>New <see cref="PackingInBytesTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static PackingInBytesTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(PackingInBytesTag), rawTag))
            {
                return null;
            }

            Int64 value = (Int64)(rawTag.Value);

            return new PackingInBytesTag(Helpers.SafeGetInt32(value, rawTag), rawTag);
        }
    }
}