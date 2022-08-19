// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;

    /// <summary>
    /// A Disposable wrapper around an <see cref="IndentedWriter"/>.
    /// Each instance indents the writer by 1.
    /// </summary>
    public class DisposableIndent : IDisposable
    {
        private IndentedWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableIndent"/> class.
        /// </summary>
        /// <param name="writer">Where the generated CPP text is written to.</param>
        public DisposableIndent(IndentedWriter writer)
        {
            this.writer = writer;

            this.writer.Increase();
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            this.writer.Decrease();
        }
    }
}
