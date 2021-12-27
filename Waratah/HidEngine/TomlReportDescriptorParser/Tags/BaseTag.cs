// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using HidEngine.Properties;

    /// <summary>
    /// Base class all Tags MUST inherit from.
    /// A Tag is any TOML section (e.g. applicationCollection) or key/value pair (e.g. logicalValueRange).
    /// </summary>
    public abstract class BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTag"/> class.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Tag.</param>
        public BaseTag(KeyValuePair<string, object> rawTag)
        {
            this.KeyName = rawTag.Key;
            this.LineNumber = TagFinder.GetInstance().FindLine(rawTag);
            this.RawTag = rawTag;
        }

        /// <summary>
        /// Gets the parsed section-name/key-name of the tag.
        /// </summary>
        /// <example>applicationCollection, size.</example>
        public string KeyName { get; }

        /// <summary>
        /// Gets the parsed section-name/key-name of the tag.
        /// </summary>
        /// <example>applicationCollection, size.</example>
        public string NonDecoratedName
        {
            get
            {
                return TagDecorator.UnDecorateTag(this.KeyName);
            }
        }

        /// <summary>
        /// Gets the rawTag for this tag.
        /// </summary>
        public KeyValuePair<string, object> RawTag { get; }

        /// <summary>
        /// Gets the line the tag appears in the TOML file.
        /// This is useful when debugging or displaying errors to the user.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Validates if the provided tag is expected for the Type.
        /// </summary>
        /// <param name="tagClassType">Type.</param>
        /// <param name="rawTag">TOML Tag in raw form.</param>
        /// <returns>True if the tag name is valid and type is expected.</returns>
        public static bool IsValidNameAndType(Type tagClassType, KeyValuePair<string, object> rawTag)
        {
            TagAttribute attribute = tagClassType.GetCustomAttribute<TagAttribute>();
            if (attribute != null)
            {
                if (TagDecorator.UnDecorateTag(rawTag).Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var attributeType in attribute.TomlTypes)
                    {
                        if (rawTag.Value.GetType() == attributeType)
                        {
                            // Additional verification here, as we know that if the TOML parser
                            // returns this type, is MUST have a size of 1.
                            if (attributeType == typeof(Dictionary<string, object>[]))
                            {
                                Dictionary<string, object>[] tomlDictionary = (Dictionary<string, object>[])rawTag.Value;

                                // Should always be 1 element if type is Dictionary.
                                if (tomlDictionary.Length != 1)
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    }

                    // A tag with this name exists, but the type is wrong.
                    // No two tags share the same name but have different sets of types, fatal error.

                    if (attribute.Kind == TagKind.RootSection)
                    {
                        throw new TomlGenericException(Resources.ExceptionTomlInvalidTypeForTopLevelSectionTag, rawTag, rawTag.Key);
                    }
                    else
                    {
                        string validTypes = string.Join(",", attribute.TomlTypes.Select(x => x.Name));
                        throw new TomlGenericException(Resources.ExceptionTomlInvalidTypeForTag, rawTag, validTypes);
                    }
                }
            }

            return false;
        }
    }
}