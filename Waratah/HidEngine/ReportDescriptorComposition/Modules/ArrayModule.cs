// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;

    /// <summary>
    /// Called Array, because the value in the Report is an index into the array of Usages. (defined by UsageRange).
    /// Since only the presence/absence of the index is given, it is effectively a bool/button value.  If not asserted, assumed to be false.
    /// LogicalMinimum/Maximum is inferred from number of Usages defined.
    /// In comparison, Variable can be any variable value.
    ///
    /// From experimentation with the Windows HID parser, Arrays may ONLY consistent of Usages from a single UsagePage, if
    /// non-extended Usages/UsagePages are used.  To mix Usages from different UsagePages, extended Usages must be used throughout
    /// the declaration of the main item.
    /// Mixing extended/non-extended Usages and UsageMin/Max is permitted.
    /// For now however, only the simple case of Usages from a single UsagePage (no extended Usages supported) will be supported.
    /// </summary>
    public class ArrayModule : BaseElementDataModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayModule"/> class.
        /// </summary>
        /// <param name="usageStart">The start Usage of the range defined by this Array.</param>
        /// <param name="usageEnd">The end Usage of the range defined by this Array.</param>
        /// <param name="count">The number indexes that can be specifed for this Array.</param>
        /// <param name="moduleFlags">The flags of this Array.</param>
        /// <param name="name">Logical name of this module. Optional.</param>
        /// <param name="parent">The parent module of this module.  There must be a <see cref="ReportModule"/> somewhere in the ancestry.</param>
        public ArrayModule(
            HidUsageId usageStart,
            HidUsageId usageEnd,
            int? count,
            DescriptorModuleFlags moduleFlags,
            string name,
            BaseModule parent)
                : base(name, parent)
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

            if (count.HasValue)
            {
                this.Count = count.Value;
            }

            // 0 is reserved to be invalid/null-state, so must have additional index for that.
            // TODO: Permit specification of null-value.
            int rangeDiff = (this.UsageEnd.Id - this.UsageStart.Id) + 1;

            this.NonAdjustedSizeInBits = (int)Math.Floor(Math.Log(rangeDiff, 2) + 1);

            this.LogicalMinimum = HidConstants.LogicalMinimumValueArrayItem;
            this.LogicalMaximum = rangeDiff;

            // For Array InputItems, only the Data/Constant, and Absolute/Relative attributes apply. (HID1_11 - PG32)
            this.ModuleFlags = moduleFlags;

            List<ReportKind> invalidReportKinds = new List<ReportKind> { ReportKind.Input };
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.VolatilityKind), invalidReportKinds);
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.WrappingKind), invalidReportKinds);
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.LinearityKind), invalidReportKinds);
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.PreferenceStateKind), invalidReportKinds);
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.MeaningfulDataKind), invalidReportKinds);
            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.ContingentKind), invalidReportKinds);
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

            descriptorItems.AddRange(this.GenerateReportDataItems(HidConstants.MainDataItemGroupingKind.Array));

            return descriptorItems;
        }
    }
}
