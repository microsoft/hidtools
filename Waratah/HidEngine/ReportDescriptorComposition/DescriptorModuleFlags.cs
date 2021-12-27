// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition
{
    using HidSpecification;

    /// <summary>
    /// Container for all Flags that apply to Input/Output/Feature items (HID1_11-6.2.2.5).
    /// </summary>
    public class DescriptorModuleFlags
    {
        /// <summary>
        /// Gets or sets a value indicating whether the item is data or constant.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemModificationKind? ModificationKind { get; set; }

        /// <summary>
        /// Gets the interpreted ModificationKind.
        /// This is how the caller should interpret the ModificationKind.
        /// </summary>
        public HidConstants.MainDataItemModificationKind ModificationKindInterpreted
        {
            get
            {
                return this.ModificationKind ?? HidConstants.MainDataItemModificationKind.Data;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is absolute or relative.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemRelationKind? RelationKind { get; set; }

        /// <summary>
        /// Gets the interpreted RelationKind.
        /// This is how the caller should interpret the RelationKind.
        /// </summary>
        public HidConstants.MainDataItemRelationKind RelationKindInterpreted
        {
            get
            {
                return this.RelationKind ?? HidConstants.MainDataItemRelationKind.Absolute;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data rolls-over when reaching either extreme high/low.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemWrappingKind? WrappingKind { get; set; }

        /// <summary>
        /// Gets the interpreted WrappingKind.
        /// This is how the caller should interpret the WrappingKind.
        /// </summary>
        public HidConstants.MainDataItemWrappingKind WrappingKindInterpreted
        {
            get
            {
                return this.WrappingKind ?? HidConstants.MainDataItemWrappingKind.NoWrap;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data represents a linear relationship with the raw-data.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemLinearityKind? LinearityKind { get; set; }

        /// <summary>
        /// Gets the interpreted LinearityKind.
        /// This is how the caller should interpret the LinearityKind.
        /// </summary>
        public HidConstants.MainDataItemLinearityKind LinearityKindInterpreted
        {
            get
            {
                return this.LinearityKind ?? HidConstants.MainDataItemLinearityKind.Linear;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control underlying the item has a preferred state to which it will return when
        /// the user is not physically interacting with the control.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemPreferenceStateKind? PreferenceStateKind { get; set; }

        /// <summary>
        /// Gets the interpreted PreferenceStateKind.
        /// This is how the caller should interpret the PreferenceStateKind.
        /// </summary>
        public HidConstants.MainDataItemPreferenceStateKind PreferenceStateKindInterpreted
        {
            get
            {
                return this.PreferenceStateKind ?? HidConstants.MainDataItemPreferenceStateKind.PreferredState;
            }
        }

        // TODO: For null state, validate there is room in the field-size to permit a null value outside of the logicalMinMax.
        // TODO: have a tag to specify what the null state will be, and then auto-generate CPP accessors to include it.

        /// <summary>
        /// Gets or sets a value indicating whether the control has a state in which it is not sending meaningful data.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemMeaningfulDataKind? MeaningfulDataKind { get; set; }

        /// <summary>
        /// Gets the interpreted MeaningfulDataKind.
        /// This is how the caller should interpret the MeaningfulDataKind.
        /// </summary>
        public HidConstants.MainDataItemMeaningfulDataKind MeaningfulDataKindInterpreted
        {
            get
            {
                return this.MeaningfulDataKind ?? HidConstants.MainDataItemMeaningfulDataKind.NoNullPosition;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value can/should be changed by the host.
        /// Note: This is <emp>NOT</emp> valid for an InputReport.
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemVolatilityKind? VolatilityKind { get; set; }

        /// <summary>
        /// Gets the interpreted VolatilityKind.
        /// This is how the caller should interpret the VolatilityKind.
        /// </summary>
        public HidConstants.MainDataItemVolatilityKind VolatilityKindInterpreted
        {
            get
            {
                return this.VolatilityKind ?? HidConstants.MainDataItemVolatilityKind.NonVolatile;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be interpreted as a numeric value, or opaque bytes (whose true meaning is
        /// understood only by the calling application).
        /// Nullable, so we can determine if this flag was ever set by the caller.
        /// </summary>
        public HidConstants.MainDataItemContingentKind? ContingentKind { get; set; }

        /// <summary>
        /// Gets the interpreted ContingentKind.
        /// This is how the caller should interpret the ContingentKind.
        /// </summary>
        public HidConstants.MainDataItemContingentKind ContingentKindInterpreted
        {
            get
            {
                return this.ContingentKind ?? HidConstants.MainDataItemContingentKind.BitField;
            }
        }

        /// <summary>
        /// Determines if two <see cref="DescriptorModuleFlags"/> have equivalent interpreted flag states for each flag.
        /// </summary>
        /// <param name="lhs">LHS <see cref="DescriptorModuleFlags"/>.</param>
        /// <param name="rhs">RHS <see cref="DescriptorModuleFlags"/>.</param>
        /// <returns>Bool indicating whether the two are equivalent.</returns>
        public static bool IsEquivalentTo(DescriptorModuleFlags lhs, DescriptorModuleFlags rhs)
        {
            bool isSameModificationFlag = lhs.ModificationKindInterpreted == rhs.ModificationKindInterpreted;
            bool isSameRelationFlag = lhs.RelationKindInterpreted == rhs.RelationKindInterpreted;
            bool isSameWrappingFlag = lhs.WrappingKindInterpreted == rhs.WrappingKindInterpreted;
            bool isSameLinearityFlag = lhs.LinearityKindInterpreted == rhs.LinearityKindInterpreted;
            bool isSamePreferredFlag = lhs.PreferenceStateKindInterpreted == rhs.PreferenceStateKindInterpreted;
            bool isSameMeaningfulFlag = lhs.MeaningfulDataKindInterpreted == rhs.MeaningfulDataKindInterpreted;
            bool isSameVolatilityFlag = lhs.VolatilityKindInterpreted == rhs.VolatilityKindInterpreted;
            bool isSameContingentFlag = lhs.ContingentKindInterpreted == rhs.ContingentKindInterpreted;

            bool isEquivalent = (
                isSameModificationFlag &&
                isSameRelationFlag &&
                isSameWrappingFlag &&
                isSameLinearityFlag &&
                isSamePreferredFlag &&
                isSameMeaningfulFlag &&
                isSameVolatilityFlag &&
                isSameContingentFlag);

            return isEquivalent;
        }
    }
}
