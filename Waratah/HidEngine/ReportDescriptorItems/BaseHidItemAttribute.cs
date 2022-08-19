// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Attribute that all ShortItems must have.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public abstract class BaseHidItemAttribute : System.Attribute
    {
        /// <summary>
        /// Generates the byte prefix for this item.
        /// </summary>
        /// <param name="size">Size of the item in bytes.</param>
        /// <returns>Encoded item with item prefix with size.</returns>
        public abstract byte[] GeneratePrefix(int size);

        /// <summary>
        /// Generates the byte prefix for this item (of particular type).
        /// </summary>
        /// <param name="typeKind">Kind of ShortItem type.</param>
        /// <param name="tag">Tag for this item.</param>
        /// <param name="size">Size of the item in bytes.</param>
        /// <returns>Encoded item with item prefix with size.</returns>
        protected static byte[] GeneratePrefixInternal(HidConstants.ShortItemTypeKind typeKind, int tag, int size)
        {
            byte bTag = (byte)(tag << 4);
            byte bType = (byte)((int)typeKind << 2);
            byte bSize = (byte)size;

            byte[] prefix = new byte[] { (byte)(bTag | bType | bSize) };

            return prefix;
        }
    }
}
