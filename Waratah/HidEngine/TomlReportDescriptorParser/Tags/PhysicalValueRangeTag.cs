// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;

    /// <summary>
    /// Represents a Physical ValueRange TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "physicalValueRange", typeof(object[]), typeof(string))]
    public class PhysicalValueRangeTag : BaseValueRangeBaseTag
    {
        private PhysicalValueRangeTag(DescriptorRange range, KeyValuePair<string, object> rawTag)
            : base(range, rawTag)
        {
        }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="PhysicalValueRangeTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an PhysicalValueRangeTag.</param>
        /// <returns>New <see cref="InputReportTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static PhysicalValueRangeTag TryParse(KeyValuePair<string, object> rawTag)
        {
            try
            {
                if (!IsValidNameAndType(typeof(PhysicalValueRangeTag), rawTag))
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
                DescriptorRange parsed = BaseValueRangeBaseTag.TryParseInternal(rawTag, false);

                return new PhysicalValueRangeTag(parsed, rawTag);
            }
            catch (DescriptorModuleParsingException e)
            {
                throw new TomlGenericException(e, rawTag);
            }
        }
    }
}
