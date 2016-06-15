namespace RazorEngineHost.Templating
{
    /// <summary>
    /// Defines the required contract for implementing a template with a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    public interface ITemplate<out TModel> : ITemplate
    {
        /// <summary>Gets the or sets the model.</summary>
        dynamic Model { get; }
    }
}