// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using HidSpecification;

    /// <summary>
    /// Declares the Usage Kinds that are supported by this TOML Tag.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SupportedHidKindsAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedHidKindsAttribute"/> class.
        /// </summary>
        /// <param name="usageKinds">Usage Kinds supported by this TOML Tag.</param>
        public SupportedHidKindsAttribute(params HidUsageKind[] usageKinds)
        {
            this.Kinds = usageKinds.ToList();
        }

        /// <summary>
        /// Gets the kinds of Usages supported.
        /// </summary>
        public List<HidUsageKind> Kinds { get; }

        /// <summary>
        /// Validates that this Usage is supported by this Type.
        /// </summary>
        /// <param name="usage">Usage to validate support for.</param>
        /// <param name="rawTag">Root TOML element describing a Usage.</param>
        /// <param name="validationType">Type to use for Usage Kind validation.</param>
        public static void ValidateSupported(HidUsageId usage, KeyValuePair<string, object> rawTag, Type validationType = null)
        {
            // Can't validate.
            if (validationType == null)
            {
                return;
            }

            // Validate the kind of this Usage is supported by the caller.
            SupportedHidKindsAttribute kindsAttribute = validationType.GetCustomAttribute<SupportedHidKindsAttribute>();
            TagAttribute typeAttribute = validationType.GetCustomAttribute<TagAttribute>();

            // Must contain at least one of the supplied kinds.
            bool isOneKindSupported = false;

            foreach (HidUsageKind kind in usage.Kinds)
            {
                if (kindsAttribute.Kinds.Contains(kind))
                {
                    isOneKindSupported = true;
                    break;
                }
            }

            if (isOneKindSupported)
            {
                return;
            }

            // Identify which tags 'could' support the Usage.
            List<TagAttribute> supportedTags = new List<TagAttribute>();

            // Find all sections that could support this Usage.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsDefined(typeof(SupportedHidKindsAttribute), true))
                    {
                        SupportedHidKindsAttribute attribute = type.GetCustomAttribute<SupportedHidKindsAttribute>();

                        foreach (HidUsageKind kind in usage.Kinds)
                        {
                            if (attribute.Kinds.Contains(kind))
                            {
                                supportedTags.Add(type.GetCustomAttribute<TagAttribute>());
                            }
                        }
                    }
                }
            }

            string supportedTagsCombined = string.Join(", ", supportedTags.Select(x => "\'" + x.Name + "\'"));
            string supportUsageKinds = string.Join(", ", usage.Kinds);

            throw new TomlGenericException($"Usage \'{usage}\' (with types, {supportUsageKinds}) is unsupported in \'{typeAttribute.Name}\'.  This Usage can be used in {supportedTagsCombined}", rawTag);
        }
    }
}
