// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ElementDataVariableModuleImpl : BaseElementDataVariableModule
    {
        public ElementDataVariableModuleImpl(
            DescriptorRange logicalRange,
            DescriptorRange physicalRange,
            int? sizeInBits,
            DescriptorModuleFlags reportFlags,
            BaseModule parent)
                : base(logicalRange, physicalRange, sizeInBits, reportFlags, null, null, string.Empty, parent)
        {
        }

        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            throw new System.NotImplementedException();
        }

        public override List<BaseElementModule> GetReportElements()
        {
            throw new System.NotImplementedException();
        }
    }

    [TestClass]
    public class BaseElementDataVariableModuleTests
    {
        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        [TestInitialize]
        public void Initialize()
        {
            // Reset the Unit definitions after each test.
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleLogicalRange()
        {
            // LogicalRange defined, SizeInBytes not.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, null, report);

                Assert.AreEqual(0, element.LogicalMinimum);
                Assert.AreEqual(10, element.LogicalMaximum);
                Assert.AreEqual(4, element.SizeInBits);
            }

            // LogicalRange not defined, SizeInBytes defined.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(null, null, 4, null, report);

                Assert.AreEqual(-8, element.LogicalMinimum);
                Assert.AreEqual(7, element.LogicalMaximum);
            }
        }

        [TestMethod]
        public void LogicalRangeStartAndEndAreEqual()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(10, 10);
            BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, null, report);

            Assert.AreEqual(10, element.LogicalMinimum);
            Assert.AreEqual(10, element.LogicalMaximum);
            Assert.AreEqual(4, element.SizeInBits);
        }

        [TestMethod]
        public void LogicalRangeMaxSigned()
        {
            // When RangeMax specified, Min/Max are signed min/max for size.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, 4, null, report);

                Assert.AreEqual(element.LogicalMinimum, -8);
                Assert.AreEqual(element.LogicalMaximum, 7);
            }

            // Boundary cases - 32bit int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, 32, null, report);

                Assert.AreEqual(element.LogicalMinimum, HidConstants.LogicalMinimumValue);
                Assert.AreEqual(element.LogicalMaximum, HidConstants.LogicalMaximumValue);
            }

            // Boundary cases - 0 int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, 0, null, report));
            }

            // Boundary cases -ve int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, -1, null, report));
            }

            // Boundary cases, > 32bit int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, 33, null, report));
            }
        }

        public void LogicalRangeMaxUnsigned()
        {
            // When RangeMax specified, Min/Max are unsigned min/max for size.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, 4, null, report);

                Assert.AreEqual(element.LogicalMinimum, 0);
                Assert.AreEqual(element.LogicalMaximum, 15);
            }

            // Boundary cases - 32bit int (same range as 31bit)
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, 32, null, report);

                Assert.AreEqual(element.LogicalMinimum, 0);
                Assert.AreEqual(element.LogicalMaximum, HidConstants.LogicalMaximumValue);
            }

            // Boundary cases - 0 int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, 0, null, report));
            }

            // Boundary cases -ve int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, -1, null, report));
            }

            // Boundary cases, > 32bit int
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(DescriptorRangeKind.MaxUnsignedSizeRange);
                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, 33, null, report));
            }
        }

        [TestMethod]
        public void LogicalRangeAndSize()
        {
            // Cannot specify both range and size.

            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, 4, null, report));
        }

        [TestMethod]
        public void ReportSizeInBits()
        {
            int SizeInBitsForRange(int start, int end)
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);
                DescriptorRange logicalRange = new DescriptorRange(start, end);
                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, null, report);

                return element.SizeInBits;
            }

            // Quick Integer.
            int QInt(int exponent, bool isPositive = true)
            {
                if (isPositive)
                {
                    return (int)(Math.Pow(2, exponent) - 1);
                }
                else
                {
                    return (int)(Math.Pow(2, exponent) * -1);
                }
            }

            // Positive ranges.
            {
                Assert.AreEqual(1, SizeInBitsForRange(0, 1));
                Assert.AreEqual(2, SizeInBitsForRange(0, 2));
                Assert.AreEqual(4, SizeInBitsForRange(0, 10));

                // Notice here how the increasing the value of RangeStart doesn't change the size of the HidConstants.Item.
                Assert.AreEqual(2, SizeInBitsForRange(1, 2));
                Assert.AreEqual(4, SizeInBitsForRange(9, 10));
            }

            // Range with 1 -ve
            {
                Assert.AreEqual(2, SizeInBitsForRange(-1, 1));
                Assert.AreEqual(5, SizeInBitsForRange(-1, 10));
                Assert.AreEqual(5, SizeInBitsForRange(-10, 10));
            }

            // Negative ranges.
            {
                Assert.AreEqual(2, SizeInBitsForRange(-2, -1));
                Assert.AreEqual(5, SizeInBitsForRange(-10, -1));

                // Notice here how the decreasing the value of RangeEnd doesn't change the size of the HidConstants.Item.
                Assert.AreEqual(5, SizeInBitsForRange(-10, -9));
            }

            {
                Assert.AreEqual(8, SizeInBitsForRange(-128, 127));

            }

            // Boundary cases
            {
                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue, Int32.MaxValue));
                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue + 1, Int32.MaxValue));
                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue + 1, Int32.MaxValue - 1));

                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue, 0));
                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue, -1));
                Assert.AreEqual(32, SizeInBitsForRange(Int32.MinValue, 1));

                Assert.AreEqual(31, SizeInBitsForRange(0, Int32.MaxValue));
                Assert.AreEqual(31, SizeInBitsForRange(1, Int32.MaxValue));
                Assert.AreEqual(31, SizeInBitsForRange(Int32.MaxValue - 1, Int32.MaxValue));
                // Notice how RangeStart changing from 0 to -1, increased the required bits.
                Assert.AreEqual(32, SizeInBitsForRange(-1, Int32.MaxValue));

                Assert.AreEqual(31, SizeInBitsForRange(0, QInt(31)));
                Assert.AreEqual(30, SizeInBitsForRange(0, QInt(30)));
                Assert.AreEqual(31, SizeInBitsForRange(0, QInt(30) + 1));

                Assert.AreEqual(31, SizeInBitsForRange(QInt(30, false) + 1, 0));
                Assert.AreEqual(31, SizeInBitsForRange(QInt(30, false), 0));
                Assert.AreEqual(32, SizeInBitsForRange(QInt(30, false) - 1, 0));
            }
        }

        [TestMethod]
        public void SimplePhysicalRange()
        {
            // Golden Path.

            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            DescriptorRange physicalRange = new DescriptorRange(0, 20);
            BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, physicalRange, null, null, report);

            Assert.AreEqual(0, element.PhysicalMinimum);
            Assert.AreEqual(20, element.PhysicalMaximum);

            Assert.AreEqual(0, element.PhysicalMinimumInterpreted);
            Assert.AreEqual(20, element.PhysicalMaximumInterpreted);
        }

        [TestMethod]
        public void PhysicalRangeUndefinedIsLogicalRange()
        {
            // Undefined PhysicalRange is the same as LogicalRange.

            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, null, report);

            Assert.AreEqual(0, element.PhysicalMinimum);
            Assert.AreEqual(0, element.PhysicalMaximum);

            Assert.AreEqual(0, element.PhysicalMinimumInterpreted);
            Assert.AreEqual(10, element.PhysicalMaximumInterpreted);
        }

        [TestMethod]
        public void PhysicalRangeCannotSpecifyNonDecimalKind()
        {
            // Cannot specify ReportRange with non-Decimal Kind.

            ReportModule report = new ReportModule(DefaultReportKind, null);
            DescriptorRange logicalRange = new DescriptorRange(0, 10);
            DescriptorRange physicalRange = new DescriptorRange(DescriptorRangeKind.MaxSignedSizeRange);

            Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, physicalRange, null, null, report));
        }

        [TestMethod]
        public void InvalidModuleFlagsForReportKind()
        {
            // Cannot specify NonVolatile Flag for Input Report.
            {
                ReportModule report = new ReportModule(ReportKind.Input, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.VolatilityKind = HidConstants.MainDataItemVolatilityKind.NonVolatile;

                Assert.ThrowsException<DescriptorModuleParsingException>(() => new ElementDataVariableModuleImpl(logicalRange, null, null, flags, report));
            }

            // Can specify NonVolatile FLag for Output Reports
            {
                ReportModule report = new ReportModule(ReportKind.Output, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.VolatilityKind = HidConstants.MainDataItemVolatilityKind.NonVolatile;

                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, flags, report);
            }

            // Can specify NonVolatile FLag for Feature Reports
            {
                ReportModule report = new ReportModule(ReportKind.Feature, null);
                DescriptorRange logicalRange = new DescriptorRange(0, 10);
                DescriptorModuleFlags flags = new DescriptorModuleFlags();
                flags.VolatilityKind = HidConstants.MainDataItemVolatilityKind.NonVolatile;

                BaseElementDataVariableModule element = new ElementDataVariableModuleImpl(logicalRange, null, null, flags, report);
            }
        }
    }
}
