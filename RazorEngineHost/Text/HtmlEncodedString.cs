namespace RazorEngineHost.Text
{
    using System.Net;

    /// <summary>Represents a Html-encoded string.</summary>
    public class HtmlEncodedString : IEncodedString
    {
        private readonly string encodedString;

        /// <summary>
        /// Initialises a new instance of <see cref="HtmlEncodedString" />
        /// </summary>
        /// <param name="value">The raw string to be encoded.</param>
        public HtmlEncodedString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            this.encodedString = WebUtility.HtmlEncode(value);
        }

        /// <summary>Gets the encoded string.</summary>
        /// <returns>The encoded string.</returns>
        public string ToEncodedString()
        {
            return this.encodedString ?? string.Empty;
        }

        /// <summary>Gets the string representation of this instance.</summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return this.ToEncodedString();
        }
    }
}