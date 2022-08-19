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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class PaddingItemTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"
                [[paddingItem]]
                    sizeInBits = 1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PaddingItemTag tag = PaddingItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Size.Value, 1);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            // Duplicate tag.
            {
                string nonDecoratedString = @"
                    [[paddingItem]]
                        sizeInBits = 1
                        sizeInBits = 1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlInvalidLocationException>(() => PaddingItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Tag other than sizeInBits.
            {
                string nonDecoratedString = @"
                    [[paddingItem]]
                        sizeInBits = 1
                        count = 1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlInvalidLocationException>(() => PaddingItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Missing sizeInBits tag.
            {
                string nonDecoratedString = @"
                    [[paddingItem]]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => PaddingItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"
                [[variableItem]]
                    sizeInBits = 1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PaddingItemTag tag = PaddingItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"
                [[PaDDINGItem]]
                    sizeInBits = 1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PaddingItemTag tag = PaddingItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Size.Value, 1);
        }
    }
}
