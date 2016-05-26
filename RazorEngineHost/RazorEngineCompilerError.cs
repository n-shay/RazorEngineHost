namespace RazorEngineHost
{
    using System;

    /// <summary>Defines a compiler error.</summary>
    [Serializable]
    public class RazorEngineCompilerError
    {
        /// <summary>The error text of the error.</summary>
        public string ErrorText { get; }

        /// <summary>The file name of the error source.</summary>
        public string FileName { get; }

        /// <summary>The line number of the error location</summary>
        public int Line { get; }

        /// <summary>The column number of the error location.</summary>
        public int Column { get; }

        /// <summary>The number of the error.</summary>
        public string ErrorNumber { get; }

        /// <summary>Indicates whether the error is a warning.</summary>
        public bool IsWarning { get; }

        /// <summary>Creates a new Compiler error instance.</summary>
        /// <param name="errorText"></param>
        /// <param name="fileName"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="errorNumber"></param>
        /// <param name="isWarning"></param>
        public RazorEngineCompilerError(
            string errorText,
            string fileName,
            int line,
            int column,
            string errorNumber,
            bool isWarning)
        {
            this.ErrorText = errorText;
            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
            this.ErrorNumber = errorNumber;
            this.IsWarning = isWarning;
        }
    }
}