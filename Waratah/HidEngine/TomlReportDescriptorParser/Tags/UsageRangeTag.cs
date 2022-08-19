// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a UsageRange TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usageRange", typeof(object[]))]
    public class UsageRangeTag : BaseTag
    {
        private UsageRangeTag(HidUsageId usageStart, HidUsageId usageEnd, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.UsageStart = usageStart;

            this.UsageEnd = usageEnd;
        }

        /// <summary>
        /// Gets the start Usage of the range.
        /// </summary>
        public HidUsageId UsageStart { get; }

        /// <summary>
        /// Gets the end Usage of the range.
        /// </summary>
        public HidUsageId UsageEnd { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageRangeTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a UsageRange.</param>
        /// <returns>New <see cref="UsageRangeTag"/> or null if it cannot be parsed as one.</returns>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageRangeTag TryParse(KeyValuePair<string, object> rawTag, Type validationType = null)
        {
            if (!IsValidNameAndType(typeof(UsageRangeTag), rawTag))
            {
                return null;
            }

            object[] tagUsageObjArray = (object[])rawTag.Value;

            if (tagUsageObjArray.Length == 3)
            {
                object usagePageObj = tagUsageObjArray[0];
                object usageIdStartObj = tagUsageObjArray[1];
                object usageIdEndObj = tagUsageObjArray[2];

                HidUsageId usageIdStart = null;
                HidUsageId usageIdEnd = null;

                if (usagePageObj.GetType() == typeof(string) &&
                    usageIdStartObj.GetType() == typeof(string) &&
                    usageIdEndObj.GetType() == typeof(string))
                {
                    string usagePageName = (string)usagePageObj;
                    string usageStartName = (string)usageIdStartObj;
                    string usageEndName = (string)usageIdEndObj;

                    usageIdStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageStartName);
                    usageIdEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageEndName);
                }
                else if (
                    usagePageObj.GetType() == typeof(Int64) &&
                    usageIdStartObj.GetType() == typeof(Int64) &&
                    usageIdEndObj.GetType() == typeof(Int64))
                {
                    Int64 usagePage = (Int64)usagePageObj;
                    Int64 usageStartId = (Int64)usageIdStartObj;
                    Int64 usageEndId = (Int64)usageIdEndObj;

                    UInt16 usagePageUInt16 = Helpers.SafeGetUInt16(usagePage, rawTag);
                    UInt16 usageStartIdUInt16 = Helpers.SafeGetUInt16(usageStartId, rawTag);
                    UInt16 usageEndIdUInt16 = Helpers.SafeGetUInt16(usageEndId, rawTag);

                    usageIdStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageStartIdUInt16);
                    usageIdEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageEndIdUInt16);
                }
                else
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsageRangeInvalidDataType, rawTag);
                }

                if (usageIdStart == null)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnknownUsage, rawTag, usagePageObj, usageIdStartObj);
                }

                if (usageIdEnd == null)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnknownUsage, rawTag, usagePageObj, usageIdEndObj);
                }

                // TODO: Need to validate all the Usages in the range (not just the start/end).
                SupportedHidKindsAttribute.ValidateSupported(usageIdStart, rawTag, validationType);
                SupportedHidKindsAttribute.ValidateSupported(usageIdEnd, rawTag, validationType);

                return new UsageRangeTag(usageIdStart, usageIdEnd, rawTag);
            }

            throw new TomlGenericException(Resources.ExceptionTomlUsageRangeInvalidDataType, rawTag);
        }
    }
}
