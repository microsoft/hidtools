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
    public class CountTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"count=1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            CountTag testTag = CountTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.Value, 1);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            {
                string nonDecoratedString = @"count=[1, 2]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => CountTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                string nonDecoratedString = @"count=[]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => CountTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidKey=1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            CountTag testTag = CountTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"CoUNT=1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            CountTag testTag = CountTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.Value, 1);
        }

        [TestMethod]
        public void ValueOutOfBounds()
        {
            {
                Int64 invalidHigh = ((Int64)Int32.MaxValue) + 1;

                string nonDecoratedString = $"count={invalidHigh}";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => CountTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                Int64 invalidLow = ((Int64)Int32.MinValue) - 1;

                string nonDecoratedString = $"count={invalidLow}";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => CountTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
