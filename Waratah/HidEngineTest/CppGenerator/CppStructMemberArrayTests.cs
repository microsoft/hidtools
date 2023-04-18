// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.CppGenerator
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.CppGenerator;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class CppStructMemberArrayTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsage = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 5");

        [TestInitialize]
        public void Initialize()
        {
            // Currently all Cpp generation requires an 8bit alignment.
            Settings.GetInstance().PackingInBytes = 1;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleCppStructMemberArrayReport()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, null, report);
            report.Initialize(1, null, new List<BaseModule> { variable });

            CppStructMemberArray cppStructMemberArray = new CppStructMemberArray(report);

            Assert.AreEqual("Payload", cppStructMemberArray.Name);
            Assert.AreEqual(1, cppStructMemberArray.Size);
        }
    }
}
