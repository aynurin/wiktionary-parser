using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class PluralOfTests : TestConverterFactory
    {
        [Fact]
        public void Irregular()
        {
            var creole = "{{en-irregular plural of|foot}}";
            var html = "<p><em>Plural of</em> foot.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Regular()
        {
            var creole = "{{plural of|pie|lang=en}}";
            var html = "<p><em>Plural of</em> pie.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Alternative()
        {
            var creole = "{{alternative plural of|kilometre|kilometers}}";
            var html = "<p><em>Plural of</em> kilometre (<em>alternative of</em> kilometers).</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
