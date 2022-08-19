// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a single CPP enum.
    /// </summary>
    public class CppEnum : ICppGenerator
    {
        // Duplicate enums are not desired, so they are cached here,
        // and existing returned if would be duplicated.
        private static List<CppEnum> uniqueCache = new List<CppEnum>();

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CppEnum"/> class.
        /// </summary>
        /// <param name="name">Name of the enum.</param>
        /// <param name="enumType">Underyling type of the enum.</param>
        /// <param name="values">Values of the enum.</param>
        private CppEnum(string name, CppFieldPrimativeDataType enumType, IReadOnlyList<string> values)
        {
            this.Name = name;
            this.NameIdSuffix = 0;

            this.EnumType = enumType;
            this.Values = values;
        }

        /// <summary>
        /// Gets the enum unique-cache.
        /// </summary>
        public static List<CppEnum> Cache
        {
            get
            {
                return uniqueCache;
            }
        }

        /// <summary>
        /// Gets the name of this CPP enumeration.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            private set
            {
                this.name = string.Concat(value.Where(char.IsLetterOrDigit));
            }
        }

        /// <summary>
        /// Gets the name suffix that ensures uniqueness across all enumerators.
        /// </summary>
        public int NameIdSuffix
        {
            get; private set;
        }

        /// <summary>
        /// Gets the typed name of this CPP enumeration.
        /// </summary>
        public string TypeName
        {
            get
            {
                string nameIdSuffix = this.NameIdSuffix == 0 ? string.Empty : this.NameIdSuffix.ToString();

                return $"{this.Name}{nameIdSuffix}";
            }
        }

        /// <summary>
        /// Gets the values of this enum.
        /// </summary>
        public IReadOnlyList<string> Values { get; private set; }

        /// <summary>
        /// Gets the underlying type of this class enum.
        /// </summary>
        public CppFieldPrimativeDataType EnumType
        {
            get; private set;
        }

        /// <summary>
        /// Resets the internal (static) unique cache.
        /// </summary>
        public static void Reset()
        {
            uniqueCache = new List<CppEnum>();
        }

        /// <summary>
        /// Generates (or finds from cache) the requested CppEnum.
        /// </summary>
        /// <param name="name">Enumerator name.</param>
        /// <param name="enumType">Underlying type of the enum.</param>
        /// <param name="values">Values of the enum.</param>
        /// <returns>Enum describing the supplied parameters.</returns>
        public static CppEnum Generate(string name, CppFieldPrimativeDataType enumType, IReadOnlyList<string> values)
        {
            CppEnum newEnum = new CppEnum(name, enumType, values);

            CppEnum foundEnum = uniqueCache.Find(x => x.Equals(newEnum));
            if (foundEnum == null)
            {
                foundEnum = newEnum;
                uniqueCache.Add(foundEnum);

                // Ensure all enums are named uniquely.
                foundEnum.NameIdSuffix = UniqueNameCache.GenerateUniqueNameSuffix(foundEnum.Name);
            }

            return foundEnum;
        }

        /// <inheritdoc/>
        /// <example>
        /// <code>
        /// enum class EnumClass1 : uint8_t
        /// {
        ///     EnumValue1,
        ///     EnumValue2,
        /// }
        /// </code>
        /// </example>
        public void GenerateCpp(IndentedWriter writer)
        {
            writer.WriteLineIndented($"enum class {this.TypeName} : {this.EnumType}");
            writer.WriteLineIndented("{");

            bool isFirstEnumValue = true;

            using (DisposableIndent indent = writer.CreateDisposableIndent())
            {
                foreach (string val in this.Values)
                {
                    writer.WriteLineIndented($"{string.Concat(val.Where(char.IsLetterOrDigit))}{(isFirstEnumValue ? " = 1" : string.Empty)},");

                    isFirstEnumValue = false;
                }
            }

            writer.WriteLineIndented("};");
            writer.WriteBlankLine();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            CppEnum objAsCppEnum = obj as CppEnum;

            return this.Equals(objAsCppEnum);
        }

        public bool Equals(CppEnum obj)
        {
            if (obj == null)
            {
                return false;
            }

            bool isSameEnumType = this.EnumType.Equals(obj.EnumType);

            bool isSameValues = this.Values.SequenceEqual(obj.Values);

            return isSameEnumType && isSameValues;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }
}
