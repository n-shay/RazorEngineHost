namespace RazorEngineHost.Text
{
    /// <summary>
    /// Defines the required contract for implementing an encoded string.
    /// </summary>
    public interface IEncodedString
    {
        /// <summary>Gets the encoded string.</summary>
        /// <returns>The encoded string.</returns>
        string ToEncodedString();
    }
}