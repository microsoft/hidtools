// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidSpecification;

    /// <summary>
    /// Maximum UsageId for a Usage range.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.UsageMaximum)]
    public class UsageMaximumItem : UsageItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageMaximumItem"/> class.
        /// </summary>
        /// <param name="usagePage">Usage page of the item.</param>
        /// <param name="usageId">Usage id of the item.</param>
        /// <param name="extendedMode">Whether this Usage is in extended mode.</param>
        public UsageMaximumItem(UInt16 usagePage, UInt16 usageId, bool extendedMode)
            : base(usagePage, usageId, extendedMode)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsExtended)
            {
                return $"UsagePage({this.UsagePage}) - UsageIdMax({this.UsageId})";
            }
            else
            {
                return $"UsageIdMax({HidUsageTableDefinitions.GetInstance().TryFindUsageId(this.UsagePage, this.UsageId)})";
            }
        }
    }
}
