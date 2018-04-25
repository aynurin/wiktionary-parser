using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class MentionsTemplateConverterTests : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertInheritedWithoutTranslation()
        {
            var creole = "From {{inh|nds|gml|vrîe}}, variant";
            var html = "<p>From Middle Low German <em>vrîe</em>, variant</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertInheritedWithTranslation()
        {
            var creole = "From {{inh|de|gmh}}, {{inh|de|goh|frī}}, from {{inh|de|gem-pro|*frijaz}}, From {{inh|pdc|goh|frī}}";
            var html = "<p>From Middle High German, Old High German <em>frī</em>, from Proto-Germanic <em>*frijaz</em>, From Old High German <em>frī</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertMixedMentions()
        {
            var creole = "From {{inh|nds|gml|vrîe}}, variant of {{m|gml|vrî}}, from {{der|nds|osx|frī}}, from {{der|nds|gem-pro|*frijaz}}, from {{der|nds|ine-pro|*prey||new}}. Compare Dutch {{m|nl|vrij}}, West Frisian {{m|fy|frij}}, English {{m|en|free}}, German {{m|de|frei}}.";
            var html = "<p>From Middle Low German <em>vrîe</em>, variant of vrî, from Old Saxon <em>frī</em>, from Proto-Germanic <em>*frijaz</em>, from Proto-Indo-European <em>*prey</em> (&ldquo;new&rdquo;). Compare Dutch vrij, West Frisian frij, English free, German frei.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertEmptyMentionsWithTranscript()
        {
            var creole = "\"Nubian {{m|onw|tr=kadīs}}\"";
            var html = "<p>\"Nubian (<em>kadīs</em>)\"</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        /// <summary>
        /// See the extra space in expected result? We will first convert the text token '"Nubian ', and then we'll look at the {{mention}}.
        /// While rendering the text token, we cannot predict if {{mention}} will render any text. This problem is somewhat similar with 
        /// <see cref="WikitextConverter.Cleanup"/> problem. 
        /// </summary>
        /// <remarks>Wiktionary itself does render '[Term?]' token for empty mentions, just as it does something similar in all other similar
        /// cases, but I don't think that's what we want.</remarks>
        [Fact]
        public void ShouldConvertEmptyMentions()
        {
            var creole = "\"Nubian {{m|onw}}\"";
            var html = "<p>\"Nubian \"</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertMentionsOnlyGloss()
        {
            var creole = "\"Nubian {{m|onw|gloss=kadis}}\"";
            var html = "<p>\"Nubian (&ldquo;kadis&rdquo;)\"</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertDerivedWithoutTranslation()
        {
            var creole = "from {{der|en|la|dictionarius}}, ";
            var html = "<p>from Latin <em>dictionarius</em>,</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertDerivedWithTranslation()
        {
            var creole = "from {{der|en|ang|frēo||free}}, ";
            var html = "<p>from Old English <em>frēo</em> (&ldquo;free&rdquo;),</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertMentionWithTranslation()
        {
            var creole = "from {{m|la|dictio||speaking}},";
            var html = "<p>from dictio (&ldquo;speaking&rdquo;),</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertMentionWithoutTranslation()
        {
            var creole = "from {{m|la|dictus}}";
            var html = "<p>from dictus</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertCog()
        {
            var creole = ", {{cog|da|-}}, {{cog|sv|-}} and {{cog|no|fri||free}}.";
            var html = "<p>, Danish, Swedish and Norwegian <em>fri</em> (&ldquo;free&rdquo;).</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertCogOnlyLanguage()
        {
            var creole = "Its pronominal use is of {{cog|gem}} origin.";
            var html = "<p>Its pronominal use is of Germanic origin.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
