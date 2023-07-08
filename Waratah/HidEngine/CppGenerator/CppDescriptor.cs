// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;

    /// <summary>
    /// Represents the contents of a CPP header file.
    /// </summary>
    public class CppDescriptor : ICppGenerator
    {
        private const int IndentSize = 4;

        private static readonly string CppHeaderIndentString = string.Empty.PadLeft(IndentSize, ' ');

        private Descriptor descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppDescriptor"/> class.
        /// </summary>
        /// <param name="descriptor">Underlying descriptor to generate from.</param>
        public CppDescriptor(Descriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// static const uint8_t hidReportDescriptor[] =
        /// {
        ///     0x05, 0x01,    // UsagePage(Generic Desktop[1])
        ///     0x09, 0x02,    // UsageId(Mouse[2])
        ///     0xA1, 0x01,    // Collection(Application)
        ///     0x85, 0x01,    //     ReportId(1)
        ///     0x09, 0x01,    //     UsageId(Pointer[1])
        ///     0xA1, 0x00,    //     Collection(Physical)
        ///     0x09, 0x2F,    //         UsageId(X[47])
        ///     0x09, 0x30,    //         UsageId(Y[48])
        ///     0x15, 0x80,    //         LogicalMinimum(-128)
        ///     0x25, 0x7F,    //         LogicalMaximum(127)
        ///     0x95, 0x02,    //         ReportCount(2)
        ///     0x75, 0x08,    //         ReportSize(8)
        ///     0x81, 0x06,    //         Input(Data, Variable, Relative)
        ///     0xC0,          //     EndCollection()
        ///     0xC0,          // EndCollection()
        /// };
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            // Determine the initial indent required to ensure all non-byte text starts at the same offset.
            int maxWireRepresentationStringLength = this.descriptor.LastGeneratedItems.Max(x => x.WireRepresentationString(WireRepresentationKind.CppHeader).Length);

            string variableModifier = string.IsNullOrEmpty(Settings.GetInstance().CppDescriptorVariableModifier) ? string.Empty : $" {Settings.GetInstance().CppDescriptorVariableModifier}";

            writer.WriteLineIndented($"static const{variableModifier} uint8_t {Settings.GetInstance().CppDescriptorName}[] = {Environment.NewLine}{{");

            foreach (ShortItem item in this.descriptor.LastGeneratedItems)
            {
                if (item.GetType() == typeof(EndCollectionItem))
                {
                    writer.Decrease();
                }

                string paddedWireRepresentation = (item.WireRepresentationString(WireRepresentationKind.CppHeader) + ",").PadRight(maxWireRepresentationStringLength + IndentSize + 1, ' ');

                writer.WriteLineNoIndent($"{CppHeaderIndentString}{paddedWireRepresentation}// ");
                writer.WriteLineIndented($"{item}");

                if (item.GetType() == typeof(CollectionItem))
                {
                    writer.Increase();
                }
            }

            writer.WriteLineIndented("};");
            writer.WriteBlankLine();
        }
    }
}
