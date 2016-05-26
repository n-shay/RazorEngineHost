namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;

    public static class RazorEngineHostExtensions
    {
        /// <summary>
        /// See <see cref="M:RazorEngine.Templating.RazorEngineService.Run(RazorEngine.Templating.ITemplateKey,System.IO.TextWriter,System.Type,System.Object,RazorEngine.Templating.DynamicViewBag)" />.
        /// Convenience method which creates a <see cref="T:System.IO.TextWriter" /> and returns the result as string.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="compiledTemplate"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string Run(this IRazorEngineHost service, ICompiledTemplate compiledTemplate, Type modelType = null, object model = null)
        {
            return WithWriter(writer => service.Run(compiledTemplate, writer, modelType, model));
        }

        /// <summary>
        /// Convenience method which calls <see cref="IRazorEngineHost.RunCompile(string,System.IO.TextWriter,System.Type,System.Object)" />.
        /// <para>See <see cref="IRazorEngineHost.RunCompile(string,System.IO.TextWriter,System.Type,System.Object)" />.</para>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="templateSource"></param>
        /// <param name="name"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RunCompile(this IRazorEngineHost service, string templateSource, string name, Type modelType = null, object model = null)
        {
            return WithWriter(writer => service.RunCompile(templateSource, writer, modelType, model));
        }

        /// <summary>
        /// Helper method to provide a TextWriter and return the written data.
        /// Convenience method which creates a <see cref="System.IO.TextWriter" /> and returns the result as string.
        /// </summary>
        /// <param name="withWriter"></param>
        /// <returns></returns>
        private static string WithWriter(Action<TextWriter> withWriter)
        {
            using (var stringWriter = new StringWriter())
            {
                withWriter(stringWriter);
                return stringWriter.ToString();
            }
        }

    }
}