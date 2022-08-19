// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidSpecification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidSpecification.Properties;

    /// <summary>
    /// A HID defined (or definable) Unit.
    /// Composed of multiple dimensions, with a Kind/Multipler/Exponent.
    /// Throughout, a float base-10 Multiplier has replaced the Unit-exponent value (for convenience).
    /// </summary>
    public class HidUnit
    {
        // Dimensions may have more than one applicable systemKind.
        // As dimensions are added to the unit, all non-overlapping kinds,
        // will be removed.
        private List<HidConstants.UnitItemSystemKind> systemKinds;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUnit"/> class.
        /// </summary>
        /// <param name="name">Name of the Unit.  Must be unique across all Units.</param>
        /// <param name="lengthExponent">Exponent of length.</param>
        /// <param name="massExponent">Exponent of mass.</param>
        /// <param name="timeExponent">Exponent of time.</param>
        /// <param name="temperatureExponent">Exponent of temperature.</param>
        /// <param name="currentExponent">Exponent of current.</param>
        /// <param name="luminousIntensityExponent">Exponent of luminous intensity.</param>
        /// <param name="multiplier">Multiplier to apply. Must of base-10. (e.g. 100, 0.001).</param>
        /// <param name="systemKinds">Kinds of system.</param>
        public HidUnit(
            string name,
            Int32? lengthExponent,
            Int32? massExponent,
            Int32? timeExponent,
            Int32? temperatureExponent,
            Int32? currentExponent,
            Int32? luminousIntensityExponent,
            double multiplier,
            HidConstants.UnitItemSystemKind[] systemKinds)
        {
            this.Name = name;

            this.LengthExponent = lengthExponent;
            this.MassExponent = massExponent;
            this.TimeExponent = timeExponent;
            this.TemperatureExponent = temperatureExponent;
            this.CurrentExponent = currentExponent;
            this.LuminousIntensityExponent = luminousIntensityExponent;

            this.Multiplier = multiplier;

            this.systemKinds = systemKinds?.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUnit"/> class.
        /// This is used to create the initial object, and then 'actually' initialized through calls to <see cref="TryAddDimension"/>.
        /// </summary>
        /// <param name="name">Name of the Unit.  Must be unique across all Units.</param>
        public HidUnit(string name)
            : this(name, null, null, null, null, null, null, 1, null)
        {
        }

        /// <summary>
        /// Gets the name of the Unit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the length exponent.
        /// </summary>
        public Int32? LengthExponent { get; private set; }

        /// <summary>
        /// Gets the mass exponent.
        /// </summary>
        public Int32? MassExponent { get; private set; }

        /// <summary>
        /// Gets the time exponent.
        /// </summary>
        public Int32? TimeExponent { get; private set; }

        /// <summary>
        /// Gets the temperature exponent.
        /// </summary>
        public Int32? TemperatureExponent { get; private set; }

        /// <summary>
        /// Gets the current exponent.
        /// </summary>
        public Int32? CurrentExponent { get; private set; }

        /// <summary>
        /// Gets the luminous intensity exponent.
        /// </summary>
        public Int32? LuminousIntensityExponent { get; private set; }

        /// <summary>
        /// Gets the multiple.  Will always be multiple of 10 (e.g. 1, 0.0001, 10000).
        /// </summary>
        public double Multiplier { get; private set; }

        /// <summary>
        /// Gets the kind of length.
        /// </summary>
        public HidConstants.UnitLengthKind LengthKind
        {
            get
            {
                if (this.LengthExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                            {
                                return HidConstants.UnitLengthKind.Centimeter;
                            }

                        case HidConstants.UnitItemSystemKind.SiRotation:
                            {
                                return HidConstants.UnitLengthKind.Radians;
                            }

                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                            {
                                return HidConstants.UnitLengthKind.Inch;
                            }

                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitLengthKind.Degrees;
                            }

                        default:
                            {
                                return HidConstants.UnitLengthKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitLengthKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the kind of mass.
        /// </summary>
        public HidConstants.UnitMassKind MassKind
        {
            get
            {
                if (this.MassExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                        case HidConstants.UnitItemSystemKind.SiRotation:
                            {
                                return HidConstants.UnitMassKind.Gram;
                            }

                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitMassKind.Slug;
                            }

                        default:
                            {
                                return HidConstants.UnitMassKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitMassKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the kind of time.
        /// </summary>
        public HidConstants.UnitTimeKind TimeKind
        {
            get
            {
                if (this.TimeExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                        case HidConstants.UnitItemSystemKind.SiRotation:
                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitTimeKind.Seconds;
                            }

                        default:
                            {
                                return HidConstants.UnitTimeKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitTimeKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the kind of temperature.
        /// </summary>
        public HidConstants.UnitTemperatureKind TemperatureKind
        {
            get
            {
                if (this.TemperatureExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                        case HidConstants.UnitItemSystemKind.SiRotation:
                            {
                                return HidConstants.UnitTemperatureKind.Kelvin;
                            }

                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitTemperatureKind.Fahrenheit;
                            }

                        default:
                            {
                                return HidConstants.UnitTemperatureKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitTemperatureKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the kind of current.
        /// </summary>
        public HidConstants.UnitCurrentKind CurrentKind
        {
            get
            {
                if (this.CurrentExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                        case HidConstants.UnitItemSystemKind.SiRotation:
                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitCurrentKind.Ampere;
                            }

                        default:
                            {
                                return HidConstants.UnitCurrentKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitCurrentKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the kind of luminous intensity.
        /// </summary>
        public HidConstants.UnitLuminousIntensityKind LuminousIntensityKind
        {
            get
            {
                if (this.LuminousIntensityExponent != 0)
                {
                    switch (this.SystemKind)
                    {
                        case HidConstants.UnitItemSystemKind.SiLinear:
                        case HidConstants.UnitItemSystemKind.SiRotation:
                        case HidConstants.UnitItemSystemKind.EnglishLinear:
                        case HidConstants.UnitItemSystemKind.EnglishRotation:
                            {
                                return HidConstants.UnitLuminousIntensityKind.Candela;
                            }

                        default:
                            {
                                return HidConstants.UnitLuminousIntensityKind.None;
                            }
                    }
                }
                else
                {
                    return HidConstants.UnitLuminousIntensityKind.None;
                }
            }
        }

        /// <summary>
        /// Gets the system applicable to this unit.
        /// </summary>
        public HidConstants.UnitItemSystemKind SystemKind
        {
            get
            {
                // A Unit can only ever be wire-encoded with a single system, we arbitarily always choose the first system.
                // Must always have at least 1 system.
                return (this.systemKinds == null || this.systemKinds?.Count == 0) ? HidConstants.UnitItemSystemKind.None : this.systemKinds[0];
            }
        }

        /// <summary>
        /// Validates that all members are within spec boundaries.
        /// This is not done as part of <see cref="TryAddDimension(HidUnit, double, int)"/>, as it's 'OK', for
        /// the Unit to be invalid intermediately (as dimensions are being added), so long as the final Unit is valid.
        /// </summary>
        /// <exception cref="HidSpecificationException">Multiplier or exponent invalid.</exception>
        public void Validate()
        {
            void ValidateExponent(Int32? exponent, string exponentName)
            {
                if (exponent.HasValue)
                {
                    try
                    {
                        HidConstants.ExponentToWireCode(exponent.Value);
                    }
                    catch (HidSpecificationException e)
                    {
                        throw new HidSpecificationException(Resources.ExceptionInvalidExponentWithDimensionName,  exponentName, e.Message);
                    }
                }
            }

            ValidateExponent(this.LengthExponent, this.LengthKind.ToString());
            ValidateExponent(this.MassExponent, this.MassKind.ToString());
            ValidateExponent(this.TimeExponent, this.TimeKind.ToString());
            ValidateExponent(this.TemperatureExponent, this.TemperatureKind.ToString());
            ValidateExponent(this.CurrentExponent, this.CurrentKind.ToString());
            ValidateExponent(this.LuminousIntensityExponent, this.LuminousIntensityKind.ToString());

            // Will throw if value invalid.
            HidConstants.MultiplierToWireCode(this.Multiplier);
        }

        /// <summary>
        /// Tries to add an existing Unit as a dimension to this Unit. (A dimension is a Unit+Multiple+Exponent Tuple)
        /// This is how more complicated (derived) Units get constructed.
        /// </summary>
        /// <param name="dimensionBaseUnit">Unit acting as the base of the dimension.</param>
        /// <param name="dimensionMultiplier">Multiple to apply to the base Unit.</param>
        /// <param name="dimensionPowerExponent">Exponent to apply to each exponent of the base Unit and Multiple.</param>
        public void TryAddDimension(HidUnit dimensionBaseUnit, double dimensionMultiplier, Int32 dimensionPowerExponent)
        {
            if (dimensionBaseUnit == null)
            {
                throw new HidSpecificationException(Resources.ExceptionInvalidDimensionBaseUnitCannotBeNull);
            }

            if (!dimensionMultiplier.IsTenMultiplier())
            {
                throw new HidSpecificationException(Resources.ExceptionMultiplierNotBased10, dimensionMultiplier);
            }

            if (dimensionPowerExponent == 0)
            {
                throw new HidSpecificationException(Resources.ExceptionInvalidDimensionExponentIsZero);
            }

            this.LengthExponent = CalculateDimensionProperty(this.LengthExponent, dimensionBaseUnit.LengthExponent, dimensionPowerExponent);

            this.MassExponent = CalculateDimensionProperty(this.MassExponent, dimensionBaseUnit.MassExponent, dimensionPowerExponent);

            this.TimeExponent = CalculateDimensionProperty(this.TimeExponent, dimensionBaseUnit.TimeExponent, dimensionPowerExponent);

            this.TemperatureExponent = CalculateDimensionProperty(this.TemperatureExponent, dimensionBaseUnit.TemperatureExponent, dimensionPowerExponent);

            this.CurrentExponent = CalculateDimensionProperty(this.CurrentExponent, dimensionBaseUnit.CurrentExponent, dimensionPowerExponent);

            this.LuminousIntensityExponent = CalculateDimensionProperty(this.LuminousIntensityExponent, dimensionBaseUnit.LuminousIntensityExponent, dimensionPowerExponent);

            this.Multiplier *= (Math.Pow(dimensionBaseUnit.Multiplier, Math.Abs(dimensionPowerExponent)) * dimensionMultiplier);

            if (this.systemKinds == null)
            {
                // First Unit added, so all systems are valid.
                this.systemKinds = dimensionBaseUnit.systemKinds;
            }
            else
            {
                // Combining two Units (to create a different Unit), must take only the systems common to both Units.
                this.systemKinds = this.systemKinds.Where(x => dimensionBaseUnit.systemKinds.Contains(x)).ToList();
            }

            bool isAnExponentValid = (this.LengthExponent.HasValue || this.MassExponent.HasValue || this.TimeExponent.HasValue || this.TemperatureExponent.HasValue || this.CurrentExponent.HasValue || this.LuminousIntensityExponent.HasValue);
            if (!isAnExponentValid)
            {
                throw new HidSpecificationException(Resources.ExceptionInvalidDimensionNoValidExponent, dimensionBaseUnit);
            }

            bool isSystemKindsValid = (this.systemKinds.Count != 0);
            if (!isSystemKindsValid)
            {
                throw new HidSpecificationException(Resources.ExceptionInvalidDimensionNoUnitSystemOverlap, dimensionBaseUnit);
            }
        }

        /// <summary>
        /// Compares this HidUnit to another HidUnit.
        /// </summary>
        /// <param name="compareUnit">HidUnit to compare against.</param>
        /// <returns>True if HidUnit functionally identically to this.</returns>
        public bool Equals(HidUnit compareUnit)
        {
            if (compareUnit == null)
            {
                return false;
            }

            bool isExponentsEqual =
                ((this.LengthExponent == compareUnit.LengthExponent) &&
                (this.MassExponent == compareUnit.MassExponent) &&
                (this.TimeExponent == compareUnit.TimeExponent) &&
                (this.TemperatureExponent == compareUnit.TemperatureExponent) &&
                (this.CurrentExponent == compareUnit.CurrentExponent) &&
                (this.LuminousIntensityExponent == compareUnit.LuminousIntensityExponent));

            bool isMultipliersEqual = (this.Multiplier == compareUnit.Multiplier);

            bool isSystemKindsEqual = (this.SystemKind == compareUnit.SystemKind);

            // Is OK if names are different.
            return (isExponentsEqual && isMultipliersEqual && isSystemKindsEqual);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string FormatExponent(int exponent, string kind)
            {
                return (exponent != 0 ? $"{kind}:{exponent}" : string.Empty);
            }

            if (this.SystemKind == HidConstants.UnitItemSystemKind.None)
            {
                return this.SystemKind.ToString();
            }
            else
            {
                string lengthExponent = FormatExponent(this.LengthExponent ?? 0, this.LengthKind.ToString());
                string massExponent = FormatExponent(this.MassExponent ?? 0, this.MassKind.ToString());
                string timeExponent = FormatExponent(this.TimeExponent ?? 0, this.TimeKind.ToString());
                string tempExponent = FormatExponent(this.TemperatureExponent ?? 0, this.TemperatureKind.ToString());
                string currentExponent = FormatExponent(this.CurrentExponent ?? 0, this.CurrentKind.ToString());
                string luminousExponent = FormatExponent(this.LuminousIntensityExponent ?? 0, this.LuminousIntensityKind.ToString());

                string[] exponents = new string[] { this.SystemKind.ToString(), lengthExponent, massExponent, timeExponent, tempExponent, currentExponent, luminousExponent };

                string combinedExponents = String.Join(", ", exponents.Where(x => x.Length != 0));

                string formattedExponents = $"\'{this.Name}\', {combinedExponents}";

                return formattedExponents;
            }
        }

        private static Int32? CalculateDimensionProperty(Int32? unitExponent, Int32? dimensionBaseUnitExponent, Int32 dimensionPowerExponent)
        {
            if (dimensionBaseUnitExponent.HasValue)
            {
                if (!unitExponent.HasValue)
                {
                    unitExponent = 0;
                }

                unitExponent += (dimensionBaseUnitExponent * dimensionPowerExponent);
            }

            return unitExponent;
        }
    }
}
