namespace RazorEngineHost.Templating
{
    using global::RazorEngineHost.Configuration;
    using global::RazorEngineHost.Text;

    /// <summary>
    /// A internal contract for the <see cref="TemplateBase" /> class.
    /// </summary>
    public interface IInternalTemplateService
    {
        /// <summary>Gets the template service configuration.</summary>
        IEngineConfiguration Configuration { get; }

        /// <summary>Gets the encoded string factory.</summary>
        IEncodedStringFactory EncodedStringFactory { get; }

        /// <summary>
        /// Adds a namespace that will be imported into the template.
        /// </summary>
        /// <param name="ns">The namespace to be imported.</param>
        void AddNamespace(string ns);
    }
}