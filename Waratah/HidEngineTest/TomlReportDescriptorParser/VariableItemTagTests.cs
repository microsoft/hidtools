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
    public class VariableItemTagTests
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
                [[variableItem]]
                    usage = ['Generic Desktop', 'X']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            VariableItemTag tag = VariableItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.IsNotNull(tag.Usage);
            Assert.IsNull(tag.UsageRange);
        }

        [TestMethod]
        public void InvalidSectionName()
        {
            string nonDecoratedString = @"
                [[arrayItem]]
                    usageRange = ['Button', 'Button 1', 'Button 3']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.IsNull(VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidUsageCombinations()
        {
            // Cannot specify both Usage and UsageRange.
            {
                string nonDecoratedString = @"
                [[variableItem]]
                    usage = ['Generic Desktop', 'X']
                    usageTransform = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Cannot specify both Usage and UsageTransform.
            {
                string nonDecoratedString = @"
                [[variableItem]]
                    usage = ['Generic Desktop', 'X']
                    usageTransform = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Cannot specify both UsageRange and UsageTransform.
            {
                string nonDecoratedString = @"
                [[variableItem]]
                    usageRange = ['Generic Desktop', 'X', 'Y']
                    usageTransform = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Missing any Usage/Range/Transform
            {
                string nonDecoratedString = @"
                [[variableItem]]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void IncludeUnitTag()
        {
            string nonDecoratedString = @"
            [[variableItem]]
                usage = ['Generic Desktop', 'X']
                unit = 'centimeter'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            VariableItemTag tag = VariableItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.IsNotNull(tag.Usage);
            Assert.IsNotNull(tag.Unit);
        }

        [TestMethod]
        public void IncludeUsageUnitMultiplierTag()
        {
            string nonDecoratedString = @"
            [[variableItem]]
                usage = ['Generic Desktop', 'X']
                usageUnitMultiplier = 100.0";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            VariableItemTag tag = VariableItemTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.IsNotNull(tag.Usage);
            Assert.AreEqual(100.0, tag.UsageUnitMultiplier.Value);
        }

        [TestMethod]
        public void InvalidUnitMultiplierCombinations()
        {
            // Cannot specify both Unit and UsageUnitMultiplier.
            {
                string nonDecoratedString = @"
                [[variableItem]]
                    usage = ['Generic Desktop', 'X']
                    unit = 'centimeter'
                    usageUnitMultiplier = 100.0";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidUsageKind()
        {
            HidUsagePage testPage = HidUsageTableDefinitions.GetInstance().CreateOrGetUsagePage(0xFFFF, "TestUsagePage");
            HidUsageId testUsage1 = testPage.AddUsageId(new HidUsageId(1, "Id1", new List<HidUsageKind>() { HidUsageKind.CA }));

            string nonDecoratedString = @"
                [[variableItem]]
                    usage = ['TestUsagePage', 'Id1']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<TomlGenericException>(() => VariableItemTag.TryParse(rawTomlTags.ElementAt(0)));
        }
    }
}