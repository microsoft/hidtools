// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.ComTypes;
    using Microsoft.HidTools.HidEngine.Properties;

    /// <summary>
    /// Represents a OutputFormat TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "outputformat", typeof(string))]
    public class OutputFormatTag : BaseTag
    {
        private OutputFormatTag(OutputFormatKind value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="OutputFormatTag"/>.
        /// </summary>
        public OutputFormatKind Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="OutputFormatTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a OutputFormatTag.</param>
        /// <returns>New <see cref="OutputFormatTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are invalid.</exception>
        public static OutputFormatTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(OutputFormatTag), rawTag))
            {
                return null;
            }

            string outputFormatName = (string)rawTag.Value;

            if (!Enum.TryParse(outputFormatName, out OutputFormatKind kind) && Enum.IsDefined(typeof(OutputFormatKind), kind))
            {
                throw new TomlGenericException(Resources.ExceptionTomlOutputFormatKindInvalid, rawTag, outputFormatName, string.Join(", ", Enum.GetNames(typeof(OutputFormatKind))));
            }

            return new OutputFormatTag(kind, rawTag);
        }
    }
}
