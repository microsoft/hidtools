// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.TomlReportDescriptorParser
{
    using System;
    using HidEngine.TomlReportDescriptorParser;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TagDecoratorTests
    {
        [TestMethod]
        public void MultipleReportDifferentType()
        {
            string nonDecoratedString = @"
                [[inputReport]]
                id = 1
                    [[inputReport.paddingItem]]
                    sizeInBits = 1
                [[outputReport]]
                id = 4
                    [[outputReport.paddingItem]]
                    sizeInBits = 2";

            string decoratedStringExpected = @"
                [[inputReport1]]
                id1 = 1
                    [[inputReport1.paddingItem2]]
                    sizeInBits2 = 1
                [[outputReport3]]
                id3 = 4
                    [[outputReport3.paddingItem4]]
                    sizeInBits4 = 2";

            string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

            Assert.AreEqual(decoratedStringExpected.Trim(), decoratedStringActual.Trim());
        }

        [TestMethod]
        public void MultipleReportSameType()
        {
            string nonDecoratedString = @"
            [[applicationCollection]]
                [[applicationCollection.inputReport]]
                id = 1
                    [[applicationCollection.inputReport.paddingItem]]
                    sizeInBits = 1
                [[applicationCollection.inputReport]]
                id = 4
                    [[applicationCollection.inputReport.paddingItem]]
                    sizeInBits = 2";

            string decoratedStringExpected = @"
            [[applicationCollection1]]
                [[applicationCollection1.inputReport2]]
                id1 = 1
                    [[applicationCollection1.inputReport2.paddingItem3]]
                    sizeInBits2 = 1
                [[applicationCollection1.inputReport4]]
                id3 = 4
                    [[applicationCollection1.inputReport4.paddingItem5]]
                    sizeInBits4 = 2";

            string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

            Assert.AreEqual(decoratedStringExpected.Trim(), decoratedStringActual.Trim());
        }

        [TestMethod]
        public void InvalidReportSection()
        {
            // Report sections can't have any section names except for those explicitly defined.

            string nonDecoratedString = @"
            [[applicationCollection]]
                [[applicationCollection.invalidSectionName]]
                id = 1
                    [[applicationCollection.invalidSectionName.paddingItem]]
                    sizeInBits = 1
                [[applicationCollection.iinvalidSectionName]]
                id = 4
                    [[applicationCollection.invalidSectionName.paddingItem]]
                    sizeInBits = 2";

            Assert.ThrowsException<TomlGenericException>(() => TagDecorator.Decorate(nonDecoratedString));
        }

        [TestMethod]
        public void PreviousSectionNameNotCompletePrefix()
        {
            {
                // Note: In below context, "applicationCollection.outputReport.paddingItem" will not be valid
                // when evaluating tags (as there is no previous output report declared), but it is valid
                // as this stage (to the decorator).

                string nonDecoratedString = @"
                [[applicationCollection]]
                    [[applicationCollection.inputReport]]
                        [[applicationCollection.inputReport.paddingItem]]
                    [[applicationCollection.outputReport.paddingItem]]";

                string decoratedStringExpected = @"
                [[applicationCollection1]]
                    [[applicationCollection1.inputReport2]]
                        [[applicationCollection1.inputReport2.paddingItem3]]
                    [[applicationCollection1.outputReport4.paddingItem5]]";

                string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

                Assert.AreEqual(decoratedStringActual.Trim(), decoratedStringExpected.Trim());
            }

            {
                // Note: In below context, "applicationCollection.inputReport.paddingItem.paddingItem" will not be valid
                // when evaluating tags (as padding HidConstants.Items can't have a nested HidConstants.Item), but it is valid
                // as this stage (to the decorator).

                // Note: In below context, "applicationCollection.outputReport.paddingItem" will not be valid
                // when evaluating tags (as there is no previous output report declared), but it is valid
                // as this stage (to the decorator).

                string nonDecoratedString = @"
                [[applicationCollection]]
                    [[applicationCollection.inputReport]]
                        [[applicationCollection.inputReport.paddingItem]]
                            [[applicationCollection.inputReport.paddingItem.paddingItem]]
                    [[applicationCollection.outputReport.paddingItem]]";

                string decoratedStringExpected = @"
                [[applicationCollection1]]
                    [[applicationCollection1.inputReport2]]
                        [[applicationCollection1.inputReport2.paddingItem3]]
                            [[applicationCollection1.inputReport2.paddingItem3.paddingItem4]]
                    [[applicationCollection1.outputReport5.paddingItem6]]";

                string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

                Assert.AreEqual(decoratedStringActual.Trim(), decoratedStringExpected.Trim());
            }
        }

        [TestMethod]
        public void CurrentSectionIsPrefixOfPreviousSection()
        {
            {
                string nonDecoratedString = @"
                [[applicationCollection]]
                    [[applicationCollection.inputReport]]
                        [[applicationCollection.inputReport.paddingItem]]
                    [[applicationCollection.inputReport]]";

                string decoratedStringExpected = @"
                [[applicationCollection1]]
                    [[applicationCollection1.inputReport2]]
                        [[applicationCollection1.inputReport2.paddingItem3]]
                    [[applicationCollection1.inputReport4]]";

                string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

                Assert.AreEqual(decoratedStringActual.Trim(), decoratedStringExpected.Trim());
            }

            {
                // Note: In below context, "applicationCollection.inputReport.paddingItem.paddingItem" will not be valid
                // when evaluating tags (as padding HidConstants.Items can't have a nested HidConstants.Item), but it is valid
                // as this stage (to the decorator).

                string nonDecoratedString = @"
                [[applicationCollection]]
                    [[applicationCollection.inputReport]]
                        [[applicationCollection.inputReport.paddingItem]]
                            [[applicationCollection.inputReport.paddingItem.paddingItem]]
                    [[applicationCollection.inputReport]]";

                string decoratedStringExpected = @"
                [[applicationCollection1]]
                    [[applicationCollection1.inputReport2]]
                        [[applicationCollection1.inputReport2.paddingItem3]]
                            [[applicationCollection1.inputReport2.paddingItem3.paddingItem4]]
                    [[applicationCollection1.inputReport5]]";

                string decoratedStringActual = TagDecorator.Decorate(nonDecoratedString);

                Assert.AreEqual(decoratedStringActual.Trim(), decoratedStringExpected.Trim());
            }
        }
    }
}
