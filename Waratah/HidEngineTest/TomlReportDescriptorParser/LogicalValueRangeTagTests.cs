// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class LogicalValueRangeTagTests
    {
        [TestMethod]
        public void SimpleTagCreationDecimal()
        {
            string nonDecoratedString = @"logicalValueRange = [0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalValueRangeTag tag = LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.Decimal);
            Assert.AreEqual(tag.Value.Minimum, 0);
            Assert.AreEqual(tag.Value.Maximum, 1);
        }

        [TestMethod]
        public void SimpleTagCreationMaxSigned()
        {
            string nonDecoratedString = @"logicalValueRange = 'maxSignedSizeRange'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalValueRangeTag tag = LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.MaxSignedSizeRange);
            Assert.IsNull(tag.Value.Minimum);
            Assert.IsNull(tag.Value.Maximum);
        }

        [TestMethod]
        public void SimpleTagCreationMaxUnsigned()
        {
            string nonDecoratedString = @"logicalValueRange = 'maxUnsignedSizeRange'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalValueRangeTag tag = LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.MaxUnsignedSizeRange);
            Assert.IsNull(tag.Value.Minimum);
            Assert.IsNull(tag.Value.Maximum);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            // 1 parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"logicalValueRange = [0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"logicalValueRange = [0, 1, 2]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Invalid string type (i.e. not 'maxSignedSizeRange')
            {
                string nonDecoratedString = @"logicalValueRange = 'NOTmaxSignedSizeRange'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidKey=[0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalValueRangeTag tag = LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"logicalVALUERange = [0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalValueRangeTag tag = LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.Decimal);
            Assert.AreEqual(tag.Value.Minimum, 0);
            Assert.AreEqual(tag.Value.Maximum, 1);
        }

        [TestMethod]
        public void ValueOutOfBounds()
        {
            {
                Int64 invalidHigh = ((Int64)HidConstants.LogicalMaximumValue) + 1;

                string nonDecoratedString = $"logicalValueRange = [0, {invalidHigh}]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)HidConstants.LogicalMinimumValue) - 1;

                string nonDecoratedString = $"logicalValueRange = [{invalidLow}, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => LogicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
