using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class LabelTests : TestConverterFactory
    {
        [Fact]
        public void Simple()
        {
            var creole = "blabla {{lb|en|here we are}} albalb";
            var html = "<p>blabla (here we are) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Strange()
        {
            // I don't see 'sort' parameter documentation, but it renders somewhat like this
            var creole = "{{lb|ja|sort=うま|shogi}}";
            var html = "<p>(shogi)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Or()
        {
            var creole = "blabla {{lb|en|one|or|another}} albalb";
            var html = "<p>blabla (one or another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Underscore()
        {
            var creole = "blabla {{lb|en|one|_|another}} albalb";
            var html = "<p>blabla (one another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Also()
        {
            var creole = "blabla {{lb|en|one|also|another}} albalb";
            var html = "<p>blabla (one, also another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Ampersand()
        {
            var creole = "blabla {{lb|en|one|&|another}} albalb";
            var html = "<p>blabla (one and another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Semicolon()
        {
            var creole = "blabla {{lb|en|one|;|another}} albalb";
            var html = "<p>blabla (one; another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Extremely()
        {
            var creole = "blabla {{lb|en|one|extremely|another}} albalb";
            var html = "<p>blabla (one, extremely another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Category()
        {
            var creole = "blabla {{lb|en|one|humorously|another}} albalb";
            var html = "<p>blabla (one, humorously another) albalb</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
