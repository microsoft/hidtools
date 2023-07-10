// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a UsageRelation TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usageRelation", typeof(object[]))]
    public class UsageRelationTag : BaseTag
    {
        private UsageRelationTag(HidUsageId usage, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = usage;
        }

        /// <summary>
        /// Gets the value of this Tag.
        /// </summary>
        public HidUsageId Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageRelationTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Usage.</param>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        /// <returns>New <see cref="UsageRelationTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageRelationTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UsageRelationTag), rawTag))
            {
                return null;
            }

            object[] tagUsageObjArray = (object[])rawTag.Value;

            if (tagUsageObjArray.Length == 2)
            {
                object usagePageObj = tagUsageObjArray[0];
                object usageIdObj = tagUsageObjArray[1];

                HidUsageId usage = null;

                if (usagePageObj.GetType() == typeof(string) &&
                    usageIdObj.GetType() == typeof(string))
                {
                    string usagePageName = (string)usagePageObj;
                    string usageIdName = (string)usageIdObj;

                    usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageIdName);
                }
                else if (
                    usagePageObj.GetType() == typeof(Int64) &&
                    usageIdObj.GetType() == typeof(Int64))
                {
                    Int64 usagePage = (Int64)usagePageObj;
                    Int64 usageId = (Int64)usageIdObj;

                    UInt16 usagePageUInt16 = Helpers.SafeGetUInt16(usagePage, rawTag);
                    UInt16 usageIdUInt16 = Helpers.SafeGetUInt16(usageId, rawTag);

                    usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageIdUInt16);
                }
                else
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsageTagInvalid, rawTag);
                }

                if (usage == null)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnknownUsage, rawTag, usagePageObj, usageIdObj);
                }

                SupportedHidKindsAttribute.ValidateSupported(usage, rawTag, typeof(PhysicalCollectionTag));

                // UsageRelations are a 'hack' to allow Sensors devices to coalesce multiple logical devices under a single ApplicationCollection
                // This is a bad pattern, but has been in place since 2014, to must support this.
                // Restrict this hack to new devices by only permitting Sensors Page Usages.
                if (usage.Page.Name != "Sensors")
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsageSectionTagInvalid, rawTag, usage.Page);
                }

                return new UsageRelationTag(usage, rawTag);
            }

            throw new TomlGenericException(Resources.ExceptionTomlUsageTagInvalid, rawTag);
        }
    }
}
