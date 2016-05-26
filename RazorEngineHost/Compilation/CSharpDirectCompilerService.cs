namespace RazorEngineHost.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Web.Razor;
    using System.Web.Razor.Parser;

    using global::RazorEngineHost.Compilation.ReferenceResolver;

    using Microsoft.CSharp;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>Defines a direct compiler service for the C# syntax.</summary>
    public class CSharpDirectCompilerService : DirectCompilerServiceBase
    {
        /// <summary>
        /// Extension of a source file without dot ("cs" for C# files or "vb" for VB.NET files).
        /// </summary>
        public override string SourceFileExtension => "cs";

        /// <summary>
        /// Initialises a new instance of <see cref="CSharp.CSharpDirectCompilerService" />.
        /// </summary>
        /// <param name="markupParserFactory">The markup parser factory to use.</param>
        [SecurityCritical]
        public CSharpDirectCompilerService(Func<ParserBase> markupParserFactory = null)
          : base(new CSharpRazorCodeLanguage(), new CSharpCodeProvider(), markupParserFactory)
        {
        }

        /// <summary>
        /// Returns a set of assemblies that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of assemblies.</returns>
        public override IEnumerable<CompilerReference> IncludeReferences()
        {
            return new[]
                {
                    CompilerReference.From(typeof (Binder).Assembly)
                };
        }

        /// <summary>Builds a type name for the specified template type.</summary>
        /// <param name="templateType">The template type.</param>
        /// <param name="modelType">The model type.</param>
        /// <returns>The string type name (including namespace).</returns>
        public override string BuildTypeName(Type templateType, Type modelType)
        {
            if (templateType == null)
                throw new ArgumentNullException(nameof(templateType));
            var modelTypeName = CompilerServicesUtility.ResolveCSharpTypeName(modelType);
            return CompilerServicesUtility.CSharpCreateGenericType(templateType, modelTypeName, false);
        }
    }
}