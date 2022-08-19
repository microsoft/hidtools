// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorComposition
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    public class ElementModuleImpl : BaseElementModule
    {
        public ElementModuleImpl(BaseModule parent) : base(parent)
        {
        }

        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            throw new System.NotImplementedException();
        }

        public override List<BaseElementModule> GetReportElements()
        {
            throw new NotImplementedException();
        }

        public void SetCount(int count)
        {
            this.Count = count;
        }

        public void SetSizeInBits(int sizeInBits)
        {
            this.NonAdjustedSizeInBits = sizeInBits;
        }
    }

    [TestClass]
    public class BaseElementModuleTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Utils.GlobalReset();
        }

        private static readonly ReportKind DefaultReportKind = ReportKind.Input;

        [TestMethod]
        public void SimpleBaseReportElement()
        {
            ReportModule report = new ReportModule(DefaultReportKind, null);

            ElementModuleImpl element = new ElementModuleImpl(report);

            Assert.AreEqual(ReportKind.Input, element.ParentReportKind);
        }

        [TestMethod]
        public void InvalidSizeInBits()
        {
            // Simple case, valid size.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);
                element.SetSizeInBits(10);

                Assert.AreEqual(10, element.SizeInBits);
            }

            // Size is 0
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => element.SetSizeInBits(0));
            }

            // Size is -ve
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => element.SetSizeInBits(-1));
            }

            // Size is too big
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => element.SetSizeInBits(33));
            }
        }

        [TestMethod]
        public void InvalidCount()
        {
            // Simple case, valid count.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);
                element.SetCount(1);

                Assert.AreEqual(1, element.Count);
            }

            // Count is -ve
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => element.SetCount(-1));
            }

            // Count is too big.
            {
                ReportModule report = new ReportModule(DefaultReportKind, null);

                ElementModuleImpl element = new ElementModuleImpl(report);

                Assert.ThrowsException<DescriptorModuleParsingException>(() => element.SetCount(UInt16.MaxValue + 1));
            }
        }

        [TestMethod]
        public void NonAdjustedSizeValidation()
        {
            // Forces 1-byte alignment.
            Settings.GetInstance().PackingInBytes = 1;

            ReportModule report = new ReportModule(DefaultReportKind, null);

            ElementModuleImpl element = new ElementModuleImpl(report);
            element.SetSizeInBits(10);

            Assert.AreEqual(10, element.NonAdjustedSizeInBits);
            Assert.AreEqual(16, element.SizeInBits);
        }
    }
}
