namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;

    public static class RazorEngineHostExtensions
    {
        /// <summary>
        ///  Convenience method which creates a <see cref="T:System.IO.TextWriter" /> and returns the result as string.
        /// <para>See <see cref="RazorEngineHost.Run" />.</para>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="templateContext"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        /// <param name="configTemplateData"></param>
        /// <returns></returns>
        public static string Run(
            this IRazorEngineHost service,
            TemplateContext templateContext,
            Type modelType = null,
            object model = null,
            Action<dynamic> configTemplateData = null)
        {
            return WithWriter(writer => service.Run(templateContext, writer, modelType, model, configTemplateData));
        }

        /// <summary>
        /// Convenience method which calls <see cref="IRazorEngineHost.RunCompile" />.
        /// <para>See <see cref="IRazorEngineHost.RunCompile" />.</para>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="templateSource"></param>
        /// <param name="name"></param>
        /// <param name="modelType"></param>
        /// <param name="model"></param>
        /// <param name="configTemplateData"></param>
        /// <returns></returns>
        public static string RunCompile(
            this IRazorEngineHost service,
            string templateSource,
            string name,
            Type modelType = null,
            object model = null,
            Action<dynamic> configTemplateData = null)
        {
            return
                WithWriter(writer => service.RunCompile(templateSource, writer, modelType, model, configTemplateData));
        }

        /// <summary>
        /// Helper method to provide a TextWriter and return the written data.
        /// Convenience method which creates a <see cref="TextWriter" /> and returns the result as string.
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