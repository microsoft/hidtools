// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.TomlReportDescriptorParser.Tags
{
    using HidEngine.ReportDescriptorComposition.Modules;

    /// <summary>
    /// Describes an interface usable by Tags for those that will generate <see cref="BaseModule"/>s.
    /// </summary>
    public interface IModuleGeneratorTag
    {
        /// <summary>
        /// Generates a descriptor module, describing this Tag.
        /// </summary>
        /// <param name="parent">Logical parent for the newly generated descriptor module.</param>
        /// <returns>Module describing this Tag.</returns>
        /// <exception cref="TomlDescriptorModuleParsingErrorException">Thrown when unable to parse into descriptor modules.</exception>
        BaseModule GenerateDescriptorModule(BaseModule parent);
    }
}