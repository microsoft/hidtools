// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Unit item.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.Unit)]
    public class UnitItem : ShortItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitItem"/> class.
        /// </summary>
        /// <param name="unit">The underlying Unit of this item.</param>
        public UnitItem(HidUnit unit)
        {
            this.Unit = unit;

            this.Unit.Validate();
        }

        /// <summary>
        /// Gets the associated Unit.
        /// </summary>
        public HidUnit Unit { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Unit({this.Unit})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            byte[] itemBytes = new byte[4];

            itemBytes[0] |= (byte)this.Unit.SystemKind;
            itemBytes[0] |= (byte)(HidConstants.ExponentToWireCode(this.Unit.LengthExponent) << 4);

            itemBytes[1] |= HidConstants.ExponentToWireCode(this.Unit.MassExponent);
            itemBytes[1] |= (byte)(HidConstants.ExponentToWireCode(this.Unit.TimeExponent) << 4);

            itemBytes[2] |= HidConstants.ExponentToWireCode(this.Unit.TemperatureExponent);
            itemBytes[2] |= (byte)(HidConstants.ExponentToWireCode(this.Unit.CurrentExponent) << 4);

            itemBytes[3] |= HidConstants.ExponentToWireCode(this.Unit.LuminousIntensityExponent);

            return EncodeToBytesWithTruncation(BitConverter.ToUInt32(itemBytes, 0));
        }
    }
}
