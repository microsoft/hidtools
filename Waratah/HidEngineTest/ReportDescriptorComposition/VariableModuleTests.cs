// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VariableModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsage = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");

        [TestMethod]
        public void SimpleVariableModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);

            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, "VariableModuleName", report);

            Assert.AreEqual(0, variable.LogicalMinimum);
            Assert.AreEqual(10, variable.LogicalMaximum);
            Assert.AreEqual(4, variable.SizeInBits);
            Assert.AreEqual("VariableModuleName", variable.Name);
        }
    }
}
