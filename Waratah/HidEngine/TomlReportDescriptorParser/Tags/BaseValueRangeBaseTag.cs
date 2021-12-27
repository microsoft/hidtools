// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Base class all ValueRange Tags MUST inherit from.
    /// </summary>
    public class BaseValueRangeBaseTag : BaseTag
    {
        /// <summary>
        /// Keyword for MaxSignedSizeRange.
        /// </summary>
        public static readonly string MaxSignedSizeRangeKeyword = "maxSignedSizeRange";

        /// <summary>
        /// Keyword for MaxUnsignedSizeRange.
        /// </summary>
        public static readonly string MaxUnsignedSizeRangeKeyword = "maxUnsignedSizeRange";

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseValueRangeBaseTag"/> class.
        /// </summary>
        /// <param name="value">The value of this range.</param>
        /// <param name="rawTag">Root TOML element describing a ValueRange.</param>>
        protected BaseValueRangeBaseTag(DescriptorRange value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="DescriptorRange"/>.
        /// </summary>
        public DescriptorRange Value { get; }

        /// <summary>
        /// Attempts to parse the ValueRange Tag into a type and range.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a ValueRange.</param>
        /// <param name="permitMaxRange">Whether use of the maxSignedSizeRange/maxUnsignedSizeRange is permitted.</param>
        /// <returns>Type and Range as a tuple.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are out of bounds.</exception>
        protected static DescriptorRange TryParseInternal(KeyValuePair<string, object> rawTag, bool permitMaxRange = true)
        {
            if (rawTag.Value.GetType() == typeof(object[]))
            {
                object[] tagArray = (object[])rawTag.Value;

                if (tagArray.Length == 2 &&
                    tagArray[0].GetType() == typeof(Int64) &&
                    tagArray[1].GetType() == typeof(Int64))
                {
                    Int64 minimum = (Int64)tagArray[0];
                    Int64 maximum = (Int64)tagArray[1];

                    int minimumInt32 = Helpers.SafeGetInt32(minimum, rawTag);
                    int maximumInt32 = Helpers.SafeGetInt32(maximum, rawTag);

                    return new DescriptorRange(minimumInt32, maximumInt32);
                }
            }
            else if (rawTag.Value.GetType() == typeof(string))
            {
                string rawTagValueString = (string)rawTag.Value;

                if (rawTagValueString.Equals(MaxSignedSizeRangeKeyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    return permitMaxRange ? new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange) : throw new TomlGenericException(Resources.ExceptionTomlValueRangeCannotUseMaxRange, rawTag, MaxSignedSizeRangeKeyword);
                }
                else if (rawTagValueString.Equals(MaxUnsignedSizeRangeKeyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    return permitMaxRange ? new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange) : throw new TomlGenericException(Resources.ExceptionTomlValueRangeCannotUseMaxRange, rawTag, MaxUnsignedSizeRangeKeyword);
                }
            }

            throw new TomlGenericException(Resources.ExceptionTomlValueRangeInvalidDataType, rawTag, MaxSignedSizeRangeKeyword, MaxUnsignedSizeRangeKeyword);
        }
    }
}
