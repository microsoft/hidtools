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
    /// Represents a Usage TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usage", typeof(object[]))]
    public class UsageTag : BaseTag
    {
        private UsageTag(HidUsageId usage, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = usage;
        }

        /// <summary>
        /// Gets the value of this Tag.
        /// </summary>
        public HidUsageId Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Usage.</param>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        /// <returns>New <see cref="UsageTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageTag TryParse(KeyValuePair<string, object> rawTag, Type validationType = null)
        {
            if (!IsValidNameAndType(typeof(UsageTag), rawTag))
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

                SupportedHidKindsAttribute.ValidateSupported(usage, rawTag, validationType);

                return new UsageTag(usage, rawTag);
            }

            throw new TomlGenericException(Resources.ExceptionTomlUsageTagInvalid, rawTag);
        }
    }
}
