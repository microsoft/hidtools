// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents a single CPP struct.
    /// </summary>
    public class CppStruct : ICppGenerator
    {
        private const string ReportName = "HidReport";

        private readonly ReportModule report;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppStruct"/> class.
        /// </summary>
        /// <param name="report">Report to build this struct from.</param>
        public CppStruct(ReportModule report)
        {
            // Structs are never nested within each other, so OK to reset caches.
            CppEnum.Reset();
            UniqueMemberNameCache.Reset();

            this.report = report;
            this.Name = report.Name;

            this.Members = new List<ICppGenerator>();
            this.Members.Add(new CppStructMemberSimple(this.report));

            if (Settings.GetInstance().PackingInBytes.HasValue)
            {
                foreach (BaseElementModule module in report.GetReportElements())
                {
                    this.Members.AddRange(ParseFromModule(module));
                }
            }
            else
            {
                this.Members.Add(new CppStructMemberArray(this.report));
            }
        }

        /// <summary>
        /// Gets the name of this struct.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            private set
            {
                if (value != null)
                {
                    // Filter-out invalid characters.
                    this.name = string.Concat(value.Where(char.IsLetterOrDigit));
                }
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
            string tempName;
            if (string.IsNullOrEmpty(this.report.Name))
            {
                tempName = $"{ReportName}{this.report}";
            }
            else
            {
                tempName = this.report.Name;
            }

            writer.WriteLineIndented($"struct {UniqueStructNameCache.GenerateUniqueName(tempName)}");

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
