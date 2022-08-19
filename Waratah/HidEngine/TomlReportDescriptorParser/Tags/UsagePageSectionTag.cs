// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a UsagePage section TOML tag.
    /// Used to create custom/vendor Usages.
    /// </summary>
    [TagAttribute(TagKind.Section, "usagePage", typeof(Dictionary<string, object>[]))]
    public class UsagePageSectionTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsagePageSectionTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private UsagePageSectionTag(IdTag id, NameTag name, List<UsageSectionTag> usages, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Id = id;
            this.PageName = name;
            this.Usages = usages;
        }

        /// <summary>
        /// Gets the Id of the UsagePage.
        /// </summary>
        public IdTag Id { get; }

        /// <summary>
        /// Gets the Name of the UsagePage.
        /// </summary>
        public NameTag PageName { get; }

        /// <summary>
        /// Gets the Usages belonging to this UsagePage.
        /// </summary>
        public List<UsageSectionTag> Usages { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="UsagePageSectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an UsagePageSection.</param>
        /// <returns>New <see cref="UsagePageSectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsagePageSectionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UsagePageSectionTag), rawTag))
            {
                return null;
            }

            IdTag id = null;
            NameTag name = null;
            List<UsageSectionTag> usages = new List<UsageSectionTag>();

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (id == null)
                {
                    id = IdTag.TryParse(child);

                    if (id != null)
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

                UsageSectionTag usage = UsageSectionTag.TryParse(child);
                if (usage != null)
                {
                    usages.Add(usage);
                    continue;
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (name == null && id == null)
            {
                throw new TomlGenericException(
                    Resources.ExceptionTomlMustSpecifyOneOfTwoKeys,
                    rawTag,
                    typeof(NameTag).GetCustomAttribute<TagAttribute>().Name,
                    typeof(IdTag).GetCustomAttribute<TagAttribute>().Name);
            }

            if (usages.Count == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUsagePageSectionMissingUsages, rawTag, typeof(UsageSectionTag).GetCustomAttribute<TagAttribute>().Name);
            }

            UsagePageSectionTag usagePageSection = new UsagePageSectionTag(id, name, usages, rawTag);
            usagePageSection.AddToGlobalUsagePageTable();

            return usagePageSection;
        }

        /// <summary>
        /// Adds the UsagePage (and it's Usages) to the Global HidUsagePage Tables.
        /// </summary>
        private void AddToGlobalUsagePageTable()
        {
            try
            {
                HidUsagePage page = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(this.Id?.ValueUInt16, this.PageName?.Value);

                foreach (UsageSectionTag usage in this.Usages)
                {
                    page.AddUsageId(usage.Value);
                }
            }
            catch (HidSpecificationException e)
            {
                throw new TomlGenericException(e, this);
            }
        }
    }
}
