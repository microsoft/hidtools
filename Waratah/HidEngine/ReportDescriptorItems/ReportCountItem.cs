// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidEngine.Properties;
    using HidSpecification;

    /// <summary>
    /// Defines the number of report items.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.ReportCount)]
    public class ReportCountItem : ShortItem
    {
        private UInt32 count = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportCountItem"/> class.
        /// </summary>
        /// <param name="count">Number of report items.</param>
        public ReportCountItem(Int32 count)
        {
            this.Count = (UInt32)count;
        }

        /// <summary>
        /// Gets the number of report items.
        /// </summary>
        public UInt32 Count
        {
            get
            {
                return this.count;
            }

            private set
            {
                if ((value > HidConstants.ReportCountMaximum) || (value < HidConstants.ReportCountMinimum))
                {
                    throw new ArgumentOutOfRangeException(string.Format(Resources.ExceptionItemInvalidCount, value));
                }

                this.count = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ReportCount({this.Count})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Count);
        }
    }
}
