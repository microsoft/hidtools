// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System.Collections.Generic;
    using System.Reflection;
    using HidSpecification;
    using Medallion;

    /// <summary>
    /// Defines a Input MainData item.
    /// </summary>
    [MainItem(HidConstants.MainItemKind.Input)]
    public class InputItem : ShortItem
    {
        // Caching flag properties as reflection is expensive.
        private static List<PropertyInfo> itemKindProperties = new List<PropertyInfo>();

        static InputItem()
        {
            IEnumerable<PropertyInfo> properties = typeof(InputItem).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                itemKindProperties.Add(property);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputItem"/> class.
        /// </summary>
        public InputItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputItem"/> class.
        /// </summary>
        /// <param name="modificationKind">Kind of modification.</param>
        /// <param name="groupingKind">Kind of grouping.</param>
        /// <param name="relationKind">Kind of relation.</param>
        /// <param name="wrapping">Kind of wrapping.</param>
        /// <param name="linearityKind">Kind of linearity.</param>
        /// <param name="preferenceStateKind">Kind of preference.</param>
        /// <param name="meaningfulDataKind">Kind of meaningfuldata.</param>
        /// <param name="contingentKind">Kind of contingent.</param>
        public InputItem(
            HidConstants.MainDataItemModificationKind modificationKind,
            HidConstants.MainDataItemGroupingKind groupingKind,
            HidConstants.MainDataItemRelationKind relationKind,
            HidConstants.MainDataItemWrappingKind wrapping,
            HidConstants.MainDataItemLinearityKind linearityKind,
            HidConstants.MainDataItemPreferenceStateKind preferenceStateKind,
            HidConstants.MainDataItemMeaningfulDataKind meaningfulDataKind,
            HidConstants.MainDataItemContingentKind contingentKind)
        {
            this.ModificationKind = modificationKind;
            this.GroupingKind = groupingKind;
            this.RelationKind = relationKind;
            this.WrappingKind = wrapping;
            this.LinearityKind = linearityKind;
            this.PreferenceStateKind = preferenceStateKind;
            this.MeaningfulDataKind = meaningfulDataKind;
            this.ContingentKind = contingentKind;
        }

        /// <summary>
        /// Gets or sets the ModificationKind.
        /// </summary>
        public HidConstants.MainDataItemModificationKind ModificationKind { get; set; }

        /// <summary>
        /// Gets or sets the GroupingKind.
        /// </summary>
        public HidConstants.MainDataItemGroupingKind GroupingKind { get; set; }

        /// <summary>
        /// Gets or sets the RelationKind.
        /// </summary>
        public HidConstants.MainDataItemRelationKind RelationKind { get; set; }

        /// <summary>
        /// Gets or sets the WrappingKind.
        /// </summary>
        public HidConstants.MainDataItemWrappingKind WrappingKind { get; set; }

        /// <summary>
        /// Gets or sets the LinearityKind.
        /// </summary>
        public HidConstants.MainDataItemLinearityKind LinearityKind { get; set; }

        /// <summary>
        /// Gets or sets the PreferenceStateKind.
        /// </summary>
        public HidConstants.MainDataItemPreferenceStateKind PreferenceStateKind { get; set; }

        /// <summary>
        /// Gets or sets the MeaningfulDataKind.
        /// </summary>
        public HidConstants.MainDataItemMeaningfulDataKind MeaningfulDataKind { get; set; }

        /// <summary>
        /// Gets or sets the ContingentKind.
        /// </summary>
        public HidConstants.MainDataItemContingentKind ContingentKind { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                "Input({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                this.ModificationKind,
                this.GroupingKind,
                this.RelationKind,
                this.WrappingKind,
                this.LinearityKind,
                this.PreferenceStateKind,
                this.MeaningfulDataKind,
                this.ContingentKind);
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            // At a minimum size must be 1.
            byte itemSize = 1;

            if (this.ContingentKind == HidConstants.MainDataItemContingentKind.BufferedBytes)
            {
                // If the bufferBytes is set, then another byte is required.
                itemSize++;
            }

            byte[] itemBytes = new byte[itemSize];

            foreach (PropertyInfo property in itemKindProperties)
            {
                HidConstants.MainDataItemAttribute attribute = property.PropertyType.GetCustomAttribute<HidConstants.MainDataItemAttribute>();

                int value = (int)property.GetValue(this);

                if (value > 0)
                {
                    itemBytes[attribute.ByteNumber] = Bits.SetBit(itemBytes[attribute.ByteNumber], attribute.BitPosition);
                }
            }

            return itemBytes;
        }
    }
}
