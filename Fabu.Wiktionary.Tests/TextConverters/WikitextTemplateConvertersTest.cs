using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextTemplateConverterTest
    {
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>()
        {
            { "en", "English" },
            { "de", "German" },
            { "fy", "West Frisian" },
            { "la", "Latin" },
            { "ang", "Old English" },
            { "gml", "Middle Low German" },
            { "gmh", "Middle High German" },
            { "goh", "Old High German" },
            { "gem-pro", "Proto-Germanic" },
            { "ine-pro", "Proto-Indo-European" },
            { "nl", "Dutch" },
            { "osx", "Old Saxon" },
            { "da", "Danish" },
            { "sv", "Swedish" },
            { "no", "Norwegian" }
        };

        private string Convert(string creole, bool allowLinks = true)
        {
            var converter = new WikitextProcessor(_dictionary, allowLinks);
            return converter.ConvertToStructured(new ContextArguments() { PageTitle = "TEST", SectionName = "TEST" }, creole).ToHtml();
        }

        [Fact]
        public void ShouldConvertSimpleIPA()
        {
            var creole = "{{ipa|/boːk/|lang=li}}";
            var html = "<p>IPA: /boːk/</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMultiIPA()
        {
            var creole = "{{ipa|/boːk/|[boːk]|lang=li}}";
            var html = "<p>IPA: /boːk/, [boːk]</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertSimpleLabel()
        {
            var creole = "blabla {{lb|en|here we are}} albalb";
            var html = "<p>blabla (English, here we are) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertOrLabel()
        {
            var creole = "blabla {{lb|en|one|or|another}} albalb";
            var html = "<p>blabla (English, one or another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertUnderscoreLabel()
        {
            var creole = "blabla {{lb|en|one|_|another}} albalb";
            var html = "<p>blabla (English, one another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertAlsoLabel()
        {
            var creole = "blabla {{lb|en|one|also|another}} albalb";
            var html = "<p>blabla (English, one, also another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertAmpersandLabel()
        {
            var creole = "blabla {{lb|en|one|&|another}} albalb";
            var html = "<p>blabla (English, one and another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertSemicolonLabel()
        {
            var creole = "blabla {{lb|en|one|;|another}} albalb";
            var html = "<p>blabla (English, one; another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertExtremelyLabel()
        {
            var creole = "blabla {{lb|en|one|extremely|another}} albalb";
            var html = "<p>blabla (English, one, extremely another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertCategoryLabel()
        {
            var creole = "blabla {{lb|en|one|humorously|another}} albalb";
            var html = "<p>blabla (English, one, humorously another) albalb</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertInheritedWithoutTranslation()
        {
            var creole = "From {{inh|nds|gml|vrîe}}, variant";
            var html = "<p>From Middle Low German <em>vrîe</em>, variant</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertInheritedWithTranslation()
        {
            var creole = "From {{inh|de|gmh}}, {{inh|de|goh|frī}}, from {{inh|de|gem-pro|*frijaz}}, From {{inh|pdc|goh|frī}}";
            var html = "<p>From Middle High German, Old High German <em>frī</em>, from Proto-Germanic <em>*frijaz</em>, From Old High German <em>frī</em></p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMixedMentions() 
        {
            var creole = "From {{inh|nds|gml|vrîe}}, variant of {{m|gml|vrî}}, from {{der|nds|osx|frī}}, from {{der|nds|gem-pro|*frijaz}}, from {{der|nds|ine-pro|*prey||new}}. Compare Dutch {{m|nl|vrij}}, West Frisian {{m|fy|frij}}, English {{m|en|free}}, German {{m|de|frei}}.";
            var html = "<p>From Middle Low German <em>vrîe</em>, variant of vrî, from Old Saxon <em>frī</em>, from Proto-Germanic <em>*frijaz</em>, from Proto-Indo-European <em>*prey</em> (&ldquo;new&rdquo;). Compare Dutch vrij, West Frisian frij, English free, German frei.</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertDerivedWithoutTranslation()
        {
            var creole = "from {{der|en|la|dictionarius}}, ";
            var html = "<p>from Latin <em>dictionarius</em>,</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertDerivedWithTranslation()
        {
            var creole = "from {{der|en|ang|frēo||free}}, ";
            var html = "<p>from Old English <em>frēo</em> (&ldquo;free&rdquo;),</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMentionWithTranslation()
        {
            var creole = "from {{m|la|dictio||speaking}},";
            var html = "<p>from dictio (&ldquo;speaking&rdquo;),</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertMentionWithoutTranslation()
        {
            var creole = "from {{m|la|dictus}}";
            var html = "<p>from dictus</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertCog()
        {
            var creole = ", {{cog|da|-}}, {{cog|sv|-}} and {{cog|no|fri||free}}.";
            var html = "<p>, Danish, Swedish and Norwegian <em>fri</em> (&ldquo;free&rdquo;).</p>";
            Assert.Equal(html, Convert(creole));
        }
    }
}
