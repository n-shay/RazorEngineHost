namespace RazorEngineHost.Configuration
{
    using System;
    using System.Collections.Generic;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Compilation.ReferenceResolver;
    using RazorEngineHost.Text;

    public interface IEngineConfiguration
    {
        /// <summary>Gets the base template type.</summary>
        Type BaseTemplateType { get; }

        /// <summary>Gets the compiler service factory.</summary>
        ICompilerServiceFactory CompilerServiceFactory { get; }

        /// <summary>Gets whether the template service is operating in debug mode.</summary>
        bool Debug { get; }

        /// <summary>Gets the encoded string factory.</summary>
        IEncodedStringFactory EncodedStringFactory { get; }
        
        /// <summary>Gets the language.</summary>
        Language Language { get; }

        /// <summary>Gets the namespaces.</summary>
        ISet<string> Namespaces { get; }

        /// <summary>Gets the reference resolver.</summary>
        IReferenceResolver ReferenceResolver { get; }
    }
}