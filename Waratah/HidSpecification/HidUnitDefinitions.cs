// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidSpecification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidSpecification.Properties;

    /// <summary>
    /// Container to store all Units (base-units/predefined and user-created).
    /// </summary>
    public class HidUnitDefinitions
    {
        private static HidUnitDefinitions instance = null;

        private List<HidUnit> units;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUnitDefinitions"/> class.
        /// </summary>
        private HidUnitDefinitions()
        {
            this.units = new List<HidUnit>();

            this.AddSiBaseUnits();
        }

        /// <summary>
        /// Gets the name assigned to the none Unit.
        /// </summary>
        public static string NoneName
        {
            get { return "none"; }
        }

        /// <summary>
        /// Lazy accessor for singleton instance of definitions.  Initializes on first invocation.
        /// </summary>
        /// <param name="clear">When true, clears all non-default Units.  Useful for unit-tests.</param>
        /// <returns>Single instance.</returns>
        public static HidUnitDefinitions GetInstance(bool clear = false)
        {
            if (instance == null || clear)
            {
                instance = new HidUnitDefinitions();
            }

            return instance;
        }

        /// <summary>
        /// Formatted string of all currently available units (names).
        /// </summary>
        /// <returns>List of available units.</returns>
        public string DefinedUnitsNames()
        {
            return string.Join(", ", this.units.Select(x => x.Name));
        }

        /// <summary>
        /// Tries to find the Unit by name.  Name is guaranteed unique across all Units.
        /// </summary>
        /// <param name="name">Name of the Unit to find.</param>
        /// <returns>The found Unit, or null if a Unit with the name cannot be found.</returns>
        public HidUnit TryFindUnitByName(string name)
        {
            return this.units.Find(x => x.Name == name);
        }

        /// <summary>
        /// Tries to add the Unit to the Global collection.
        /// </summary>
        /// <param name="unit">Unit to add.</param>
        /// <exception cref="Exception">Thrown when the Unit cannot be added.</exception>
        public void TryAddUnit(HidUnit unit)
        {
            // Validate Unit (internally).
            unit.Validate();

            // Validate Unit against all other units.
            if (this.TryFindUnitByName(unit.Name) != null)
            {
                // Cannot duplicate another Unit's name.  This is the unique identifier.
                throw new HidSpecificationException(Resources.ExceptionDuplicatedUnitName, unit.Name);
            }

            this.units.Add(unit);
        }

        /// <summary>
        /// Predefined/Base-Units from which all other Units must derive.
        /// </summary>
        private void AddSiBaseUnits()
        {
            this.TryAddUnit(new HidUnit(NoneName, null, null, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.None }));

            // Length
            this.TryAddUnit(new HidUnit("centimeter", 1, null, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear }));
            this.TryAddUnit(new HidUnit("inch", 1, null, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.EnglishLinear }));
            this.TryAddUnit(new HidUnit("radians", 1, null, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.SiRotation }));
            this.TryAddUnit(new HidUnit("degrees", 1, null, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.EnglishRotation }));

            // Mass
            this.TryAddUnit(new HidUnit("gram", null, 1, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear, HidConstants.UnitItemSystemKind.SiRotation }));
            this.TryAddUnit(new HidUnit("slug", null, 1, null, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.EnglishLinear, HidConstants.UnitItemSystemKind.EnglishRotation }));

            // Time
            this.TryAddUnit(new HidUnit("second", null, null, 1, null, null, null, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear, HidConstants.UnitItemSystemKind.SiRotation, HidConstants.UnitItemSystemKind.EnglishLinear, HidConstants.UnitItemSystemKind.EnglishRotation }));

            // Temperature
            this.TryAddUnit(new HidUnit("kelvin", null, null, null, 1, null, null, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear, HidConstants.UnitItemSystemKind.SiRotation }));
            this.TryAddUnit(new HidUnit("fahrenheit", null, null, null, 1, null, null, 1, new[] { HidConstants.UnitItemSystemKind.EnglishLinear, HidConstants.UnitItemSystemKind.EnglishRotation }));

            // Current
            this.TryAddUnit(new HidUnit("ampere", null, null, null, null, 1, null, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear, HidConstants.UnitItemSystemKind.SiRotation, HidConstants.UnitItemSystemKind.EnglishLinear, HidConstants.UnitItemSystemKind.EnglishRotation }));

            // Luminous Intensity
            this.TryAddUnit(new HidUnit("candela", null, null, null, null, null, 1, 1, new[] { HidConstants.UnitItemSystemKind.SiLinear, HidConstants.UnitItemSystemKind.SiRotation, HidConstants.UnitItemSystemKind.EnglishLinear, HidConstants.UnitItemSystemKind.EnglishRotation }));
        }
    }
}
