// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class UnitDimensionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"centimeter = [100.0, 3.0]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UnitDimensionTag testTag = UnitDimensionTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);

            Assert.AreEqual(100, testTag.Multiplier);
            Assert.AreEqual(3, testTag.PowerExponent);
            Assert.AreEqual("centimeter", testTag.NonDecoratedName);
        }

        [TestMethod]
        public void InvalidTagCreation()
        {
            // Multiplier/Exponent must be doubles.
            {
                string nonDecoratedString = @"centimeter = [2, 1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UnitDimensionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Must be exactly 2 floats.
            {
                string nonDecoratedString = @"centimeter = [2.0]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UnitDimensionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // PowerExponent cannot have a decimal component.
            {
                string nonDecoratedString = @"centimeter = [2.0, 1.1]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UnitDimensionTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
