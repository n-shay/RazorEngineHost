namespace RazorEngineHost.Compilation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a type context that describes a template to compile.
    /// </summary>
    public class TypeContext
    {
        /// <summary>Gets the class name.</summary>
        public string ClassName { get; private set; }

        /// <summary>Gets or sets the model type.</summary>
        public Type ModelType { get; set; }

        /// <summary>Gets the set of namespace imports.</summary>
        public ISet<string> Namespaces { get; private set; }

        /// <summary>Gets or sets the template content.</summary>
        public string TemplateContent { get; set; }

        /// <summary>Gets or sets the base template type.</summary>
        public Type TemplateType { get; set; }

        /// <summary>
        /// Initialises a new instance of <see cref="TypeContext" />.
        /// </summary>
        internal TypeContext()
        {
            this.ClassName = CompilerServicesUtility.GenerateClassName();
            this.Namespaces = new HashSet<string>();
        }

        /// <summary>
        /// Creates a new TypeContext instance with the given classname and the given namespaces.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="namespaces"></param>
        internal TypeContext(string className, ISet<string> namespaces)
        {
            this.ClassName = className;
            this.Namespaces = namespaces;
        }
    }
}