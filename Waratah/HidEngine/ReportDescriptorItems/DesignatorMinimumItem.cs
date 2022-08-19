// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Defines the index of the starting designator associated with an array or bitmap.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.DesignatorMinimum)]
    public class DesignatorMinimumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignatorMinimumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public DesignatorMinimumItem(UInt32 value)
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
            return $"DesignatorMinimum({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
