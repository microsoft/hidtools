// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.HidTools.HidEngine.Properties;
    using Microsoft.HidTools.HidSpecification;

    /// <summary>
    /// Describes the output format of the descriptor.
    /// </summary>
    public enum OutputFormatKind
    {
        /// <summary>
        /// Output as plain-text.
        /// </summary>
        PlainText,

        /// <summary>
        /// Output as (plain) C++ header.
        /// </summary>
        Cpp,

        /// <summary>
        /// Output as (macro) C++ header).
        /// </summary>
        CppMacro,
    }

    /// <summary>
    /// Container for all Global settings.
    /// </summary>
    public class Settings
    {
        private const string DefaultCppDescriptorName = "hidReportDescriptor";
        private const string DefaultCppMacroDescriptorName = "HID_REPORT_DESCRIPTOR";
        private const string FileExtension = ".wara";
        private const string HidUsageTablesFileExtension = ".pdf";

        private static Settings instance = null;

        private int? packingInBytes = null;
        private int[] validPackingValues = { 1, 2, 4 };

        private string sourceFilePath = string.Empty;
        private string destinationFilePath = string.Empty;
        private string hidUsageTablesFilePath = string.Empty;
        private string cppDescriptorName = string.Empty;

        private Settings()
        {
            this.Optimize = true;
            this.OutputFormat = OutputFormatKind.Cpp;
        }

        /// <summary>
        /// Gets or sets the alignment (in bytes) for report item fields.
        /// </summary>
        public int? PackingInBytes
        {
            get
            {
                return this.packingInBytes;
            }

            set
            {
                if (value != null)
                {
                    if (this.validPackingValues.Contains(value.Value))
                    {
                        this.packingInBytes = value;
                        return;
                    }

                    throw new ArgumentException(Resources.ExceptionSettingsPackingBytesMustBeSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the descriptor should be optimized.
        /// Without optimization, the descriptor could be very verbose.
        /// </summary>
        public bool Optimize
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value of the source descriptor file.
        /// </summary>
        public string SourceFilePath
        {
            get
            {
                return this.sourceFilePath;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new SettingsGenericException(Resources.ExceptionSettingsMissingSourceFile);
                }

                string fullPath = Path.GetFullPath(value);

                if (!File.Exists(fullPath))
                {
                    throw new SettingsGenericException(Resources.ExceptionSettingsSourceFileDoesNotExist, fullPath);
                }

                if (!Path.GetExtension(fullPath).Equals(FileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SettingsGenericException(Resources.ExceptionSettingsSourceFileExtensionUnsupported, FileExtension);
                }

                this.sourceFilePath = fullPath;
            }
        }

        /// <summary>
        /// Gets or sets a value of the description file.
        /// </summary>
        public string DestinationFilePath
        {
            get
            {
                string fullPath = this.destinationFilePath;

                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    // Default to name of source file.
                    fullPath = Path.GetFullPath(Path.GetFileNameWithoutExtension(this.SourceFilePath));
                }

                if (this.OutputFormat == OutputFormatKind.Cpp || this.OutputFormat == OutputFormatKind.CppMacro)
                {
                    return System.IO.Path.ChangeExtension(fullPath, "h");
                }
                else
                {
                    return System.IO.Path.ChangeExtension(fullPath, "txt");
                }
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string fullPath = Path.GetFullPath(value);

                    string ext = Path.GetExtension(fullPath);
                    if (!string.IsNullOrEmpty(ext))
                    {
                        throw new SettingsGenericException(Resources.ExceptionSettingsDestinationFileNoExtension);
                    }

                    this.destinationFilePath = fullPath;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value of the Alternative HID Usage Tables file.
        /// </summary>
        public string HidUsageTablesFilePath
        {
            get
            {
                return this.hidUsageTablesFilePath;
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string fullPath = Path.GetFullPath(value);

                    if (!File.Exists(fullPath))
                    {
                        throw new SettingsGenericException(Resources.ExceptionSettingsHidUsageTablesFileDoesNotExist, fullPath);
                    }

                    if (!Path.GetExtension(fullPath).Equals(HidUsageTablesFileExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new SettingsGenericException(Resources.ExceptionSettingsHidUsageTablesFileExtensionUnsupported, HidUsageTablesFileExtension);
                    }

                    this.hidUsageTablesFilePath = fullPath;
                    HidSpecification.HidUsageTableDefinitions.AlternativeHidUsageTablesFilePath = this.hidUsageTablesFilePath;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Name of the generated CPP descriptor byte array.
        /// </summary>
        public string CppDescriptorName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.cppDescriptorName))
                {
                    return this.cppDescriptorName;
                }

                switch (this.OutputFormat)
                {
                    case OutputFormatKind.CppMacro:
                    {
                        return DefaultCppMacroDescriptorName;
                    }

                    default:
                    {
                        return DefaultCppDescriptorName;
                    }
                }
            }

            set
            {
                this.cppDescriptorName = value;
            }
        }

        /// <summary>
        /// Gets or sets the variable modifier of the generated CPP descriptor byte array.
        /// </summary>
        public string CppDescriptorVariableModifier
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the descriptor output format.
        /// </summary>
        public OutputFormatKind OutputFormat
        {
            get; set;
        }

        /// <summary>
        /// Lazy accessor for singleton instance of Settings.  Initializes on first invocation.
        /// </summary>
        /// <param name="clear">When true, clears all existing settings.  Useful for unit-tests.</param>
        /// <returns>Single instance.</returns>
        public static Settings GetInstance(bool clear = false)
        {
            if (instance == null || clear)
            {
                instance = new Settings();
            }

            return instance;
        }

        /// <summary>
        /// Calculates the actual size (in bits), given the current global Packing setting.
        /// This will return either the same or greater value.
        /// </summary>
        /// <param name="unpackedSizeInBits">Initially unpacked size (in bits).</param>
        /// <returns>Packed size (in bits).</returns>
        /// <exception cref="ArgumentOutOfRangeException">Outside range of 1 -> 32.</exception>
        public int CalculateSizeInBitsWithPacking(int unpackedSizeInBits)
        {
            if ((unpackedSizeInBits > HidConstants.SizeInBitsMaximum) || (unpackedSizeInBits < HidConstants.SizeInBitsMinimum))
            {
                throw new ArgumentOutOfRangeException($"Invalid for size {unpackedSizeInBits}.");
            }

            if (this.PackingInBytes.HasValue)
            {
                int packedSizeInBits = unpackedSizeInBits;

                // TODO: While this works, it's not super efficient...
                while (packedSizeInBits % (this.PackingInBytes * 8) != 0)
                {
                    packedSizeInBits++;
                }

                // While 24bits is byte-aligned, it cannot be expressed using CPP types.
                // Round up to 32bits.
                if (packedSizeInBits == 24)
                {
                    packedSizeInBits = 32;
                }

                return packedSizeInBits;
            }
            else
            {
                return unpackedSizeInBits;
            }
        }
    }

    /// <summary>
    /// Describes an exception that occurs during the parsing of the global Settings.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "TrivialClass")]
    public class SettingsGenericException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsGenericException"/> class.
        /// </summary>
        /// <param name="messageFormat">Error message format.  Used a base for generated error code.</param>
        /// <param name="messageFormatArgs">Arguments for message format.</param>
        public SettingsGenericException(string messageFormat, params object[] messageFormatArgs)
            : base(FormatMessage(messageFormat, messageFormatArgs))
        {
            this.ErrorCode = messageFormat.GetHashCode();
        }

        /// <summary>
        /// Gets the ErrorCode of this exception.
        /// </summary>
        public int ErrorCode { get; }

        private static string FormatMessage(string messageFormat, params object[] messageFormatArgs)
        {
            // Unique error-code is a simple has of the error string.
            // Assumed no two error strings will ever be the same.
            int errorCode = messageFormat.GetHashCode();

            string formattedMessage = string.Format(messageFormat, messageFormatArgs);

            string formattedError = string.Format(Resources.ExceptionSettingsRootMessage, errorCode, formattedMessage);

            return formattedError;
        }
    }
}
