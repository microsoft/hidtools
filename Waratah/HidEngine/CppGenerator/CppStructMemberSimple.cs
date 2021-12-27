// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.CppGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;

    /// <summary>
    /// Describies a simple struct member (i.e. discrete type.)
    /// </summary>
    public class CppStructMemberSimple : ICppGenerator
    {
        private const string ReportIdName = "ReportId";
        private const string ButtonName = "Button";
        private const string PaddingName = "Padding";
        private const string PluralSuffix = "s";

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberSimple"/> class.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        public CppStructMemberSimple(ReportModule module)
        {
            this.Type = CppFieldPrimativeDataType.uint8_t;
            this.ArraySize = 1;

            this.Name = ReportIdName;
            this.InitialValue = module.Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberSimple"/> class.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        public CppStructMemberSimple(VariableModule module)
        {
            this.Type = CppFieldPrimativeDataTypeExtension.CalculateType(module.LogicalMinimum, module.SizeInBits);
            this.ArraySize = module.Count;

            // Explicit name is preferred, but optional. UsageName is a good back-up as it must always be present.
            string baseName = string.IsNullOrEmpty(module.Name) ? module.Usage.Name : module.Name;

            // Always suffix a 1bit field (even if padded) with Button suffix.
            string buttonNameSuffix = module.NonAdjustedSizeInBits == 1 ? ButtonName : string.Empty;

            this.Name = baseName + buttonNameSuffix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberSimple"/> class.
        /// Must be called for each index in <see cref="VariableRangeModule"/>.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        /// <param name="index">Index from within the range to initialize from. 0-based.</param>
        public CppStructMemberSimple(VariableRangeModule module, int index)
        {
            this.Type = CppFieldPrimativeDataTypeExtension.CalculateType(module.LogicalMinimum, module.SizeInBits);
            this.ArraySize = 1;

            // Range must be contiguous, so safe to access Ids within explicit bounds.
            HidUsageId indexId = HidUsageTableDefinitions.GetInstance().TryFindUsageId(module.UsageStart.Page.Id, (ushort)(module.UsageStart.Id + index));

            // Explicit name is preferred, but optional. UsageName is a good back-up as it must always be present.
            string baseName = string.IsNullOrEmpty(module.Name) ? indexId.Name : module.Name;

            // Always suffix a 1bit field (even if padded) with Button suffix.
            string buttonNameSuffix = module.NonAdjustedSizeInBits == 1 ? ButtonName : string.Empty;

            this.Name = baseName + buttonNameSuffix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStructMemberSimple"/> class.
        /// </summary>
        /// <param name="module">The module to initialize from.</param>
        public CppStructMemberSimple(PaddingModule module)
        {
            this.Type = CppFieldPrimativeDataTypeExtension.CalculateType(0, module.SizeInBits);
            this.ArraySize = 1;

            this.Name = PaddingName;
        }

        /// <summary>
        /// Gets the name of this member.
        /// </summary>
        public string Name
        {
            get
            {
                string nameSuffix = (this.ArraySize != 1) ? PluralSuffix : string.Empty;

                return this.name + nameSuffix;
            }

            private set
            {
                // Filter-out invalid characters.
                this.name = string.Concat(value.Where(char.IsLetterOrDigit));
            }
        }

        /// <summary>
        /// Gets the Type of underlying member (when not an Array).
        /// </summary>
        public CppFieldPrimativeDataType Type { get; private set; }

        /// <summary>
        /// Gets the size of the array, if this member represents an array.
        /// </summary>
        public int ArraySize { get; private set; }

        /// <summary>
        /// Gets the InitialValue of the field.  Typically this is null.
        /// Member will be assigned this value in-line in generated CPP.
        /// </summary>
        public int? InitialValue { get; private set; }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// uint32_t Properties[10];
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            string typeStr = Enum.GetName(typeof(CppFieldPrimativeDataType), this.Type);

            string arrayStr = (this.ArraySize > 1) ? $"[{this.ArraySize}]" : string.Empty;

            string initialValue = this.InitialValue.HasValue ? $" = {this.InitialValue}" : string.Empty;

            int uniqueNameSuffix = UniqueNameCache.GenerateUniqueNameSuffix(this.Name);
            string nameIdSuffix = uniqueNameSuffix == 0 ? string.Empty : uniqueNameSuffix.ToString();

            string combined = $"{typeStr} {this.Name}{nameIdSuffix}{arrayStr}{initialValue};";

            writer.WriteLineIndented(combined);
        }
    }
}
