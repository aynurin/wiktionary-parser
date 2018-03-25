using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class CreoleConverterTest
    {
        [Fact]
        public void ShouldConvertEmphasis()
        {
            var creole = "A domesticated subspecies (''Felis silvestris catus'') of feline animal";
            var html = "A domesticated subspecies (<em>Felis silvestris catus</em>) of feline animal";
            var converter = new CreoleConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldConvertLinks()
        {
            var creole = "A domesticated [[subspecies]] ([[Felis silvestris catus]]) of [[feline]] animal, commonly kept as a house [[pet]]";
            var html = "A domesticated <a href=\"subspecies\">subspecies</a> (<a href=\"Felis silvestris catus\">Felis silvestris catus</a>) of <a href=\"feline\">feline</a> animal, commonly kept as a house <a href=\"pet\">pet</a>";
            var converter = new CreoleConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShoudConvertMacros()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. {{defdate|from 8th c.}}";
            var html = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. <span class=\"defdate\">from 8th c.</span>";
            var converter = new CreoleConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        [Fact]
        public void ShouldStripHtml()
        {
            var creole = "from 8<sup>th</sup>c.";
            var html = "from 8thc.";
            var converter = new CreoleConverter();
            var result = converter.ConvertToStructured(creole);
            Assert.Equal(html, result.ToHtml());
        }

        // todo: paragraphs
    }
}
