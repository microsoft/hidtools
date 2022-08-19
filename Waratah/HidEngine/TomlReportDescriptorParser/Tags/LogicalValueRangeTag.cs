// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;

    /// <summary>
    /// Represents a Logical ValueRange TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "logicalValueRange", typeof(object[]), typeof(string))]
    public class LogicalValueRangeTag : BaseValueRangeBaseTag
    {
        private LogicalValueRangeTag(DescriptorRange range, KeyValuePair<string, object> rawTag)
            : base(range, rawTag)
        {
        }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="LogicalValueRangeTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an LogicalValueRangeTag.</param>
        /// <returns>New <see cref="InputReportTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static LogicalValueRangeTag TryParse(KeyValuePair<string, object> rawTag)
        {
            try
            {
                if (!IsValidNameAndType(typeof(LogicalValueRangeTag), rawTag))
                {
                    return null;
                }
            }
            catch (TomlGenericException)
            {
                // Catch this exception of incorrect type, so TryParseInternal can do better analysis.
            }

            try
            {
                DescriptorRange parsed = BaseValueRangeBaseTag.TryParseInternal(rawTag);

                return new LogicalValueRangeTag(parsed, rawTag);
            }
            catch (DescriptorModuleParsingException e)
            {
                throw new TomlGenericException(e, rawTag);
            }
        }
    }
}
