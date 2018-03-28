using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextConverterTest : BaseTextConverterTest
    {
        protected override ITextConverter GetConverter() => new WikitextConverter();
    }
}
