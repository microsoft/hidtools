// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;

    /// <summary>
    /// Represents an ApplicationCollection TOML tag.
    /// Every ApplicationCollection must be associated with a single Usage and 1 or more Reports.
    /// </summary>
    [TagAttribute(TagKind.RootSection, "applicationCollection", typeof(Dictionary<string, object>[]))]
    [SupportedHidKinds(HidUsageKind.CA)]
    public class ApplicationCollectionTag : BaseTag, IModuleGeneratorTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollectionTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        /// <param name="usage">UsageTag associated with this ApplicationCollection.</param>
        /// <param name="reports">Report Tags associated with this ApplicationCollection.</param>
        /// <param name="rawTag">Root TOML element describing an ApplicationCollection.</param>
        private ApplicationCollectionTag(UsageTag usage, List<BaseReportTag> reports, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            // Can potentially be null; validation will happen at different layer.
            this.Usage = usage;

            // Can potentially be empty; validation will happen at different layer.
            this.Reports = reports;
        }

        /// <summary>
        /// Gets the associated Usage for the ApplicationCollection.
        /// </summary>
        public UsageTag Usage { get; }

        /// <summary>
        /// Gets the associated Reports for the ApplicationCollection.
        /// <para>Order of Reports in List is important.</para>
        /// </summary>
        public IReadOnlyList<BaseReportTag> Reports { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="ApplicationCollectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an ApplicationCollection.</param>
        /// <returns>New <see cref="ApplicationCollectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        public static ApplicationCollectionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(ApplicationCollectionTag), rawTag))
            {
                return null;
            }

            UsageTag usage = null;
            List<BaseReportTag> reports = new List<BaseReportTag>();

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                // Looking for Usage and Report tags.
                // Any other tag encountered is an error.

                // Guaranteed by TOML parser that duplicate keys cannot occur.
                // Only bother attempting to parse a Usage if one hasn't been found already.
                if (usage == null)
                {
                    usage = UsageTag.TryParse(child, typeof(ApplicationCollectionTag));
                    if (usage != null)
                    {
                        continue;
                    }
                }

                InputReportTag inputReport = InputReportTag.TryParse(child);
                if (inputReport != null)
                {
                    reports.Add(inputReport);
                    continue;
                }

                OutputReportTag outputReport = OutputReportTag.TryParse(child);
                if (outputReport != null)
                {
                    reports.Add(outputReport);
                    continue;
                }

                FeatureReportTag featureReport = FeatureReportTag.TryParse(child);
                if (featureReport != null)
                {
                    reports.Add(featureReport);
                    continue;
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            return new ApplicationCollectionTag(usage, reports, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            ApplicationCollectionModule applicationCollection = new ApplicationCollectionModule();
            List<ReportModule> reports = new List<ReportModule>();

            foreach (IModuleGeneratorTag tag in this.Reports)
            {
                reports.Add((ReportModule)tag.GenerateDescriptorModule(applicationCollection));
            }

            try
            {
                applicationCollection.Initialize(this.Usage?.Value, reports);
            }
            catch (DescriptorModuleParsingException parsingException)
            {
                // If exception thrown, catch it, and then give the line number (tacked on the end).
                throw new TomlGenericException(parsingException, this);
            }

            return applicationCollection;
        }
    }
}
