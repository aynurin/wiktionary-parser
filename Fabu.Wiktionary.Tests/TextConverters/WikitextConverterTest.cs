using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System.Collections.Generic;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextConverterTest
    {
        private string Convert(string creole, bool allowLinks = false)
        {
            var converter = new WikitextProcessor(new Dictionary<string, string>()
                {
                    { "en", "English" }
                }, allowLinks);
            return converter.ConvertToStructured(new ContextArguments() { PageTitle = "TEST", SectionName = "TEST" }, creole).ToHtml();
        }

        [Fact]
        public void ShouldProcessMultiline()
        {
            var creole = @"
A domesticated subspecies

Felis silvestris catus

Of feline animal";
            var html = "<p>\r\nA domesticated subspecies\r\n\r</p><p>Felis silvestris catus\r\n\r</p><p>Of feline animal</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShouldConvertEmphasis()
        {
            var creole = "A domesticated subspecies (''Felis silvestris catus'') of feline animal";
            var html = "<p>A domesticated subspecies (<em>Felis silvestris catus</em>) of feline animal</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShouldConvertLinks()
        {
            var creole = "A domesticated [[subspecies]] ([[Felis silvestris catus]]) of [[feline]] animal, commonly kept as a house [[pet]]";
            var html = "<p>A domesticated <a href=\"subspecies\">subspecies</a> (<a href=\"Felis silvestris catus\">Felis silvestris catus</a>) of <a href=\"feline\">feline</a> animal, commonly kept as a house <a href=\"pet\">pet</a></p>";
            var result = Convert(creole, true);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShoudConvertMacros()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. {{defdate|from 8th c.}}";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. (from 8th c.)</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShouldProcessHtml()
        {
            var creole = "<hr />from 8<sup>th</sup>c.";
            var html = "<p>from 8<sup>th</sup>c.</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShouldProcessPlainStrings()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        [Fact]
        public void ShouldProcessUnorderedLists()
        {
            var creole = @"Blabla!
* first item
* second item
* third item

Balala!";
            var html = "<p>Blabla!\r</p><ul><li> first item\r</li><li> second item\r</li><li> third item\r</li></ul><p>\r\nBalala!</p>";
            var result = Convert(creole);
            Assert.Equal(html, result);
        }

        // todo: paragraphs
    }
}
