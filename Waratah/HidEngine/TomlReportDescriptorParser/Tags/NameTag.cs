// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents a Name TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "name", typeof(string))]
    public class NameTag : BaseTag
    {
        private NameTag(string name, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = name;
        }

        /// <summary>
        /// Gets the value of this <see cref="NameTag"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="NameTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Name.</param>
        /// <returns>New <see cref="NameTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static NameTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(NameTag), rawTag))
            {
                return null;
            }

            string name = (string)rawTag.Value;

            if (string.IsNullOrEmpty(name))
            {
                throw new TomlGenericException(Resources.ExceptionTomlCppNameEmpty, rawTag);
            }

            // Only letters, digits, whitespace permitted.
            // ':', '-', '/', '\' permitted because Sensor's Usages use it...
            string filteredName = string.Concat(name.Where(x => (char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == ':' || x == '-' || x == '\\' || x == '/')));
            if (name != filteredName)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCppNameNonDigitsLetters, rawTag);
            }

            return new NameTag(name, rawTag);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value}";
        }
    }
}
