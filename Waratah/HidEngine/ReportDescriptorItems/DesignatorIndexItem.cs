// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Index to describe what body part to be used for a control.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.DesignatorIndex)]
    public class DesignatorIndexItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignatorIndexItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public DesignatorIndexItem(UInt32 value)
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
            return $"DesignatorIndex({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
