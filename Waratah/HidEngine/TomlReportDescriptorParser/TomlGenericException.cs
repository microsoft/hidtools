// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidEngine.ReportDescriptorComposition;
    using Microsoft.HidTools.HidEngine.TomlReportDescriptorParser.Tags;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes an exception that occurs during the parsing of the TOML report descriptor.
    /// </summary>
    public class TomlGenericException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="messageFormat">Error message format.  Used a base for generated error code.</param>
        /// <param name="rawTag">Raw TOML tag.  Symbol and line number extracted from here.</param>
        /// <param name="messageFormatArgs">Arguments for message format.</param>
        public TomlGenericException(string messageFormat, KeyValuePair<string, object> rawTag, params object[] messageFormatArgs)
            : base(FormatMessage(messageFormat, rawTag, messageFormatArgs))
        {
            this.ErrorCode = messageFormat.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="messageFormat">Error message format.  Used a base for generated error code.</param>
        /// <param name="keyName">Symbol of associated tag.</param>
        /// <param name="tagLine">Line symbol occurs on.</param>
        /// <param name="messageFormatArgs">Arguments for message format.</param>
        public TomlGenericException(string messageFormat, string keyName, int tagLine, params object[] messageFormatArgs)
            : base(FormatMessage(messageFormat, keyName, tagLine, messageFormatArgs))
        {
            this.ErrorCode = messageFormat.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="e">Well-formatted exception.</param>
        /// <param name="tag">Emitting tag.</param>
        public TomlGenericException(DescriptorModuleParsingException e, BaseTag tag)
            : this(e.MessageFormat, tag.RawTag, e.MessageFormatArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="e">Well-formatted exception.</param>
        /// <param name="rawTag">Raw TOML tag.  Symbol and line number extracted from here.</param>
        public TomlGenericException(DescriptorModuleParsingException e, KeyValuePair<string, object> rawTag)
            : this(e.MessageFormat, rawTag, e.MessageFormatArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="e">Well-formatted exception.</param>
        public TomlGenericException(DescriptorModuleParsingException e)
            : base(FormatMessage(e.MessageFormat, e.MessageFormatArgs))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="e">Well-formatted exception.</param>
        /// <param name="rawTag">TOML-tag associated with exception.</param>
        public TomlGenericException(HidSpecificationException e, KeyValuePair<string, object> rawTag)
            : this(e.MessageFormat, rawTag, e.MessageFormatArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlGenericException"/> class.
        /// </summary>
        /// <param name="e">Well-formatted exception.</param>
        /// <param name="tag">Emitting tag.</param>
        public TomlGenericException(HidSpecificationException e, BaseTag tag)
            : this(e.MessageFormat, tag.RawTag, e.MessageFormatArgs)
        {
        }

        /// <summary>
        /// Gets the ErrorCode of this exception.
        /// </summary>
        public int ErrorCode { get; }

        private static string FormatMessage(string messageFormat, KeyValuePair<string, object> closestTag, params object[] messageFormatArgs)
        {
            // Unique error-code is a simple has of the error string.
            // Assumed no two error strings will ever be the same.
            int errorCode = messageFormat.GetHashCode();

            string keyName = TagDecorator.UnDecorateTag(closestTag);
            int tagLine = TagFinder.GetInstance().FindLine(closestTag);

            string formattedMessage = string.Format(messageFormat, messageFormatArgs);

            // Removing all periods '.' at end, in cause multiple have been added by intermediate format strings.
            string formattedError = string.Format(Resources.ExceptionRootMessage, keyName, tagLine, errorCode, formattedMessage).TrimEnd('.') + ".";

            return formattedError;
        }

        private static string FormatMessage(string messageFormat, string keyName, int tagLine, params object[] messageFormatArgs)
        {
            // Unique error-code is a simple has of the error string.
            // Assumed no two error strings will ever be the same.
            int errorCode = messageFormat.GetHashCode();

            string formattedMessage = string.Format(messageFormat, messageFormatArgs);

            // Removing all periods '.' at end, in cause multiple have been added by intermediate format strings.
            string formattedError = string.Format(Resources.ExceptionRootMessage, keyName, tagLine, errorCode, formattedMessage).TrimEnd('.') + ".";

            return formattedError;
        }

        private static string FormatMessage(string messageFormat, params object[] messageFormatArgs)
        {
            // Unique error-code is a simple has of the error string.
            // Assumed no two error strings will ever be the same.
            int errorCode = messageFormat.GetHashCode();

            string formattedMessage = string.Format(messageFormat, messageFormatArgs);

            // Removing all periods '.' at end, in cause multiple have been added by intermediate format strings.
            string formattedError = string.Format(Resources.ExceptionRootMessageNoTag, errorCode, formattedMessage).TrimEnd('.') + ".";

            return formattedError;
        }
    }

    /// <summary>
    /// Describes an exception that is thrown when a Tag is detected in an invalid location.
    /// </summary>
    /// <remarks>e.g. An InputReport cannot define SizeInBits tag.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "TrivialClass")]
    public class TomlInvalidLocationException : TomlGenericException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TomlInvalidLocationException"/> class.
        /// </summary>
        /// <param name="invalidTagAtLocation">Tag that is invalid at location.</param>
        public TomlInvalidLocationException(KeyValuePair<string, object> invalidTagAtLocation)
            : base(Resources.ExceptionTomlInvalidRootTagPlacement, invalidTagAtLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TomlInvalidLocationException"/> class.
        /// </summary>
        /// <param name="invalidTagAtLocation">Tag that is invalid at location.</param>
        /// <param name="parentTag">Parent of Tag that is invalid at location.</param>
        public TomlInvalidLocationException(KeyValuePair<string, object> invalidTagAtLocation, KeyValuePair<string, object> parentTag)
            : base(Resources.ExceptionTomlInvalidTagPlacement, invalidTagAtLocation, TagDecorator.UnDecorateTag(parentTag), TagFinder.GetInstance().FindLine(parentTag).ToString())
        {
        }
    }
}
