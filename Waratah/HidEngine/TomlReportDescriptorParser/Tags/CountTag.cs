// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.Properties;

    /// <summary>
    /// Represents a Count TOML tag.
    /// CountTags allow for multiple, identical Items.
    /// Must be associated with an Item.
    /// </summary>
    [TagAttribute(TagKind.Key, "count", typeof(Int64))]
    public class CountTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        /// <param name="value">The value of this <see cref="CountTag"/>.</param>
        /// <param name="rawTag">Root TOML element describing a Count.</param>
        private CountTag(int value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="CountTag"/>.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="CountTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Count.</param>
        /// <returns>New <see cref="CountTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are out of bounds.</exception>
        public static CountTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(CountTag), rawTag))
            {
                return null;
            }

            Int64 count = (Int64)rawTag.Value;
            if (count == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotBeZero, rawTag);
            }

            return new CountTag(Helpers.SafeGetInt32(count, rawTag), rawTag);
        }
    }
}
