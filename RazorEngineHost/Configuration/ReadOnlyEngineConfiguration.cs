namespace RazorEngineHost.Configuration
{
    using System;
    using System.Collections.Generic;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Compilation.ReferenceResolver;
    using RazorEngineHost.Templating;
    using RazorEngineHost.Text;

    public class ReadOnlyEngineConfiguration : IEngineConfiguration
    {
        public Type BaseTemplateType { get; }

        public ICompilerServiceFactory CompilerServiceFactory { get; }

        public bool Debug { get; }

        public IEncodedStringFactory EncodedStringFactory { get; }

        public Language Language { get; }

        public ISet<string> Namespaces { get; }

        public IReferenceResolver ReferenceResolver { get; }

        public ReadOnlyEngineConfiguration(IEngineConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            this.BaseTemplateType = config.BaseTemplateType;
            if (this.BaseTemplateType != null && !typeof(TemplateBase).IsAssignableFrom(this.BaseTemplateType))
                throw new ArgumentOutOfRangeException(nameof(config), $"The configured {nameof(this.BaseTemplateType)} must implement {typeof(TemplateBase).Name}!");
            this.CompilerServiceFactory = config.CompilerServiceFactory;
            if (this.CompilerServiceFactory == null)
                throw new ArgumentNullException(nameof(config), $"The configured {nameof(this.CompilerServiceFactory)} cannot be null!");
            this.Debug = config.Debug;
            this.EncodedStringFactory = config.EncodedStringFactory;
            if (this.EncodedStringFactory == null)
                throw new ArgumentNullException(nameof(config), $"The configured {nameof(this.EncodedStringFactory)} cannot be null!");
            this.Language = config.Language;
            this.Namespaces = config.Namespaces;
            if (this.Namespaces == null)
                throw new ArgumentNullException(nameof(config), $"The configured {nameof(this.Namespaces)} cannot be null!");
            this.ReferenceResolver = config.ReferenceResolver;
            if (this.ReferenceResolver == null)
                throw new ArgumentNullException(nameof(config), $"The configured {nameof(this.ReferenceResolver)} cannot be null!");
        }
    }
}