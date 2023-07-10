// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents an Feature Report TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "outputReport", typeof(Dictionary<string, object>[]))]
    public class OutputReportTag : BaseReportTag, IModuleGeneratorTag
    {
        // Unique report ids for across all Output reports.
        private static List<int> uniqueReportIds = new List<int>();

        private OutputReportTag(IdTag id, NameTag name, UsageRelationTag relation, List<IModuleGeneratorTag> items, KeyValuePair<string, object> rawTag)
            : base(id, name, relation, items, rawTag)
        {
        }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="OutputReportTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an Output Report.</param>
        /// <returns>New <see cref="OutputReportTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static OutputReportTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(OutputReportTag), rawTag))
            {
                return null;
            }

            var parsed = BaseReportTag.TryParseInternal(rawTag);

            return new OutputReportTag(parsed.id, parsed.name, parsed.relation, parsed.items, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            return this.GenerateDescriptorModuleInternal(parent, ReportKind.Output, uniqueReportIds);
        }
    }
}
