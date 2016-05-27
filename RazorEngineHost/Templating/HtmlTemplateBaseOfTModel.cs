namespace RazorEngineHost.Templating
{
    /// <summary>
    /// Provides a base implementation of an html template with a model.
    /// </summary>
    /// <remarks>
    /// This type does not currently serve a purpose, and the WriteAttribute* API has been migrated to the TemplateBase type. This type is not deprecated, as it
    /// may form the basis for a future template that supports MVC like @Html syntax.
    /// </remarks>
    /// <typeparam name="TModel">The model type.</typeparam>
    public class HtmlTemplateBase<TModel> : TemplateBase<TModel>
    {
        public HtmlHelper Html { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            this.Html = new HtmlHelper();
        }
    }
}