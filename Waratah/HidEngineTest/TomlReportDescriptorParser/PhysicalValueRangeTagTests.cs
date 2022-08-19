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
    public class PhysicalValueRangeTagTests
    {
        [TestMethod]
        public void SimpleTagCreationDecimal()
        {
            string nonDecoratedString = @"physicalValueRange = [0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PhysicalValueRangeTag tag = PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.Decimal);
            Assert.AreEqual(tag.Value.Minimum, 0);
            Assert.AreEqual(tag.Value.Maximum, 1);
        }

        [TestMethod]
        public void SimpleTagCreationMaxSigned()
        {
            string nonDecoratedString = @"physicalValueRange = 'maxSignedSizeRange'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void SimpleTagCreationMaxUnsigned()
        {
            string nonDecoratedString = @"physicalValueRange = 'maxUnsignedSizeRange'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            // 1 parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"physicalValueRange = [0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"physicalValueRange = [0, 1, 2]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Invalid string type (i.e. not 'maxSignedSizeRange')
            {
                string nonDecoratedString = @"physicalValueRange = 'NOTmaxSignedSizeRange'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidKey=[0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PhysicalValueRangeTag tag = PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"physicalVALUERange = [0, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PhysicalValueRangeTag tag = PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Kind, DescriptorRangeKind.Decimal);
            Assert.AreEqual(tag.Value.Minimum, 0);
            Assert.AreEqual(tag.Value.Maximum, 1);
        }

        [TestMethod]
        public void ValueOutOfBounds()
        {
            {
                Int64 invalidHigh = ((Int64)HidConstants.PhysicalMaximumValue) + 1;

                string nonDecoratedString = $"physicalValueRange = [0, {invalidHigh}]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)HidConstants.PhysicalMinimumValue) - 1;

                string nonDecoratedString = $"physicalValueRange = [{invalidLow}, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PhysicalValueRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
