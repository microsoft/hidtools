// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Diagnostics;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Actual type is lowercase.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Items are self-explanatory.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Simple enum.")]
    public enum CppFieldPrimativeDataType
    {
        uint8_t,
        int8_t,
        uint16_t,
        int16_t,
        uint32_t,
        int32_t,
    }

    /// <summary>
    /// Container around CalculateType method.
    /// </summary>
    public static class CppFieldPrimativeDataTypeExtension
    {
        /// <summary>
        /// Determines the CPP primative type from it's minimum value and size.
        /// </summary>
        /// <param name="min">Minimum value of type.</param>
        /// <param name="sizeInBits">Size of the type (in bits).</param>
        /// <returns>CPP primative type.</returns>
        public static CppFieldPrimativeDataType CalculateType(int min, int sizeInBits)
        {
            bool isPositive = min >= 0;

            switch (sizeInBits)
            {
                case 8:
                {
                    return isPositive ? CppFieldPrimativeDataType.uint8_t : CppFieldPrimativeDataType.int8_t;
                }

                case 16:
                {
                    return isPositive ? CppFieldPrimativeDataType.uint16_t : CppFieldPrimativeDataType.int16_t;
                }

                case 32:
                {
                    return isPositive ? CppFieldPrimativeDataType.uint32_t : CppFieldPrimativeDataType.int32_t;
                }

                default:
                {
                    System.Environment.FailFast("Must be either 8, 16 or 32");

                    // Will never get to here.
                    return CppFieldPrimativeDataType.uint8_t;
                }
            }
        }
    }
}
