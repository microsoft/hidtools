// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.HidTools.HidEngine.CppGenerator
{
    using System;
    using System.Text;

    /// <summary>
    /// StringBuilder wrapper providing automatic indentation.
    /// </summary>
    public class IndentedWriter
    {
        private const int IndentSize = 4;

        private int indentLevel;

        private StringBuilder sb;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedWriter"/> class.
        /// </summary>
        public IndentedWriter()
        {
            this.sb = new StringBuilder();
            this.indentLevel = 0;
        }

        /// <summary>
        /// Finalizes and returns the written string.
        /// </summary>
        /// <returns>Finalized written string.</returns>
        public string Generate()
        {
            return this.sb.ToString().Trim();
        }

        /// <summary>
        /// Increases indent by 1.
        /// </summary>
        public void Increase()
        {
            this.indentLevel++;
        }

        /// <summary>
        /// Safely Decreases indent by 1.
        /// </summary>
        public void Decrease()
        {
            if (this.indentLevel > 0)
            {
                this.indentLevel--;
            }
        }

        /// <summary>
        /// Creates a disposable indent.
        /// </summary>
        /// <returns>Disposable indent.</returns>
        public DisposableIndent CreateDisposableIndent()
        {
            return new DisposableIndent(this);
        }

        /// <summary>
        /// Writes the supplied line with the currently defined indent.
        /// </summary>
        /// <param name="line">Line to write.</param>
        public void WriteLineIndented(string line)
        {
            string indentedLine = (new string(' ', this.indentLevel * IndentSize)) + line;

            this.sb.AppendLine(indentedLine);
        }

        /// <summary>
        /// Writes the supplied line with no prefix.
        /// </summary>
        /// <param name="line">Line to write.</param>
        public void WriteLineNoIndent(string line)
        {
            this.sb.Append(line);
        }

        /// <summary>
        /// Writes a blank line.
        /// </summary>
        public void WriteBlankLine()
        {
            this.sb.AppendLine();
        }
    }
}
