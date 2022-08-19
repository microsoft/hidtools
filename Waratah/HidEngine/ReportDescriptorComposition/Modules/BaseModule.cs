// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;

    /// <summary>
    /// Abstract base class for all descriptor modules.
    /// Modules are the high-level logical blocks of a descriptor (i.e. Reports, Collections, VariableItems...).
    /// </summary>
    public abstract class BaseModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModule"/> class.
        /// </summary>
        /// <param name="parent">The parent module of this module.</param>
        public BaseModule(BaseModule parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the size (in bits) of this module (including all children).
        /// </summary>
        public abstract int TotalSizeInBits
        {
            get;
        }

        /// <summary>
        /// Gets the size (in bits) of this module (including all children) if size wasn't adjusted by settings (i.e. for padding).
        /// </summary>
        public abstract int TotalNonAdjustedSizeInBits
        {
            get;
        }

        /// <summary>
        /// Gets the parent module of this <see cref="BaseModule"/>.
        /// If no parent module (e.g. <see cref="ApplicationCollectionModule"/>) then this will be null.
        /// </summary>
        public BaseModule Parent { get; }

        /// <summary>
        /// Gets the leaf modules that are children of this module.
        /// </summary>
        /// <returns>All leaf/tail modules (i.e. not collections, reports...)</returns>
        public abstract List<BaseElementModule> GetReportElements();

        /// <summary>
        /// Generates all Descriptor Items belonging to this and any child items.
        /// </summary>
        /// <param name="optimize">Whether to optimize the generated items, to remove duplications etc...</param>
        /// <returns>List of Items.</returns>
        public abstract List<ShortItem> GenerateDescriptorItems(bool optimize);
    }
}
