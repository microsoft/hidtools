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
    /// Represents a UsageUnitMultiplier TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "usageUnitMultiplier", typeof(double))]
    public class UsageUnitMultiplierTag : BaseTag
    {
        private UsageUnitMultiplierTag(double value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="UsageUnitMultiplierTag"/>.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageUnitMultiplierTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="UsageUnitMultiplierTag"/>.</param>
        /// <returns>New <see cref="UsageUnitMultiplierTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageUnitMultiplierTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UsageUnitMultiplierTag), rawTag))
            {
                return null;
            }

            double multiplier = (double)(rawTag.Value);
            if (multiplier == 0)
            {
                throw new TomlGenericException(HidEngine.Properties.Resources.ExceptionTomlCannotBeZero, rawTag);
            }

            try
            {
                HidConstants.MultiplierToWireCode(multiplier);
            }
            catch (HidSpecificationException e)
            {
                throw new TomlGenericException(e, rawTag);
            }

            return new UsageUnitMultiplierTag(multiplier, rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value}";
        }
    }
}
