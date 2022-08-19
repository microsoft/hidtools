// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a Units Dimension TOML tag.
    /// </summary>
    public class UnitDimensionTag : BaseTag
    {
        private UnitDimensionTag(double multiplier, int powerExponent, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Multiplier = multiplier;
            this.PowerExponent = powerExponent;
        }

        /// <summary>
        /// Gets the Multipler exponent (base-10) for this dimension.
        /// This will be factored into the calculation of UnitExponent(), and not a nibble
        /// in Unit().
        /// This exponent has not been validated against the permissible expressive range of <see cref="HidConstants.ExponentMaximum"/> <see cref="HidConstants.ExponentMinimum"/>.
        /// </summary>
        public double Multiplier { get; }

        /// <summary>
        /// Gets the Power exponent for this dimension.
        /// This will be factored into the calculation of the Unit() nibble.
        /// </summary>
        public int PowerExponent { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UnitDimensionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a <see cref="UnitDimensionTag"/>.</param>
        /// <returns>New <see cref="UnitDimensionTag"/> or throws exception.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UnitDimensionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            // All dimension tags must be an array of 2 doubles.
            if (rawTag.Value.GetType() != typeof(object[]))
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnitDimensionArrayDimension, rawTag);
            }

            object[] tagArray = (object[])rawTag.Value;

            if (tagArray.Length == 2 &&
                tagArray[0].GetType() == typeof(double) &&
                tagArray[1].GetType() == typeof(double))
            {
                double multiplier = (double)tagArray[0];
                double power = (double)tagArray[1];

                // Validate power.
                if ((Int32)power != power)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnitDimensionPowerFieldFractional, rawTag, power);
                }

                return new UnitDimensionTag(multiplier, (Int32)power, rawTag);
            }

            throw new TomlGenericException(Resources.ExceptionTomlUnitDimensionArrayDimension, rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.NonDecoratedName} [{this.Multiplier}, {this.PowerExponent}]";
        }
    }
}
