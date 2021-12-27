// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using HidSpecification;

    /// <summary>
    /// Instruction to place a copy of the global itme state table on the stack.
    /// </summary>
    [GlobalItem(HidConstants.GlobalItemKind.Push)]
    public class PushItem : ShortItem
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return "Push()";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            // Creates an array of zero size.
            return new byte[] { };
        }
    }
}
