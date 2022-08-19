// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class UsageKindsTagTest
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"types = ['Sel', 'OOC', 'MC']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageKindsTag testTag = UsageKindsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);

            Assert.AreEqual(3, testTag.Value.Count);
            Assert.IsTrue(testTag.Value.Contains(Microsoft.HidTools.HidSpecification.HidUsageKind.Sel));
            Assert.IsTrue(testTag.Value.Contains(Microsoft.HidTools.HidSpecification.HidUsageKind.OOC));
            Assert.IsTrue(testTag.Value.Contains(Microsoft.HidTools.HidSpecification.HidUsageKind.MC));
        }

        [TestMethod]
        public void InvalidTagCreation()
        {
            {
                string nonDecoratedString = @"types = ['foo']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageKindsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                string nonDecoratedString = @"types = []";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageKindsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                string nonDecoratedString = @"types = ['Sel', 'MC', 'Sel']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageKindsTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"foobar = ['Sel']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageKindsTag testTag = UsageKindsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"tYPes = ['Sel']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageKindsTag testTag = UsageKindsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);

            Assert.AreEqual(1, testTag.Value.Count);
            Assert.IsTrue(testTag.Value.Contains(Microsoft.HidTools.HidSpecification.HidUsageKind.Sel));
        }
    }
}
