// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a Unit TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "unit", typeof(string))]
    public class UnitTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private UnitTag(HidUnit unit, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = unit;
        }

        /// <summary>
        /// Gets the value of this Tag.
        /// </summary>
        public HidUnit Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UnitTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a UnitTag.</param>
        /// <returns>New <see cref="UnitTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UnitTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UnitTag), rawTag))
            {
                return null;
            }

            string unitName = (string)rawTag.Value;

            HidUnit unit = HidUnitDefinitions.GetInstance().TryFindUnitByName(unitName);
            if (unit == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnitNameNotFound, rawTag, unitName);
            }

            return new UnitTag(unit, rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value}";
        }
    }
}
