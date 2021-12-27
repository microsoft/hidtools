// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using System.Reflection;
    using HidEngine.Properties;
    using HidSpecification;

    /// <summary>
    /// Represents a Usage section TOML tag.
    /// Used to create custom/vendor Usages.
    /// </summary>
    [TagAttribute(TagKind.Section, "usage", typeof(Dictionary<string, object>[]))]
    public class UsageSectionTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageSectionTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private UsageSectionTag(IdTag id, NameTag name, UsageKindsTag kinds, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Id = id;
            this.UsageName = name;
            this.Kinds = kinds;

            this.Value = new HidUsageId(this.Id.ValueUInt16, this.UsageName.Value, this.Kinds.Value);
        }

        /// <summary>
        /// Gets the Id of the Usage.
        /// </summary>
        public IdTag Id { get; }

        /// <summary>
        /// Gets the Name of the Usage.
        /// </summary>
        public NameTag UsageName { get; }

        /// <summary>
        /// Gets Usage Kinds for this Usage.
        /// </summary>
        public UsageKindsTag Kinds { get; }

        /// <summary>
        /// Gets the cooked value of this <see cref="UsageSectionTag"/>.
        /// </summary>
        public HidUsageId Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="UsageSectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an UsageSectionTag.</param>
        /// <returns>New <see cref="UsageSectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageSectionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UsageSectionTag), rawTag))
            {
                return null;
            }

            IdTag id = null;
            NameTag name = null;
            UsageKindsTag kinds = null;

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (id == null)
                {
                    id = IdTag.TryParse(child);

                    if (id != null)
                    {
                        continue;
                    }
                }

                if (name == null)
                {
                    name = NameTag.TryParse(child);

                    if (name != null)
                    {
                        continue;
                    }
                }

                if (kinds == null)
                {
                    kinds = UsageKindsTag.TryParse(child);

                    if (kinds != null)
                    {
                        continue;
                    }
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (id == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlMissingRequiredKey, rawTag, typeof(IdTag).GetCustomAttribute<TagAttribute>().Name);
            }

            if (name == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlMissingRequiredKey, rawTag, typeof(NameTag).GetCustomAttribute<TagAttribute>().Name);
            }

            if (kinds == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlMissingRequiredKey, rawTag, typeof(UsageKindsTag).GetCustomAttribute<TagAttribute>().Name);
            }

            return new UsageSectionTag(id, name, kinds, rawTag);
        }
    }
}
