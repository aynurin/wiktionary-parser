using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class TenseFaceOfTests : TestConverterFactory
    {
        [Fact]
        public void ThirdPersonSingularOf()
        {
            var creole = "{{en-third-person singular of|pie}}";
            var html = "<p><em>Third-person singular simple present indicative of</em> pie.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void ArchaicThirdPersonSingularOf()
        {
            var creole = "{{en-archaic third-person singular of|have}}";
            var html = "<p><em>(archaic) Third-person singular simple present indicative of</em> have.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void PastOf()
        {
            var creole = "{{en-past of|lionize}}";
            var html = "<p><em>Simple past and past participle of</em> lionize.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SimplePastOf()
        {
            var creole = "{{en-simple past of|weave}}";
            var html = "<p><em>Simple past of</em> weave.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void PresentParticipleOf()
        {
            var creole = "{{present participle of|absorb|lang=en|nocat=1}}";
            var html = "<p><em>Present participle of</em> absorb.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
