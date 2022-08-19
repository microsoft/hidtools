// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents a single CPP struct.
    /// </summary>
    public class CppStruct : ICppGenerator
    {
        private readonly ReportModule report;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStruct"/> class.
        /// </summary>
        /// <param name="report">Report to build this struct from.</param>
        public CppStruct(ReportModule report)
        {
            // Structs are never nested within each other, so OK to reset caches.
            CppEnum.Reset();
            UniqueNameCache.Reset();

            this.report = report;

            this.Members = new List<ICppGenerator>();
            this.Members.Add(new CppStructMemberSimple(this.report));

            foreach (BaseElementModule module in report.GetReportElements())
            {
                this.Members.AddRange(ParseFromModule(module));
            }
        }

        /// <summary>
        /// Gets the members that are part of this struct.
        /// </summary>
        public List<ICppGenerator> Members { get; }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// struct HidReportInput1
        /// {
        ///     uint32_t Field1;
        ///     uint32_t Field2;
        /// }
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            writer.WriteLineIndented($"struct HidReport{this.report}");
            writer.WriteLineIndented("{");

            using (DisposableIndent indent = writer.CreateDisposableIndent())
            {
                CppEnum.Cache.ForEach(x => x.GenerateCpp(writer));

                this.Members.ForEach(x => x.GenerateCpp(writer));
            }

            writer.WriteLineIndented("};");
            writer.WriteBlankLine();
        }

        /// <summary>
        /// Parses CppStructMembers from supplied module.  Most modules result in only a single member.
        /// </summary>
        /// <param name="module">Module to parse.</param>
        /// <returns>Parsed CppStructMembers.</returns>
        private static List<ICppGenerator> ParseFromModule(BaseModule module)
        {
            List<ICppGenerator> members = new List<ICppGenerator>();

            switch (module)
            {
                case VariableModule castModule:
                {
                    members.Add(new CppStructMemberSimple(castModule));
                    break;
                }

                case VariableRangeModule castModule:
                {
                    for (int i = 0; i < castModule.Count; i++)
                    {
                        members.Add(new CppStructMemberSimple(castModule, i));
                    }

                    break;
                }

                case ArrayModule castModule:
                {
                    members.Add(new CppStructMemberEnum(castModule));
                    break;
                }

                case PaddingModule castModule:
                {
                    members.Add(new CppStructMemberSimple(castModule));
                    break;
                }

                default:
                {
                    System.Environment.FailFast($"Unsupported base type {module.GetType().Name}");
                    break;
                }
            }

            return members;
        }
    }
}
