// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DescriptorRangeTests
    {
        [TestMethod]
        public void SimpleRange()
        {
            {
                DescriptorRange range = new DescriptorRange(0, 10);

                Assert.AreEqual(0, range.Minimum);
                Assert.AreEqual(10, range.Maximum);
                Assert.AreEqual(DescriptorRangeKind.Decimal, range.Kind);
            }

            {
                DescriptorRange range = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);

                Assert.IsNull(range.Minimum);
                Assert.IsNull(range.Maximum);
                Assert.AreEqual(DescriptorRangeKind.MaxSignedSizeRange, range.Kind);
            }

            {
                DescriptorRange range = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);

                Assert.IsNull(range.Minimum);
                Assert.IsNull(range.Maximum);
                Assert.AreEqual(DescriptorRangeKind.MaxUnsignedSizeRange, range.Kind);
            }

            {
                DescriptorRange range = new DescriptorRange(10, 10);

                Assert.AreEqual(10, range.Minimum);
                Assert.AreEqual(10, range.Maximum);
                Assert.AreEqual(DescriptorRangeKind.Decimal, range.Kind);
            }
        }

        [TestMethod]
        public void InvalidRange()
        {
            // Range max must always be >= min.
            Assert.ThrowsException<DescriptorModuleParsingException>(() => new DescriptorRange(10, 0));
        }
    }
}
