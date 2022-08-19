// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition
{
    using System;
    using System.Diagnostics;
    using Microsoft.HidTools.HidEngine.Properties;

    /// <summary>
    /// Kind of module range.
    /// </summary>
    public enum DescriptorRangeKind
    {
        /// <summary>
        /// Range is explicitly defined with both Minimum and Maximum.
        /// </summary>
        Decimal,

        /// <summary>
        /// Range is implicitly defined as minimum/maximum signed size for the item.
        /// Only meaningful if a size is defined somewhere else.
        /// </summary>
        /// <example>With size of 4, range is [-6, 7] (2^(4-1) - 1)</example>
        MaxSignedSizeRange,

        /// <summary>
        /// Range is implicitly defined as 0 and maximum unsigned size for the item.
        /// Due to external limitations, unsigned 32bit range is not support, so it's range
        /// will be truncated to [0, (2^31)-1], which is the same as 31bit unsigned range.
        /// Only meaningful if a size is defined somewhere else.
        /// </summary>
        /// <example>With size of 4, range is [0, 15] (2^(4) - 1)</example>
        MaxUnsignedSizeRange,
    }

    /// <summary>
    /// Describes a value range, consisting of a Minimum and a Maximum.
    /// </summary>
    public class DescriptorRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorRange"/> class.
        /// Implicitly assigns <see cref="Kind"/> to <see cref="DescriptorRangeKind.Decimal"/>.
        /// </summary>
        /// <param name="minimum">Minimum value of this range.</param>
        /// <param name="maximum">Maximum value of this range.</param>
        public DescriptorRange(int minimum, int maximum)
        {
            if (minimum > maximum)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorInvalidLogicalRange, maximum, minimum);
            }

            this.Minimum = minimum;
            this.Maximum = maximum;

            this.Kind = DescriptorRangeKind.Decimal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorRange"/> class.
        /// </summary>
        /// <param name="kind">Kind of range.  Cannot specify <see cref="DescriptorRangeKind.Decimal"/>.</param>
        public DescriptorRange(DescriptorRangeKind kind)
        {
            if (kind == DescriptorRangeKind.Decimal)
            {
                System.Environment.FailFast("Cannot specify Decimal Kind in constructor");
            }

            this.Minimum = null;
            this.Maximum = null;

            this.Kind = kind;
        }

        /// <summary>
        /// Gets the minimum value of the range.
        /// Will be null when RangeType is <see cref="DescriptorRangeKind.MaxSignedSizeRange"/> or <see cref="DescriptorRangeKind.MaxUnsignedSizeRange"/>.
        /// </summary>
        public Int32? Minimum { get; }

        /// <summary>
        /// Gets the maximum value of the range.
        /// Will be null when RangeType is <see cref="DescriptorRangeKind.MaxSignedSizeRange"/> or <see cref="DescriptorRangeKind.MaxUnsignedSizeRange"/>.
        /// </summary>
        public Int32? Maximum { get; }

        /// <summary>
        /// Gets the kind of range.
        /// </summary>
        public DescriptorRangeKind Kind { get; }
    }
}
