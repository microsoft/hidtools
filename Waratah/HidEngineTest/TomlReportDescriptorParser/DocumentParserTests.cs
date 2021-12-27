// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumentParserTests
    {
        [TestMethod]
        public void InvalidTomlSectionName()
        {
            // Below TOML string has an extra period (.) between inputReport and paddingItem
            // This is invalid for a TOML doc.
            // This error will be caught be parsing the TOML doc before the Decorater has a chance to run.
            string nonDecoratedString = @"
                [[inputReport]]
                id = 1
                    [[inputReport..paddingItem]]
                    sizeInBits = 1";

            Assert.ThrowsException<Nett.Parser.ParseException>(() => TomlDocumentParser.TryParseReportDescriptor(nonDecoratedString));
        }

        [TestMethod]
        public void MultipleApplicationCollections()
        {
            string nonDecoratedString =
              @"[[applicationCollection]]
                usage = ['Generic Desktop', 'Gamepad']

                    [[applicationCollection.inputReport]]

                        # Two axis rocker (tilting forward/backward and left/right)
                        [[applicationCollection.inputReport.physicalCollection]]
                        usage = ['Generic Desktop', 'Pointer']

                            [[applicationCollection.inputReport.physicalCollection.variableItem]]
                            usage = ['Generic Desktop', 'X']
                            logicalValueRange = [-1, 1]
                [[applicationCollection]]
                usage = ['Generic Desktop', 'Gamepad']

                    [[applicationCollection.inputReport]]

                        # Two axis rocker (tilting forward/backward and left/right)
                        [[applicationCollection.inputReport.physicalCollection]]
                        usage = ['Generic Desktop', 'Pointer']

                            [[applicationCollection.inputReport.physicalCollection.variableItem]]
                            usage = ['Generic Desktop', 'X']
                            logicalValueRange = [-1, 1]";

            Descriptor parser = TomlDocumentParser.TryParseReportDescriptor(nonDecoratedString);

            Assert.AreEqual(parser.ApplicationCollections.Count, 2);

            Assert.IsNotNull(parser.ApplicationCollections[0]);
            Assert.IsNotNull(parser.ApplicationCollections[0].Reports);
            Assert.AreEqual(parser.ApplicationCollections[0].Reports.Count, 1);

            Assert.IsNotNull(parser.ApplicationCollections[1]);
            Assert.IsNotNull(parser.ApplicationCollections[1].Usage);
            Assert.IsNotNull(parser.ApplicationCollections[1].Reports);
            Assert.AreEqual(parser.ApplicationCollections[1].Reports.Count, 1);
        }

        [TestMethod]
        public void InvalidTopLevelTags()
        {
            // Only "applicationCollection" tag is valid top-level tag.

            // Try parsing a 'valid' tag (inputReport) but at the wrong location
            // Note: An invalid tag section name will be filtered out by the DocumentParser
            // during decoration.

            string nonDecoratedString =
              @"[[applicationCollection]]
                usage = ['Generic Desktop', 'Gamepad']

                    [[applicationCollection.inputReport]]

                        # Two axis rocker (tilting forward/backward and left/right)
                        [[applicationCollection.inputReport.physicalCollection]]
                        usage = ['Generic Desktop', 'Pointer']

                            [[applicationCollection.inputReport.physicalCollection.variableItem]]
                            usage = ['Generic Desktop', 'X']
                            logicalValueRange = [-1, 1]
                [[inputReport]]";

            Assert.ThrowsException<TomlInvalidLocationException>(() => TomlDocumentParser.TryParseReportDescriptor(nonDecoratedString));
        }

        [TestMethod]
        public void DuplicateSettingsSection()
        {
            // Only a single "settings" section is permitted.

            string nonDecoratedString = @"
                [[settings]]
                packingInBytes = 1
                [[settings]]
                packingInBytes = 2";

            Assert.ThrowsException<TomlGenericException>(() => TomlDocumentParser.TryParseReportDescriptor(nonDecoratedString));
        }
    }
}
