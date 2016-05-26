namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the required contract for implementing a template service.
    /// The main API for running templates.
    /// </summary>
    public interface IRazorEngineHost : IDisposable
    {
        /// <summary>Compiles the specified template and caches it.</summary>
        /// <param name="templateSource">The template.</param>
        /// <param name="modelType">The model type.</param>
        ICompiledTemplate Compile(string templateSource, Type modelType = null);

        /// <summary>
        /// Runs the given cached template.
        /// When the cache does not contain the template
        /// it will be compiled and cached beforehand.
        /// </summary>
        /// <param name="templateSource"></param>
        /// <param name="writer"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        void RunCompile(string templateSource, TextWriter writer, Type modelType = null, object model = null);

        /// <summary>Runs the given cached template.</summary>
        /// <param name="compiledTemplate"></param>
        /// <param name="writer"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        void Run(ICompiledTemplate compiledTemplate, TextWriter writer, Type modelType = null, object model = null);
    }
}