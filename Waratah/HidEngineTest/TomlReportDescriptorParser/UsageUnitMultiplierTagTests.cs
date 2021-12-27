// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class UsageUnitMultiplierTagTests
    {
        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"usageUnitMultiplier = 100.0";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageUnitMultiplierTag testTag = UsageUnitMultiplierTag.TryParse(rawTomlTags.ElementAt(0));

            Assert.AreEqual(100.0, testTag.Value);
        }
    }
}