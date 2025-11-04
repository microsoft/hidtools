// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PermitCustomVariableItemSize TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "permitcustomvariableitemsize", typeof(bool))]
    public class PermitCustomVariableItemSizeTag : BaseTag
    {
        private PermitCustomVariableItemSizeTag(bool value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PermitCustomVariableItemSizeTag"/> is asserted.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="PermitCustomVariableItemSizeTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="PermitCustomVariableItemSizeTag"/>.</param>
        /// <returns>New <see cref="PermitCustomVariableItemSizeTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static PermitCustomVariableItemSizeTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(PermitCustomVariableItemSizeTag), rawTag))
            {
                return null;
            }

            bool value = (bool)(rawTag.Value);

            return new PermitCustomVariableItemSizeTag(value, rawTag);
        }
    }
}