// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorItems
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class LocalItemsTests
    {
        [TestMethod]
        public void ValidateUsageItem()
        {
            CollectionAssert.AreEqual(new UsageItem(0xFF, 0x01, false).WireRepresentation(), new byte[] { 0x09, 0x01 });

            CollectionAssert.AreEqual(new UsageItem(0x05, 0x01, true).WireRepresentation(), new byte[] { 0x0B, 0x01, 0x00, 0x05, 0x00 });
        }

        [TestMethod]
        public void ValidateUsageMinimumItem()
        {
            CollectionAssert.AreEqual(new UsageMinimumItem(0xFF, 0x01, false).WireRepresentation(), new byte[] { 0x19, 0x01 });

            CollectionAssert.AreEqual(new UsageMinimumItem(0x05, 0x01, true).WireRepresentation(), new byte[] { 0x1B, 0x01, 0x00, 0x05, 0x00 });
        }

        [TestMethod]
        public void ValidateUsageMaximumItem()
        {
            CollectionAssert.AreEqual(new UsageMaximumItem(0xFF, 0x01, false).WireRepresentation(), new byte[] { 0x29, 0x01 });

            CollectionAssert.AreEqual(new UsageMaximumItem(0x05, 0x01, true).WireRepresentation(), new byte[] { 0x2B, 0x01, 0x00, 0x05, 0x00 });
        }

        [TestMethod]
        public void ValidateDesignatorIndexItem()
        {
            CollectionAssert.AreEqual(new DesignatorIndexItem(0x01).WireRepresentation(), new byte[] { 0x39, 0x01 });

            CollectionAssert.AreEqual(new DesignatorIndexItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x3B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateDesignatorMinimumItem()
        {
            CollectionAssert.AreEqual(new DesignatorMinimumItem(0x01).WireRepresentation(), new byte[] { 0x49, 0x01 });

            CollectionAssert.AreEqual(new DesignatorMinimumItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x4B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateDesignatorMaximumItem()
        {
            CollectionAssert.AreEqual(new DesignatorMaximumItem(0x01).WireRepresentation(), new byte[] { 0x59, 0x01 });

            CollectionAssert.AreEqual(new DesignatorMaximumItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x5B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateStringIndexItem()
        {
            CollectionAssert.AreEqual(new StringIndexItem(0x01).WireRepresentation(), new byte[] { 0x79, 0x01 });

            CollectionAssert.AreEqual(new StringIndexItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x7B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateStringMinimumItem()
        {
            CollectionAssert.AreEqual(new StringMinimumItem(0x01).WireRepresentation(), new byte[] { 0x89, 0x01 });

            CollectionAssert.AreEqual(new StringMinimumItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x8B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateStringMaximumItem()
        {
            CollectionAssert.AreEqual(new StringMaximumItem(0x01).WireRepresentation(), new byte[] { 0x99, 0x01 });

            CollectionAssert.AreEqual(new StringMaximumItem(UInt32.MaxValue).WireRepresentation(), new byte[] { 0x9B, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateDelimiter()
        {
            CollectionAssert.AreEqual(new DelimiterItem(false).WireRepresentation(), new byte[] { 0xA9, 0x00 });

            CollectionAssert.AreEqual(new DelimiterItem(true).WireRepresentation(), new byte[] { 0xA9, 0x01 });
        }
    }
}
