using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextConverterTest : TestConverterFactory
    {
        [Fact]
        public void Multiline()
        {
            var creole = @"
A domesticated subspecies

Felis silvestris catus

Of feline animal";
            var html = "<p>A domesticated subspecies</p><p>Felis silvestris catus</p><p>Of feline animal</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertEmphasis()
        {
            var creole = "A domesticated subspecies (''Felis silvestris catus'') of feline animal";
            var html = "<p>A domesticated subspecies (<em>Felis silvestris catus</em>) of feline animal</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertLinks()
        {
            var creole = "A domesticated [[subspecies]] ([[Felis silvestris catus]]) of [[feline]] animal, commonly kept as a house [[pet]]";
            var html = "<p>A domesticated <a href=\"subspecies\">subspecies</a> (<a href=\"Felis silvestris catus\">Felis silvestris catus</a>) of <a href=\"feline\">feline</a> animal, commonly kept as a house <a href=\"pet\">pet</a></p>";
            Assert.Equal(html, Convert(creole, true).ToHtml());
        }

        [Fact]
        public void Macros()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. {{defdate|from 8th c.}}";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. (from 8th c.)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Html()
        {
            var creole = "<hr />from 8<sup>th</sup>c.";
            var html = "<p>from 8<sup>th</sup>c.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void PlainStrings()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void UnorderedLists()
        {
            var creole = @"Blabla!
* first item
*: first sub item
* second item
** second sub item 1
** second sub item 2
* third item

Balala!";
            var html = "<p>Blabla!</p><ul><li><p> first item</p><p> first sub item</p></li><li><p> second item</p><p> second sub item 1</p><p> second sub item 2</p></li><li><p> third item</p></li></ul><p>Balala!</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void QuotesInUnorderedLists()
        {
            var creole = @"# {{form of|Form, used before a vowel sound,|a|lang=en}}
#* {{quote-book|year=1898|author={{w|Winston Churchill (novelist)|Winston Churchill}}
|title={{w|The Celebrity}}|chapter=2
|passage=Sunning himself on the board steps, I saw for the first time Mr. Farquhar Fenelon Cooke. He was dressed out in broad gaiters and bright tweeds, like '''an''' English tourist, and his face might have belonged to Dagon, idol of the Philistines.}}
# {{lb|en|rare|nonstandard}} {{form of|Form|a|lang=en}} ''used by a few speakers and writers before {{IPAchar|/h/|lang=en}}, especially if weakly pronounced.''";
            var html = "<ol><li><p>Form, used before a vowel sound, of <em>a</em></p><p>1898, Winston Churchill, chapter 2, in <em>The Celebrity</em>: &ldquo;Sunning himself on the board steps, I saw for the first time Mr. Farquhar Fenelon Cooke. He was dressed out in broad gaiters and bright tweeds, like <strong>an</strong> English tourist, and his face might have belonged to Dagon, idol of the Philistines.&rdquo;</p></li><li><p>(rare, nonstandard) Form of <em>a</em> <em>used by a few speakers and writers before /h/, especially if weakly pronounced.</em></p></li></ol>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void EmptyLinebreak()
        {
            var creole = "\r\n";
            var html = "";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        // todo: paragraphs
    }
}
