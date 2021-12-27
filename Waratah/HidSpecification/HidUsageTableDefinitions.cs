// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidSpecification
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using HidSpecification.Properties;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Container to store all UsagePages and Usages as defined by HID Usage Tables document.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HidUsageTableDefinitions
    {
        private static HidUsageTableDefinitions instance = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidUsageTableDefinitions"/> class.
        /// </summary>
        /// <param name="usagePages">All publicly documented UsagePages.</param>
        /// <param name="usageTableVersion">Version of the HID Usage Table.</param>
        /// <param name="usageTableRevision">Revision of the HID Usage Table.</param>
        /// <param name="usageTableSubRevisionInternal">Subrevision of the HID Usage Table.</param>
        /// <param name="lastGenerated">Date of table generation.</param>
        [JsonConstructor]
        public HidUsageTableDefinitions(
            List<HidUsagePage> usagePages,
            UInt16 usageTableVersion,
            UInt16 usageTableRevision,
            UInt16 usageTableSubRevisionInternal,
            DateTime lastGenerated)
        {
            // TODO: Validate UsagePages (e.g. for duplicates)
            // Should do it here in-case the cached Table has been modified/corrupted
            // in some way. (i.e. can't guranatee that it's correct).

            this.UsagePages = usagePages;
            foreach (HidUsagePage page in this.UsagePages)
            {
                page.UpdateHidUsageIds();
            }

            this.UsageTableVersion = usageTableVersion;
            this.UsageTableRevision = usageTableRevision;
            this.UsageTableSubRevisionInternal = usageTableSubRevisionInternal;

            this.LastGenerated = lastGenerated;
        }

        /// <summary>
        /// Gets the Version of the table.
        /// </summary>
        [JsonProperty]
        public UInt16 UsageTableVersion { get; }

        /// <summary>
        /// Gets the Revision of the table.
        /// </summary>
        [JsonProperty]
        public UInt16 UsageTableRevision { get; }

        /// <summary>
        /// Gets the Subrevision of the table.
        /// </summary>
        [JsonProperty]
        public UInt16 UsageTableSubRevisionInternal { get; }

        /// <summary>
        /// Gets the date the table was generated.
        /// </summary>
        [JsonProperty]
        public DateTime LastGenerated { get; }

        /// <summary>
        /// Gets the UsagePages of the table.
        /// </summary>
        [JsonProperty]
        public List<HidUsagePage> UsagePages { get; }

        /// <summary>
        /// Lazy accessor for singleton instance of Spec.  Initializes on first invocation.
        /// </summary>
        /// <param name="clear">When true, clears all non-default Units.  Useful for unit-tests.</param>
        /// <returns>Single instance.</returns>
        public static HidUsageTableDefinitions GetInstance(bool clear = false)
        {
            // TODO: Could support multiple versions of the HUT,
            // and define which version to use in TOML.
            if (instance == null || clear)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HidSpecification.ParsedUsageTables.ParsedHidUsageTable.json"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string parsedHidUsagesSpecContents = reader.ReadToEnd();

                        instance = DeserializeFromJson(parsedHidUsagesSpecContents);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Deserializes JSON to this.
        /// </summary>
        /// <param name="inputFilePathContents">Formatted JSON to be deserialized.</param>
        /// <returns>Deserialized table.</returns>
        public static HidUsageTableDefinitions DeserializeFromJson(string inputFilePathContents)
        {
            return JsonConvert.DeserializeObject<HidUsageTableDefinitions>(inputFilePathContents);
        }

        /// <summary>
        /// Serializes this as JSON to file.
        /// </summary>
        /// <param name="outputFilePath">File path to serialize to.</param>
        public void SerializeToJson(string outputFilePath)
        {
            string serializedFileContents = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(outputFilePath, serializedFileContents);
        }

        /// <summary>
        /// Attempts to find the UsagePage.
        /// </summary>
        /// <param name="usagePageId">Id of the UsagePage to find.</param>
        /// <returns>Found UsagePage, or null.</returns>
        public HidUsagePage TryFindUsagePage(UInt16 usagePageId)
        {
            return this.UsagePages.Where(x => x.Id == usagePageId).FirstOrDefault();
        }

        /// <summary>
        /// Attempts to find the UsagePage.
        /// </summary>
        /// <param name="usagePageName">Name of the UsagePage to find.</param>
        /// <returns>Found UsagePage, or null.</returns>
        public HidUsagePage TryFindUsagePage(string usagePageName)
        {
            return this.UsagePages.Where(x => x.Name == usagePageName).FirstOrDefault();
        }

        /// <summary>
        /// Attempts to find a Usage from the Global Usage Tables, from the UsagePages's name, and UsageId's name.
        /// </summary>
        /// <param name="usagePageName">Name of the UsagePage of the UsageId.</param>
        /// <param name="usageIdName">Name of the UsageId.</param>
        /// <returns> <see cref="HidUsageId"/> found, or null.</returns>
        public HidUsageId TryFindUsageId(string usagePageName, string usageIdName)
        {
            foreach (HidUsagePage page in this.UsagePages)
            {
                if (page.Name.Equals(usagePageName, StringComparison.OrdinalIgnoreCase))
                {
                    HidUsageId foundUsageId = page.TryFindUsageId(usageIdName);
                    if (foundUsageId != null)
                    {
                        return foundUsageId;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to find a Usage from the Global Usage Tables, from the UsagePages's id, and UsageId's id.
        /// </summary>
        /// <param name="usagePageId">Id of the UsagePage of the UsageId.</param>
        /// <param name="usageId">Id of the UsageId.</param>
        /// <returns> <see cref="HidUsageId"/> found, or null.</returns>
        public HidUsageId TryFindUsageId(UInt16 usagePageId, UInt16 usageId)
        {
            foreach (HidUsagePage page in this.UsagePages)
            {
                if (page.Id == usagePageId)
                {
                    HidUsageId foundUsageId = page.TryFindUsageId(usageId);
                    if (foundUsageId != null)
                    {
                        return foundUsageId;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Either creates a new UsagePage with the supplied parameters, or returns an existing UsagePage,
        /// with name or id (or both) matching.
        /// </summary>
        /// <param name="id">Id of the UsagePage that will be looked-up or created.  May be null.</param>
        /// <param name="name">Name of the Usage that will be looked-up or created.  May be null/empty.</param>
        /// <remarks>id or name must be specified.</remarks>
        /// <returns>UsagePage found or newly created.</returns>
        public HidUsagePage CreateOrGetUsagePage(UInt16? id, string name)
        {
            if (name != null && name != string.Empty && id != null)
            {
                // Both name and id were supplied.
                // Attempt to find a UsagePage with the same id and name.
                // If either (but not both) id and name match an existing UsagePage, this is an error.
                // If neither match an existing UsagePage, a new UsagePage will be created.

                HidUsagePage pageFromId = this.TryFindUsagePage(id.Value);
                HidUsagePage pageFromName = this.TryFindUsagePage(name);

                bool isBothIdAndNameMatchExisting = (pageFromId != null) && (pageFromId == pageFromName);
                bool isNeitherIdAndNameMatchExisting = (pageFromId == null) && (pageFromName == null);

                if (isBothIdAndNameMatchExisting)
                {
                    // Found a matching UsagePage.
                    return pageFromId;
                }
                else if (isNeitherIdAndNameMatchExisting)
                {
                    // This is a new (valid) UsagePage definition, create the UsagePage.
                    HidUsagePage page = new HidUsagePage(id.Value, name);

                    this.UsagePages.Add(page);

                    return page;
                }
                else if (pageFromName != null && pageFromId == null)
                {
                    throw new HidSpecificationException(Resources.ExceptionUsagePageNameMatchesIdDoesnt, name, pageFromName, id.Value);
                }
                else if (pageFromName == null && pageFromId != null)
                {
                    throw new HidSpecificationException(Resources.ExceptionUsagePageIdMatchesNameDoesnt, id.Value, pageFromId, name);
                }
                else if (pageFromName != null && pageFromId != null)
                {
                    throw new HidSpecificationException(Resources.ExceptionUsagePageNameAndIdMatchDifferentPages, name, pageFromName, id, pageFromId);
                }
            }
            else if (name != null)
            {
                HidUsagePage pageFromName = this.TryFindUsagePage(name);
                if (pageFromName == null)
                {
                    throw new HidSpecificationException(Resources.ExceptionUsagePageNameNotFoundIdOmitted, name);
                }

                return pageFromName;
            }
            else if (id != null)
            {
                HidUsagePage pageFromId = this.TryFindUsagePage(id.Value);
                if (pageFromId == null)
                {
                    throw new HidSpecificationException(Resources.ExceptionUsagePageIdNotFoundNameOmitted, id);
                }

                return pageFromId;
            }

            return null;
        }

        /// <summary>
        /// Validates that all Usages in the contiguous range defined by start/end exist.
        /// </summary>
        /// <param name="usageIdStart">First UsageId in the range.</param>
        /// <param name="usageIdEnd">last UsageId in the range.</param>
        public void ValidateRange(HidUsageId usageIdStart, HidUsageId usageIdEnd)
        {
            // Naturally, for a valid range, the Usages must be referring to the same page.
            if (!usageIdStart.Page.Equals(usageIdEnd.Page))
            {
                throw new HidSpecificationException(Resources.ExceptionRangeHasDifferentUsagePages, usageIdStart.Page, usageIdEnd.Page);
            }

            if (usageIdStart.Id.CompareTo(usageIdEnd.Id) >= 0)
            {
                throw new HidSpecificationException(Resources.ExceptionRangeUsageStartIdBiggerThanUsageEndId, usageIdStart.Id, usageIdEnd.Id);
            }

            // By definition, all generated UsageIds exists on a Page, so ungenerated UsageIds must exist between them.
            if (usageIdStart.Page.Kind == HidUsagePageKind.Generated)
            {
                return;
            }
            else
            {
                for (ushort i = usageIdStart.Id; i <= usageIdEnd.Id; i++)
                {
                    if (usageIdStart.Page.TryFindUsageId(i) == null)
                    {
                        throw new HidSpecificationException(Resources.ExceptionRangeUsageDoesNotExistInRange, usageIdStart.Page, i, usageIdStart, usageIdEnd);
                    }
                }
            }
        }
    }
}
