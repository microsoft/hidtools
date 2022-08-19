// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Base for all VariableItems that convey data in a report. (e.g. VariableItem, VariableRangeItem).
    /// </summary>
    public abstract class BaseElementDataVariableModule : BaseElementDataModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElementDataVariableModule"/> class.
        /// </summary>
        /// <param name="logicalRange">The logical range of this item. May be null.</param>
        /// <param name="physicalRange">The physical range of this item.  May be null.</param>
        /// <param name="sizeInBits">The size of this item in bits.  May be null.</param>
        /// <param name="moduleFlags">The flags of this variable item. May be null.</param>
        /// <param name="unit">The unit of this module.  May be null.</param>
        /// <param name="usageUnitMultiplier">The unit multiplier of this module.  May be null.</param>
        /// <param name="name">Logical name of this module. Optional.</param>
        /// <param name="parent">The parent module of this module.  There must be a <see cref="ReportModule"/> somewhere in the ancestry.</param>
        public BaseElementDataVariableModule(
            DescriptorRange logicalRange,
            DescriptorRange physicalRange,
            int? sizeInBits,
            DescriptorModuleFlags moduleFlags,
            HidUnit unit,
            double? usageUnitMultiplier,
            string name,
            BaseModule parent)
                : base(name, parent)
        {
            this.ValidateAndSetSizeAndLogicalRange(logicalRange, sizeInBits);

            this.ValidateAndSetPhysicalRange(physicalRange);

            // If moduleFlags is null, the property assignment will create an empty instance with default values.
            this.ModuleFlags = moduleFlags;

            this.Unit = unit;

            this.UsageUnitMultiplier = usageUnitMultiplier;

            this.ValidateModuleFlagNullForReportKinds(nameof(this.ModuleFlags.VolatilityKind), new List<ReportKind> { ReportKind.Input });
        }

        /// <summary>
        /// Gets or sets the minimum physical value that this item can express.
        /// Physical value is the Logical value with a unit factor applied.
        /// If both PhysicalMinimum and PhysicalMaximum are 0, then they are parsed as the same
        /// value as LogicalMinimum/LogicalMaximum.
        /// </summary>
        public Int32 PhysicalMinimum { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum physical value that this item can express.
        /// Physical value is the Logical value with a unit factor applied.
        /// If both PhysicalMinimum and PhysicalMaximum are 0, then they are parsed as the same
        /// value as LogicalMinimum/LogicalMaximum.
        /// </summary>
        public Int32 PhysicalMaximum { get; protected set; }

        /// <summary>
        /// Gets the Unit of this item.
        /// </summary>
        public HidUnit Unit { get; }

        /// <summary>
        /// Gets the UsageUnitMultiplier of this item.
        /// </summary>
        public double? UsageUnitMultiplier { get; }

        /// <summary>
        /// Gets a value indicating whether the PhysicalRange is Undefined.
        /// </summary>
        public bool IsPhysicalRangeUndefined
        {
            get
            {
                bool isPhysicalMaximumUndefined = (this.PhysicalMaximum == HidConstants.PhysicalMaximumUndefinedValue);
                bool isPhysicalMinimumUndefined = (this.PhysicalMinimum == HidConstants.PhysicalMinimumUndefinedValue);

                return (isPhysicalMaximumUndefined && isPhysicalMinimumUndefined);
            }
        }

        /// <summary>
        /// Gets the interpreted PhysicalMinimum.
        /// This is how the caller should interpret the PhysicalMinimum.
        /// </summary>
        /// <example><see cref="PhysicalMinimum"/> is defined as 0, which means it should be interpreted as the value defined in the LogicalMinimum.</example>
        public Int32 PhysicalMinimumInterpreted
        {
            get
            {
                return this.IsPhysicalRangeUndefined ? this.LogicalMinimum : this.PhysicalMinimum;
            }
        }

        /// <summary>
        /// Gets the interpreted PhysicalMinimum.
        /// This is how the caller should interpret the PhysicalMinimum.
        /// </summary>
        /// <example><see cref="PhysicalMaximum"/> is defined as 0, which means it should be interpreted as the value defined in the LogicalMaximum.</example>
        public Int32 PhysicalMaximumInterpreted
        {
            get
            {
                return this.IsPhysicalRangeUndefined ? this.LogicalMaximum : this.PhysicalMaximum;
            }
        }

        /// <summary>
        /// Generates <see cref="ShortItem"/>s common to all child modules.
        /// </summary>
        /// <param name="groupingKind">Kind or group this Item represents.</param>
        /// <returns><see cref="ShortItem"/>s common to all child modules.</returns>
        protected List<ShortItem> GenerateReportDataVariableItems(HidConstants.MainDataItemGroupingKind groupingKind)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new PhysicalMinimumItem(this.PhysicalMinimum));

            descriptorItems.Add(new PhysicalMaximumItem(this.PhysicalMaximum));

            if (this.Unit != null)
            {
                descriptorItems.Add(new UnitItem(this.Unit));

                descriptorItems.Add(new UnitExponentItem(this.Unit.Multiplier));
            }
            else if (this.UsageUnitMultiplier != null)
            {
                descriptorItems.Add(new UnitItem(HidUnitDefinitions.GetInstance().TryFindUnitByName(HidUnitDefinitions.NoneName)));

                descriptorItems.Add(new UnitExponentItem(this.UsageUnitMultiplier.Value));
            }
            else
            {
                descriptorItems.Add(new UnitItem(HidUnitDefinitions.GetInstance().TryFindUnitByName(HidUnitDefinitions.NoneName)));

                descriptorItems.Add(new UnitExponentItem(HidConstants.UnitExponentUndefinedValueMultiplier));
            }

            descriptorItems.AddRange(this.GenerateReportDataItems(groupingKind));

            return descriptorItems;
        }

        private static int MinSignedValueForSize(int size)
        {
            if (size < 2)
            {
                // Need at least 2 bits for a signed value.
                // Unsigned value is permitted to have only 1bit.
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementSizeTooSmallForMaxSigned, size);
            }

            return (int)(Math.Pow(2, size - 1) * -1);
        }

        private static int MaxSignedValueForSize(int size)
        {
            if (size < 2)
            {
                // Need at least 2 bits for a signed value.
                // Unsigned value is permitted to have only 1bit.
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementSizeTooSmallForMaxSigned, size);
            }

            return (int)(Math.Pow(2, size - 1) - 1);
        }

        private static int MaxUnsignedValueForSize(int size)
        {
            // Due to external limitations, unsigned 32bit range is not support, so it's range
            // will be truncated to [0, (2^31)-1], which is the same as 31bit unsigned range.
            if (size == HidConstants.SizeInBitsMaximum)
            {
                size = HidConstants.SizeInBitsMaximum - 1;
            }

            return (int)(Math.Pow(2, size) - 1);
        }

        private static int RequiredBitsForLogicalRange(int start, int end)
        {
            Debug.Assert(start <= end, "Invalid range.");

            // Doesn't matter if end is also negative.
            bool isSigned = (start < 0);

            // TODO: This isn't very efficent...
            for (int i = (isSigned ? 2 : 1); i <= HidConstants.SizeInBitsMaximum; i++)
            {
                double lowerBound = isSigned ? MinSignedValueForSize(i) : 0;
                double upperBound = isSigned ? MaxSignedValueForSize(i) : MaxUnsignedValueForSize(i);

                bool isStartInBounds = (start <= upperBound) && (start >= lowerBound);
                bool isEndInBounds = (end <= upperBound) && (start >= lowerBound);

                if (isStartInBounds && isEndInBounds)
                {
                    return i;
                }
            }

            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorCouldNotCalculateLogicalRange, start, end);
        }

        private void ValidateAndSetSizeAndLogicalRange(DescriptorRange logicalRange, int? sizeInBits)
        {
            if (logicalRange == null)
            {
                // Use the Size as basis for logical range; default is max signed range.
                // Size must be specified.
                if (!sizeInBits.HasValue)
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementSizeMustBeSpecified);
                }

                this.NonAdjustedSizeInBits = sizeInBits.Value;

                this.LogicalMinimum = MinSignedValueForSize(this.NonAdjustedSizeInBits);
                this.LogicalMaximum = MaxSignedValueForSize(this.NonAdjustedSizeInBits);
            }
            else
            {
                switch (logicalRange.Kind)
                {
                    case DescriptorRangeKind.Decimal:
                    {
                        // Not permissible for both Size and explicit LogicalRange to be specified.
                        if (sizeInBits.HasValue)
                        {
                            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementCannotSpecifySize);
                        }

                        this.LogicalMinimum = logicalRange.Minimum.Value;
                        this.LogicalMaximum = logicalRange.Maximum.Value;

                        this.NonAdjustedSizeInBits = RequiredBitsForLogicalRange(this.LogicalMinimum, this.LogicalMaximum);

                        break;
                    }

                    case DescriptorRangeKind.MaxSignedSizeRange:
                    {
                        // Size must be specified.
                        if (!sizeInBits.HasValue)
                        {
                            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementSizeMustBeSpecifiedWhenMaxSigned);
                        }

                        this.NonAdjustedSizeInBits = sizeInBits.Value;

                        this.LogicalMinimum = MinSignedValueForSize(this.NonAdjustedSizeInBits);
                        this.LogicalMaximum = MaxSignedValueForSize(this.NonAdjustedSizeInBits);

                        break;
                    }

                    case DescriptorRangeKind.MaxUnsignedSizeRange:
                    {
                        // Size must be specified.
                        if (!sizeInBits.HasValue)
                        {
                            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorElementSizeMustBeSpecifiedWhenMaxUnsigned);
                        }

                        this.NonAdjustedSizeInBits = sizeInBits.Value;

                        this.LogicalMinimum = 0;
                        this.LogicalMaximum = MaxUnsignedValueForSize(this.NonAdjustedSizeInBits);

                        break;
                    }

                    default:
                    {
                        System.Environment.FailFast($"Unknown ReportRangeKind {logicalRange.Kind}");
                        break;
                    }
                }
            }
        }

        private void ValidateAndSetPhysicalRange(DescriptorRange physicalRange)
        {
            // Guaranteed when gotten to here, LogicalMinimum/LogicalMaximum have been initialized.

            if (physicalRange == null)
            {
                this.PhysicalMinimum = HidConstants.PhysicalMinimumUndefinedValue;
                this.PhysicalMaximum = HidConstants.PhysicalMaximumUndefinedValue;
            }
            else
            {
                if (physicalRange.Kind != DescriptorRangeKind.Decimal)
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorPhysicalRangeMustBeDecimal);
                }

                this.PhysicalMinimum = physicalRange.Minimum.Value;
                this.PhysicalMaximum = physicalRange.Maximum.Value;
            }
        }
    }
}
