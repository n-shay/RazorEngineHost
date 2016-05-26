﻿namespace RazorEngineHost.Configuration
{
    using System;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Text;

    /// <summary>
    /// Defines the required contract for implementing a configuration builder.
    /// </summary>
    public interface IConfigurationBuilder
    {
        ///// <summary>Sets the activator.</summary>
        ///// <param name="activator">The activator instance.</param>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ActivateUsing(IActivator activator);

        ///// <summary>Sets the activator.</summary>
        ///// <typeparam name="TActivator">The activator type.</typeparam>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ActivateUsing<TActivator>() where TActivator : IActivator, new();

        ///// <summary>Sets the activator.</summary>
        ///// <param name="activator">The activator delegate.</param>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ActivateUsing(Func<InstanceContext, ITemplate> activator);

        /// <summary>Sets the compiler service factory.</summary>
        /// <param name="factory">The compiler service factory.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder CompileUsing(ICompilerServiceFactory factory);

        /// <summary>Sets the compiler service factory.</summary>
        /// <typeparam name="TCompilerServiceFactory">The compiler service factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder CompileUsing<TCompilerServiceFactory>() where TCompilerServiceFactory : ICompilerServiceFactory, new();

        /// <summary>Sets the encoded string factory.</summary>
        /// <param name="factory">The encoded string factory.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder EncodeUsing(IEncodedStringFactory factory);

        /// <summary>Sets the encoded string factory.</summary>
        /// <typeparam name="TEncodedStringFactory">The encoded string factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder EncodeUsing<TEncodedStringFactory>() where TEncodedStringFactory : IEncodedStringFactory, new();

        ///// <summary>Sets the manager used to locate unknown templates.</summary>
        ///// <typeparam name="TManager">The manager type.</typeparam>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ManageUsing<TManager>() where TManager : ITemplateManager, new();

        ///// <summary>Sets the manager used to locate unknown templates.</summary>
        ///// <param name="manager">The manager instance to use.</param>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ManageUsing(ITemplateManager manager);

        ///// <summary>
        ///// Sets the resolver delegate used to locate unknown templates.
        ///// </summary>
        ///// <param name="resolver">The resolver delegate to use.</param>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder ResolveUsing(Func<string, string> resolver);

        /// <summary>Includes the specified namespaces</summary>
        /// <param name="namespaces">The set of namespaces to include.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder IncludeNamespaces(params string[] namespaces);

        ///// <summary>
        ///// Loads all dynamic assemblies with Assembly.Load(byte[]).
        ///// This prevents temp files from being locked (which makes it impossible for RazorEngine to delete them).
        ///// At the same time this completely shuts down any sandboxing/security.
        ///// Use this only if you have a limited amount of static templates (no modifications on rumtime),
        ///// which you fully trust and when a seperate AppDomain is no solution for you!.
        ///// This option will also hurt debugging.
        ///// 
        ///// OK, YOU HAVE BEEN WARNED.
        ///// </summary>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder DisableTempFileLocking();

        ///// <summary>Sets the default activator.</summary>
        ///// <returns>The current configuration builder.</returns>
        //IConfigurationBuilder UseDefaultActivator();

        /// <summary>Sets the default compiler service factory.</summary>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder UseDefaultCompilerServiceFactory();

        /// <summary>Sets the default encoded string factory.</summary>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder UseDefaultEncodedStringFactory();

        /// <summary>Sets the base template type.</summary>
        /// <param name="baseTemplateType">The base template type.</param>
        /// <returns>The current configuration builder/.</returns>
        IConfigurationBuilder WithBaseTemplateType(Type baseTemplateType);

        /// <summary>Sets the code language.</summary>
        /// <param name="language">The code language.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder WithCodeLanguage(Language language);

        /// <summary>Sets the encoding.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder WithEncoding(Encoding encoding);
    }
}