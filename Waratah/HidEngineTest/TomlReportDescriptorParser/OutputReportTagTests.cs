// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class OutputReportTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"
                [[outputReport]]
                    [[outputReport.logicalCollection]]
                    usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                        [[outputReport.logicalCollection.variableItem]]
                        usage = ['Lighting And Illumination', 'LampCount']
                        logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            OutputReportTag tag = OutputReportTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.IsNull(tag.Id);
            Assert.AreEqual(tag.Items.Count, 1);
        }

        [TestMethod]
        public void InvalidSectionName()
        {
            string nonDecoratedString = @"
                [[featureReport]]
                    [[featureReport.logicalCollection]]
                    usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                        [[featureReport.logicalCollection.variableItem]]
                        usage = ['Lighting And Illumination', 'LampCount']
                        logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.IsNull(OutputReportTag.TryParse(rawTomlTags.ElementAt(0)));
        }
    }
}
