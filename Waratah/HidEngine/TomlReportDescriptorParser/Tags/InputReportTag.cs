// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents an Input Report TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "inputReport", typeof(Dictionary<string, object>[]))]
    public class InputReportTag : BaseReportTag, IModuleGeneratorTag
    {
        // Unique report ids for across all Input reports.
        private static List<int> uniqueReportIds = new List<int>();

        private InputReportTag(IdTag id, List<IModuleGeneratorTag> items, KeyValuePair<string, object> rawTag)
            : base(id, items, rawTag)
        {
        }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="InputReportTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an InputReport.</param>
        /// <returns>New <see cref="InputReportTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static InputReportTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(InputReportTag), rawTag))
            {
                return null;
            }

            var parsed = BaseReportTag.TryParseInternal(rawTag);

            return new InputReportTag(parsed.id, parsed.items, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            return this.GenerateDescriptorModuleInternal(parent, ReportKind.Input, uniqueReportIds);
        }
    }
}
