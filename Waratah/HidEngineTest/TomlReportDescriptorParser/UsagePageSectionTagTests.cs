// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HidSpecification;
    using Nett;

    /// <summary>
    /// Summary description for UsagePageSectionTagTests
    /// </summary>
    [TestClass]
    public class UsagePageSectionTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            string nonDecoratedString = @"
                [[usagePage]]
                id = 1000
                name = 'New UsagePage 1'

                    [[usagePage.usage]]
                    id = 1
                    name = 'New Usage 1'
                    types = ['Sel', 'OOC', 'MC']

                    [[usagePage.usage]]
                    id = 2
                    name = 'New Usage 2'
                    types = ['Sel']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            // Reset Global UsageTables.
            HidUsageTableDefinitions.GetInstance(true);

            UsagePageSectionTag testTag = UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0));

            Assert.AreEqual(1000, testTag.Id.ValueUInt16);
            Assert.AreEqual("New UsagePage 1", testTag.PageName.Value);
            Assert.AreEqual(2, testTag.Usages.Count);

            Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsagePage(1000));
            Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1000, 1));
            Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1000, 2));
        }

        [TestMethod]
        public void InvalidSectionParameters()
        {
            // Missing name and id tags.
            {
                string nonDecoratedString = @"
                    [[usagePage]]

                        [[usagePage.usage]]
                        id = 1
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']

                        [[usagePage.usage]]
                        id = 2
                        name = 'New Usage 2'
                        types = ['Sel']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Invalid Tag.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    invalidTag = 1

                        [[usagePage.usage]]
                        id = 1
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']

                        [[usagePage.usage]]
                        id = 2
                        name = 'New Usage 2'
                        types = ['Sel']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlInvalidLocationException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Missing child Usages
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1000
                    name = 'New UsagePage 1'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage with duplicate id
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1000
                    name = 'New UsagePage 1'

                        [[usagePage.usage]]
                        id = 1
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']

                        [[usagePage.usage]]
                        id = 1
                        name = 'New Usage 2'
                        types = ['Sel']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    
        [TestMethod]
        public void SimpleAdditionToPublicUsagePage()
        {
            // Contains both id and name
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1
                    name = 'Generic Desktop'

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']

                        [[usagePage.usage]]
                        id = 1001
                        name = 'New Usage 2'
                        types = ['Sel']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                UsagePageSectionTag testTag = UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0));

                Assert.AreEqual(1, testTag.Id.ValueUInt16);
                Assert.AreEqual("Generic Desktop", testTag.PageName.Value);

                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsagePage(1));
                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1, 1000));
                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1, 1001));
            }

            // Contains only id tag.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                UsagePageSectionTag testTag = UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0));

                Assert.AreEqual(1, testTag.Id.ValueUInt16);

                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsagePage(1));
                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1, 1000));
            }

            // Contains only name tag.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    name = 'Generic Desktop'

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                UsagePageSectionTag testTag = UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0));

                Assert.AreEqual("Generic Desktop", testTag.PageName.Value);

                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsagePage(1));
                Assert.IsNotNull(HidUsageTableDefinitions.GetInstance().TryFindUsageId(1, 1000));
            }
        }

        [TestMethod]
        public void InvalidAdditionToPublicUsagePage()
        {
            // id exists, name exists, but are mismatching.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1
                    name = 'Consumer'

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // id exists, but name does not.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1
                    name = 'Invalid Name'

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // name exists, but id does not.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 10000
                    name = 'Generic Desktop'

                        [[usagePage.usage]]
                        id = 1000
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // usage already exists in page.
            {
                string nonDecoratedString = @"
                    [[usagePage]]
                    id = 1
                    name = 'Generic Desktop'

                        [[usagePage.usage]]
                        id = 1
                        name = 'New Usage 1'
                        types = ['Sel', 'OOC', 'MC']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                // Reset Global UsageTables.
                HidUsageTableDefinitions.GetInstance(true);

                Assert.ThrowsException<TomlGenericException>(() => UsagePageSectionTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }
    }
}
