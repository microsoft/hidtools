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
    public class UnitSectionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"
                [[unit]]
                    name = 'meter'
                    centimeter = [100.0, 1.0]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UnitSectionTag testTag = UnitSectionTag.TryParse(rawTomlTags.ElementAt(0));

            Assert.AreEqual("meter", testTag.Name.Value);
            Assert.AreEqual(1, testTag.Dimensions.Count);
            Assert.AreEqual("centimeter", testTag.Dimensions[0].NonDecoratedName);

            // As part of UnitSectionTag parsing, the valid Unit will be added to the Global Unit Table
            HidUnit foundHidUnit = HidUnitDefinitions.GetInstance().TryFindUnitByName("meter");
            Assert.IsNotNull(foundHidUnit);
        }

        [TestMethod]
        public void InvalidTagMissingName()
        {
            string nonDecoratedString = @"
                [[unit]]
                    centimeter = [100.0, 1.0]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => UnitSectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidTagMissingDimensions()
        {
            string nonDecoratedString = @"
                [[unit]]
                    name = 'meter'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => UnitSectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidTagInvalidDimension()
        {
            string nonDecoratedString = @"
                [[unit]]
                    name = 'meter'
                    invalidDimension = [100.0, 1.0]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => UnitSectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }
    }
}