using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextTemplateConverterTest
    {
        private WikitextProcessor _converter = new WikitextProcessor(new Dictionary<string, string>()
        {
            { "en", "English" },
            { "la", "Latin" },
            { "ang", "Old English" }
        });

        private string Convert(string creole) => _converter.ConvertToStructured(creole).ToHtml();

        [Fact]
        public void ShouldConvertDerivedWithoutTranslation()
        {
            var creole = "from {{der|en|la|dictionarius}}, ";
            var html = "<p>from Latin dictionarius,</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertDerivedWithTranslation()
        {
            var creole = "from {{der|en|ang|frēo||free}}, ";
            var html = "<p>from Old English frēo (&ldquo;free&rdquo;),</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMentionWithTranslation()
        {
            var creole = "from {{m|la|dictio||speaking}},";
            var html = "<p>from dictio (&ldquo;speaking&rdquo;),</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMentionWithoutTranslation()
        {
            var creole = "from {{m|la|dictus}}";
            var html = "<p>from dictus</p>";
            Assert.Equal(html, Convert(creole));
        }
    }
}
