namespace RazorEngineHost.Compilation.ReferenceResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Resolves the assemblies by using all currently loaded assemblies. See <see cref="ReferenceResolver.IReferenceResolver" />
    /// </summary>
    public class UseCurrentAssembliesReferenceResolver : IReferenceResolver
    {
        /// <summary>
        /// See <see cref="M:RazorEngine.Compilation.ReferenceResolver.IReferenceResolver.GetReferences(RazorEngine.Compilation.TypeContext,System.Collections.Generic.IEnumerable{RazorEngine.Compilation.ReferenceResolver.CompilerReference})" />
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeAssemblies"></param>
        /// <returns></returns>
        public IEnumerable<CompilerReference> GetReferences(TypeContext context = null, IEnumerable<CompilerReference> includeAssemblies = null)
        {
            return CompilerServicesUtility.GetLoadedAssemblies().Where<Assembly>((Func<Assembly, bool>)(a =>
            {
                if (!a.IsDynamic && File.Exists(a.Location))
                    return !a.Location.Contains("CompiledRazorTemplates.Dynamic");
                return false;
            })).GroupBy<Assembly, string>((Func<Assembly, string>)(a => a.GetName().Name)).Select<IGrouping<string, Assembly>, Assembly>((Func<IGrouping<string, Assembly>, Assembly>)(grp => grp.First<Assembly>((Func<Assembly, bool>)(y => y.GetName().Version == grp.Max<Assembly, Version>((Func<Assembly, Version>)(x => x.GetName().Version)))))).Select<Assembly, CompilerReference>((Func<Assembly, CompilerReference>)(a => CompilerReference.From(a))).Concat<CompilerReference>(includeAssemblies ?? Enumerable.Empty<CompilerReference>());
        }
    }
}