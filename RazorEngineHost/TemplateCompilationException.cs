namespace RazorEngineHost
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;

    using RazorEngineHost.Compilation;
    using RazorEngineHost.Compilation.ReferenceResolver;

    /// <summary>
    /// Defines an exception that occurs during compilation of the template.
    /// </summary>
    [Serializable]
    public class TemplateCompilationException : Exception
    {
        /// <summary>Gets the set of compiler errors.</summary>
        public ReadOnlyCollection<RazorEngineCompilerError> CompilerErrors { get; }

        /// <summary>Gets some copilation specific (temporary) data.</summary>
        public CompilationData CompilationData { get; }

        /// <summary>Gets the generated source code.</summary>
        public string SourceCode => this.CompilationData.SourceCode;

        /// <summary>Gets the source template that wasn't compiled.</summary>
        public string Template { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="T:RazorEngine.Templating.TemplateCompilationException" />.
        /// </summary>
        /// <param name="errors">The set of compiler errors.</param>
        /// <param name="files">The source code that wasn't compiled.</param>
        /// <param name="template">The source template that wasn't compiled.</param>
        public TemplateCompilationException(IEnumerable<RazorEngineCompilerError> errors, CompilationData files, string template)
          : base(GetMessage(errors, files, template))
        {
            this.CompilerErrors = new ReadOnlyCollection<RazorEngineCompilerError>(errors.ToList());
            this.CompilationData = files;
            this.Template = template;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="T:RazorEngine.Templating.TemplateCompilationException" /> from serialised data.
        /// </summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        protected TemplateCompilationException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
            var int32 = info.GetInt32("Count");
            var engineCompilerErrorList = new List<RazorEngineCompilerError>();
            var type = typeof(RazorEngineCompilerError);
            for (var index = 0; index < int32; ++index)
                engineCompilerErrorList.Add((RazorEngineCompilerError)info.GetValue("CompilerErrors[" + index + "]", type));
            this.CompilerErrors = new ReadOnlyCollection<RazorEngineCompilerError>(engineCompilerErrorList);
            var sourceCode = info.GetString("SourceCode");
            if (string.IsNullOrEmpty(sourceCode))
                sourceCode = null;
            var tmpFolder = info.GetString("TmpFolder");
            if (string.IsNullOrEmpty(tmpFolder))
                tmpFolder = null;
            this.CompilationData = new CompilationData(sourceCode, tmpFolder);
            this.Template = info.GetString("Template");
        }

        internal static string Separate(string rawLines)
        {
            return $"\n------------- START -----------\n{ rawLines}\n------------- END -----------\n";
        }

        /// <summary>
        /// Gets a exact error message of the given error collection
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="files"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        internal static string GetMessage(IEnumerable<RazorEngineCompilerError> errors, CompilationData files, string template)
        {
            var str1 = string.Join("\n\t", errors.Select(error =>
                $" - {(error.IsWarning ? "warning" : "error")}: ({ error.Line}, { error.Column}) { error.ErrorText}"));
            string str2 =
                $"The template we tried to compile is: { Separate(template ?? string.Empty)}\n";
            var str3 = string.Empty;
            if (files.TmpFolder != null)
                str3 =
                    $"Temporary files of the compilation can be found in (please delete the folder): { files.TmpFolder}\n";
            var str4 = string.Empty;
            if (files.SourceCode != null)
                str4 =
                    $"The generated source code is: { Separate(files.SourceCode)}\n";
            var str5 = "\nList of loaded Assemblies:\n" + string.Join("\n\tLoaded Assembly: ", new UseCurrentAssembliesReferenceResolver().GetReferences().Select(r => r.GetFile()));
            return
                "Errors while compiling a Template.\nPlease try the following to solve the situation:\n" +
                "  * If the problem is about missing/invalid references or multiple defines either try to load \n" +
                "    the missing references manually (in the compiling appdomain!) or\n" +
                "    Specify your references manually by providing your own IReferenceResolver implementation.\n" +
                "    See https://antaris.github.io/RazorEngine/ReferenceResolver.html for details.\n" +
                "    Currently all references have to be available as files!\n" +
                "  * If you get 'class' does not contain a definition for 'member': \n" +
                "        try another modelType (for example 'null' to make the model dynamic).\n" +
                "        NOTE: You CANNOT use typeof(dynamic) to make the model dynamic!\n" +
                "    Or try to use static instead of anonymous/dynamic types.\n" +
                "More details about the error:\n" +
                $"{ str1}\n" +
                $"{ str3}{ str2}{ str4}{ str5}";
        }

        /// <summary>Gets the object data for serialisation.</summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Count", this.CompilerErrors.Count);
            for (var index = 0; index < this.CompilerErrors.Count; ++index)
                info.AddValue("CompilerErrors[" + index + "]", this.CompilerErrors[index]);
            info.AddValue("SourceCode", this.CompilationData.SourceCode ?? string.Empty);
            info.AddValue("TmpFolder", this.CompilationData.TmpFolder ?? string.Empty);
            info.AddValue("Template", this.Template ?? string.Empty);
        }
    }
}