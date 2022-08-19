// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents a Id TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "id", typeof(Int64))]
    public class IdTag : BaseTag
    {
        private IdTag(int value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="IdTag"/>.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the value of this <see cref="IdTag"/> as a UInt16.
        /// </summary>
        public UInt16 ValueUInt16
        {
            get
            {
                return Helpers.SafeGetUInt16(this.Value, this.RawTag);
            }
        }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="IdTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an Id.</param>
        /// <returns>New <see cref="IdTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static IdTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(IdTag), rawTag))
            {
                return null;
            }

            Int64 id = (Int64)(rawTag.Value);
            if (id == 0)
            {
                throw new TomlGenericException(HidEngine.Properties.Resources.ExceptionTomlCannotBeZero, rawTag);
            }

            return new IdTag(Helpers.SafeGetInt32(id, rawTag), rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value}";
        }
    }
}
