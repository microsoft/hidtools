// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ConsoleTables;
    using Microsoft.HidTools.HidEngine.CppGenerator;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes a HID Report Descriptor.
    /// </summary>
    public class Descriptor
    {
        private const int IndentSize = 4;

        private static readonly string Indent = string.Empty.PadLeft(IndentSize, '.');

        /// <summary>
        /// Initializes a new instance of the <see cref="Descriptor"/> class.
        /// </summary>
        /// <param name="applicationCollections">Application collections comprising this report descriptor.</param>
        public Descriptor(List<ApplicationCollectionModule> applicationCollections)
        {
            if (applicationCollections == null || applicationCollections.Count == 0)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorNoApplicationCollections);
            }

            this.ApplicationCollections = applicationCollections;

            if (Settings.GetInstance().Optimize)
            {
                this.Optimize();
            }
            else
            {
                this.DeOptimize();
            }

            int descriptorSize = this.GenerateDescriptorBytes().Length;
            if (descriptorSize > HidConstants.DescriptorSizeInBytesMaximum)
            {
                throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorTooBig, HidConstants.DescriptorSizeInBytesMaximum);
            }
        }

        /// <summary>
        /// Gets the discrete Report Items for this descriptor.
        /// </summary>
        public IReadOnlyList<ShortItem> LastGeneratedItems { get; private set; }

        /// <summary>
        /// Gets the ApplicationCollections within this ReportDescriptor.
        /// </summary>
        public IReadOnlyList<ApplicationCollectionModule> ApplicationCollections { get; }

        /// <summary>
        /// Removes any optimization performed on the generated descriptor items.
        /// </summary>
        public void DeOptimize()
        {
            this.LastGeneratedItems = this.GenerateDescriptorItems(false);
        }

        /// <summary>
        /// Optimizes the generated descriptor items to reduce total size of the descriptor on the wire.  Size of reports remains same.
        /// </summary>
        public void Optimize()
        {
            this.LastGeneratedItems = this.GenerateDescriptorItems(true);

            this.LastGeneratedItems = OptimizeRemoveDuplicateGlobals(this.LastGeneratedItems);
        }

        /// <summary>
        /// Generates the descriptor as a sequence of encoded bytes as defined by the "Device Class Definition for Human Interface Devices(HID)" (2001).
        /// </summary>
        /// <returns>The descriptor as a sequence of encoded bytes.</returns>
        public byte[] GenerateDescriptorBytes()
        {
            List<byte> descriptorBytes = new List<byte>();
            foreach (ShortItem item in this.LastGeneratedItems)
            {
                descriptorBytes.AddRange(item.WireRepresentation());
            }

            return descriptorBytes.ToArray();
        }

        /// <summary>
        /// Generates text representation of this descriptor, according to global settings.
        /// </summary>
        /// <returns>Text representing the descriptor.</returns>
        public string GenerateText()
        {
            if (Settings.GetInstance().GenerateCpp)
            {
                return this.ToCppHeaderFile();
            }
            else
            {
                return this.ToString(true);
            }
        }

        /// <summary>
        /// Generates a CPP compatible header file including descriptor (in byte format), and supporting Report structs.
        /// </summary>
        /// <returns>Stringified CPP header file of descriptor.</returns>
        public string ToCppHeaderFile()
        {
            CppHeader header = new CppHeader(this);

            return header.GenerateCpp(this.GenerateSummary(true));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// Returns a string that respresents the current object.
        /// </summary>
        /// <param name="isWithBytes">Whether to include the raw-bytes in output.</param>
        /// <returns>A string that represents the current object.</returns>
        public string ToString(bool isWithBytes)
        {
            if (this.LastGeneratedItems == null)
            {
                return string.Empty;
            }

            // Determine the initial indent required to ensure all non-byte text starts at the same offset.
            int maxWireRepresentationStringLength = this.LastGeneratedItems.Max(x => x.WireRepresentationString().Length);

            StringBuilder descriptorText = new StringBuilder();
            int indentLevel = 0;

            foreach (ShortItem item in this.LastGeneratedItems)
            {
                if (item.GetType() == typeof(EndCollectionItem))
                {
                    indentLevel -= 1;
                }

                if (isWithBytes)
                {
                    string paddedItemWireRepresentation = item.WireRepresentationString().PadRight(maxWireRepresentationStringLength + IndentSize, '.');
                    string indentation = this.GenerateIndent(indentLevel, Indent);

                    descriptorText.AppendLine($"{paddedItemWireRepresentation}{indentation}{item}");
                }
                else
                {
                    string indentation = this.GenerateIndent(indentLevel, Indent);

                    descriptorText.AppendLine($"{indentation}{item}");
                }

                if (item.GetType() == typeof(CollectionItem))
                {
                    indentLevel += 1;
                }
            }

            return descriptorText.ToString();
        }

        /// <summary>
        /// Generates a summary of this descriptor than can be printed on a Console to the user.
        /// </summary>
        /// <param name="asCppComment">Whether to generate summary as a CPP comment.</param>
        /// <returns>Summary of this Descriptor.</returns>
        public string GenerateSummary(bool asCppComment = false)
        {
            StringBuilder summary = new StringBuilder();

            summary.AppendLine($"HID Usage Tables: {HidSpecification.HidUsageTableDefinitions.GetInstance().UsageTableVersionReadable}");
            summary.AppendLine($"Descriptor size: {this.GenerateDescriptorBytes().Length} (bytes)");

            IEnumerable<ReportModule> reports = this.ApplicationCollections.SelectMany(x => x.Reports);
            IOrderedEnumerable<ReportModule> orderedReports = reports.ToList().OrderBy(x => x.Id).ThenBy(x => x.Kind);

            // Note: ConsoleTable requires initialization by "ConsoleTable.From" for alignment setting to apply.
            var summaryTableRows = orderedReports.Select(x => new { ReportId = x.Id, Kind = x.Kind.ToString(), ReportSizeInBytes = x.TotalSizeInBits / 8 });
            string summaryTableText = ConsoleTable.From(summaryTableRows).Configure(o => o.NumberAlignment = Alignment.Right).ToStringAlternative();

            summary.Append(summaryTableText);

            if (asCppComment)
            {
                StringBuilder summaryAsComment = new StringBuilder();
                using (System.IO.StringReader reader = new System.IO.StringReader(summary.ToString()))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        summaryAsComment.AppendLine($"// {line}");
                    }
                }

                return summaryAsComment.ToString().Trim();
            }
            else
            {
                return summary.ToString();
            }
        }

        /// <summary>
        /// Optimizes descriptor items by removing duplicate Global items.
        /// </summary>
        private static List<ShortItem> OptimizeRemoveDuplicateGlobals(IReadOnlyList<ShortItem> lastGeneratedDescriptorItems)
        {
            int currentPhysicalMinimum = HidConstants.PhysicalMinimumUndefinedValue;
            int currentPhysicalMaximum = HidConstants.PhysicalMaximumUndefinedValue;
            double currentUnitExponentMultiplier = HidConstants.UnitExponentUndefinedValueMultiplier;

            UnitItem currentUnitItem = new UnitItem(HidUnitDefinitions.GetInstance().TryFindUnitByName(HidUnitDefinitions.NoneName));

            // There is no implicit/default value for Logical Minimum/Maximum.
            int? currentLogicalMinimum = null;
            int? currentLogicalMaximum = null;

            UsagePageItem currentUsagePage = null;
            uint currentReportCount = 0;
            uint currentReportSize = 0;

            // No implicit/default value for ReportIds.
            ReportIdItem currentReportIdItem = null;

            List<ShortItem> descriptorItemsOptimized = new List<ShortItem>();

            foreach (ShortItem item in lastGeneratedDescriptorItems)
            {
                if (item.GetType() == typeof(PhysicalMinimumItem))
                {
                    PhysicalMinimumItem castItem = (PhysicalMinimumItem)(item);

                    if (castItem.Value == currentPhysicalMinimum)
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }

                    currentPhysicalMinimum = castItem.Value;
                }
                else if (item.GetType() == typeof(PhysicalMaximumItem))
                {
                    PhysicalMaximumItem castItem = (PhysicalMaximumItem)(item);

                    if (castItem.Value == currentPhysicalMaximum)
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }

                    currentPhysicalMaximum = castItem.Value;
                }
                else if (item.GetType() == typeof(LogicalMinimumItem))
                {
                    LogicalMinimumItem castItem = (LogicalMinimumItem)(item);

                    if (currentLogicalMinimum == null || castItem.Value != currentLogicalMinimum)
                    {
                        currentLogicalMinimum = castItem.Value;
                    }
                    else
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }
                }
                else if (item.GetType() == typeof(LogicalMaximumItem))
                {
                    LogicalMaximumItem castItem = (LogicalMaximumItem)(item);

                    if (currentLogicalMaximum == null || castItem.Value != currentLogicalMaximum)
                    {
                        currentLogicalMaximum = castItem.Value;
                    }
                    else
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }
                }
                else if (item.GetType() == typeof(UsagePageItem))
                {
                    UsagePageItem castItem = (UsagePageItem)(item);

                    // Note: Must check for null, as currentUsagePage is initialized to null.
                    if (currentUsagePage != null && castItem.Value == currentUsagePage.Value)
                    {
                        continue;
                    }

                    currentUsagePage = castItem;
                }
                else if (item.GetType() == typeof(ReportCountItem))
                {
                    ReportCountItem castItem = (ReportCountItem)(item);

                    if (castItem.Count == currentReportCount)
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }

                    currentReportCount = castItem.Count;
                }
                else if (item.GetType() == typeof(ReportSizeItem))
                {
                    ReportSizeItem castItem = (ReportSizeItem)(item);

                    if (castItem.SizeInBits == currentReportSize)
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }

                    currentReportSize = castItem.SizeInBits;
                }
                else if (item.GetType() == typeof(UnitExponentItem))
                {
                    UnitExponentItem castItem = (UnitExponentItem)(item);

                    if (castItem.Value == currentUnitExponentMultiplier)
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }

                    currentUnitExponentMultiplier = castItem.Value;
                }
                else if (item.GetType() == typeof(UnitItem))
                {
                    UnitItem castItem = (UnitItem)(item);

                    if (currentUnitItem == null || !castItem.Unit.Equals(currentUnitItem.Unit))
                    {
                        currentUnitItem = castItem;
                    }
                    else
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }
                }
                else if (item.GetType() == typeof(ReportIdItem))
                {
                    ReportIdItem castItem = (ReportIdItem)(item);

                    if (currentReportIdItem == null || !(castItem.Id == currentReportIdItem.Id))
                    {
                        currentReportIdItem = castItem;
                    }
                    else
                    {
                        // Ignore; don't add to new list.
                        continue;
                    }
                }

                descriptorItemsOptimized.Add(item);
            }

            return descriptorItemsOptimized;
        }

        private List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();
            foreach (ApplicationCollectionModule ac in this.ApplicationCollections)
            {
                descriptorItems.AddRange(ac.GenerateDescriptorItems(optimize));
            }

            return descriptorItems;
        }

        private string GenerateIndent(int indentLevel, string indent)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append(indent);
            }

            return sb.ToString();
        }
    }
}
