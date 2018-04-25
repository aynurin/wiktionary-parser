using System.Linq;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextTemplateConverterTest : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertSimpleIPA()
        {
            var creole = "{{ipa|/boːk/|lang=li}}";
            var html = "<p>IPA: /boːk/</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertMultiIPA()
        {
            var creole = "{{ipa|/boːk/|[boːk]|lang=li}}";
            var html = "<p>IPA: /boːk/, [boːk]</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertNonGlossDefinition()
        {
            var creole = "{{lb|en|chiefly|nautical}} {{non-gloss definition|Short form of}} [[cat-o'-nine-tails]]";
            var html = "<p>(chiefly nautical) <em>Short form of</em> cat-o'-nine-tails</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertSense()
        {
            var creole = "{{sense|sense}}";
            var html = "<p>(<em>sense</em>):</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertGloss()
        {
            var creole = "{{gloss|gloss}}";
            var html = "<p>(gloss)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertNonGloss()
        {
            var creole = "{{non-gloss|non-gloss}}";
            var html = "<p><em>non-gloss</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertQualifier()
        {
            var creole = "{{qualifier|qualifier}}";
            var html = "<p>(<em>qualifier</em>)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertHomophones()
        {
            var creole = "{{homophones|broach|lang=en}}";
            var html = "<p>Homophone: broach</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertHomophonesMoreThanOne()
        {
            var creole = "{{homophones|bous|bout|lang=fr}}";
            var html = "<p>Homophones: bous, bout</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertCircaAntePost()
        {
            var creole = "{{circa|2002}} {{ante|abc}} {{post|1988}}";
            var html = "<p><em>c.</em> 2002 <em>a.</em> abc <em>p.</em> 1988</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertAudio()
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
