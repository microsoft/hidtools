// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a single CPP struct.
    /// </summary>
    public class CppStruct : ICppGenerator
    {
        private const string DefaultReportNamePrefix = "HidReport";
        private const string ReportMacroIdSuffix = "_ID";

        private readonly ReportModule report;

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

            if (this.report.Name != null)
            {
                // Filter-out invalid characters.
                this.Name = string.Concat(this.report.Name.Where(char.IsLetterOrDigit));
            }
            else
            {
                this.Name = $"{DefaultReportNamePrefix}{this.report}";
            }

            this.UniqueName = UniqueStructNameCache.GenerateUniqueName(this.Name);

            // Note: Since UniqueName is unique, using it as a basis for macroName, will alway guarantee uniqueness.
            this.ReportIdMacroName = GenerateReportIdMacroNameFromUniqueName(this.UniqueName);

            this.Members = new List<ICppGenerator>();
            this.Members.Add(new CppStructMemberSimple(this.report, this.ReportIdMacroName));

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
        public string Name { get; }

        /// <summary>
        /// Gets the unique-name of this struct.
        /// </summary>
        public string UniqueName { get; }

        /// <summary>
        /// Gets the name of the ReportId associated with this struct.
        /// </summary>
        public string ReportIdMacroName { get; }

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
            writer.WriteLineIndented($"#define {this.ReportIdMacroName} ({this.report.Id})");

            writer.WriteLineIndented($"struct {this.UniqueName}");

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

        private static string GenerateReportIdMacroNameFromUniqueName(string uniqueName)
        {
            // Assume camelCase uniqueName.
            // Standard macro format is ALL CAPs with underscores between words.

            string macroName = string.Empty;
            if (uniqueName.Length < 2)
            {
                // Too short to insert underscores.
                macroName = uniqueName.ToUpper();
            }
            else
            {
                for (int i = 0; i < (uniqueName.Length - 1); i++)
                {
                    macroName += char.ToUpper(uniqueName[i]);

                    if (char.IsLower(uniqueName[i]) && char.IsUpper(uniqueName[i + 1]))
                    {
                        // End of word detected.
                        macroName += "_";
                    }
                }

                macroName += char.ToUpper(uniqueName[uniqueName.Length - 1]);
            }

            macroName += CppStruct.ReportMacroIdSuffix;

            return macroName;
        }
    }
}
