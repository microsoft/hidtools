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
    /// Base for all Items that convey data in a report. (e.g. VariableItem, LogicalCollections).
    /// <see cref="PaddingModule"/> does not inherit from this as it doesn't contain data.
    /// </summary>
    public abstract class BaseElementDataModule : BaseElementModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElementDataModule"/> class.
        /// </summary>
        /// <param name="name">Logical name of this module.</param>
        /// <param name="parent">The parent module of this module.</param>
        public BaseElementDataModule(string name, BaseModule parent)
            : base(parent)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the minimum logical value that this item can express.
        /// This must be within the bounds expressible by the number of bits.
        /// </summary>
        public Int32 LogicalMinimum { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum logical value that this item can express.
        /// This must be within the bounds expressible by the number of bits.
        /// </summary>
        public Int32 LogicalMaximum { get; protected set; }

        /// <summary>
        /// Gets or sets the 'name' of this item.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Generates <see cref="ShortItem"/>s common to all ReportDataItems.
        /// </summary>
        /// <param name="groupingKind">Kind or group this Item represents.</param>
        /// <returns><see cref="ShortItem"/>s common to all ReportDataItems.</returns>
        protected List<ShortItem> GenerateReportDataItems(HidConstants.MainDataItemGroupingKind groupingKind)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new LogicalMinimumItem(this.LogicalMinimum));

            descriptorItems.Add(new LogicalMaximumItem(this.LogicalMaximum));

            descriptorItems.AddRange(this.GenerateReportItems(groupingKind));

            return descriptorItems;
        }

        /// <summary>
        /// Validates the flag is null when the parent report is one of supplied kinds.
        /// </summary>
        /// <param name="moduleFlagPropertyName">Name of the flag property to validate.</param>
        /// <param name="reportKinds">ReportKinds that do not support this flag.</param>
        protected void ValidateModuleFlagNullForReportKinds(string moduleFlagPropertyName, List<ReportKind> reportKinds)
        {
            PropertyInfo flagProperty = typeof(DescriptorModuleFlags).GetProperty(moduleFlagPropertyName);

            if (reportKinds != null)
            {
                foreach (ReportKind kind in reportKinds)
                {
                    object flagValue = flagProperty.GetValue(this.ModuleFlags);

                    if (this.ParentReportKind == kind && flagValue != null)
                    {
                        throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorArrayModuleInvalidFlag, flagValue, this.ParentReportKind);
                    }
                }
            }
        }
    }
}
