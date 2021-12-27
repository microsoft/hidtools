// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using System;
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Base class all Report Tags MUST inherit from.
    /// </summary>
    public abstract class BaseReportTag : BaseTag
    {
        private static readonly int ReportIdBase = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseReportTag"/> class.
        /// </summary>
        /// <param name="id">Report Id.</param>
        /// <param name="items">Items belonging to this report.</param>
        /// <param name="rawTag">Root TOML element describing a Report.</param>
        protected BaseReportTag(IdTag id, List<IModuleGeneratorTag> items, KeyValuePair<string, object> rawTag)
            : base(rawTag)
        {
            this.Id = id;
            this.Items = items;
        }

        /// <summary>
        /// Gets Id of the report.
        /// Guaranteed to be unique for amongst all reports of a given type.
        /// </summary>
        public IdTag Id { get; }

        /// <summary>
        /// Gets the items belonging to this report.
        /// A report can contain collections (e.g. LogicalCollection) and individual items (e.g. PaddingItem, Usage).
        /// </summary>
        public IReadOnlyList<IModuleGeneratorTag> Items { get; }

        /// <summary>
        /// Attempts to parse the Report Tag into an Id and Items.
        /// </summary>
        /// <param name="rawTag">Root TOML element describing a Report.</param>
        /// <returns>Id and Items as a tuple.  Can be null.</returns>
        /// <exception cref="TomlInvalidLocationException">Thrown when an unexpected tag is encountered.</exception>
        protected static (IdTag id, List<IModuleGeneratorTag> items) TryParseInternal(KeyValuePair<string, object> rawTag)
        {
            IdTag reportId = null;
            List<IModuleGeneratorTag> reportItems = new List<IModuleGeneratorTag>();

            Dictionary<string, object> reportChildren = ((Dictionary<string, object>[])rawTag.Value)[0];
            foreach (KeyValuePair<string, object> child in reportChildren)
            {
                if (reportId == null)
                {
                    reportId = IdTag.TryParse(child);

                    if (reportId != null)
                    {
                        continue;
                    }
                }

                LogicalCollectionTag logicalCollection = null;
                if ((logicalCollection = LogicalCollectionTag.TryParse(child)) != null)
                {
                    reportItems.Add(logicalCollection);
                    continue;
                }

                PhysicalCollectionTag physicalCollection = null;
                if ((physicalCollection = PhysicalCollectionTag.TryParse(child)) != null)
                {
                    reportItems.Add(physicalCollection);
                    continue;
                }

                PaddingItemTag padding = null;
                if ((padding = PaddingItemTag.TryParse(child)) != null)
                {
                    reportItems.Add(padding);
                    continue;
                }

                ArrayItemTag arrayItem = null;
                if ((arrayItem = ArrayItemTag.TryParse(child)) != null)
                {
                    reportItems.Add(arrayItem);
                    continue;
                }

                VariableItemTag variableItem = null;
                if ((variableItem = VariableItemTag.TryParse(child)) != null)
                {
                    reportItems.Add(variableItem);
                    continue;
                }

                throw new TomlInvalidLocationException(child, rawTag);
            }

            return (reportId, reportItems);
        }

        /// <summary>
        /// Generates a descriptor module, describing this Report.
        /// </summary>
        /// <param name="parent">Logical parent for the newly generated descriptor module.</param>
        /// <param name="kind">Type of report.</param>
        /// <param name="reportIdsForKind">Known report ids for this report kind.</param>
        /// <returns>Module describing this Tag.</returns>
        /// <exception cref="TomlGenericException">Thrown when unable to parse into descriptor modules.</exception>
        protected BaseModule GenerateDescriptorModuleInternal(BaseModule parent, ReportKind kind, List<int> reportIdsForKind)
        {
            this.AddReportIdIfUniqueOrThrow(reportIdsForKind);

            ReportModule report = new ReportModule(kind, parent);

            List<BaseModule> reportModules = new List<BaseModule>();
            foreach (IModuleGeneratorTag tag in this.Items)
            {
                BaseModule module = tag.GenerateDescriptorModule(report);
                if (module != null)
                {
                    reportModules.Add(module);
                }
            }

            try
            {
                // TODO: Perhaps valid reportId check (and no duplicate reportId) should be done in the ReportDescriptorComposer layer. not here???
                report.Initialize(this.FindOrGenerateReportId(reportIdsForKind), reportModules);
            }
            catch (DescriptorModuleParsingException parsingException)
            {
                // If exception thrown, catch it, and then give the line number (tacked on the end).
                throw new TomlGenericException(parsingException, this);
            }

            return report;
        }

        private void AddReportIdIfUniqueOrThrow(List<int> reportIds)
        {
            if (this.Id != null)
            {
                if (reportIds.Contains(this.Id.Value))
                {
                    throw new TomlGenericException(HidEngine.Properties.Resources.ExceptionTomlReportItemDuplicateReportId, this.RawTag, this.Id.Value);
                }

                reportIds.Add(this.Id.Value);
            }
        }

        private int FindOrGenerateReportId(List<int> knownReportIdsForReportKind)
        {
            if (this.Id == null)
            {
                int nextReportId = ReportIdBase;

                while (true)
                {
                    if (knownReportIdsForReportKind.Contains(nextReportId))
                    {
                        nextReportId++;
                    }
                    else
                    {
                        break;
                    }
                }

                knownReportIdsForReportKind.Add(nextReportId);

                knownReportIdsForReportKind.Sort();

                return nextReportId;
            }
            else
            {
                return this.Id.Value;
            }
        }
    }
}
