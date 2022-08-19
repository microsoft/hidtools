// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    /// <summary>
    /// Interface describing a class that can generate CPP text.
    /// </summary>
    public interface ICppGenerator
    {
        /// <summary>
        /// Generates CPP text representing the implementers state.
        /// </summary>
        /// <param name="writer">Where the generated CPP text is written to.</param>
        void GenerateCpp(IndentedWriter writer);
    }
}
