namespace RazorEngineHost.Compilation
{
    using System;
    using System.Security;
    using System.Web.Razor;
    using System.Web.Razor.Parser;

    /// <summary>Defines the custom razor engine host.</summary>
    [SecurityCritical]
    public class RazorEngineHost : System.Web.Razor.RazorEngineHost
    {
        /// <summary>Gets or sets the default template type.</summary>
        public Type DefaultBaseTemplateType { get; set; }

        /// <summary>Gets or sets the default model type.</summary>
        public Type DefaultModelType { get; set; }

        /// <summary>
        /// Initialises a new instance of <see cref="RazorEngineHost" />.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <param name="markupParserFactory">The markup parser factory delegate.</param>
        public RazorEngineHost(RazorCodeLanguage language, Func<ParserBase> markupParserFactory)
            :base(language, markupParserFactory)
        {
        }

        ///// <summary>Decorates the code parser.</summary>
        ///// <param name="incomingCodeParser">The code parser.</param>
        ///// <returns>The decorated parser.</returns>
        //[SecurityCritical]
        //public virtual ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        //{
        //    if (incomingCodeParser is CSharpCodeParser)
        //        return (ParserBase)new RazorEngineHost.Compilation.CSharp.CSharpCodeParser();
        //    if (incomingCodeParser is System.Web.Razor.Parser.VBCodeParser)
        //        return (ParserBase)new RazorEngineHost.Compilation.VisualBasic.VBCodeParser();
        //    return base.DecorateCodeParser(incomingCodeParser);
        //}
    }
}