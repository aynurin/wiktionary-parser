using System.Collections.Generic;
using Fabu.Wiktionary.TextConverters.Wiki;

namespace Fabu.Wiktionary.TextConverters
{
    public interface ITextConverter
    {
        FormattedString ConvertToStructured(ContextArguments args, string wikitext);
    }

    public class ContextArguments
    {
        public ContextArguments()
        {
        }

        public string PageTitle { get; set; }
        public string SectionName { get; set; }
    }

    /// <summary>
    /// Contains a formatted string that is easy to render as rich text label or as HTML on mobile devices, or serialize as JSON for storage
    /// </summary>
    public class FormattedString
    {
        private readonly string _data;
        private readonly List<Wiki.Proninciation> _proninciations;

        public FormattedString(string data, List<Wiki.Proninciation> proninciations)
        {
            _data = data;
            _proninciations = proninciations;
        }

        public List<Proninciation> Proninciations => _proninciations;

        public string ToHtml()
        {
            return _data;
        }
    }
}
