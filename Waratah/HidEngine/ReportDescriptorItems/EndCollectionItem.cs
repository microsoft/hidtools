// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorItems
{
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Identifies the end of a Collection.
    /// </summary>
    [MainItem(HidConstants.MainItemKind.EndCollection)]
    public class EndCollectionItem : ShortItem
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return "EndCollection()";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            // Creates an array of zero size.
            // EndCollection has no data bytes.
            return new byte[] { };
        }
    }
}
