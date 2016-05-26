namespace RazorEngineHost.Compilation
{
    using System;
    using System.Security;

    /// <summary>
    /// Provides a default implementation of a <see cref="T:ICompilerServiceFactory" />.
    /// </summary>
    [Serializable]
    public class DefaultCompilerServiceFactory : ICompilerServiceFactory
    {
        /// <summary>
        /// Creates a <see cref="ICompilerService" /> that supports the specified language.
        /// </summary>
        /// <param name="language">The <see cref="T:RazorEngine.Language" />.</param>
        /// <returns>An instance of <see cref="ICompilerService" />.</returns>
        [SecuritySafeCritical]
        public ICompilerService CreateCompilerService(Language language)
        {
            if (language == Language.CSharp)
                return new CSharpDirectCompilerService();
            //if (language == Language.VisualBasic)
                //return (ICompilerService)new VBDirectCompilerService(true, (Func<ParserBase>)null);
            throw new ArgumentException("Unsupported language: " + language);
        }
    }
}