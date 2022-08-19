// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Medallion;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Kind of wire representation.
    /// </summary>
    public enum WireRepresentationKind
    {
        /// <summary>
        /// Simple wire representation. Raw bytes with minimal formatting.
        /// </summary>
        Simple,

        /// <summary>
        /// Representation suitable for inclusion in a C++ header and for it to be compilable.
        /// </summary>
        CppHeader,
    }

    /// <summary>
    /// Describes a HID Short Item. All HID Short items must implement this.<br/>
    /// Packs a Size, Type, and Tag into the first byte.
    /// This first byte may be followed by 0, 1, 2, 4 optional data bytes depending on the size of the data.
    /// <code>
    /// +-------+-------------------------+-----------------------+--------------------------+<br/>
    /// | Bytes |            0            |           1           |             2            |<br/>
    /// +-------+-------+-------+---------+-----------------------+--------------------------+<br/>
    /// | Bits  |  0 1  |  2 3  | 4 5 6 7 | 8 9 10 11 12 13 14 15 | 16 17 18 19 20 21 22 23  |<br/>
    /// +-------+-------+-------+---------+-----------------------+--------------------------+<br/>
    /// | Parts | bSize | bType |   bTag  |         [data]        |          [data]          |<br/>
    /// +-------+-------+-------+---------+-----------------------+--------------------------+<br/>
    /// </code>
    /// All Short items are 1-5bytes long (pictured above is a 3byte item).
    /// </summary>
    public abstract class ShortItem
    {
        /// <summary>
        /// Raw bytes of this Short item.
        /// </summary>
        /// <returns>Short item bytes.</returns>
        public byte[] WireRepresentation()
        {
            byte[] dataBytes = this.GenerateItemData();

            byte[] prefixByte = this.GeneratePrefix(dataBytes.Length);

            byte[] combined = prefixByte.Concat(dataBytes).ToArray();

            return combined;
        }

        /// <summary>
        /// String representation of the raw bytes of this item.
        /// </summary>
        /// <param name="kind">Kind of representation.</param>
        /// <returns>String representation.</returns>
        public string WireRepresentationString(WireRepresentationKind kind = WireRepresentationKind.Simple)
        {
            switch (kind)
            {
                case WireRepresentationKind.Simple:
                {
                    return BitConverter.ToString(this.WireRepresentation());
                }

                case WireRepresentationKind.CppHeader:
                {
                    string bytes = "0x" + BitConverter.ToString(this.WireRepresentation());

                    bytes = bytes.Replace("-", ", 0x");

                    return bytes;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Encodes the value to bytes, truncating all leading zeros (0).
        /// </summary>
        /// <param name="encodeValue">Value to encode to bytes.</param>
        /// <returns>Byte representation of value.</returns>
        protected static byte[] EncodeToBytesWithTruncation(UInt16 encodeValue)
        {
            byte[] intBytes = BitConverter.GetBytes(encodeValue);

            if (encodeValue > 0)
            {
                // Removes leading zeros (0).
                while (intBytes[intBytes.Length - 1] == 0)
                {
                    intBytes = intBytes.Reverse().Skip(1).Reverse().ToArray();
                }

                // Size must be either 1,2 or 4.
                if (intBytes.Length == 3)
                {
                    intBytes = intBytes.Append((byte)0x00).ToArray();
                }
            }
            else
            {
                // Value of 0, always needs a single 0 byte.

                intBytes = new byte[] { 0 };
            }

            return intBytes;
        }

        /// <summary>
        /// Encodes the value to bytes, truncating all leading zeros (0).
        /// </summary>
        /// <param name="encodeValue">Value to encode to bytes.</param>
        /// <returns>Byte representation of value.</returns>
        protected static byte[] EncodeToBytesWithTruncation(UInt32 encodeValue)
        {
            byte[] intBytes = BitConverter.GetBytes(encodeValue);

            if (encodeValue > 0)
            {
                // Removes leading zeros (0).
                while (intBytes[intBytes.Length - 1] == 0)
                {
                    intBytes = intBytes.Reverse().Skip(1).Reverse().ToArray();
                }

                // Size must be either 1,2 or 4.
                if (intBytes.Length == 3)
                {
                    intBytes = intBytes.Append((byte)0x00).ToArray();
                }
            }
            else
            {
                // Value of 0, always needs a single 0 byte.

                intBytes = new byte[] { 0 };
            }

            return intBytes;
        }

        /// <summary>
        /// Encodes the value to bytes, truncating all leading zeros (0).
        /// </summary>
        /// <param name="encodeValue">Value to encode to bytes.</param>
        /// <returns>Byte representation of value.</returns>
        protected static byte[] EncodeToBytesWithTruncation(Int32 encodeValue)
        {
            byte[] intBytes = BitConverter.GetBytes(encodeValue);

            // If value is negative, then attempt to remove uncessary all-1's bytes.
            if (encodeValue < 0)
            {
                while (intBytes[intBytes.Length - 1] == 0xFF && (intBytes.Length > 1) && Bits.GetBit(intBytes[intBytes.Length - 2], 7))
                {
                    intBytes = intBytes.Reverse().Skip(1).Reverse().ToArray();
                }

                // Size must be either 1,2 or 4.
                if (intBytes.Length == 3)
                {
                    intBytes = intBytes.Append((byte)0xFF).ToArray();
                }
            }
            else if (encodeValue > 0)
            {
                // If value is positive, attempt to remove uncessary all-0's bytes

                while (intBytes[intBytes.Length - 1] == 0 && !Bits.GetBit(intBytes[intBytes.Length - 2], 7))
                {
                    intBytes = intBytes.Reverse().Skip(1).Reverse().ToArray();
                }

                // Size must be either 1,2 or 4.
                if (intBytes.Length == 3)
                {
                    intBytes = intBytes.Append((byte)0x00).ToArray();
                }
            }
            else
            {
                // Value of 0, always needs a single 0 byte.

                intBytes = new byte[] { 0 };
            }

            return intBytes;
        }

        /// <summary>
        /// Generates the 2-5 data bytes for this item.
        /// </summary>
        /// <returns>Item data bytes.</returns>
        protected abstract byte[] GenerateItemData();

        private static byte EncodeDataSize(int size)
        {
            switch (size)
            {
                case 0:
                {
                    return 0;
                }

                case 1:
                {
                    return 1;
                }

                case 2:
                {
                    return 2;
                }

                case 4:
                {
                    return 3;
                }

                default:
                {
                    throw new Exception(string.Format(Resources.ExceptionDescriptorInvalidDataSize, size));
                }
            }
        }

        private byte[] GeneratePrefix(int size)
        {
            int encodedSize = EncodeDataSize(size);

            GlobalItemAttribute globalAttribute = this.GetType().GetCustomAttribute<GlobalItemAttribute>(false);
            if (globalAttribute != null)
            {
                return globalAttribute.GeneratePrefix(encodedSize);
            }

            MainItemAttribute mainAttribute = this.GetType().GetCustomAttribute<MainItemAttribute>(false);
            if (mainAttribute != null)
            {
                return mainAttribute.GeneratePrefix(encodedSize);
            }

            LocalItemAttribute localAttribute = this.GetType().GetCustomAttribute<LocalItemAttribute>(false);
            if (localAttribute != null)
            {
                return localAttribute.GeneratePrefix(encodedSize);
            }

            throw new Exception(Resources.ExceptionItemCouldNotFindAttribute);
        }
    }
}
