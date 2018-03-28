using Fabu.Wiktionary.TextConverters.Wiki;

namespace Fabu.Wiktionary.TextConverters
{
    public interface ITextConverter
    {
        FormattedString ConvertToStructured(string wikitext);
    }

    /// <summary>
    /// Contains a formatted string that is easy to render as rich text label or as HTML on mobile devices, or serialize as JSON for storage
    /// </summary>
    public class FormattedString
    {
        private readonly string _data;

        public FormattedString(string data)
        {
            _data = data;
        }

        public string ToHtml()
        {
            return _data;
            ;
        }
    }
}
