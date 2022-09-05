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
    /// Represents a single CPP struct array member.
    /// </summary>
    public class CppStructMemberEnum : ICppGenerator
    {
        private const string ArrayItemSuffix = "ArrayItem";
        private const string ArrayEnumSuffix = "ArrayValues";
        private const string PluralSuffix = "s";

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberEnum"/> class.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        public CppStructMemberEnum(ArrayModule module)
        {
            this.ElementCount = module.Count;

            List<string> enumValues = new List<string>();
            for (ushort i = 0; i <= (module.UsageEnd.Id - module.UsageStart.Id); i++)
            {
                HidUsageId id = HidUsageTableDefinitions.GetInstance().TryFindUsageId(module.UsageStart.Page.Id, (ushort)(module.UsageStart.Id + i));

                enumValues.Add(id.Name);
            }

            // Explicit name is preferred, but optional. UsageName is a good back-up as it must always be present.
            string baseName = string.IsNullOrEmpty(module.Name) ? module.UsageStart.Page.Name : module.Name;

            string enumName = $"{baseName}{ArrayEnumSuffix}";
            CppFieldPrimativeDataType enumType = CppFieldPrimativeDataTypeExtension.CalculateType(module.LogicalMinimum, module.SizeInBits);

            this.Enumerator = CppEnum.Generate(enumName, enumType, enumValues);

            this.Name = baseName;
        }

        /// <summary>
        /// Gets the name of this member.
        /// </summary>
        public string Name
        {
            get
            {
                string nameSuffix = (this.ElementCount != 1) ? PluralSuffix : string.Empty;

                return this.name + nameSuffix;
            }

            private set
            {
                this.name = string.Concat(value.Where(char.IsLetterOrDigit)) + ArrayItemSuffix;
            }
        }

        /// <summary>
        /// Gets the number of elements in the Cpp Array.
        /// </summary>
        public int ElementCount { get; private set; }

        /// <summary>
        /// Gets the enumerator when if this is an Array.
        /// </summary>
        public CppEnum Enumerator { get; private set; }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// uint32_t Properties[10];
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            string arrayStr = (this.ElementCount > 1) ? $"[{this.ElementCount}]" : string.Empty;

            int uniqueNameSuffix = UniqueMemberNameCache.GenerateUniqueNameSuffix(this.Name);
            string nameIdSuffix = uniqueNameSuffix == 0 ? string.Empty : uniqueNameSuffix.ToString();

            string combined = $"{this.Enumerator.TypeName} {this.Name}{nameIdSuffix}{arrayStr};";

            writer.WriteLineIndented(combined);
        }
    }
}
