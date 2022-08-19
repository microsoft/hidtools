// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;

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
