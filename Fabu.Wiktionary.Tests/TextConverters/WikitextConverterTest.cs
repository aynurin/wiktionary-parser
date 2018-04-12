using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System.Collections.Generic;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextConverterTest : BaseTextConverterTest
    {
        protected override ITextConverter GetConverter() => new WikitextProcessor(new Dictionary<string, string>()
        {
            { "en", "English" }
        });
    }
}
