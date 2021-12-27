// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidSpecification;

    [TestClass]
    public class UsageRangeTagTests
    {
        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"usageRange = ['Button', 'Button 1', 'Button 3']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRangeTag tag = UsageRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usageRange");

            HidUsageKind[] usageKinds = new HidUsageKind[] { HidUsageKind.Sel, HidUsageKind.OOC, HidUsageKind.MC, HidUsageKind.OSC };

            Assert.AreEqual(tag.UsageStart.Name, "Button 1");
            Assert.AreEqual(tag.UsageStart.Id, 1);
            CollectionAssert.AreEqual(tag.UsageStart.Kinds, usageKinds);
            Assert.AreEqual(tag.UsageStart.Page.Kind, HidUsagePageKind.Generated);
            Assert.AreEqual(tag.UsageStart.Page.Name, "Button");
            Assert.AreEqual(tag.UsageStart.Page.Id, 9);

            Assert.AreEqual(tag.UsageEnd.Name, "Button 3");
            Assert.AreEqual(tag.UsageEnd.Id, 3);
            CollectionAssert.AreEqual(tag.UsageEnd.Kinds, usageKinds);
            Assert.AreEqual(tag.UsageEnd.Page.Kind, HidUsagePageKind.Generated);
            Assert.AreEqual(tag.UsageEnd.Page.Name, "Button");
            Assert.AreEqual(tag.UsageEnd.Page.Id, 9);
        }

        [TestMethod]
        public void SimpleTagCreationInteger()
        {
            string nonDecoratedString = @"usageRange = [9, 1, 3]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRangeTag tag = UsageRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "usageRange");

            HidUsageKind[] usageKinds = new HidUsageKind[] { HidUsageKind.Sel, HidUsageKind.OOC, HidUsageKind.MC, HidUsageKind.OSC };

            Assert.AreEqual(tag.UsageStart.Name, "Button 1");
            Assert.AreEqual(tag.UsageStart.Id, 1);
            CollectionAssert.AreEqual(tag.UsageStart.Kinds, usageKinds);
            Assert.AreEqual(tag.UsageStart.Page.Kind, HidUsagePageKind.Generated);
            Assert.AreEqual(tag.UsageStart.Page.Name, "Button");
            Assert.AreEqual(tag.UsageStart.Page.Id, 9);

            Assert.AreEqual(tag.UsageEnd.Name, "Button 3");
            Assert.AreEqual(tag.UsageEnd.Id, 3);
            CollectionAssert.AreEqual(tag.UsageEnd.Kinds, usageKinds);
            Assert.AreEqual(tag.UsageEnd.Page.Kind, HidUsagePageKind.Generated);
            Assert.AreEqual(tag.UsageEnd.Page.Name, "Button");
            Assert.AreEqual(tag.UsageEnd.Page.Id, 9);
        }

        [TestMethod]
        public void InvalidKeyParametersString()
        {
            // 2 string parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageRange = ['Button', 'Button 1']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 4 string parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageRange = ['Button', 'Button 1', 'Button 5', 'Button 4']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage/Page doesn't exist.
            {
                string nonDecoratedString = @"usageRange = ['Invalid', 'Button 1', 'Button 3']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyParametersInteger()
        {
            // 2 int parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageRange = [9, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // 4 int parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageRange = [9, 1, 3, 4]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage/Page doesn't exist.
            {
                string nonDecoratedString = @"usageRange = [0, 0, 0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage Page/Id out-of-bounds.
            {
                Int64 invalidHigh = ((Int64)UInt16.MaxValue) + 1;

                string nonDecoratedString = $"usageRange=[{invalidHigh}, 1, 3]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)UInt16.MinValue) - 1;

                string nonDecoratedString = $"usageRange=[1, {invalidLow}, 3]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageRangeTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidTag=['Button', 'Button 1', 'Button 3']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRangeTag tag = UsageRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"UsAgeRANGe=['Button', 'Button 1', 'Button 3']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageRangeTag tag = UsageRangeTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.NonDecoratedName, "UsAgeRANGe");
        }
    }
}
