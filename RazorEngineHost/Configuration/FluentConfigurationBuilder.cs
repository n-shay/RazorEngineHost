namespace RazorEngineHost.Configuration
{
    using System;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Text;

    /// <summary>
    /// Provides a default implementation of a <see cref="IConfigurationBuilder" />.
    /// </summary>
    internal class FluentConfigurationBuilder : IConfigurationBuilder
    {
        private readonly EngineConfiguration config;

        /// <summary>
        /// Initialises a new instance of <see cref="FluentConfigurationBuilder" />.
        /// </summary>
        /// <param name="config">The default configuration that we build a new configuration from.</param>
        public FluentConfigurationBuilder(EngineConfiguration config)
        {
            this.config = config;
        }

        ///// <summary>Sets the activator.</summary>
        ///// <param name="activator">The activator instance.</param>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ActivateUsing(IActivator activator)
        //{
        //    if (activator == null)
        //        throw new ArgumentNullException("activator");
        //    this._config.Activator = activator;
        //    return (IConfigurationBuilder)this;
        //}

        ///// <summary>Sets the activator.</summary>
        ///// <typeparam name="TActivator">The activator type.</typeparam>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ActivateUsing<TActivator>() where TActivator : IActivator, new()
        //{
        //    return this.ActivateUsing((IActivator)Activator.CreateInstance<TActivator>());
        //}

        ///// <summary>Sets the activator.</summary>
        ///// <param name="activator">The activator delegate.</param>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ActivateUsing(Func<InstanceContext, ITemplate> activator)
        //{
        //    if (activator == null)
        //        throw new ArgumentNullException("activator");
        //    this._config.Activator = (IActivator)new DelegateActivator(activator);
        //    return (IConfigurationBuilder)this;
        //}

        ///// <summary>
        ///// Sets that dynamic models should be fault tollerant in accepting missing properties.
        ///// </summary>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder AllowMissingPropertiesOnDynamic()
        //{
        //    this._config.AllowMissingPropertiesOnDynamic = true;
        //    return (IConfigurationBuilder)this;
        //}

        /// <summary>Sets the compiler service factory.</summary>
        /// <param name="factory">The compiler service factory.</param>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder CompileUsing(ICompilerServiceFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            this.config.CompilerServiceFactory = factory;
            return this;
        }

        /// <summary>Sets the compiler service factory.</summary>
        /// <typeparam name="TCompilerServiceFactory">The compiler service factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder CompileUsing<TCompilerServiceFactory>() where TCompilerServiceFactory : ICompilerServiceFactory, new()
        {
            return this.CompileUsing(Activator.CreateInstance<TCompilerServiceFactory>());
        }

        /// <summary>Sets the encoded string factory.</summary>
        /// <param name="factory">The encoded string factory.</param>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder EncodeUsing(IEncodedStringFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            this.config.EncodedStringFactory = factory;
            return this;
        }

        /// <summary>Sets the encoded string factory.</summary>
        /// <typeparam name="TEncodedStringFactory">The encoded string factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder EncodeUsing<TEncodedStringFactory>() where TEncodedStringFactory : IEncodedStringFactory, new()
        {
            return this.EncodeUsing(Activator.CreateInstance<TEncodedStringFactory>());
        }

        /// <summary>Includes the specified namespaces</summary>
        /// <param name="namespaces">The set of namespaces to include.</param>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder IncludeNamespaces(params string[] namespaces)
        {
            if (namespaces == null)
                throw new ArgumentNullException(nameof(namespaces));
            foreach (string @namespace in namespaces)
                this.config.Namespaces.Add(@namespace);
            return this;
        }
        
        ///// <summary>Sets the resolve used to locate unknown templates.</summary>
        ///// <typeparam name="TResolver">The resolve type.</typeparam>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ManageUsing<TResolver>() where TResolver : ITemplateManager, new()
        //{
        //    this._config.TemplateManager = (ITemplateManager)Activator.CreateInstance<TResolver>();
        //    return (IConfigurationBuilder)this;
        //}
        
        ///// <summary>Sets the resolver used to locate unknown templates.</summary>
        ///// <param name="resolver">The resolver instance to use.</param>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ManageUsing(ITemplateManager resolver)
        //{
        //    this._config.TemplateManager = resolver;
        //    return (IConfigurationBuilder)this;
        //}

        ///// <summary>
        ///// Sets the resolver delegate used to locate unknown templates.
        ///// </summary>
        ///// <param name="resolver">The resolver delegate to use.</param>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder ResolveUsing(Func<string, string> resolver)
        //{
        //    this._config.TemplateManager = (ITemplateManager)new DelegateTemplateManager(resolver);
        //    return (IConfigurationBuilder)this;
        //}

        ///// <summary>
        ///// Loads all dynamic assemblies with Assembly.Load(byte[]).
        ///// This prevents temp files from being locked (which makes it impossible for RazorEngineHost to delete them).
        ///// At the same time this completely shuts down any sandboxing/security.
        ///// Use this only if you have a limited amount of static templates (no modifications on rumtime),
        ///// which you fully trust and when a seperate AppDomain is no solution for you!.
        ///// This option will also hurt debugging.
        ///// 
        ///// OK, YOU HAVE BEEN WARNED.
        ///// </summary>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder DisableTempFileLocking()
        //{
        //    this._config.DisableTempFileLocking = true;
        //    return (IConfigurationBuilder)this;
        //}

        ///// <summary>Sets the default activator.</summary>
        ///// <returns>The current configuration builder.</returns>
        //public IConfigurationBuilder UseDefaultActivator()
        //{
        //    this._config.Activator = (IActivator)new DefaultActivator();
        //    return (IConfigurationBuilder)this;
        //}

        /// <summary>Sets the default compiler service factory.</summary>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder UseDefaultCompilerServiceFactory()
        {
            this.config.CompilerServiceFactory = new DefaultCompilerServiceFactory();
            return this;
        }

        /// <summary>Sets the default encoded string factory.</summary>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder UseDefaultEncodedStringFactory()
        {
            this.config.EncodedStringFactory = new HtmlEncodedStringFactory();
            return this;
        }

        /// <summary>Sets the base template type.</summary>
        /// <param name="baseTemplateType">The base template type.</param>
        /// <returns>The current configuration builder/.</returns>
        public IConfigurationBuilder WithBaseTemplateType(Type baseTemplateType)
        {
            this.config.BaseTemplateType = baseTemplateType;
            return this;
        }

        /// <summary>Sets the code language.</summary>
        /// <param name="language">The code language.</param>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder WithCodeLanguage(Language language)
        {
            this.config.Language = language;
            return this;
        }

        /// <summary>Sets the encoding.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The current configuration builder.</returns>
        public IConfigurationBuilder WithEncoding(Encoding encoding)
        {
            switch (encoding)
            {
                case Encoding.Html:
                    this.config.EncodedStringFactory = new HtmlEncodedStringFactory();
                    break;
                case Encoding.Raw:
                    this.config.EncodedStringFactory = new RawStringFactory();
                    break;
                default:
                    throw new ArgumentException("Unsupported encoding: " + encoding);
            }
                
            return this;
        }
    }
}