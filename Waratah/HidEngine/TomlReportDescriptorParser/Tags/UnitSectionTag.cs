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
    /// Represents a Unit section TOML tag.
    /// </summary>
    [TagAttribute(TagKind.RootSection, "unit", typeof(Dictionary<string, object>[]))]
    public class UnitSectionTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSectionTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private UnitSectionTag(NameTag name, List<UnitDimensionTag> dimensionTags, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Name = name;
            this.Dimensions = dimensionTags;
        }

        /// <summary>
        /// Gets the name of this Unit.
        /// The uniqueness of this name will be verified internally.
        /// </summary>
        public NameTag Name { get; }

        /// <summary>
        /// Gets the dimensions of this Unit.
        /// These dimensions may reference previously defined Units.
        /// </summary>
        public List<UnitDimensionTag> Dimensions { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UnitSectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a UnitSectionTag.</param>
        /// <returns>New <see cref="UnitSectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UnitSectionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UnitSectionTag), rawTag))
            {
                return null;
            }

            NameTag name = null;
            List<UnitDimensionTag> dimensionTags = new List<UnitDimensionTag>();

            Dictionary<string, object> collectionChildren = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in collectionChildren)
            {
                if (name == null)
                {
                    name = NameTag.TryParse(child);
                    if (name != null)
                    {
                        continue;
                    }
                }

                UnitDimensionTag dimensionTag = UnitDimensionTag.TryParse(child);
                if (dimensionTag != null)
                {
                    // No need validate against duplicates, TOML parser prevents tags with same KeyName.
                    // Note: It's still possible for 'functionally' duplicate tags to still exist (e.g. centimeters and meters are duplicates of length).
                    dimensionTags.Add(dimensionTag);
                }
            }

            if (name == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlMissingRequiredKey, rawTag, typeof(NameTag).GetCustomAttribute<TagAttribute>().Name);
            }

            if (dimensionTags.Count == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnitsSectionMissingDimensions, rawTag, HidUnitDefinitions.GetInstance().DefinedUnitsNames());
            }

            // Validate supplied dimensions are 'real' and add them to Unit and cache.

            HidUnit newUnit = new HidUnit(name.Value);
            foreach (UnitDimensionTag dimension in dimensionTags)
            {
                HidUnit dimensionUnit = HidUnitDefinitions.GetInstance().TryFindUnitByName(dimension.NonDecoratedName);
                if (dimensionUnit == null)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnitSectionCannotFindDimension, rawTag, dimension.NonDecoratedName);
                }

                try
                {
                    newUnit.TryAddDimension(dimensionUnit, dimension.Multiplier, dimension.PowerExponent);
                }
                catch (HidSpecificationException e)
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUnitDimensionInvalid, dimension.RawTag, e.Message);
                }
            }

            try
            {
                HidUnitDefinitions.GetInstance().TryAddUnit(newUnit);
            }
            catch (HidSpecificationException e)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUnitInvalidExponent, rawTag, e.Message);
            }

            return new UnitSectionTag(name, dimensionTags, rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name.Value}";
        }
    }
}
