// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;

    /// <summary>
    /// Base for all modules contained within a report (e.g. <see cref="VariableModule"/>, <see cref="PaddingModule"/>).
    /// </summary>
    public abstract class BaseElementModule : BaseModule
    {
        private int nonAdjustedSizeSet = 0;
        private int count = HidConstants.ReportCountMinimum;
        private DescriptorModuleFlags flags = new DescriptorModuleFlags();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElementModule"/> class.
        /// </summary>
        /// <param name="parent">The parent module of this module.</param>
        public BaseElementModule(BaseModule parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the flags for this module.
        /// Guaranteed to never be null.
        /// </summary>
        public DescriptorModuleFlags ModuleFlags
        {
            get
            {
                return this.flags;
            }

            protected set
            {
                this.flags = value ?? new DescriptorModuleFlags();
            }
        }

        /// <summary>
        /// Gets or sets the number of fields belonging to this item. (i.e. ReportCount).
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }

            protected set
            {
                if ((value > HidConstants.ReportCountMaximum) || (value < HidConstants.ReportCountMinimum))
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorBaseElementModuleInvalidCount, value);
                }

                this.count = value;
            }
        }

        /// <summary>
        /// Gets the 'actual'/adjusted size of each field belonging to this item.
        /// </summary>
        public int SizeInBits
        {
            get
            {
                return Settings.GetInstance().CalculateSizeInBitsWithPacking(this.NonAdjustedSizeInBits);
            }
        }

        /// <summary>
        /// Gets or sets the 'requested' size for this module; what the size would be if Settings has not overridden it.
        /// </summary>
        public int NonAdjustedSizeInBits
        {
            get
            {
                return this.nonAdjustedSizeSet;
            }

            set
            {
                if ((value > HidConstants.SizeInBitsMaximum) || (value < HidConstants.SizeInBitsMinimum))
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorBaseElementModuleInvalidSize, value);
                }

                this.nonAdjustedSizeSet = value;
            }
        }

        /// <summary>
        /// Gets the kind of the report that includes this item.
        /// </summary>
        public ReportKind ParentReportKind
        {
            get
            {
                // Examines all ancestors until a Report is encountered.

                BaseModule parent = this.Parent;

                while (parent != null && parent.GetType() != typeof(ReportModule))
                {
                    parent = parent.Parent;
                }

                if (parent == null)
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorBaseElementModuleCannotFindReport);
                }

                return ((ReportModule)parent).Kind;
            }
        }

        /// <inheritdoc/>
        public override int TotalSizeInBits
        {
            get
            {
                return this.Count * this.SizeInBits;
            }
        }

        /// <inheritdoc/>
        public override int TotalNonAdjustedSizeInBits
        {
            get
            {
                return this.Count * this.NonAdjustedSizeInBits;
            }
        }

        /// <inheritdoc/>
        public override List<BaseElementModule> GetReportElements()
        {
            List<BaseElementModule> leafModules = new List<BaseElementModule>();

            leafModules.Add(this);

            return leafModules;
        }

        /// <summary>
        /// Generates <see cref="ShortItem"/>s common to all children.
        /// </summary>
        /// <param name="groupingKind">Kind or group this module represents.</param>
        /// <returns><see cref="ShortItem"/>s common to all children.</returns>
        protected List<ShortItem> GenerateReportItems(HidConstants.MainDataItemGroupingKind groupingKind)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new ReportCountItem(this.Count));

            descriptorItems.Add(new ReportSizeItem(this.SizeInBits));

            ShortItem reportItemForKind = null;
            switch (this.ParentReportKind)
            {
                case ReportKind.Input:
                {
                    reportItemForKind = new InputItem(
                        this.ModuleFlags.ModificationKindInterpreted,
                        groupingKind,
                        this.ModuleFlags.RelationKindInterpreted,
                        this.ModuleFlags.WrappingKindInterpreted,
                        this.ModuleFlags.LinearityKindInterpreted,
                        this.ModuleFlags.PreferenceStateKindInterpreted,
                        this.ModuleFlags.MeaningfulDataKindInterpreted,
                        this.ModuleFlags.ContingentKindInterpreted);

                    break;
                }

                case ReportKind.Output:
                {
                    reportItemForKind = new OutputItem(
                        this.ModuleFlags.ModificationKindInterpreted,
                        groupingKind,
                        this.ModuleFlags.RelationKindInterpreted,
                        this.ModuleFlags.WrappingKindInterpreted,
                        this.ModuleFlags.LinearityKindInterpreted,
                        this.ModuleFlags.PreferenceStateKindInterpreted,
                        this.ModuleFlags.MeaningfulDataKindInterpreted,
                        this.ModuleFlags.VolatilityKindInterpreted,
                        this.ModuleFlags.ContingentKindInterpreted);

                    break;
                }

                case ReportKind.Feature:
                {
                    reportItemForKind = new FeatureItem(
                        this.ModuleFlags.ModificationKindInterpreted,
                        groupingKind,
                        this.ModuleFlags.RelationKindInterpreted,
                        this.ModuleFlags.WrappingKindInterpreted,
                        this.ModuleFlags.LinearityKindInterpreted,
                        this.ModuleFlags.PreferenceStateKindInterpreted,
                        this.ModuleFlags.MeaningfulDataKindInterpreted,
                        this.ModuleFlags.VolatilityKindInterpreted,
                        this.ModuleFlags.ContingentKindInterpreted);

                    break;
                }

                default:
                {
                    System.Environment.FailFast($"Unknown ReportKind {this.ParentReportKind}");
                    break;
                }
            }

            descriptorItems.Add(reportItemForKind);

            return descriptorItems;
        }
    }
}