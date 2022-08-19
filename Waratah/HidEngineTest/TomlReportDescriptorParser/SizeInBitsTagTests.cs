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
    public class SizeInBitsTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"sizeInBits=1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            SizeInBitsTag testTag = SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.Value, 1);
        }

        [TestMethod]
        public void InvalidKeyParameters()
        {
            {
                string nonDecoratedString = @"sizeInBits=[1, 2]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                string nonDecoratedString = @"sizeInBits=[]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalidKey=[1]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            SizeInBitsTag testTag = SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"sizeInBITS=1";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            SizeInBitsTag testTag = SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
            Assert.AreEqual(testTag.Value, 1);
        }

        [TestMethod]
        public void ValueOutOfBounds()
        {
            {
                Int64 invalidHigh = ((Int64)Byte.MaxValue) + 1;

                string nonDecoratedString = $"sizeInBits={invalidHigh}";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            {
                // Cannot be zero.
                Int64 invalidLow = ((Int64)0);

                string nonDecoratedString = $"sizeInBits={invalidLow}";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => SizeInBitsTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
