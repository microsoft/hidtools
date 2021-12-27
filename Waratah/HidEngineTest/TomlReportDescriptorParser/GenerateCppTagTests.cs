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
    public class GenerateCppTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"generateCpp = true";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            GenerateCppTag testTag = GenerateCppTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(true, testTag.Value);
        }

        [TestMethod]
        public void InvalidTagCreation()
        {
            string nonDecoratedString = @"generateCpp = 1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => GenerateCppTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"generateCppFoo = true";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            GenerateCppTag testTag = GenerateCppTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"generateCPP = true";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            GenerateCppTag testTag = GenerateCppTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(true, testTag.Value);
        }
    }
}
