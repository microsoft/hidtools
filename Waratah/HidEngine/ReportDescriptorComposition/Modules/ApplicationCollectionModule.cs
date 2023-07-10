// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes an abstract HID 'ApplicationCollection'
    /// ApplicationCollections in HID describe a Top-Level-Device (TLD) that corresponds to a 'real' device in Windows.
    /// A single 'real' device can have multiple ApplicationCollections, which corresponds to a Windows composite device.
    /// All ApplicationCollections MUST have an associated Usage (as allowable in the HID usage tables), and one or more Reports.
    /// </summary>
    public class ApplicationCollectionModule : BaseModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollectionModule"/> class.
        /// Main 'construction' occurs through <see cref="Initialize(HidUsage, List{ReportModule})"/>, this constructor only exists so
        /// a placeholder can exist for child items to reference their (this) parent.
        /// </summary>
        public ApplicationCollectionModule()
            : base(null)
        {
            // ApplicationCollection is at the top of the stack, so no parent.
        }

        /// <summary>
        /// Gets usage associated with this ApplicationCollection.
        /// Guaranteed to never to null.
        /// </summary>
        public HidUsageId Usage { get; private set; }

        /// <summary>
        /// Gets reports associated with this ApplicationCollection.
        /// Guaranteed to contain at least 1 report.
        /// </summary>
        public List<ReportModule> Reports { get; private set; }

        /// <inheritdoc/>
        public override int TotalSizeInBits
        {
            get
            {
                return this.Reports.Select(x => x.TotalSizeInBits).Sum();
            }
        }

        /// <inheritdoc/>
        public override int TotalNonAdjustedSizeInBits
        {
            get
            {
                return this.Reports.Select(x => x.TotalNonAdjustedSizeInBits).Sum();
            }
        }

        /// <summary>
        /// Associates this ApplicationCollection with it's Usage and Reports.
        /// </summary>
        /// <param name="usage">Must be a valid Usage.</param>
        /// <param name="reports">Non-empty list of <see cref="ReportModule"/>s.</param>
        public void Initialize(HidUsageId usage, List<ReportModule> reports)
        {
            if (reports == null || reports.Count == 0)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorApplicationCollectionModuleNoReports);
            }

            this.Reports = reports;

            this.Usage = usage ?? throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorModuleNoUsage);

            // TODO: Ensure all Reports have unique ids for each Kind of report.
        }

        /// <summary>
        /// Generates a list of HID Report Items that represent this ApplicationCollection and it's child Reports.
        /// An <see cref="ApplicationCollectionModule"/> will always generate items in the following order:-.
        /// <br/>
        /// <code>
        /// UsagePage(...)<br/>
        /// UsageId(...)<br/>
        /// Collection(Application)<br/>
        ///     Report1Items()...<br/>
        ///     Report2Items()...<br/>
        ///     ...<br/>
        ///     ReportNItems()...<br/>
        /// EndCollection()<br/>
        /// </code>
        /// </summary>
        /// <param name="optimize">Flag of whether to optimize.</param>
        /// <returns>List of HID Report Items describing this ApplicationCollection.</returns>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            descriptorItems.Add(new UsagePageItem(this.Usage.Page.Id));

            descriptorItems.Add(new UsageItem(this.Usage.Page.Id, this.Usage.Id, false));

            descriptorItems.Add(new CollectionItem(HidConstants.MainItemCollectionKind.Application));

            // All Reports with the same UsageRelation, are to be grouped within (the same) PhysicalCollection.
            // This is to provide Sensors with the required mechanism to have multiple reports within the same PhysicalCollection.
            // This pattern (used by Sensors) is deprecated, but still necessary to support.
            // In the non-Sensors case, all Reports will have a null UsageRelation.
            // Note: For simplicity, the PhysicalCollection is added 'here' (during item creation), rather than a higher layer adding it's own CollectionModule.

            List<HidUsageId> usageRelations = this.Reports.Select(x => x.Relation).ToList();

            foreach (HidUsageId usageRelation in usageRelations.Distinct())
            {
                if (usageRelation != null)
                {
                    descriptorItems.Add(new UsagePageItem(usageRelation.Page.Id));

                    descriptorItems.Add(new UsageItem(usageRelation.Page.Id, usageRelation.Id, false));

                    descriptorItems.Add(new CollectionItem(HidConstants.MainItemCollectionKind.Physical));
                }

                foreach (ReportModule report in this.Reports)
                {
                    if (report.Relation == usageRelation)
                    {
                        descriptorItems.AddRange(report.GenerateDescriptorItems(optimize));
                    }
                }

                if (usageRelation != null)
                {
                    descriptorItems.Add(new EndCollectionItem());
                }
            }

            descriptorItems.Add(new EndCollectionItem());

            return descriptorItems;
        }

        /// <inheritdoc/>
        public override List<BaseElementModule> GetReportElements()
        {
            List<BaseElementModule> elements = new List<BaseElementModule>();

            foreach (BaseModule child in this.Reports)
            {
                elements.AddRange(child.GetReportElements());
            }

            return elements;
        }
    }
}
