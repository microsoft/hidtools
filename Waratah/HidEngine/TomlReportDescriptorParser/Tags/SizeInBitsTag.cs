// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents a SizeInBits TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "sizeInBits", typeof(Int64))]
    public class SizeInBitsTag : BaseTag
    {
        private SizeInBitsTag(byte value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the SizeInBitsTag.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="SizeInBitsTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Size.</param>
        /// <returns>New <see cref="SizeInBitsTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static SizeInBitsTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(SizeInBitsTag), rawTag))
            {
                return null;
            }

            Int64 sizeInBits = (Int64)(rawTag.Value);

            if (sizeInBits == 0)
            {
                throw new TomlGenericException(HidEngine.Properties.Resources.ExceptionTomlCannotBeZero, rawTag);
            }

            return new SizeInBitsTag(Helpers.SafeGetByte(sizeInBits, rawTag), rawTag);
        }
    }
}
