// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidSpecification
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Generates HidUsages on-the-fly (according to a pattern) rather than at compile-time.
    /// This is used primarily by the Button/Ordinal pages, where there are 65535 UsageIds (all 'pre-defined').
    /// It would be silly (and wasteful) to pre-generate all of these, so rather it is done on demand.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HidUsageGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsageGenerator"/> class.
        /// </summary>
        /// <param name="namePrefix">Name of every generated Usage.</param>
        /// <param name="startUsageId">First valid UsageId.</param>
        /// <param name="endUsageId">Last valid UsageId.</param>
        /// <param name="kinds">Kinds to associate with generated UsageIds.</param>
        public HidUsageGenerator(string namePrefix, ushort startUsageId, ushort endUsageId, List<HidUsageKind> kinds)
        {
            this.NamePrefix = namePrefix;
            this.StartUsageId = startUsageId;
            this.EndUsageId = endUsageId;
            this.Kinds = kinds;
        }

        /// <summary>
        /// Gets the Name all generated Usages shall have.
        /// </summary>
        [JsonProperty]
        public string NamePrefix { get; }

        /// <summary>
        /// Gets the first valid UsageId for this generator.  All IDs between <see cref="StartUsageId"/> and <see cref="EndUsageId"/> (inclusive) are valid.
        /// </summary>
        [JsonProperty]
        public ushort StartUsageId { get; }

        /// <summary>
        /// Gets the ast valid UsageId for this generator.  All IDs between <see cref="StartUsageId"/> and <see cref="EndUsageId"/> (inclusive) are valid.
        /// </summary>
        [JsonProperty]
        public ushort EndUsageId { get; }

        /// <summary>
        /// Gets the Usage kinds as defined the HID Usage Table. Most UsageIds will only have a single kind.
        /// </summary>
        [JsonProperty]
        public List<HidUsageKind> Kinds { get; }

        /// <summary>
        /// Attempts to generate a new <see cref="HidUsageId"/> from the specified Id.
        /// </summary>
        /// <param name="id">Id of the Usage to generate.</param>
        /// <param name="page">Page of the Usage to generate.</param>
        /// <returns>null if the id is out-of-range.</returns>
        public HidUsageId TryGenerate(ushort id, HidUsagePage page)
        {
            if (id < this.StartUsageId || id > this.EndUsageId)
            {
                return null;
            }

            // Has it been generated before?
            HidUsageId existingUsageId = page.UsageIds.Where(x => x.Id == id)?.FirstOrDefault();
            if (existingUsageId != null)
            {
                return existingUsageId;
            }

            // Name of this new usage (e.g. "Button 59").
            string usageIdName = $"{this.NamePrefix} {id}";

            return new HidUsageId(id, usageIdName, this.Kinds, page);
        }

        /// <summary>
        /// Attempts to generate a new <see cref="HidUsageId"/> from the specified Id string
        /// Expects a format of "Button 4", "Foo Whizz Bar 33", etc...
        /// </summary>
        /// <param name="id">Id of the Usage to generate.</param>
        /// <param name="page">Page of the Usage to generate.</param>
        /// <returns>null if unable to generate the <see cref="HidUsageId"/>.</returns>
        public HidUsageId TryGenerate(string id, HidUsagePage page)
        {
            // Expect a usage (1 or more space seperated words) with a numbered suffix.
            // e.g. "Button 4", "Instance 59", "Foo Whizz Bar 33"
            string[] tokens = id.Split(' ');

            // Must have at least a single word and a number.
            if (tokens.Length < 2)
            {
                return null;
            }

            // Validate the requested usage prefix is the same as that of the UsagePage name.
            // Ignore the last token (which should be a number).
            if (!id.StartsWith(this.NamePrefix))
            {
                return null;
            }

            ushort numberSuffix = 0;
            if (!ushort.TryParse(tokens.Last(), out numberSuffix))
            {
                return null;
            }

            // Actually generate the HidUsage from Id and Page.
            return this.TryGenerate(numberSuffix, page);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{this.StartUsageId}-{this.EndUsageId}]";
        }
    }
}
