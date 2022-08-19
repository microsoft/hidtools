// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidSpecification
{
    using System;

    /// <summary>
    /// Describes an exception that is thrown when an error occurs within HidSpecification.
    /// Simple wrapper around <see cref="Exception"/>, that Parser can explicitly capture.
    /// All exceptions explicitly issue by the HidSpecification classes will be of this type.
    /// </summary>
    public class HidSpecificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HidSpecificationException"/> class.
        /// </summary>
        /// <param name="messageFormat">Message.</param>
        /// <param name="messageFormatArgs">Arguments.</param>
        public HidSpecificationException(string messageFormat, params object[] messageFormatArgs)
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
