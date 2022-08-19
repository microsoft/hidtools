// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorItems
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Microsoft.HidTools.HidSpecification;

    [TestClass]
    public class GlobalItemsTests
    {
        /// <summary>
        /// For the purposes of accurate test execution times, this test will run 'first' to account
        /// for the time to load the modules under test (as well as the test framework).
        /// </summary>
        [TestMethod]
        public void ValidateUsagePageItem()
        {
            CollectionAssert.AreEqual(new UsagePageItem(0x85).WireRepresentation(), new byte[] { 0x05, 0x85 });

            CollectionAssert.AreEqual(new UsagePageItem(0xFF).WireRepresentation(), new byte[] { 0x05, 0xFF });

            CollectionAssert.AreEqual(new UsagePageItem(0x100).WireRepresentation(), new byte[] { 0x06, 0x00, 0x01 });

            CollectionAssert.AreEqual(new UsagePageItem(0xFFFF).WireRepresentation(), new byte[] { 0x06, 0xFF, 0xFF });
        }

        [TestMethod]
        public void ValidateLogicalMinimumItem()
        {
            CollectionAssert.AreEqual(new LogicalMinimumItem(0).WireRepresentation(), new byte[] { 0x15, 0x00 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-1).WireRepresentation(), new byte[] { 0x15, 0xFF });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-3).WireRepresentation(), new byte[] { 0x15, 0xFD });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-127).WireRepresentation(), new byte[] { 0x15, 0x81 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-129).WireRepresentation(), new byte[] { 0x16, 0x7F, 0xFF });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-32768).WireRepresentation(), new byte[] { 0x16, 0x00, 0x80 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-32769).WireRepresentation(), new byte[] { 0x17, 0xFF, 0x7F, 0xFF, 0xFF });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-16777217).WireRepresentation(), new byte[] { 0x17, 0xFF, 0xFF, 0xFF, 0xFE });

            CollectionAssert.AreEqual(new LogicalMinimumItem(-2147483647).WireRepresentation(), new byte[] { 0x17, 0x01, 0x00, 0x00, 0x80 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(1).WireRepresentation(), new byte[] { 0x15, 0x01 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(3).WireRepresentation(), new byte[] { 0x15, 0x03 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(127).WireRepresentation(), new byte[] { 0x15, 0x7F });

            CollectionAssert.AreEqual(new LogicalMinimumItem(128).WireRepresentation(), new byte[] { 0x16, 0x80, 0x00 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(32767).WireRepresentation(), new byte[] { 0x16, 0xFF, 0x7F });

            CollectionAssert.AreEqual(new LogicalMinimumItem(32768).WireRepresentation(), new byte[] { 0x17, 0x00, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(32769).WireRepresentation(), new byte[] { 0x17, 0x01, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(16777215).WireRepresentation(), new byte[] { 0x17, 0xFF, 0xFF, 0xFF, 0x00 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(16777217).WireRepresentation(), new byte[] { 0x17, 0x01, 0x00, 0x00, 0x01 });

            CollectionAssert.AreEqual(new LogicalMinimumItem(2147483647).WireRepresentation(), new byte[] { 0x17, 0xFF, 0xFF, 0xFF, 0x7F });
        }

        [TestMethod]
        public void ValidateLogicalMaximumItem()
        {
            CollectionAssert.AreEqual(new LogicalMaximumItem(0).WireRepresentation(), new byte[] { 0x25, 0x00 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-1).WireRepresentation(), new byte[] { 0x25, 0xFF });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-3).WireRepresentation(), new byte[] { 0x25, 0xFD });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-127).WireRepresentation(), new byte[] { 0x25, 0x81 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-129).WireRepresentation(), new byte[] { 0x26, 0x7F, 0xFF });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-32768).WireRepresentation(), new byte[] { 0x26, 0x00, 0x80 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-32769).WireRepresentation(), new byte[] { 0x27, 0xFF, 0x7F, 0xFF, 0xFF });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-16777217).WireRepresentation(), new byte[] { 0x27, 0xFF, 0xFF, 0xFF, 0xFE });

            CollectionAssert.AreEqual(new LogicalMaximumItem(-2147483647).WireRepresentation(), new byte[] { 0x27, 0x01, 0x00, 0x00, 0x80 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(1).WireRepresentation(), new byte[] { 0x25, 0x01 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(3).WireRepresentation(), new byte[] { 0x25, 0x03 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(127).WireRepresentation(), new byte[] { 0x25, 0x7F });

            CollectionAssert.AreEqual(new LogicalMaximumItem(128).WireRepresentation(), new byte[] { 0x26, 0x80, 0x00 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(32767).WireRepresentation(), new byte[] { 0x26, 0xFF, 0x7F });

            CollectionAssert.AreEqual(new LogicalMaximumItem(32768).WireRepresentation(), new byte[] { 0x27, 0x00, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(32769).WireRepresentation(), new byte[] { 0x27, 0x01, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(16777215).WireRepresentation(), new byte[] { 0x27, 0xFF, 0xFF, 0xFF, 0x00 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(16777217).WireRepresentation(), new byte[] { 0x27, 0x01, 0x00, 0x00, 0x01 });

            CollectionAssert.AreEqual(new LogicalMaximumItem(2147483647).WireRepresentation(), new byte[] { 0x27, 0xFF, 0xFF, 0xFF, 0x7F });
        }

        [TestMethod]
        public void ValidatePhysicalMinimumItem()
        {
            CollectionAssert.AreEqual(new PhysicalMinimumItem(0).WireRepresentation(), new byte[] { 0x35, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-1).WireRepresentation(), new byte[] { 0x35, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-3).WireRepresentation(), new byte[] { 0x35, 0xFD });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-127).WireRepresentation(), new byte[] { 0x35, 0x81 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-129).WireRepresentation(), new byte[] { 0x36, 0x7F, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-32768).WireRepresentation(), new byte[] { 0x36, 0x00, 0x80 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-32769).WireRepresentation(), new byte[] { 0x37, 0xFF, 0x7F, 0xFF, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-16777217).WireRepresentation(), new byte[] { 0x37, 0xFF, 0xFF, 0xFF, 0xFE });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(-2147483647).WireRepresentation(), new byte[] { 0x37, 0x01, 0x00, 0x00, 0x80 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(1).WireRepresentation(), new byte[] { 0x35, 0x01 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(3).WireRepresentation(), new byte[] { 0x35, 0x03 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(127).WireRepresentation(), new byte[] { 0x35, 0x7F });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(128).WireRepresentation(), new byte[] { 0x36, 0x80, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(32767).WireRepresentation(), new byte[] { 0x36, 0xFF, 0x7F });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(32768).WireRepresentation(), new byte[] { 0x37, 0x00, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(32769).WireRepresentation(), new byte[] { 0x37, 0x01, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(16777215).WireRepresentation(), new byte[] { 0x37, 0xFF, 0xFF, 0xFF, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(16777217).WireRepresentation(), new byte[] { 0x37, 0x01, 0x00, 0x00, 0x01 });

            CollectionAssert.AreEqual(new PhysicalMinimumItem(2147483647).WireRepresentation(), new byte[] { 0x37, 0xFF, 0xFF, 0xFF, 0x7F });
        }

        [TestMethod]
        public void ValidatePhysicalMaximumItem()
        {
            CollectionAssert.AreEqual(new PhysicalMaximumItem(0).WireRepresentation(), new byte[] { 0x45, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-1).WireRepresentation(), new byte[] { 0x45, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-3).WireRepresentation(), new byte[] { 0x45, 0xFD });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-127).WireRepresentation(), new byte[] { 0x45, 0x81 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-129).WireRepresentation(), new byte[] { 0x46, 0x7F, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-32768).WireRepresentation(), new byte[] { 0x46, 0x00, 0x80 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-32769).WireRepresentation(), new byte[] { 0x47, 0xFF, 0x7F, 0xFF, 0xFF });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-16777217).WireRepresentation(), new byte[] { 0x47, 0xFF, 0xFF, 0xFF, 0xFE });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(-2147483647).WireRepresentation(), new byte[] { 0x47, 0x01, 0x00, 0x00, 0x80 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(1).WireRepresentation(), new byte[] { 0x45, 0x01 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(3).WireRepresentation(), new byte[] { 0x45, 0x03 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(127).WireRepresentation(), new byte[] { 0x45, 0x7F });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(128).WireRepresentation(), new byte[] { 0x46, 0x80, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(32767).WireRepresentation(), new byte[] { 0x46, 0xFF, 0x7F });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(32768).WireRepresentation(), new byte[] { 0x47, 0x00, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(32769).WireRepresentation(), new byte[] { 0x47, 0x01, 0x80, 0x00, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(16777215).WireRepresentation(), new byte[] { 0x47, 0xFF, 0xFF, 0xFF, 0x00 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(16777217).WireRepresentation(), new byte[] { 0x47, 0x01, 0x00, 0x00, 0x01 });

            CollectionAssert.AreEqual(new PhysicalMaximumItem(2147483647).WireRepresentation(), new byte[] { 0x47, 0xFF, 0xFF, 0xFF, 0x7F });
        }

        [TestMethod]
        public void ValidateUnitExponentItem()
        {
            // Valid values are between -8 and 7

            CollectionAssert.AreEqual(new UnitExponentItem(1).WireRepresentation(), new byte[] { 0x55, 0x00 });

            CollectionAssert.AreEqual(new UnitExponentItem(10).WireRepresentation(), new byte[] { 0x55, 0x01 });

            CollectionAssert.AreEqual(new UnitExponentItem(100).WireRepresentation(), new byte[] { 0x55, 0x02 });

            CollectionAssert.AreEqual(new UnitExponentItem(1_000).WireRepresentation(), new byte[] { 0x55, 0x03 });

            CollectionAssert.AreEqual(new UnitExponentItem(10_000).WireRepresentation(), new byte[] { 0x55, 0x04 });

            CollectionAssert.AreEqual(new UnitExponentItem(100_000).WireRepresentation(), new byte[] { 0x55, 0x05 });

            CollectionAssert.AreEqual(new UnitExponentItem(1000_000).WireRepresentation(), new byte[] { 0x55, 0x06 });

            CollectionAssert.AreEqual(new UnitExponentItem(10_000_000).WireRepresentation(), new byte[] { 0x55, 0x07 });

            CollectionAssert.AreEqual(new UnitExponentItem(0.1).WireRepresentation(), new byte[] { 0x55, 0x0F });

            CollectionAssert.AreEqual(new UnitExponentItem(0.01).WireRepresentation(), new byte[] { 0x55, 0x0E });

            CollectionAssert.AreEqual(new UnitExponentItem(0.001).WireRepresentation(), new byte[] { 0x55, 0x0D });

            CollectionAssert.AreEqual(new UnitExponentItem(0.000_1).WireRepresentation(), new byte[] { 0x55, 0x0C });

            CollectionAssert.AreEqual(new UnitExponentItem(0.000_01).WireRepresentation(), new byte[] { 0x55, 0x0B });

            CollectionAssert.AreEqual(new UnitExponentItem(0.000_001).WireRepresentation(), new byte[] { 0x55, 0x0A });

            CollectionAssert.AreEqual(new UnitExponentItem(0.000_000_1).WireRepresentation(), new byte[] { 0x55, 0x09 });

            CollectionAssert.AreEqual(new UnitExponentItem(0.000_000_01).WireRepresentation(), new byte[] { 0x55, 0x08 });

            Assert.ThrowsException<HidSpecificationException>(() => new UnitExponentItem(0.000_000_001).WireRepresentation());

            Assert.ThrowsException<HidSpecificationException>(() => new UnitExponentItem(100_000_000).WireRepresentation());
        }

        [TestMethod]
        public void ValidateUnitItem()
        {
            {
                HidUnit unit = new HidUnit("unit", 2, 0, 0, 0, 0, 0, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.SiLinear });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x65, 0x21 });
            }

            {
                HidUnit unit = new HidUnit("unit", 2, 0, 0, 0, 0, 0, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.EnglishRotation });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x65, 0x24 });
            }

            {
                HidUnit unit = new HidUnit("unit", 0, 3, 0, 0, 0, 0, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.SiLinear });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x66, 0x01, 0x03});
            }

            {
                HidUnit unit = new HidUnit("unit", 0, 0, 0, 0, 0, 7, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.SiLinear });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x67, 0x01, 0x00, 0x00, 0x07 });
            }

            {
                HidUnit unit = new HidUnit("unit", 2, 3, 4, 5, 6, 7, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.SiLinear });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x67, 0x21, 0x43, 0x65, 0x07 });
            }

            {
                HidUnit unit = new HidUnit("unit", 2, 3, 4, 5, 6, 7, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.SiRotation });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x67, 0x22, 0x43, 0x65, 0x07 });
            }

            {
                HidUnit unit = new HidUnit("unit", 2, 3, 4, 5, 6, 7, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.EnglishLinear });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x67, 0x23, 0x43, 0x65, 0x07 });
            }

            {
                HidUnit unit = new HidUnit("unit", 2, 3, 4, 5, 6, 7, 1, new HidConstants.UnitItemSystemKind[] { HidConstants.UnitItemSystemKind.EnglishRotation });

                UnitItem unitItem = new UnitItem(unit);

                CollectionAssert.AreEqual(unitItem.WireRepresentation(), new byte[] { 0x67, 0x24, 0x43, 0x65, 0x07 });
            }
        }

        [TestMethod]
        public void ValidateReportSizeItem()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportSizeItem(0));

            CollectionAssert.AreEqual(new ReportSizeItem(1).WireRepresentation(), new byte[] { 0x75, 0x01 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportSizeItem(65536));
        }

        [TestMethod]
        public void ValidateReportIdItem()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportIdItem(0));

            CollectionAssert.AreEqual(new ReportIdItem(1).WireRepresentation(), new byte[] { 0x85, 0x01 });

            CollectionAssert.AreEqual(new ReportIdItem(255).WireRepresentation(), new byte[] { 0x85, 0xFF });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportIdItem(256));
        }

        [TestMethod]
        public void ValidateReportCountItem()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportCountItem(0));

            CollectionAssert.AreEqual(new ReportCountItem(1).WireRepresentation(), new byte[] { 0x95, 0x01 });

            CollectionAssert.AreEqual(new ReportCountItem(255).WireRepresentation(), new byte[] { 0x95, 0xFF });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReportCountItem(65536));
        }

        [TestMethod]
        public void ValidatePushItem()
        {
            PushItem pushItem = new PushItem();

            CollectionAssert.AreEqual(pushItem.WireRepresentation(), new byte[] { 0xA4 });
        }

        [TestMethod]
        public void ValidatePopItem()
        {
            PopItem popItem = new PopItem();

            CollectionAssert.AreEqual(popItem.WireRepresentation(), new byte[] { 0xB4 });
        }
    }
}
