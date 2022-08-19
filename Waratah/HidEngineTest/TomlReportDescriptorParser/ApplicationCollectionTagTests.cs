// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ApplicationCollectionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            // Reset the Unit definitions after each test.
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"
                [[applicationCollection]]
                    usage = ['Lighting And Illumination', 'LampArray']
                        [[applicationCollection.inputReport]]
                            [[applicationCollection.inputReport.logicalCollection]]
                            usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                                [[applicationCollection.inputReport.logicalCollection.variableItem]]
                                usage = ['Lighting And Illumination', 'LampCount']
                                logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            ApplicationCollectionTag tag = ApplicationCollectionTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.IsNotNull(tag.Usage);
            Assert.IsNotNull(tag.Reports);
            Assert.AreEqual(tag.Reports.Count, 1);
        }

        [TestMethod]
        public void InvalidChildTag()
        {
            // Contains single invalid tag 'invalidChildTag'

            string nonDecoratedString = @"
                [[applicationCollection]]
                    usage = ['Lighting And Illumination', 'LampArray']
                    invalidChildTag = 1
                        [[applicationCollection.inputReport]]
                            [[applicationCollection.inputReport.logicalCollection]]
                            usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                                [[applicationCollection.inputReport.logicalCollection.variableItem]]
                                usage = ['Lighting And Illumination', 'LampCount']
                                logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlInvalidLocationException>(() => ApplicationCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidSectionName()
        {
            // Trying to parse a section that is not an ApplicationCollection should return null.

            string nonDecoratedString = @"
                [[variableItem]]
                    usage = ['UsagePage1', 'UsageId1']
                    sizeInBits = 1
                    logicalValueRange = [1, 2]
                    reportFlags = ['data', 'absolute']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.IsNull(ApplicationCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidUsageKind()
        {
            HidUsagePage testPage = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(0xFFFF, "TestUsagePage");
            HidUsageId testUsage1 = testPage.AddUsageId(new HidUsageId(1, "Id1", new List<HidUsageKind>() { HidUsageKind.DV }));
            HidUsageId testUsage2 = testPage.AddUsageId(new HidUsageId(2, "Id2", new List<HidUsageKind>() { HidUsageKind.DV }));

            string nonDecoratedString = @"
                [[applicationCollection]]
                    usage = ['TestUsagePage', 'Id1']
                        [[applicationCollection.inputReport]]
                            [[applicationCollection.inputReport.variableItem]]
                            usage = ['TestUsagePage', 'Id2']
                            logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => ApplicationCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }
    }
}
