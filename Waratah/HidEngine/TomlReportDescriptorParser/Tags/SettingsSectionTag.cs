// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;

    /// <summary>
    /// Represents a Settings section TOML tag.
    /// Global settings are configured upon successful parsing.
    /// </summary>
    [TagAttribute(TagKind.RootSection, "settings", typeof(Dictionary<string, object>[]))]
    public class SettingsSectionTag : BaseTag
    {
        private static SettingsSectionTag instance = null;

        private SettingsSectionTag(PackingInBytesTag packing, OptimizeTag optimize, GenerateCppTag generateCpp, CppDescriptorNameTag descriptorName, CppDescriptorVariableModifierTag descriptorVariableModifier, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Packing = packing;
            this.Optimize = optimize;
            this.GenerateCpp = generateCpp;
            this.CppDescriptorName = descriptorName;
            this.CppDescriptorVariableModifier = descriptorVariableModifier;
        }

        /// <summary>
        /// Gets the packing/alignment of Report fields (in bytes).
        /// </summary>
        public PackingInBytesTag Packing { get; }

        /// <summary>
        /// Gets whether the report should be optimized.
        /// </summary>
        public OptimizeTag Optimize { get; }

        /// <summary>
        /// Gets whether cpp output should be generated.
        /// </summary>
        public GenerateCppTag GenerateCpp { get; }

        /// <summary>
        /// Gets the name of the cpp variable to hold the descriptor bytes.
        /// </summary>
        public CppDescriptorNameTag CppDescriptorName { get; }

        /// <summary>
        /// Gets the name of the cpp variable modifer of the descriptor bytes.
        /// </summary>
        public CppDescriptorVariableModifierTag CppDescriptorVariableModifier { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="SettingsSectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an SettingsSectionTag.</param>
        /// <param name="reset">Resets the internal state; used only for testing.</param>
        /// <returns>New <see cref="SettingsSectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static SettingsSectionTag TryParse(KeyValuePair<string, object> rawTag, bool reset = false)
        {
            if (!IsValidNameAndType(typeof(SettingsSectionTag), rawTag))
            {
                return null;
            }

            // Only a single instance of Settings tag is expected to be encountered.
            if (instance != null && reset == false)
            {
                throw new TomlGenericException(Resources.ExceptionTomlDuplicateSettingsTag, rawTag, instance.NonDecoratedName, instance.LineNumber);
            }

            PackingInBytesTag packing = null;
            OptimizeTag optimize = null;
            GenerateCppTag generateCpp = null;
            CppDescriptorNameTag descriptorName = null;
            CppDescriptorVariableModifierTag descriptorVariableModifier = null;

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (packing == null)
                {
                    packing = PackingInBytesTag.TryParse(child);

                    if (packing != null)
                    {
                        continue;
                    }
                }

                if (optimize == null)
                {
                    optimize = OptimizeTag.TryParse(child);

                    if (optimize != null)
                    {
                        continue;
                    }
                }

                if (generateCpp == null)
                {
                    generateCpp = GenerateCppTag.TryParse(child);

                    if (generateCpp != null)
                    {
                        continue;
                    }
                }

                if (descriptorName == null)
                {
                    descriptorName = CppDescriptorNameTag.TryParse(child);

                    if (descriptorName != null)
                    {
                        continue;
                    }
                }

                if (descriptorVariableModifier == null)
                {
                    descriptorVariableModifier = CppDescriptorVariableModifierTag.TryParse(child);

                    if (descriptorVariableModifier != null)
                    {
                        continue;
                    }
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            instance = new SettingsSectionTag(packing, optimize, generateCpp, descriptorName, descriptorVariableModifier, rawTag);
            instance.AddToGlobalSettings();

            return instance;
        }

        private void AddToGlobalSettings()
        {
            try
            {
                Settings.GetInstance().PackingInBytes = this.Packing?.Value;
            }
            catch (Exception e)
            {
                throw new TomlGenericException(Resources.ExceptionTomlValueOutOfBounds, this.Packing.RawTag, this.Packing.Value, e.Message);
            }

            if (this.Optimize != null)
            {
                Settings.GetInstance().Optimize = this.Optimize.Value;
            }

            if (this.GenerateCpp != null)
            {
                Settings.GetInstance().GenerateCpp = this.GenerateCpp.Value;
            }

            if (this.CppDescriptorName != null)
            {
                Settings.GetInstance().CppDescriptorName = this.CppDescriptorName.Value;
            }

            if (this.CppDescriptorVariableModifier != null)
            {
                Settings.GetInstance().CppDescriptorVariableModifier = this.CppDescriptorVariableModifier.Value;
            }
        }
    }
}