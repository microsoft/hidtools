// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using System.Globalization;
    using HidSpecification;

    /// <summary>
    /// Unit exponent item.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.UnitExponent)]
    public class UnitExponentItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitExponentItem"/> class.
        /// </summary>
        /// <param name="value">Value of the item.</param>
        public UnitExponentItem(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this item.
        /// </summary>
        public double Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            string value = string.Empty;

            if (this.Value >= 0 && this.Value < 1)
            {
                value = this.Value.ToString("N8").TrimEnd('0');
            }
            else
            {
                // Print with ',' separating every 3 digits (only works with values > 1).
                value = this.Value.ToString("#,#", CultureInfo.InvariantCulture);
            }

            return $"UnitExponent({value})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            byte[] itemBytes = new byte[1];

            itemBytes[0] = HidConstants.MultiplierToWireCode(this.Value);

            return itemBytes;
        }
    }
}
