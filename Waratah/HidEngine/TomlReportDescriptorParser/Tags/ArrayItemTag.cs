// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents an ArrayItem TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "arrayItem", typeof(Dictionary<string, object>[]))]
    [SupportedHidKinds(HidUsageKind.BufferedBytes, HidUsageKind.DF, HidUsageKind.DV, HidUsageKind.LC, HidUsageKind.MC, HidUsageKind.NAry, HidUsageKind.OOC, HidUsageKind.OSC, HidUsageKind.RTC, HidUsageKind.Sel, HidUsageKind.SF, HidUsageKind.SV, HidUsageKind.UM)]
    public class ArrayItemTag : BaseTag, IModuleGeneratorTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayItemTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private ArrayItemTag(
            UsageRangeTag usageRange,
            CountTag count,
            ReportFlagsTag reportFlags,
            NameTag name,
            KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.UsageRange = usageRange;
            this.Count = count;
            this.ReportFlags = reportFlags;
            this.Name = name;
        }

        /// <summary>
        /// Gets the associated UsageRange for the ArrayItem.
        /// Will be null when <see cref="IsRange"/> is false.
        /// </summary>
        public UsageRangeTag UsageRange { get; }

        /// <summary>
        /// Gets the Count for the ArrayItem.
        /// </summary>
        public CountTag Count { get; }

        /// <summary>
        /// Gets the ReportFlags for the ArrayItem.
        /// </summary>
        public ReportFlagsTag ReportFlags { get; }

        /// <summary>
        /// Gets the (optional) Name of the ArrayItem.
        /// </summary>
        public NameTag Name { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="ArrayItemTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an ArrayItem.</param>
        /// <returns>New <see cref="ArrayItemTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        public static ArrayItemTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(ArrayItemTag), rawTag))
            {
                return null;
            }

            UsageRangeTag usageRange = null;
            CountTag count = null;
            ReportFlagsTag reportFlags = null;
            NameTag name = null;

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (usageRange == null)
                {
                    usageRange = UsageRangeTag.TryParse(child, typeof(ArrayItemTag));

                    if (usageRange != null)
                    {
                        continue;
                    }
                }

                if (count == null)
                {
                    count = CountTag.TryParse(child);

                    if (count != null)
                    {
                        continue;
                    }
                }

                if (reportFlags == null)
                {
                    reportFlags = ReportFlagsTag.TryParse(child);

                    if (reportFlags != null)
                    {
                        continue;
                    }
                }

                if (name == null)
                {
                    name = NameTag.TryParse(child);

                    if (name != null)
                    {
                        continue;
                    }
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (usageRange == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlArrayItemMissingUsageRangeTag, rawTag);
            }

            return new ArrayItemTag(usageRange, count, reportFlags, name, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            try
            {
                ArrayModule module = new ArrayModule(
                    this.UsageRange.UsageStart,
                    this.UsageRange.UsageEnd,
                    this.Count?.Value,
                    this.ReportFlags?.Value,
                    this.Name?.Value,
                    parent);

                return module;
            }
            catch (DescriptorModuleParsingException parsingException)
            {
                // If exception thrown, catch it, and then give the line number (tacked on the end).
                throw new TomlGenericException(parsingException, this);
            }
        }
    }
}