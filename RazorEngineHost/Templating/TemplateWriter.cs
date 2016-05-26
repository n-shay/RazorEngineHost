namespace RazorEngineHost.Templating
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>Defines a template writer used for helper templates.</summary>
    public class TemplateWriter
    {
        private readonly Action<TextWriter> writerDelegate;

        /// <summary>
        /// Initialises a new instance of <see cref="TemplateWriter" />.
        /// </summary>
        /// <param name="writer">The writer delegate used to write using the specified <see cref="TextWriter" />.</param>
        public TemplateWriter(Action<TextWriter> writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            this.writerDelegate = writer;
        }

        /// <summary>
        /// Executes the write delegate and returns the result of this <see cref="TemplateWriter" />.
        /// </summary>
        /// <returns>The string result of the helper template.</returns>
        public override string ToString()
        {
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.writerDelegate(stringWriter);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Writes the helper result of the specified text writer.
        /// </summary>
        /// <param name="writer">The text writer to write the helper result to.</param>
        public void WriteTo(TextWriter writer)
        {
            this.writerDelegate(writer);
        }
    }
}