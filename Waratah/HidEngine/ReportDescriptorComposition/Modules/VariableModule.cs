// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

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

        /// <summary>
        /// Combines provided VariableModules, and generates all Descriptor Items to describe the combination.
        /// </summary>
        /// <param name="combinableVariableModules">VariableModules to combine and generate items for.</param>
        /// <returns>List of Items.</returns>
        public static List<ShortItem> GenerateDescriptorItemsForCombinable(List<VariableModule> combinableVariableModules)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            if (combinableVariableModules.Count == 1)
            {
                // Always safe to access the first item as we know it's size == 1.
                descriptorItems.AddRange(combinableVariableModules[0].GenerateDescriptorItems(true));
            }
            else if (combinableVariableModules.Count > 1)
            {
                List<HidUsageId> combinableUsages = new List<HidUsageId>();

                foreach (VariableModule item in combinableVariableModules)
                {
                    combinableUsages.Add(item.Usage);
                }

                // Always safe to access the first item as we know it's size > 0.
                descriptorItems.AddRange(combinableVariableModules[0].GenerateDescriptorItems(combinableUsages, combinableVariableModules.Count));
            }

            return descriptorItems;
        }

        /// <summary>
        /// Whether this VariableModule is combinable with every module is the provided list.
        /// </summary>
        /// <param name="combinableVariableModules">Modules to test this module is combinable with.</param>
        /// <returns>Whether this VariableModule is combinable.</returns>
        public bool IsDescriptorCombinableWith(List<VariableModule> combinableVariableModules)
        {
            // Nothing in the buffer to combine with.
            if (combinableVariableModules.Count == 0)
            {
                return this.IsCanBeDescriptorCombined();
            }

            // Validate it can be combined will all other VariableModules in the buffer.
            // Note: Should only really need to validate against 1 of them as the combinable property (should be) transitive.
            bool isCanBeCombined = true;
            foreach (VariableModule combinableItem in combinableVariableModules)
            {
                isCanBeCombined &= combinableItem.IsCanBeDescriptorCombined(this);
            }

            return isCanBeCombined;
        }

        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            /// This is just a specialization of the more generic <see cref="GenerateDescriptorItems(List{HidUsageId}, int)"/>,
            /// where only 1 (i.e. this) Usage exists.
            return this.GenerateDescriptorItems(new List<HidUsageId> { this.Usage }, this.Count);
        }

        /// <summary>
        /// Whether this module is combinable with another.
        /// </summary>
        /// <returns>Bool indicating whether combination is possible at all.</returns>
        public bool IsCanBeDescriptorCombined()
        {
            // Count must be 1 to be considered for combination.
            //
            // hid_11:6.2.2.8 "Local Items" - "While Local items do not carry over to the next Main item,
            // they may apply to more than one control within a single item. For example, if an Input item defining
            // five controls is preceded by three Usage tags, the three usages would be assigned sequentially to the
            // first three controls, and the third usage would also be assigned to the fourth and fifth controls."
            //
            // i.e. Cannot combine modules with ReportCount > 1 as HID parsers will attribute
            // the last Usage with the 'balance' of ReportCount.
            return (this.Count == 1);
        }

        /// <summary>
        /// Whether this module can be 'combined' with another.
        /// </summary>
        /// <param name="other">Other module to attempt to combine with.</param>
        /// <returns>Bool indicating whether combination is possible.</returns>
        public bool IsCanBeDescriptorCombined(VariableModule other)
        {
            bool isCanBeCombined = (other.IsCanBeDescriptorCombined() && this.IsCanBeDescriptorCombined());

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
        /// <param name="combinableUsages">Usages to combine together into a variable item.</param>
        /// <param name="combinedReportCount">Combined ReportCount for this item.</param>
        /// <returns>List of HID Report Items describing this <see cref="VariableModule"/>.</returns>
        private List<ShortItem> GenerateDescriptorItems(List<HidUsageId> combinableUsages, int combinedReportCount)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new UsagePageItem(this.Usage.Page.Id));

            // Combine all the UsageIds together.
            foreach (HidUsageId usage in combinableUsages)
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
    }
}
