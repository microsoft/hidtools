// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidSpecification;

    /// <summary>
    /// Usage Page Item.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.UsagePage)]
    public class UsagePageItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsagePageItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public UsagePageItem(UInt16 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this item.
        /// </summary>
        public UInt16 Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"UsagePage({HidUsageTableDefinitions.GetInstance().TryFindUsagePage(this.Value)})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
