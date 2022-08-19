// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Minimum string index of range.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.StringMinimum)]
    public class StringMinimumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMinimumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public StringMinimumItem(UInt32 value)
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
            return $"StringMinimum({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
