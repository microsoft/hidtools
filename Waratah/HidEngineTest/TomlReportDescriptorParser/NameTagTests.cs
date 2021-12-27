// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class NameTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"name = 'happyname'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            NameTag testTag = NameTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual("happyname", testTag.Value);
        }

        [TestMethod]
        public void InvalidTagCreation()
        {
            {
                string nonDecoratedString = @"name = ''";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => NameTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // May not have non letters/digits/whitespace
            {
                string nonDecoratedString = @"name = 'ABC123@'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => NameTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalid = 'happyname'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            NameTag testTag = NameTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"NaME = 'happyname'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            NameTag testTag = NameTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual("happyname", testTag.Value);
        }
    }
}
