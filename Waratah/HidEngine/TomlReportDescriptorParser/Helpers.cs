// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.TomlReportDescriptorParser
{
    using System;
    using System.Collections.Generic;
    using Microsoft.HidTools.HidEngine.Properties;

    /// <summary>
    /// Generic container for Helper methods.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Safe casts an Int64 value to an Int32.
        /// If out of bounds, an exception is thrown.
        /// </summary>
        /// <param name="value">Value to convert to Int32.</param>
        /// <param name="rawTag">The tag the value belongs to.</param>
        /// <returns>Converted Int32.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are out of bounds.</exception>
        public static Int32 SafeGetInt32(Int64 value, KeyValuePair<string, object> rawTag)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (OverflowException e)
            {
                throw new TomlGenericException(Resources.ExceptionTomlValueOutOfBounds, rawTag, value, e.Message);
            }
        }

        /// <summary>
        /// Safe casts an Int64 value to an UInt16.
        /// If out of bounds, an exception is thrown.
        /// </summary>
        /// <param name="value">Value to convert to UInt16.</param>
        /// <param name="rawTag">The tag the value belongs to.</param>
        /// <returns>Converted UInt16.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are out of bounds.</exception>
        public static UInt16 SafeGetUInt16(Int64 value, KeyValuePair<string, object> rawTag)
        {
            try
            {
                return Convert.ToUInt16(value);
            }
            catch (OverflowException e)
            {
                throw new TomlGenericException(Resources.ExceptionTomlValueOutOfBounds, rawTag, value, e.Message);
            }
        }

        /// <summary>
        /// Safe casts an Int64 value to a UInt8/byte.
        /// If out of bounds, an exception is thrown.
        /// </summary>
        /// <param name="value">Value to convert to UInt8/byte.</param>
        /// <param name="rawTag">The tag the value belongs to.</param>
        /// <returns>Converted UInt8/byte.</returns>
        /// <exception cref="TomlGenericException">Thrown when supplied values are out of bounds.</exception>
        public static byte SafeGetByte(Int64 value, KeyValuePair<string, object> rawTag)
        {
            try
            {
                return Convert.ToByte(value);
            }
            catch (OverflowException e)
            {
                throw new TomlGenericException(Resources.ExceptionTomlValueOutOfBounds, rawTag, value, e.Message);
            }
        }
    }
}
