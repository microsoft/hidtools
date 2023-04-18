// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes a simple array member.
    /// </summary>
    public class CppStructMemberArray : ICppGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberArray"/> class.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        public CppStructMemberArray(ReportModule module)
        {
            this.Size = (module.TotalSizeInBits / 8);
            this.Name = "Payload";
        }

        /// <summary>
        /// Gets the name of this array.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the size of the array. (i.e. element count).
        /// </summary>
        public int Size { get; private set; }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// uint8_t Payload[10];
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            int uniqueNameSuffix = UniqueMemberNameCache.GenerateUniqueNameSuffix(this.Name);
            string nameIdSuffix = uniqueNameSuffix == 0 ? string.Empty : uniqueNameSuffix.ToString();

            string combined = $"uint8_t {this.Name}{nameIdSuffix}[{this.Size}];";

            writer.WriteLineIndented(combined);
        }
    }
}
