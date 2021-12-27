// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.CppGenerator
{
    using HidEngine;
    using HidEngine.CppGenerator;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class CppStructMemberEnumTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        private static readonly HidUsageId DefaultUsageStart = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 1");
        private static readonly HidUsageId DefaultUsageEnd = HidUsageTableDefinitions.GetInstance().TryFindUsageId("Button", "Button 3");

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
        public void SimpleCppStructMemberEnum()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, null, report);

            CppStructMemberEnum cppStructMemberEnum = new CppStructMemberEnum(array);

            Assert.AreEqual("ButtonArrayItem", cppStructMemberEnum.Name);
            Assert.AreEqual(1, cppStructMemberEnum.ElementCount);
            Assert.AreEqual("ButtonArrayValues", cppStructMemberEnum.Enumerator.Name);
            Assert.AreEqual("ButtonArrayValues", cppStructMemberEnum.Enumerator.TypeName);
            Assert.AreEqual(3, cppStructMemberEnum.Enumerator.Values.Count);
            Assert.AreEqual(CppFieldPrimativeDataType.uint8_t, cppStructMemberEnum.Enumerator.EnumType);
        }

        /// <summary>
        /// Supplied name of ArrayModule must be respected (rather than the default/auto-generated name).
        /// </summary>
        [TestMethod]
        public void NonDefaultName()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 1, null, "TestName", report);

            CppStructMemberEnum cppStructMemberEnum = new CppStructMemberEnum(array);

            Assert.AreEqual("TestNameArrayValues", cppStructMemberEnum.Enumerator.Name);
        }

        /// <summary>
        /// When the generated member is a CPP array, the name must be suffix with an 's' (for plurality).
        /// </summary>
        [TestMethod]
        public void HasSuffixWhenMultipleElements()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            // Count == 2, means there will be an array of size 2.
            ArrayModule array = new ArrayModule(DefaultUsageStart, DefaultUsageEnd, 2, null, null, report);

            CppStructMemberEnum cppStructMemberEnum = new CppStructMemberEnum(array);

            // Suffix of s is added when multiple items.
            Assert.AreEqual("ButtonArrayItems", cppStructMemberEnum.Name);
            Assert.AreEqual(2, cppStructMemberEnum.ElementCount);
        }
    }
}
