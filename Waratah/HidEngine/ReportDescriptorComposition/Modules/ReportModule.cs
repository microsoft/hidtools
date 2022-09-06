// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorItems;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Kind of ReportDescriptor Report.
    /// </summary>
    public enum ReportKind
    {
        /// <summary>
        /// Input Report.
        /// </summary>
        Input,

        /// <summary>
        /// Output Report.
        /// </summary>
        Output,

        /// <summary>
        /// Feature Report.
        /// </summary>
        Feature,
    }

    /// <summary>
    /// Describes an abstract HID 'Report'.
    /// Reports belong to a single ApplicationCollection, and represents a discrete/complete message,
    /// that will be sent-to/recieved-from a device.
    /// </summary>
    public class ReportModule : BaseModule
    {
        // Maximum allowed bits permitted to occur in bytes after the first byte for the Module.
        private const int MaxAllowedRemainingBits = (HidConstants.ItemByteSpanningMaximum - 1) * 8;

        private int id = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportModule"/> class.
        /// </summary>
        /// <param name="kind">Kind of report this is.</param>
        /// <param name="parent">The parent module of this module.</param>
        public ReportModule(ReportKind kind, BaseModule parent)
            : base(parent)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the Kind of Report.
        /// </summary>
        public ReportKind Kind { get; private set; }

        /// <summary>
        /// Gets the Id of this Report.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }

            private set
            {
                if (value < HidConstants.ReportIdMinimum || value > HidConstants.ReportIdMaximum)
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorReportIdInvalidRange, value, HidConstants.ReportIdMinimum, HidConstants.ReportIdMaximum);
                }

                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the 'name' of this module.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the children contained within this Report.
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
        /// Secondary step of initialization, to associate Report with it's Id and children.
        /// <p/>
        /// Initialization is split into two steps to permit the Parent of an Item
        /// to be created before it's children, and the children to have a reference to it's parent.
        /// </summary>
        /// <param name="id">Id of this Report.  It is up to the caller to ensure Ids are unique for all reports of a particular kind across all ApplicationCollections.</param>
        /// <param name="name">Logical name of this module. Optional.</param>
        /// <param name="children">Items belonging to this Report.</param>
        public void Initialize(int id, string name, List<BaseModule> children)
        {
            this.Id = id;
            this.Name = name;

            if (children == null || children.Count == 0)
            {
                throw new DescriptorModuleParsingException(HidEngine.Properties.Resources.ExceptionDescriptorReportModuleNoItems);
            }
            else
            {
                this.Children = children;
            }

            this.ValidateAndFix();
        }

        /// <inheritdoc/>
        public override List<ShortItem> GenerateDescriptorItems(bool optimize)
        {
            List<ShortItem> descriptorItems = new List<ShortItem>();

            // Always safe to convert to UInt32, as previous checks prevent -ve values.
            descriptorItems.Add(new ReportIdItem(Convert.ToUInt32(this.Id)));

            foreach (BaseModule item in this.Children)
            {
                descriptorItems.AddRange(item.GenerateDescriptorItems(optimize));
            }

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

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Kind}{this.Id}";
        }

        private void ValidateAndFix()
        {
            this.PadOverSpanningModules();

            this.PadReportToByteAlign();

            this.CombineContiguousPaddingModules();

            this.ValidateDoesNotContainOnlyPaddingModules();
        }

        /// <summary>
        /// Identifies modules spanning more than <see cref="HidConstants.ItemByteSpanningMaximum"/> bytes, adding
        /// padding or increasing field size to correct overspanning. (In compliance with hid_11:8.4).
        /// </summary>
        private void PadOverSpanningModules()
        {
            bool isModuleModified = true;

            // Insert/modify modules to mitigate any overspanning.
            while (isModuleModified)
            {
                int cumulativeSizeInBits = 0;
                isModuleModified = false;

                foreach (BaseElementModule module in this.GetReportElements())
                {
                    // Overspanning can happen only if a Module element is >=26. (min size to span 4bytes).
                    // Note: ArrayModule will never overspan, as it's max element size is sizeof(Usage) == 16bit.
                    if (!(module is ArrayModule) && this.TryPadOverspanningBaseReportElementModule(module, cumulativeSizeInBits))
                    {
                        isModuleModified = true;
                        break;
                    }
                    else
                    {
                        cumulativeSizeInBits += module.TotalSizeInBits;
                    }
                }
            }
        }

        /// <summary>
        /// Byte-aligns the report (if required.).
        /// hid_11:8.4 - "A report is always byte-aligned. If required, reports are padded with bits (0) until the next byte boundary is reached.".
        /// </summary>
        private void PadReportToByteAlign()
        {
            bool isWholeBytes = (this.TotalSizeInBits % 8) == 0;

            if (!isWholeBytes)
            {
                int requiredBitsToBeWhole = 8 - (this.TotalSizeInBits % 8);

                PaddingModule padding = new PaddingModule(requiredBitsToBeWhole, this);

                this.Children.Add(padding);
            }
        }

        /// <summary>
        /// Attempts to combines any contiguous paddingModules, into a single module.
        /// Guarantees non-combination if overspanning would occur.
        /// </summary>
        private void CombineContiguousPaddingModules()
        {
            bool isModuleModified = true;

            while (isModuleModified)
            {
                int cumulativeSizeInBits = 0;
                isModuleModified = false;

                // Don't examine intermediate collections, since the BaseReportElements are
                // the only ones that have bits in the report.
                List<BaseElementModule> modules = this.GetReportElements();

                for (int i = 0; i < modules.Count; i++)
                {
                    if (modules[i] is PaddingModule currentPaddingModule)
                    {
                        // Prevent indexing into non-existent 'next' module.
                        if ((i + 1) < modules.Count && modules[i + 1] is PaddingModule nextPaddingModule)
                        {
                            // Found 2 contiguous paddingModules.

                            // NonAdjustedSizeInBits should always be used, regardless of "Packing".
                            //
                            // With-Packing: Overspanning can never occur (as all fields are on 8bit+ boundaries), so it doesn't matter if
                            // combined paddingModule size is smaller than previous (will always still be on 8bit+ boundary).
                            // Without-Packing: Overspanning can occur, but the (adjusted) size is the same as the non-adjusted, so combining
                            // will never change the effective size of padding.
                            int combinedPaddingSizeInBits = currentPaddingModule.NonAdjustedSizeInBits + nextPaddingModule.NonAdjustedSizeInBits;

                            // Determine if these paddingModules can be combined.
                            // TODO: Opportunity here for better optimization, (e.g. can't combine 31bit + 17bit padding to 1x 48bit (>32).  Rather than leave them separate,
                            // more efficient to express as 2x24bit (paddingModule count == 2).
                            // Since this amount of padding should be "very" uncommon, don't bother with this for now.
                            if (combinedPaddingSizeInBits <= HidConstants.SizeInBitsMaximum)
                            {
                                if (!this.IsModuleFieldOverSpanning(cumulativeSizeInBits, combinedPaddingSizeInBits, out _))
                                {
                                    // Contiguous paddingModules can be combined.

                                    // Increase the size of the 1st paddingModule, so it represents the combined size.
                                    currentPaddingModule.NonAdjustedSizeInBits = combinedPaddingSizeInBits;

                                    // Remove 2nd paddingModule as it's no longer needed.
                                    switch (nextPaddingModule.Parent)
                                    {
                                        case ReportModule parentReportModule:
                                        {
                                            parentReportModule.Children.Remove(nextPaddingModule);
                                            break;
                                        }

                                        case CollectionModule parentCollection:
                                        {
                                            parentCollection.Children.Remove(nextPaddingModule);
                                            break;
                                        }

                                        default:
                                        {
                                            System.Environment.FailFast("PaddingModule must be a child of either a Report or BaseCollection");
                                            break;
                                        }
                                    }

                                    // Start loop again since we've removed a module, and the list is out-of-whack.
                                    isModuleModified = true;
                                    break;
                                }
                            }
                        }
                    }

                    // Not a paddingModule that can be combined with another.
                    cumulativeSizeInBits += modules[i].TotalSizeInBits;
                }
            }
        }

        /// <summary>
        /// Pads module if it overspans in Report. (hid_11 spec (8.4 Report Contraints), "An item field cannot span more than 4 bytes in a report.").
        /// </summary>
        /// <param name="module">Module to pad if it overspans.  Prefix-padding will be added 'before' this module if required.</param>
        /// <param name="reportSizeInBits">Size of the report up to this module.</param>
        /// <returns><b>true</b> if padding was added, <b>false</b> otherwise.</returns>
        /// <exception cref="DescriptorModuleParsingException">Module overspans and could not be padded.</exception>
        private bool TryPadOverspanningBaseReportElementModule(BaseElementModule module, int reportSizeInBits)
        {
            // Try adding a mix of up to 7bits prefix-padding, and increasing the size of each field of the Module
            // to see if this will solve the problem.
            for (int fieldSizeInBits = module.SizeInBits; fieldSizeInBits <= HidConstants.SizeInBitsMaximum; fieldSizeInBits++)
            {
                for (int paddingSizeInBits = 0; paddingSizeInBits < 8; paddingSizeInBits++)
                {
                    if (!this.IsAnyModuleFieldOverspanning(reportSizeInBits, module.Count, fieldSizeInBits, paddingSizeInBits))
                    {
                        if (fieldSizeInBits == module.SizeInBits && paddingSizeInBits == 0)
                        {
                            // Module did not initially overspan - bail out early with no padding.
                            return false;
                        }
                        else
                        {
                            // Adding padding bits between this Module and the previous Module to fill-up these bits.
                            // This moves the current Module to start at the next byte.
                            // Note: paddingSize will always be non-zero, as first step of overspanning is to pad to next byte.
                            // TODO: Communicate to the user overspanning correction has occurred. (Don't want to use the generic console, so no good way to do this yet...
                            // perhaps, a seperate Verbose DebugChannel, that can be optionally written to console or log file.)
                            this.TryInsertPaddingBeforeModule(module, paddingSizeInBits);
                            module.NonAdjustedSizeInBits = fieldSizeInBits;

                            // Indicate to the caller a module overspan was corrected.
                            return true;
                        }
                    }
                }
            }

            // Couldn't correct overspanning, fatal error.
            // TODO: Determine what circumstance this could actually happen (if any)
            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorOverspanningCouldntBePrevented);
        }

        /// <summary>
        /// Determines if any field in this module overspans. (VariableModules can have multiple fields associated with them).
        /// Module is not explicitly a parameter, to permit easier experimentation with field/padding sizes.
        /// </summary>
        /// <param name="reportSizeInBits">Size of report up to this module (in bits).</param>
        /// <param name="fieldCount">Number of fields associated with this module.</param>
        /// <param name="fieldSizeInBits">Size of each module field (in bits).</param>
        /// <param name="prefixPaddingSizeInBits">Size of prefix-padding (in bits).</param>
        /// <returns><b>true</b> if module will overspan, <b>false</b> otherwise.</returns>
        private bool IsAnyModuleFieldOverspanning(int reportSizeInBits, int fieldCount, int fieldSizeInBits, int prefixPaddingSizeInBits)
        {
            reportSizeInBits += prefixPaddingSizeInBits;

            // Validate every field produced by this module, will not overspan.
            for (int i = 0; i < fieldCount; i++)
            {
                if (this.IsModuleFieldOverSpanning(reportSizeInBits, fieldSizeInBits, out _))
                {
                    return true;
                }
                else
                {
                    reportSizeInBits += fieldSizeInBits;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a specific module field overspans.
        /// Module field is defined by it's position in the report, and it's size.
        /// </summary>
        /// <param name="reportSizeInBits">Size of report up to this module field (in bits).</param>
        /// <param name="fieldSizeInBits">Size of module field (in bits).</param>
        /// <param name="paddingSizeInBits">Size of prefix-padding (in bits), to ensure field does not overspan. 0 if <b>false</b> returned.</param>
        /// <returns><b>true</b> if module field will overspan, <b>false</b> otherwise.</returns>
        private bool IsModuleFieldOverSpanning(int reportSizeInBits, int fieldSizeInBits, out int paddingSizeInBits)
        {
            paddingSizeInBits = 0;

            if (fieldSizeInBits > HidConstants.SizeInBitsMaximum || fieldSizeInBits < HidConstants.SizeInBitsMinimum)
            {
                throw new ArgumentException($"Invalid field size {fieldSizeInBits}");
            }

            int bitPositionInLastReportByte = reportSizeInBits % 8;

            // Remaining bits in the report byte that can be utilized for this field's bits.
            int bitsRemainingInByte = 8 - bitPositionInLastReportByte;

            // Bits of this field that cannot fit into the last report byte.
            // Will be -ve when field can fit into remaining byte bits.
            int bitsNotInByte = fieldSizeInBits - bitsRemainingInByte;

            if (bitsNotInByte > MaxAllowedRemainingBits)
            {
                paddingSizeInBits = bitsRemainingInByte;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to insert a PaddingModule before the provided module.
        /// This is useful to prevent module overspanning.
        /// </summary>
        /// <param name="beforeModule">Module to insert new PaddingModule before.</param>
        /// <param name="paddingSizeInBits">Size of padding to add (in bits).</param>
        private void TryInsertPaddingBeforeModule(BaseModule beforeModule, int paddingSizeInBits)
        {
            List<BaseModule> modules = null;

            switch (beforeModule.Parent)
            {
                case ReportModule parentReportModule:
                {
                    modules = parentReportModule.Children;
                    break;
                }

                case CollectionModule parentCollection:
                {
                    modules = parentCollection.Children;
                    break;
                }

                default:
                {
                    throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorReportUnexpectedModuleEncountered);
                }
            }

            for (int i = 0; i < modules.Count; i++)
            {
                if (Object.ReferenceEquals(modules[i], beforeModule))
                {
                    // Assumed multiple PaddingModules will be combined, so OK to just add another here, even
                    // if there was another immediately previous that may have been suitable.
                    modules.Insert(i, new PaddingModule(paddingSizeInBits, beforeModule.Parent));

                    return;
                }
            }

            // Requested 'beforeModule' was not a child of it's reported parent.
            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorReportUnexpectedModuleEncountered);
        }

        /// <summary>
        /// Validates that this ReportModule doesn't contain 'only' paddingItems, as otherwise what's the point
        /// of the Report!.
        /// </summary>
        private void ValidateDoesNotContainOnlyPaddingModules()
        {
            foreach (BaseElementModule module in this.GetReportElements())
            {
                if (!(module is PaddingModule))
                {
                    return;
                }
            }

            throw new DescriptorModuleParsingException(Resources.ExceptionDescriptorReportOnlyPaddingItemsEncountered);
        }
    }
}
