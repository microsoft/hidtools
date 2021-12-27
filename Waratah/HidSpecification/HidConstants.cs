// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidSpecification
{
    using System;
    using System.Globalization;
    using HidSpecification.Properties;

    /// <summary>
    /// All HID constants (manually) extracted from the HID 1.11 specification.
    /// Unlike the HID Usage Tables, HID 1.11 is not in a parseable format.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row", Justification = "Better formatting in this file.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self explanatory enums from names.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Most constants are self explanatory.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "OK for constants file")]
    public static class HidConstants
    {
        // hid_11:6.2.2.7 - "Report ID zero is reserved and should not be used."
        // ReportId is permitted to be absent, but can never be zero.
        // For simplicity of implementation, an absent ReportId is NEVER permitted.
        public const Int32 ReportIdMinimum = 1;

        // hid_11:6.2.2.7 - "All data reports for the device are preceded by a single byte ID field" (so max == 2^8-1 == 255)
        public const Int32 ReportIdMaximum = 255;

        public const UInt16 UsageIdMinimum = 1;
        public const UInt16 UsageIdMaximum = UInt16.MaxValue;

        public const UInt16 UsagePageIdMinimum = 1;
        public const UInt16 UsagePageIdMaximum = UInt16.MaxValue;

        // From the hid_11 spec, it looks like a ReportCount size of 4bytes is allowed (2^(4*8) - 1)
        // However, the Microsoft HID APIs can only express the UINT16 range (see HIDP_VALUE_CAPS.ReportCount)
        // so we will align to that restriction.
        // <seealso cref="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/hidpi/ns-hidpi-_hidp_value_caps"/>
        public const Int32 ReportCountMaximum = UInt16.MaxValue;
        public const Int32 ReportCountMinimum = 1;

        public const Int32 PhysicalMinimumUndefinedValue = 0;
        public const Int32 PhysicalMaximumUndefinedValue = 0;

        // From the hid_11 spec, HID supports both [0, <see cref="UInt32.MaxValue"/>] and [<see cref="Int32.MinValue"/>, <see cref="Int32.MaxValue"/>] ranges
        // However, the Microsoft HID APIs can only express the INT32 range (see HIDP_VALUE_CAPS.LogicalMin)
        // so we will align to that restriction.
        // <seealso cref="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/hidpi/ns-hidpi-_hidp_value_caps"/>
        public const Int32 LogicalMinimumValue = Int32.MinValue;
        public const Int32 LogicalMaximumValue = Int32.MaxValue;

        public const Int32 PhysicalMinimumValue = Int32.MinValue;
        public const Int32 PhysicalMaximumValue = Int32.MaxValue;

        // From hid_11 spec (pg 32), "An Array field returns a 0 value when no controls in the array are asserted.
        // The Logical Minimum equals 1"
        public const Int32 LogicalMinimumValueArrayItem = 1;

        // No point in having zero sized item.
        public const Int32 SizeInBitsMinimum = 1;

        // Since LogicalMin/Max are signed 32bit integers, the largest item can only be 32bits (including the sign-bit).
        public const Int32 SizeInBitsMaximum = 32;

        // hid_11:8.4 - "An item field cannot span more than 4 bytes in a report."
        public const Int32 ItemByteSpanningMaximum = 4;

        // hid_11:5.10 - "It is highly recommended that 0 be included in the set of Null values"
        public const Int32 DefaultNullValue = 0;

        // USB descriptor parameter (wLength) is defined as a UINT16, so that is the max that can be expressed.
        // TODO: Validate Windows can parse a descriptor this size.
        public const Int32 DescriptorSizeInBytesMaximum = UInt16.MaxValue;

        // HID descriptors can only ever be part of the Interface Descriptor.
        public const Int32 UsbHidInterfaceClassType = 0x03;
        public const Int32 UsbHidInterfaceBootSubClassType = 0x01;

        public const Int32 UsbHidInterfaceProtocolKeyboard = 0x01;
        public const Int32 UsbHidInterfaceProtocolMouse = 0x02;

        // hid_11:6.2.1 - "HID Descriptor"
        public const Int32 UsbHidDescriptorType = 0x21;

        public const Int32 UsbHidReportDescriptorType = 0x22;
        public const Int32 UsbHidPhysicalDescriptorType = 0x23;

        public const Int32 VendorCollectionKindStart = 0x80;
        public const Int32 VendorCollectionKindEnd = 0xFF;

        // hid_11:7.2 - "Class Specific Requests".  wLength is 2 bytes.
        // Additionally, Microsoft HID APIs only support a USHORT as well.
        // <seealso cref="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/hidpi/ns-hidpi-_hidp_caps"/>
        // TODO: Investigate whether other HID transports (I2C/SPI/BT) have their own max report size.
        public const Int32 ReportSizeInBytesMaximum = UInt16.MaxValue;

        // USB 2.0 will fragment Reports into 64byte chunks at the wire.
        // Reports within this size then, are guaranteed to be delivered in 1 transfer, rather than multiple.
        // This might be interesting for low-latency concerns.
        public const Int32 UsbReportSizeBeforeFragmentationInBytesMaximum = 64;

        public const Int32 ExponentMaximum = 7;
        public const Int32 ExponentMinimum = -8;
        public const Int32 UnitExponentUndefinedValue = 0;

        // While the wire-exponent is 0, the multiplier is 1, (10^0 = 1).
        public const Int32 UnitExponentUndefinedValueMultiplier = 1;

        /// <summary>
        /// The type of Short item.
        /// These values correspond to those defined for bType in hut_11:6.2.2.2.
        /// </summary>
        public enum ShortItemTypeKind
        {
            Main    = 0b00,
            Global  = 0b01,
            Local   = 0b10,
        }

        /// <summary>
        /// Kind of Global item.
        /// </summary>
        public enum GlobalItemKind
        {
            UsagePage       = 0b0000,
            LogicalMinimum  = 0b0001,
            LogicalMaximum  = 0b0010,
            PhysicalMinimum = 0b0011,
            PhysicalMaximum = 0b0100,
            UnitExponent    = 0b0101,
            Unit            = 0b0110,
            ReportSize      = 0b0111,
            ReportId        = 0b1000,
            ReportCount     = 0b1001,
            Push            = 0b1010,
            Pop             = 0b1011,
        }

        /// <summary>
        /// Kind of Local item.
        /// </summary>
        public enum LocalItemKind
        {
            Usage               = 0b0000,
            UsageMinimum        = 0b0001,
            UsageMaximum        = 0b0010,
            DesignatorIndex     = 0b0011,
            DesignatorMinimum   = 0b0100,
            DesignatorMaximum   = 0b0101,
            StringIndex         = 0b0111,
            StringMinimum       = 0b1000,
            StringMaximum       = 0b1001,
            Delimiter           = 0b1010,
        }

        /// <summary>
        /// Kind of Main item.
        /// </summary>
        public enum MainItemKind
        {
            Input           = 0b1000,
            Output          = 0b1001,
            Feature         = 0b1011,
            Collection      = 0b1010,
            EndCollection   = 0b1100,
        }

        [System.AttributeUsage(System.AttributeTargets.Enum)]
        public class MainDataItemAttribute : System.Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MainDataItemAttribute"/> class.
            /// </summary>
            /// <param name="byteNumber">Primary byte of this item.</param>
            /// <param name="bitPosition">Position within the primary byte.</param>
            public MainDataItemAttribute(int byteNumber, int bitPosition)
            {
                this.ByteNumber = byteNumber;
                this.BitPosition = bitPosition;
            }

            /// <summary>
            /// Gets the byte position of this MainData Item.
            /// </summary>
            public int ByteNumber { get; }

            /// <summary>
            /// Gets the bit position within the byte of this MainData Item.
            /// </summary>
            public int BitPosition { get; }
        }

        /// <summary>
        /// Kind of control modification indicating whether the item is data or constant.
        /// </summary>
        [MainDataItem(0, 0)]
        public enum MainDataItemModificationKind
        {
            /// <summary>
            /// Indicates the item is defining report fields that contain modifiable device data.
            /// </summary>
            Data = 0,

            /// <summary>
            /// Indicates the item is a static read-only/constant field in a report and cannot be modified (written) by the host.
            /// </summary>
            Constant = 1,
        }

        /// <summary>
        /// Kind of control grouping.
        /// </summary>
        [MainDataItem(0, 1)]
        public enum MainDataItemGroupingKind
        {
            Array = 0,
            Variable = 1,
        }

        /// <summary>
        /// Kind of control relation.
        /// </summary>
        [MainDataItem(0, 2)]
        public enum MainDataItemRelationKind
        {
            /// <summary>
            /// Indicates value of item is based on a fixed origin (e.g. touch-pad data).
            /// </summary>
            Absolute = 0,

            /// <summary>
            /// Indicates value of item is the change in value from the last report (e.g. mouse data).
            /// </summary>
            Relative = 1,
        }

        /// <summary>
        /// Kind of control wrapping indicating whether data rolls-over when reaching either extreme high/low.
        /// </summary>
        [MainDataItem(0, 3)]
        public enum MainDataItemWrappingKind
        {
            /// <summary>
            /// Indicates the value of the item does <em>not</em> wrap.
            /// </summary>
            NoWrap = 0,

            /// <summary>
            /// Indicates the value of the item wraps. (e.g. dial that can spin freely 360 degrees will output only values 0-10.
            /// If Wrap is indicated, the next value reported after passing the 10 position in the increasing direction would be 0.)
            /// </summary>
            Wrap = 1,
        }

        /// <summary>
        /// Kind of control linearity indicating whether the data represents a linear relationship with the raw-data.
        /// </summary>
        [MainDataItem(0, 4)]
        public enum MainDataItemLinearityKind
        {
            /// <summary>
            /// Indicates the data has not been processed to represent something other than a linear relationship to what was captured.
            /// </summary>
            Linear = 0,

            /// <summary>
            /// Indicates the data has been processed on the device and no longer represents a linear relationship between what was captured
            /// and the data reported. (e.g. acceleration curves and joysticks dead zones).
            /// </summary>
            NonLinear = 1,
        }

        /// <summary>
        /// Kind of control preference state indicating whether the control underlying the item has a preferred state to which it will return when
        /// the user is not physically interacting with the control.
        /// </summary>
        [MainDataItem(0, 5)]
        public enum MainDataItemPreferenceStateKind
        {
            /// <summary>
            /// Indicates the underyling control has a preferred state (e.g. push-buttons, self-centering joysticks).
            /// </summary>
            PreferredState = 0,

            /// <summary>
            /// Indicates the underlying control does <em>not</em> have a preferred state (e.g. toggle-button).
            /// </summary>
            NoPreferredState = 1,
        }

        /// <summary>
        /// Kind of control meaningful data indicating whether the control has a state in which it is not sending meaningful data.
        /// </summary>
        [MainDataItem(0, 6)]
        public enum MainDataItemMeaningfulDataKind
        {
            /// <summary>
            /// Indicates any value of this field is meaningful.
            /// </summary>
            NoNullPosition = 0,

            /// <summary>
            /// Indicates any value outside of the LogicalMinimum/LogicalMaximum is a logical null-value.
            /// e.g. a hat-switch/D-Pad, where if the hat switch is not being pressed, it is in a null state.
            /// </summary>
            NullState = 1,
        }

        /// <summary>
        /// Kind of control volatility indicating whether the value can/should be changed by the host.
        /// Note: This is <emp>NOT</emp> valid for an InputReport.
        /// </summary>
        [MainDataItem(0, 7)]
        public enum MainDataItemVolatilityKind
        {
            /// <summary>
            /// Indicates the value shouldn't/cannot be changed by the host.
            /// </summary>
            NonVolatile = 0,

            /// <summary>
            /// Indicates the value can change with or without host interaction.
            /// </summary>
            Volatile = 1,
        }

        /// <summary>
        /// Kind of control contingent indicating whether the field should be interpreted as a numeric value, or opaque bytes (whose true meaning is
        /// understood only by the calling application).
        /// </summary>
        [MainDataItem(1, 0)]
        public enum MainDataItemContingentKind
        {
            /// <summary>
            /// Indicates the value should be interpreted as a numeric value.
            /// </summary>
            BitField = 0,

            /// <summary>
            /// Indicates the value should be interpreted as a series of opaque bytes.
            /// </summary>
            BufferedBytes = 1,
        }

        /// <summary>
        /// Kind of Collection.
        /// </summary>
        public enum MainItemCollectionKind
        {
            /// <summary>
            /// Groups discrete items that are associated with a specific geometric point. (e.g. X and Y axis on a mouse).
            /// </summary>
            Physical        = 0x00,
            Application     = 0x01,

            /// <summary>
            /// Groups discrete items to form a composite data-structure. (e.g. a data-buffer and a byte count of the data).
            /// </summary>
            Logical         = 0x02,
            Report          = 0x03,
            NamedArray      = 0x04,
            UsageSwitch     = 0x05,
            UsageModifier   = 0x06,
            VendorDefined   = 0xFF,
        }

        /// <summary>
        /// Kind of units system.
        /// </summary>
        public enum UnitItemSystemKind
        {
            None = 0x00,
            SiLinear = 0x01,
            SiRotation = 0x02,
            EnglishLinear = 0x03,
            EnglishRotation = 0x04,

            // 0x05 - 0xE are reserved.
            Vendor = 0x0F,
        }

        /// <summary>
        /// Kind of unit length.
        /// </summary>
        public enum UnitLengthKind
        {
            None,
            Centimeter,
            Radians,
            Inch,
            Degrees,
        }

        /// <summary>
        /// Kind of unit mass.
        /// </summary>
        public enum UnitMassKind
        {
            None,
            Gram,
            Slug,
        }

        /// <summary>
        /// Kind of unit time.
        /// </summary>
        public enum UnitTimeKind
        {
            None,
            Seconds,
        }

        /// <summary>
        /// Kind of unit temperature.
        /// </summary>
        public enum UnitTemperatureKind
        {
            None,
            Kelvin,
            Fahrenheit,
        }

        /// <summary>
        /// Kind of unit current.
        /// </summary>
        public enum UnitCurrentKind
        {
            None,
            Ampere,
        }

        /// <summary>
        /// Kind of unit luminous intensity.
        /// </summary>
        public enum UnitLuminousIntensityKind
        {
            None,
            Candela,
        }

        /// <summary>
        /// Looks-up the wire-code for the given multiplier.
        /// Used for Unit and UnitExponent items.
        /// </summary>
        /// <param name="multiplier">Multiplier to look-up.</param>
        /// <returns>The wire-code for the given multiplier.</returns>
        /// <exception cref="HidSpecificationException">When passed invalid multiple value.</exception>
        public static byte MultiplierToWireCode(double multiplier)
        {
            if (!multiplier.IsTenMultiplier() || multiplier == 0)
            {
                throw new HidSpecificationException(Resources.ExceptionMultiplierNotBased10, multiplier);
            }

            try
            {
                return ExponentToWireCode(multiplier.Exponent());
            }
            catch
            {
                string multiplierMinimum = Math.Pow(10, ExponentMinimum).ToDisplayString();
                string multiplierMaximum = Math.Pow(10, ExponentMaximum).ToDisplayString();
                string multiplierStr = multiplier.ToDisplayString();

                throw new HidSpecificationException(Resources.ExceptionMultiplierInvalid, multiplierMinimum, multiplierMaximum, multiplierStr);
            }
        }

        /// <summary>
        /// Looks-up the wire-code for the given exponent.
        /// Used for Unit and UnitExponent items.
        /// </summary>
        /// <param name="exponent">Exponent to look-up.</param>
        /// <returns>The wire-code for the given exponent.</returns>
        /// <exception cref="HidSpecificationException">When passed invalid exponent value.</exception>
        public static byte ExponentToWireCode(int? exponent)
        {
            int normalizedExponent = exponent ?? 0;

            // Code implementation to generate values of code exponent table in hid_11:6.2.2.7. (pgs 39-40).
            if (normalizedExponent >= 0 && normalizedExponent <= ExponentMaximum)
            {
                return (byte)normalizedExponent;
            }
            else if (normalizedExponent < 0 && normalizedExponent >= ExponentMinimum)
            {
                // -8 ==> 8, -7 ==> 9, ... -1 ==> 15.
                return (byte)(16 + normalizedExponent);
            }

            throw new HidSpecificationException(Resources.ExceptionInvalidExponent, ExponentMinimum, ExponentMaximum, normalizedExponent);
        }
    }
}
