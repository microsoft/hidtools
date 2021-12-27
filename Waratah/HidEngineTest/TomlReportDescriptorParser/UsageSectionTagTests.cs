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
    public class UsageSectionTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"
                [[usage]]
                    id = 1
                    name = 'Usage Id Name'
                    types = ['Sel', 'OOC', 'MC']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageSectionTag testTag = UsageSectionTag.TryParse(rawTomlTags.ElementAt(0));

            Assert.AreEqual(1, testTag.Id.ValueUInt16);
            Assert.AreEqual("Usage Id Name", testTag.UsageName.Value);
            Assert.AreEqual(3, testTag.Kinds.Value.Count);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            // Missing id tag.
            {
                string nonDecoratedString = @"
                    [[usage]]
                        name = 'Usage Id Name'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Missing name tag.
            {
                string nonDecoratedString = @"
                    [[usage]]
                        id = 1
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Missing types tag.
            {
                string nonDecoratedString = @"
                    [[usage]]
                        id = 1
                        name = 'Usage Id Name'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Invalid tag.
            {
                string nonDecoratedString = @"
                    [[usage]]
                        id = 1
                        name = 'Usage Id Name'
                        types = ['Sel', 'OOC', 'MC']
                        invalidTag = 1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlInvalidLocationException>(() => UsageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
