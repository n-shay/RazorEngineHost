namespace RazorEngineHost.Text
{
    /// <summary>Represents an unencoded string.</summary>
    public class RawString : IEncodedString
    {
        private readonly string value;

        /// <summary>
        /// Initialises a new instance of <see cref="RawString" />
        /// </summary>
        /// <param name="value">The value</param>
        public RawString(string value)
        {
            this.value = value;
        }

        /// <summary>Gets the encoded string.</summary>
        /// <returns>The encoded string.</returns>
        public string ToEncodedString()
        {
            return this.value ?? string.Empty;
        }

        /// <summary>Gets the string representation of this instance.</summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return this.ToEncodedString();
        }
    }
}