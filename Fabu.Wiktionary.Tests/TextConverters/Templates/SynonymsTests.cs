using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class SynonymsTests : TestConverterFactory
    {
        [Fact]
        public void Synonym()
        {
            var creole = "{{synonyms|en|library}}";
            var html = "<p>Synonyms: library.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SynSeveral()
        {
            var creole = "{{syn|en|unconstrained|unfettered|unhindered}}";
            var html = "<p>Synonyms: unconstrained, unfettered, unhindered.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Antonyms()
        {
            var creole = "{{ant|en|below|beneath|alt1=Below|tr2=бенис|q1=xyz}}";
            var html = "<p>Antonyms: Below (<em>xyz</em>), beneath (бенис).</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Hypernyms()
        {
            var creole = "{{hyper|en|below|beneath|alt1=Below|tr1=белоу|q1=xyz}}";
            var html = "<p>Hypernyms: Below (белоу) (<em>xyz</em>), beneath.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
