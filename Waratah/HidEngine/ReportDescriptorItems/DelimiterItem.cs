// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Defines the beginning or end of a set of local items.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.Delimiter)]
    public class DelimiterItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimiterItem"/> class.
        /// </summary>
        /// <param name="isOpen">Whether the delimiter is an open or close.</param>
        public DelimiterItem(bool isOpen)
        {
            this.IsOpen = isOpen;
        }

        /// <summary>
        /// Gets a value indicating whether this is an open or close delimiter.
        /// </summary>
        public bool IsOpen { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsOpen)
            {
                return "Delimiter(Open)";
            }
            else
            {
                return "Delimiter(Close)";
            }
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            // TODO: This Emulates the HID Descriptor Tool, but I wonder why we need to have the zero byte at all.
            if (this.IsOpen)
            {
                return EncodeToBytesWithTruncation(1);
            }
            else
            {
                return EncodeToBytesWithTruncation(0);
            }
        }
    }
}
