// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Defines the index of the ending designator associated with an array or bitmap.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.DesignatorMaximum)]
    public class DesignatorMaximumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignatorMaximumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public DesignatorMaximumItem(UInt32 value)
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
            return $"DesignatorMaximum({this.Value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
