// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using HidEngine.Properties;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidSpecification;

    /// <summary>
    /// Represents an Logical Collection TOML tag.
    /// </summary>
    [TagAttribute(TagKind.Section, "logicalCollection", typeof(Dictionary<string, object>[]))]
    [SupportedHidKinds(HidUsageKind.CL, HidUsageKind.NAry, HidUsageKind.UM, HidUsageKind.US)]
    public class LogicalCollectionTag : BaseTag, IModuleGeneratorTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalCollectionTag"/> class.
        /// </summary>
        /// <param name="usage">UsageTag associated with this ApplicationCollection.</param>
        /// <param name="tags">ItemTags and (sub) CollectionTags in this collection.</param>
        /// <param name="rawTag">Root TOML element describing a non-ApplicationCollection collection.</param>
        private LogicalCollectionTag(UsageTag usage, UsageTransformTag usageTransform, List<IModuleGeneratorTag> tags, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Usage = usage;
            this.UsageTransform = usageTransform;
            this.Tags = tags;
        }

        /// <summary>
        /// Gets the associated Usage for this Collection.
        /// </summary>
        public UsageTag Usage { get; }

        /// <summary>
        /// Gets the associated UsageTransform for this Collection.
        /// </summary>
        public UsageTransformTag UsageTransform { get; }

        /// <summary>
        /// Gets the associated <see cref="IModuleGeneratorTag"/>s and (sub) <see cref="CollectionTag"/>s for this collection.
        /// <para>Elements must be cast to the underlying type before they can be inspected.</para>
        /// <para>Order of objects in List is important.</para>
        /// </summary>
        public IReadOnlyList<IModuleGeneratorTag> Tags { get; }

        /// <summary>
        /// Attempts to parse the given tag as an <see cref="CollectionTag"/>.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a non-ApplicationCollection collection.</param>
        /// <returns>New <see cref="CollectionTag"/> or null if it cannot be parsed as one.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        /// <exception cref="TomlGenericException">Thrown when supplied values are invalid.</exception>
        public static LogicalCollectionTag TryParse(KeyValuePair<string, object> rawTag)
        {
            if (!IsValidNameAndType(typeof(LogicalCollectionTag), rawTag))
            {
                return null;
            }

            UsageTag usage = null;
            UsageTransformTag usageTransform = null;
            List<IModuleGeneratorTag> tags = new List<IModuleGeneratorTag>();

            Dictionary<string, object> collectionChildren = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in collectionChildren)
            {
                // Guaranteed by TOML parser that duplicate keys cannot occur.
                // Only bother attempting to parse a Usage if one hasn't been found already.
                if (usage == null)
                {
                    usage = UsageTag.TryParse(child, typeof(LogicalCollectionTag));
                    if (usage != null)
                    {
                        continue;
                    }
                }

                if (usageTransform == null)
                {
                    usageTransform = UsageTransformTag.TryParse(child, typeof(LogicalCollectionTag));
                    if (usageTransform != null)
                    {
                        continue;
                    }
                }

                LogicalCollectionTag logicalCollection = null;
                if ((logicalCollection = LogicalCollectionTag.TryParse(child)) != null)
                {
                    tags.Add(logicalCollection);
                    continue;
                }

                PhysicalCollectionTag physicalCollection = null;
                if ((physicalCollection = PhysicalCollectionTag.TryParse(child)) != null)
                {
                    tags.Add(physicalCollection);
                    continue;
                }

                PaddingItemTag padding = null;
                if ((padding = PaddingItemTag.TryParse(child)) != null)
                {
                    tags.Add(padding);
                    continue;
                }

                ArrayItemTag arrayItem = null;
                if ((arrayItem = ArrayItemTag.TryParse(child)) != null)
                {
                    tags.Add(arrayItem);
                    continue;
                }

                VariableItemTag variableItem = null;
                if ((variableItem = VariableItemTag.TryParse(child)) != null)
                {
                    tags.Add(variableItem);
                    continue;
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            if (usage != null && usageTransform != null)
            {
                throw new TomlGenericException(Resources.ExceptionTomlCannotSpecifyBothKeys, rawTag, usage.NonDecoratedName, usageTransform.NonDecoratedName);
            }

            if (usage == null && usageTransform == null)
            {
                throw new TomlGenericException(
                    Resources.ExceptionTomlMustSpecifyOneOfTwoKeys,
                    rawTag,
                    typeof(UsageTag).GetCustomAttribute<TagAttribute>().Name,
                    typeof(UsageTransformTag).GetCustomAttribute<TagAttribute>().Name);
            }

            return new LogicalCollectionTag(usage, usageTransform, tags, rawTag);
        }

        /// <inheritdoc/>
        public BaseModule GenerateDescriptorModule(BaseModule parent)
        {
            try
            {
                CollectionModule collection = new CollectionModule(HidConstants.MainItemCollectionKind.Logical, parent);

                List<BaseModule> collectionItems = new List<BaseModule>();
                foreach (IModuleGeneratorTag tag in this.Tags)
                {
                    BaseModule module = tag.GenerateDescriptorModule(parent);
                    if (module != null)
                    {
                        collectionItems.Add(module);
                    }
                }

                HidUsageId usage = (this.Usage != null) ? (this.Usage.Value) : (this.UsageTransform.Value);
                collection.Initialize(usage, collectionItems);

                return collection;
            }
            catch (DescriptorModuleParsingException parsingException)
            {
                // If exception thrown, catch it, and then give the line number (tacked on the end).
                throw new TomlGenericException(parsingException, this);
            }
        }
    }
}
