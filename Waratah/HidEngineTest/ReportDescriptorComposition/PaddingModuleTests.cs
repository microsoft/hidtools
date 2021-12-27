// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using HidEngine.ReportDescriptorComposition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;

    [TestClass]
    public class PaddingModuleTests
    {
        [TestMethod]
        public void SimplePaddingModule()
        {
            ReportModule report = new ReportModule(ReportKind.Input, null);

            PaddingModule module = new PaddingModule(4, report);

            Assert.AreEqual(module.SizeInBits, 4);
        }
    }
}
