namespace RazorEngineHost.Compilation
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;

    using global::RazorEngineHost.Templating;

    public class CompilationResults : GeneratorResults
    {
        public TemplateBase Compiled { get; }
        public IList<CompilerError> Errors { get; }

        public CompilationResults(GeneratorResults generatorResults, IList<CompilerError> errors)
            : base(false, generatorResults.Document, generatorResults.ParserErrors, generatorResults.GeneratedCode, generatorResults.DesignTimeLineMappings)
        {
            this.Errors = errors;
        }

        public CompilationResults(GeneratorResults generatorResults, TemplateBase compiled)
            : base(true, generatorResults.Document, generatorResults.ParserErrors, generatorResults.GeneratedCode, generatorResults.DesignTimeLineMappings)
        {
            this.Compiled = compiled;
        }
    }

    public class CompilationResults<TModel> : GeneratorResults
    {
        public TemplateBase<TModel> Compiled { get; }
        public IList<CompilerError> Errors { get; }

        public CompilationResults(GeneratorResults generatorResults, IList<CompilerError> errors)
            : base(false, generatorResults.Document, generatorResults.ParserErrors, generatorResults.GeneratedCode, generatorResults.DesignTimeLineMappings)
        {
            this.Errors = errors;
        }

        public CompilationResults(GeneratorResults generatorResults, TemplateBase<TModel> compiled)
            : base(true, generatorResults.Document, generatorResults.ParserErrors, generatorResults.GeneratedCode, generatorResults.DesignTimeLineMappings)
        {
            this.Compiled = compiled;
        }
    }
}
