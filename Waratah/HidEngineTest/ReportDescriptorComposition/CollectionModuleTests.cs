﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class CollectionModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 5");

        private static readonly HidUsageId DefaultCollectionUsage = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Lighting And Illumination", "LampArrayAttributesReport");

        [TestMethod]
        public void SimpleCollection()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);

            // Using LogicalCollection, as is a simple specialization of BaseCollection.
            CollectionModule logicalCollection = new CollectionModule(HidConstants.MainItemCollectionKind.Logical, report);

            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, string.Empty, report);

            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableModule variable = new VariableModule(DefaultUsageStart, 1, logicalRange, null, null, null, null, null, string.Empty, report);

            logicalCollection.Initialize(DefaultCollectionUsage, new List<BaseModule> { array, variable });

            Assert.AreEqual(HidConstants.MainItemCollectionKind.Logical, logicalCollection.Kind);
            Assert.AreEqual((array.TotalSizeInBits + variable.TotalSizeInBits), logicalCollection.TotalSizeInBits);
        }

        /// <summary>
        /// Modules will be combined when all fields are identical (and ReportCount == 1).  This reduces the number of descriptor items.
        /// The combined module will have all the same fields, except, multiple UsageIds and enlarged ReportCount
        /// </summary>
        [TestMethod]
        public void OptimizedCollectionsCombineFields()
        {
            // Two VariableModules with ReportCount==1, preceed VariableModule with ReportCount==3
            // First two VariableModules will be combined.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                // Using LogicalCollection, as is a simple specialization of BaseCollection.
                CollectionModule logicalCollection = new CollectionModule(HidConstants.MainItemCollectionKind.Logical, report);

                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                VariableModule variable1 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 1"), 1, logicalRange, null, null, null, null, null, string.Empty, report);
                VariableModule variable2 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 2"), 1, logicalRange, null, null, null, null, null, string.Empty, report);
                VariableModule variable3 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 3"), 3, logicalRange, null, null, null, null, null, string.Empty, report);

                logicalCollection.Initialize(DefaultCollectionUsage, new List<BaseModule> { variable1, variable2, variable3 });

                List<ShortItem> generatedItems = logicalCollection.GenerateDescriptorItems(true);

                uint[] foundReportItemCountValue = generatedItems.Where(x => x is ReportCountItem).Select(x => ((ReportCountItem)x).Count).ToArray();

                CollectionAssert.AreEqual(foundReportItemCountValue, new uint[] { 2, 3 });
            }

            // Two VariableModules with ReportCount==1, after VariableModule with ReportCount==3
            // Last two VariableModules will be combined.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                // Using LogicalCollection, as is a simple specialization of BaseCollection.
                CollectionModule logicalCollection = new CollectionModule(HidConstants.MainItemCollectionKind.Logical, report);

                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                VariableModule variable1 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 1"), 1, logicalRange, null, null, null, null, null, string.Empty, report);
                VariableModule variable2 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 2"), 1, logicalRange, null, null, null, null, null, string.Empty, report);
                VariableModule variable3 = new VariableModule(HidUsageTableDefinitions.GetInstance().TryFindUsageId("Ordinal", "Instance 3"), 3, logicalRange, null, null, null, null, null, string.Empty, report);

                logicalCollection.Initialize(DefaultCollectionUsage, new List<BaseModule> {variable3, variable1, variable2});

                List<ShortItem> generatedItems = logicalCollection.GenerateDescriptorItems(true);

                uint[] foundReportItemCountValue = generatedItems.Where(x => x is ReportCountItem).Select(x => ((ReportCountItem)x).Count).ToArray();

                CollectionAssert.AreEqual(foundReportItemCountValue, new uint[] { 3, 2 });
            }
        }

        /// <summary>
        /// Report cannot contain only paddingItems, must contain at least 1 item with a usage.
        /// Because, otherwise, what's the point on the Report!
        /// </summary>
        [TestMethod]
        public void CannotContainOnlyPaddingModules()
        {
            string nonDecoratedTomlDoc = @"
                [[applicationCollection]]
                usage = ['Generic Desktop', 'Mouse']

                    [[applicationCollection.inputReport]]

                        [[applicationCollection.inputReport.physicalCollection]]
                        usage = ['Generic Desktop', 'Pointer']

                            [[applicationCollection.inputReport.physicalCollection.paddingItem]]
                            sizeInBits = 4";

            Assert.ThrowsException<TomlGenericException>(() => TomlDocumentParser.TryParseReportDescriptor(nonDecoratedTomlDoc));
        }
    }
}
