// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.Properties;

    /// <summary>
    /// Represents a DescriptorName TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "cppDescriptorName", typeof(string))]
    public class CppDescriptorNameTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CppDescriptorNameTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private CppDescriptorNameTag(string value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="CppDescriptorNameTag"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="CppDescriptorNameTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="CppDescriptorNameTag"/>.</param>
        /// <returns>New <see cref="CppDescriptorNameTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static CppDescriptorNameTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(CppDescriptorNameTag), rawTag))
            {
                return null;
            }

            string descriptorName = (string)rawTag.Value;

            if (string.IsNullOrEmpty(descriptorName))
            {
                throw new TomlGenericException(Resources.ExceptionTomlCppNameEmpty, rawTag);
            }

            // Only letters, digits.
            string filteredName = string.Concat(descriptorName.Where(x => char.IsLetterOrDigit(x)));
            if (descriptorName != filteredName)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCppNameNonDigitsLetters, rawTag);
            }

            return new CppDescriptorNameTag(descriptorName, rawTag);
        }
    }
}
