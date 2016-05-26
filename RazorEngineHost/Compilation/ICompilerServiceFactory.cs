namespace RazorEngineHost.Compilation
{
    /// <summary>
    /// Defines the required contract for implementing a compiler service factory.
    /// </summary>
    public interface ICompilerServiceFactory
    {
        /// <summary>
        /// Creates a <see cref="T:ICompilerService" /> that supports the specified language.
        /// </summary>
        /// <param name="language">The <see cref="T:MicroRazorHost.Language" />.</param>
        /// <returns>An instance of <see cref="T:ICompilerService" />.</returns>
        ICompilerService CreateCompilerService(Language language);
    }
}