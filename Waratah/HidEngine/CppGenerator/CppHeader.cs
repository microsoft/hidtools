// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.CppGenerator
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Represents the contents of a Descriptor CPP header file.
    /// </summary>
    public class CppHeader
    {
        private Descriptor descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppHeader"/> class.
        /// </summary>
        /// <param name="descriptor">Descriptor to generate the CppHeader from.</param>
        public CppHeader(Descriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Generates a CPP header-file containing the descriptor (in byte format), and Report structs.
        /// </summary>
        /// <returns>Stringified CPP header-file.</returns>
        public string GenerateCpp()
        {
            IndentedWriter writer = new IndentedWriter();

            writer.WriteLineIndented("#include <memory>");
            writer.WriteBlankLine();

            CppDescriptor cppDescriptor = new CppDescriptor(this.descriptor);
            cppDescriptor.GenerateCpp(writer);

            if (Settings.GetInstance().PackingInBytes.HasValue)
            {
                writer.WriteLineIndented("#pragma pack(push,1)");
                writer.WriteBlankLine();

                foreach (ApplicationCollectionModule ac in this.descriptor.ApplicationCollections)
                {
                    foreach (ReportModule report in ac.Reports)
                    {
                        new CppStruct(report).GenerateCpp(writer);
                    }
                }

                writer.WriteLineIndented("#pragma pack(pop)");
            }

            return writer.Generate();
        }
    }
}
