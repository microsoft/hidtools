// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class UsageRelationTagTests
    {
        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"usageRelation = ['Sensors', 'Biometric: Human Presence']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRelationTag tag = UsageRelationTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usageRelation");
            Assert.AreEqual(tag.Value.Name, "Biometric: Human Presence");
            Assert.AreEqual(tag.Value.Id, 0x0011);
            Assert.AreEqual(tag.Value.Kinds[1], Microsoft.HidTools.HidSpecification.HidUsageKind.CP);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Sensors");
            Assert.AreEqual(tag.Value.Page.Id, 0x20);
        }

        [TestMethod]
        public void SimpleTagCreationInteger()
        {
            string nonDecoratedString = @"usageRelation=[0x20, 0x11]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRelationTag tag = UsageRelationTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usageRelation");
            Assert.AreEqual(tag.Value.Name, "Biometric: Human Presence");
            Assert.AreEqual(tag.Value.Id, 0x0011);
            Assert.AreEqual(tag.Value.Kinds[1], Microsoft.HidTools.HidSpecification.HidUsageKind.CP);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Sensors");
            Assert.AreEqual(tag.Value.Page.Id, 0x20);
        }

        [TestMethod]
        public void InvalidKeyParametersString()
        {
            // 1 string parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usageRelation='Invalid Single String'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 string parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usageRelation=['Invalid', 'Triple', 'String']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                string nonDecoratedString = @"usageRelation=['Invalid', 'Usage']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage isn't a Sensor Usage.
            {
                string nonDecoratedString = @"usageRelation=['Generic Desktop', 'Pointer']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyParametersInteger()
        {
            // 1 int parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usageRelation=1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 int parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usageRelation=[1, 2, 3]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                // UsageId 0 is Reserved.
                string nonDecoratedString = @"usageRelation=[0, 0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage Page/Id out-of-bounds.
            {
                Int64 invalidHigh = ((Int64)UInt16.MaxValue) + 1;

                string nonDecoratedString = $"usageRelation=[{invalidHigh}, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)UInt16.MinValue) - 1;

                string nonDecoratedString = $"usageRelation=[1, {invalidLow}]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRelationTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidTag=['Lighting And Illumination', 'LampArray']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRelationTag tag = UsageRelationTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"uSaGeRELation = ['Sensors', 'Biometric: Human Presence']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRelationTag testTag = UsageRelationTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.NonDecoratedName, "uSaGeRELation");
        }
    }
}
