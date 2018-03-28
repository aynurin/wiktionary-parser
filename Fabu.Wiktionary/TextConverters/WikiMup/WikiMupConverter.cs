using Fabu.Wiktionary.TextConverters.Wiki;
using Mup;
using MwParserFromScratch;
using System.IO;
using System.Text;

namespace Fabu.Wiktionary.TextConverters.WikiMup
{
    public class WikiMupConverter : ITextConverter
    {
        public FormattedString ConvertToStructured(string wikitext)
        {
            if (wikitext == null)
                return null;

            var parser = new CreoleParser();
            var parseTree = parser.Parse(wikitext);
            var htmlWriterVisitor = new HtmlWriterVisitor();
            var html = parseTree.Accept(htmlWriterVisitor);
            return new FormattedString(html);
        }
    }
}
