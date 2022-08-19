// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Defines the Id of the Report.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.ReportId)]
    public class ReportIdItem : ShortItem
    {
        private UInt32 id = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportIdItem"/> class.
        /// </summary>
        /// <param name="reportId">Id of the report.</param>
        public ReportIdItem(UInt32 reportId)
        {
            this.Id = reportId;
        }

        /// <summary>
        /// Gets the Id of the report.
        /// </summary>
        public UInt32 Id
        {
            get
            {
                return this.id;
            }

            private set
            {
                if (value < HidConstants.ReportIdMinimum || value > HidConstants.ReportIdMaximum)
                {
                    throw new ArgumentOutOfRangeException(string.Format(Resources.ExceptionItemInvalidReportId, value, HidConstants.ReportIdMinimum, HidConstants.ReportIdMaximum));
                }

                this.id = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ReportId({this.Id})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            return EncodeToBytesWithTruncation(this.Id);
        }
    }
}
