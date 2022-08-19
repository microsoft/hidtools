// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using System.Linq;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Usage index for an item usage.  represents a suggested usage for the item or collection.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.Usage)]
    public class UsageItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageItem"/> class.
        /// </summary>
        /// <param name="usagePage">Page of the Usage.</param>
        /// <param name="usageId">Id of the Usage.</param>
        /// <param name="extendedMode">Whether this is an extended usage.</param>
        public UsageItem(UInt16 usagePage, UInt16 usageId, bool extendedMode)
        {
            this.UsagePage = usagePage;

            this.UsageId = usageId;

            this.IsExtended = extendedMode;
        }

        /// <summary>
        /// Gets the Page associated with this item.
        /// </summary>
        public UInt16 UsagePage { get; }

        /// <summary>
        /// Gets the Id associated with this item.
        /// </summary>
        public UInt16 UsageId { get; }

        /// <summary>
        /// Gets a value indicating whether this is in extended mode.
        /// </summary>
        public bool IsExtended { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsExtended)
            {
                return $"UsagePage({this.UsagePage}) - UsageId({this.UsageId})";
            }
            else
            {
                return $"UsageId({HidUsageTableDefinitions.GetInstance().TryFindUsageId(this.UsagePage, this.UsageId)})";
            }
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            if (this.UsageId == 0)
            {
                throw new InvalidOperationException("UsageId must be set");
            }

            if (this.IsExtended)
            {
                // (pg. 41), UsageId consumes lower-order 16bits, and UsagePage consumes high-order 16bits.
                byte[] itemBytes = BitConverter.GetBytes(this.UsageId);

                itemBytes = itemBytes.Concat(BitConverter.GetBytes(this.UsagePage)).ToArray();

                return itemBytes;
            }
            else
            {
                byte[] itemBytes = EncodeToBytesWithTruncation(this.UsageId);

                return itemBytes;
            }
        }
    }
}
