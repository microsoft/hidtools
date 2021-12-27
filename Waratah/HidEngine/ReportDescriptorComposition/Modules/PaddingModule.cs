// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;

    /// <summary>
    /// Describes an abstract padding module, used to manually byte-align descriptor modules.
    /// </summary>
    public class PaddingModule : BaseElementModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaddingModule"/> class.
        /// </summary>
        /// <param name="sizeInBits">Size of padding.</param>
        /// <param name="parent">The parent module of this module.</param>
        public PaddingModule(int sizeInBits, BaseModule parent)
            : base(parent)
        {
            this.NonAdjustedSizeInBits = sizeInBits;

            this.Count = 1;

            this.ModuleFlags.ModificationKind = HidConstants.MainDataItemModificationKind.Constant;
            this.ModuleFlags.RelationKind = HidConstants.MainDataItemRelationKind.Absolute;
            this.ModuleFlags.WrappingKind = HidConstants.MainDataItemWrappingKind.NoWrap;
            this.ModuleFlags.LinearityKind = HidConstants.MainDataItemLinearityKind.Linear;
            this.ModuleFlags.PreferenceStateKind = HidConstants.MainDataItemPreferenceStateKind.PreferredState;
            this.ModuleFlags.MeaningfulDataKind = HidConstants.MainDataItemMeaningfulDataKind.NoNullPosition;
            this.ModuleFlags.ContingentKind = HidConstants.MainDataItemContingentKind.BitField;

            if (this.ParentReportKind != ReportKind.Input)
            {
                this.ModuleFlags.VolatilityKind = HidConstants.MainDataItemVolatilityKind.NonVolatile;
            }
        }

        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            return this.GenerateReportItems(HidConstants.MainDataItemGroupingKind.Variable);
        }
    }
}
