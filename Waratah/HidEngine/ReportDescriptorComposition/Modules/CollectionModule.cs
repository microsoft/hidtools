// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes any abstract HID non-Application collection (e.g. Logical/Physical).
    /// ApplicationCollections have their own specialization <see cref="ApplicationCollectionModule"/>.
    /// <p/>
    /// <see cref="CollectionModule"/>s consist of a single Usage, a Type and 1 or more child items (e.g. <see cref="CollectionModule"/>, <see cref="VariableModule"/>, <see cref="PaddingModule"/>).
    /// </summary>
    public class CollectionModule : BaseModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionModule"/> class.
        /// Main 'initialization' occurs through <see cref="Initialize(HidUsageId, List{BaseModule})"/>.
        /// <p/>
        /// This constructor only exists so a placeholder can exist for child modules to reference their (this) parent.
        /// </summary>
        /// <param name="kind">Kind of collection.</param>
        /// <param name="parent">The parent module of this module.  There must be a <see cref="ReportModule"/> somewhere in the ancestry.</param>
        public CollectionModule(HidConstants.MainItemCollectionKind kind, BaseModule parent)
            : base(parent)
        {
            if (kind != HidConstants.MainItemCollectionKind.Logical && kind != HidConstants.MainItemCollectionKind.Physical)
            {
                throw new DescriptorModuleParsingException(
                    Resources.ExceptionDescriptorCollectionModuleInvalidSupport,
                    Enum.GetName(typeof(HidConstants.MainItemCollectionKind), kind),
                    Enum.GetName(typeof(HidConstants.MainItemCollectionKind), HidConstants.MainItemCollectionKind.Logical),
                    Enum.GetName(typeof(HidConstants.MainItemCollectionKind), HidConstants.MainItemCollectionKind.Physical));
            }

            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of collection this is.
        /// </summary>
        public HidConstants.MainItemCollectionKind Kind { get; }

        /// <summary>
        /// Gets the associated Usage.
        /// </summary>
        public HidUsageId Usage { get; private set; }

        /// <summary>
        /// Gets the modules that are part of this collection.
        /// </summary>
        public List<BaseModule> Children { get; private set; }

        /// <inheritdoc/>
        public override int TotalSizeInBits
        {
            get
            {
                return this.Children.Select(x => x.TotalSizeInBits).Sum();
            }
        }

        /// <inheritdoc/>
        public override int TotalNonAdjustedSizeInBits
        {
            get
            {
                return this.Children.Select(x => x.TotalNonAdjustedSizeInBits).Sum();
            }
        }

        /// <summary>
        /// Associates this <see cref="CollectionModule"/> with it's Usage and children.
        /// </summary>
        /// <param name="usage">Must be a valid Usage.</param>
        /// <param name="children">Non-empty list of <see cref="BaseModule"/>s.</param>
        public virtual void Initialize(HidUsageId usage, List<BaseModule> children)
        {
            if (children?.Count == 0)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorCollectionModuleNoItems);
            }

            this.Children = children;

            this.Usage = usage ?? throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorModuleNoUsage);

            this.ValidateDoesNotContainOnlyPaddingModules();
        }

        /// <summary>
        /// Generates a list of HID Report Items that represent this Collection and it's child items.
        /// A <see cref="CollectionModule"/> will always generate items in the following order:-.<br/>
        /// <code>
        /// UsagePage(...)<br/>
        /// UsageId(...)<br/>
        /// Collection(...)<br/>
        ///     Item1()...<br/>
        ///     Item2()...<br/>
        ///     ...<br/>
        ///     ItemN()...<br/>
        /// EndCollection()<br/>
        /// </code>
        /// </summary>
        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new UsagePageItem(this.Usage.Page.Id));

            descriptorItems.Add(new UsageItem(this.Usage.Page.Id, this.Usage.Id, false));

            descriptorItems.Add(new CollectionItem(this.Kind));

            descriptorItems.AddRange(this.GenerateChildDescriptorItems(optimize));

            descriptorItems.Add(new EndCollectionItem());

            return descriptorItems;
        }

        /// <inheritdoc/>
        public override List<BaseElementModule> GetReportElements()
        {
            List<BaseElementModule> leafModules = new List<BaseElementModule>();

            foreach (BaseModule child in this.Children)
            {
                leafModules.AddRange(child.GetReportElements());
            }

            return leafModules;
        }

        private List<ShortItem> GenerateChildDescriptorItems(bool optimize)
        {
            List<ShortItem> childDescriptorItems = new List<ShortItem>();

            if (optimize)
            {
                // Proactively perform an optimization here to descriptor-combine VariableModules at the same level in this Collection.

                List<VariableModule> combinableVariableModules = new List<VariableModule>();
                foreach (BaseModule childModule in this.Children)
                {
                    if (childModule is VariableModule castVariableModule)
                    {
                        if (castVariableModule.IsDescriptorCombinableWith(combinableVariableModules))
                        {
                            combinableVariableModules.Add(castVariableModule);
                        }
                        else
                        {
                            // This VariableModule can't be descriptor-combined.
                            // Generate items for all known descriptor-combinable VariableModules, and then restart the process again.
                            childDescriptorItems.AddRange(VariableModule.GenerateDescriptorItemsForCombinable(combinableVariableModules));
                            combinableVariableModules.Clear();

                            combinableVariableModules.Add(castVariableModule);
                        }
                    }
                    else
                    {
                        // This module can't be descriptor-combined.
                        // Generate items for all known descriptor-combinable VariableModules, and then restart the process again.
                        childDescriptorItems.AddRange(VariableModule.GenerateDescriptorItemsForCombinable(combinableVariableModules));
                        combinableVariableModules.Clear();

                        childDescriptorItems.AddRange(childModule.GenerateDescriptorItems(true));
                    }
                }

                // Add anything remaining in the buffer.
                childDescriptorItems.AddRange(VariableModule.GenerateDescriptorItemsForCombinable(combinableVariableModules));
            }
            else
            {
                foreach (BaseModule item in this.Children)
                {
                    childDescriptorItems.AddRange(item.GenerateDescriptorItems(false));
                }
            }

            return childDescriptorItems;
        }

        private void ValidateDoesNotContainOnlyPaddingModules()
        {
            foreach (BaseElementModule module in this.GetReportElements())
            {
                if (!(module is PaddingModule))
                {
                    return;
                }
            }

            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorCollectionOnlyPaddingItemsEncountered);
        }
    }
}
