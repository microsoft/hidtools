// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using System.Globalization;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// The maximum value of a control.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.LogicalMinimum)]
    public class LogicalMinimumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalMinimumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public LogicalMinimumItem(Int32 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the LogicalMinimum. <br/>
        /// Technically, HID supports both [0, <see cref="UInt32.MaxValue"/>] and [<see cref="Int32.MinValue"/>, <see cref="Int32.MaxValue"/>] ranges.
        /// However, the Microsoft HID APIs can only express the LONG/INT32 range (see HIDP_VALUE_CAPS.LogicalMin, https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/hidpi/ns-hidpi-_hidp_value_caps)
        /// so we will align to that restriction.
        /// </summary>
        public Int32 Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            string value = string.Empty;

            if (this.Value == 0)
            {
                value = this.Value.ToString();
            }
            else
            {
                // Print with ',' separating every 3 digits (only works with values > 1).
                value = this.Value.ToString("#,#", CultureInfo.InvariantCulture);
            }

            return $"LogicalMinimum({value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
