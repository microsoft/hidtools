// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Nett;

    [TestClass]
    public class TagFinderTests
    {
        [TestMethod]
        public void SimpleFindLine()
        {
            string nonDecoratedString =
                @"[[applicationCollection]]
                    [[applicationCollection.inputReport]]
                        [[applicationCollection.inputReport.logicalCollection]]
                            [[applicationCollection.inputReport.logicalCollection.variableItem]]";

            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedString);
            TagFinder finder = new TagFinder(decoratedTomlDoc);

            Assert.AreEqual(finder.FindLine("inputReport2"), 2);
        }
    }
}
