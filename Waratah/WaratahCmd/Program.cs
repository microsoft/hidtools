// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace WaratahCmd
{
    using CommandLine;
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using System;
    using System.Drawing;
    using System.IO;
    using Console = Colorful.Console;

    class Program
    {
        static void Main(string[] args)
        {
            CommandLineOptions options = null;

            ParserResult<CommandLineOptions> result = Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed((CommandLineOptions opt) => {

                options = opt;

            }).WithNotParsed(error => {

                return;
            });

            if (result.Tag == ParserResultType.NotParsed)
            {
                Environment.Exit(1);
            }

            try
            {
                Settings.GetInstance().SourceFilePath = options.SourceFile;
                Settings.GetInstance().DestinationFilePath = options.DestinationFile;

                Console.WriteLine("Reading from file \'{0}\'", Settings.GetInstance().SourceFilePath);
                string descriptorTomlDoc = File.ReadAllText(Settings.GetInstance().SourceFilePath);

                Descriptor descriptor = TomlDocumentParser.TryParseReportDescriptor(descriptorTomlDoc);

                string destinationFileContents = descriptor.GenerateText();

                Console.WriteLine("Creating output file \'{0}\'", Settings.GetInstance().DestinationFilePath);
                StreamWriter destinationFile = File.CreateText(Settings.GetInstance().DestinationFilePath);
                Console.WriteLine("Successfully created output file \'{0}\'", Settings.GetInstance().DestinationFilePath);

                destinationFile.Write(destinationFileContents);
                destinationFile.Flush();
                Console.WriteLine("Successfully generated descriptor file \'{0}\'", Settings.GetInstance().DestinationFilePath, Color.Green);

                Console.WriteLine(descriptor.GenerateSummary(), Color.LightGreen);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, Color.Red);
                Environment.Exit(1);
            }

            return;
        }
    }
}
