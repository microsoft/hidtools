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
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Kinds of UsagePage.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HidUsagePageKind
    {
        /// <summary>
        /// All Usages within Page are pre-defined.
        /// </summary>
        Defined,

        /// <summary>
        /// Usages are generated on-demand (e.g. Ordinal/Button).
        /// </summary>
        Generated,
    }

    /// <summary>
    /// Represents a UsagePage as defined by the HID specification.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HidUsagePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsagePage"/> class.
        /// Does not contain any Usages.
        /// </summary>
        /// <param name="id">Id of the new Page.</param>
        /// <param name="name">Name of the new Page.</param>
        public HidUsagePage(ushort id, string name)
        {
            this.Initialize(id, name, null, HidUsagePageKind.Defined, 0, 0, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsagePage"/> class.
        /// </summary>
        /// <param name="id">Id of the new Page.</param>
        /// <param name="name">Name of the new Page.</param>
        /// <param name="startUsageId">Start Id of Usages that can be generated.</param>
        /// <param name="endUsageId">End Id of Usages that can be generated.</param>
        /// <param name="kinds">Kinds for each Usage that will be generated.</param>
        public HidUsagePage(ushort id, string name, string usageNamePrefix, ushort startUsageId, ushort endUsageId, List<HidUsageKind> kinds)
        {
            this.Initialize(id, name, usageNamePrefix, HidUsagePageKind.Generated, startUsageId, endUsageId, kinds);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsagePage"/> class.
        /// Used for both Defined and Generated UsagePages by JSON serialization.
        /// </summary>
        /// <param name="id">Id of the new Page.</param>
        /// <param name="name">Name of the new Page.</param>
        /// <param name="kind">Kind of UsagePage.</param>
        /// <param name="startUsageId">Start Id of Usages that can be generated.</param>
        /// <param name="endUsageId">End Id of Usages that can be generated.</param>
        /// <param name="kinds">Kinds for each Usage that will be generated.</param>
        [JsonConstructor]
        public HidUsagePage(ushort id, string name, HidUsagePageKind kind, ushort startUsageId, ushort endUsageId, List<HidUsageKind> kinds)
        {
            this.Initialize(id, name, null, kind, startUsageId, endUsageId, kinds);
        }

        /// <summary>
        /// Gets the Kind of UsagePage this is.
        /// Dictates whether <see cref="UsageIds"/> or <see cref="UsageIdGenerator"/> are null.
        /// </summary>
        [JsonProperty]
        public HidUsagePageKind Kind { get; private set; }

        /// <summary>
        /// Gets the Page Id as defined in the HID Usage Table.
        /// </summary>
        [JsonProperty]
        public ushort Id { get; private set; }

        /// <summary>
        /// Gets the Page Name as defined in the HID Usage Table.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the list of known <see cref="HidUsageId"/>s that are associated with this <see cref="HidUsagePage"/>.
        /// </summary>
        [JsonProperty]
        public List<HidUsageId> UsageIds { get; private set; }

        /// <summary>
        /// Gets the alternative to list of Ids, where <see cref="HidUsageId"/>s can be generated automatically.
        /// </summary>
        [JsonProperty]
        public HidUsageGenerator UsageIdGenerator { get; private set; }

        /// <summary>
        /// If UsagePage created from JSON deserialization, UsageIds will not have the link
        /// back to it's UsagePage.  Add that link now.
        /// </summary>
        public void UpdateHidUsageIds()
        {
            foreach (HidUsageId id in this.UsageIds)
            {
                id.SetPage(this);
            }
        }

        /// <summary>
        /// Attempts to find the UsageId.
        /// </summary>
        /// <param name="usageIdName">Name of the UsageId to find.</param>
        /// <returns>Found UsageId, or null.</returns>
        public HidUsageId TryFindUsageId(string usageIdName)
        {
            // Check the Predefined and cached Generated UsageIds first.
            HidUsageId matchingUsageId = this.UsageIds.Where(x => x.Name.Equals(usageIdName, StringComparison.OrdinalIgnoreCase))?.FirstOrDefault();
            if (matchingUsageId != null)
            {
                return matchingUsageId;
            }

            // Couldn't find it, try and generate it (if applicable).
            if (this.Kind == HidUsagePageKind.Generated && this.UsageIdGenerator != null)
            {
                HidUsageId generatedUsageId = this.UsageIdGenerator.TryGenerate(usageIdName, this);
                if (generatedUsageId != null)
                {
                    generatedUsageId.SetPage(this);

                    return generatedUsageId;
                }
            }

            // Couldn't find it.
            return null;
        }

        /// <summary>
        /// Attempts to find the UsageId.
        /// </summary>
        /// <param name="usageId">Id of the UsageId to find.</param>
        /// <returns>Found UsageId, or null.</returns>
        public HidUsageId TryFindUsageId(UInt16 usageId)
        {
            // Check the Predefined and cached Generated UsageIds first.
            HidUsageId matchingUsageId = this.UsageIds.Where(x => x.Id == usageId)?.FirstOrDefault();
            if (matchingUsageId != null)
            {
                return matchingUsageId;
            }

            // Couldn't find it, try and generate it (if applicable).
            if (this.Kind == HidUsagePageKind.Generated && this.UsageIdGenerator != null)
            {
                HidUsageId generatedUsageId = this.UsageIdGenerator.TryGenerate(usageId, this);
                if (generatedUsageId != null)
                {
                    generatedUsageId.SetPage(this);

                    return generatedUsageId;
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to add a UsageId to this UsagePage.
        /// </summary>
        /// <param name="usageId">UsageId to add.</param>
        /// <returns>Added UsageId.  Same as supplied to method; returned as a convience.</returns>
        public HidUsageId AddUsageId(HidUsageId usageId)
        {
            if (this.Kind != HidUsagePageKind.Defined)
            {
                throw new HidSpecificationException(Resources.ExceptionUsagePageCannotAddIdToGeneratedPage, this);
            }

            // No Usage with the same Id expected to be present on the UsagePage.
            HidUsageId foundUsageIdFromId = this.TryFindUsageId(usageId.Id);
            if (foundUsageIdFromId != null)
            {
                throw new HidSpecificationException(Resources.ExceptionUsagePageUsageIdAlreadyExists, usageId, this, foundUsageIdFromId, this.GetNextAvailableId());
            }

            // No Usage with the same Name expected to be present on the UsagePage.
            HidUsageId foundUsageIdFromName = this.TryFindUsageId(usageId.Name);
            if (foundUsageIdFromName != null)
            {
                throw new HidSpecificationException(Resources.ExceptionUsagePageUsageNameAlreadyExists, usageId, this, foundUsageIdFromName);
            }

            usageId.SetPage(this);

            this.UsageIds.Add(usageId);

            return usageId;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string hexid = $"0x{this.Id:X4}";

            return $"{this.Name}[{hexid}]";
        }

        /// <summary>
        /// Checks for equality to another <see cref="HidUsagePage"/>.
        /// </summary>
        /// <param name="comparePage"><see cref="HidUsagePage"/> to compare against.</param>
        /// <returns>True if <see cref="HidUsagePage"/> functionally identically to this.</returns>
        public bool Equals(HidUsagePage comparePage)
        {
            return (comparePage.Id == this.Id && string.Equals(comparePage.Name, comparePage.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Compares this <see cref="HidUsagePage"/>. to another <see cref="HidUsagePage"/>.
        /// </summary>
        /// <param name="comparePage"><see cref="HidUsagePage"/>. to compare against.</param>
        /// <returns>0 if identical, -1/1 if not.</returns>
        public int CompareTo(HidUsagePage comparePage)
        {
            if (comparePage.Id == this.Id && !comparePage.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("PageIds are the same but Names are different.  Something is terribly screwed-up");
            }

            return this.Id.CompareTo(comparePage.Id);
        }

        private void Initialize(ushort id, string name, string usageNamePrefix, HidUsagePageKind pageKind, ushort startUsageId, ushort endUsageId, List<HidUsageKind> kinds)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name can't be empty or null");
            }

            this.Id = id;
            this.Name = name;
            this.Kind = pageKind;

            switch (pageKind)
            {
                case HidUsagePageKind.Defined:
                {
                    this.UsageIds = new List<HidUsageId>();
                    this.UsageIdGenerator = null;

                    break;
                }

                case HidUsagePageKind.Generated:
                {
                    this.UsageIds = new List<HidUsageId>();
                    this.UsageIdGenerator = new HidUsageGenerator(usageNamePrefix, startUsageId, endUsageId, kinds);

                    break;
                }

                default:
                {
                    System.Diagnostics.Debug.Assert(false, "Unexpected");
                    break;
                }
            }
        }

        /// <summary>
        /// Identifies the next id that does not have a defined UsageId.
        /// </summary>
        private UInt16 GetNextAvailableId()
        {
            // Fetch ids of all UsageIds.
            List<UInt16> sortedIds = this.UsageIds.Select(x => x.Id).ToList();
            sortedIds.Sort();

            int firstAvailableId = 1;

            // Examine each id sequentially, and find the first missing from the sequence.
            foreach (UInt16 id in sortedIds)
            {
                if (firstAvailableId == id)
                {
                    firstAvailableId++;
                }
                else
                {
                    // Found the first available id.
                    break;
                }
            }

            if (firstAvailableId > HidConstants.UsageIdMaximum)
            {
                throw new HidSpecificationException(Resources.ExceptionCannotFindNextAvailableUsageId);
            }

            return Convert.ToUInt16(firstAvailableId);
        }
    }
}
