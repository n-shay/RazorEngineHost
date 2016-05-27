namespace RazorEngineHost.Templating
{
    using System;

    public class CompilationResults
    {
        public bool Succeeded => this.CompilationException == null;

        public Exception CompilationException { get; }

        private CompilationResults()
        {
            
        }

        private CompilationResults(Exception exception)
        {
            this.CompilationException = exception;
        }

        internal static CompilationResults Success() => new CompilationResults();

        internal static CompilationResults Failed(Exception ex) => new CompilationResults(ex);
    }
}