// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;

    /// <summary>
    /// Represents a DescriptorVariableModifier TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "cppDescriptorVariableModifier", typeof(string))]
    public class CppDescriptorVariableModifierTag : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CppDescriptorVariableModifierTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private CppDescriptorVariableModifierTag(string value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="CppDescriptorVariableModifierTag"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="CppDescriptorVariableModifierTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an <see cref="CppDescriptorVariableModifierTag"/>.</param>
        /// <returns>New <see cref="CppDescriptorVariableModifierTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static CppDescriptorVariableModifierTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(CppDescriptorVariableModifierTag), rawTag))
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

            return new CppDescriptorVariableModifierTag(descriptorName, rawTag);
        }
    }
}
