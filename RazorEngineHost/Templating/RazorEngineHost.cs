namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;

    using global::RazorEngineHost.Configuration;

    public class RazorEngineHost : IRazorEngineHost
    {
        private bool disposed;

        /// <summary>The internal core instance.</summary>
        internal RazorEngineCore Core { get; }

        /// <summary>Gets the template service configuration.</summary>
        internal IEngineConfiguration Configuration { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="RazorEngineHost" />
        /// </summary>
        /// <param name="config">The template service configuration.</param>
        internal RazorEngineHost(IEngineConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            //if (config.Debug && config.DisableTempFileLocking)
            //    throw new InvalidOperationException("Debug && DisableTempFileLocking is not supported, you need to disable one of them. When Roslyn has been released and you are seeing this, open an issue as this might be possible now.");
            this.Configuration = config;
            this.Core = new RazorEngineCore(new ReadOnlyEngineConfiguration(config));
        }

        ~RazorEngineHost()
        {
            this.Dispose(false);
        }

        public static IRazorEngineHost Create(IEngineConfiguration config)
        {
            return new RazorEngineHost(config);
        }

        public static IRazorEngineHost Create(Action<IConfigurationBuilder> configBuilder)
        {
            var config = new EngineConfiguration();
            configBuilder(new FluentConfigurationBuilder(config));
            return new RazorEngineHost(config);
        }

        public TemplateContext Compile(string templateSource, Type modelType = null)
        {
            try
            {
                CheckModelType(modelType);
                var compiledTemplate = this.Core.Compile(templateSource, modelType);
                return new TemplateContext(compiledTemplate);
            }
            catch (ArgumentException ex)
            {
                return new TemplateContext(ex);
            }
            catch (TemplateLoadingException ex)
            {
                return new TemplateContext(ex);
            }
            catch (TemplateCompilationException ex)
            {
                return new TemplateContext(ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandled compilation exception occured.", ex);
            }
        }

        public void RunCompile(string templateSource, TextWriter writer, Type modelType = null, object model = null, Action<dynamic> configTemplateData = null)
        {
            var context = this.Compile(templateSource, modelType);
            CheckTemplateContext(context);
            this.Run(context, writer, modelType, model, configTemplateData);
        }

        public void Run(TemplateContext templateContext, TextWriter writer, Type modelType = null, object model = null, Action<dynamic> configTemplateData = null)
        {
            CheckModelType(modelType);
            CheckTemplateContext(templateContext);
            if (templateContext.CompiledTemplate.ModelType != modelType)
                throw new ArgumentException(
                    $"Compiled template model does not match the {nameof(modelType)} provided.",
                    nameof(modelType));
            this.Core.RunTemplate(templateContext.CompiledTemplate, writer, model, configTemplateData);
        }

        /// <summary>
        /// Checks if the given model-type has a reference to an anonymous type and throws.
        /// </summary>
        /// <param name="modelType">the type to check</param>
        internal static void CheckModelType(Type modelType)
        {
            // TODO: check if this validation still required.
            if (!(modelType == null) && Compilation.CompilerServicesUtility.IsAnonymousTypeRecursive(modelType))
                throw new ArgumentException(
                    "We cannot support anonymous model types as those are internal! \nHowever you can just use 'dynamic' (modelType == null) and we try to make it work for you (at the cost of performance).");
        }

        /// <summary>
        /// Checks if the given template-context has compiled successfully.
        /// </summary>
        /// <param name="context">The template context to check</param>
        internal static void CheckTemplateContext(TemplateContext context)
        {
            if (!context.Results.Succeeded)
                throw new ArgumentException("Provided template failed to compile.", nameof(context), context.Results.CompilationException);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            // Do additional disposing here...

            this.disposed = true;
        }
    }
}