using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextConverterTest
    {
        [Fact]
        public void ShouldProcessMultiline()
        {
            var creole = @"
A domesticated subspecies

Felis silvestris catus

Of feline animal";
            var html = "<p>\r\nA domesticated subspecies\r\n\r</p><p>Felis silvestris catus\r\n\r</p><p>Of feline animal</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldConvertEmphasis()
        {
            var creole = "A domesticated subspecies (''Felis silvestris catus'') of feline animal";
            var html = "<p>A domesticated subspecies (<em>Felis silvestris catus</em>) of feline animal</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldConvertLinks()
        {
            var creole = "A domesticated [[subspecies]] ([[Felis silvestris catus]]) of [[feline]] animal, commonly kept as a house [[pet]]";
            var html = "<p>A domesticated <a href=\"subspecies\">subspecies</a> (<a href=\"Felis silvestris catus\">Felis silvestris catus</a>) of <a href=\"feline\">feline</a> animal, commonly kept as a house <a href=\"pet\">pet</a></p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShoudConvertMacros()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. {{defdate|from 8th c.}}";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. <span class=\"defdate\">from 8th c.</span></p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldStripHtml()
        {
            var creole = "from 8<sup>th</sup>c.";
            var html = "<p>from 8thc.</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldProcessPlainStrings()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        // todo: paragraphs
    }
}
