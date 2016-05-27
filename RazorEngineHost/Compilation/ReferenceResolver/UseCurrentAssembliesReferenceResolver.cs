namespace RazorEngineHost.Compilation.ReferenceResolver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Resolves the assemblies by using all currently loaded assemblies. See <see cref="IReferenceResolver" />
    /// </summary>
    public class UseCurrentAssembliesReferenceResolver : IReferenceResolver
    {
        /// <summary>
        /// See <see cref="IReferenceResolver.GetReferences(TypeContext,IEnumerable{CompilerReference})" />
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeAssemblies"></param>
        /// <returns></returns>
        public IEnumerable<CompilerReference> GetReferences(
            TypeContext context = null,
            IEnumerable<CompilerReference> includeAssemblies = null)
        {
            return CompilerServicesUtility.GetLoadedAssemblies()
                .Where(
                    a =>
                        {
                            if (!a.IsDynamic && File.Exists(a.Location))
                                return !a.Location.Contains("CompiledRazorTemplates.Dynamic");
                            return false;
                        })
                .GroupBy(a => a.GetName().Name)
                .Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version)))
                .Select(CompilerReference.From)
                .Concat(includeAssemblies ?? Enumerable.Empty<CompilerReference>());
        }
    }
}