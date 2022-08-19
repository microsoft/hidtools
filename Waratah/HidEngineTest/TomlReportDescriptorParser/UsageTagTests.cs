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
    public class UsageTagTests
    {
        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"usage=['Lighting And Illumination', 'LampArray']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTag tag = UsageTag.TryParse(rawTomlTags.ElementAt(0), typeof(ApplicationCollectionTag));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usage");
            Assert.AreEqual(tag.Value.Name, "LampArray");
            Assert.AreEqual(tag.Value.Id, 1);
            Assert.AreEqual(tag.Value.Kinds[0], Microsoft.HidTools.HidSpecification.HidUsageKind.CA);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Lighting And Illumination");
            Assert.AreEqual(tag.Value.Page.Id, 89);
        }

        [TestMethod]
        public void SimpleTagCreationInteger()
        {
            string nonDecoratedString = @"usage=[89, 1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTag tag = UsageTag.TryParse(rawTomlTags.ElementAt(0), typeof(ApplicationCollectionTag));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usage");
            Assert.AreEqual(tag.Value.Name, "LampArray");
            Assert.AreEqual(tag.Value.Id, 1);
            Assert.AreEqual(tag.Value.Kinds[0], Microsoft.HidTools.HidSpecification.HidUsageKind.CA);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Lighting And Illumination");
            Assert.AreEqual(tag.Value.Page.Id, 89);
        }

        [TestMethod]
        public void InvalidKeyParametersString()
        {
            // 1 string parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usage='Invalid Single String'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 string parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usage=['Invalid', 'Triple', 'String']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                string nonDecoratedString = @"usage=['Invalid', 'Usage']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyParametersInteger()
        {
            // 1 int parameter. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usage=1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 3 int parameters. (2 and only 2 are permitted).
            {
                string nonDecoratedString = @"usage=[1, 2, 3]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                // UsageId 0 is Reserved.
                string nonDecoratedString = @"usage=[0, 0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage Page/Id out-of-bounds.
            {
                Int64 invalidHigh = ((Int64)UInt16.MaxValue) + 1;

                string nonDecoratedString = $"usage=[{invalidHigh}, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)UInt16.MinValue) - 1;

                string nonDecoratedString = $"usage=[1, {invalidLow}]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidTag=['Lighting And Illumination', 'LampArray']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTag tag = UsageTag.TryParse(rawTomlTags.ElementAt(0), null);
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"UsAge=['Lighting And Illumination', 'LampArray']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTag testTag = UsageTag.TryParse(rawTomlTags.ElementAt(0), null);
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.NonDecoratedName, "UsAge");
        }
    }
}
