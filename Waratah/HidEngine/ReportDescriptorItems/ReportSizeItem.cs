// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidEngine.Properties;
    using HidSpecification;

    /// <summary>
    /// Describes the number of bits for each report item.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.ReportSize)]
    public class ReportSizeItem : ShortItem
    {
        private UInt32 size = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSizeItem"/> class.
        /// </summary>
        /// <param name="size">The size of the report item.</param>
        public ReportSizeItem(Int32 size)
        {
            this.SizeInBits = (UInt32)size;
        }

        /// <summary>
        /// Gets the size of the item.
        /// </summary>
        public UInt32 SizeInBits
        {
            get
            {
                return this.size;
            }

            private set
            {
                if ((value > HidConstants.SizeInBitsMaximum) || (value < HidConstants.SizeInBitsMinimum))
                {
                    throw new ArgumentOutOfRangeException(string.Format(Resources.ExceptionItemInvalidSize, value));
                }

                this.size = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ReportSize({this.SizeInBits})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.SizeInBits);
        }
    }
}
