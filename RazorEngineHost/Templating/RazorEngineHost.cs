namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;

    using global::RazorEngineHost.Compilation;
    using global::RazorEngineHost.Configuration;

    public class RazorEngineHost : IRazorEngineHost
    {
        private bool disposed;
        
        /// <summary>The internal core instance.</summary>
        internal RazorEngineCore Core { get; }

        /// <summary>Gets the template service configuration.</summary>
        internal IEngineConfiguration Configuration { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="T:RazorEngine.Templating.TemplateService" />
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

        public ICompiledTemplate Compile(string templateSource, Type modelType = null)
        {
            CheckModelType(modelType);
            return this.Core.Compile(templateSource, modelType);
        }

        public void RunCompile(string templateSource, TextWriter writer, Type modelType = null, object model = null)
        {
            CheckModelType(modelType);
            var compiledTemplate = this.Compile(templateSource, modelType);
            this.Run(compiledTemplate, writer, modelType, model);
        }

        public void Run(ICompiledTemplate compiledTemplate, TextWriter writer, Type modelType = null, object model = null)
        {
            CheckModelType(modelType);
            if (compiledTemplate.ModelType != modelType)
                throw new ArgumentException($"Compiled template model does not match the {nameof(modelType)} provided.", nameof(modelType));
            this.Core.RunTemplate(compiledTemplate, writer, model);
        }

        /// <summary>
        /// Checks if the given model-type has a reference to an anonymous type and throws.
        /// </summary>
        /// <param name="modelType">the type to check</param>
        internal static void CheckModelType(Type modelType)
        {
            if (!(modelType == null) && CompilerServicesUtility.IsAnonymousTypeRecursive(modelType))
                throw new ArgumentException("We cannot support anonymous model types as those are internal! \nHowever you can just use 'dynamic' (modelType == null) and we try to make it work for you (at the cost of performance).");
        }

        ///// <summary>
        ///// Checks if we need to wrap the given model in
        ///// an <see cref="T:RazorEngine.Compilation.RazorDynamicObject" /> instance and wraps it.
        ///// </summary>
        ///// <param name="modelType">the model-type</param>
        ///// <param name="original">the original model</param>
        ///// <param name="allowMissing">true when we should allow missing properties on dynamic models.</param>
        ///// <returns>the original model or an wrapper object.</returns>
        //internal static object GetDynamicModel(Type modelType, object original, bool allowMissing)
        //{
        //    object obj = original;
        //    if (modelType == (Type)null && original != null)
        //    {
        //        if (CompilerServicesUtility.IsAnonymousTypeRecursive(original.GetType()))
        //            obj = RazorDynamicObject.Create(original, allowMissing);
        //        else if (allowMissing)
        //            obj = RazorDynamicObject.Create(original, allowMissing);
        //    }
        //    return obj;
        //}

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