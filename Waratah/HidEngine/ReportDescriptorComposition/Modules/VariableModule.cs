// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;

    /// <summary>
    /// Called Variable, as the value in the report can be any in the defined range. (defined by logicalMin/Max).
    /// In comparison, an Array, may only be an index (0 -> n-1) into the array of Usages defined by the UsageRange,
    /// hence, an Array is an array of bools/Buttons, with Usages optionally asserted.
    /// </summary>
    public class VariableModule : BaseElementDataVariableModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableModule"/> class.
        /// </summary>
        /// <param name="usage">Usage associated with this module.</param>
        /// <param name="count">The number of instances of this module that appear in the report.</param>
        /// <param name="logicalRange">The logical range of this module. May be null.</param>
        /// <param name="physicalRange">The physical range of this module. May be null.</param>
        /// <param name="sizeInBits">The size of this module in bits. May be null.</param>
        /// <param name="moduleFlags">The flags of this variable module. May be null.</param>
        /// <param name="unit">The unit of this module.  May be null.</param>
        /// <param name="usageUnitMultiplier">The unit multiplier of this module.  May be null.</param>
        /// <param name="name">Logical name of this module. Optional.</param>
        /// <param name="parent">The parent module of this module.  There must be a <see cref="ReportModule"/> somewhere in the ancestry.</param>
        public VariableModule(
            HidUsageId usage,
            int? count,
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
            this.Usage = usage ?? throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorModuleNoUsage);

            if (count.HasValue)
            {
                this.Count = count.Value;
            }
        }

        /// <summary>
        /// Gets the associated Usage.
        /// </summary>
        public HidUsageId Usage { get; }

        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            /// This is just a specialization of the more generic <see cref="GenerateDescriptorItems(List{HidUsageId}, int)"/>,
            /// where only 1 (i.e. this) Usage exists.
            return this.GenerateDescriptorItems(new List<HidUsageId> { this.Usage }, this.Count);
        }

        /// <summary>
        /// When variable module have all the same attributes (e.g. LogicalMaximum, Size, etc...),
        /// they can (and should) be combined together to proactively save descriptor space and increase brevity.<br/>
        /// <code>
        /// UsagePage(...)<br/>
        /// UsageId(...)<br/>
        /// UsageId(...)<br/>
        /// UsageId(...)<br/>
        /// LogicalMaximum(...)<br/>
        /// ReportCount(...)<br/>
        /// ReportSize(...)<br/>
        /// Input(...)<br/>
        /// </code>
        /// </summary>
        /// <param name="combineUsages">Usages to combine together into a variable item.</param>
        /// <param name="combinedReportCount">Combined ReportCount for this item.</param>
        /// <returns>List of HID Report Items describing this <see cref="VariableModule"/>.</returns>
        public List<ShortItem> GenerateDescriptorItems(List<HidUsageId> combineUsages, int combinedReportCount)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new UsagePageItem(this.Usage.Page.Id));

            // Combine all the UsageIds together.
            foreach (HidUsageId usage in combineUsages)
            {
                descriptorItems.Add(new UsageItem(usage.Page.Id, usage.Id, false));
            }

            int cachedCount = this.Count;

            // Temporarily set the base Count to the # items that are being combined.
            // This allows the existing generation logic to work.
            this.Count = combinedReportCount;

            descriptorItems.AddRange(this.GenerateReportDataVariableItems(HidConstants.MainDataItemGroupingKind.Variable));

            // Restore previous count.
            this.Count = cachedCount;

            return descriptorItems;
        }

        /// <summary>
        /// Whether this module is combinable with another.
        /// </summary>
        /// <returns>Bool indicating whether combination is possible at all.</returns>
        public bool IsCanBeDescriptorCombined()
        {
            // Count must be 1 to be considered for combination.
            return this.Count == 1;
        }

        /// <summary>
        /// Whether this module can be 'combined' with another.
        /// </summary>
        /// <param name="other">Other module to attempt to combine with.</param>
        /// <returns>Bool indicating whether combination is possible.</returns>
        public bool IsCanBeDescriptorCombined(VariableModule other)
        {
            // Can only be descriptor-combined if this corresponds to a single field.
            bool isCanBeCombined = (other.IsCanBeDescriptorCombined() == this.IsCanBeDescriptorCombined());

            // Validate all other fields are the same, except for UsageId; where it being different is the whole point...
            bool isSameUsagePage = (other.Usage.Page.Id == this.Usage.Page.Id);
            bool isSameSizeInBits = (other.SizeInBits == this.SizeInBits);
            bool isSameLogicalMinimum = (other.LogicalMinimum == this.LogicalMinimum);
            bool isSameLogicalMaximum = (other.LogicalMaximum == this.LogicalMaximum);
            bool isSamePhysicalMinimum = (other.PhysicalMinimum == this.PhysicalMinimum);
            bool isSamePhysicalMaximum = (other.PhysicalMaximum == this.PhysicalMaximum);
            bool isSameIsPhysicalRangeUndefined = (other.IsPhysicalRangeUndefined == this.IsPhysicalRangeUndefined);
            bool isSameInterpretedPhysicalMinimum = (other.PhysicalMinimumInterpreted == this.PhysicalMinimumInterpreted);
            bool isSameInterpretedPhysicalMaximum = (other.PhysicalMaximumInterpreted == this.PhysicalMaximumInterpreted);
            bool isSameUnit = this.Unit?.Equals(other.Unit) ?? (this.Unit == null && other.Unit == null);
            bool isSameUsageUnitMultiplier = (other.UsageUnitMultiplier == this.UsageUnitMultiplier);
            bool isSameReportType = (other.ParentReportKind == this.ParentReportKind);
            bool isSameModuleFlags = DescriptorModuleFlags.IsEquivalentTo(this.ModuleFlags, other.ModuleFlags);

            return isCanBeCombined &&
                isSameUsagePage &&
                isSameSizeInBits &&
                isSameLogicalMinimum &&
                isSameLogicalMaximum &&
                isSamePhysicalMinimum &&
                isSamePhysicalMaximum &&
                isSameIsPhysicalRangeUndefined &&
                isSameInterpretedPhysicalMinimum &&
                isSameInterpretedPhysicalMaximum &&
                isSameUnit &&
                isSameUsageUnitMultiplier &&
                isSameReportType &&
                isSameModuleFlags;
        }
    }
}
