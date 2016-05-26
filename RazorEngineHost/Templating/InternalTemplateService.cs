namespace RazorEngineHost.Templating
{
    using global::RazorEngineHost.Configuration;
    using global::RazorEngineHost.Text;

    internal class InternalTemplateService : IInternalTemplateService
    {
        private readonly RazorEngineCore service;

        public IEngineConfiguration Configuration => this.service.Configuration;

        /// <summary>Gets the encoded string factory.</summary>
        public IEncodedStringFactory EncodedStringFactory => this.service.Configuration.EncodedStringFactory;

        public InternalTemplateService(RazorEngineCore service)
        {
            this.service = service;
        }

        /// <summary>
        /// Adds a namespace that will be imported into the template.
        /// </summary>
        /// <param name="ns">The namespace to be imported.</param>
        public void AddNamespace(string ns)
        {
            this.Configuration.Namespaces.Add(ns);
        }
    }
}