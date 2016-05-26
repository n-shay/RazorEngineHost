namespace RazorEngineHost.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    using global::RazorEngineHost.Compilation.ReferenceResolver;
    using global::RazorEngineHost.Templating;

    /// <summary>Provides a base implementation of a compiler service.</summary>
    public abstract class CompilerServiceBase : ICompilerService
    {
        /// <summary>All references we used until now.</summary>
        private readonly HashSet<CompilerReference> references = new HashSet<CompilerReference>();

        /// <summary>The namespace for dynamic templates.</summary>
        protected internal const string DynamicTemplateNamespace = "CompiledRazorTemplates.Dynamic";

        /// <summary>A prefix for all dynamically created classes.</summary>
        protected internal const string ClassNamePrefix = "RazorEngine_";

        /// <summary>Gets or sets the assembly resolver.</summary>
        public IReferenceResolver ReferenceResolver { get; set; }

        /// <summary>Gets the code language.</summary>
        public RazorCodeLanguage CodeLanguage
        {
            [SecurityCritical]
            get;
        }

        /// <summary>
        /// Gets or sets whether the compiler service is operating in debug mode.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets whether the compiler should load assemblies with Assembly.Load(byte[])
        /// to prevent files from being locked.
        /// </summary>
        public bool DisableTempFileLocking { get; set; }

        /// <summary>Gets the markup parser.</summary>
        public ParserBaseCreator MarkupParserFactory
        {
            [SecurityCritical]
            get;
        }

        /// <summary>
        /// Extension of a source file without dot ("cs" for C# files or "vb" for VB.NET files).
        /// </summary>
        public abstract string SourceFileExtension { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="CompilerServiceBase" />
        /// </summary>
        /// <param name="codeLanguage">The code language.</param>
        /// <param name="markupParserFactory">The markup parser factory.</param>
        [SecurityCritical]
        protected CompilerServiceBase(RazorCodeLanguage codeLanguage, ParserBaseCreator markupParserFactory)
        {
            this.CodeLanguage = codeLanguage;
            this.MarkupParserFactory = markupParserFactory ?? new ParserBaseCreator(null);
            this.ReferenceResolver = new UseCurrentAssembliesReferenceResolver();
            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        }

        [SecurityCritical]
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();
            var name = args.Name;
            return this.references
                .OfType<CompilerReference.DirectAssemblyReference>()
                .Where(ar => ar.Assembly.GetName().FullName == name)
                .Select(ar => ar.Assembly)
                .FirstOrDefault()
                   ?? this.references
                       .OfType<CompilerReference.FileReference>()
                       .Where(fr => AssemblyName.GetAssemblyName(fr.File).FullName == name)
                       .Select(fr => Assembly.LoadFrom(fr.File))
                       .FirstOrDefault();
        }

        /// <summary>
        /// Tries to create and return a unique temporary directory.
        /// </summary>
        /// <returns>the (already created) temporary directory</returns>
        protected static string GetDefaultTemporaryDirectory()
        {
            var flag = false;
            var num = 0;
            var path = "";
            while (!flag && num < 10)
            {
                ++num;
                try
                {
                    path = Path.Combine(Path.GetTempPath(), "RazorEngine_" + Path.GetRandomFileName());
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        flag = Directory.Exists(path);
                    }
                }
                catch (IOException)
                {
                    if (num > 8)
                        throw;
                }
            }
            if (!flag)
                throw new Exception("Could not create a temporary directory! Maybe all names are already used?");
            return path;
        }

        /// <summary>
        /// Returns a new temporary directory ready to be used.
        /// This can be overwritten in subclases to change the created directories.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTemporaryDirectory()
        {
            return GetDefaultTemporaryDirectory();
        }

        /// <summary>Builds a type name for the specified template type.</summary>
        /// <param name="templateType">The template type.</param>
        /// <param name="modelType">The model type.</param>
        /// <returns>The string type name (including namespace).</returns>
        public abstract string BuildTypeName(Type templateType, Type modelType);

        /// <summary>
        /// Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        [SecurityCritical]
        public abstract Tuple<Type, CompilationData> CompileType(TypeContext context);

        /// <summary>
        /// Creates a <see cref="RazorEngineHost" /> used for class generation.
        /// </summary>
        /// <param name="templateType">The template base type.</param>
        /// <param name="modelType">The model type.</param>
        /// <param name="className">The class name.</param>
        /// <returns>An instance of <see cref="RazorEngineHost" />.</returns>
        [SecurityCritical]
        private RazorEngineHost CreateHost(Type templateType, Type modelType, string className)
        {
            var razorEngineHost = new RazorEngineHost(this.CodeLanguage, this.MarkupParserFactory.Create)
                {
                    DefaultBaseTemplateType = templateType,
                    DefaultModelType = modelType,
                    DefaultBaseClass = this.BuildTypeName(templateType, modelType),
                    DefaultClassName = className,
                    DefaultNamespace = "CompiledRazorTemplates.Dynamic",
                    GeneratedClassContext = new GeneratedClassContext(
                        "Execute",
                        "Write",
                        "WriteLiteral",
                        "WriteTo",
                        "WriteLiteralTo",
                        "MicroRazorHost.Templating.TemplateWriter")
                        {
                            ResolveUrlMethodName = "ResolveUrl"
                        }
                };
            return razorEngineHost;
        }

        /// <summary>
        /// Gets the source code from Razor for the given template.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="template">The template to compile.</param>
        /// <param name="namespaceImports">The set of namespace imports.</param>
        /// <param name="templateType">The template type.</param>
        /// <param name="modelType">The model type.</param>
        /// <returns></returns>
        [SecurityCritical]
        public string GetCodeCompileUnit(
            string className,
            string template,
            ISet<string> namespaceImports,
            Type templateType,
            Type modelType)
        {
            return this.GetCodeCompileUnit(
                new TypeContext(className, namespaceImports)
                    {
                        TemplateContent = template,
                        TemplateType = templateType,
                        ModelType = modelType
                    });
        }

        /// <summary>Helper method to generate the prefered assembly name.</summary>
        /// <param name="context">the context of the current compilation.</param>
        /// <returns></returns>
        protected string GetAssemblyName(TypeContext context)
        {
            return $"{ "CompiledRazorTemplates.Dynamic"}.{ context.ClassName}";
        }

        /// <summary>Inspects the source and returns the source code.</summary>
        /// <param name="results"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [SecurityCritical]
        public abstract string InspectSource(GeneratorResults results, TypeContext context);

        /// <summary>Gets the code compile unit used to compile a type.</summary>
        /// <param name="context"></param>
        /// <returns>A <see cref="T:System.CodeDom.CodeCompileUnit" /> used to compile a type.</returns>
        [SecurityCritical]
        public string GetCodeCompileUnit(TypeContext context)
        {
            var className = context.ClassName;
            var templateContent = context.TemplateContent;
            var namespaces = context.Namespaces ?? new HashSet<string>();
            var modelType = context.ModelType;
            if (string.IsNullOrEmpty(className))
                throw new ArgumentException("Class name is required.");
            if (templateContent == null)
                throw new ArgumentException("Template is required.");
            var templateType = context.TemplateType ?? (modelType == null
                ? typeof(TemplateBase)
                : typeof(TemplateBase<>));
            var host = this.CreateHost(templateType, modelType, className);
            foreach (var @namespace in GetNamespaces(templateType, namespaces))
                host.NamespaceImports.Add(@namespace);
            return this.GetGeneratorResult(host, context);
        }

        /// <summary>Gets the generator result.</summary>
        /// <param name="host">The razor engine host.</param>
        /// <param name="context">The compile context.</param>
        /// <returns>The generator result.</returns>
        [SecurityCritical]
        private string GetGeneratorResult(RazorEngineHost host, TypeContext context)
        {
            GeneratorResults code;
            using (TextReader templateReader = new StringReader(context.TemplateContent))
                code = new RazorTemplateEngine(host).GenerateCode(templateReader, null, null, null);
            return this.InspectSource(code, context);
        }

        /// <summary>Gets any required namespace imports.</summary>
        /// <param name="templateType">The template type.</param>
        /// <param name="otherNamespaces">The requested set of namespace imports.</param>
        /// <returns>A set of namespace imports.</returns>
        private static IEnumerable<string> GetNamespaces(Type templateType, IEnumerable<string> otherNamespaces)
        {
            return templateType.GetCustomAttributes(typeof(RequireNamespacesAttribute), true)
                .Cast<RequireNamespacesAttribute>()
                .SelectMany(a => a.Namespaces)
                .Concat(otherNamespaces)
                .Distinct();
        }

        /// <summary>
        /// Returns a set of references that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of references.</returns>
        public virtual IEnumerable<CompilerReference> IncludeReferences()
        {
            return Enumerable.Empty<CompilerReference>();
        }

        /// <summary>
        /// Helper method to get all references for the given compilation.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected IEnumerable<CompilerReference> GetAllReferences(TypeContext context)
        {
            foreach (var reference in this.ReferenceResolver.GetReferences(context, this.IncludeReferences()))
            {
                this.references.Add(reference);
                yield return reference;
            }
        }

        /// <summary>
        /// This class only exists because we cannot use Func&lt;ParserBase&gt; in non security-critical class.
        /// </summary>
        [SecurityCritical]
        public class ParserBaseCreator
        {
            /// <summary>The parser creator.</summary>
            private readonly Func<ParserBase> creator;

            /// <summary>Create a new ParserBaseCreator instance.</summary>
            /// <param name="creator">The parser creator.</param>
            public ParserBaseCreator(Func<ParserBase> creator)
            {
                this.creator = creator ?? (() => new HtmlMarkupParser() as ParserBase);
            }

            /// <summary>Execute the given delegate.</summary>
            /// <returns></returns>
            public ParserBase Create()
            {
                return this.creator();
            }
        }
    }
}