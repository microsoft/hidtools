// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest.ReportDescriptorItems
{
    using HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    class ShortItemStaticMethodExposer : ShortItem
    {
        public static byte[] EncodeSignedInteger(Int32 encodeValue)
        {
            return EncodeToBytesWithTruncation(encodeValue);
        }

        protected override byte[] GenerateItemData()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class ShortItemTests
    {
        // Note: This test takes at least 3.5mins to run, so run sparingly
        //[TestMethod]
        [Priority(2)]
        public void ValidateEncodeSignedIntegerExhaustive()
        {
            Func<Int32, byte[]> EncodeValue = ShortItemStaticMethodExposer.EncodeSignedInteger;

            for (int i = int.MinValue; i < int.MaxValue; i++)
            {
                byte[] encodedBytes = EncodeValue(i);

                switch (encodedBytes.Length)
                {
                    case 1:
                    {
                        Assert.AreEqual(encodedBytes[0], (byte)i);
                        break;
                    }
                    case 2:
                    {
                        Assert.AreEqual(BitConverter.ToInt16(encodedBytes, 0), i);
                        break;
                    }
                    case 4:
                    {
                        Assert.AreEqual(BitConverter.ToInt32(encodedBytes, 0), i);
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }
}
