// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using System;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Minimum UsageId for a Usage range.
    /// </summary>
    [LocalItem(HidConstants.LocalItemKind.UsageMinimum)]
    public class UsageMinimumItem : UsageItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageMinimumItem"/> class.
        /// </summary>
        /// <param name="usagePage">Usage page of the item.</param>
        /// <param name="usageId">Usage id of the item.</param>
        /// <param name="extendedMode">Whether this Usage is in extended mode.</param>
        public UsageMinimumItem(UInt16 usagePage, UInt16 usageId, bool extendedMode)
            : base(usagePage, usageId, extendedMode)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsExtended)
            {
                return $"UsagePage({this.UsagePage}) - UsageIdMin({this.UsageId})";
            }
            else
            {
                return $"UsageIdMin({HidUsageTableDefinitions.GetInstance().TryFindUsageId(this.UsagePage, this.UsageId)})";
            }
        }
    }
}
