namespace RazorEngineHost.Templating
{
    using System;
    using System.Reflection;

    using global::RazorEngineHost.Compilation;

    /// <summary>
    /// A simple readonly implementation of <see cref="ICompiledTemplate" />.
    /// </summary>
    internal class CompiledTemplate : ICompiledTemplate
    {
        public CompilationData CompilationData { get; }

        //public ITemplateKey Key { get; }

        public string Template { get; }

        public Type TemplateType { get; }

        public Assembly TemplateAssembly => this.TemplateType.Assembly;

        public Type ModelType { get; }

        public CompiledTemplate(CompilationData tempFiles, /*ITemplateKey key,*/ string source, Type templateType, Type modelType)
        {
            this.CompilationData = tempFiles;
            //this.Key = key;
            this.Template = source;
            this.TemplateType = templateType;
            this.ModelType = modelType;
        }
    }
}