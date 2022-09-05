// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ReportModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 5");

        [TestMethod]
        public void SimpleReportModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);

            // Requires 3bits.
            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, string.Empty, report);

            // Requires 5bits.
            DescriptorRange logicalRange = new DescriptorRange(0, 16);
            VariableModule variable = new VariableModule(DefaultUsageStart, 1, logicalRange, null, null, null, null, null, string.Empty, report);

            report.Initialize(1, null, new List<BaseModule> { array, variable });

            Assert.AreEqual(2, report.Children.Count);
            Assert.AreEqual(1, report.Id);
            Assert.AreEqual(ReportKind.Input, report.Kind);
            Assert.AreEqual(8, report.TotalSizeInBits);
        }

        [TestMethod]
        public void ReportPadding()
        {
            // PaddingModule is added when report size is not a multiple of 8.

            // 1 bit padding
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                VariableModule variable = new VariableModule(DefaultUsageStart, 1, null, null, 7, null, null, null, string.Empty, report);

                report.Initialize(1, null, new List<BaseModule> { variable });

                Assert.AreEqual(2, report.Children.Count);
                Assert.AreEqual(1, report.Children[1].TotalSizeInBits);
            }

            // 6 bit padding
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                VariableModule variable = new VariableModule(DefaultUsageStart, 1, null, null, 2, null, null, null, string.Empty, report);

                report.Initialize(1, null, new List<BaseModule> { variable });

                Assert.AreEqual(2, report.Children.Count);
                Assert.AreEqual(6, report.Children[1].TotalSizeInBits);
            }
        }

        [TestMethod]
        public void InvalidId()
        {
            void CreateReportWithInvalidId(int id)
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, string.Empty, report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => report.Initialize(id, null, new List<BaseModule> { array }));
            }

            CreateReportWithInvalidId(0);
            CreateReportWithInvalidId(-1);
            CreateReportWithInvalidId(256);
        }

        [TestMethod]
        public void SimplePaddingModuleCombination()
        {
            // PaddingModules are optimistically combined (where possible) to reduce number of modules.
            // Combining below 2 4bit paddingModule into a single 8bit paddingModule.
            {
                string nonDecoratedTomlDoc = @"
                    [[applicationCollection]]
                    usage = ['Generic Desktop', 'Keyboard']

                        [[applicationCollection.inputReport]]

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 4

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 4";

                ValidateModuleSizes(nonDecoratedTomlDoc, 8);
            }

            // No combination possible. (only a single paddingModule).
            {
                string nonDecoratedTomlDoc = @"
                    [[applicationCollection]]
                    usage = ['Generic Desktop', 'Keyboard']

                        [[applicationCollection.inputReport]]

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 8";

                ValidateModuleSizes(nonDecoratedTomlDoc, 8);
            }

            // Can be combined, including 3bit paddingModule auto-added to ensure byte-alignment.
            {
                string nonDecoratedTomlDoc = @"
                    [[applicationCollection]]
                    usage = ['Generic Desktop', 'Keyboard']

                        [[applicationCollection.inputReport]]

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 7

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 6";

                // Extra bits added at end for report byte-alignment.
                ValidateModuleSizes(nonDecoratedTomlDoc, 16);
            }

            // Can't be combined, as each field must be <= 32bits.
            {
                string nonDecoratedTomlDoc = @"
                    [[applicationCollection]]
                    usage = ['Generic Desktop', 'Keyboard']

                        [[applicationCollection.inputReport]]

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 31

                            [[applicationCollection.inputReport.paddingItem]]
                            sizeInBits = 9";

                // Extra bits added at end for report byte-alignment.
                ValidateModuleSizes(nonDecoratedTomlDoc, 31, 9);
            }
        }

        [TestMethod]
        public void SimpleOverSpanning()
        {
            string nonDecoratedTomlDoc = @"
                [[applicationCollection]]
                usage = ['Generic Desktop', 'Keyboard']

                    [[applicationCollection.inputReport]]

                        [[applicationCollection.inputReport.paddingItem]]
                        sizeInBits = 7

                        [[applicationCollection.inputReport.variableItem]]
                        usage = ['Button', 'Button 1']
                        sizeInBits = 26
                        count = 1
                        logicalValueRange = 'maxUnsignedSizeRange'";

            // hid_11 spec (8.4 Report Contraints), "An item field cannot span more than 4 bytes in a report."
            // Smallest possible occurrence of (26) bits being spread over 5bytes.
            //
            // Expected bit layout without padding (each Module# represented as bit).  (TotalSize == 40)
            //      11111112|22222222|22222222|22222222|2
            // Expected bit layout with padding (TotalSize == 40)
            //      1111111x|22222222|22222222|22222222|22xxxxxx
            ValidateModuleSizes(nonDecoratedTomlDoc, 8, 26, 6);
        }

        [TestMethod]
        public void OverspanningVariableModuleRequiresPaddingAndSizeChange()
        {
            string nonDecoratedTomlDoc = @"
                [[applicationCollection]]
                usage = ['Generic Desktop', 'Keyboard']

                    [[applicationCollection.inputReport]]

                        [[applicationCollection.inputReport.paddingItem]]
                        sizeInBits = 7

                        [[applicationCollection.inputReport.variableItem]]
                        usage = ['Button', 'Button 1']
                        sizeInBits = 27
                        count = 5
                        logicalValueRange = 'maxUnsignedSizeRange'";

            // VariableModule1 requires 27bits x 5.
            // PaddingModule1 is 7bits, means these next 27bits will be spread across 5 bytes.
            // Padding must be added:-
            //  - Before VariableModule1 (1bit) (to ensure first VariableModule1 element does not overspan).
            //  - And each element's size must be increased (to 28bits), to ensure no subsequent elements, will overspan.
            ValidateModuleSizes(nonDecoratedTomlDoc, 8, 140, 4);
        }

        [TestMethod]
        public void OverspanningVariableRangeModule()
        {
            string nonDecoratedTomlDoc = @"
                [[applicationCollection]]
                usage = ['Generic Desktop', 'Keyboard']

                    [[applicationCollection.inputReport]]

                        [[applicationCollection.inputReport.paddingItem]]
                        sizeInBits = 7

                        [[applicationCollection.inputReport.variableItem]]
                        usageRange = ['Button', 'Button 1', 'Button 5']
                        sizeInBits = 27
                        logicalValueRange = 'maxUnsignedSizeRange'";

            // VariableRangeModule1 requires 27bits x 5.
            // PaddingModule1 is 7bits, means these next 27bits will be spread across 5 bytes.
            // Padding must be added:-
            //  - Before VariableModule1 (1bit) (to ensure first VariableRangeModule1 element does not overspan).
            //  - And each element's size must be increased (to 28bits), to ensure no subsequent elements, will overspan.
            ValidateModuleSizes(nonDecoratedTomlDoc, 8, 140, 4);
        }

        void ValidateModuleSizes(string nonDecoratedTomlDoc, params int[] moduleSizes)
        {
            Descriptor descriptor = TomlDocumentParser.TryParseReportDescriptor(nonDecoratedTomlDoc);

            int expectedTotalSize = moduleSizes.Sum();

            Assert.AreEqual(1, descriptor.ApplicationCollections.Count);
            Assert.AreEqual(expectedTotalSize, descriptor.ApplicationCollections[0].TotalSizeInBits);

            Assert.AreEqual(1, descriptor.ApplicationCollections[0].Reports.Count);
            Assert.AreEqual(moduleSizes.Length, descriptor.ApplicationCollections[0].Reports[0].GetReportElements().Count);

            for (int i = 0; i < moduleSizes.Length; i++)
            {
                Assert.AreEqual(moduleSizes[i], descriptor.ApplicationCollections[0].Reports[0].GetReportElements()[i].TotalSizeInBits);
            }
        }
    }
}
