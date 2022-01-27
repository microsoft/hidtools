// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using HidEngine;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;
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
    }
}
