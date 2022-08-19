// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Mandatory attribute for all Main items specializations of <see cref="ShortItem"/>.
    /// This allows association of a class with a specific Main item tag.
    /// </summary>
    public class MainItemAttribute : BaseHidItemAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainItemAttribute"/> class.
        /// </summary>
        /// <param name="kind">Kind of Main item.</param>
        public MainItemAttribute(HidConstants.MainItemKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of Main item.
        /// </summary>
        public HidConstants.MainItemKind Kind { get; }

        /// <inheritdoc/>
        public override byte[] GeneratePrefix(int size)
        {
            return GeneratePrefixInternal(HidConstants.ShortItemTypeKind.Main, (int)this.Kind, size);
        }
    }
}
