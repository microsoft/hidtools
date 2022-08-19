// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.ReportDescriptorComposition
{
    using System;

    /// <summary>
    /// Describes an exception that is thrown when an error occurs parsing the Descriptor.
    /// Simple wrapper around <see cref="Exception"/>, that Parser can explicitly capture.
    /// All exceptions explicitly issue by the Composer will be of this type.
    /// </summary>
    public class DescriptorModuleParsingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorModuleParsingException"/> class.
        /// </summary>
        /// <param name="messageFormat">Message.</param>
        /// <param name="messageFormatArgs">Arguments.</param>
        public DescriptorModuleParsingException(string messageFormat, params object[] messageFormatArgs)
            : base(string.Format(messageFormat, messageFormatArgs))
        {
            this.ErrorCode = messageFormat.GetHashCode();

            this.MessageFormat = messageFormat;
            this.MessageFormatArgs = messageFormatArgs;
        }

        /// <summary>
        /// Gets the ErrorCode of this exception.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets the format of the error message.
        /// Kept seperate to permit errorcode calculation based on unique message.
        /// </summary>
        public string MessageFormat { get; }

        /// <summary>
        /// Gets the arguments for the error message.
        /// </summary>
        public object[] MessageFormatArgs { get; }
    }
}
