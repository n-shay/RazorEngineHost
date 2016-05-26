namespace RazorEngineHost.Templating
{
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Security;

    using global::RazorEngineHost.Compilation;
    using global::RazorEngineHost.Compilation.ReferenceResolver;
    using global::RazorEngineHost.Configuration;

    internal class RazorEngineCore
    {
        public IEngineConfiguration Configuration { get; }

        public RazorEngineCore(ReadOnlyEngineConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            this.Configuration = config;
        }

        /// <summary>Compiles the specified template.</summary>
        /// <param name="templateSource">The string template.</param>
        /// <param name="modelType">The model type.</param>
        public ICompiledTemplate Compile(/*ITemplateKey key,*/string templateSource, Type modelType)
        {
            //ITemplateSource templateSource = this.Resolve(key);
            var templateType = this.CreateTemplateType(templateSource, modelType);
            return new CompiledTemplate(templateType.Item2, templateSource, templateType.Item1, modelType);
        }

        /// <summary>
        /// Creates an instance of <see cref="ITemplate" /> from the specified string template.
        /// </summary>
        /// <param name="template">The compiled template.</param>
        /// <param name="model">The model instance or NULL if no model exists.</param>
        /// <returns>An instance of <see cref="ITemplate" />.</returns>
        internal virtual ITemplate CreateTemplate(ICompiledTemplate template, object model)
        {
            var instance = (ITemplate)Activator.CreateInstance(template.TemplateType);
            instance.InternalTemplateService = new InternalTemplateService(this);
            instance.SetModel(model);
            return instance;
        }

        /// <summary>
        /// Creates a <see cref="Type" /> that can be used to instantiate an instance of a template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="modelType">The model type or NULL if no model exists.</param>
        /// <returns>An instance of <see cref="Type" />.</returns>
        [SecuritySafeCritical]
        public virtual Tuple<Type, CompilationData> CreateTemplateType(string razorTemplate, Type modelType)
        {
            var typeContext = new TypeContext
                {
                    ModelType = modelType ?? typeof (DynamicObject),
                    TemplateContent = razorTemplate,
                    TemplateType = this.Configuration.BaseTemplateType ?? typeof (TemplateBase<>)
                };
            foreach (var @namespace in this.Configuration.Namespaces)
                typeContext.Namespaces.Add(@namespace);
            var compilerService = this.Configuration.CompilerServiceFactory.CreateCompilerService(this.Configuration.Language);
            compilerService.Debug = this.Configuration.Debug;
            //compilerService.DisableTempFileLocking = this.Configuration.DisableTempFileLocking;
            compilerService.ReferenceResolver = this.Configuration.ReferenceResolver ?? new UseCurrentAssembliesReferenceResolver();
            return compilerService.CompileType(typeContext);
        }

        /// <summary>Runs the specified template and returns the result.</summary>
        /// <param name="template">The template to run.</param>
        /// <param name="writer"></param>
        /// <param name="model"></param>
        /// <returns>The string result of the template.</returns>
        public void RunTemplate(ICompiledTemplate template, TextWriter writer, object model)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            this.CreateTemplate(template, model).Run(writer);
        }

    }
}