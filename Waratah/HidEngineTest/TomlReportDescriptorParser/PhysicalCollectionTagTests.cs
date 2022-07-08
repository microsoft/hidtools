// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class PhysicalCollectionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            // Reset the Unit definitions after each test.
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimplePhysicalCollectionTag()
        {
            string nonDecoratedTomlDoc = @"
                [[physicalCollection]]
                usage = ['Generic Desktop', 'Pointer']
                    [[physicalCollection.variableItem]]
                    usage = ['Generic Desktop', 'X']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            PhysicalCollectionTag tag = PhysicalCollectionTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Usage.Value.Name, "Pointer");
            Assert.AreEqual(tag.Usage.Value.Page.Name, "Generic Desktop");
            Assert.IsNotNull(tag.Tags);
        }

        [TestMethod]
        public void MissingUsageTag()
        {
            string nonDecoratedTomlDoc = @"
                [[physicalCollection]]
                    [[physicalCollection.variableItem]]
                    usage = ['Generic Desktop', 'X']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => PhysicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidSectionName()
        {
            string nonDecoratedString = @"
                [[logicalCollection]]
                usage = ['Generic Desktop', 'Pointer']
                    [[logicalCollection.variableItem]]
                    usage = ['Generic Desktop', 'X']
                    logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.IsNull(PhysicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidUsageKind()
        {
            HidUsagePage testPage = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(0xFFFF, "TestUsagePage");
            HidUsageId testUsage1 = testPage.AddUsageId(new HidUsageId(1, "Id1", new List<HidUsageKind>() { HidUsageKind.DV }));
            HidUsageId testUsage2 = testPage.AddUsageId(new HidUsageId(2, "Id2", new List<HidUsageKind>() { HidUsageKind.DV }));

            string nonDecoratedString = @"
                [[physicalCollection]]
                    usage = ['TestUsagePage', 'Id1']
                        [[physicalCollection.variableItem]]
                        usage = ['TestUsagePage', 'Id2']
                        logicalValueRange = [0, 100]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => PhysicalCollectionTag.TryParse(rawTomlTags.ElementAt(0)));
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
                    [[inputReport.physicalCollection]]
                    usage = ['Sensors', 'Biometric: Human Presence']
                        [[inputReport.physicalCollection.variableItem]]
                        usage = ['Sensors', 'Property: Persistent Unique ID']
                        sizeInBits = 8";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            InputReportTag tag = InputReportTag.TryParse(rawTomlTags.ElementAt(0));
            ReportModule reportModule = (ReportModule)tag.GenerateDescriptorModule(null);

            Assert.AreEqual(1, reportModule.Children.Count);

            CollectionModule physicalCollectionModule = (CollectionModule)reportModule.Children.First();
            foreach (BaseModule child in physicalCollectionModule.Children)
            {
                // The parent of the child, must be the child's parent.
                Assert.AreEqual(physicalCollectionModule, child.Parent);
            }
        }
    }
}
