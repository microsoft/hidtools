// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser
{
    using System.Collections.Generic;
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidEngine.TomlReportDescriptorParser.Tags;
    using Nett;

    /// <summary>
    /// Parsers for HID TOML documents.
    /// </summary>
    public static class TomlDocumentParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TomlDocumentParser"/> class.
        /// Parses a well-formatted HID TOML text document into a hierarchy of HID tags.
        /// </summary>
        /// <param name="nonDecoratedTomlDoc">User supplied TOML document to parse.</param>
        /// <returns>New <see cref="Descriptor"/>.  Exceptions will be thrown is cannot be parsed.</returns>
        public static Descriptor TryParseReportDescriptor(string nonDecoratedTomlDoc)
        {
            // Validate the TOML document first (by calling into the TOML library), before trying to decorate.
            // This will detect any obvious TOML issues upfront.
            // If the document is invalid an exception will be thrown.
            // Note: The parsed tags can't be used for later processing as they aren't decorated.
            Toml.ReadString(nonDecoratedTomlDoc);

            // Decorate ReportDescriptor tags in the TOML to disambiguate section names for the TOML parser.
            string decoratedTomlDoc = TagDecorator.Decorate(nonDecoratedTomlDoc);

            // Used to locate a specific decorated tag (and it's line) for use in debugging/blaming.
            TagFinder.Initialize(decoratedTomlDoc);

            List<ApplicationCollectionModule> applicationCollections = new List<ApplicationCollectionModule>();

            Dictionary<string, object> rawTomlTags = Toml.ReadString(decoratedTomlDoc).ToDictionary();
            foreach (KeyValuePair<string, object> rawTomlTag in rawTomlTags)
            {
                // New Settings will be added to the Global settings from TryParse.
                if (SettingsSectionTag.TryParse(rawTomlTag) != null)
                {
                    continue;
                }

                // New Units will be added to the Global Units from TryParse.
                if (UnitSectionTag.TryParse(rawTomlTag) != null)
                {
                    continue;
                }

                // New Usages will be added to the Global HUT from TryParse.
                if (UsagePageSectionTag.TryParse(rawTomlTag) != null)
                {
                    continue;
                }

                ApplicationCollectionTag tag = ApplicationCollectionTag.TryParse(rawTomlTag);
                if (tag != null)
                {
                    applicationCollections.Add((ApplicationCollectionModule)tag.GenerateDescriptorModule(null));
                    continue;
                }

                throw new TomlInvalidLocationException(rawTomlTag);
            }

            try
            {
                return new Descriptor(applicationCollections);
            }
            catch (DescriptorModuleParsingException e)
            {
                throw new TomlGenericException(e);
            }
        }
    }
}
