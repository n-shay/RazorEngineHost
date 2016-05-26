namespace RazorEngineHost.Templating
{
    using System;

    /// <summary>
    /// Provides a base implementation of a template with a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    public class TemplateBase<TModel> : TemplateBase, ITemplate<TModel>
    {
        internal override Type ModelType => typeof (TModel);

        /// <summary>Gets or sets the model.</summary>
        public TModel Model { get; private set; }

        /// <summary>
        /// Initialises a new instance of <see cref="TemplateBase{TModel}" />.
        /// </summary>
        protected TemplateBase()
        {
        }

        /// <summary>Set the model.</summary>
        /// <param name="model"></param>
        public override void SetModel(object model)
        {
            this.Model = DynamicData.ToDynamic(model);
        }
    }
}