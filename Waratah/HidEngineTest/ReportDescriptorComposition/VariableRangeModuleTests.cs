// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System;

    [TestClass]
    public class VariableRangeModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 5");

        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleVariableRangeModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);

            VariableRangeModule variableRange = new VariableRangeModule(DefaultUsageStart, DefaultUsageEnd, logicalRange, null, null, null, null, null, "VariableRangeModuleName", report);

            Assert.AreEqual(0, variableRange.LogicalMinimum);
            Assert.AreEqual(10, variableRange.LogicalMaximum);
            Assert.AreEqual(4, variableRange.SizeInBits);
            Assert.AreEqual(5, variableRange.Count);
            Assert.AreEqual("VariableRangeModuleName", variableRange.Name);
        }

        [TestMethod]
        public void InvalidUsageRange()
        {
            // Usages do not belong to the same Page.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);

                HidUsageId usageWithDifferentPage = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Lighting And Illumination", "PositionXInMicrometers");

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new VariableRangeModule(DefaultUsageStart, usageWithDifferentPage, logicalRange, null, null, null, null, null, string.Empty, report));
            }

            // End Usage has a lesser value than Start Usage.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);

                HidUsageId usageButton6 = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 6");

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new VariableRangeModule(usageButton6, DefaultUsageEnd, logicalRange, null, null, null, null, null, string.Empty, report));
            }

            // Start Usage and End Usage are the same.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new VariableRangeModule(DefaultUsageStart, DefaultUsageStart, logicalRange, null, null, null, null, null, string.Empty, report));
            }
        }

        [TestMethod]
        public void InvalidUsageRangeNotContiguous()
        {
            HidUsagePage testPage = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(0xFFFF, "TestUsagePage");
            HidUsageId testUsage1 = testPage.AddUsageId(new HidUsageId(1, "Id1", new List<HidUsageKind>() { HidUsageKind.DV }));
            HidUsageId testUsage3 = testPage.AddUsageId(new HidUsageId(3, "Id3", new List<HidUsageKind>() { HidUsageKind.DV }));

            // UsageIds in range are not contiguous (i.e. there are undefined UsageIds in the range.)
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);

            Assert.ThrowsException<HidSpecificationException>(() => new VariableRangeModule(testUsage1, testUsage3, logicalRange, null, null, null, null, null, string.Empty, report));
        }
    }
}
