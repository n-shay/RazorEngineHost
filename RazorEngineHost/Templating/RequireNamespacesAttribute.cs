namespace RazorEngineHost.Templating
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Allows base templates to define require template imports when
    /// generating templates.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class RequireNamespacesAttribute : Attribute
    {
        /// <summary>Gets the set of required namespace imports.</summary>
        public IEnumerable<string> Namespaces { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="T:RazorEngine.Templating.RequireNamespacesAttribute" />.
        /// </summary>
        /// <param name="namespaces">The set of required namespace imports.</param>
        public RequireNamespacesAttribute(params string[] namespaces)
        {
            if (namespaces == null)
                throw new ArgumentNullException(nameof(namespaces));
            var stringSet = new HashSet<string>();
            foreach (var @namespace in namespaces)
                stringSet.Add(@namespace);
            this.Namespaces = stringSet;
        }
    }
}