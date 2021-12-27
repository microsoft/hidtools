// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.CppGenerator
{
    using HidEngine;
    using HidEngine.CppGenerator;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class CppStructMemberSimpleTests
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
        public void SimpleCppStructMemberSimpleReport()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, null, report);
            report.Initialize(1, new List<BaseModule> { variable });

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(report);

            Assert.AreEqual("ReportId", cppStructMemberSimple.Name);
            Assert.AreEqual(1, cppStructMemberSimple.InitialValue);
            Assert.AreEqual(CppFieldPrimativeDataType.uint8_t, cppStructMemberSimple.Type);
        }

        [TestMethod]
        public void SimpleCppStructMemberSimpleVariableModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, null, report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variable);

            Assert.AreEqual("Button1", cppStructMemberSimple.Name);
            Assert.IsNull(cppStructMemberSimple.InitialValue);
            Assert.AreEqual(1, cppStructMemberSimple.ArraySize);
            Assert.AreEqual(CppFieldPrimativeDataType.uint8_t, cppStructMemberSimple.Type);
        }

        [TestMethod]
        public void NonDefaultNameVariableModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, "TestName", report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variable);

            Assert.AreEqual("TestName", cppStructMemberSimple.Name);
        }

        [TestMethod]
        public void ButtonSuffixVariableModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 1);
            VariableModule variable = new VariableModule(DefaultUsage, 1, logicalRange, null, null, null, null, null, "TestName", report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variable);

            Assert.AreEqual("TestNameButton", cppStructMemberSimple.Name);
        }

        [TestMethod]
        public void SimpleCppStructMemberSimpleVariableRangeModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableRangeModule variableRange = new VariableRangeModule(DefaultUsageStart, DefaultUsageEnd, logicalRange, null, null, null, null, null, null, report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variableRange, 1);

            Assert.AreEqual("Button2", cppStructMemberSimple.Name);
            Assert.IsNull(cppStructMemberSimple.InitialValue);
            Assert.AreEqual(1, cppStructMemberSimple.ArraySize);
            Assert.AreEqual(CppFieldPrimativeDataType.uint8_t, cppStructMemberSimple.Type);
        }

        [TestMethod]
        public void NonDefaultNameVariableRangeModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            VariableRangeModule variableRange = new VariableRangeModule(DefaultUsageStart, DefaultUsageEnd, logicalRange, null, null, null, null, null, "TestName", report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variableRange, 1);

            Assert.AreEqual("TestName", cppStructMemberSimple.Name);
        }

        [TestMethod]
        public void ButtonSuffixVariableRangeModule()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 1);
            VariableRangeModule variableRange = new VariableRangeModule(DefaultUsageStart, DefaultUsageEnd, logicalRange, null, null, null, null, null, "TestName", report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(variableRange, 1);

            Assert.AreEqual("TestNameButton", cppStructMemberSimple.Name);
        }

        [TestMethod]
        public void SimpleCppStructMemberSimplePaddingModule()
        {
            ReportModule report = new ReportModule(ReportKind.Input, null);
            PaddingModule padding = new PaddingModule(4, report);

            CppStructMemberSimple cppStructMemberSimple = new CppStructMemberSimple(padding);

            Assert.AreEqual("Padding", cppStructMemberSimple.Name);
            Assert.IsNull(cppStructMemberSimple.InitialValue);
            Assert.AreEqual(1, cppStructMemberSimple.ArraySize);
            Assert.AreEqual(CppFieldPrimativeDataType.uint8_t, cppStructMemberSimple.Type);
        }
    }
}
