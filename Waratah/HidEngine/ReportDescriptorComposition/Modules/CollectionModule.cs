// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorComposition.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorItems;
    using HidSpecification;

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
                // Proactively perform an optimization here to descriptor-combine VariableItems at the same level in this collection.

                List<VariableModule> combinableItemsBuffer = new List<VariableModule>();
                foreach (BaseModule item in this.Children)
                {
                    if (this.IsCanBeDescriptorCombinedWithPrevious(item, combinableItemsBuffer))
                    {
                        combinableItemsBuffer.Add((VariableModule)(item));
                    }
                    else
                    {
                        // This item can't be descriptor-combined.
                        // Add any known descriptor-combin'able items, and then restart the process again.
                        childDescriptorItems.AddRange(this.GenerateDescriptorItemsForCombinableVariableItems(combinableItemsBuffer));
                        combinableItemsBuffer.Clear();

                        if (item.GetType() == typeof(VariableModule))
                        {
                            combinableItemsBuffer.Add((VariableModule)(item));
                        }
                        else
                        {
                            childDescriptorItems.AddRange(item.GenerateDescriptorItems(false));
                        }
                    }
                }

                // Add anything remaining in the buffer.
                childDescriptorItems.AddRange(this.GenerateDescriptorItemsForCombinableVariableItems(combinableItemsBuffer));
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

        private bool IsCanBeDescriptorCombinedWithPrevious(BaseModule item, List<VariableModule> combinableItemsBuffer)
        {
            if (item.GetType() == typeof(VariableModule))
            {
                VariableModule castItem = (VariableModule)(item);

                // Nothing in the buffer to combine with.
                if (combinableItemsBuffer.Count == 0)
                {
                    return castItem.IsCanBeDescriptorCombined();
                }

                // Validate it can be combined will all other items in the buffer.
                // Note: should only really need to validate against 1 of them and the combinable property is transitive.
                bool isCanBeCombined = true;
                foreach (VariableModule combinableItem in combinableItemsBuffer)
                {
                    isCanBeCombined &= combinableItem.IsCanBeDescriptorCombined(castItem);
                }

                return isCanBeCombined;
            }

            return false;
        }

        private List<ShortItem> GenerateDescriptorItemsForCombinableVariableItems(List<VariableModule> combinableItemsBuffer)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            if (combinableItemsBuffer.Count > 0)
            {
                List<HidUsageId> combinableUsages = new List<HidUsageId>();

                foreach (VariableModule item in combinableItemsBuffer)
                {
                    combinableUsages.Add(item.Usage);
                }

                // Always safe to access the first item as we know it's size > 0.
                descriptorItems.AddRange(combinableItemsBuffer[0].GenerateDescriptorItems(combinableUsages, combinableItemsBuffer.Count));
            }

            return descriptorItems;
        }
    }
}
