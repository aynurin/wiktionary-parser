using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class LabelTemplateConverterTests : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertSimpleLabel()
        {
            var creole = "blabla {{lb|en|here we are}} albalb";
            var html = "<p>blabla (here we are) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertStrangeLabel()
        {
            // I don't see 'sort' parameter documentation, but it renders somewhat like this
            var creole = "{{lb|ja|sort=うま|shogi}}";
            var html = "<p>(shogi)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertOrLabel()
        {
            var creole = "blabla {{lb|en|one|or|another}} albalb";
            var html = "<p>blabla (one or another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertUnderscoreLabel()
        {
            var creole = "blabla {{lb|en|one|_|another}} albalb";
            var html = "<p>blabla (one another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertAlsoLabel()
        {
            var creole = "blabla {{lb|en|one|also|another}} albalb";
            var html = "<p>blabla (one, also another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertAmpersandLabel()
        {
            var creole = "blabla {{lb|en|one|&|another}} albalb";
            var html = "<p>blabla (one and another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertSemicolonLabel()
        {
            var creole = "blabla {{lb|en|one|;|another}} albalb";
            var html = "<p>blabla (one; another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertExtremelyLabel()
        {
            var creole = "blabla {{lb|en|one|extremely|another}} albalb";
            var html = "<p>blabla (one, extremely another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertCategoryLabel()
        {
            var creole = "blabla {{lb|en|one|humorously|another}} albalb";
            var html = "<p>blabla (one, humorously another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
