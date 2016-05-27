namespace RazorEngineHost.Templating
{
    using System;

    public class TemplateContext
    {
        public TemplateContext(ICompiledTemplate compiledTemplate)
        {
            this.CompiledTemplate = compiledTemplate;
            this.Results = CompilationResults.Success();
        }

        public TemplateContext(Exception compilationException)
        {
            this.Results = CompilationResults.Failed(compilationException);
        }

        public ICompiledTemplate CompiledTemplate { get; }

        public CompilationResults Results { get; }
    }
}