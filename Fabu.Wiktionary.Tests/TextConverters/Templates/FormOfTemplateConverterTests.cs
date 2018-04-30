using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class FormOfTemplateConverterTests : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertSimpleAlternativeSpellingOf()
        {
            var creole = "{{alternative spelling of|swap|lang=en}}";
            var html = "<p><em>Alternative spelling of</em> swap.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void ShouldConvertSimpleAlternativeSpellingOfNoCapNoDot()
        {
            var creole = "{{alternative spelling of|swap|lang=en|nocap=1|nodot=1}}";
            var html = "<p><em>alternative spelling of</em> swap</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void ShouldConvertSimpleAlternativeSpellingOfTwoFromClauses()
        {
            var creole = "{{alternative spelling of|kilometre|from=US|from2=Canada|lang=en}}";
            var html = "<p><em>US and Canada spelling of</em> kilometre.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
