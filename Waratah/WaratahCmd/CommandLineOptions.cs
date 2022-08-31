// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace WaratahCmd
{
    using CommandLine;
    using CommandLine.Text;
    using System.Collections.Generic;

    public class CommandLineOptions
    {
        [Value(0)]
        [Option('s', "source", Required = true, HelpText = "Source file, must be in descriptor .toml format")]
        public string SourceFile { get; set; }

        [Value(1)]
        [Option('d', "destination", Required = false, HelpText = "Destination file without suffix.  Suffix determined dynamically from settings.")]
        public string DestinationFile { get; set; }

        [Value(2)]
        [Option('h', "hidusagetables", Required = false, HelpText = "Alternative HID Usage Tables file.  Must be a PDF provided by usb.org/hid")]
        public string HidUsageTablesFile { get; set; }

        [Usage(ApplicationAlias = "WaratahCmd")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Generate HID descriptor representation from TOML", new CommandLineOptions { SourceFile = "descriptor.toml" })
                };
            }
        }
    }
}
