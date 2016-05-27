namespace RazorEngineHost.Compilation
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Web.Razor;
    using System.Web.Razor.Parser;

    /// <summary>
    /// Provides a base implementation of a direct compiler service.
    /// </summary>
    public abstract class DirectCompilerServiceBase : CompilerServiceBase, IDisposable
    {
        private readonly CodeDomProvider codeDomProvider;
        private bool disposed;

        /// <summary>The underlaying CodeDomProvider instance.</summary>
        public virtual CodeDomProvider CodeDomProvider => this.codeDomProvider;

        /// <summary>
        /// Initialises a new instance of <see cref="DirectCompilerServiceBase" />.
        /// </summary>
        /// <param name="codeLanguage">The razor code language.</param>
        /// <param name="codeDomProvider">The code dom provider used to generate code.</param>
        /// <param name="markupParserFactory">The markup parser factory.</param>
        [SecurityCritical]
        protected DirectCompilerServiceBase(
            RazorCodeLanguage codeLanguage,
            CodeDomProvider codeDomProvider,
            Func<ParserBase> markupParserFactory)
            : base(codeLanguage, new ParserBaseCreator(markupParserFactory))
        {
            this.codeDomProvider = codeDomProvider;
        }

        /// <summary>
        /// Creates the compile results for the specified <see cref="TypeContext" />.
        /// </summary>
        /// <param name="context">The type context.</param>
        /// <returns>The compiler results.</returns>
        [SecurityCritical]
        private Tuple<CompilerResults, string> Compile(TypeContext context)
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            var codeCompileUnit = this.GetCodeCompileUnit(context);
            var array = this.GetAllReferences(context)
                .Select(
                    a =>
                        a.GetFile(
                            msg =>
                                new ArgumentException(
                                    $"Unsupported CompilerReference given to CodeDom compiler (only references which can be resolved to files are supported: {msg})!")))
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToArray();
            var options = new CompilerParameters
                {
                    GenerateInMemory = false,
                    GenerateExecutable = false,
                    IncludeDebugInformation = this.Debug,
                    TreatWarningsAsErrors = false,
                    TempFiles = new TempFileCollection(this.GetTemporaryDirectory(), true),
                    CompilerOptions =
                        $"/target:library /optimize /define:RAZORENGINE {(array.Any(a => a.Contains("mscorlib.dll")) ? "/nostdlib" : "")}"
                };
            options.ReferencedAssemblies.AddRange(array);
            var fileName = Path.Combine(options.TempFiles.TempDir, $"{this.GetAssemblyName(context)}.dll");
            options.TempFiles.AddFile(fileName, true);
            options.OutputAssembly = fileName;
            var compilerResults = this.codeDomProvider.CompileAssemblyFromSource(options, codeCompileUnit);
            if (this.Debug)
            {
                var flag2 = false;
                var genTempPath = Path.Combine(
                    compilerResults.TempFiles.TempDir,
                    "generated_template." + this.SourceFileExtension);
                if (!File.Exists(genTempPath))
                {
                    File.WriteAllText(genTempPath, codeCompileUnit);
                    flag2 = true;
                }
                if (!flag2)
                {
                    foreach (string tempFile in compilerResults.TempFiles)
                    {
                        if (tempFile.EndsWith("." + this.SourceFileExtension))
                        {
                            File.Copy(tempFile, genTempPath, true);
                            break;
                        }
                    }
                }
            }
            return Tuple.Create(compilerResults, codeCompileUnit);
        }

        /// <summary>
        /// Inspects the GeneratorResults and returns the source code.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [SecurityCritical]
        public override string InspectSource(GeneratorResults results, TypeContext context)
        {
            foreach (var codeNamespace in results.GeneratedCode.Namespaces.Cast<CodeNamespace>().ToList())
            {
                foreach (var codeTypeDeclaration in codeNamespace.Types.Cast<CodeTypeDeclaration>().ToList())
                {
                    foreach (var codeTypeMember in codeTypeDeclaration.Members.Cast<CodeTypeMember>().ToList())
                    {
                        var snippetTypeMember = codeTypeMember as CodeSnippetTypeMember;
                        if (snippetTypeMember != null && snippetTypeMember.Text.Contains("#line hidden"))
                            snippetTypeMember.Text = snippetTypeMember.Text.Replace("#line hidden", "");
                    }
                }
            }
            var codeType = results.GeneratedCode.Namespaces[0].Types[0];
            //if (context.ModelType != null && CompilerServicesUtility.IsDynamicType(context.ModelType))
            //    codeType.CustomAttributes.Add(
            //        new CodeAttributeDeclaration(new CodeTypeReference(typeof (HasDynamicModelAttribute))));
            GenerateConstructors(CompilerServicesUtility.GetConstructors(context.TemplateType).ToArray(), codeType);
            //this.Inspect(results.GeneratedCode);
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                this.CodeDomProvider.GenerateCodeFromCompileUnit(
                    results.GeneratedCode,
                    stringWriter,
                    new CodeGeneratorOptions());
                return sb.ToString();
            }
        }

        /// <summary>
        /// Generates any required contructors for the specified type.
        /// </summary>
        /// <param name="constructors">The set of constructors.</param>
        /// <param name="codeType">The code type declaration.</param>
        private static void GenerateConstructors(
            ConstructorInfo[] constructors,
            CodeTypeDeclaration codeType)
        {
            if (constructors == null || !constructors.Any())
                return;
            foreach (var codeConstructor in codeType.Members.OfType<CodeConstructor>().ToArray())
                codeType.Members.Remove(codeConstructor);
            foreach (var constructor in constructors)
            {
                var codeConstructor = new CodeConstructor
                    {
                        Attributes = MemberAttributes.Public
                    };
                foreach (var parameter in constructor.GetParameters())
                {
                    codeConstructor.Parameters.Add(
                        new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name));
                    codeConstructor.BaseConstructorArgs.Add(new CodeSnippetExpression(parameter.Name));
                }
                codeType.Members.Add(codeConstructor);
            }
        }

        [SecurityCritical]
        private Tuple<Type, CompilationData> CompileTypeImpl(TypeContext context)
        {
            var tuple = this.Compile(context);
            var compilerResults = tuple.Item1;
            var files = compilerResults.TempFiles == null
                ? new CompilationData(tuple.Item2, null)
                : new CompilationData(tuple.Item2, compilerResults.TempFiles.TempDir);
            if (compilerResults.Errors != null && compilerResults.Errors.HasErrors)
                throw new TemplateCompilationException(
                    compilerResults.Errors.Cast<CompilerError>()
                        .Select(
                            error =>
                                new RazorEngineCompilerError(
                                    error.ErrorText,
                                    error.FileName,
                                    error.Line,
                                    error.Column,
                                    error.ErrorNumber,
                                    error.IsWarning))
                        .ToArray(),
                    files,
                    context.TemplateContent);
            var pathToAssembly = compilerResults.PathToAssembly;
            compilerResults.CompiledAssembly = !this.DisableTempFileLocking
                ? Assembly.LoadFile(pathToAssembly)
                : Assembly.Load(File.ReadAllBytes(pathToAssembly));
            var type = compilerResults.CompiledAssembly.GetType("CompiledRazorTemplates.Dynamic." + context.ClassName);
            if (type == null)
            {
                try
                {
                    compilerResults.CompiledAssembly.GetTypes();
                }
                catch (Exception ex)
                {
                    throw new TemplateLoadingException("Unable to load types of the loaded assembly", ex);
                }
                throw new TemplateLoadingException("We could not find the type in the compiled assembly!");
            }
            return Tuple.Create(type, files);
        }

        [SecurityCritical]
        private Tuple<Type, CompilationData> CompileType_Windows(TypeContext context)
        {
            var impersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
            try
            {
                return this.CompileTypeImpl(context);
            }
            finally
            {
                impersonationContext.Undo();
            }
        }

        /// <summary>
        /// Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        [SecurityCritical]
        public override Tuple<Type, CompilationData> CompileType(TypeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            new PermissionSet(PermissionState.Unrestricted).Assert();
            return !(Type.GetType("Mono.Runtime") != null)
                ? this.CompileType_Windows(context)
                : this.CompileTypeImpl(context);
        }

        /// <summary>Releases managed resourced used by this instance.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases managed resources used by this instance.</summary>
        /// <param name="disposing">Are we explicily disposing of this instance?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.disposed)
                return;
            this.codeDomProvider.Dispose();
            this.disposed = true;
        }
    }
}