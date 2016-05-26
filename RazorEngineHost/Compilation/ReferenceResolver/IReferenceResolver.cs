﻿namespace RazorEngineHost.Compilation.ReferenceResolver
{
    using System.Collections.Generic;

    /// <summary>
    /// Tries to resolve the references for a given compilation option.
    /// </summary>
    public interface IReferenceResolver
    {
        /// <summary>Resolve the reference for a compilation process.</summary>
        /// <param name="context">gives context about the compilation process.</param>
        /// <param name="includeAssemblies">The references that should be included (requested by the compiler itself)</param>
        /// <returns>the references which will be used in the compilation process.</returns>
        IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies = null);
    }
}