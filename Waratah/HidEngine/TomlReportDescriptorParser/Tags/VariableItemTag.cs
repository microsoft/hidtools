// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Represents an VariableItem TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "variableItem", typeof(Dictionary<string, object>[]))]
    [SupportedHidKinds(HidUsageKind.BufferedBytes, HidUsageKind.DF, HidUsageKind.DV, HidUsageKind.LC, HidUsageKind.MC, HidUsageKind.NAry, HidUsageKind.OOC, HidUsageKind.OSC, HidUsageKind.RTC, HidUsageKind.Sel, HidUsageKind.SF, HidUsageKind.SV, HidUsageKind.UM)]
    public class VariableItemTag : BaseTag, IModuleGeneratorTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableItemTag"/> class.
        /// Can only be instantiated via <see cref="TryParse(KeyValuePair{string, object})"/>.
        /// </summary>
        private VariableItemTag(
            bool isRange,
            UsageTag usage,
            UsageRangeTag usageRange,
            UsageTransformTag usageTransform,
            BaseValueRangeBaseTag logicalValueRange,
            BaseValueRangeBaseTag physicalValueRange,
            SizeInBitsTag size,
            CountTag count,
            ReportFlagsTag flags,
            UnitTag unit,
            UsageUnitMultiplierTag usageUnitMultipler,
            NameTag name,
            KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.IsRange = isRange;
            this.Usage = usage;
            this.UsageRange = usageRange;
            this.UsageTransform = usageTransform;
            this.LogicalValueRange = logicalValueRange;
            this.PhysicalValueRange = physicalValueRange;
            this.Size = size;
            this.Count = count;
            this.ReportFlags = flags;
            this.Unit = unit;
            this.UsageUnitMultiplier = usageUnitMultipler;
            this.Name = name;
        }

        /// <summary>
        /// Gets a value indicating whether this is a range VariableItem.
        /// </summary>
        public bool IsRange { get; }

        /// <summary>
        /// Gets the associated Usage for the VariableItem.
        /// Will be null when <see cref="IsRange"/> is true.
        /// </summary>
        public UsageTag Usage { get; }

        /// <summary>
        /// Gets the associated UsageRange for the VariableItem.
        /// Will be null when <see cref="IsRange"/> is false.
        /// </summary>
        public UsageRangeTag UsageRange { get; }

        /// <summary>
        /// Gets the associated UsageTransform for this VariableItem.
        /// </summary>
        public UsageTransformTag UsageTransform { get; }

        /// <summary>
        /// Gets the associated LogicalValueRange for the VariableItem.
        /// </summary>
        public BaseValueRangeBaseTag LogicalValueRange { get; }

        /// <summary>
        /// Gets the associated PhysicalValueRange for the VariableItem.
        /// </summary>
        public BaseValueRangeBaseTag PhysicalValueRange { get; }

        /// <summary>
        /// Gets the size (in bits) for the VariableItem.
        /// </summary>
        public SizeInBitsTag Size { get; }

        /// <summary>
        /// Gets the Count for the VariableItem.
        /// </summary>
        public CountTag Count { get; }

        /// <summary>
        /// Gets the ReportFlags for the VariableItem.
        /// </summary>
        public ReportFlagsTag ReportFlags { get; }

        /// <summary>
        /// Gets the Unit for the VariableItem.
        /// </summary>
        public UnitTag Unit { get; }

        /// <summary>
        /// Gets the UsageUnitMultiplier for the VariableItem.
        /// </summary>
        public UsageUnitMultiplierTag UsageUnitMultiplier { get; }

        /// <summary>
        /// Gets the (optional) Name of the VariableItem.
        /// </summary>
        public NameTag Name { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="VariableItemTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing an VariableItem.</param>
        /// <returns>New <see cref="VariableItemTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values in tag are out of bounds.</exception>
        public static VariableItemTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(VariableItemTag), rawTag))
            {
                return null;
            }

            UsageTag usage = null;
            UsageRangeTag usageRange = null;
            UsageTransformTag usageTransform = null;
            BaseValueRangeBaseTag logicalValueRange = null;
            BaseValueRangeBaseTag physicalValueRange = null;
            SizeInBitsTag size = null;
            CountTag count = null;
            ReportFlagsTag reportFlags = null;
            UnitTag unit = null;
            UsageUnitMultiplierTag usageUnitMultiplier = null;
            NameTag name = null;

            Dictionary<string, object> children = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in children)
            {
                if (usage == null)
                {
                    usage = UsageTag.TryParse(child, typeof(VariableItemTag));

                    if (usage != null)
                    {
                        continue;
                    }
                }

                if (usageRange == null)
                {
                    usageRange = UsageRangeTag.TryParse(child, typeof(VariableItemTag));

                    if (usageRange != null)
                    {
                        continue;
                    }
                }

                if (usageTransform == null)
                {
                    usageTransform = UsageTransformTag.TryParse(child, typeof(VariableItemTag));
                    if (usageTransform != null)
                    {
                        continue;
                    }
                }

                if (logicalValueRange == null)
                {
                    logicalValueRange = LogicalValueRangeTag.TryParse(child);

                    if (logicalValueRange != null)
                    {
                        continue;
                    }
                }

                if (physicalValueRange == null)
                {
                    physicalValueRange = PhysicalValueRangeTag.TryParse(child);

                    if (physicalValueRange != null)
                    {
                        continue;
                    }
                }

                if (size == null)
                {
                    size = SizeInBitsTag.TryParse(child);

                    if (size != null)
                    {
                        continue;
                    }
                }

                if (count == null)
                {
                    count = CountTag.TryParse(child);

                    if (count != null)
                    {
                        continue;
                    }
                }

                if (reportFlags == null)
                {
                    reportFlags = ReportFlagsTag.TryParse(child);

                    if (reportFlags != null)
                    {
                        continue;
                    }
                }

                if (unit == null)
                {
                    unit = UnitTag.TryParse(child);

                    if (unit != null)
                    {
                        continue;
                    }
                }

                if (usageUnitMultiplier == null)
                {
                    usageUnitMultiplier = UsageUnitMultiplierTag.TryParse(child);

                    if (usageUnitMultiplier != null)
                    {
                        continue;
                    }
                }

                if (name == null)
                {
                    name = NameTag.TryParse(child);

                    if (name != null)
                    {
                        continue;
                    }
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (usage != null && usageTransform != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, usage.NonDecoratedName, usageTransform.NonDecoratedName);
            }

            bool hasUsage = (usage != null || usageTransform != null);

            bool isRange = false;
            if (hasUsage && usageRange == null)
            {
                isRange = false;
            }
            else if (!hasUsage && usageRange != null)
            {
                isRange = true;
            }
            else if (usage != null && usageRange != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, usage.NonDecoratedName, usageRange.NonDecoratedName);
            }
            else if (usageTransform != null && usageRange != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, usageTransform.NonDecoratedName, usageRange.NonDecoratedName);
            }
            else
            {
                throw new TomlGenericException(
                    Resources.ExceptionTomlMustSpecifyOneOfThreeKeys,
                    rawTag,
                    typeof(UsageTag).GetCustomAttribute<TagAttribute>().Name,
                    typeof(UsageRangeTag).GetCustomAttribute<TagAttribute>().Name,
                    typeof(UsageTransformTag).GetCustomAttribute<TagAttribute>().Name);
            }

            if (isRange && count != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, count.NonDecoratedName, usageRange.NonDecoratedName);
            }

            if (unit != null && usageUnitMultiplier != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, unit.NonDecoratedName, usageUnitMultiplier.NonDecoratedName);
            }

            return new VariableItemTag(isRange, usage, usageRange, usageTransform, logicalValueRange, physicalValueRange, size, count, reportFlags, unit, usageUnitMultiplier, name, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            try
            {
                if (this.IsRange)
                {
                    VariableRangeModule module = new VariableRangeModule(
                        this.UsageRange.UsageStart,
                        this.UsageRange.UsageEnd,
                        this.LogicalValueRange?.Value,
                        this.PhysicalValueRange?.Value,
                        this.Size?.Value,
                        this.ReportFlags?.Value,
                        this.Unit?.Value,
                        this.UsageUnitMultiplier?.Value,
                        this.Name?.Value,
                        parent);

                    return module;
                }
                else
                {
                    HidUsageId usage = (this.Usage != null) ? (this.Usage.Value) : (this.UsageTransform.Value);
                    VariableModule module = new VariableModule(
                        usage,
                        this.Count?.Value,
                        this.LogicalValueRange?.Value,
                        this.PhysicalValueRange?.Value,
                        this.Size?.Value,
                        this.ReportFlags?.Value,
                        this.Unit?.Value,
                        this.UsageUnitMultiplier?.Value,
                        this.Name?.Value,
                        parent);

                    return module;
                }
            }
            catch (DescriptorModuleParsingException parsingException)
            {
                // If exception thrown, catch it, and then give the line number (tacked on the end).
                throw new TomlGenericException(parsingException, this);
            }
        }
    }
}