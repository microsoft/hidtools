// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class SettingsSectionTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleTagCreation()
        {
            {
                string nonDecoratedString = @"
                    [[settings]]
                    packingInBytes = 1";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                SettingsSectionTag testTag = SettingsSectionTag.TryParse(rawTomlTags.ElementAt(0), true);

                Assert.IsNotNull(testTag);
                Assert.AreEqual(1, testTag.Packing.Value);

                Assert.AreEqual(1, Settings.GetInstance().PackingInBytes);
            }

            {
                string nonDecoratedString = @"
                    [[settings]]
                    packingInBytes = 1
                    optimize = true
                    generateCpp = true
                    cppDescriptorName = 'happyDescriptorName'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                SettingsSectionTag testTag = SettingsSectionTag.TryParse(rawTomlTags.ElementAt(0), true);

                Assert.IsNotNull(testTag);

                Assert.AreEqual(1, testTag.Packing.Value);
                Assert.AreEqual(1, Settings.GetInstance().PackingInBytes);

                Assert.AreEqual(true, testTag.Optimize.Value);
                Assert.AreEqual(true, Settings.GetInstance().Optimize);

                Assert.AreEqual(true, testTag.GenerateCpp.Value);
                Assert.AreEqual(true, Settings.GetInstance().GenerateCpp);

                Assert.AreEqual("happyDescriptorName", testTag.CppDescriptorName.Value);
                Assert.AreEqual("happyDescriptorName", Settings.GetInstance().CppDescriptorName);
            }
        }
    }
}