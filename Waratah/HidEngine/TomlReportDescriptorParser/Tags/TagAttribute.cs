// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;

    /// <summary>
    /// Kind of TOML Tag.
    /// </summary>
    public enum TagKind
    {
        /// <summary>
        /// Tag describes a TOML section.
        /// </summary>
        Section,

        /// <summary>
        /// Tag describes a TopLevel TOML section. (e.g. applicationCollection).
        /// </summary>
        RootSection,

        /// <summary>
        /// Tag describes a TOML Key.
        /// </summary>
        Key,
    }

    /// <summary>
    /// Mandatory attribute for all TOML Tag classes.
    /// This allows annotation of the Type, (rather than through static variables),
    /// allowing reflection discovery techniques to validate tag Names and Types during parsing.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TagAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagAttribute"/> class.
        /// </summary>
        /// <param name="kind">Kind of TOML Tag.</param>
        /// <param name="name">Name of the TOML Tag in the TOML document.</param>
        /// <param name="tomlTypes">Supported <see cref="Type"/>s for this Tag.</param>
        public TagAttribute(TagKind kind, string name, params Type[] tomlTypes)
        {
            this.Name = name;
            this.Kind = kind;
            this.TomlTypes = tomlTypes;
        }

        /// <summary>
        /// Gets the kind of TOML Tag.
        /// </summary>
        public TagKind Kind { get; }

        /// <summary>
        /// Gets the named string for the TOML Tag that is used in the TOML document.
        /// This is used during parsing and validation.
        /// </summary>
        /// <example>applicationCollection.</example>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="Type"/>s supported by this Tag.
        /// This is used during parsing and validation.
        /// </summary>
        public Type[] TomlTypes { get; }
    }
}
