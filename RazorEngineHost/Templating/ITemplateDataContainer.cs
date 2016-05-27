namespace RazorEngineHost.Templating
{
    public interface ITemplateDataContainer
    {
        /// <summary>
        /// Gets the dynamic template data.
        /// </summary>
        dynamic TemplateData { get; }
    }
}