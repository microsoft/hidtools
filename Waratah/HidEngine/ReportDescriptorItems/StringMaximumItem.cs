// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidSpecification;

    /// <summary>
    /// Maximum string index of range.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.StringMaximum)]
    public class StringMaximumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMaximumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public StringMaximumItem(UInt32 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this item.
        /// </summary>
        public UInt32 Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"StringMaximum({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
