// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents an ApplicationCollection TOML tag.
    /// Every ApplicationCollection must be associated with a single Usage and 1 or more Reports.
    /// </summary>
    [TagAttribute(TagKind.Section, "paddingItem", typeof(Dictionary<string, object>[]))]
    public class PaddingItemTag : BaseTag, IModuleGeneratorTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaddingItemTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        /// <param name="size">Size of this PaddingItem.</param>
        /// <param name="rawTag">Root TOML element describing an PaddingItem.</param>
        private PaddingItemTag(SizeInBitsTag size, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Size = size;
        }

        /// <summary>
        /// Gets the size of this PaddingItem.
        /// </summary>
        public SizeInBitsTag Size { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="PaddingItemTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an PaddingItem.</param>
        /// <returns>New <see cref="PaddingItemTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static PaddingItemTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(PaddingItemTag), rawTag))
            {
                return null;
            }

            SizeInBitsTag size = null;

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (size == null)
                {
                    size = SizeInBitsTag.TryParse(child);

                    if (size != null)
                    {
                        continue;
                    }
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (size == null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlMissingRequiredKey, rawTag, typeof(SizeInBitsTag).GetCustomAttribute<TagAttribute>().Name);
            }

            return new PaddingItemTag(size, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            return new PaddingModule(this.Size.Value, parent);
        }
    }
}