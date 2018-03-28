using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.WikiMup;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikiMupConverterTest : BaseTextConverterTest
    {
        protected override ITextConverter GetConverter() => new WikiMupConverter();
    }
}
