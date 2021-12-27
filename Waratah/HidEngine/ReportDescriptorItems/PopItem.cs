// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using HidSpecification;

    /// <summary>
    /// Instruction to replace the item state table with the top structure from the stack.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.Pop)]
    public class PopItem : ShortItem
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return "Pop()";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            // Creates an array of zero size.
            return new byte[] { };
        }
    }
}
