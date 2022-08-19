// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents a Usage Kinds TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "types", typeof(object[]))]
    public class UsageKindsTag : BaseTag
    {
        private UsageKindsTag(List<HidUsageKind> kinds, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = kinds;
        }

        /// <summary>
        /// Gets the value of this <see cref="UsageKindsTag"/>.
        /// Most Usages will only have a single kind.
        /// </summary>
        public List<HidUsageKind> Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="UsageKindsTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Usage Kinds.</param>
        /// <returns>New <see cref="UsageKindsTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static UsageKindsTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(UsageKindsTag), rawTag))
            {
                return null;
            }

            object[] tagKinds = (object[])rawTag.Value;

            if (tagKinds.Length == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlUsageKindsEmpty, rawTag, string.Join(", ", Enum.GetNames(typeof(HidUsageKind))));
            }

            List<HidUsageKind> kinds = new List<HidUsageKind>();
            foreach (string tagKind in tagKinds)
            {
                if (Enum.TryParse(tagKind, out HidUsageKind kind) && Enum.IsDefined(typeof(HidUsageKind), kind))
                {
                    if (kinds.Contains(kind))
                    {
                        throw new TomlGenericException(Resources.ExceptionTomlUsageKindsDuplicate, rawTag, kind);
                    }

                    kinds.Add(kind);
                }
                else
                {
                    throw new TomlGenericException(Resources.ExceptionTomlUsageKindInvalid, rawTag, tagKind, string.Join(", ", Enum.GetNames(typeof(HidUsageKind))));
                }
            }

            return new UsageKindsTag(kinds, rawTag);
        }
    }
}
