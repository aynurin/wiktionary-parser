using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class AbbreviationTests : TestConverterFactory
    {
        [Fact]
        public void Example()
        {
            var creole = "{{abbreviation of|מִכָּל מָקוֹם|tr=mikól makóm|dot=:|lang=he}} [[nevertheless]]";
            var html = "<p><em>Abbreviation of</em> מִכָּל מָקוֹם (<em>mikól makóm</em>): nevertheless</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Category()
        {
            var creole = "{{abbreviation of|category|lang=en|cap=brrr}}";
            var html = "<p><em>brrr</em> category.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Caterpillar()
        {
            var creole = "{{abbreviation of|lang=en|caterpillar|nocap=1}}";
            var html = "<p><em>abbreviation of</em> caterpillar.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
