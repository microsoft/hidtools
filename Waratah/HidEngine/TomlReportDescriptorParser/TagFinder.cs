// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Helper, to locate a TOML tag within TOML Document.
    /// </summary>
    public class TagFinder
    {
        private static TagFinder instance;

        private string decoratedTomlDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagFinder"/> class.
        /// </summary>
        /// <param name="decoratedTomlDocument">A decorated TOML document.</param>
        public TagFinder(string decoratedTomlDocument)
        {
            this.decoratedTomlDocument = decoratedTomlDocument;
        }

        /// <summary>
        /// Initializes the Global TagFinder.
        /// </summary>
        /// <param name="decoratedTomlDocument">A decorated TOML document.</param>
        public static void Initialize(string decoratedTomlDocument)
        {
            instance = new TagFinder(decoratedTomlDocument);
        }

        /// <summary>
        /// Global accessor for singleton instance of TagFinder.  MUST have previously called <see cref="Initialize(string)"/>.
        /// </summary>
        /// <returns>Single instance.</returns>
        public static TagFinder GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Finds the line (within the TOML doc) the tag appears on.
        /// This relies on the uniqueness of the tag decoration.
        /// </summary>
        /// <param name="tag">Tag to search for within the TOML doc.</param>
        /// <returns>Line which tag appears on.  0 if it doesn't appear on any line.</returns>
        public int FindLine(KeyValuePair<string, object> tag)
        {
            return this.FindLine(tag.Key);
        }

        /// <summary>
        /// Finds the line (within the TOML doc) the keyName appears on.
        /// This relies on the uniqueness of the tag decoration.
        /// </summary>
        /// <param name="keyName">Keyname to search for within the TOML doc.</param>
        /// <returns>Line which keyName appears on.  0 if it doesn't appear on any line.</returns>
        public int FindLine(string keyName)
        {
            using (StringReader reader = new StringReader(this.decoratedTomlDocument))
            {
                int currentLineNumber = 1;
                string currentLine = string.Empty;

                // Finds the first occurrence of the keyName within the document.
                // Due to the way TOML docs are structure, this first occurrence is
                // the most interesting.
                while ((currentLine = reader.ReadLine()) != null)
                {
                    if (currentLine.IndexOf(keyName) != -1)
                    {
                        return currentLineNumber;
                    }

                    currentLineNumber++;
                }

                return 0;
            }
        }
    }
}
