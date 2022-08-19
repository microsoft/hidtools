// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Mandatory attribute for all Global items specializations of <see cref="ShortItem"/>.
    /// This allows association of a class with a specific Global item tag.
    /// </summary>
    public class GlobalItemAttribute : BaseHidItemAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalItemAttribute"/> class.
        /// </summary>
        /// <param name="kind">Kind of global item.</param>
        public GlobalItemAttribute(HidConstants.GlobalItemKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of Global item.
        /// </summary>
        public HidConstants.GlobalItemKind Kind { get; }

        /// <inheritdoc/>
        public override byte[] GeneratePrefix(int size)
        {
            return GeneratePrefixInternal(HidConstants.ShortItemTypeKind.Global, (int)this.Kind, size);
        }
    }
}
