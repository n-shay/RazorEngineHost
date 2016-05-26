namespace RazorEngineHost.Templating
{
    using System.IO;

    public interface ITemplate
    {
        /// <summary>Sets the internal template service.</summary>
        IInternalTemplateService InternalTemplateService { set; }

        /// <summary>Set the model of the template (if applicable).</summary>
        /// <param name="model"></param>
        void SetModel(object model);

        /// <summary>Executes the compiled template.</summary>
        void Execute();

        /// <summary>Runs the template and returns the result.</summary>
        /// <param name="writer"></param>
        /// <returns>The merged result of the template.</returns>
        void Run(TextWriter writer);

        /// <summary>Writes the specified object to the result.</summary>
        /// <param name="value">The value to write.</param>
        void Write(object value);

        /// <summary>Writes the specified string to the result.</summary>
        /// <param name="literal">The literal to write.</param>
        void WriteLiteral(string literal);
    }
}