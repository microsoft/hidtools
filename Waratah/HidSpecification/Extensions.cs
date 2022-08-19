// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidSpecification
{
    using System.Globalization;

    /// <summary>
    /// Extension to Double.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Extension Class")]
    public static class DoubleExtension
    {
        /// <summary>
        /// Formatted string of double suitable for display.
        /// Adds separators and removes trailing zero's.
        /// </summary>
        /// <param name="self">This double.</param>
        /// <returns>Formatted string.</returns>
        public static string ToDisplayString(this double self)
        {
            if (self >= 0 && self < 1)
            {
                return self.ToString("N8").TrimEnd('0');
            }
            else
            {
                // Print with ',' separating every 3 digits (only works with values > 1).
                return self.ToString("#,#", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Whether this double is a base-10 multiplier.
        /// </summary>
        /// <param name="self">This double.</param>
        /// <example> 100, 0.001, are multipliers.  0.2, 2, 20 are not.</example>
        /// <returns>True if a base-10 multiplier.</returns>
        public static bool IsTenMultiplier(this double self)
        {
            return Mantissa(self) == 1;
        }

        /// <summary>
        /// Mantissa of the double.
        /// </summary>
        /// <param name="self">This double.</param>
        /// <example>For 2.0x10^7, the Mantissa is 2.</example>
        /// <returns>The mantissa.</returns>
        public static double Mantissa(this double self)
        {
            return ParseDoubleParts(self).Item1;
        }

        /// <summary>
        /// Exponent of the double.
        /// </summary>
        /// <param name="self">This double.</param>
        /// <example>For 2.0x10^7, the Exponent is 7.</example>
        /// <returns>The exponent.</returns>
        public static int Exponent(this double self)
        {
            return ParseDoubleParts(self).Item2;
        }

        private static (double, int) ParseDoubleParts(double d)
        {
            var doubleParts = d.ToString(@"E15").Split('E');

            double mantissa = double.Parse(doubleParts[0]);
            int exponent = int.Parse(doubleParts[1]);

            return (mantissa, exponent);
        }
    }
}
