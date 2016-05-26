namespace RazorEngineHost.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Compilation.ReferenceResolver;
    using RazorEngineHost.Text;

    public class EngineConfiguration : IEngineConfiguration
    {
        public Type BaseTemplateType { get; set; }

        public ICompilerServiceFactory CompilerServiceFactory { get; set; }

        public bool Debug { get; set; }

        public IEncodedStringFactory EncodedStringFactory { get; set; }

        public Language Language { get; set; }

        public ISet<string> Namespaces { get; set; }

        public IReferenceResolver ReferenceResolver { get; set; }

        /// <summary>
        /// Initialises a new instance of <see cref="EngineConfiguration" />.
        /// </summary>
        [SecuritySafeCritical]
        public EngineConfiguration()
        {
            //this.Activator = (IActivator)new DefaultActivator();
            this.CompilerServiceFactory = new DefaultCompilerServiceFactory();
            this.EncodedStringFactory = new HtmlEncodedStringFactory();
            this.ReferenceResolver = new UseCurrentAssembliesReferenceResolver();
            //this.CachingProvider = (ICachingProvider)new DefaultCachingProvider();
            //this.TemplateManager = (ITemplateManager)new DelegateTemplateManager();
            this.Namespaces = new HashSet<string>()
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq"
                };
            this.Language = Language.CSharp;
        }
    }
}