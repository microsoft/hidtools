// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Mandatory attribute for all Local items specializations of <see cref="ShortItem"/>.
    /// This allows association of a class with a specific Local item tag.
    /// </summary>
    public class LocalItemAttribute : BaseHidItemAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalItemAttribute"/> class.
        /// </summary>
        /// <param name="kind">Kind of Local item.</param>
        public LocalItemAttribute(HidConstants.LocalItemKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of Local item.
        /// </summary>
        public HidConstants.LocalItemKind Kind { get; }

        /// <inheritdoc/>
        public override byte[] GeneratePrefix(int size)
        {
            return GeneratePrefixInternal(HidConstants.ShortItemTypeKind.Local, (int)this.Kind, size);
        }
    }
}
