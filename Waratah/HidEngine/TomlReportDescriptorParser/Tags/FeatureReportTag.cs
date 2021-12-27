// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents an Feature Report TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "featureReport", typeof(Dictionary<string, object>[]))]
    public class FeatureReportTag : BaseReportTag, IModuleGeneratorTag
    {
        // Unique report ids for across all Feature reports.
        private static List<int> uniqueReportIds = new List<int>();

        private FeatureReportTag(IdTag id, List<IModuleGeneratorTag> items, KeyValuePair<string, object> rawTag)
            : base(id, items, rawTag)
        {
        }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="FeatureReportTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an Feature Report.</param>
        /// <returns>New <see cref="FeatureReportTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static FeatureReportTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(FeatureReportTag), rawTag))
            {
                return null;
            }

            var parsed = TryParseInternal(rawTag);

            return new FeatureReportTag(parsed.id, parsed.items, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            return this.GenerateDescriptorModuleInternal(parent, ReportKind.Feature, uniqueReportIds);
        }
    }
}
