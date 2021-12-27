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
    }
}
