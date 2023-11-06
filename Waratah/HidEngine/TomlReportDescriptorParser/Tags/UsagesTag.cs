// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a Usages TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usages", typeof(object[]))]
    public class UsagesTag : BaseTag
    {
        private UsagesTag(List<HidUsageId> usages, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Usages = usages;
        }

        /// <summary>
        /// Gets the list of Usages.
        /// </summary>
        public List<HidUsageId> Usages { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsagesTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a list of Usages.</param>
        /// <returns>New <see cref="UsagesTag"/> or null if it cannot be parsed as one.</returns>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsagesTag TryParse(KeyValuePair<string, object> rawTag, Type validationType = null)
        {
            if (!IsValidNameAndType(typeof(UsagesTag), rawTag))
            {
                return null;
            }

            object[] tagUsageObjArray = (object[])rawTag.Value;

            List<HidUsageId> usages = new List<HidUsageId>();

            foreach (object tagUsageObj in tagUsageObjArray)
            {
                // Validate all entries are expected type.
                if (tagUsageObj.GetType() != typeof(object[]))
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsagesTagInvalidDataType, rawTag);
                }

                object[] usageObj = tagUsageObj as object[];

                if (usageObj.Length == 2)
                {
                    object usagePageObj = usageObj[0];
                    object usageIdObj = usageObj[1];

                    HidUsageId usage = null;

                    if (usagePageObj.GetType() == typeof(string) && usageIdObj.GetType() == typeof(string))
                    {
                        string usagePageName = (string)usagePageObj;
                        string usageIdName = (string)usageIdObj;

                        usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageIdName);
                    }
                    else if (usagePageObj.GetType() == typeof(Int64) && usageIdObj.GetType() == typeof(Int64))
                    {
                        Int64 usagePage = (Int64)usagePageObj;
                        Int64 usageId = (Int64)usageIdObj;

                        UInt16 usagePageUInt16 = Helpers.SafeGetUInt16(usagePage, rawTag);
                        UInt16 usageIdUInt16 = Helpers.SafeGetUInt16(usageId, rawTag);

                        usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageIdUInt16);
                    }
                    else
                    {
                        throw new TomlGenericException(Resources.ExceptionTomlUsagesTagInvalidDataType, rawTag);
                    }

                    SupportedHidKindsAttribute.ValidateSupported(usage, rawTag, validationType);

                    usages.Add(usage);
                }
                else
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsagesTagInvalidDataType, rawTag);
                }
            }

            if (usages.Count == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUsagesTagInvalidDataType, rawTag);
            }

            return new UsagesTag(usages, rawTag);
        }
    }
}
