// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using System.Globalization;
    using HidSpecification;

    /// <summary>
    /// The minimum physical value of a control.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.PhysicalMinimum)]
    public class PhysicalMinimumItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalMinimumItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public PhysicalMinimumItem(Int32 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the PhysicalMinimum. <br/>
        /// Technically, HID supports both [0, <see cref="UInt32.MaxValue"/>] and [<see cref="Int32.MinValue"/>, <see cref="Int32.MaxValue"/>] ranges.
        /// However, the Microsoft HID APIs can only express the LONG/INT32 range (see HIDP_VALUE_CAPS.PhysicalMin, https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/hidpi/ns-hidpi-_hidp_value_caps)
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

            return $"PhysicalMinimum({value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Value);
        }
    }
}
