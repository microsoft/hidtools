﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class LogicalCollectionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            // Reset the Unit definitions after each test.
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleLogicalCollectionTag()
        {
            string nonDecoratedTomlDoc = @"
                [[logicalCollection]]
                usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                    [[logicalCollection.variableItem]]
                    usage = ['Lighting And Illumination', 'LampCount']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            LogicalCollectionTag tag = LogicalCollectionTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Usage.Value.Name, "LampArrayAttributesReport");
            Assert.AreEqual(tag.Usage.Value.Page.Name, "Lighting And Illumination");
            Assert.IsNotNull(tag.Tags);
        }

        [TestMethod]
        public void MissingUsageTag()
        {
            string nonDecoratedTomlDoc = @"
                [[logicalCollection]]
                    [[logicalCollection.variableItem]]
                    usage = ['Lighting And Illumination', 'LampCount']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => LogicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidSectionName()
        {
            string nonDecoratedString = @"
                [[physicalCollection]]
                usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                    [[physicalCollection.variableItem]]
                    usage = ['Lighting And Illumination', 'LampCount']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.IsNull(LogicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidUsageKind()
        {
            HidUsagePage testPage = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(0xFFFF, "TestUsagePage");
            HidUsageId testUsage1 = testPage.AddUsageId(new HidUsageId(1, "Id1", new List<HidUsageKind>() { HidUsageKind.DV }));
            HidUsageId testUsage2 = testPage.AddUsageId(new HidUsageId(2, "Id2", new List<HidUsageKind>() { HidUsageKind.DV }));

            string nonDecoratedString = @"
                [[logicalCollection]]
                    usage = ['TestUsagePage', 'Id1']
                        [[logicalCollection.variableItem]]
                        usage = ['TestUsagePage', 'Id2']
                        logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => LogicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        /// <summary>
        /// Previously found a bug where the parent of child modules (generated from logicalCollection/physicalCollection)
        /// was not the collection module.
        /// </summary>
        [TestMethod]
        public void GeneratedModuleRelationshipsAreValid()
        {
            string nonDecoratedTomlDoc = @"
                [[inputReport]]
                    [[inputReport.logicalCollection]]
                    usage = ['Lighting And Illumination', 'LampArrayAttributesReport']
                        [[inputReport.logicalCollection.variableItem]]
                        usage = ['Lighting And Illumination', 'LampCount']
                        sizeInBits = 8";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            InputReportTag tag = InputReportTag.TryParse(rawTomlTags.ElementAt(0));
            ReportModule reportModule = (ReportModule)tag.GenerateDescriptorModule(null);

            Assert.AreEqual(1, reportModule.Children.Count);

            CollectionModule logicalCollectionModule = (CollectionModule)reportModule.Children.First();
            foreach (BaseModule child in logicalCollectionModule.Children)
            {
                // The parent of the child, must be the child's parent.
                Assert.AreEqual(logicalCollectionModule, child.Parent);
            }
        }
    }
}
