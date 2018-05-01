using System.Linq;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class OtherTest : TestConverterFactory
    {
        [Fact]
        public void SimpleIPA()
        {
            var creole = "{{ipa|/boːk/|lang=li}}";
            var html = "<p>IPA: /boːk/</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void MultiIPA()
        {
            var creole = "{{ipa|/boːk/|[boːk]|lang=li}}";
            var html = "<p>IPA: /boːk/, [boːk]</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void NonGlossDefinition()
        {
            var creole = "{{lb|en|chiefly|nautical}} {{non-gloss definition|Short form of}} [[cat-o'-nine-tails]]";
            var html = "<p>(chiefly nautical) <em>Short form of</em> cat-o'-nine-tails</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Sense()
        {
            var creole = "{{sense|sense}}";
            var html = "<p>(<em>sense</em>):</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Gloss()
        {
            var creole = "{{gloss|gloss}}";
            var html = "<p>(gloss)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void NonGloss()
        {
            var creole = "{{non-gloss|non-gloss}}";
            var html = "<p><em>non-gloss</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Qualifier()
        {
            var creole = "{{qualifier|qualifier}}";
            var html = "<p>(<em>qualifier</em>)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Homophones()
        {
            var creole = "{{homophones|broach|lang=en}}";
            var html = "<p>Homophone: broach</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void HomophonesMoreThanOne()
        {
            var creole = "{{homophones|bous|bout|lang=fr}}";
            var html = "<p>Homophones: bous, bout</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void CircaAntePost()
        {
            var creole = "{{circa|2002}} {{ante|abc}} {{post|1988}}";
            var html = "<p><em>c.</em> 2002 <em>a.</em> abc <em>p.</em> 1988</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ISBN()
        {
            var creole = "{{ISBN|1841692336}}";
            var html = "<p>ISBN 1841692336</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void InitialismOf()
        {
            var creole = "{{initialism of|{{pedlink|GNU Free Documentation License}}|lang=en}}";
            var html = "<p><em>Initialism of</em> GNU Free Documentation License.</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void InitialismOfNoCapNoDot()
        {
            var creole = "{{initialism of|{{pedlink|GNU Free Documentation License}}|lang=en|nocap=1|nodot=1}}";
            var html = "<p><em>initialism of</em> GNU Free Documentation License</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Audio()
        {
            /*
      "dictionary:{{audio|en-us-dictionary.ogg|Audio (US)|lang=en}}",
      "dictionary:{{audio|en-uk-dictionary.ogg|Audio (UK)|lang=en}}",
      "free:{{audio|en-us-free.ogg|Audio (US)|lang=en}}",
      "free:{{audio|En-uk-free.ogg|Audio (UK)|lang=en}}",
      "encyclopedia:{{audio|en-us-encyclopedia.ogg|Audio (US)|lang=en}}"*/

            var creole = ", {{audio|en-us-dictionary.ogg|Audio (US)|lang=en}}.";
            var html = "<p>, .</p>";
            var formatted = Convert(creole, sectionName: "Pronunciation");
            Assert.Equal(html, formatted.ToHtml());
            Assert.Single(formatted.Proninciations);
            var pron = formatted.Proninciations.First();
            Assert.Equal("Audio (US)", pron.Label);
            Assert.Equal("English", pron.Language);
            Assert.Equal("en-us-dictionary.ogg", pron.FileName);
        }
    }
}
