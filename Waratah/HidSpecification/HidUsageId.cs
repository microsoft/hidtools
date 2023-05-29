// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidSpecification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.HidTools.HidSpecification.Properties;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Usage Kinds, as described in the HID Usage Tables.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Items self-explanatory.")]
    public enum HidUsageKind
    {
        // Button Kinds
        LC,
        OOC,
        MC,
        OSC,
        RTC,

        // Data Kinds
        Sel,
        SV,
        SF,
        DV,
        DF,
        BufferedBytes,

        // Collection Kinds.
        NAry,
        CA,
        CL,
        CP,
        US,
        UM,
    }

    /// <summary>
    /// Represents a UsageId as defined by the HID specification.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HidUsageId
    {
        private bool isTransformed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsageId"/> class.
        /// Will automatically add it to the the <see cref="HidUsagePage"/> internal list.
        /// </summary>
        /// <param name="id">Id of new UsageId.</param>
        /// <param name="name">Name of the new UsageId.</param>
        /// <param name="kinds">Valid kinds for this UsageId.</param>
        /// <param name="page">UsagePage this UsageId will be added to.</param>
        /// <param name="isTransformed">If this was generated as a result of transformation.</param>
        [JsonConstructor]
        public HidUsageId(ushort id, string name, List<HidUsageKind> kinds, HidUsagePage page = null, bool isTransformed = false)
        {
            this.Id = id;
            this.Name = name;
            this.Kinds = kinds;
            this.Page = null;
            this.isTransformed = isTransformed;

            page?.UsageIds.Add(this);
        }

        /// <summary>
        /// Gets the Usage Id as defined in the HID Usage Table.
        /// </summary>
        [JsonProperty]
        public ushort Id { get; }

        /// <summary>
        /// Gets the Usage Name as defined in the HID Usage Table.
        /// </summary>
        [JsonProperty]
        public string Name { get; }

        /// <summary>
        /// Gets the Usage kinds as defined in the HID Usage Table.
        /// Most UsageIds will only have a single kind.
        /// </summary>
        [JsonProperty]
        public List<HidUsageKind> Kinds { get; }

        /// <summary>
        /// Gets the Usage Page associated with this UsageId.
        /// Value updated via a seperate Setter to place nice with JSON serialization.
        /// </summary>
        public HidUsagePage Page { get; private set; }

        /// <summary>
        /// Sets the associated <see cref="HidUsagePage"/>.
        /// </summary>
        /// <param name="page">Page to associated with this Id.</param>
        public void SetPage(HidUsagePage page)
        {
            this.Page = page;
        }

        /// <summary>
        /// Generates a transformed <see cref="HidUsageId"/> by XOR'ing this with another <see cref="HidUsageId"/>.
        /// </summary>
        /// <param name="usageTransform"><see cref="HidUsageId"/> to transform this with.</param>
        /// <returns>New <see cref="HidUsageId"/> with transform applied.</returns>
        public HidUsageId Transform(HidUsageId usageTransform)
        {
            if (this.Page != usageTransform.Page)
            {
                throw new HidSpecificationException(Resources.ExceptionInvalidTransformUsageId, usageTransform.Page, this.Page);
            }

            // Now transform the Usage with the UsageTransform.
            // Transformation occurs by OR'ing the Usage Ids together.
            UInt16 usageIdTransformed = (UInt16)(this.Id | usageTransform.Id);
            string usageNameTransformed = $"{this.Name} || ({usageTransform.Name})";

            HidUsageId usageTransformed = this.Page.TryFindUsageId(usageIdTransformed);
            if (usageTransformed == null)
            {
                // Couldn't find an existing transformed Usage, create one.
                usageTransformed = new HidUsageId(usageIdTransformed, usageNameTransformed, this.Kinds, null, true);
                this.Page.AddUsageId(usageTransformed);
            }
            else
            {
                // The transformed Usage already existed.  Ensure that it's a transformed Usage, and not a 'real' one that is already defined.
                if (!usageTransformed.isTransformed)
                {
                    throw new HidSpecificationException(Resources.ExceptionInvalidTransformConflictWithExistingUsageId, usageTransformed);
                }
            }

            return usageTransformed;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string hexid = $"0x{this.Id:X4}";

            return $"{this.Name}[{hexid}]";
        }

        /// <summary>
        /// Checks for equality to another <see cref="HidUsageId"/>.
        /// </summary>
        /// <param name="compareId"><see cref="HidUsageId"/> to compare against.</param>
        /// <returns>True if <see cref="HidUsageId"/> functionally identically to this.</returns>
        public bool Equals(HidUsageId compareId)
        {
            return (compareId.Id == this.Id && string.Equals(this.Name, compareId.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Compares this <see cref="HidUsageId"/>. to another <see cref="HidUsageId"/>.
        /// </summary>
        /// <param name="compareId"><see cref="HidUsageId"/>. to compare against.</param>
        /// <returns>0 if identical, -1/1 if not.</returns>
        public int CompareTo(HidUsageId compareId)
        {
            if (compareId.Id == this.Id && !compareId.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new HidSpecificationException(Resources.ExceptionUsageNamesMismatch, this.Name, compareId.Name);
            }

            return this.Id.CompareTo(compareId.Id);
        }
    }
}
