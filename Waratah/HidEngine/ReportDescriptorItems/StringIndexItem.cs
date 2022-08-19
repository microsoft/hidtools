// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Index for a string descriptor.  Allows a string to be associated with a particular item or control.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.StringIndex)]
    public class StringIndexItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIndexItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public StringIndexItem(UInt32 value)
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
            return $"StringIndex({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
