namespace RazorEngineHost.Templating
{
    using global::RazorEngineHost.Text;

    public class HtmlHelper
    {
        public IEncodedString Raw(string rawString)
        {
            return new RawString(rawString);
        }

        public IEncodedString Raw(object value)
        {
            return new RawString(value?.ToString());
        }

        public IEncodedString Encode(string value)
        {
            return new HtmlEncodedString(value);
        }

        public IEncodedString Encode(object value)
        {
            return new HtmlEncodedString(value?.ToString());
        }

    }
}