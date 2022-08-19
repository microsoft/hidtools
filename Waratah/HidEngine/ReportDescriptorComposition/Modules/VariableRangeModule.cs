// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Called VariableRange, as the value in the report can be any in the defined range. (defined by logicalMin/Max).
    /// Similar to a Variable, except multiple Usages are defined (by the range), and number of items is implicit by the range.
    /// This allows many Variable items to be defined, with a smaller descriptor.
    /// </summary>
    public class VariableRangeModule : BaseElementDataVariableModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableRangeModule"/> class.
        /// </summary>
        /// <param name="usageStart">The start Usage of the range defined by this module.</param>
        /// <param name="usageEnd">The end Usage of the range defined by this module.</param>
        /// <param name="logicalRange">The logical range of this module. May be null.</param>
        /// <param name="physicalRange">The physical range of this module. May be null.</param>
        /// <param name="sizeInBits">The size of this module in bits. May be null.</param>
        /// <param name="moduleFlags">The flags of this variable module. May be null.</param>
        /// <param name="unit">The unit of this module.  May be null.</param>
        /// <param name="usageUnitMultiplier">The unit multiplier of this module.  May be null.</param>
        /// <param name="name">Logical name of this module. Optional.</param>
        /// <param name="parent">The parent module of this module.  There must be a <see cref="ReportModule"/> somewhere in the ancestry.</param>
        public VariableRangeModule(
            HidUsageId usageStart,
            HidUsageId usageEnd,
            DescriptorRange logicalRange,
            DescriptorRange physicalRange,
            int? sizeInBits,
            DescriptorModuleFlags moduleFlags,
            HidUnit unit,
            double? usageUnitMultiplier,
            string name,
            BaseModule parent)
                : base(logicalRange, physicalRange, sizeInBits, moduleFlags, unit, usageUnitMultiplier, name, parent)
        {
            this.UsageStart = usageStart ?? throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorUsageStartMissing);

            this.UsageEnd = usageEnd ?? throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorUsageEndMissing);

            // Naturally, for a valid range, the Usages must be referring to the same page.
            if (!this.UsageStart.Page.Equals(this.UsageEnd.Page))
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorUsagePagesDiffer, this.UsageStart.Page, this.UsageEnd.Page);
            }

            if (this.UsageStart.Id.CompareTo(this.UsageEnd.Id) >= 0)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorUsageStartIdGreaterThanEnd, this.UsageStart, this.UsageEnd);
            }

            // All Usages within the range must exist (i.e. must be contiguous).
            HidUsageTableDefinitions.GetInstance().ValidateRange(this.UsageStart, this.UsageEnd);

            this.Count = (this.UsageEnd.Id - this.UsageStart.Id) + 1;
        }

        /// <summary>
        /// Gets the start Usage of the range.
        /// </summary>
        public HidUsageId UsageStart { get; }

        /// <summary>
        /// Gets the end Usage of the range.
        /// </summary>
        public HidUsageId UsageEnd { get; }

        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new UsagePageItem(this.UsageStart.Page.Id));

            descriptorItems.Add(new UsageMinimumItem(this.UsageStart.Page.Id, this.UsageStart.Id, false));

            descriptorItems.Add(new UsageMaximumItem(this.UsageEnd.Page.Id, this.UsageEnd.Id, false));

            descriptorItems.AddRange(this.GenerateReportDataVariableItems(HidConstants.MainDataItemGroupingKind.Variable));

            return descriptorItems;
        }
    }
}
