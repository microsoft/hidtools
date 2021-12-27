// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using HidEngine;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ArrayModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 5");

        [TestMethod]
        public void SimpleArrayModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, "ArrayModuleName", report);

            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(1, array.LogicalMinimum);
            Assert.AreEqual(5, array.LogicalMaximum);
            Assert.AreEqual(3, array.SizeInBits);
            Assert.AreEqual(3, array.TotalSizeInBits);
            Assert.AreEqual("Button 1", array.UsageStart.Name);
            Assert.AreEqual("Button 5", array.UsageEnd.Name);
            Assert.AreEqual("ArrayModuleName", array.Name);
        }

        [TestMethod]
        public void InvalidUsageRange()
        {
            // Usages do not belong to the same Page.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                HidUsageId usageWithDifferentPage = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Lighting And Illumination", "PositionXInMicrometers");

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ArrayModule(DefaultUsageStart, usageWithDifferentPage, 1, null, string.Empty, report));
            }

            // End Usage has a lesser value than Start Usage.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                HidUsageId usageButton6 = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 6");

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ArrayModule(usageButton6, DefaultUsageEnd, 1, null, string.Empty, report));
            }

            // Start Usage and End Usage are the same.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ArrayModule(DefaultUsageStart, DefaultUsageStart, 1, null, string.Empty, report));
            }
        }

        [TestMethod]
        public void InvalidModuleFlagsForReportKind()
        {
            void ValidateInvalidModuleFlagsForInputReport(DescriptorModuleFlags flags)
            {
                ReportModule report = new ReportModule(ReportKind.Input, null);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, flags, string.Empty, report));
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.VolatilityKind = HidConstants.MainDataItemVolatilityKind.NonVolatile;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.WrappingKind = HidConstants.MainDataItemWrappingKind.NoWrap;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.LinearityKind = HidConstants.MainDataItemLinearityKind.Linear;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.PreferenceStateKind = HidConstants.MainDataItemPreferenceStateKind.PreferredState;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.MeaningfulDataKind = HidConstants.MainDataItemMeaningfulDataKind.NoNullPosition;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }

            {
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.ContingentKind = HidConstants.MainDataItemContingentKind.BitField;
                ValidateInvalidModuleFlagsForInputReport(flags);
            }
        }
    }
}
