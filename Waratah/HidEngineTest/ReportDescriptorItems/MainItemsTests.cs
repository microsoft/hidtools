// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorItems
{
    using HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using HidSpecification;

    [TestClass]
    public class MainItems
    {
        [TestMethod]
        public void ValidateInputItem()
        {
            InputItem inputItem = new InputItem();

            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x00 });

            inputItem.ModificationKind = HidConstants.MainDataItemModificationKind.Constant;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x01 });

            inputItem.GroupingKind = HidConstants.MainDataItemGroupingKind.Variable;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x03 });

            inputItem.RelationKind = HidConstants.MainDataItemRelationKind.Relative;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x07 });

            inputItem.WrappingKind = HidConstants.MainDataItemWrappingKind.Wrap;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x0F });

            inputItem.LinearityKind = HidConstants.MainDataItemLinearityKind.NonLinear;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x1F });

            inputItem.PreferenceStateKind = HidConstants.MainDataItemPreferenceStateKind.NoPreferredState;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x3F });

            inputItem.MeaningfulDataKind = HidConstants.MainDataItemMeaningfulDataKind.NullState;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x81, 0x7F });

            inputItem.ContingentKind = HidConstants.MainDataItemContingentKind.BufferedBytes;
            CollectionAssert.AreEqual(inputItem.WireRepresentation(), new byte[] { 0x82, 0x7F, 0x01 });
        }

        [TestMethod]
        public void ValidateOutputItem()
        {
            OutputItem outputItem = new OutputItem();

            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x00 });

            outputItem.ModificationKind = HidConstants.MainDataItemModificationKind.Constant;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x01 });

            outputItem.GroupingKind = HidConstants.MainDataItemGroupingKind.Variable;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x03 });

            outputItem.RelationKind = HidConstants.MainDataItemRelationKind.Relative;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x07 });

            outputItem.WrappingKind = HidConstants.MainDataItemWrappingKind.Wrap;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x0F });

            outputItem.LinearityKind = HidConstants.MainDataItemLinearityKind.NonLinear;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x1F });

            outputItem.PreferenceStateKind = HidConstants.MainDataItemPreferenceStateKind.NoPreferredState;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x3F });

            outputItem.MeaningfulDataKind = HidConstants.MainDataItemMeaningfulDataKind.NullState;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0x7F });

            outputItem.VolatilityKind = HidConstants.MainDataItemVolatilityKind.Volatile;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x91, 0xFF });

            outputItem.ContingentKind = HidConstants.MainDataItemContingentKind.BufferedBytes;
            CollectionAssert.AreEqual(outputItem.WireRepresentation(), new byte[] { 0x92, 0xFF, 0x01 });
        }

        [TestMethod]
        public void ValidateFeatureItem()
        {
            FeatureItem featureItem = new FeatureItem();

            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x00 });

            featureItem.ModificationKind = HidConstants.MainDataItemModificationKind.Constant;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x01 });

            featureItem.GroupingKind = HidConstants.MainDataItemGroupingKind.Variable;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x03 });

            featureItem.RelationKind = HidConstants.MainDataItemRelationKind.Relative;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x07 });

            featureItem.WrappingKind = HidConstants.MainDataItemWrappingKind.Wrap;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x0F });

            featureItem.LinearityKind = HidConstants.MainDataItemLinearityKind.NonLinear;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x1F });

            featureItem.PreferenceStateKind = HidConstants.MainDataItemPreferenceStateKind.NoPreferredState;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x3F });

            featureItem.MeaningfulDataKind = HidConstants.MainDataItemMeaningfulDataKind.NullState;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0x7F });

            featureItem.VolatilityKind = HidConstants.MainDataItemVolatilityKind.Volatile;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB1, 0xFF });

            featureItem.ContingentKind = HidConstants.MainDataItemContingentKind.BufferedBytes;
            CollectionAssert.AreEqual(featureItem.WireRepresentation(), new byte[] { 0xB2, 0xFF, 0x01 });
        }

        [TestMethod]
        public void ValidateCollectionItem()
        {
            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.Physical).WireRepresentation(), new byte[] { 0xA1, 0x00 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.Application).WireRepresentation(), new byte[] { 0xA1, 0x01 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.Logical).WireRepresentation(), new byte[] { 0xA1, 0x02 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.Report).WireRepresentation(), new byte[] { 0xA1, 0x03 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.NamedArray).WireRepresentation(), new byte[] { 0xA1, 0x04 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.UsageSwitch).WireRepresentation(), new byte[] { 0xA1, 0x05 });

            CollectionAssert.AreEqual(new CollectionItem(HidConstants.MainItemCollectionKind.UsageModifier).WireRepresentation(), new byte[] { 0xA1, 0x06 });

            CollectionAssert.AreEqual(new CollectionItem(0x80).WireRepresentation(), new byte[] { 0xA1, 0x80 });

            // Must be between 0x80 - 0xFF;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CollectionItem(0x7F));
        }

        [TestMethod]
        public void ValidateEndCollectionItem()
        {
            EndCollectionItem endCollectionItem = new EndCollectionItem();

            CollectionAssert.AreEqual(endCollectionItem.WireRepresentation(), new byte[] { 0xC0 });
        }
    }
}
