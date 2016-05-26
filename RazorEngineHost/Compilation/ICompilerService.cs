﻿namespace RazorEngineHost.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    using global::RazorEngineHost.Compilation.ReferenceResolver;

    public interface ICompilerService
    {
        /// <summary>Gets or sets the reference resolver.</summary>
        IReferenceResolver ReferenceResolver { get; set; }

        /// <summary>
        /// Gets or sets whether the compiler service is operating in debug mode.
        /// </summary>
        bool Debug { get; set; }

        /// <summary>
        /// Gets or sets whether the compiler should load assemblies with Assembly.Load(byte[])
        /// to prevent files from being locked.
        /// </summary>
        bool DisableTempFileLocking { get; set; }

        /// <summary>
        /// Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        [SecurityCritical]
        Tuple<Type, CompilationData> CompileType(TypeContext context);

        /// <summary>
        /// Returns a set of assemblies that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of assemblies.</returns>
        IEnumerable<CompilerReference> IncludeReferences();
    }
}