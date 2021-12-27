// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorComposition;

    /// <summary>
    /// Represents a Report Flags TOML Tag.
    /// </summary>
    [TagAttribute(TagKind.Key, "reportFlags", typeof(object[]))]
    public class ReportFlagsTag : BaseTag
    {
        // Caching flag properties as reflection is expensive.
        private static List<PropertyInfo> reportFlagProperties;

        // Caching flag property attribute names as reflection is expensive.
        private static List<string> reportFlagPropertiesNames;

        static ReportFlagsTag()
        {
            IEnumerable<PropertyInfo> properties = typeof(DescriptorModuleFlags).GetProperties();

            reportFlagProperties = new List<PropertyInfo>();
            reportFlagPropertiesNames = new List<string>();

            foreach (PropertyInfo reportFlagProperty in properties)
            {
                /// We know from the definition of the <see cref="DescriptorModuleFlags"/>, that properties
                /// of interest are all the Nullable ones.
                Type propertyType = Nullable.GetUnderlyingType(reportFlagProperty.PropertyType);
                if (propertyType != null)
                {
                    // Enums will only ever have 2 values.
                    reportFlagPropertiesNames.Add(propertyType.GetEnumValues().GetValue(0).ToString());
                    reportFlagPropertiesNames.Add(propertyType.GetEnumValues().GetValue(1).ToString());

                    reportFlagProperties.Add(reportFlagProperty);
                }
            }
        }

        private ReportFlagsTag(DescriptorModuleFlags value, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this <see cref="ReportFlagsTag"/>.
        /// </summary>
        public DescriptorModuleFlags Value { get; }

        /// <summary>
        /// Attempts to parse the given tag as a <see cref="ReportFlagsTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a <see cref="ReportFlagsTag"/>.</param>
        /// <returns>New <see cref="ReportFlagsTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static ReportFlagsTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(ReportFlagsTag), rawTag))
            {
                return null;
            }

            object[] tagReportFlags = (object[])rawTag.Value;

            if (tagReportFlags.Length == 0)
            {
                throw new TomlGenericException(Resources.ExceptionTomlReportFlagsCannotBeEmpty, rawTag, string.Join(", ", reportFlagPropertiesNames));
            }

            // Validate every flag is valid.
            foreach (string tagReportFlag in tagReportFlags)
            {
                if (!reportFlagPropertiesNames.Contains(tagReportFlag, StringComparer.InvariantCultureIgnoreCase))
                {
                    throw new TomlGenericException(Resources.ExceptionTomlInvalidReportFlag, rawTag, tagReportFlag, string.Join(", ", reportFlagPropertiesNames));
                }
            }

            DescriptorModuleFlags reportFlagsModule = ExtractFlagsFromTag(rawTag, tagReportFlags);

            return new ReportFlagsTag(reportFlagsModule, rawTag);
        }

        private static DescriptorModuleFlags ExtractFlagsFromTag(KeyValuePair<string, object> rawTag, object[] tagReportFlags)
        {
            DescriptorModuleFlags reportFlagsModule = new DescriptorModuleFlags();

            foreach (PropertyInfo reportFlagProperty in reportFlagProperties)
            {
                bool previouslyFoundMatchingProperty = false;
                string previouslyFoundFlag = string.Empty;
                bool previouslyFoundIsDefault = false;

                foreach (string tagReportFlag in tagReportFlags)
                {
                    // We know the underlying type is Nullable (by definition).
                    Type propertyType = Nullable.GetUnderlyingType(reportFlagProperty.PropertyType);

                    // The values of the enums are the same as the strings.
                    bool isMatchingDefault = tagReportFlag.Equals(propertyType.GetEnumValues().GetValue(0).ToString(), StringComparison.InvariantCultureIgnoreCase);
                    bool isMatchingAlternate = tagReportFlag.Equals(propertyType.GetEnumValues().GetValue(1).ToString(), StringComparison.InvariantCultureIgnoreCase);

                    if (isMatchingDefault || isMatchingAlternate)
                    {
                        // Identify duplicates.
                        if (previouslyFoundMatchingProperty)
                        {
                            if (previouslyFoundIsDefault == isMatchingDefault)
                            {
                                throw new TomlGenericException(Resources.ExceptionTomlDuplicateReportFlag, rawTag, tagReportFlag);
                            }
                            else
                            {
                                throw new TomlGenericException(Resources.ExceptionTomlMutuallyExclusiveReportFlags, rawTag, tagReportFlag, previouslyFoundFlag);
                            }
                        }

                        var propertyValue = propertyType.GetEnumValues().GetValue(isMatchingDefault ? 0 : 1);

                        reportFlagProperty.SetValue(reportFlagsModule, propertyValue);

                        previouslyFoundFlag = propertyValue.ToString();
                        previouslyFoundMatchingProperty = true;
                        previouslyFoundIsDefault = isMatchingDefault;
                    }
                }
            }

            return reportFlagsModule;
        }
    }
}
