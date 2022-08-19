// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.HidTools.HidSpecification;

    [TestClass]
    public class ReportFlagsTagTests
    {
        delegate void TestDelegate(string s);
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"reportFlags=['constant']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            ReportFlagsTag reportFlagsTag = ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(reportFlagsTag);
            Assert.AreEqual(reportFlagsTag.NonDecoratedName, "reportFlags");

            Assert.AreEqual(HidConstants.MainDataItemModificationKind.Constant, reportFlagsTag.Value.ModificationKind.Value);
            Assert.IsNull(reportFlagsTag.Value.RelationKind);
            Assert.IsNull(reportFlagsTag.Value.ContingentKind);
            Assert.IsNull(reportFlagsTag.Value.LinearityKind);
            Assert.IsNull(reportFlagsTag.Value.MeaningfulDataKind);
            Assert.IsNull(reportFlagsTag.Value.WrappingKind);
            Assert.IsNull(reportFlagsTag.Value.PreferenceStateKind);
        }

        [TestMethod]
        public void SingleReportTagSet()
        {
            DescriptorModuleFlags ConstructModuleFlags(string flagName)
            {
                string nonDecoratedString = $"reportFlags=['{flagName}']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                ReportFlagsTag reportFlagsTag = ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0));
                Assert.IsNotNull(reportFlagsTag);

                return reportFlagsTag.Value;
            }

            Assert.AreEqual(HidConstants.MainDataItemModificationKind.Data, ConstructModuleFlags("data").ModificationKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemModificationKind.Constant, ConstructModuleFlags("constant").ModificationKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemRelationKind.Absolute, ConstructModuleFlags("absolute").RelationKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemRelationKind.Relative, ConstructModuleFlags("relative").RelationKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemWrappingKind.NoWrap, ConstructModuleFlags("noWrap").WrappingKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemWrappingKind.Wrap, ConstructModuleFlags("wrap").WrappingKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemLinearityKind.Linear, ConstructModuleFlags("linear").LinearityKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemLinearityKind.NonLinear, ConstructModuleFlags("nonLinear").LinearityKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemPreferenceStateKind.PreferredState, ConstructModuleFlags("preferredState").PreferenceStateKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemPreferenceStateKind.NoPreferredState, ConstructModuleFlags("noPreferredState").PreferenceStateKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemMeaningfulDataKind.NoNullPosition, ConstructModuleFlags("noNullPosition").MeaningfulDataKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemMeaningfulDataKind.NullState, ConstructModuleFlags("nullState").MeaningfulDataKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemVolatilityKind.NonVolatile, ConstructModuleFlags("nonVolatile").VolatilityKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemVolatilityKind.Volatile, ConstructModuleFlags("volatile").VolatilityKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemContingentKind.BitField, ConstructModuleFlags("bitField").ContingentKind.Value);
            Assert.AreEqual(HidConstants.MainDataItemContingentKind.BufferedBytes, ConstructModuleFlags("bufferedBytes").ContingentKind.Value);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            // No tags in array.
            {
                string nonDecoratedString = @"reportFlags=[]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Invalid tag in array.
            {
                string nonDecoratedString = @"reportFlags=['invalidtagname']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Duplicate tag.
            {
                string nonDecoratedString = @"reportFlags=['data', 'data']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidTag=['data']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            ReportFlagsTag tag = ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"rePortFLAGs=['data']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            ReportFlagsTag testTag = ReportFlagsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(HidConstants.MainDataItemModificationKind.Data, testTag.Value.ModificationKind);
        }
    }
}
