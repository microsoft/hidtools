// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;

    /// <summary>
    /// Helper to permit a regular TOML parser to artifically disambiguate multiple ReportDescriptor tags.
    /// TOML does NOT permit multiple section to have the same names (as the underlying structure is a hash-map).
    /// Each TOML section-name will be decorated with a numeric suffix to ensure each section-name token is unique.
    /// </summary>
    public static class TagDecorator
    {
        private static readonly string SectionPrefix = "[[";

        private static readonly string SectionSuffix = "]]";

        private static readonly char SectionSeparator = '.';

        private static readonly char KeyNameSeparator = '=';

        private static readonly List<TagAttribute> CachedTagAttributes = new List<TagAttribute>();

        private static readonly string[] NewlineSeparators = { "\r\n", "\r", "\n" };

        static TagDecorator()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsDefined(typeof(TagAttribute), true))
                    {
                        TagAttribute attribute = type.GetCustomAttribute<TagAttribute>();
                        CachedTagAttributes.Add(attribute);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the non-decorated section name for a tag.
        /// </summary>
        /// <param name="rawTag">Raw TOML tag to un-decorated.</param>
        /// <returns>Non-decorated section name.</returns>
        public static string UnDecorateTag(KeyValuePair<string, object> rawTag)
        {
            return UnDecorateTag(rawTag.Key);
        }

        /// <summary>
        /// Generates the non-decorated section name for a tag.
        /// </summary>
        /// <param name="rawTagName">TOML tag name to un-decorated.</param>
        /// <returns>Non-decorated section name.</returns>
        public static string UnDecorateTag(string rawTagName)
        {
            char[] digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            return rawTagName.TrimEnd(digits);
        }

        /// <summary>
        /// Decorates each TOML tag section-name token with an integer suffix.
        /// </summary>
        /// <param name="nonDecoratedTomlDoc">User supplied TOML document to parse.</param>
        /// <remarks>
        /// Nett Toml parser doesn't allow duplicate section-names, so we will add a monotomically increasing integer to the end of every token,
        /// to keep it happy.  This will also make for easier debugging.
        /// </remarks>
        /// <example>
        /// [[applicationCollection1]]
        /// usage = ['Generic Desktop', 'Mouse']
        ///     [[applicationCollection1.inputReport2]]
        ///         [[applicationCollection1.inputReport2.physicalCollection3]]
        ///         usage = ['Generic Desktop', 'Pointer']
        ///         ...
        ///         [[applicationCollection1.inputReport2.physicalCollection3.variableItem4]]
        ///         usage = ['Generic Desktop', 'X']
        ///         ...
        ///         [[applicationCollection1.inputReport2.physicalCollection3.variableItem5]]
        ///         usage = ['Generic Desktop', 'Y']
        ///         ...
        ///         [[applicationCollection1.inputReport2.physicalCollection3.paddingItem7]]
        ///         sizeInBits = 5.
        /// </example>
        /// <returns>TOML document with each section-name suffixed.  Formatting is preserved.</returns>
        public static string Decorate(string nonDecoratedTomlDoc)
        {
            string decoratedTomlDoc = DecorateSectionNames(nonDecoratedTomlDoc);

            decoratedTomlDoc = DecorateKeyNames(decoratedTomlDoc);

            return decoratedTomlDoc;
        }

        private static string DecorateSectionNames(string nonDecoratedTomlDoc)
        {
            DecoratedSectionToken.Reset();

            // Temp buffer to hold all section tokens that have previously been discovered (and decorated),
            // as the recursive, nested sections are explored.
            // As the sections terminate, tokens are removed, until a new undiscovered is encountered.
            List<DecoratedSectionToken> tokensInPreviousSection = new List<DecoratedSectionToken>();

            List<(string DecoratedName, string NonDecoratedName)> sectionNames = new List<(string, string)>();

            List<string> nonDecoratedSectionNames = GenerateCleanSectionNames(nonDecoratedTomlDoc);

            // Used only by exceptions to find the first occurrence of a section name.
            // Cannot use the Global TagFinder singleton, as it hasn't yet been initialized with the data
            // from this class.
            // It's OK to use the non-decorated section names here, as only the instance is cared about.
            TagFinder finder = new TagFinder(nonDecoratedTomlDoc);

            foreach (string nonDecoratedSectionName in nonDecoratedSectionNames)
            {
                List<string> sectionNameTokens = nonDecoratedSectionName.Split(SectionSeparator).ToList();

                // Ensure there aren't any unknown/invalid tokens in the section name.
                ValidateSectionNameTokens(nonDecoratedSectionName, sectionNameTokens, finder);

                // Determine how many tokens previously discovered, (sequentially) match this section name.
                // As sections are recursively explored, this will increase.  As they terminate (and sections higher
                // in the hierarchy need to be explored), this will decrease.
                int tokenMatchCount;
                for (tokenMatchCount = 0; tokenMatchCount < Math.Min(tokensInPreviousSection.Count, sectionNameTokens.Count); tokenMatchCount++)
                {
                    // Stop when a 'new' token is encountered at a sequentially earlier position than expected.
                    // e.g. previous tokens == {token1, token2, token3}. current section == {token1, token4, token5}.
                    // match Count == 1
                    if (tokensInPreviousSection[tokenMatchCount].Text != sectionNameTokens[tokenMatchCount])
                    {
                        break;
                    }
                }

                if (tokenMatchCount == tokensInPreviousSection.Count)
                {
                    if (sectionNameTokens.Count > tokensInPreviousSection.Count)
                    {
                        // Previous section name is a perfect prefix to this section.
                        // e.g. previous={token1, token2, token3}, current={token1, token2, token3, |token4|}

                        tokensInPreviousSection.AddRange(DecoratedSectionToken.GenerateTokens(sectionNameTokens.Skip(tokensInPreviousSection.Count)));
                    }
                    else
                    {
                        // Previous section name is the same as the current section name.
                        // e.g. previous={token1, token2, token3}, current={token1, token2, token3}

                        // Remove last previously cached decorated token.
                        tokensInPreviousSection.RemoveRange(Math.Abs(tokensInPreviousSection.Count - 1), 1);

                        // Add new decorated token (which will have a new suffix) for the same position.
                        tokensInPreviousSection.AddRange(DecoratedSectionToken.GenerateTokens(sectionNameTokens.Skip(tokensInPreviousSection.Count)));
                    }
                }
                else
                {
                    // Previous section name is not a complete prefix for the current section name (or not a prefix at all).
                    // e.g. previous={token1, |token2, token3, token4|}, current={token1, |token5, token6, token7|}
                    // e.g. previous={|token1, token2, token3|}, current={|token4, token5, token6|}

                    // In the case where the name is shorter that the previous line and the last token name are the same,
                    // then decrease the match count by 1, so that the previous token with the same name will be removed as well (i.e. remove token2 and token3)
                    // e.g. previous={|token1, token2|, token3}, current={|token1, token2|}
                    if (sectionNameTokens.Count == tokenMatchCount && sectionNameTokens.Count < tokensInPreviousSection.Count)
                    {
                        // TODO: Find a better way to do this; this seems overly-specific for a non-edgecase.
                        tokenMatchCount--;
                    }

                    tokensInPreviousSection.RemoveRange(tokenMatchCount, Math.Abs(tokenMatchCount - tokensInPreviousSection.Count));

                    tokensInPreviousSection.AddRange(DecoratedSectionToken.GenerateTokens(sectionNameTokens.Skip(tokenMatchCount)));
                }

                // Now tokensInPreviousSection contains all the tokens in the current section.

                // Add monotonically increasing numeric suffix to the end of each token.
                // Tokens that were common to the previous and current section names will have the same numeric suffix.
                string decoratedStringBuilder = tokensInPreviousSection[0].Text + tokensInPreviousSection[0].Suffix;
                for (int i = 1; i < tokensInPreviousSection.Count; i++)
                {
                    decoratedStringBuilder += $"{SectionSeparator}{tokensInPreviousSection[i].Text}{tokensInPreviousSection[i].Suffix}";
                }

                string decoratedSectionName = $"{SectionPrefix}{decoratedStringBuilder}{SectionSuffix}";

                sectionNames.Add((decoratedSectionName, nonDecoratedSectionName));
            }

            // All section names have been decorated.
            // Go through the TOML doc and replace the non-decorated with the decorated section names.

            string decoratedTomlDoc = nonDecoratedTomlDoc;
            foreach (var sectionName in sectionNames)
            {
                string nonDecoratedSectionNameWithSuffixPrefix = $"{SectionPrefix}{sectionName.NonDecoratedName}{SectionSuffix}";

                // Replace only the first occurence of the non-decorated section name with the decorated version.
                // This allows step-by-step replacement.
                decoratedTomlDoc = decoratedTomlDoc.Replace(nonDecoratedSectionNameWithSuffixPrefix, sectionName.DecoratedName, true);
            }

            return decoratedTomlDoc;
        }

        private static string DecorateKeyNames(string tomlDoc)
        {
            string decoratedTomlDoc = string.Empty;

            // Monotonically increasing counter that is appended to the end
            // of every key-name.
            int suffixCounter = 1;
            foreach (string line in tomlDoc.Split(NewlineSeparators, StringSplitOptions.None))
            {
                // This line contains a key-value pair.
                if (line.Contains(KeyNameSeparator))
                {
                    // Delete all characters after the separator, including the separator.
                    string keyName = line.Substring(0, line.IndexOf(KeyNameSeparator)).Trim();

                    string newKeyName = keyName + suffixCounter;

                    string decoratedLine = line.Replace(keyName, newKeyName, true);

                    decoratedTomlDoc += (decoratedLine + System.Environment.NewLine);

                    suffixCounter++;
                }
                else
                {
                    decoratedTomlDoc += (line + System.Environment.NewLine);
                }
            }

            // Ensure that at least something is always returned.
            if (decoratedTomlDoc == string.Empty)
            {
                decoratedTomlDoc = tomlDoc;
            }

            return decoratedTomlDoc;
        }

        /// <summary>
        /// Create a clean, parsable list of section names without suffixes/prefixes
        /// (e.g. "applicationCollection.inputReport").
        /// These will be used later to replace strings in main document.
        /// </summary>
        private static List<string> GenerateCleanSectionNames(string tomlDoc)
        {
            List<string> rawSectionNames = new List<string>();
            foreach (string line in tomlDoc.Split(NewlineSeparators, StringSplitOptions.None))
            {
                // Ignore commented-out lines (starts with #)
                if (line.Contains(SectionPrefix) && line.Contains(SectionSuffix) && !line.TrimStart().StartsWith("#"))
                {
                    string sectionNameWithPrefix = line.Trim().Replace(SectionPrefix, string.Empty);
                    string sectionNameWithPrefixOrSuffix = sectionNameWithPrefix.Replace(SectionSuffix, string.Empty);

                    rawSectionNames.Add(sectionNameWithPrefixOrSuffix);
                }
            }

            return rawSectionNames;
        }

        /// <summary>
        /// Validates section name tokens against all valid section name tokens.
        /// </summary>
        private static void ValidateSectionNameTokens(string nonDecoratedSectionName, List<string> sectionNameTokens, TagFinder finder)
        {
            foreach (string token in sectionNameTokens)
            {
                bool matchedToken = false;

                // Determine if this token matches any valid/known section name tokens.
                foreach (TagAttribute attribute in CachedTagAttributes)
                {
                    if ((attribute.Kind == TagKind.Section || attribute.Kind == TagKind.RootSection) && token.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedToken = true;
                    }
                }

                if (!matchedToken)
                {
                    // The 'finder' will only find the first occurrence, so ValidateSectionNameTokens
                    // must be called on each sectionName in order of appearance
                    throw new TomlGenericException(Resources.ExceptionTomlInvalidSectionNameToken, token, finder.FindLine(token));
                }
            }
        }

        /// <summary>
        /// Helper class that pairs tokens and numeric suffixes.
        /// </summary>
        private class DecoratedSectionToken
        {
            // Monotomically increasing counter, used to uniquely
            // this token, by a suffix.
            private static int tokenSuffixCounter = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="DecoratedSectionToken"/> class.
            /// </summary>
            /// <param name="tokenText">Text of the token.</param>
            public DecoratedSectionToken(string tokenText)
            {
                this.Text = tokenText;
                this.SetSuffix();
            }

            public string Text { get; }

            public int Suffix { get; private set; }

            public static List<DecoratedSectionToken> GenerateTokens(IEnumerable<string> names)
            {
                List<DecoratedSectionToken> tokens = new List<DecoratedSectionToken>();

                foreach (string name in names)
                {
                    tokens.Add(new DecoratedSectionToken(name));
                }

                return tokens;
            }

            /// <summary>
            /// A global static is used across all tokens for a specific TOML document
            /// For each new TOML doc, the counter must be reset.
            /// </summary>
            public static void Reset()
            {
                tokenSuffixCounter = 1;
            }

            public override string ToString()
            {
                return this.Text + this.Suffix;
            }

            private void SetSuffix()
            {
                this.Suffix = tokenSuffixCounter++;
            }
        }
    }
}
