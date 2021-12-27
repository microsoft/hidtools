// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.Properties;
    using HidSpecification;

    /// <summary>
    /// Represents a UsageTransform TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usageTransform", typeof(object[]))]
    public class UsageTransformTag : BaseTag
    {
        private UsageTransformTag(HidUsageId usage, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = usage;
        }

        /// <summary>
        /// Gets the value of this Tag.
        /// </summary>
        public HidUsageId Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageTransformTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a UsageTransform.</param>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        /// <returns>New <see cref="UsageTransformTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageTransformTag TryParse(KeyValuePair<string, object> rawTag, Type validationType = null)
        {
            if (!IsValidNameAndType(typeof(UsageTransformTag), rawTag))
            {
                return null;
            }

            object[] tagUsageObjArray = (object[])rawTag.Value;
            if (tagUsageObjArray.Length != 3)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUsageTransformInvalidDataType, rawTag);
            }

            object usagePageObj = tagUsageObjArray[0];
            object usageIdObj = tagUsageObjArray[1];
            object usageIdTransformObj = tagUsageObjArray[2];

            HidUsageId usage = null;
            HidUsageId usageTransform = null;

            if (usagePageObj.GetType() == typeof(string) &&
                usageIdObj.GetType() == typeof(string) &&
                usageIdTransformObj.GetType() == typeof(string))
            {
                string usagePageName = (string)usagePageObj;
                string usageIdName = (string)usageIdObj;
                string usageIdTransformName = (string)usageIdTransformObj;

                // By definition, the UsagePage of the transform UsageId must be the same as the regular Usage.
                usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageIdName);
                usageTransform = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageName, usageIdTransformName);
            }
            else if (
                usagePageObj.GetType() == typeof(Int64) &&
                usageIdObj.GetType() == typeof(Int64) &&
                usageIdTransformObj.GetType() == typeof(Int64))
            {
                Int64 usagePage = (Int64)usagePageObj;
                Int64 usageId = (Int64)usageIdObj;
                Int64 usageIdTransform = (Int64)usageIdTransformObj;

                UInt16 usagePageUInt16 = Helpers.SafeGetUInt16(usagePage, rawTag);
                UInt16 usageIdUInt16 = Helpers.SafeGetUInt16(usageId, rawTag);
                UInt16 usageIdTransformUInt16 = Helpers.SafeGetUInt16(usageIdTransform, rawTag);

                // By definition, the UsagePage of the transform UsageId must be the same as the regular Usage.
                usage = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageIdUInt16);
                usageTransform = HidUsageTableDefinitions.GetInstance().TryFindUsageId(usagePageUInt16, usageIdTransformUInt16);
            }
            else
            {
                throw new TomlGenericException(Resources.ExceptionTomlUsageTransformInvalidDataType, rawTag);
            }

            if (usage == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnknownUsage, rawTag, usagePageObj, usageIdObj);
            }

            if (usageTransform == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnknownUsage, rawTag, usagePageObj, usageIdTransformObj);
            }

            HidUsageId usageTransformed = usage.Transform(usageTransform);

            SupportedHidKindsAttribute.ValidateSupported(usageTransformed, rawTag, validationType);

            return new UsageTransformTag(usageTransformed, rawTag);
        }
    }
}
