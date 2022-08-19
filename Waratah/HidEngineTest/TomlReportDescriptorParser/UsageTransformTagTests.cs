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
    public class UsageTransformTagTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleTagCreationString()
        {
            string nonDecoratedString = @"usageTransform = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTransformTag tag = UsageTransformTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Id, 0x1591);
            Assert.AreEqual(tag.Value.Name, "Data Field: Activity Type || (Modifier: Change Sensitivity Absolute)");
            Assert.AreEqual(tag.Value.Kinds[0], Microsoft.HidTools.HidSpecification.HidUsageKind.NAry);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Sensors");
            Assert.AreEqual(tag.Value.Page.Id, 0x20);
        }

        [TestMethod]
        public void SimpleTagCreationInteger()
        {
            string nonDecoratedString = @"usageTransform = [32, 1425, 4096]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTransformTag tag = UsageTransformTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag.Value.Id, 0x1591);
            Assert.AreEqual(tag.Value.Name, "Data Field: Activity Type || (Modifier: Change Sensitivity Absolute)");
            Assert.AreEqual(tag.Value.Kinds[0], Microsoft.HidTools.HidSpecification.HidUsageKind.NAry);
            Assert.AreEqual(tag.Value.Page.Kind, Microsoft.HidTools.HidSpecification.HidUsagePageKind.Defined);
            Assert.AreEqual(tag.Value.Page.Name, "Sensors");
            Assert.AreEqual(tag.Value.Page.Id, 0x20);
        }

        [TestMethod]
        public void InvalidKeyParametersString()
        {
            // 2 string parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageTransform = ['Sensors', 'Data Field: Activity Type']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                string nonDecoratedString = @"usageTransform = ['Sensors', 'INVALID', 'Modifier: Change Sensitivity Absolute']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Transform Usage doesn't exist.
            {
                string nonDecoratedString = @"usageTransform = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute INVALID']";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyParametersInteger()
        {
            // 2 int parameters. (3 and only 3 are permitted).
            {
                string nonDecoratedString = @"usageTransform = [32, 1425]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Usage doesn't exist.
            {
                string nonDecoratedString = @"usageTransform = [32, 4095, 4096]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Transform Usage doesn't exist.
            {
                string nonDecoratedString = @"usageTransform = [32, 1425, 4097]";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void TransformUsageSameAsExiting()
        {
            // Since transforming a Usage (with another Usage) is a simple OR operation,
            // it's possible (and likely) that a random combination would override 

            string nonDecoratedString = @"usageTransform = [1, 2, 4]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            Assert.ThrowsException<HidSpecificationException>(() => UsageTransformTag.TryParse(rawTomlTags.ElementAt(0)));
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"usageTransformINVALID = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTransformTag tag = UsageTransformTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void CaseInsensitiveKeyName()
        {
            string nonDecoratedString = @"UsAGETransFOrm = ['Sensors', 'Data Field: Activity Type', 'Modifier: Change Sensitivity Absolute']";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            UsageTransformTag testTag = UsageTransformTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNotNull(testTag);
        }
    }
}
