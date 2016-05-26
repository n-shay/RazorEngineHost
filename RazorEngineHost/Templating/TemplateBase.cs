namespace RazorEngineHost.Templating
{
    using System;
    using System.IO;
    using System.Text;

    using global::RazorEngineHost.Text;

    public abstract class TemplateBase : ITemplate
    {
        internal TextWriter CurrentWriter { get; set; }

        /// <summary>Gets or sets the template service.</summary>
        public IInternalTemplateService InternalTemplateService { internal get; set; }

        internal virtual Type ModelType => null;

        public virtual void SetModel(object model)
        {
        }

        /// <summary>Executes the compiled template.</summary>
        public virtual void Execute()
        {
        }

        public void Run(TextWriter writer)
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            {
                this.CurrentWriter = stringWriter;
                this.Execute();
                stringWriter.Flush();
                this.CurrentWriter = null;
                writer.Write(builder.ToString());
            }
        }

        /// <summary>Writes the specified object to the result.</summary>
        /// <param name="value">The value to write.</param>
        public virtual void Write(object value)
        {
            this.WriteTo(this.CurrentWriter, value);
        }

        /// <summary>Writes the specified template helper result.</summary>
        /// <param name="helper">The template writer helper.</param>
        public virtual void Write(TemplateWriter helper)
        {
            helper?.WriteTo(this.CurrentWriter);
        }
        
        /// <summary>Writes an attribute to the result.</summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="values"></param>
        public virtual void WriteAttribute(string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
        {
            this.WriteAttributeTo(this.CurrentWriter, name, prefix, suffix, values);
        }

        /// <summary>
        /// Writes an attribute to the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The name of the attribute to be written.</param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="values"></param>
        public virtual void WriteAttributeTo(TextWriter writer, string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
        {
            var flag1 = true;
            var flag2 = false;
            if (values.Length == 0)
            {
                this.WritePositionTaggedLiteral(writer, prefix);
                this.WritePositionTaggedLiteral(writer, suffix);
            }
            else
            {
                foreach (var attributeValue in values)
                {
                    var positionTagged = attributeValue.Value;
                    var nullable = new bool?();
                    if (positionTagged.Value is bool)
                        nullable = (bool)positionTagged.Value;
                    if (positionTagged.Value != null && (!nullable.HasValue || nullable.Value))
                    {
                        string literal;
                        if ((literal = positionTagged.Value as string) == null)
                            literal = positionTagged.Value.ToString();
                        if (nullable.HasValue)
                            literal = name;
                        if (flag1)
                        {
                            this.WritePositionTaggedLiteral(writer, prefix);
                            flag1 = false;
                        }
                        else
                            this.WritePositionTaggedLiteral(writer, attributeValue.Prefix);
                        if (attributeValue.Literal)
                            this.WriteLiteralTo(writer, literal);
                        else if (positionTagged.Value is IEncodedString)
                            this.WriteTo(writer, positionTagged.Value);
                        else
                            this.WriteTo(writer, literal);
                        flag2 = true;
                    }
                }
                if (!flag2)
                    return;
                this.WritePositionTaggedLiteral(writer, suffix);
            }
        }

        /// <summary>Writes the specified string to the result.</summary>
        /// <param name="literal">The literal to write.</param>
        public virtual void WriteLiteral(string literal)
        {
            this.WriteLiteralTo(this.CurrentWriter, literal);
        }

        /// <summary>
        /// Writes a string literal to the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="literal">The literal to be written.</param>
        public virtual void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (literal == null)
                return;
            writer.Write(literal);
        }

        /// <summary>
        /// Writes a <see cref="T:RazorEngine.PositionTagged`1" /> literal to the result.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The literal to be written.</param>
        private void WritePositionTaggedLiteral(TextWriter writer, PositionTagged<string> value)
        {
            this.WriteLiteralTo(writer, value.Value);
        }

        /// <summary>
        /// Writes the specified object to the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value to be written.</param>
        public virtual void WriteTo(TextWriter writer, object value)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (value == null)
                return;
            if (value is IEncodedString)
            {
                writer.Write(value);
            }
            else
            {
                var encodedString = this.InternalTemplateService.EncodedStringFactory.CreateEncodedString(value);
                writer.Write(encodedString);
            }
        }

        /// <summary>
        /// Writes the specfied template helper result to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="helper">The template writer helper.</param>
        public virtual void WriteTo(TextWriter writer, TemplateWriter helper)
        {
            helper?.WriteTo(writer);
        }
        
        /// <summary>Resolves the specified path</summary>
        /// <param name="path">The path.</param>
        /// <returns>The resolved path.</returns>
        public virtual string ResolveUrl(string path)
        {
            if (path.StartsWith("~"))
                path = path.Substring(1);
            return path;
        }
    }
}
