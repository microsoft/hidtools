// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.HidTools.HidEngine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class OutputFormatTagTests
    {
        [TestMethod]
        public void SimpleTagCreation()
        {
            {
                string nonDecoratedString = @"outputFormat = 'Cpp'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                OutputFormatTag testTag = OutputFormatTag.TryParse(rawTomlTags.ElementAt(0));
                Assert.IsNotNull(testTag);
                Assert.AreEqual(OutputFormatKind.Cpp, testTag.Value);
            }

            {
                string nonDecoratedString = @"outputFormat = 'CppMacro'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                OutputFormatTag testTag = OutputFormatTag.TryParse(rawTomlTags.ElementAt(0));
                Assert.IsNotNull(testTag);
                Assert.AreEqual(OutputFormatKind.CppMacro, testTag.Value);
            }

            {
                string nonDecoratedString = @"outputFormat = 'PlainText'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                OutputFormatTag testTag = OutputFormatTag.TryParse(rawTomlTags.ElementAt(0));
                Assert.IsNotNull(testTag);
                Assert.AreEqual(OutputFormatKind.PlainText, testTag.Value);
            }
        }

        [TestMethod]
        public void InvalidTagCreation()
        {
            {
                string nonDecoratedString = @"outputFormat = ''";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => OutputFormatTag.TryParse(rawTomlTags.ElementAt(0)));
            }

            // Only 3 valid options (see above) and are case sensitive.
            {
                string nonDecoratedString = @"outputFormat = 'CPP'";

                string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
                TagFinder.Initialize(decoratedTomlDoc);
                Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

                Assert.ThrowsException<TomlGenericException>(() => OutputFormatTag.TryParse(rawTomlTags.ElementAt(0)));
            }
        }

        [TestMethod]
        public void InvalidKeyName()
        {
            string nonDecoratedString = @"invalid = 'happyname'";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder.Initialize(decoratedTomlDoc);
            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();

            OutputFormatTag testTag = OutputFormatTag.TryParse(rawTomlTags.ElementAt(0));
            Assert.IsNull(testTag);
        }
    }
}
