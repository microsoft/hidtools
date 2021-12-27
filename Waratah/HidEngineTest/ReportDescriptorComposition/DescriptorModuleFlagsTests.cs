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
    public class DescriptorModuleFlagsTests
    {
        [TestMethod]
        public void SimpleDescriptorModuleFlags()
        {
            DescriptorModuleFlags flags = new DescriptorModuleFlags();
            flags.ModificationKind = HidConstants.MainDataItemModificationKind.Data;

            Assert.AreEqual(HidConstants.MainDataItemModificationKind.Data, flags.ModificationKindInterpreted);
        }

        [TestMethod]
        public void EquivalentFlags()
        {
            DescriptorModuleFlags lhsFlags = new DescriptorModuleFlags();
            lhsFlags.ModificationKind = HidConstants.MainDataItemModificationKind.Data;

            DescriptorModuleFlags rhsFlags = new DescriptorModuleFlags();
            // All flags are null.

            Assert.IsTrue(DescriptorModuleFlags.IsEquivalentTo(lhsFlags, rhsFlags));
        }
    }
}
